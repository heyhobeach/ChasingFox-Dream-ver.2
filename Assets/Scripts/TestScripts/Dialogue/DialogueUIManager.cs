using JetBrains.Annotations;
using System;
using System.Collections;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [Tooltip("ver 오브젝트 들어갈 항목")]
    /// <summary>
    /// ver오브젝트 들어갈변수
    /// </summary>
    public GameObject Vertical;

    //public Transform targetTransform;
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
    /// <summary>
    /// 해당 변수는 애니메이션 중에 대사가 넘어가지 않게 하기 위함 만약 해당 상황이 없다면 사용 안 해도 괜찮음, 현재 사용 안 하는 변수 나중에 어떻게 될지 몰라 놔두겠음
    /// </summary>
    public bool is_closing = false;

    public float BoxSizeRatio = 0.34f;
    public float fontRatio;
    public float nameRatio;

    public bool none_state = false;



    /// <summary>
    /// start,end,speed
    /// </summary>
    private int[] typing_speed_arr = { 0, 0, 0 };

    IEnumerator co = null;

    public IEnumerator co_closeAinm;

    Vector3 start_pos, end_pos;

    //public delegate void TestDel();
    //public TestDel testDel;

    /// <summary>
    /// 타이핑 속도 제어 위한 변수 , 명령어가 사용되었는지 확인을 위해 사용중임
    /// </summary>
    bool isTyping = false;

    /// <summary>
    /// typing이 끝났는지 확인 위해서, static인 이유는 다른 클래스에서 해당 동작이 끝났는지 확인 하려고
    /// </summary>
    public static bool isTypingEnd = false;

    private delegate void delayDelegeate();

    private RectTransform intRect;

    private SetCharImage imagesetter;

    public BrutalData brutalData;
    public int brutalScroe;

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
        TextBoxSizeChange();
        CharactorImageSizeChange();

        //Debug.Log("intRect test"+intRect.sizeDelta + "" + intRect.position);
        // setTestPosition(targetTransform.position);
    }

    private void Awake()
    {
        intRect=this.transform.GetChild(1).GetComponent<RectTransform>();
        is_closing = false;

        //brutalData = GameManager.GetBrutalData();//여기 주석 풀면 max게이지 기준
        //brutalScroe = brutalData.Brutality;
        brutalScroe = GameManager.Brutality;//지금 테스트 해 보니 브루탈 100기준 설정되어있는듯
        Debug.Log("brutal 수치 " + brutalScroe);
        //co_closeAinm = ClosingAnim();
    }

    // Update is called once per frame
    private void Update()
    {
        //Debug.Log("자식 사이즈" + this.transform.GetChild(0).GetComponent<RectTransform>().rect.size);

        //TextBoxSizeChange();
        //CharactorImageSizeChange();
        //Debug.Log("intRect test" + intRect.sizeDelta + "" + intRect.position);
    }

    /// <summary>
    /// 글자 크기 위치 폰트 사이즈 조절하는 함수
    /// </summary>
    private void TextBoxSizeChange()//여기서 지금 ui크기에 맞게 대사창 위치, 이름을 맞추고 제작해야함 여기서 조절 필요
    {
        //int크기 스케일링
        intRect.sizeDelta = new Vector2(transform.GetComponent<RectTransform>().rect.width, transform.GetComponent<RectTransform>().rect.height* BoxSizeRatio);
        intRect.position = new Vector3(0, intRect.sizeDelta.y/2,intRect.position.z);

        //아래는 ver을 맞추기위해
        content.fontSize = intRect.sizeDelta.y * fontRatio;
        //Vertical.GetComponent<RectTransform>().sizeDelta = intRect.sizeDelta* 0.68f;//해당 0.75는 intRect크기에 비례한 intRect의 크기
        Vertical.GetComponent<RectTransform>().sizeDelta = new Vector2(intRect.sizeDelta.x*0.9f,intRect.sizeDelta.y * 0.68f);//해당 0.75는 intRect크기에 비례한 intRect의 크기
        // Vertical.GetComponent<RectTransform>().sizeDelta.y/2
        //                                                                          왜 0.38이지? 계산 사으로 안 맞는 느낌인데, 0.34가 딱코
        Vertical.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);


        //이름 부분 위치 수정
        namemesh.fontSize = intRect.sizeDelta.y * nameRatio;
        name_rect.sizeDelta = new Vector2(intRect.sizeDelta.x, intRect.sizeDelta.y*0.3f);//namemesh.fontSize + namemesh.fontSize/6
        name_rect.position = new Vector3(name_rect.sizeDelta.x / 2, Vertical.GetComponent<RectTransform>().sizeDelta.y+name_rect.sizeDelta.y/2, 0);
    }


    /// <summary>
    /// 이미지 크기 설정 및 위치 조정
    /// </summary>
    private void CharactorImageSizeChange()//1980, 1080 비율 유지
    {

        for (int i = 0; i < this.transform.GetChild(0).childCount; i++)
        {
            transform.GetChild(0).GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetComponent<RectTransform>().rect.width*0.328f, transform.GetComponent<RectTransform>().rect.width * 0.36645f);
            float yPox = transform.GetChild(0).GetChild(0).transform.GetComponent<RectTransform>().rect.height;

            //Debug.Log(yPox + "yPos");
            if (i == 0)//이렇게 한 이유는 이미지는 공간 2개 밖에없을거같아서
            {
                transform.GetChild(0).GetChild(i).GetComponent<RectTransform>().position = new Vector3(transform.GetComponent<RectTransform>().rect.width * 0.3f / 2, yPox*0.95f, 0); //yPox/2
            }
            else
            {
                transform.GetChild(0).GetChild(i).GetComponent<RectTransform>().position = new Vector3(Screen.width- transform.GetComponent<RectTransform>().rect.width * 0.3f/2, yPox * 0.95f, 0);//yPox/2
            }
        }
        RectTransform _rect = transform.GetChild(0).GetComponent<RectTransform>();
        _rect.position = new Vector2(_rect.position.x, 540);//여기는 크게 영향이 안감
    }


    /// <summary>
    /// 위치를 받으면 대사창 위치가 옮겨짐
    /// </summary>
    /// <param name="pos"></param>
    public void SetTextPosition(Vector3 pos)//추후 대사 2개이상 발생시 가운데값
    {
        // Vector3 _pos;
        Transform dialogueUiTransform = this.transform.GetChild(1).GetComponent<Transform>();//스크린 좌표로 변환 필요, 0
        //dialogueUiTransform.position = pos;
        dialogueUiTransform.position=Camera.main.WorldToScreenPoint(new Vector3(pos.x,pos.y+1,pos.z));//타겟 오브젝트 위치에 대사 오브젝트 위치 움직임
        Debug.Log(pos);
    }
    public void Setname(string name)
    {
        namemesh.text = name;
    }

    /// <summary>
    /// 브루탈 수치에 따라 파일명 유동적으로 변경하는함수
    /// </summary>
    /// <param name="image_name"></param>
    /// <returns></returns>
    public string ChangeBrutalName(string image_name)
    {
        string newName = "";
        if (Regex.IsMatch(image_name, "Reaper"))//6번째에 퍼센트 삽입
        {
            if (brutalScroe == 100)
            {
                brutalScroe = 100;
            }else if (brutalScroe >= 90 && brutalScroe < 100)
            {
                brutalScroe = 90;
            }else if (brutalScroe <= 35)
            {
                brutalScroe = 35;
            }
            newName = image_name.Insert(6, brutalScroe.ToString());
            //Debug.Log("브루탈 이미지" + newName);//브루탈 이미지 확인할때 필요한 부분
            return newName;
        }

        return image_name;
    }

    public void SetImage(string image_name,string image_dir,bool is_disable=false)//이 부분은 next text에서 계속 불러옴 그래서 그런 느낌을 원하면 여기서 값을 조정해야하는게 맞ㅇ므
    {
        //string str = @"^[a-zA-Z]";
        if(image_name == null)//추가 한 부분
        {
            return;
        }
        bool isAlone = Regex.IsMatch(image_dir, @"^alone_");
        if (isAlone)
        {
            image_dir = image_dir.Substring(6);
        }
        image_dir = Regex.Replace(image_dir, @"[^a-zA-Z]", "");
        if (image_dir == "none")
        {
            none_state = true;
        }
        else
        {
            none_state = false;
        }

        Debug.Log("변경후"+image_dir);
        
        //나중에 선택지때 중앙만 오게 된다면 여기서 설정 할 예정
        imagesetter.ChangeImage(ChangeBrutalName(image_name),image_dir,isAlone);//좌우 기준
    }

    public void LoadImage()//이 부분은 한번만 일어남 이미지 켤때
    {
        //여기에서 isalone 받아야함
        Tuple<string, string>[] name_list = namemesh.transform.parent.GetComponent<InteractionEvent>().GetImageNameList();
        //Debug.Log(string.Format("제공 문자열 {0} 결과 값 {1}", name_list[0].Item2, Regex.IsMatch(name_list[0].Item2, @"^alone_")));//마지막 부분에 에러남 


        if (name_list == null)
        {
            Debug.Log("튜플이 null 입니다");
            //throw new Exception("튜플이 null 입니다");
            return;
        }
        string nameStr = name_list[0].Item2;
        bool isAlone = Regex.IsMatch(name_list[0].Item2, @"^alone_");
        //for(int i = 0; i < name_list.Length; i++)//현재 대화의 다음의 모습이 보이는거 같은데 해당 방식이 맞음 이유는 다음 대사에 대한 이미지를 가져와야하기때문에
        //{
        //    Debug.Log("name is" + name_list[i].Item1+"dir is" + name_list[i].Item2);
        //
        //}
        SetImage(name_list[1].Item1, name_list[1].Item2);
        SetImage(name_list[0].Item1, nameStr,isAlone);
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
        //while (!UIController.Instance.is_dialogue_on)
        StopCoroutine(co);
        // int br_count = 0;
        //float width=content.fontSize* GetContentLength(_content, ref br_count);
        //float hight = content.fontSize + (content.fontSize * br_count);
        //content.rectTransform.sizeDelta = new Vector2(width, hight);
        co = Typing(_content,isTyping);
        StartCoroutine(co);
    }
    // public async void SetContent(string[] _contentArr)//배열로 받을 예정 선택지 관련 내용 , 여기서 배열로 사용 예정
    public void SetContent(string[] _contentArr)//배열로 받을 예정 선택지 관련 내용 , 여기서 배열로 사용 예정
    {
        //StopCoroutine(co);
        // int br_count = 0;
        //float width = content.fontSize * GetContentLength(_contentArr, ref br_count);//여기부분은 수정 해야하는데 아마 선택지에 br이 안들어갈거같아서 방치
        //float hight = content.fontSize + (content.fontSize * br_count);              //
        //content.rectTransform.sizeDelta = new Vector2(width, hight);                 //
    
        CreatSelect(_contentArr);
        Debug.Log("비동기 시작");
        //await ImageSliding();
        //await imagesetter.ImageAnim();
        Debug.Log("비동기 끝");
        //co = TextSliding(_contentArr);//선택지 배열 움직이는 슬라이딩 애니메이션
        //StartCoroutine(co);
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
                TMP.color = Color.white;
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
        //fixedVertical.GetComponent<VerticalLayoutGroup>().enabled = true;
        //첫 설정때 contentArr 설정 필요 지금 contentArr이 아무것도 없다고 되어있음 따라서 contentArr[0]에는 content가 들어가야함
        // string pattern = "<[^>]*>?";
        if(isTyping)//만약 타이핑 관련 명령어 사용시 해당 값에따라 defalut인지 아닌지, 명령어 사용시 false로 되어서 defalt speed가 적용이 안됨
        {
            Array.Clear(typing_speed_arr,0,typing_speed_arr.Length);
            typing_speed = DEFAULT_SPEED;
        }
        isTyping = true;
        if (contentArr.Length>1)//사유 오브젝트 없음
        {
          DestroySelectBox();
        }
        isTypingEnd = false;
        content.text = null;
        // if (content.color != Color.black)
        // {
        //     content.color = Color.black;
        // }
        content.color = Color.white;
        if (str == "")
        {
            yield return null;
        }
        // string tag = "<";
        for (int i = 0; i < str.Length; i++)
        {
            if (isTypingEnd)
            {
                break;
            }
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
        content.text = str;
        isTypingEnd = true;
        //Debug.Log("타이핑 종료");
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

    /// <summary>
    /// 해당 라인들 수정 필요
    /// </summary>
    public void EnableUI()
    {
        this.transform.gameObject.SetActive(true);
    }
    public void DisableUI()
    {
        this.transform.gameObject.SetActive(false);
    }
    public void Setclear()
    {
        namemesh.text = "";
        content.text = "";
    }
    public bool GetTextActive()//수정 필요할듯 이상함 , 없어도 될듯한데?
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
        //is_closing = true;
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
        content.color = Color.white;
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
            //select.transform.localPosition = new Vector3(content.transform.localPosition.x - 1, content.transform.localPosition.y-(height*i), content.transform.localPosition.z);
            Debug.Log("선택지 위치" + select.transform.localPosition + "content 높이"+content.rectTransform.sizeDelta);

            //select.transform.localScale = new Vector3(1, 2, 1);
            //      select.transform.parent = content.transform.parent;
            select.text = strArr[i];
            select.color = Color.gray;
            //if (max_widht < select.rectTransform.sizeDelta.x)
            //{
            //    max_widht=select.rectTransform.sizeDelta.x;
            //}
            //select.gameObject.SetActive(false);
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

    public async Awaitable ImageSliding()//해당 ui나오는중에는 입력이 되면 안됨 여기서 코루틴 작업들 진행 그러면 해당 작업이 끝나고 나서 뒤에 작업들이 진행이 됨
    {                                   //여기서 위치가 맞게 나옴
        Debug.Log("비동기중");
        float time = 0;
        float duration = 3f;
        float yPox = transform.GetChild(0).GetChild(0).transform.GetComponent<RectTransform>().rect.height;
        //transform.GetChild(0).GetChild(i).GetComponent<RectTransform>().position = new Vector3(transform.GetComponent<RectTransform>().rect.width * 0.3f / 2, yPox/2, 0); 

        RectTransform main_rect = transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>();
        while (time < duration)
        {
            time +=Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }
        main_rect.position= new Vector3(transform.GetComponent<RectTransform>().rect.width * 0.3f / 4, yPox / 2, 0);
        Debug.Log("5초끝");

    }

}