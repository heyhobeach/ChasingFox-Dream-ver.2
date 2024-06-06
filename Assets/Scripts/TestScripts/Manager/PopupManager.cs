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
                instance = new PopupManager();
            }return instance;
        }
    }


    void Start()
    {
        for(int i  = 0; i < transform.childCount; i++)
        {
            Debug.Log(transform.GetChild(i).name);  
            transform.GetChild(i).gameObject.SetActive(false);  
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DeathPop()
    {
        Debug.Log("Á×À½ ÆË¾÷");
        PageManger.Instance.RoadRetry();
        //Debug.Log(gameObject.name);
        //transform.GetChild(0).gameObject.SetActive(false);
        //Debug.Log(transform.GetChild(0).gameObject.name);  
    }
}
