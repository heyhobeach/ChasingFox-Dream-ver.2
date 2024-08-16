using System;
using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
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
        instance.controllers.Pop();
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

    public void TimeScale(float t) => Time.timeScale = t;

    private void OnDestroy()
    {
        BehaviourNode.clone.Clear();
        StopAllCoroutines();
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
    }

    private void Update()
    {
        if(controllers.Count > 0) controllers.Peek().Controller();
    }
}
