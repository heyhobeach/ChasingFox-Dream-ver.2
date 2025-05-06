using Collection;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SituationControll : MonoBehaviour
{
    public const int CHP_COUNT = 12;


    public NewsScriptorble[] wingman_news;

    public GameObject WingmanTimelines;
    public int chapter = 3;

    public bool is_get_nesw = false;

    GameObject target_timeline;

    public GameObject door_canvas;

    public SceneNode[] scenearr = new SceneNode[CHP_COUNT];

    public GameObject mesteryObj;

    public class SceneNode
    {
        public string sceneName;
        public string prev;
        public string next;
        public Dictionary<string, string> stage_branch;
        public SceneNode(string name)
        {
            sceneName = name;
            stage_branch = new Dictionary<string, string>();
        }
    }


    public enum Situation
    {
        InventoryBox,
        Wingman,
        door,
        sofa
    }

    private bool isTrigger = false;

    public Situation situation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        if (situation == Situation.door)
        {
            for (int i = 0; i < CHP_COUNT; i++)
            {
                string str = string.Format("chp{0}", i);
                scenearr[i] = new SceneNode(str);
            }
            scenearr[0].next = "chp1";
            scenearr[0].prev = "MainMenu";
            for (int i = 1; i < CHP_COUNT - 1; i++)
            {
                string str = string.Format("chp{0}", i + 1);
                scenearr[i].next = str;
                str = string.Format("chp{0}", i - 1);
                scenearr[i].prev = str;
            }
            scenearr[CHP_COUNT - 1].prev = "chp11";
            scenearr[CHP_COUNT - 1].next = "MainMenu";
        }
        //scenearr[0].stage_branch["brutal"] = "chp2";// 설명 및 예시 scenearr[0](0chp) 의 brutal 분기는 chp2

        /*foreach (var i in scenearr)//연결 확인용
        {
            Debug.Log(i.sceneName + "설정 완료");
            Debug.Log(string.Format(
            "prev = {0} : current = {1} : next = {2}",
            i.prev ?? "NULL",
            i.sceneName,
            i.next ?? "NULL"
        ));
            if (i.stage_branch == null)
            {
                Debug.Log("is Null");
            }
            else
            {
                Debug.Log(i.stage_branch.Count + "count");
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        isTrigger = false;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        isTrigger = true;
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F)&&isTrigger)
        {
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
                CallDoorSystem();
                break;
            case Situation.sofa:
                MesterySystem();
                break;
        }
    }

    public void MesterySystem()
    {
        Debug.Log("mesterySystem");
        mesteryObj.SetActive(true);
    }

    public void PopInventory()
    {
        ServiceLocator.Get<GameManager>().InventoryEnable();
    }


    /// <summary>
    /// 윙맨 관련 정보
    /// </summary>
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

    public void CallDoorSystem()
    {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        //door_canvas.gameObject.SetActive(true);
    }

    public void NextSatge()
    {
        
        int current = chapter;
        current = 1;//테스트용
        Debug.Log(scenearr[current].stage_branch.Count);
        if (scenearr[current].stage_branch.Count > 0)
        {
            //branch 관련 로드
            //ServiceLocator.Get<GameManager>().LoadScene(scenearr[current].stage_branch["brutal"]);
        }
        else
        {
            ServiceLocator.Get<GameManager>().LoadScene(scenearr[current].next);
        }
    }

    public void CloseDoorPop()
    {
        if (door_canvas == null)
        {
            Debug.LogError("NUll 에러");
            Debug.Log(this.gameObject.GetComponent<SituationControll>().door_canvas);
            return;
        }
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        //door_canvas.GetComponent<Canvas>().enabled = false;
        //Debug.Log(door_canvas.gameObject.name);
        //door_canvas.gameObject.SetActive(false);
    }
}
