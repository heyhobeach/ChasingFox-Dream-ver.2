using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static System.Net.WebRequestMethods;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class UIManager : MonoBehaviour
{
    public Transform targetTransform;
    /// <summary>
    /// 선택지 생성한 오브젝트 담는 배열
    /// </summary>
    TMP_Text[] ContentArr = null;
    /// <summary>
    /// 다이얼로그 상자 크기
    /// </summary>
    float size;

    int select_count;
    //string test_str;
    /// <summary>
    /// 텍스트 오브젝트 이동 애니메이션 시간
    /// </summary>
    [SerializeField]
    private float duration = 3f;
    //public Text
    public TMP_Text namemesh;
    public TMP_Text content;

    public bool is_select_show = false;
    public bool is_closing = false;

    /// <summary>
    /// start,end,speed
    /// </summary>
    private int[] typing_speed_arr = { 0, 0, 0 };

    IEnumerator co = null;

    public IEnumerator co_closeAinm;

    Vector3 start_pos, end_pos;

    //public delegate void TestDel();
    //public TestDel testDel;

    bool isTyping = false;

    private delegate void delayDelegeate();


    [SerializeField]
    public float typing_speed = 0.05f;
    private const float DEFAULT_SPEED= 0.05f;
    //public InteractionEvent interactionEvent;

    // Start is called before the first frame update
    void Start()
    {
        //testDel = ActTest;
        co = Typing("",isTyping);
        ContentArr = new TMP_Text[1];
        size= content.rectTransform.rect.size.y;
        setTestPosition(targetTransform.position);
    }

    private void Update()
    {
        setTestPosition(targetTransform.position);
    }
    private void Awake()
    {
        is_closing = false;
        //co_closeAinm = ClosingAnim();
    }

    // Update is called once per frame

    public void setTestPosition(Vector3 pos)
    {
        Vector3 _pos;
        Transform dialogueUiTransform = this.transform.GetChild(0).GetComponent<Transform>();//스크린 좌표로 변환 필요
        //dialogueUiTransform.position = pos;
        dialogueUiTransform.position=Camera.main.WorldToScreenPoint(new Vector3(pos.x,pos.y+1,pos.z));//타겟 오브젝트 위치에 대사 오브젝트 위치 움직임
        //_pos = pos;
        //_pos = GetComponentInChildren<Transform>().position;
        //_pos = pos;
        Debug.Log(pos);
    }
    public void Setname(string name)
    {
        namemesh.text = name;
    }

    public void SetContent(string _content)
    {
        StopCoroutine(co);
        co = Typing(_content,isTyping);
        StartCoroutine(co);
    }
    public void SetContent(string[] _contentArr)//배열로 받을 예정
    {
        StopCoroutine(co);
        CreatSelect(_contentArr);
        co = TextSliding(_contentArr);
        StartCoroutine(co);
    }
    public void ChangeText(int countNum)//화살표 맞게 글자 색 변경하는 부분
    {
        TMP_Text TMP;
        content.color = Color.gray;
        for (int i = 0; i < select_count; i++)
        {
            TMP = content.transform.parent.GetChild(i).GetComponent<TMP_Text>();
            if (i == countNum)
            {
                TMP.color = Color.black;
            }
            else
            {
                TMP.color = Color.gray;
            }
        }
    }
    /// <summary>
    /// 두 값을 비교해서 num이 더 크다면 num을 증가시킴
    /// </summary>
    /// <param name="num">태그에서 start,end숫자</param>
    /// <param name="pivot">for문 에서 i숫자</param>
    private void Increase(ref int num,int pivot)
    {
        if (num >= pivot)
        {
            num++;
        }
    }
    private void SetTypingSpeed(int start,int end,int speed)
    {
        typing_speed_arr[0] = start;
        typing_speed_arr[1] = end;
        typing_speed_arr[2] = speed;
    }

    public void UpArrow(ref int countNum)
    {
        if (is_select_show) return;
        countNum--;
        ChangeText(countNum);

    }
    public void DownArrow(ref int countNum)
    {
        if (is_select_show) return;
        countNum++;
        ChangeText(countNum);
    }
    IEnumerator Typing(string str,bool s)
    {
        GameObject fixedVertical = content.transform.parent.gameObject;
        fixedVertical.GetComponent<VerticalLayoutGroup>().enabled = true;
        //첫 설정때 contentArr 설정 필요 지금 contentArr이 아무것도 없다고 되어있음 따라서 contentArr[0]에는 content가 들어가야함
        string pattern = "<[^>]*>?";
        if(isTyping)
        {
            Array.Clear(typing_speed_arr,0,typing_speed_arr.Length);
            typing_speed = DEFAULT_SPEED;
        }
        isTyping = true;
        if (ContentArr.Length>1)//사유 오브젝트 없음
        {
          DestroySelectBox();
        }
        content.text = null;
        if (content.color != Color.black)
        {
            content.color = Color.black;
        }
        if (str == "")
        {
            yield return null;
        }
        string tag = "<";
        for (int i = 0; i < str.Length; i++)
        {
            IgnoreTag(str, ref i, ref typing_speed_arr);

            if (i >= typing_speed_arr[0] && i <= typing_speed_arr[1])
            {
                typing_speed = typing_speed_arr[2] * 0.02f;
                Debug.Log("typing_speed=>" + typing_speed);
            }
            else
            {
                typing_speed = DEFAULT_SPEED;
            }
            content.text += str[i];
            //content.text++str[i]+tag;
            yield return new WaitForSeconds(typing_speed);
        }
        //SetTypingSpeed(-1, -1, (int)(DEFAULT_SPEED*0.02f));
        Debug.Log("타이핑 종료");
        Array.Clear(typing_speed_arr, 0,typing_speed_arr.Length);
    }

    /// <summary>
    /// 태그를 무시하기 위한 함수
    /// </summary>
    /// <param name="str">태그가 포함된 문자열 전체</param>
    /// <param name="i">반복문 안에서 사용되는 반복되는 i 태그를 벗어날때까지 i를 증가 시킴</param>
    /// <param name="start_end_arr">명령어 사용시 처음과 끝</param>
    void IgnoreTag(string str, ref int i, ref int[] start_end_arr)
    {
        if (str[i] == '<')
        {
            //Debug.Log("태그 시작");
            int j = 0;
            string tag = "<";

            while (true)//태그 무시하고 삽입하기 위함 태그 무시하는게 
            {
                //Debug.Log("tag test" + str[i+j]);       
                j++;
                tag += str[i + j];
                Increase(ref start_end_arr[0], i);
                Increase(ref start_end_arr[1], i);
                if (str[i + j] == '>')
                {
                    i += j;
                    Increase(ref start_end_arr[0], i);
                    Increase(ref start_end_arr[1], i);
                    Debug.Log("Tag = " + tag);
                    content.text += tag;
                    i++;
                    break;
                }
            }
        }
    }

    void DestroySelectBox()
    {
        //Debug.Log("선택 위치"+c)
        Debug.Log("파괴");
        for(int i = 1; i < ContentArr.Length; i++)
        {
            Destroy(ContentArr[i].transform.gameObject);
        }
        ContentArr = new TMP_Text[1];
        //ContentArr = null;
    }
    public void CloseSelceet(int choseIndex)
    {
        if (ContentArr.Length == 1) return;
        //Debug.Log("선택번호"+choseIndex);
        //int childs = this.gameObject.transform.parent.transform.childCount;
        int childs = content.transform.parent.transform.childCount;
        //Debug.Log("자식수"+childs);
        is_closing = true;
        //Debug.Log("선택 자식"+content.transform.parent.GetChild(choseIndex).GetComponent<TMP_Text>().text);
        GameObject selectobj = content.transform.parent.GetChild(choseIndex).gameObject;
        string tmpstr = selectobj.GetComponent<TMP_Text>().text;
        //content.text = tmpstr;//여기가 문제 발생중
        start_pos=selectobj.transform.position;
        end_pos =content.transform.parent.GetChild(childs-1).transform.position;
        Debug.Log(string.Format("start_pos {0} end_pos{1}",start_pos,end_pos));
        DestroySelectBox();
        //StartCoroutine(ClosingAnim(()=>{}));
        //StartCoroutine(ClosingAnim(testDel));
        //0 1 2 아래로 -50x
        //선택한 번호의 위치 계산 
        //해당 위치로 스무스하게 이동
        //enumerator를 이용해 보간 이동을 아래로 하도록 위치는 텍스트 3번째 기본 텍스트 위치 기준
    }

    public string UpSizeText(string _str,int start,int end, int size)//리턴으로 진행하는게 맞을듯 함 그런데 이제 텍스트 삽입이 여러개가 되어야한다면 해당 부분
    {
        Debug.Log("UpsizeText 실행");
        string headtag = string.Format("<size={0}>", size);
        string tailtag = string.Format("</size>");
        string targetstring = "";
        for(int i = start; i <= end; i++)
        {
            targetstring += _str[i];
        }
        //Debug.Log(targetstring + "targetstring");
        string change_string = headtag+targetstring+tailtag;
        Debug.Log(change_string + "targetstring");
        return _str.Replace(targetstring, change_string); 
    }
    public void TypingSpeed(int start,int end,int speed)
    {
        Debug.Log(string.Format("TypingSpeed start{0} end{1}, speed{2}", content.text[start], content.text[end], speed));
        isTyping = false;
        SetTypingSpeed(start,end,speed);
    }

    public IEnumerator ClosingAnim(Action Act=null)
    //IEnumerator ClosingAnim()
    {
        is_closing = true;
        //yield return new WaitForSecondsRealtime(1);
        GameObject fixedVertical = content.transform.parent.gameObject;
        fixedVertical.GetComponent<VerticalLayoutGroup>().enabled = false;
        float t = 0;
        float _duration = 1;
        content.transform.position = start_pos;
        //Debug.Log("출발 위치 " + start_pos+"content 위치"+content.transform.position);
        while (t < _duration)
        {
            //보간이동 내용
            t = t / _duration;
            //1 - (1 - x) * (1 - x);
            float lerp_y=Mathf.Lerp(content.transform.position.y, end_pos.y, t);
            //Debug.Log("lerp y is" + lerp_y);
            content.transform.position = new Vector3(content.transform.position.x, lerp_y, content.transform.position.z);
            t += Time.deltaTime;
            yield return null;
        }
        //Debug.Log("1초 끝");
        is_closing = false;
        if (Act == null)
        {
            yield return null;
        }
        else
        {
            Debug.Log("액션 시작");
            Act();
        }
    }
    /// <summary>
    /// 애니메이션은 타이핑 되는중에 출력도 다 되어야함 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="aniNum"></param>
    public void TextAni(int start,int end,int aniNum)
    {
        Debug.Log(string.Format("{0}에서 {1}까지 {2}번 애니메이션 재생", start, end, aniNum));
        switch (aniNum)
        {
            case 0: break;
            default: break;
        }
    }
    void CreatSelect(string[] strArr)
    {
        select_count = strArr.Length;
        Debug.Log("strArr length " + strArr.Length);    
        //Debug.Log("strArr[0]" + strArr[0]);
        //for(int i=0;i<strArr.Length;i++)
        content.text = strArr[0];
        content.color = Color.black;
        ContentArr = new TMP_Text[strArr.Length];
        ContentArr[0] = content;
        for (int i = 1; i < select_count; i++)//오브젝트 생성과 텍스트 배치
        {
            Debug.Log(content.transform.parent.name);
            TMP_Text select = Instantiate(content, this.transform.position-new Vector3(this.transform.position.x-100,this.transform.position.y,this.transform.position.z), Quaternion.identity);
            ContentArr[i] = select;
            select.transform.SetParent(content.transform.parent);
            //      select.transform.parent = content.transform.parent;
            select.text = strArr[i];
            select.color = Color.gray;
        }
    }

    IEnumerator TextSliding(string[] strArr)//배열로 받을 예정
    {
        //is_select_show = true;
        float delta = 0;
        GameObject fixedVertical = content.transform.parent.gameObject;
        fixedVertical.GetComponent<VerticalLayoutGroup>().enabled = false;
        Debug.Log("여기" + ContentArr[0]);
        Debug.Log("여기" + ContentArr[1]);

        Debug.Log("여기" + ContentArr[2]);
        float endPos = ContentArr[0].transform.position.x;
        int count = 0;
        Debug.Log("size" + size);
        while (delta <= duration&(count<3))
        {
            float t = delta / duration;
            t = 1 - Mathf.Pow(1 - t, 3);
            float current = Mathf.Lerp(this.transform.position.x - 100, endPos, t);//시작위치,도착위치,t
            ContentArr[count].transform.position = new Vector3(current, ContentArr[0].transform.position.y-(size*count), ContentArr[count].transform.position.z);//여기 문제
            delta += Time.deltaTime;
            if(delta> duration)
            {
                Debug.Log("증가");
                count++;
                delta = 0;
                continue;
            }
            //Debug.Log(ContentArr[0].text + "위치 " + current);
            yield return null;
        }
        fixedVertical.GetComponent<VerticalLayoutGroup>().enabled = true;
        is_select_show = false;


    }
}