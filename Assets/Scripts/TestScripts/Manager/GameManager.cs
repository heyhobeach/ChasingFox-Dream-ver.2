using System;
using System.Collections.Generic;
using BehaviourTree;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;



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
            ApplySaveData();
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

    [DisableInInspector] public List<Map> maps = new List<Map>();
    [DisableInInspector] public List<EventTrigger> eventTriggers = new List<EventTrigger>();

    private Coroutine mapsearchCoroutine;

    private float _ingameTimescale = 1f;
    public float ingameTimescale
    {
        get => _ingameTimescale;
        set =>_ingameTimescale = value;
    }
    public void SetIngameTimescale(float value) => ServiceLocator.Get<GameManager>().ingameTimescale = value;
    public void SetSystemTimescale(float value) => Time.timeScale = value;
    public float ingameDeltaTime { get => Time.deltaTime * ingameTimescale; }

    public bool isPaused { get; private set; }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
        ServiceLocator.Unregister(this);
        BehaviourNode.clone.Clear();
        StopAllCoroutines();
        mapsearchCoroutine = null;
        if (isPaused) Pause();
        ServiceLocator.Get<CameraManager>().proCamera2DRooms.OnStartedTransition.RemoveListener(MoveNextRoom);
    }

    private void Awake()
    {
        ServiceLocator.Register<GameManager>(this);
        PageManger.Instance.aoComplatedAction += Init;

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

    private void Init()
    {
        Debug.Log("GameManager Init called");
        Time.fixedDeltaTime = 0.02f;
        var saveData = SystemManager.Instance.saveData;
        Scene currentActiveScene = SceneManager.GetActiveScene();

        maps = maps.OrderBy(x => x.transform.GetSiblingIndex()).ToList();
        eventTriggers = eventTriggers.OrderBy(x => x.transform.GetSiblingIndex()).ToList();

        if(mapsearchCoroutine == null) mapsearchCoroutine = StartCoroutine(MapSearchStart());

        var allPlayerObjs = FindObjectsByType<Player>(FindObjectsSortMode.None);
        player = allPlayerObjs.FirstOrDefault(p => p.gameObject.scene == currentActiveScene);
        if (player == null)
        {
            Debug.LogError("Active scene does not contain a Player object!");
            return;
        }

        var allInteractionEvents = FindObjectsByType<InteractionEvent>(FindObjectsSortMode.None);
        interactionEvent = allInteractionEvents.FirstOrDefault(e => e.gameObject.scene == currentActiveScene);
        if (interactionEvent == null) Debug.LogWarning("Active scene does not contain an InteractionEvent object!");

        var playerScript = player.GetComponent<Player>();
        var playerControllerScript = player.GetComponent<PlayerController>();

        karmaRatio = saveData.playerData.karma;
        playerScript.Init(saveData.playerData);
        playerControllerScript.Init(saveData.playerData);

        for (int i=0; i<maps.Count; i++) CreateWallRoom(i).enabled = false;
        if(maps[PlayerData.lastRoomIdx].used) 
        {
            player.transform.position = maps[PlayerData.lastRoomIdx].position;
            karmaRatio = maps[PlayerData.lastRoomIdx].playerData.karma;
            playerScript.Init(maps[PlayerData.lastRoomIdx].playerData);
            playerControllerScript.Init(maps[PlayerData.lastRoomIdx].playerData);
        }
        for(int i=0; i<eventTriggers.Count; i++)
        {
            if(EventTriggerData.currentEventTriggerData != null 
                && EventTriggerData.currentEventTriggerData.guid.Equals(eventTriggers[i].eventTriggerData.guid))
            {
                // player.transform.position = EventTriggerData.currentEventTriggerData.targetPosition;
                eventTriggers[i].OnTrigger();
            }
            eventTriggers[i].GetComponent<BoxCollider2D>().enabled = true;
        } 

        ProCamera2D.Instance.CameraTargets.Add(new CameraTarget(){ TargetTransform = player.transform });
        ProCamera2D.Instance.MoveCameraInstantlyToPosition(player.transform.position);
        ServiceLocator.Get<CameraManager>().proCamera2DRooms.OnStartedTransition.AddListener(MoveNextRoom);
        MoveNextRoom(PlayerData.lastRoomIdx, -1);
    }

    public void ApplySaveData()
    {
        var saveData = SystemManager.Instance.saveData;
        if (saveData == null) return;

        Debug.Log("ApplySaveData called");

        Scene currentActiveScene = SceneManager.GetActiveScene();
        var allPlayerObjs = FindObjectsByType<Player>(FindObjectsSortMode.None);
        player = allPlayerObjs.FirstOrDefault(p => p.gameObject.scene == currentActiveScene);

        if(saveData.chapter == currentActiveScene.name)
        {
            PlayerData.lastRoomIdx = saveData.stageIdx;

            InventoryManager.Instance.invendata.inventory?.Clear();
            foreach(var item in SystemManager.Instance.saveData.inventoryData) 
                InventoryManager.Instance.inventory?.AddInventory(item);

            for(int i=0; i<maps.Count; i++) maps[i].Init(saveData.mapData[i]);

            for(int i=0; i<eventTriggers.Count; i++) 
            {
                eventTriggers[i].Init(saveData.eventTriggerData[i]);
                if(!string.IsNullOrEmpty(saveData.currentEventTriggerDataGuid)
                    && eventTriggers[i].eventTriggerData.guid.Equals(saveData.currentEventTriggerDataGuid)) 
                        EventTriggerData.currentEventTriggerData = eventTriggers[i].eventTriggerData;
            }
        }
        else SaveData();
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

        rect = ServiceLocator.Get<CameraManager>().proCamera2DRooms.Rooms[currentRoomIndex].Dimensions;
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

    public bool LoadScene(string name, bool active = true)
    {
        if (SceneManager.GetActiveScene().name != name) 
        {
            DataReset();
            SaveData();
        }
        ServiceLocator.Get<UIController>()?.DialogueCanvasSetFalse();
        return PageManger.Instance.LoadScene(name, active);
    }
    public bool RetryScene() => LoadScene(SceneManager.GetActiveScene().name, false);

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
        DataReset();
        PageManger.Instance.Quit();
    }

    public void SaveData()
    {
        var mapDatas = new MapData.JsonData[maps.Count];
        for (int i = 0; i < maps.Count; i++) mapDatas[i] = maps[i].mapData;

        var eventTriggerDatas = new EventTriggerData.JsonData[eventTriggers.Count];
        for (int i = 0; i < eventTriggers.Count; i++) eventTriggerDatas[i] = eventTriggers[i].eventTriggerData;

        var items = InventoryManager.Instance.invendata.inventory?.Values.ToArray();

        var playerData = player.GetComponent<Player>().GetJsonData();
        playerData.pcm = player.GetComponent<PlayerController>().DataSet();

        SystemManager.Instance.saveData = new SaveData(){
            chapter = SceneManager.GetActiveScene().name,
            stageIdx = PlayerData.lastRoomIdx,
            currentEventTriggerDataGuid = EventTriggerData.currentEventTriggerData?.guid,
            inventoryData = items,
            playerData = playerData,
            mapData = mapDatas,
            eventTriggerData = eventTriggerDatas
        };

        SystemManager.Instance.SaveData(SystemManager.Instance.saveIndex);
    }

    private void DataReset()
    {
        foreach (var map in maps) map.DataReset();
        foreach (var eventTrigger in eventTriggers) eventTrigger.DataReset();
        SystemManager.Instance.saveData.chapter = SceneManager.GetActiveScene().name;
        PlayerData.lastRoomIdx = 0;
        EventTriggerData.currentEventTriggerData = null;
    }
    public void InventoryEnable()
    {
        if(inventoryCanvas != null) inventoryCanvas.SetActive(true);
    }

    public void InventoryDisable()
    {
        if(inventoryCanvas != null) inventoryCanvas.SetActive(false);
    }
}
