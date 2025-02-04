using Collection;
using System.Linq;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SituationControll : MonoBehaviour
{


    public NewsScriptorble[] wingman_news;

    public GameObject WingmanTimelines;
    public int chapter = 3;

    public bool is_get_nesw = false;

    GameObject target_timeline;

    public enum Situation
    {
        InventoryBox,
        Wingman,
        door
    }

    private bool iskeydown = false;

    public Situation situation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        iskeydown = false;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            iskeydown = true;
        }
        if (iskeydown)
        {
        
            iskeydown = false;
            ObjectEvent();
        }

    }

    public void ObjectEvent()
    {
        switch (situation)
        {
            case Situation.InventoryBox:
                Debug.Log("inventoryBox");
                PopInventory();
                break;
            case Situation.Wingman:
                Debug.Log("Wingman");
                Receive();
                break;
            case Situation.door:
                Debug.Log("door");
                break;
        }
    }


    public void PopInventory()
    {
        GameManager.Instance.InventoryEnable();
    }



    public void Receive()
    {
        Debug.Log("Receive");
        //chapter = DatabaseManager.instance.chapter;//여기 chapter는 나중에 저장된 데이터를 가져올 예정 해
        target_timeline = WingmanTimelines.transform.GetChild(chapter - 3).gameObject;//브런치 없는 타임라인
        if (target_timeline.GetComponent<PlayableDirector>().time == 0)
        {
            if (!is_get_nesw)
            {
                InteractionEvent.Instance.move(0);

                //target_timeline.GetComponent<TimeLineEnd>().PDPLay();
                target_timeline.GetComponent<PlayableDirector>().Play();

                if (wingman_news == null)
                {
                    Debug.LogError("윙맨 뉴스 없음 뉴스 넣어주세요");
                    return;
                }
                if (chapter < 3)
                {
                    Debug.LogError("윙맨이 신문을 가지고 있지 않는 챕터 입니다");
                    return;
                }
                is_get_nesw = true;
            }
            else//선택 이후 신문 받은 경우 타임라인 재생
            {

                //target_timeline = WingmanTimelines.transform.GetChild(branch).gameObject;//브런치 타임라인을 저장 할 예정 신문 받은 응답에 대한 타임라인이 여러개가 나온다면 해당 타임라인 혹은
                                                                                           //타임라인 덩어리 target_timeline과 같은걸 넣어서 오브젝트를 가져올 예정

                //int id =Random.Range(1, 4);//여기 숫자는 윙맨의 대사 id에 따라 달라질 예정입니다
                //InteractionEvent.Instance.move(id);
                //target_timeline = WingmanTimelines.transform.GetChild(chapter - 3).gameObject;
                target_timeline.GetComponent<PlayableDirector>().Play();
            }

        }



    }


    /// <summary>
    /// 저장된 정보 받아서 씬을 로딩 할 예정
    /// </summary>
    public void Door()
    {
        //SceneManager.LoadScene();
    }
}
