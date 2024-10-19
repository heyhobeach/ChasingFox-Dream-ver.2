using JetBrains.Annotations;
using System;
using System.Collections;
using System.Net.Mime;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// using static UnityEditor.Timeline.TimelinePlaybackControls;

public class UIManager : MonoBehaviour
{

    public GameObject Vertical;

    public Transform targetTransform;
    /// <summary>
    /// 선택지 생성한 오브젝트 담는 배열
    /// </summary>
    TMP_Text[] contentArr = null;
    /// <summary>
    /// 다이얼로그 상자 크기
    /// </summary>
    float size;

    /// <summary>
    /// 선택지 배열로 받을시 최대 크기 찾기
    /// </summary>
    float max_widht;

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

    public float BoxSizeRatio = 0.1f;

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

    private RectTransform intRect;

    private SetCharImage imagesetter;


    [SerializeField]
    public float typing_speed = 0.05f;
    private const float DEFAULT_SPEED= 0.05f;
    //public InteractionEvent interactionEvent;
    private RectTransform name_rect;
    // Start is called before the first frame update
    void Start()
    {
        //testDel = ActTest;
        co = Typing("",isTyping);
        contentArr = new TMP_Text[1];
        size= content.rectTransform.rect.size.y;
        name_rect= namemesh.transform.GetComponent<RectTransform>();
        imagesetter=this.transform.GetChild(0).GetComponent<SetCharImage>();
        // setTestPosition(targetTransform.position);
    }

    private void Awake()
    {
        intRect=this.transform.GetChild(1).GetComponent<RectTransform>();
        is_closing = false;
        //co_closeAinm = ClosingAnim();
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.Log("자식 사이즈" + this.transform.GetChild(0).GetComponent<RectTransform>().rect.size);

        TextBoxSizeChange();
        CharactorImageSizeChange();
    }

    private void TextBoxSizeChange()
    {
        //int크기 스케일링
        intRect.sizeDelta = new Vector2(transform.GetComponent<RectTransform>().rect.width, transform.GetComponent<RectTransform>().rect.height* BoxSizeRatio);
        intRect.position = new Vector3(0, intRect.sizeDelta.y/2,intRect.position.z);

        //아래는 ver을 맞추기위해
        content.fontSize = intRect.sizeDelta.y * 0.6f;
        Vertical.GetComponent<RectTransform>().sizeDelta = intRect.sizeDelta * 0.75f;//해당 0.75는 intRect크기에 비례한 intRect의 크기
        Vertical.GetComponent<RectTransform>().position = new Vector3(intRect.sizeDelta.x/2,intRect.sizeDelta.y/2,intRect.position.z);


        //이름 부분 위치 수정
        Debug.Log("이름 부분 " + namemesh.transform.name);
        namemesh.fontSize = intRect.sizeDelta.y * 0.3f;
        name_rect.sizeDelta = new Vector2(intRect.sizeDelta.x, namemesh.fontSize + namemesh.fontSize/6);
        name_rect.position = new Vector3(name_rect.sizeDelta.x / 2, intRect.sizeDelta.y+name_rect.sizeDelta.y/2, 0);
    }

