using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PageManger : MonoBehaviour
{
    // Start is called before the first frame update
    //���� �ʱ�ȭ�� �� �ص� �ɰ� ������ �Ʒ��� ���� �ʱ�ȭ ���
    //private static Lazy<SceneManger> instance = new Lazy<SceneManger>(() => new SceneManger());
    //public static SceneManger Instance { get { return instance.Value; } }   

    //������ : ������ ���� ������� �ذ��ؼ� �̱��� ����� ���ϼ� ������?

    private static PageManger instance;
    public static PageManger Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject() { name = "PageManager" };
                instance = obj.AddComponent<PageManger>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private Scene currentScene;
    private string prevSceneName = "";
    private string newSceneName = "";
    public int formIdx = -1;
    public PlayerController.PlayerControllerMask playerControllerMask;

    private List<MapData> clearedMaps = new();
    private List<EventTriggerData> clearedTriggers = new();

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(Instance);
        SceneManager.activeSceneChanged += (prv, ne)=> {
            currentScene = ne; 
            if(prv.name != null && !prv.name.Equals("Loading")) prevSceneName = prv.name;
            if(ne.name != null && !ne.name.Equals("Loading")) newSceneName = ne.name;
            if(prevSceneName.Equals("")) prevSceneName = newSceneName;
            if(!newSceneName.Equals(prevSceneName))
            {
                foreach(var map in clearedMaps)
                {
                    map.position = Vector3.zero;
                    map.used = false;
                }
                foreach(var trigger in clearedTriggers) trigger.used = false;
                clearedMaps.Clear();
                clearedTriggers.Clear();
                prevSceneName = newSceneName;
                formIdx = -1;
            }
        };
    }
    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
    }
    public void RoadRetry()//
    {
        SceneManager.LoadScene(currentScene.name);
    }

    public void LoadScene(string sceneName)
    {
        var loadAo = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        var currentScene = SceneManager.GetActiveScene();
        loadAo.completed += (ao) => { 
            var unloadAo = SceneManager.UnloadSceneAsync(currentScene);
            unloadAo.completed += (uao) => SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        };
    }

    public void AddClearList(MapData map)
    {
        if(clearedMaps.Contains(map)) return;
        clearedMaps.Add(map);
    }
    public void AddClearList(EventTriggerData trigger)
    {
        if(clearedTriggers.Contains(trigger)) return;
        clearedTriggers.Add(trigger);
    }
}
