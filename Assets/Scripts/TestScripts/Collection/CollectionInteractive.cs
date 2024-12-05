using UnityEngine;
using Collection;
using Unity.VisualScripting;

public class CollectionInteractive : EventTrigger
{
    public Collection.CollectionScriptorble scriptorbleobj;


    [ContextMenu("CollectionPopup")]
    public void CallCollectionPopup()
    {
        Debug.Log("collection의 이름 입니다" + scriptorbleobj._name);
        Debug.Log("collection의 내용 입니다" + scriptorbleobj._context);
        CollectionCanvasController.Instance.Popup();
        CollectionCanvasController.Instance.SetContentText(scriptorbleobj._context);
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("수집품 벗어남");
        CollectionCanvasController.Instance.PopupEnd();
        //closepopup
    }

}
