using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TempDeathPopup : MonoBehaviour, IPointerClickHandler, IBaseController
{
    public TMP_Text text;

    public void Controller() {}

    void OnEnable()
    {
        text.text = info;
        ((IBaseController)this).AddController();
    }
    void OnDisable() => ((IBaseController)this).RemoveController();

    public UnityEvent unityEvent;
    public string info;

    public void OnPointerClick(PointerEventData eventData)
    {   
        unityEvent?.Invoke();
        transform.gameObject.SetActive(false);
    }
}
