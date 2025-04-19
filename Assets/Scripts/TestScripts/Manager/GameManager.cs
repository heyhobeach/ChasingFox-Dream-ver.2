using System;
using System.Collections.Generic;
using BehaviourTree;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

public delegate void EnemyDeathDel(EnemyUnit enemyUnit);
public delegate void GunsoundDel(Transform transform, Vector2 pos, Vector2 size);

[RequireComponent(typeof(ControllerManager))]
public partial class GameManager : MonoBehaviour
{

#if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
    {
        if(playModeStateChange == PlayModeStateChange.EnteredPlayMode) 
        {
            Init();
            PageManger.Instance.aoComplatedAction -= Init;
        }
    }
#endif

    public InteractionEvent interactionEvent;

    public GameObject inventoryCanvas;

    private event EnemyDeathDel onEnemyDeath;
    private event GunsoundDel onGunsound;

    public void AddEnemyDeath(EnemyDeathDel del) => onEnemyDeath += del;
    public void AddGunsound(GunsoundDel del) => onGunsound += del;
    public void DelEnemyDeath(EnemyDeathDel del) => onEnemyDeath -= del;
    public void DelGunsound(GunsoundDel del) => onGunsound -= del;

    public void OnEnemyDeath(EnemyUnit enemyUnit) => onEnemyDeath?.Invoke(enemyUnit);
    public void OnGunsound(Transform transform, Vector2 pos, Vector2 size) => onGunsound?.Invoke(transform, pos, size);

    public BrutalDatas brutalDatas;
    public HumanDatas humanDatas;

    private const int karma = 100;
    [SerializeField][Range(0, 100)] private int karmaRatio = 65;

    public static int Humanity { get => ServiceLocator.Get<GameManager>().karmaRatio; set { ServiceLocator.Get<GameManager>().karmaRatio = value; ServiceLocator.Get<GameManager>().ClampRatio(); } }
    public static int Brutality { get => karma - ServiceLocator.Get<GameManager>().karmaRatio; set { ServiceLocator.Get<GameManager>().karmaRatio = -value; ServiceLocator.Get<GameManager>().ClampRatio(); } }
    private int ClampRatio() => Mathf.Clamp(karmaRatio, 0, 100);

    public static int GetHumanData() => ServiceLocator.Get<GameManager>().humanDatas.counts[Humanity / 10];
    public static BrutalData GetBrutalData() => ServiceLocator.Get<GameManager>().brutalDatas.brutalDatas[Brutality / 10];

    public Player player;

    public List<Map> maps = new List<Map>();
    public List<EventTrigger> eventTriggers = new List<EventTrigger>();

    private Coroutine mapsearchCoroutine;

    public int targetFrame = 60;
    private float deltaTime = 0f;
    public static float fps { get; private set; }

    public bool isPaused { get; private set; }

    public void TimeScale(float t) => Time.timeScale = t;

