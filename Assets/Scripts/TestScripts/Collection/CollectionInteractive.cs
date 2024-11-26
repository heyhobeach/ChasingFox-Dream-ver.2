using UnityEngine;
using Collection;

public class CollectionInteractive : MonoBehaviour
{
    public Collection.CollectionScriptorble scriptorbleobj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")//여기서 팝업창 띄우면 될듯
        {
            Debug.Log("collection의 이름 입니다" + scriptorbleobj._name);
            Debug.Log("collection의 내용 입니다" + scriptorbleobj._context);
        }
    }


}
