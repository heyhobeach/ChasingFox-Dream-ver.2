using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneScript : MonoBehaviour
{
    public GameObject go;

    void Start()
    {
        var temp = go.GetComponent<TempDeathPopup>();
        temp.info = "Press to Start";
        temp.unityEvent.RemoveAllListeners();
        temp.unityEvent.AddListener(() => PageManger.Instance.LoadScene("Chp0"));
        go.SetActive(true);
    }
}
