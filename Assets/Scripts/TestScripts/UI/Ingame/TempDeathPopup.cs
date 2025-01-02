using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TempDeathPopup : MonoBehaviour, IPointerClickHandler, IBaseController
{
    public TMP_Text text;
    
    private Action _onDown;
    public Action onDown { get => _onDown; set => throw new NotImplementedException(); }
    private Action _onUp;
    public Action onUp { get => _onUp; set => throw new NotImplementedException(); }

    public void Controller() 
    {
        if (Input.anyKeyDown&&!Input.GetKeyDown(KeyCode.Escape)) 
        {
            unityEvent?.Invoke();
            transform.gameObject.SetActive(false);
        }
    }

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
