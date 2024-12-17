using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private static PopupManager instance=null;
    public static PopupManager Instance
    {
        get {
            if(instance == null)
            {
                var obj = new GameObject() { name = "PopupManager" };
                instance = obj.AddComponent<PopupManager>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }

    [SerializeField] private GameObject deathPopup;
    [SerializeField] private GameObject pausePopup;
    [SerializeField] private GameObject quitInfo;

    void Awake()
    {
        if(instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(instance);
    }


    void Start()
    {
        if(deathPopup == null) deathPopup = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Death Popup"), transform);
        if(pausePopup == null) pausePopup = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Pause Popup"), transform);
        if(quitInfo == null) quitInfo = Instantiate(Resources.Load<GameObject>("Prefabs/UI/DefaultUI"), transform);
        // for(int i  = 0; i < transform.childCount; i++)
        // {
        //     // Debug.Log(transform.GetChild(i).name);  
        //     transform.GetChild(i).gameObject.SetActive(false);  
        // }
        quitInfo.SetActive(true);
    }

    // // Update is called once per frame
    // void Update()
    // {
    // }
    public void DeathPop()
    {
        // Debug.Log("���� �˾�");
        var temp = deathPopup.GetComponent<TempDeathPopup>();
        temp.info = "Press to Continue";
        temp.unityEvent.RemoveAllListeners();
        temp.unityEvent.AddListener(GameManager.Instance.RetryScene);
        deathPopup.SetActive(true);
        //Debug.Log(gameObject.name);
        //transform.GetChild(0).gameObject.SetActive(false);
        //Debug.Log(transform.GetChild(0).gameObject.name);  
    }
    public void PausePop(bool enabled)
    {
        pausePopup.SetActive(enabled);
    }
}
