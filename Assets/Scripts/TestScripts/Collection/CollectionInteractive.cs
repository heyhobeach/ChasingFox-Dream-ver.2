using UnityEngine;

public class CollectionInteractive : EventTrigger
{
    public Collection.CollectionScriptorble scriptorbleobj;


    [ContextMenu("CollectionPopup")]
    public void CallCollectionPopup()
    {
        Collider2D collider = this.gameObject.GetComponent<BoxCollider2D>();
        Debug.Log("collection의 이름 입니다" + scriptorbleobj._name);
        Debug.Log("collection의 내용 입니다" + scriptorbleobj._context);
        Debug.Log("collection의 위치 입니다" + collider.transform.position.x+","+collider.bounds.center.y + collider.bounds.extents.y);
        Debug.Log("collection의 위치 입니다" + this.gameObject.transform.position);
        Vector2 vec=Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
        Debug.Log("스크린 위치 vec"+vec);
        CollectionCanvasController.Instance.SetPosition(vec); 
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
