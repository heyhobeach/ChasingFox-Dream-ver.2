using System;
using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using Com.LuisPedroFonseca.ProCamera2D;
using Unity.VisualScripting;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    public SoundManager soundManager;

    private static GameManager instance;
    public static GameManager Instance { get => instance; }

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

    public BrutalDatas brutalDatas;
    public HumanDatas humanDatas;

    private const int karma = 100;
    [SerializeField] [Range(0, 100)] private int karmaRatio = 65;

    public static int Humanity { get => instance.karmaRatio; set { instance.karmaRatio = value; instance.ClampRatio(); } }
    public static int Brutality { get => karma-instance.karmaRatio; set { instance.karmaRatio = -value; instance.ClampRatio(); } }
    private int ClampRatio() => Mathf.Clamp(karmaRatio, 0, 100);

    public static int GetHumanData() => instance.humanDatas.counts[Humanity/10];
    public static BrutalData GetBrutalData() => instance.brutalDatas.brutalDatas[Brutality/10];

    public Player player { get; private set; }
    
    public List<Map> maps = new List<Map>();

    public void TimeScale(float t) => Time.timeScale = t;

    private void OnDestroy()
    {
        BehaviourNode.clone.Clear();
        StopAllCoroutines();
        CameraManager.Instance.proCamera2DRooms.OnStartedTransition.RemoveListener(CreateWallRoom);
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        if(controllers == null) controllers = new();
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        StartCoroutine(MapSearchStart());
        CameraManager.Instance.proCamera2DRooms.OnStartedTransition.AddListener(CreateWallRoom);
    }

    private void Update()
    {
        if(controllers.Count > 0) controllers.Peek().Controller();
        if(Input.GetKeyDown(KeyCode.Alpha0)) TimeScale(1);
        if(Input.GetKeyDown(KeyCode.Alpha1)) TimeScale(0);
        if(Input.GetKeyDown(KeyCode.Alpha2)) TimeScale(0.2f);
    }

    public void CreateWallRoom(int currentRoomIndex, int previousRoomIndex)
    {
        Rect rect;
        GameObject go;

        rect = CameraManager.Instance.proCamera2DRooms.Rooms[currentRoomIndex].Dimensions;
        var bl = new Vector2(rect.x-(rect.width*0.5f), rect.y-(rect.height*0.5f));
        var tr = new Vector2(rect.x+(rect.width*0.5f), rect.y+(rect.height*0.5f));

        go = new GameObject(){
            name = "wall",
            layer = LayerMask.NameToLayer("Map"),
            tag = "Map"
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

        bottomLeft = new Vector2Int((int)(rect.x-(rect.width*0.5f)), (int)(rect.y-(rect.height*0.5f)));
        topRight = new Vector2Int((int)(rect.x+(rect.width*0.5f)), (int)(rect.y+(rect.height*0.5f)));
        isLoad = true;

        maps[currentRoomIndex].OnStart();
        if(previousRoomIndex >= 0) maps[previousRoomIndex].OnEnd();
    }
}
