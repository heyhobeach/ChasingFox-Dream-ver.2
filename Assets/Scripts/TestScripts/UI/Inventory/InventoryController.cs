using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public InventoryScripable inventorydata;
    public GameObject list;

    public GameObject TextBox;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    public void NewsInventorySet()
    {
        inventorydata=InventoryManager.Instance.GetInventoryAll();
        SetListBox();
        Debug.Log("invenData +" + inventorydata.inventory.Count);
    }
    public void TraceInventorySet()
    {
        inventorydata = null;
        SetListBox();
    }
    public void SetListBox()
    {
        ClearListBox();
        for(int i = 0; i < inventorydata.inventory.Count; i++)
        {
            GameObject gobj=list.transform.GetChild(i).gameObject;
            gobj.SetActive(true);
            //gobj=Instantiate(TextBox);
            //gobj.transform.SetParent(list.transform);

            GameObject text = gobj.transform.GetChild(0).gameObject;// 텍스트 부분
            Debug.Log(text.name);
            text.GetComponent<TMP_Text>().text = inventorydata.inventory[i].name;
        }
    }

    public void SetContent(int i)
    {

    }

    public void ClearListBox()
    {
        for(int i = 0; i < list.transform.childCount; i++)
        {
            list.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    
}
