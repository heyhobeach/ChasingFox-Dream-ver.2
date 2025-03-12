using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    /// <summary>
    /// 인벤토리 데이터 스크립터블을 갈아끼우기 위한 inventorydata
    /// </summary>
    public InventoryScripable inventorydata;
    /// <summary>
    /// 뷰포트 아래에 있는 선택지 버튼 리스트 저장용
    /// </summary>
    [Tooltip("뷰포트 하위 오브젝트 저장용")]
    public GameObject list;

    /// <summary>
    /// 버튼 개수 확인용
    /// </summary>
    private GameObject content;


    GameObject contentObj;

    /// <summary>
    /// 흔적, 뉴스 분류 확인용
    /// </summary>
    public bool is_trace = true;

    /// <summary>
    /// 인벤토리 몇번을 눌렀냐 확인용
    /// </summary>
    public static int select_num;


    /// <summary>
    /// 오브젝트 활성화 될 때 자동 사이즈 맞춤
    /// </summary>
    private void OnEnable()
    {
        //Viewport = list.transform.GetChild(0).transform.GetChild(0).gameObject;
        content = list.transform.parent.gameObject;
        RectTransform content_rect = content.GetComponent<RectTransform>();//전체 사이즈 조절을 위해
        contentObj = this.transform.GetChild(1).gameObject;//결과 보여주는창
        int count = 0;
        for (int i = 0; i < list.transform.childCount; i++)//활성화 되어있는
        {
            if (list.transform.GetChild(i).gameObject.activeSelf)
            {
                count++;
            }

        }
        //Debug.Log("count =" + count);
        content_rect.sizeDelta = new Vector2(content_rect.sizeDelta.x, count * 95);//아래 ui 크기 대충 맞추기
        list.GetComponent<RectTransform>().sizeDelta = new Vector2(content_rect.sizeDelta.x, count * 95);
    }

    public void NewsInventorySet()//inventoryCanvas ->News 오브젝트 버튼에 할당 되어있는 오브젝트
    {
        inventorydata = InventoryManager.Instance.GetInventoryAll();
        ClearListBox();
        if (inventorydata.news == null)
        {
            Debug.LogError("뉴스 없음");
            return;
        }
        Debug.Log("set News List" + inventorydata.news.Count);
        if (4 > inventorydata.news.Count)//아마 화면에 뜨는게 4개 뜨던가 오브젝트 생성 부분, 정확히 잘 되는지 확인해야함
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject obj = Instantiate(list.transform.GetChild(j).gameObject);
                obj.transform.SetParent(list.transform);
            }
            //5개씩 추가 생성 안 부서지도록? 만약 매번 하면 지금 인벤토리 열때마다
        }
        int i = 0;
        foreach (var item in inventorydata.news)//인벤 데이터 만큼 반복하면서 true 함
        {
            GameObject gobj = list.transform.GetChild(i++).gameObject;
            gobj.SetActive(true);
            ButtonInfo buttonInfo = gobj.GetComponent<ButtonInfo>();

            buttonInfo.button_news = new System.Tuple<int, Inventory.News>(item.Key, item.Value);//button에 Info 매칭 하는 부분
            GameObject text = gobj.transform.GetChild(0).gameObject;// 텍스트 부분
            //Debug.Log(text.name);
            text.GetComponent<TMP_Text>().text = buttonInfo.button_news.Item2.image_name;//여기부터 문제
        }
        contentObj.transform.GetChild(0).gameObject.SetActive(false);
        contentObj.transform.GetChild(1).gameObject.SetActive(true);
        is_trace = false;
        //Debug.Log("NewsData +" + inventorydata.news.Count);
    }
    public void TraceInventorySet()
    {
        inventorydata = InventoryManager.Instance.GetInventoryAll();//inventoryCanvas ->trace 오브젝트 버튼에 할당 되어있는 오브젝트
        //SetTraceListBox();
        ClearListBox();

        if (inventorydata == null)//inventorydata.inventory null에러
        {
            Debug.LogError("수집품이 없음");
            return;
        }
        Debug.Log("set Trace List" + inventorydata.inventory.Count);
        if (4 > inventorydata.inventory.Count)//아마 화면에 뜨는게 4개 뜨던가
        //if (list.transform.childCount<inventorydata.inventory.Count)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject obj = Instantiate(list.transform.GetChild(j).gameObject);
                obj.transform.SetParent(list.transform);
            }
            //5개씩 추가 생성 안 부서지도록? 만약 매번 하면 지금 인벤토리 열때마다
        }
        int i = 0;
        foreach (var item in inventorydata.inventory)//인벤 데이터 만큼 반복하면서 true 함
        {
            GameObject gobj = list.transform.GetChild(i++).gameObject;
            gobj.SetActive(true);
            ButtonInfo buttonInfo = gobj.GetComponent<ButtonInfo>();

            buttonInfo.button_info = new System.Tuple<int, Inventory.Info>(item.Key, item.Value);//button에 Info 매칭 하는 부분
            GameObject text = gobj.transform.GetChild(0).gameObject;// 텍스트 부분
            //Debug.Log(text.name);
            text.GetComponent<TMP_Text>().text = buttonInfo.button_info.Item2.news.image_name;
        }
        contentObj.transform.GetChild(0).gameObject.SetActive(true);
        contentObj.transform.GetChild(1).gameObject.SetActive(false);
        is_trace = true;
    }
    public void SetTraceListBox()//이거 버튼 할당 안 되어있는 부분 같은데
    {
        ClearListBox();

        if (inventorydata == null)//inventorydata.inventory null에러
        {
            Debug.LogError("수집품이 없음");
            return;
        }
        Debug.Log("set Trace List" + inventorydata.inventory.Count);
        if (4 > inventorydata.inventory.Count)//여기 수정 해야할듯
        //if (list.transform.childCount<inventorydata.inventory.Count)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject obj = Instantiate(list.transform.GetChild(j).gameObject);
                obj.transform.SetParent(list.transform);
            }
            //5개씩 추가 생성 안 부서지도록? 만약 매번 하면 지금 인벤토리 열때마다
        }
        int i = 0;
        foreach (var item in inventorydata.inventory)//인벤 데이터 만큼 반복하면서 true 함
        {
            GameObject gobj = list.transform.GetChild(i).gameObject;
            gobj.SetActive(true);
            ButtonInfo buttonInfo = gobj.GetComponent<ButtonInfo>();

            buttonInfo.button_info = new System.Tuple<int, Inventory.Info>(item.Key, item.Value);//button에 Info 매칭 하는 부분
            GameObject text = gobj.transform.GetChild(0).gameObject;// 텍스트 부분
            //Debug.Log(text.name);
            text.GetComponent<TMP_Text>().text = buttonInfo.button_info.Item2.news.image_name;
        }
    }

    public void SetNewsListBox()//이거 버튼 할당 안 되어있는 부분 같은데
    {
        ClearListBox();
        if (inventorydata.news == null)
        {
            Debug.LogError("뉴스 없음");
            return;
        }
        Debug.Log("set News List" + inventorydata.news.Count);
        if (4 > inventorydata.news.Count)//여기 수정 해야할듯
        //if (list.transform.childCount<inventorydata.inventory.Count)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject obj = Instantiate(list.transform.GetChild(j).gameObject);
                obj.transform.SetParent(list.transform);
            }
            //5개씩 추가 생성 안 부서지도록? 만약 매번 하면 지금 인벤토리 열때마다
        }
        int i = 0;
        foreach (var item in inventorydata.news)//인벤 데이터 만큼 반복하면서 true 함
        {
            GameObject gobj = list.transform.GetChild(i).gameObject;
            gobj.SetActive(true);
            ButtonInfo buttonInfo = gobj.GetComponent<ButtonInfo>();

            buttonInfo.button_news = new System.Tuple<int, Inventory.News>(item.Key, item.Value);//button에 Info 매칭 하는 부분
            GameObject text = gobj.transform.GetChild(0).gameObject;// 텍스트 부분
            //Debug.Log(text.name);
            text.GetComponent<TMP_Text>().text = buttonInfo.button_news.Item2.image_name;//여기부터 문제
        }
    }


    /// <summary>
    /// 버튼 눌렸을때 내용 컨트롤 하는 부분
    /// </summary>
    public void SetContent()
    {
        if (is_trace)
        {
            Debug.Log("id" + select_num + "content" + inventorydata.inventory[select_num].context);
            contentObj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = inventorydata.inventory[select_num].context;
        }
        else
        {
            contentObj.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = inventorydata.news[select_num].image;
        }

    }

    public void ClearListBox()
    {
        for (int i = 0; i < list.transform.childCount; i++)
        {
            list.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void InventoryImage()//스크롤 길이 정하는 함수  
    {
        //구현해야함
    }


}