    private void OnDestroy()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
        ServiceLocator.Unregister(this);
        BehaviourNode.clone.Clear();
        StopAllCoroutines();
        mapsearchCoroutine = null;
        CameraManager.Instance?.proCamera2DRooms.OnStartedTransition.RemoveListener(MoveNextRoom);
        if (isPaused) Pause();
    }

    private void OnApplicationQuit() => SaveData();

    private void Awake()
    {
        ServiceLocator.Register<GameManager>(this);
        PageManger.Instance.aoComplatedAction += Init;

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif

        Application.targetFrameRate = targetFrame;
    }

    private void Init()
    {
        var saveData = SystemManager.Instance.saveData;

        if(mapsearchCoroutine == null) mapsearchCoroutine = StartCoroutine(MapSearchStart());
        if(saveData != null && saveData.chapter != SceneManager.GetActiveScene().name) DataReset();

        player = FindFirstObjectByType<Player>();
        interactionEvent = FindFirstObjectByType<InteractionEvent>();

        var playerScript = player.GetComponent<Player>();
        var playerControllerScript = player.GetComponent<PlayerController>();
        playerScript.Init(saveData.playerData);
        playerControllerScript.Init(saveData.playerData);
        karmaRatio = saveData.karma;
        PlayerData.lastRoomIdx = saveData.chapterIdx;

        for (int i = 0; i < maps.Count; i++) CreateWallRoom(i).enabled = false;
        if(saveData.mapDatas == null || saveData.mapDatas.Length == 0)
        {
            var mapDatas = new MapData.JsonData[maps.Count];
            for (int i = 0; i < maps.Count; i++) mapDatas[i] = maps[i].mapData;
            saveData.mapDatas = mapDatas;
        }
        for (int i = 0; i < maps.Count; i++) maps[i].Init(saveData.mapDatas[i]);

        if(saveData.eventTriggerDatas == null || saveData.eventTriggerDatas.Length == 0)
        {
            var eventTriggerDatas = new EventTriggerData.JsonData[eventTriggers.Count];
            for (int i = 0; i < eventTriggers.Count; i++) 
            {
                eventTriggerDatas[i] = eventTriggers[i].eventTriggerData;
            }
            saveData.eventTriggerDatas = eventTriggerDatas;
        }
        for (int i = 0; i < eventTriggers.Count; i++) 
        {
            eventTriggers[i].Init(saveData.eventTriggerDatas[i]);
        }

        SystemManager.Instance.saveData = saveData;

        if (!string.IsNullOrEmpty(saveData.eventTriggerInstanceID))
        {
            var trigger = eventTriggers.Find(x => x.eventTriggerData.guid.Equals(saveData.eventTriggerInstanceID));
            if (trigger) 
            {
                trigger.OnTrigger(saveData.eventIdx);
                player.transform.position = trigger.targetPosition;
            }
        }

        ProCamera2D.Instance.CameraTargets.Add(new CameraTarget(){ TargetTransform = player.transform });
        ProCamera2D.Instance.MoveCameraInstantlyToPosition(player.transform.position);
        CameraManager.Instance.proCamera2DRooms.OnStartedTransition.AddListener(MoveNextRoom);
        MoveNextRoom(PlayerData.lastRoomIdx, -1);
    }

    private void Update()
    {
        deltaTime += Time.unscaledDeltaTime - deltaTime;
    }
    private void FixedUpdate()
    {
        fps = 1.0f / deltaTime;
        // if(fps < 120) Time.fixedDeltaTime = 0.02f / (120 / (int)fps);
        // else Time.fixedDeltaTime = 0.02f;
    }

    public void MoveNextRoom(int currentRoomIndex, int previousRoomIndex)
    {
        CreateWallRoom(currentRoomIndex);
        isLoad = true;

        if(!maps[currentRoomIndex].used) PlayerData.lastRoomIdx = currentRoomIndex;
        if (!maps[currentRoomIndex].cleared) maps[currentRoomIndex].OnStart(player.transform.position);
        if (previousRoomIndex >= 0 && previousRoomIndex != currentRoomIndex) maps[previousRoomIndex].OnEnd();
    }
    public EdgeCollider2D CreateWallRoom(int currentRoomIndex)
    {
        Rect rect;
        GameObject go;

        rect = CameraManager.Instance.proCamera2DRooms.Rooms[currentRoomIndex].Dimensions;
        var bl = new Vector2(rect.x - (rect.width * 0.5f), rect.y - (rect.height * 0.5f));
        var tr = new Vector2(rect.x + (rect.width * 0.5f), rect.y + (rect.height * 0.5f));

        if (!maps[currentRoomIndex].edgeCollider2D)
        {
            go = new GameObject() {
                name = "wall",
                layer = LayerMask.NameToLayer("Wall"),
                tag = "Wall"
            };
            var edge = go.AddComponent<EdgeCollider2D>();
            edge.SetPoints(new List<Vector2>{
                new Vector2(bl.x, bl.y),
                new Vector2(bl.x, tr.y),
                new Vector2(tr.x, tr.y),
                new Vector2(tr.x, bl.y),
                new Vector2(bl.x, bl.y)
            });

            maps[currentRoomIndex].edgeCollider2D = edge;
        }

        bottomLeft = new Vector2Int(Mathf.RoundToInt(rect.x - (rect.width * 0.5f)), Mathf.RoundToInt(rect.y - (rect.height * 0.5f) + 1));
        topRight = new Vector2Int(Mathf.RoundToInt(rect.x + (rect.width * 0.5f) - 1), Mathf.RoundToInt(rect.y + (rect.height * 0.5f)));

        return maps[currentRoomIndex].edgeCollider2D;
    }

    public void LoadScene(string name, bool active = true)
    {
        ServiceLocator.Get<UIController>().DialogueCanvasSetFalse();
        SaveData();
        PageManger.Instance.LoadScene(name, active);
    }
    public void RetryScene() => LoadScene(SceneManager.GetActiveScene().name, false);

    public void Pause()
    {
        Pause(!isPaused);
        InventoryDisable();
    }
    public void Pause(bool isPause)
    {
        this.isPaused = isPause;
        Time.timeScale = isPause ? 0 : 1;
        PopupManager.Instance.PausePop(isPause);
    }

    public void Quit()
    {
        SaveData();
        PageManger.Instance.Quit();
    }

    private void SaveData()
    {
        var mapDatas = new MapData.JsonData[maps.Count];
        for (int i = 0; i < maps.Count; i++) mapDatas[i] = maps[i].mapData;
        var eventTriggerDatas = new EventTriggerData.JsonData[eventTriggers.Count];
        for (int i = 0; i < eventTriggers.Count; i++) eventTriggerDatas[i] = eventTriggers[i].eventTriggerData;
        SystemManager.Instance.saveData.mapDatas = mapDatas;
        SystemManager.Instance.saveData.eventTriggerDatas = eventTriggerDatas;
        SystemManager.Instance.saveData.karma = karmaRatio;
        SystemManager.Instance.saveData.chapterIdx = PlayerData.lastRoomIdx;
        SystemManager.Instance.SaveData(SystemManager.Instance.saveIndex);
    }

    private void DataReset()
    {
        SystemManager.Instance.saveData.mapDatas = null;
        SystemManager.Instance.saveData.eventTriggerDatas = null;
        SystemManager.Instance.saveData.chapter = SceneManager.GetActiveScene().name;
        SystemManager.Instance.saveData.karma = karmaRatio;
        SystemManager.Instance.saveData.playerData = null;
        PlayerData.lastRoomIdx = 0;
        SystemManager.Instance.saveData.chapterIdx = 0;
        SystemManager.Instance.UpdateDataForEventTrigger(null, 0);
    }
    public void InventoryEnable()
    {
        inventoryCanvas?.SetActive(true);
    }

    public void InventoryDisable()
    {
        inventoryCanvas?.SetActive(false);
    }
}