    private void CharactorImageSizeChange()
    {
        float yPox = transform.GetChild(0).GetChild(0).transform.GetComponent<RectTransform>().rect.height;
        for (int i = 0; i < this.transform.GetChild(1).childCount; i++)
        {
            transform.GetChild(0).GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetComponent<RectTransform>().rect.width*0.3f, transform.GetComponent<RectTransform>().rect.width * 0.3f);
            if (i == 0)//이렇게 한 이유는 이미지는 공간 2개 밖에없을거같아서
            {
                transform.GetChild(0).GetChild(i).GetComponent<RectTransform>().position = new Vector3(transform.GetComponent<RectTransform>().rect.width * 0.3f / 2, yPox/2, 0); 
            }
            else
            {
                transform.GetChild(0).GetChild(i).GetComponent<RectTransform>().position = new Vector3(Screen.width- transform.GetComponent<RectTransform>().rect.width * 0.3f/2, yPox/2, 0);
            }
        }
    }


    /// <summary>
    /// 위치를 받으면 대사창 위치가 옮겨짐
    /// </summary>
    /// <param name="pos"></param>
    public void SetTextPosition(Vector3 pos)//추후 대사 2개이상 발생시 가운데값
    {
        Vector3 _pos;
        Transform dialogueUiTransform = this.transform.GetChild(1).GetComponent<Transform>();//스크린 좌표로 변환 필요, 0
        //dialogueUiTransform.position = pos;
        dialogueUiTransform.position=Camera.main.WorldToScreenPoint(new Vector3(pos.x,pos.y+1,pos.z));//타겟 오브젝트 위치에 대사 오브젝트 위치 움직임
        Debug.Log(pos);
    }
    public void Setname(string name)
    {
        namemesh.text = name;
    }

    public void SetImage(string image_name,string image_dir)
    {
        //string str = @"^[a-zA-Z]";
        image_dir = Regex.Replace(image_dir, @"[^a-zA-Z]", "");
        Debug.Log("변경후"+image_dir);
        //나중에 선택지때 중앙만 오게 된다면 여기서 설정 할 예정
        imagesetter.ChangeImage(image_name,image_dir);//좌우 기준
    }


    /// <summary>
    /// 글자 수 받아오는곳
    /// </summary>
    /// <param name="strings"></param>
    /// <param name="br_count">br개수 리턴</param>
    /// <returns>글자수</returns>
    public int GetContentLength(string[] strings,ref int br_count)
    {
        int max = 0;
        string str = "";
        foreach(string s in strings)
        {
            int str_count_temp = 0;
            int br_count_temp = 0;
            string tag = "";
            for(int i=0;i< s.Length; i++)
            {
                if (s[i] == '<')
                {
                    while (true)
                    {
                        tag += s[i];
                        if (s[i++] == '>')
                        {
                            tag += s[i];
                            if (tag == "<br>")//태그가 끝난시점에 br이라면 br_count증가
                            {
                                br_count_temp++;
                            }
                            break;
                        }
                    }
                }
                str += s[i];
                str_count_temp++;
            }
            if(str_count_temp > max)
            {
                max= str_count_temp;
            }
            if (br_count_temp > br_count)
            {
                br_count=br_count_temp;
            }
        }
        Debug.Log("내용: "+str+"글자수 :" + max);
        return max;
    }

    public int GetContentLength(string str, ref int br_count)
    {
        return GetContentLength(new string[] { str },ref br_count) ; 
    }

    public void SetContent(string _content)
    {
        StopCoroutine(co);
        int br_count = 0;
        //float width=content.fontSize* GetContentLength(_content, ref br_count);
        //float hight = content.fontSize + (content.fontSize * br_count);
        //content.rectTransform.sizeDelta = new Vector2(width, hight);
        co = Typing(_content,isTyping);
        StartCoroutine(co);
    }
    public void SetContent(string[] _contentArr)//배열로 받을 예정
    {
        StopCoroutine(co);
        int br_count = 0;
        //float width = content.fontSize * GetContentLength(_contentArr, ref br_count);//여기부분은 수정 해야하는데 아마 선택지에 br이 안들어갈거같아서 방치
        //float hight = content.fontSize + (content.fontSize * br_count);              //
        //content.rectTransform.sizeDelta = new Vector2(width, hight);                 //

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
    /// 태그의 내용 <size></size>처럼 안의 내용을 피하기 위함
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
        if (contentArr.Length>1)//사유 오브젝트 없음
        {
          DestroySelectBox();
        }
        content.text = null;
        // if (content.color != Color.black)
        // {
        //     content.color = Color.black;
        // }
        if (str == "")
        {
            yield return null;
        }
        string tag = "<";
        for (int i = 0; i < str.Length; i++)
        {
            IgnoreTag(str, ref i, ref typing_speed_arr);
            if (str[i]=='>') { continue; }

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
                    //tag += '>';
                    content.text += tag;
                    //i++;
                    break;
                }
            }
        }
    }
    public void EnableUI()
    {
        this.transform.gameObject.SetActive(true);
    }
    public void DisableUI()
    {
        this.transform.gameObject.SetActive(false);
    }
    public bool GetTextActive()//수정 필요할듯 이상함
    {
        //GameObject g = gameObject.transform.GetChild(0).transform.GetChild(0).gameObject;
        Debug.Log(Vertical.name);
        if (Vertical.activeSelf)
        {
            Debug.Log("켜져있음");
            return true;
        }
        else
        {
            Debug.Log("꺼져있음");
            return false;
        }
    }
    void DestroySelectBox()
    {
        //Debug.Log("선택 위치"+c)
        Debug.Log("파괴");
        for(int i = 1; i < contentArr.Length; i++)
        {
            Destroy(contentArr[i].transform.gameObject);
        }
        contentArr = new TMP_Text[1];
        //ContentArr = null;
    }
    public void CloseSelceet(int choseIndex)
    {
        if (contentArr.Length == 1) return;
        int childs = content.transform.parent.transform.childCount;
        is_closing = true;
        GameObject selectobj = content.transform.parent.GetChild(choseIndex).gameObject;
        string tmpstr = selectobj.GetComponent<TMP_Text>().text;
        start_pos=selectobj.transform.position;
        end_pos =content.transform.parent.GetChild(childs-1).transform.position;
        Debug.Log(string.Format("start_pos {0} end_pos{1}",start_pos,end_pos));
        DestroySelectBox();
        content.text = tmpstr;
    }

    public string UpSizeText(string _str,int start,int end, float size)//리턴으로 진행하는게 맞을듯 함 그런데 이제 텍스트 삽입이 여러개가 되어야한다면 해당 부분
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
    public void TypingSpeed(string _str, int start, int end, int speed)
    {
        Debug.Log(_str.Length);
        Debug.Log(string.Format("TypingSpeed start{0} end{1}, speed{2}", _str[start], _str[end], speed));
        isTyping = false;
        SetTypingSpeed(start,end, speed);
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
        contentArr = new TMP_Text[strArr.Length];
        contentArr[0] = content;
        Debug.Log("Content 위치" + content.transform.localPosition);
        max_widht = content.rectTransform.sizeDelta.x;
        for (int i = 1; i < select_count; i++)//오브젝트 생성과 텍스트 배치
        {
            Debug.Log(content.transform.parent.name);
            TMP_Text select = Instantiate(content, content.transform.localPosition - new Vector3(content.transform.localPosition.x-1, content.transform.localPosition.y, content.transform.localPosition.z), Quaternion.identity);
            Debug.Log(select.transform.localScale);
            contentArr[i] = select;
            select.transform.SetParent(content.transform.parent);
            float height = content.rectTransform.sizeDelta.y;
            select.transform.localPosition = new Vector3(content.transform.localPosition.x - 1, content.transform.localPosition.y-(height*i), content.transform.localPosition.z);
            Debug.Log("선택지 위치" + select.transform.localPosition + "content 높이"+content.rectTransform.sizeDelta);

            select.transform.localScale = new Vector3(1, 2, 1);
            //      select.transform.parent = content.transform.parent;
            select.text = strArr[i];
            select.color = Color.gray;
            if (max_widht < select.rectTransform.sizeDelta.x)
            {
                max_widht=select.rectTransform.sizeDelta.x;
            }
            select.gameObject.SetActive(false);
        }
        Debug.Log("최대 크기" + max_widht);
        //content.gameObject.SetActive(false);
    }

    IEnumerator TextSliding(string[] strArr)//배열로 받을 예정
    {
        //is_select_show = true;
        int a = 0;
        Debug.Log("maxSize" + max_widht);
        GetContentLength(strArr, ref a);
        float endPos = contentArr[0].transform.localPosition.x;//이미지 내려가는 애니메이션
        content.gameObject.SetActive(false);
        Vector2 verticalSize = Vertical.GetComponent<RectTransform>().sizeDelta;
        float currentSize = verticalSize.y;
        int count_increase = contentArr.Length - 1;
        float height = content.rectTransform.sizeDelta.y;
        float delta = 0;
        float ver_y = 0;
        while (delta<=duration)
        {
            float t = delta / duration;
            ver_y = Mathf.Lerp(currentSize, currentSize + (height * count_increase), t);
            Vertical.GetComponent<RectTransform>().sizeDelta = new Vector2(max_widht, ver_y);
            delta += Time.deltaTime;
            yield return null;
        }
        delta = 0;


        GameObject fixedVertical = content.transform.parent.gameObject;//텍스트 슬라이딩 해 오는 애니메이션
        fixedVertical.GetComponent<VerticalLayoutGroup>().enabled = false;

        int count = 0;
        Debug.Log("height" + height);
        while (delta <= duration&(count<strArr.Length))
        {
            float t = delta / duration;
            t = 1 - Mathf.Pow(1 - t, 3);
            contentArr[count].gameObject.SetActive(true);
            float current = Mathf.Lerp(content.transform.localPosition.x - 1, endPos, t);//시작위치,도착위치,t
            contentArr[count].transform.localPosition = new Vector3(current, contentArr[0].transform.localPosition.y-((height) *count), contentArr[count].transform.localPosition.z);//여기 문제
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
        fixedVertical.GetComponent<ContentSizeFitter>().verticalFit=ContentSizeFitter.FitMode.PreferredSize;
        is_select_show = false;


    }

}