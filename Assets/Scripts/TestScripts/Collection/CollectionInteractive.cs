using UnityEngine;
using Collection;
using Unity.VisualScripting;

public class CollectionInteractive : EventTrigger
{
    public Collection.CollectionScriptorble scriptorbleobj;

    
    public void TestFunc()
    {
        Debug.Log("collection의 이름 입니다" + scriptorbleobj._name);
        Debug.Log("collection의 내용 입니다" + scriptorbleobj._context);
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        //OnEnable()
    }

}
