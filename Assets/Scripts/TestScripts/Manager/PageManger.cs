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

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(Instance);
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

    public void Quit()
    {
        StartCoroutine(QuitCoroutine());
    }
    private IEnumerator QuitCoroutine()
    {
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }
}
