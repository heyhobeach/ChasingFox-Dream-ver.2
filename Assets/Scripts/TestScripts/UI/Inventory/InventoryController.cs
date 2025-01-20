using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public InventoryScripable inventorydata;
    public GameObject list;

    public GameObject TextBox;

    public static int select_num;
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
        int i = 0;
        foreach(var item in inventorydata.inventory)
        {
            GameObject gobj=list.transform.GetChild(i).gameObject;
            gobj.SetActive(true);
            ButtonInfo buttonInfo=gobj.GetComponent<ButtonInfo>();

            //gobj=Instantiate(TextBox);
            //gobj.transform.SetParent(list.transform);
            buttonInfo.button_info = new System.Tuple<int, Inventory.Info>(item.Key,item.Value);//button에 Info 매칭 하는 부분
            GameObject text = gobj.transform.GetChild(0).gameObject;// 텍스트 부분
            Debug.Log(text.name);
            text.GetComponent<TMP_Text>().text = buttonInfo.button_info.Item2._name;
        }
    }

    public void SetContent()
    {
        GameObject contentObj=this.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
        Debug.Log("id" + select_num + "content" + inventorydata.inventory[select_num].context);

        //Debug.Log(contentObj.name);

        contentObj.GetComponent<TMP_Text>().text = inventorydata.inventory[select_num].context;
    }

    public void ClearListBox()
    {
        for(int i = 0; i < list.transform.childCount; i++)
        {
            list.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    
}
