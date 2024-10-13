using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Unity.Loading;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices.WindowsRuntime;


public class InteractionEvent : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] DialogueEvent dialogue;
    //UIManager ui;



    public static bool isSkip = false;

    [Tooltip("해당 아이디로 이동")]
    public int num = 0;
    int contentNum = 0;

    [Tooltip("대화 뭉텅이 번호")]
    public int indexNum = 0;
    int contentlength = 0;

    string[] command = new string[1];
    bool start = false;

    Coroutine skipco = null;

    /// <summary>
    /// uimanager 스킬븥 접근 변수
    /// </summary>
    UIManager _Uimanager;

    /// <summary>
    /// 명령어 끼리 , 를 분리하는 정규식
    /// </summary>
    string SPLIT_COMMAND_PASER = @"[""!,]";//명령어 분리 정규식

    //private delegate void delayDelegeate();//문제없이 돌아갈경우 삭제
    //private delayDelegeate _nextDelegate;

    /// <summary>
    /// 이전에 실행될 명령어 리스트
    /// </summary>
    public List<Command> precommands = new List<Command>();
    /// <summary>
    /// 이후에 실행될 명령어 리스트
    /// </summary>
    public List<Command> postcommands = new List<Command>();

    public abstract class Command
    {
        public UIManager _uiManger;
        public virtual void OnExecute() { }
        public virtual string OnExecute(string str_) { return str_; }
    }

    public class SizeCommand : Command
    {
        public int start;
        public int end;
        public float size;
        public string _str;
        public SizeCommand(string[] args, string str, UIManager manager)
        {
            Debug.Log("Get Sting" + str);
            start = int.Parse(args[0]);
            end = int.Parse(args[1]);
            size = float.Parse(args[2]);
            _uiManger = manager;
            _str = str;

        }
        public override string OnExecute(string str_ = "")
        {
            //base.OnExecute();
            Debug.Log("onExecute테스트");
            _str = _uiManger.UpSizeText(_str, start, end, size);
            return _str;
        }
    }

    public class SpeedCommand : Command
    {
        public int start;
        public int end;
        public int _speed;
        public string _str;
        public SpeedCommand(string[] args,string str, UIManager manager)
        {
            start = int.Parse(args[0]);
            end = int.Parse(args[1]);
            _speed = int.Parse(args[2]);
            _uiManger = manager;
            _str = str;

        }
        public override void OnExecute()
        {
            //base.OnExecute();
            Debug.Log("onExecute테스트");
            _uiManger.TypingSpeed(_str, start, end, _speed);
        }
    }

    public class TimeCommand : Command
    {
        InteractionEvent _manager;
        public TimeCommand(InteractionEvent manager)
        {
            _manager = manager;

        }
        public override void OnExecute()
        {
            //base.OnExecute();
            Debug.Log("Skip onExecute테스트");
            _manager.skipco=_manager.StartCoroutine(_manager.ChocieTimer(5, true, null));
            //_uiManger.
        }
    }

    public class BrutalCommand : Command
    {
        InteractionEvent _manager;
        public BrutalCommand(InteractionEvent manager)
        {
            _manager = manager;

        }
        public override void OnExecute()
        {
            //base.OnExecute();
            Debug.Log("onExecute테스트");
            //_uiManger.
        }
    }

    public class PoliceCommand : Command
    {
        InteractionEvent _manager;
        public PoliceCommand(InteractionEvent manager)
        {
            _manager = manager;

        }
        public override void OnExecute()
        {
            //base.OnExecute();
            Debug.Log("Police onExecute테스트");
            //_manager.
        }
    }

    public class PlayCommand : Command
    {
        InteractionEvent _manager;
        public PlayCommand(InteractionEvent manager)
        {
            _manager = manager;

        }
        public override void OnExecute()
        {
            //base.OnExecute();
            Debug.Log("onExecute테스트");
            //_manager.
        }
    }

    public class AnimCommand : Command
    {
        public int start;
        public int end;
        public int aniNum;
        public AnimCommand(string[] args, UIManager manager)
        {
            start = int.Parse(args[0]);
            end = int.Parse(args[1]);
            aniNum = int.Parse(args[2]);
            _uiManger = manager;

        }
        public override void OnExecute()
        {
            //base.OnExecute();
            Debug.Log("onExecute테스트");
            _uiManger.TextAni(start, end, aniNum);
        }
    }

    public class HigherCommand : Command
    {
        public int value;
        InteractionEvent _manager;
        public HigherCommand(string[] args, InteractionEvent manager)
        {
            value = int.Parse(args[0]);
            _manager = manager;

        }
        public override void OnExecute()
        {
            //base.OnExecute();
            Debug.Log("onExecute테스트");
            //_uiManger.ani(start, end, size);
        }
    }

    public class MoveCommand : Command
    {
        public int id;
        InteractionEvent _event;

        public MoveCommand(string[] args, InteractionEvent manager)
        {
            id = int.Parse(args[0]);
            _event = manager;

        }
        public override void OnExecute()
        {
            //base.OnExecute();
            Debug.Log("onExecute테스트");
            _event.move(id);
            //_uiManger.ani(start, end, size);
        }
    }

    [ContextMenu("GetDialogue")]
    public Dialogue[] GetDialogue()
    {

        if (indexNum >= DatabaseManager.instance.indexList.Count)
        {
            indexNum = DatabaseManager.instance.indexList.Count - 1;
        }
        dialogue.line.y = DatabaseManager.instance.indexList[indexNum];//마지막 라인을 받아오기는 하지만 필요한건 마지막라인이 아닌 인덱스? 딕셔너리에 들어가는 그 y가 필요함
        if (dialogue.line.x > dialogue.line.y)
        {
            Debug.Log("대화 종료");
            return null;
        }
        dialogue.dialouses = null;
        dialogue.dialouses = DatabaseManager.instance.GetDialogues((int)dialogue.line.x, (int)dialogue.line.y);//y값 찾아오는 법
        if(dialogue.dialouses == null)
        {
            Debug.Log("GetDialogues error");
        }
        else
        {
            Debug.Log("Getdialoogues succese" + dialogue.dialouses.Length);
            Debug.Log(string.Format("content {0} num{1}", contentNum, num));
            Debug.Log(string.Format("Get content {0}", dialogue.dialouses[0].context));
            num = 0;
        }
        _Uimanager.content.text = "";
        command = Regex.Split(dialogue.dialouses[num].command[contentNum], SPLIT_COMMAND_PASER, RegexOptions.IgnorePatternWhitespace);//이게 위로 간다면?
        //Debug.Log("길이" + dialogue.dialouses.Length);
        return dialogue.dialouses;
    }

    private void Awake()
    {
        _Uimanager = gameObject.GetComponentInParent<UIManager>();
    }
    private void Start()
    {
        GetDialogue();
        //command[0] = "";
        foreach (var i in DatabaseManager.instance.indexList)
        {
            //Debug.Log(string.Format("list {0}", i));
        }

    }

    private void Update()
    {
        //Debug.Log("size"+this.transform.parent.GetComponent<RectTransform>().rect.size);
        //if ((num <= dialogue.dialouses.Length))//line을 조절 해야함 대화가 끝나는 시점을 정하려면 line.y를 설정해야함

        HandleDialogue();
        //if (num > dialogue.dialouses.Length)
        //{
        //    EndDialogue();
        //}
    }

    public void SetSkip(bool skip) => isSkip = skip;
    public void SetSkip(Transform tf) {}

    private void HandleCommand()
    {
        //string str = "";
        //foreach (var com in command)
        //{
        //    str += com;
        //}
        //Debug.Log("명령어" + str);//이전 명령어를 가져옴
        //명령어 호출 시점 조절 했으므로 명령어 구별해서 호출 시점 구분
        if (command.Length > 0)
        {
            //Debug.Log("command size is " + command.Length + "command [1]" + command[1]);

            command = spaceremove(command);
            CallFunction(command);
            //Debug.Log("command size is " + command.Length+"command [0]" + command[0]);
            //command[0] = "";

        }
        contentNum = 0;
    }

    private void CallCommand(ref List<Command> _commandList)
    {
        foreach (var _command in _commandList)
        {
            if (_command is SizeCommand)
            {
                //Debug.Log("sizecommand 호출 테스트" + "id" + dialogue.dialouses[num].id + "이름" + dialogue.dialouses[num].name);//여기는 한번
                string str = _command.OnExecute("");//이 str을 대입
                //Debug.Log(str);
                dialogue.dialouses[num].context[contentNum] = str;
                //Debug.Log("size 변경후 " + str);
                continue;
            }
            _command.OnExecute();

        }
        _commandList.RemoveAll(x => true);
    }
    public void SetNextContext()
    {
        //while (gameObject.GetComponentInParent<UIManager>().is_closing) { }//역시나 무한루프
        //Debug.Log("nextContext");
        //
        CallCommand(ref postcommands);//이후 실행되어야하는 명령어들
        HandleCommand();

        //Thread.Sleep(1000);
        if (num < dialogue.dialouses.Length)
        {
            _Uimanager.EnableUI();
            CallCommand(ref precommands);//이전에 실행되어야할 명령어들
                                         //이게 두번 일어나는듯?
            Debug.Log("명령어 호출 테스트" + "id" + dialogue.dialouses[num].id + "이름" + dialogue.dialouses[num].name);//여기는 한번
            _Uimanager.Setname(dialogue.dialouses[num].name);//이름 변경 되는중 마찬가지로 내용도 같이 하면 될듯
                                                             //Debug.Log(dialogue.dialouses[num].context.Length);
                                                             //Debug.Log(string.Format("num => {0} contentnum ={1}", num, contentNum));
            contentlength = dialogue.dialouses[num].context.Length;
            //Debug.Log("contentLength"+contentlength);//지금 자꾸 길이가 0이라고 나옴
            if (contentlength == 1)
            {
                _Uimanager.SetContent(string.Join("", dialogue.dialouses[num].context[contentNum]));

            }
            else
            {
                Debug.Log("선택지 부분");
                string[] textSum = new string[contentlength];
                //gameObject.GetComponentInParent<UIManager>().SetContent(string.Join("", ""));
                for (int index = 0; index < contentlength; index++)//한번만 호출 되어야함
                {
                    //Debug.Log(string.Format("index =>{0} : content=>{1}",index, dialogue.dialouses[num].context[index]));
                    textSum[index] = dialogue.dialouses[num].context[index];


                }
                _Uimanager.SetContent(textSum);
                if (start == false)
                {
                    start = true;
                    StartCoroutine(ChocieTimer(10, start, Timeover));
                }
            }



                Debug.Log(string.Format("num {3}, name {0}: content{1} , 명령어{2}", dialogue.dialouses[num].name, dialogue.dialouses[num].context[contentNum], dialogue.dialouses[num].command[contentNum],num));
                if (++num == dialogue.dialouses.Length)
                {
                    //command = Regex.Split(dialogue.dialouses[num-1].command[contentNum], SPLIT_COMMAND_PASER, RegexOptions.IgnorePatternWhitespace);//여기까지는 순서 맞음 command받기전에 num이 증가되어야함
                    return;
                }
                command = Regex.Split(dialogue.dialouses[num].command[contentNum], SPLIT_COMMAND_PASER, RegexOptions.IgnorePatternWhitespace);//여기까지는 순서 맞음 command받기전에 num이 증가되어야함


            Debug.Log("command length"+command.Length);
            Debug.Log("Getdialoogues succese" + dialogue.dialouses.Length);

        }
        else
        {
            EndDialogue();
        }
        contentNum = 0;
        Debug.Log("contentNum 조욜");

        //num++;
    }
    public void HandleDialogue()
    {
        if (( isSkip) && !_Uimanager.is_closing)//f누를때 문제 생기는듯?

        //if ((Input.GetKeyDown(KeyCode.F) || isSkip)&&!_Uimanager.is_closing)//f누를때 문제 생기는듯?
        {
            isSkip = false;
            if (skipco != null)
            {
                StopCoroutine(skipco);
                skipco = null;
            }

            //Debug.Log("선택 번호" + contentNum);
            
            _Uimanager.CloseSelceet(contentNum);

            if (_Uimanager.is_closing)
            {
                Debug.Log("닫는중");
                StartCoroutine(_Uimanager.ClosingAnim(SetNextContext));
                //Debug.Log("실행 했음");
                return;
                //클로징 중이면 넘어가면 안 됨
                //Debug.Log("클로징 확인" + gameObject.GetComponentInParent<UIManager>().is_closing);
                //return;
            }
            SetNextContext();

        }
        if (contentlength > 1)//선택지 부분
        {
            Debug.Log("선택지 부분");
            int temp = num;
            if (num > dialogue.dialouses.Length - 1)
            {
                temp = dialogue.dialouses.Length - 1;
            }
            //string textSum = "";
            //if (start == false)//이게 사용되는 부분인지 모르겠넹,처음 부터 선택지일경우?//일단 주석 처리 했는데 만나자 마자 선택지가 발생하는 경우? 그때 아마 사용 될것 같음 지금은 아마 사용 안 될듯
            //{
            //    start = true;
            //    StartCoroutine(ChocieTimer(5, start, Timeover));
            //}
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                //countnum은 downArrow가 실행 되면 값이 변하게 되어있음
                if (contentlength - 1 > (contentNum))
                {
                    _Uimanager.DownArrow(ref contentNum);
                    command = Regex.Split(dialogue.dialouses[temp].command[contentNum], SPLIT_COMMAND_PASER, RegexOptions.IgnorePatternWhitespace);
                    return;
                }

            }

            if (Input.GetKeyDown(KeyCode.UpArrow) & (contentNum > 0))
            {
                _Uimanager.UpArrow(ref contentNum);
                command = Regex.Split(dialogue.dialouses[temp].command[contentNum], SPLIT_COMMAND_PASER, RegexOptions.IgnorePatternWhitespace);
                return;
            }

        }
        else
        {
            start = false;
        }
    }

    /// <summary>
    /// 라인 불러오기
    /// </summary>
    /// <param name="nums">[Num IndexNum Line]</param>
    public void CallLine(string nums)
    {
        var numarr = Array.ConvertAll(nums.Split(), int.Parse);
        indexNum = numarr[1];
        dialogue.line.x = numarr[2];
        GetDialogue();
        num = numarr[0];
    }

    public void _DisableUI()
    {
        _Uimanager.DisableUI();
    }

    private void EndDialogue()
    {
        Debug.Log("대화끝");
        if (_Uimanager.GetTextActive())
        {
            _DisableUI();
            Debug.Log("비활성화");
            isSkip = false;
            return;
        }
        if ( (indexNum < DatabaseManager.instance.indexList.Count))
        //if (Input.GetKeyDown(KeyCode.X) & (indexNum < DatabaseManager.instance.indexList.Count))
        {
            dialogue.line.x = ++dialogue.line.y;
            indexNum++;
            //num--;

            if (GetDialogue() == null)
            {
                Debug.Log("대화 끝났습니다");
                _Uimanager.content.text = "대화종료";//추후 나중에 삭제 확인용
                return;
            }
            num = 0;
            SetNextContext();
            Debug.Log(dialogue.dialouses.Length);

        }
    }

    private void CallFunction(string[] _functions)
    {
        int temp = num;
        if (num> dialogue.dialouses.Length-1)
        {
            temp = dialogue.dialouses.Length - 1;
        }
        string SPLIT_NUM = @"([a-z]+|\ )+";//공백 분리 정규식//새로운식([a-z]+|\ )+
        string GET_COMMAND = @"[a-z]{1,}";
        //Debug.LogFormat("명령어 호출 1번째 요소" + _functions[0]);
        //Debug.Log("_functions 체크 " + _functions.ToString());

        foreach (var func in _functions)
        {
            Debug.Log("num=>" +num+ "func"+func);
            string[] strarr = Regex.Split(func, SPLIT_NUM);//
            string[] filteredSubstrings = strarr.Where(s => s != Regex.Match(s, SPLIT_NUM).ToString()).ToArray();
            int n;
            //string[] numarr = Array.FindAll(strarr, s => !string.IsNullOrEmpty(s) && (int.TryParse(s, out n)));
            Debug.Log(string.Format("mat 체크=>{0}", func));
            var mat = Regex.Matches(func, GET_COMMAND);
            Debug.Log(string.Format("커맨드 체크 =>{0}", mat[0]));
            switch (mat[0].ToString())
            {
                case "size":
                    {
                        Debug.Log("Num=>" + num);//선택지 부분에서만 문제
                        precommands.Add(new SizeCommand(filteredSubstrings, dialogue.dialouses[temp].context[contentNum], _Uimanager));
                        //size(filteredSubstrings); 
                    }
                    break;
                case "speed":
                    {
                        precommands.Add(new SpeedCommand(filteredSubstrings, dialogue.dialouses[temp].context[contentNum], _Uimanager));
                        //speed(filteredSubstrings); 
                    }
                    break;
                case "time":
                    {
                        precommands.Add(new TimeCommand(this));
                        //time(); 
                    }
                    break;
                case "brutal":
                    {
                        postcommands.Add(new BrutalCommand(this));
                        brutal(); 
                    }
                    break;
                case "police":
                    {
                        postcommands.Add(new PoliceCommand(this));
                        //police(); 
                    }
                    break;
                case "play":
                    {
                        postcommands.Add(new PlayCommand(this));
                    }
                    break;
                case "anime":
                    { 
                        precommands.Add(new AnimCommand(filteredSubstrings,_Uimanager));
                        //anime(filteredSubstrings); 
                    }
                    break;
                case "move":
                    {
                        postcommands.Add(new MoveCommand(filteredSubstrings, this));
                        //postcommands.Add(new MoveCommand(filteredSubstrings, this));
                    }
                    break;
                case "higher"://시작 전 혹은 후 라고 되어있는데 언제 어떻게 될지
                    {
                        postcommands.Add(new HigherCommand(filteredSubstrings,this));
                        //precommands.Add(new HigherCommand(filteredSubstrings, this));
                    }
                    break;

            }
            command = new string[1] { "" };
        }
    }
    private string[] spaceremove(string[] com)//공백 제거 함수
    {
        List<string> temp = new List<string>();
        //int index = 0;
        foreach (var j in com)
        {
            if (j.ToString() != "")
            {
                temp.Add(j);
            }
        }

        return temp.ToArray();

    }


    public void size(string[] command_args)//시작 끝 수치
    {
        Debug.Log(string.Format("switch_size {0} {1} {2}", command_args[0], command_args[1], command_args[2]));
        //SizeCommand sizeCommand = new SizeCommand(command_args,_Uimanager);
        //sizeCommand.size();
        //_Uimanager.UpSizeText(int.Parse(command_args[0]), int.Parse(command_args[1]), int.Parse(command_args[2]));

        //Debug.Log("switch_size");
    }
    public void speed(string[] command_args)//시작 끝 수치
    {
        Debug.Log(string.Format("switch_speed {0} {1} {2}", command_args[0], command_args[1], command_args[2]));
        //_Uimanager.TypingSpeed(int.Parse(command_args[0]), int.Parse(command_args[1]), int.Parse(command_args[2]));
    }
    public void time()
    {
        Debug.Log("switch_time");
    }
    public void brutal()
    {
        Debug.Log("switch_brutal");
    }
    public void police()
    {
        Debug.Log("switch_plice");
    }
    public void play()
    {
        Debug.Log("switch_play");
    }
    public void anime(string[] command_args)//시작 끝 종류
    {
        Debug.Log(string.Format("switch_anime {0} {1} {2}", command_args[0], command_args[1], command_args[2]));
    }
    public void move(int id)//호출 순서? 조금 수정 필요
    {
        Debug.Log(string.Format("switch_move {0} current {1} ", id, dialogue.dialouses[num].id));
        while (num != id)
        {
            if (id > int.Parse(dialogue.dialouses[num].id))
            {
                num++;
            }
            else if (id < int.Parse(dialogue.dialouses[num].id))
            {
                num--;
            }
            else
            {
                break;
            }
        }

        //num = id-DatabaseManager.instance.startLine;//그냥 변환이 안 되는중
        //Debug.Log("자료형"+command_args)

        //num--;
        Debug.Log(num + "변환 테스트");
        contentNum = 0;
    }



    public void Timeover()
    {
        Debug.Log("time over");
        if (contentlength > 1 && num <= dialogue.dialouses.Length)
        {
            Debug.Log(string.Format("{0} num {1} contentNum", num - 1, contentNum));    
            Debug.Log("Time over" + dialogue.dialouses[num - 1].context[0]);
            //_Uimanager.CloseSelceet(0);
            //command = Regex.Split(dialogue.dialouses[num - 1].command[0], SPLIT_COMMAND_PASER, RegexOptions.IgnorePatternWhitespace);
            contentNum = 0;
            foreach(var i in command)
            {
                Debug.Log("Command"+ i );

            }
            isSkip = true;
        }
    }
    IEnumerator ChocieTimer(float seconds, bool start, Action? act)
    {
        if (start)
        {
            Debug.Log("코루틴 초" + seconds);
            yield return new WaitForSeconds(seconds);
            Debug.Log("time 코루틴 시작");
        }

        if (act == null)
        {
            Debug.Log("null이므로 skip 실행");
            isSkip = true;
            skipco = null;
        }
        else
        {
            Debug.Log("선택지 act");
            act();
        }

    }
}