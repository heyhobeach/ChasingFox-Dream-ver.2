using UnityEngine;

public class CollectionInteractive : EventTrigger
{
    public Collection.CollectionScriptorble scriptorbleobj;


    [ContextMenu("CollectionPopup")]
    public void CallCollectionPopup()
    {
        
        Collider2D collider = this.gameObject.GetComponent<BoxCollider2D>();
        Vector2 pos = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y+collider.bounds.extents.y*2);
        Vector2 vec=Camera.main.WorldToScreenPoint(pos);
        CollectionCanvasController.Instance.SetPosition(this.gameObject.transform.position); 
        CollectionCanvasController.Instance.Popup();

        CollectionCanvasController.Instance.SetContentText(scriptorbleobj._context);

        Debug.Log("before show inven");
        GameManager.Instance.inventoryManager.showinven();
        Debug.Log("after show inven");

    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("수집품 벗어남");
        CollectionCanvasController.Instance.PopupEnd();
        //closepopup
    }

}
