using System;
using System.Collections.Generic;
using BehaviourTree;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get => instance; }

    public SoundManager soundManager;
    private PopupManager popupManager;
    public InteractionEvent interactionEvent;

    private Stack<IBaseController> controllers = new();
    public static void PushController(IBaseController @base)
    {
        if(instance.controllers.Contains(@base)) return;
        instance.controllers.Push(@base);
    }
    public static void PopController(IBaseController @base)
    {
        if(instance.controllers.Count > 0 && instance.controllers.Peek() != @base)
        {
            Stack<IBaseController> temp = new();
            while(instance.controllers.Count > 0 && !temp.Equals(instance.controllers.Peek())) temp.Push(instance.controllers.Pop());
            while(temp.Count > 0) instance.controllers.Push(temp.Pop());
        }
        if(instance.controllers.Count > 0) instance.controllers.Pop();
    }
    public static IBaseController GetTopController() => instance.controllers.Peek();

    public BrutalDatas brutalDatas;
    public HumanDatas humanDatas;

    private const int karma = 100;
    [SerializeField] [Range(0, 100)] private int karmaRatio = 65;

    public static int Humanity { get => instance.karmaRatio; set { instance.karmaRatio = value; instance.ClampRatio(); } }
    public static int Brutality { get => karma-instance.karmaRatio; set { instance.karmaRatio = -value; instance.ClampRatio(); } }
    private int ClampRatio() => Mathf.Clamp(karmaRatio, 0, 100);

    public static int GetHumanData() => instance.humanDatas.counts[Humanity/10];
    public static BrutalData GetBrutalData() => instance.brutalDatas.brutalDatas[Brutality/10];

    public Player player;
    
    public List<Map> maps = new List<Map>();
    public List<EventTrigger> eventTriggers = new List<EventTrigger>();

    public int targetFrame = 60;
    private float deltaTime = 0f;
    public static float fps { get; private set; }

    public bool isPaused  { get; private set; }

    public void TimeScale(float t) => Time.timeScale = t;

    private void OnDestroy()
    {
        instance = null;
        if(isPaused) Pause();
        maps.Clear();
        eventTriggers.Clear();
        BehaviourNode.clone.Clear();
        StopAllCoroutines();
        CameraManager.Instance.proCamera2DRooms.OnStartedTransition.RemoveListener(MoveNextRoom);
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
        if(controllers == null) controllers = new();
        player = FindObjectOfType<Player>();
        interactionEvent = FindObjectOfType<InteractionEvent>();
        popupManager = PopupManager.Instance;
    }

    private void Start()
    {
        StartCoroutine(MapSearchStart());
        if(maps[PlayerData.lastRoomIdx].used) 
        {
            player.GetComponent<Player>().Init(maps[PlayerData.lastRoomIdx].playerData);
            player.transform.position = maps[PlayerData.lastRoomIdx].position;
            ProCamera2D.Instance.MoveCameraInstantlyToPosition(player.transform.position);
        }
        else player.GetComponent<Player>().Init();
        for(int i=0; i<maps.Count; i++) CreateWallRoom(i).enabled = false;
        CameraManager.Instance.proCamera2DRooms.OnStartedTransition.AddListener(MoveNextRoom);
    }

    private void Update()
    {
        if(controllers.Count > 0) controllers.Peek().Controller();
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
        if(!maps[currentRoomIndex].cleared) maps[currentRoomIndex].OnStart(player.transform.position);
        if(previousRoomIndex >= 0 && previousRoomIndex != currentRoomIndex) maps[previousRoomIndex].OnEnd();
    }
    public EdgeCollider2D CreateWallRoom(int currentRoomIndex)
    {
        Rect rect;
        GameObject go;

        rect = CameraManager.Instance.proCamera2DRooms.Rooms[currentRoomIndex].Dimensions;
        var bl = new Vector2(rect.x-(rect.width*0.5f), rect.y-(rect.height*0.5f));
        var tr = new Vector2(rect.x+(rect.width*0.5f), rect.y+(rect.height*0.5f));

        if(!maps[currentRoomIndex].edgeCollider2D)
        {
            go = new GameObject(){
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

        bottomLeft = new Vector2Int(Mathf.RoundToInt(rect.x-(rect.width*0.5f)-2), Mathf.RoundToInt(rect.y-(rect.height*0.5f)-2));
        topRight = new Vector2Int(Mathf.RoundToInt(rect.x+(rect.width*0.5f)+2), Mathf.RoundToInt(rect.y+(rect.height*0.5f)+2));

        return maps[currentRoomIndex].edgeCollider2D;
    }

    public void ResetScene()
    {
        foreach(var map in maps) map.Reset();
        foreach(var trigger in eventTriggers) trigger.used = false;
        PlayerData.lastRoomIdx = 0;
    }

    public void LoadScene(string name) => PageManger.Instance.LoadScene(name);
    public void RetryScene() => PageManger.Instance.LoadScene(SceneManager.GetActiveScene().name);
    public void Pause()
    {
        // TODO : Pause UI 추가 및 기타 기능 구현
        if(isPaused)
        {
            Time.timeScale = 1;
            PopupManager.Instance.PausePop(false);
            isPaused = false;
        }
        else 
        {
            Time.timeScale = 0;
            PopupManager.Instance.PausePop(true);
            isPaused = true;
        }
    }
}
