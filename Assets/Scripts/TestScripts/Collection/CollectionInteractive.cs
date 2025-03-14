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
        Debug.Log(string.Format("내용 :{0}", scriptorbleobj._context));
        CollectionCanvasController.Instance.SetContentText(scriptorbleobj._context);

    }

    public void ReadingGlassPopup()
    {
        CollectionCanvasController.Instance.ImageSetPosition(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y ));//+ GetComponent<BoxCollider2D>().bounds.extents.y * 2)
        CollectionCanvasController.Instance.ImagePopup();
    }
    protected override void OnTriggerExit2D(Collider2D collision)
    {
        CollectionCanvasController.Instance.PopupEnd();
        base.OnTriggerExit2D (collision);
        eventIdx = 0;
        //closepopup
    }

}
