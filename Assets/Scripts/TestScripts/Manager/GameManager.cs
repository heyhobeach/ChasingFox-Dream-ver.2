using System;
using System.Collections.Generic;
using BehaviourTree;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void EnemyDeathDel(EnemyUnit enemyUnit);
public delegate void GunsoundDel(Transform transform, Vector2 pos, Vector2 size);

[RequireComponent(typeof(ControllerManager))]
public partial class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get => instance; }

    public SoundManager soundManager;
    private PopupManager popupManager;
    private ControllerManager controllerManager;
    public InteractionEvent interactionEvent;

    private EnemyDeathDel onEnemyDeath;
    private GunsoundDel onGunsound;

    public InventoryManager inventoryManager;

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

    public static int Humanity { get => instance.karmaRatio; set { instance.karmaRatio = value; instance.ClampRatio(); } }
    public static int Brutality { get => karma - instance.karmaRatio; set { instance.karmaRatio = -value; instance.ClampRatio(); } }
    private int ClampRatio() => Mathf.Clamp(karmaRatio, 0, 100);

    public static int GetHumanData() => instance.humanDatas.counts[Humanity / 10];
    public static BrutalData GetBrutalData() => instance.brutalDatas.brutalDatas[Brutality / 10];

    public Player player;

    public List<Map> maps = new List<Map>();
    public List<EventTrigger> eventTriggers = new List<EventTrigger>();

    public int targetFrame = 60;
    private float deltaTime = 0f;
    public static float fps { get; private set; }

    public bool isPaused { get; private set; }

    private bool isClear;

    public void TimeScale(float t) => Time.timeScale = t;

    private void OnDestroy()
    {
        instance = null;
        if(!isClear) SaveData();
        if (isPaused) Pause();
        maps.Clear();
        eventTriggers.Clear();
        BehaviourNode.clone.Clear();
        StopAllCoroutines();
        CameraManager.Instance?.proCamera2DRooms.OnStartedTransition.RemoveListener(MoveNextRoom);
    }

    private void Awake()
    {
        Application.targetFrameRate = targetFrame;
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        player = FindObjectOfType<Player>();
        interactionEvent = FindObjectOfType<InteractionEvent>();
        popupManager = PopupManager.Instance;
        controllerManager = ControllerManager.Instance;
    }

    private void Start()
    {
        StartCoroutine(MapSearchStart());
        karmaRatio = SystemManager.Instance.saveData.karma;
        if (maps[PlayerData.lastRoomIdx].used)
        {
            player.GetComponent<Player>().Init(maps[PlayerData.lastRoomIdx].playerData);
            player.transform.position = maps[PlayerData.lastRoomIdx].position;
            ProCamera2D.Instance.MoveCameraInstantlyToPosition(player.transform.position);
        }
        else player.GetComponent<Player>().Init();
        for (int i = 0; i < maps.Count; i++) CreateWallRoom(i).enabled = false;
        CameraManager.Instance?.proCamera2DRooms.OnStartedTransition.AddListener(MoveNextRoom);
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

        if (!maps[currentRoomIndex].used) PlayerData.lastRoomIdx = currentRoomIndex;
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
                // layer = LayerMask.NameToLayer("Map"),
                // tag = "Map"
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

        bottomLeft = new Vector2Int(Mathf.RoundToInt(rect.x - (rect.width * 0.5f) - 2), Mathf.RoundToInt(rect.y - (rect.height * 0.5f) - 2));
        topRight = new Vector2Int(Mathf.RoundToInt(rect.x + (rect.width * 0.5f) + 2), Mathf.RoundToInt(rect.y + (rect.height * 0.5f) + 2));

        return maps[currentRoomIndex].edgeCollider2D;
    }

    public void ResetScene(string nextScene)
    {
        foreach (var map in maps) map.Reset();
        foreach (var trigger in eventTriggers) trigger.used = false;
        SystemManager.Instance.saveData.mapDatas = null;
        SystemManager.Instance.saveData.eventTriggerDatas = null;
        SystemManager.Instance.saveData.chapter = nextScene;
        SystemManager.Instance.saveData.karma = karmaRatio;
        PlayerData.lastRoomIdx = 0;
        SaveData();
    }

    public void LoadScene(string name)
    {
        UIController.Instance.DialogueCanvasSetFalse();
        if(isClear) Instance.ResetScene(name);
        else SaveData();
        PageManger.Instance.LoadScene(name);
    }
    public void RetryScene() {
        PageManger.Instance.LoadScene(SceneManager.GetActiveScene().name);
        UIController.Instance.DialogueCanvasSetFalse();
    }
    public void Pause()
    {
        Pause(!isPaused);
    }

    public void Pause(bool isPause)
    {
        this.isPaused = isPause;
        Time.timeScale = isPause ? 0 : 1;
        PopupManager.Instance.PausePop(isPause);
    }

    public void Clear(bool isclear) => this.isClear = isclear;

    public void Quit() => PageManger.Instance.Quit();

    private void SaveData()
    {
        Debug.Log("SaveData");
        if(SystemManager.Instance.saveData.mapDatas == null)
        {
            var mapDatas = new MapData[maps.Count];
            for (int i = 0; i < maps.Count; i++) mapDatas[i] = maps[i].mapData;
        }
        if(SystemManager.Instance.saveData.eventTriggerDatas == null)
        {
            var eventTriggerDatas = new EventTriggerData[eventTriggers.Count];
            for (int i = 0; i < eventTriggers.Count; i++) eventTriggerDatas[i] = eventTriggers[i].eventTriggerData;
        }
        SystemManager.Instance.saveData.chapterIdx = PlayerData.lastRoomIdx;
        SystemManager.Instance.SaveData(SystemManager.Instance.saveIndex);
    }
}
