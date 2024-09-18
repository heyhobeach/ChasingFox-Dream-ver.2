using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TempDeathPopup : MonoBehaviour, IPointerClickHandler, IBaseController
{
    public void Controller() {}

    void OnEnable() => ((IBaseController)this).AddController();
    void OnDisable() => ((IBaseController)this).RemoveController();

    public void OnPointerClick(PointerEventData eventData)
    {   
        PageManger.Instance.RoadRetry();
        transform.parent.gameObject.SetActive(false);
    }
}
