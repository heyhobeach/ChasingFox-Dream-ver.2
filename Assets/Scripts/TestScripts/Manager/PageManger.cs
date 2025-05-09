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
    public bool isLoadingScene { get { return nextAo != null; } }
    private AsyncOperation nextAo;
    public Action aoComplatedAction;

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

    public bool LoadScene(string sceneName, bool active = true)
    {
        if(nextAo != null)
        {
            Debug.LogWarning("Scene is already loading. Please wait until the current scene is loaded.");
            return false;
        }
        var currentScene = SceneManager.GetActiveScene();
        nextAo = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        nextAo.completed += (ao) => 
        {
            var prevAo = SceneManager.UnloadSceneAsync(currentScene);
            aoComplatedAction?.Invoke();
            aoComplatedAction = null;
            nextAo = null;
        };
        nextAo.allowSceneActivation = active;
        return true;
    }
    public void SceneActive() => nextAo.allowSceneActivation = true;

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
