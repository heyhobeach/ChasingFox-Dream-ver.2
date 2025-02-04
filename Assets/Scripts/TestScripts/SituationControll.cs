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
            ObjectEvent();
            iskeydown = false;
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
        if (!is_get_nesw)
        {
            InteractionEvent.Instance.move(0);
            GameObject target_timeline = WingmanTimelines.transform.GetChild(chapter - 3).gameObject;
            //target_timeline.GetComponent<TimeLineEnd>().PDPLay();
            target_timeline.GetComponent<PlayableDirector>().Play();

            if (wingman_news == null)
            {
                Debug.LogError("윙맨 뉴스 없음 뉴스 넣어주세요");
                return;
            }

            chapter = DatabaseManager.instance.chapter;
            if (chapter < 3)
            {
                Debug.LogError("윙맨이 신문을 가지고 있지 않는 챕터 입니다");
                return;
            }
            //Debug.Log(wingman_news[chapter - 3].image);
            //UIController.Instance.DialogueCanvasSetTrue();
            //InventoryManager.Instance.AddNews(wingman_news[DatabaseManager.instance.chapter-3]);//될것 같음
            is_get_nesw = true;
        }
        else
        {
            int id =Random.Range(1, 4);//여기 숫자는 윙맨의 대사 id에 따라 달라질 예정입니다
            InteractionEvent.Instance.move(id);
            GameObject target_timeline = WingmanTimelines.transform.GetChild(chapter - 3).gameObject;
            target_timeline.GetComponent<PlayableDirector>().Play();
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
