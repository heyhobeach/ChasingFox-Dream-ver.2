using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        SceneManager.activeSceneChanged += (prv, ne)=> {currentScene = ne; };
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
            unloadAo.completed += (uao) => SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        };
    }
}
