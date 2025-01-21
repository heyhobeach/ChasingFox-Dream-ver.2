using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    /// <summary>
    /// 인벤토리 데이터 스크립터블을 갈아끼우기 위한 inventorydata
    /// </summary>
    public InventoryScripable inventorydata;
    public GameObject list;

    /// <summary>
    /// 
    /// </summary>
    public GameObject content;

    public static int select_num;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    private void OnEnable()
    {
        //Viewport = list.transform.GetChild(0).transform.GetChild(0).gameObject;
        content = list.transform.parent.gameObject;
        RectTransform content_rect = content.GetComponent<RectTransform>();

        int count = 0;
        for(int i=0;i<list.transform.childCount;i++)
        {
            if (list.transform.GetChild(i).gameObject.activeSelf)
            {
                count++;
            }
            
        }
        //Debug.Log("count =" + count);
        content_rect.sizeDelta = new Vector2(content_rect.sizeDelta.x, count * 95);
        list.GetComponent<RectTransform>().sizeDelta = new Vector2(content_rect.sizeDelta.x, count * 95);
    }

    public void NewsInventorySet()
    {
        inventorydata=InventoryManager.Instance.GetInventoryAll();
        SetListBox();
        Debug.Log("invenData +" + inventorydata.inventory.Count);
    }
    public void TraceInventorySet()
    {
        inventorydata = InventoryManager.Instance.GetNewsDataAll();
        SetListBox();
        Debug.Log("NewsData +" + inventorydata.inventory.Count);
    }
    public void SetListBox()
    {
        ClearListBox();

        if (4> inventorydata.inventory.Count)
        //if (list.transform.childCount<inventorydata.inventory.Count)
        {
            for(int j = 0; j < 4; j++)
            {
                GameObject obj=Instantiate(list.transform.GetChild(j).gameObject);
                obj.transform.SetParent(list.transform);
            }
            //5개씩 추가 생성 안 부서지도록? 만약 매번 하면 지금 인벤토리 열때마다
        }
        int i = 0;
        foreach(var item in inventorydata.inventory)//인벤 데이터 만큼 반복하면서 true 함
        {
            GameObject gobj=list.transform.GetChild(i).gameObject;
            gobj.SetActive(true);
            ButtonInfo buttonInfo=gobj.GetComponent<ButtonInfo>();

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

    public void SetScrollHeight()//스크롤 길이 정하는 함수  
    {

    }

    
}
