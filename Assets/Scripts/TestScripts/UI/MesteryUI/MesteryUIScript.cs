
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.Search;

//using UnityEditor.Rendering;

using UnityEngine;

using UnityEngine.UIElements;
using UnityEngine.Windows;
using static UnityEditor.Recorder.OutputPath;


public class MesteryUIScript : MonoBehaviour
{

    VisualElement visualElement;//메인 부분
    VisualElement dragGhost = null;
    VisualElement textContainer;//일기
    VisualElement textContainerContent;//일기의 내용을 담고 있음
    VisualElement mesteryContainer;

    /// <summary>
    /// 버튼들을 담고있는 객체
    /// </summary>
    VisualElement inven_button_parent;
    /// <summary>
    /// inven 버튼 좌
    /// </summary>
    Button inven_left_button;
    /// <summary>
    /// inven 버튼 우
    /// </summary>
    Button inven_right_button;

    /// <summary>
    /// 일기 부분 버튼 담는 객체
    /// </summary>
    VisualElement diary_button_parent;
    /// <summary>
    /// 일기 부분 버튼 왼쪽
    /// </summary>
    Button diary_button_back;
    /// <summary>
    /// 일기 부분 버튼 오른쪽
    /// </summary>
    Button diary_button_finsh;


    VisualElement diary;

    VisualElement currentProblem;
    string dragGhostName = "";

    /// <summary>
    /// 현재 info_keys가 id에 맞게 정확하게 들어있다는 보장이 없음
    /// </summary>
    private List<int> info_keys = new List<int>();
    bool is_drag = false;

    bool is_sentence = false;

    Vector2 startMousePosition;
    InventoryScripable inventoryScripable;

    int num = 0;

    string SPLIT_COMMAND_PASER = @"[""!,]";

    string[] contentContextArray;
    int contentContextArrayIndex = 0;
    List<Tuple<string,string>>answerTexts=new List<Tuple<string, string>>();
    List<string>originText=new List<string>();

    int mesteryEventNum;
    int hidden_start_index = -1;

    [Tooltip("추리 시스템 진행할때 현재 챕터 가능한 부분")]
    public int currentChapterNum = 2;

    bool is_hiddenmode = false;

    TextElement currentElement;
    bool ishome = true;


    private async void OnEnable()
    {
        hidden_start_index = -1;
  
        Debug.Log(InventoryManager.Instance.GetInventoryAll().name);
        inventoryScripable = InventoryManager.Instance.GetInventoryAll();//인벤토리에서 데이터를 가져옴
        

        Dictionary<int, Inventory.Info>.KeyCollection keys = InventoryManager.Instance.GetinventoryKeys();//키를 가져오기 위한 변수


        foreach (var key in keys)//현재 수집 되어있는 key를 가져옴
        {
            //Debug.Log("keytype"+key.GetType());
            //Debug.Log("key =>" + key+InventoryManager.Instance.GetInfo_(key).context);//내용

            info_keys.Add(key);
        }
        if (inventoryScripable.inventory == null)
        {
            Debug.LogError("inventoryScripable Null");
        }


        // UIDocument에서 rootVisualElement 가져오기
        var root = GetComponent<UIDocument>().rootVisualElement;

        // textContainer 가져오기
        textContainer = root.Q<VisualElement>("textContainer");
        //textContainer.setpa
        visualElement = root.Q<VisualElement>("VisualElement");
        textContainerContent = root.Q<VisualElement>("textContainerContent");
        mesteryContainer = root.Q<VisualElement>("MysteryContainer");


        visualElement.Add(diary);
        visualElement.style.visibility = Visibility.Hidden;


        ButtonSet(root);

        await CloseRoom(0.5f, GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("VisualElement"));
        ishome = true;
        currentElement = new TextElement();

        SetDiaryText(ref textContainer, StartEvent("Diary_0.001"));//처음 수집품별 다이어리 내용
        //ui toolkit에서 제공하는 함수로 이벤트 등록에 사용됨

        //패널에 함수 등록
        visualElement.RegisterCallback<PointerDownEvent>(OnPointerDown);
        visualElement.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        visualElement.RegisterCallback<PointerUpEvent>(evt =>
        {
            is_drag = false;
            is_sentence = false;
            if (dragGhost != null)
                visualElement.Remove(dragGhost);
            dragGhost = null;
            //dragGhost = null;
        });

        var panel = root.Q<VisualElement>("TracerNotePanel");
        panel.style.scale = new Vector2(0, 0);




        var clickable = textContainer.Query<TextElement>().Class("clickable").Build();
        foreach (var i in clickable)
        {
            Debug.Log("clickable" + i.name);
            i.RegisterCallback<PointerDownEvent>(LoadMestery);

        }

    }

    public async Awaitable OpenRoom(float time, VisualElement visual)//지금 스프라이트 랜더러에서 값이 변경이 안되는듯함
    {
        var background = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("BackGround");
        float current = 0;
        float value_a = 255*0.2f;
        while (current < time)
        {
            await Awaitable.EndOfFrameAsync();
            current += Time.deltaTime / time;
            value_a = Mathf.Lerp(255, 0, current);
            Debug.Log("알파값" + value_a);
            background.style.backgroundColor = new Color(0, 0, 0, value_a/255f);
            //spriteRenderer.color.a= value_a;
        }
        background.style.backgroundColor = new Color(0,0,0, 0);
        visual.style.display = DisplayStyle.None;
        this.gameObject.SetActive(false);
    }

    public async Awaitable CloseRoom(float time,VisualElement visual)//지금 스프라이트 랜더러에서 값이 변경이 안되는듯함
    {
        //diary_button_parent.style.display = DisplayStyle.None;
        var background = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("BackGround");
        float current = 0;
        float value_a = 255;
        diary_button_parent.style.display = DisplayStyle.None;
        diary_button_parent.style.visibility = Visibility.Hidden;
        while (current < time)
        {
            await Awaitable.EndOfFrameAsync();
            current += Time.deltaTime / time;
            value_a = Mathf.Lerp(0, 255, current);
            Debug.Log("알파값" + value_a);
            background.style.backgroundColor = new Color(0, 0, 0, value_a / 255f);
            //spriteRenderer.color.a= value_a;
        }
        background.style.backgroundColor = new Color(0,0,0, 0.2f);
        visual.style.visibility = Visibility.Visible;

        visual.schedule.Execute(() =>
        {
            // 이 람다 표현식 안의 코드가 다음 UI 업데이트 주기(다음 프레임)에 실행됩니다.
            //Invoke("ButtonCreateTest", 0.3f);
            diary_button_parent.style.display = DisplayStyle.Flex;
            diary_button_parent.style.visibility = Visibility.Visible;
        }).ExecuteLater(300);//하드 코딩인데 타이임이 적당하게 맞는데?
        //diary_button_parent.visible = true;
    }

    private void ButtonCreateTest()
    {
        diary_button_parent.style.display = DisplayStyle.Flex;
        diary_button_parent.visible = true;
    }

    private void ButtonSet(VisualElement root)
    {
        inven_button_parent = root.Q<VisualElement>("button_parent");
        inven_left_button = root.Q<Button>("left_button");
        inven_right_button = root.Q<Button>("right_button");
        inven_left_button.clickable.clicked += InvenLeftButtonEvent;
        inven_right_button.clickable.clicked += InvenRightButtonEvent;
        inven_button_parent.visible = false;

        diary_button_parent = root.Q<VisualElement>("ButtonList");
        diary_button_back = root.Q<Button>("Back");
        diary_button_finsh = root.Q<Button>("Finsh");
        diary_button_back.clickable.clicked += DiaryBackButtonEvent;
        diary_button_finsh.clickable.clicked += DiaryFinishButtonEvent;
        diary_button_parent.style.display = DisplayStyle.None;
        diary_button_parent.style.visibility = Visibility.Hidden;
    }

    private void DiaryFinishButtonEvent()//정답 확인
    {
        if (ishome)
        {
            EndMeystery();
            return;
        }
        Debug.Log("일기 finish");

        List<string>strings = new List<string>();
        var container = mesteryContainer.Query<TextElement>(className: "dropArea");
        string pattern = @"\s*,\s*";
        string textWithoutQuotes = answerTexts[contentContextArrayIndex].Item1.Replace("\"", "").Trim();
        string hiddentextWithoutQuotes = answerTexts[contentContextArrayIndex].Item2.Replace("\"", "").Trim();
        string[] answer_strings = Regex.Split(textWithoutQuotes, pattern);
        string[] hidden_answer_strings= Regex.Split(hiddentextWithoutQuotes, pattern);

        foreach(string str in hidden_answer_strings)
        {
            Debug.Log("히드 정답 " + str);
        }
        int index = 0;
        foreach (var i in container.ToList())
        {
            Debug.Log("제출 답변" + i.text+"정답은 "+ answer_strings[index]);//범위 넘어갈때 문제 생김
            index++;
            strings.Add(i.text.Trim());
        }
        if (answerTexts[contentContextArrayIndex].Item1.Length <= 1)//문제가 아닐경우 해당 경우 마지막 라인 수정 필요
        {
            //&& contentContextArrayIndex <= contentContextArray.Length - 1
            Debug.Log("문제가 아님");
            contentContextArrayIndex++;
            if (contentContextArrayIndex > contentContextArray.Length - 1)//여기가 중요
            {
                Debug.Log("마지막 라인 종료");
                HomeDiary();
                //EndMeystery();
            }
            else
            {
                SetDiaryTextProblem();
            }

            return;
        }
        //bool is_correct = false;
        foreach(string str in strings)
        {
            Debug.Log("제출 정답 =>" + str);
        }
        foreach(string str in answer_strings)
        {
            Debug.Log("일반 정답 =>" + str);
        }
        foreach (string str in hidden_answer_strings)
        {
            Debug.Log("히든 정답 =>" + str);
        }
        if (answer_strings.SequenceEqual(strings.ToArray()))//정답일경우
        {
            //is_correct = true;
            contentContextArrayIndex++;
            if(contentContextArrayIndex<=contentContextArray.Length-1)
            {
                CorrectRespon();
                //Debug.Log("마지막 라인 종료");
                //textContainer.RemoveFromClassList("diary-left");
                //GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("TracerNote").RemoveFromClassList("test2-2");
                ////추가 필요 flex none
                //currentElement.RemoveFromClassList("clickable");
                //currentElement.AddToClassList("sentence");
                //currentElement.text = originText[contentContextArrayIndex];
                //textContainerContent.style.display = DisplayStyle.Flex;
                //mesteryContainer.style.display = DisplayStyle.None;
                ////EndMeystery();
                //return;
            }
            //is_hiddenmode = false;

        }else if (hidden_answer_strings.SequenceEqual(strings.ToArray()))
        {
            Debug.Log("히든 정답 맞춤");
            is_hiddenmode = true;
            if (contentContextArrayIndex <= contentContextArray.Length - 1)
            {
                CorrectRespon();
                //Debug.Log("마지막 라인 종료");
                //textContainer.RemoveFromClassList("diary-left");
                //GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("TracerNote").RemoveFromClassList("test2-2");
                ////추가 필요 flex none
                //currentElement.RemoveFromClassList("clickable");
                //currentElement.AddToClassList("sentence");
                //currentElement.text = originText[contentContextArrayIndex];
                //textContainerContent.style.display = DisplayStyle.Flex;
                //mesteryContainer.style.display = DisplayStyle.None;
                ////EndMeystery();
                //return;
            }
        }
        else
        {
            //is_correct = false;
            //textContainer.style.display = DisplayStyle.Flex;
            textContainer.Q<TextElement>("WrongAnswer").style.display = DisplayStyle.Flex;

            Invoke("WrongAnswerPopDown", 1);
            Debug.Log("오답");
        }

        if (contentContextArrayIndex > contentContextArray.Length-1 )//여기가 중요
        {
            Debug.Log("마지막 라인 종료");
            HomeDiary();
            //EndMeystery();
            return;
        }
    }
    private void HomeDiary()
    {
        ishome = true;
        textContainer.RemoveFromClassList("diary-left");
        GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("TracerNote").RemoveFromClassList("test2-2");
        //추가 필요 flex none
        currentElement.RemoveFromClassList("clickable");
        currentElement.AddToClassList("sentence");
        currentElement.text = originText[contentContextArrayIndex - 1];
        textContainerContent.style.display = DisplayStyle.Flex;
        mesteryContainer.style.display = DisplayStyle.None;
        currentElement.UnregisterCallback<PointerDownEvent>(LoadMestery);
    }

    private void WrongAnswerPopDown()
    {
        textContainer.Q<TextElement>("WrongAnswer").style.display = DisplayStyle.None;
    }

    private void DiaryBackButtonEvent()
    {
        Debug.Log("일기 back 버튼");
    }



    private void SetDiaryText(ref VisualElement textContainer, Dialogue[] dialogues)//diary csv (현재는 테스트파일2) 데이터를 가져와서 사용하는 부분
    {
        List<VisualElement> textList = new List<VisualElement>();

        bool isProblemCSV = false;

        List<string> contentContextList=new List<string>();

        Debug.Log("dialogue length" + dialogues.Length);
        for (int i = 0; i < dialogues.Length; i++)
        {
            //Debug.Log("LowIndex = " + dialogues[i].context.Length);
            for (int rowIndex = 0; rowIndex < dialogues[i].context.Length; rowIndex++)
            {
                //if (InventoryManager.Instance.GetInfo_(info_keys[i]).news.chapter != currentChapterNum) { continue; }
                VisualElement visuallist = new VisualElement { name = "textList" };
                visuallist.style.flexWrap = Wrap.Wrap;
                string contentContext = "";
                contentContext = dialogues[i].context[rowIndex];
                

                //mesteryContainer
                //string[] keys = InventoryManager.Instance.GetInfo_(info_keys[i]).keywords;
                string[] keys = InventoryManager.Instance.GetInfo_(int.Parse(dialogues[i].id)).keywords;
                if (dialogues[i].problem.Length > 0)//해당 부분없으면 다른 csv파일상에서 접근시 문제 생김
                {
                    isProblemCSV = true;
                    originText.Add(contentContext);
                    Debug.Log("row index count" + rowIndex);
                    Debug.Log("길이"+dialogues[i].problem[rowIndex].Length);
                    if (dialogues[i].problem[rowIndex].Length <= 1)//문제 빈칸일때//&& dialogues[i].mode[rowIndex] != "hidden"
                    {
                        Debug.Log("빈칸 부분");
                        Debug.Log("문제 부분 작동중 이지만 빈칸임" + dialogues[i].problem[rowIndex] + "모드" + dialogues[i].mode[rowIndex]);

                        answerTexts.Add(new Tuple<string,string> ( "",""));
                        contentContext = dialogues[i].context[rowIndex];
                    }
                    else//문제에 내용있을때
                    {
                        Debug.Log("문제 부분");
                        Debug.Log(string.Format("각 길이 확인{0} ||||| {1}", dialogues[i].problem.Length, dialogues[i].correct_answer.Length));
                        Debug.Log("문제 부분 작동 " + dialogues[i].problem[rowIndex] + "정답은 " + dialogues[i].correct_answer[rowIndex]);


                        //Debug.Log("히든 정답" + dialogues[i].hidden_answer[rowIndex] + "모드" + dialogues[i].mode[rowIndex]);//hidden정답 부분

                        contentContext = dialogues[i].problem[rowIndex];
                        Debug.Log("히든 정답" + dialogues[i].hidden_answer[rowIndex]);
                        //if (dialogues[i].hidden_answer[rowIndex])
                        answerTexts.Add(new Tuple<string, string>(dialogues[i].correct_answer[rowIndex], dialogues[i].hidden_answer[rowIndex]));
                    }

                    Debug.Log("contentContext =>" + contentContext);
                    if (dialogues[i].mode[rowIndex] == "hidden"&&hidden_start_index==-1)
                    {
                        hidden_start_index = rowIndex;
                        Debug.Log("hidden index is " + hidden_start_index);
                    }
                }
                else
                {
                    isProblemCSV = false;
                }

                contentContextList.Add(contentContext);
                if (isProblemCSV)
                {
                    //SetDairyTextProblem(visuallist, contentContext, keys);
                }
                else
                {
                    SetDairyTextNormal(visuallist, contentContext, keys, dialogues[i].id);
                    
                    textList.Add(visuallist);

                    visuallist.style.flexDirection = FlexDirection.Column;
                    visuallist.style.width = Length.Percent(100);
                    textList[i].style.flexDirection = FlexDirection.Column;
                }


                //Debug.Log(visuallist.childCount);    

                //textList.Add(visuallist);


                //textList[i].name = "TextList";
                textContainerContent.style.flexDirection = FlexDirection.Column;
                //var t = visuallist.Query<TextElement>().Build();
                //visuallist = new VisualElement();
            }
        }

        contentContextArray = contentContextList.ToArray();

        if (!isProblemCSV)
        {
            for (int number = 0; number < textList.Count; number++)
            {
                textContainerContent.Add(textList[number]);
            }
        }
        else
        {
            Debug.Log("textlist count is " + textList.Count);
            
            for (int number = 0; number < textList.Count; number++)
            {
                textContainerContent.Add(textList[number]);
                //mesteryContainer.Add(textList[number]);
            }
        }


        Debug.Log("answerTexts length " + answerTexts.Count +" , contentContextArray"+contentContextArray.Length);
    }

    private void CorrectRespon()
    {
        Debug.Log("히든 모드 체크" + is_hiddenmode);
        if (contentContextArrayIndex > contentContextArray.Length)
        {
            Debug.LogError("버튼 범위 벗어남");
            return;
        }



        VisualElement problem = currentProblem;
        problem.Clear();
        currentProblem.Clear();
        //problem.Clear();


        string str = originText[contentContextArrayIndex-1];//여기 문자열 교체하는 방식으로
        if (is_hiddenmode&&contentContextArrayIndex<=hidden_start_index)//hidden 처음 만났을때 번호로 이동하기위함
        {
            Debug.Log("hidden start index is " + hidden_start_index);
            contentContextArrayIndex = hidden_start_index+1;
            str = originText[contentContextArrayIndex-1];
        }


        string[] pSplits = str.Split(" ");
        foreach (string s in pSplits)
        {
            Debug.Log("문장 테스트 " + s + "길이 " + s.Length);
            if (s.Length == 0)
            {
                continue;
            }
            TextElement tElement = new TextElement { text = s, name = "TextElement" };
            tElement.style.whiteSpace = WhiteSpace.PreWrap;
            tElement.AddToClassList("sentence");
            currentProblem.Add(tElement);
        }

        SetDiaryTextProblem();
        Debug.Log("버튼 확인 " + contentContextArray[contentContextArrayIndex]);
    }

    private void SetDairyTextNormal(VisualElement visuallist, string contentContext, string[] keys,string id)
    {
        string[] parts = Regex.Split(contentContext, @"<br\s*/?>");//나눈 문장들 들어 있음
        //string[] keys = InventoryManager.Instance.GetInfo_(info_keys[i]).keywords;
        List<List<string>> tags = new List<List<string>>();//___으로 변환 되어있는 내용에서 해당 번째가 person인지 destination인지 확인용
        foreach (string part in parts)//br기준,사실상 없는거나 마찬가지
        {
            //Debug.Log("part is " + part+"id =>"+id+"key length"+keys.Length);
            //if (keys.Length < 1)
            //{
            //    keys = new string[] { "" };
            //}
            //Debug.Log("id=>" + id);
            //Debug.Log(string.Format("part is {0} , id => {1} key is {2}", part, id, keys[0]));
            string[] _part;
            _part = part.Split(' ');



            var textelement = new TextElement { text = part, name = "textelement" };



            foreach (string p in _part)//여기 드래그 관련 내용들은 csv가 아닌 수집품의 내용 관련으로 갈것임 지금 해당내용은 테스트용이라고 생각하는것이 좋음 띄워 쓰기 관련은 인벤토리(수집품) 추리시 발생, 스페이스 기준
            {
                bool check = keys.Length > 0
             && keys[0] != null
             && part != null
             && part.Contains(keys[0]);
                //int a = part.IndexOf(keys[0]);
                //Debug.Log($"분리 후: [{p}] check[{check}]"); //지금 분리도 안 되는거같은데

                if (check)
                {
                    Debug.Log(keys[0] + "Key 가지고 있음");
                    textelement.AddToClassList("clickable");
                    textelement.RegisterCallback<PointerDownEvent>(evt => {//여기서 수정 해 볼까
                        mesteryEventNum = int.Parse(id);
                        currentElement = textelement;
                        //Debug.Log("id" + id); 
                    });
                    textelement.RegisterCallback<PointerDownEvent>(LoadMestery);

                }
                else
                {
                    textelement.AddToClassList("sentence");
                }
                visuallist.Add(textelement);



            }
        }

    }
    private void SetDiaryTextProblem()
    {

        if ((hidden_start_index == contentContextArrayIndex))
        {
            contentContextArrayIndex = contentContextArray.Length;
            //this.gameObject.SetActive(false);
            mesteryContainer.RemoveFromClassList("diary-left");
            GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("TracerNote").RemoveFromClassList("test2-2");
            //EndMeystery();
            Debug.Log("정답 못 맞춘 상태 히든 부분");
            return;
        }
        else
        {
            Debug.Log(string.Format("hidden index{0} contentarrayindex{1}", hidden_start_index, contentContextArrayIndex));
        }
        VisualElement visuallist = new VisualElement { name = "visuallistLine"};//얘의 위치를 알아야함


        visuallist.AddToClassList("textOri");
        if (mesteryContainer.childCount >0)
        {
            visuallist.AddToClassList("textPos");
        }
       
        visuallist.style.flexWrap = Wrap.Wrap;
        //visuallist.style.position = Position.Absolute;//아래에서 relative로 해야함

        //string[] keys = InventoryManager.Instance.GetInfo_(info_keys[contentContextArrayIndex]).keywords;
        Debug.Log("contentContextArrayIndex" + contentContextArrayIndex);
        string contentContext = contentContextArray[contentContextArrayIndex];
        string[] parts = Regex.Split(contentContext, @"<br\s*/?>");//나눈 문장들 들어 있음
        List<List<string>> tags = new List<List<string>>();//___으로 변환 되어있는 내용에서 해당 번째가 person인지 destination인지 확인용
        foreach (string part in parts)//br기준,사실상 없는거나 마찬가지
        {
            Debug.Log("part is " + part);
            string[] _part;
            float estimatedLineHeightInPixels = 50f;
            visuallist.style.minHeight = new StyleLength(new Length(estimatedLineHeightInPixels, LengthUnit.Pixel));
            Debug.Log("part = " + part);
            _part = Regex.Split(part, "(!(?:[a-zA-Z]+))");
            MatchCollection matches = Regex.Matches(part, "(!([a-zA-Z]+))");//순서는 여기가 맞음, !XXXX패턴있는거 순서 찾는중
            tags.Add(new List<string>());
            foreach (var match in matches)
            {
                //Debug.Log(iter + "번째" + "매치된것은" + match.ToString());
                tags[0].Add(match.ToString());

            }


            /// <summary>
            /// 문제 ___있는 부분의 가로 열을 담는부분
            /// </summary>
            VisualElement problemElement = new VisualElement { name = "problemElementline" };
            //var textelement = new TextElement { text = part, name = "textelement" };

            foreach (string p in _part)//여기 드래그 관련 내용들은 csv가 아닌 수집품의 내용 관련으로 갈것임 지금 해당내용은 테스트용이라고 생각하는것이 좋음 띄워 쓰기 관련은 인벤토리(수집품) 추리시 발생, 스페이스 기준
            {
                //bool check = (keys.Length > 0 ? part.ContainsAny(keys[0]) : false);
                //Debug.Log($"분리 후: [{p}] check[{check}]"); //지금 분리도 안 되는거같은데


                Debug.Log("p =" + p);
                if (Regex.IsMatch(p, "(!([a-zA-Z]+))"))
                {
                    string replaceString = Regex.Replace(p, "(!([a-zA-Z]+))", "____");
                    Debug.Log(string.Format("before = {0} after {1}", p, replaceString));
                    TextElement underbarElement = new TextElement { text = replaceString, name = "underbarTextElement" };
                    underbarElement.AddToClassList("dropArea");
                    underbarElement.RegisterCallback<PointerUpEvent>(evt =>
                    {
                        if (is_sentence)
                        {
                            var querylabel = dragGhost.Query<TextElement>().Build();
                            Debug.Log("문제 위치에 놓았습니다 내용 =>" + querylabel.First().text);

                            underbarElement.text = querylabel.First().text;
                            currentProblem = underbarElement.parent;
                            string linestring = "";
                            foreach (var line in problemElement.Query<TextElement>().Build().ToList())//지금 정답과 동일해야한다고 생각했는데 정답만 맞추면 되는거 아닌가 싶음
                            {
                                linestring += line.text;
                            }
                            Debug.Log("정답 제출 텍스트" + linestring);
                        }
                    });
                    underbarElement.style.whiteSpace = WhiteSpace.PreWrap;
                    problemElement.Add(underbarElement);
                }
                else
                {
                    //띄워쓰기 기준으로 분리 해야함 안 그러면 너무 길어짐
                    string[] pSplits = p.Split(" ");
                    foreach (string s in pSplits)
                    {
                        Debug.Log("문장 테스트 " + s + "길이 " + s.Length);
                        if (s.Length == 0)
                        {
                            continue;
                        }
                        TextElement tElement = new TextElement { text = s, name = "TextElement" };
                        tElement.style.whiteSpace = WhiteSpace.PreWrap;
                        tElement.AddToClassList("sentence");
                        problemElement.Add(tElement);
                    }
                }
                problemElement.style.flexDirection = FlexDirection.Row;
                problemElement.style.flexWrap = Wrap.Wrap;
                visuallist.Add(problemElement);
            }
        }
        mesteryContainer.Add(visuallist);
        visuallist.schedule.Execute(() => {
            // 여기에 다음 프레임에 하고 싶은 동작
            visuallist.RemoveFromClassList("textPos");
        }).ExecuteLater(0);
        //visuallist.style.position = Position.Relative;

    }

    public void VisualElementChangeEvent(GeometryChangedEvent evt)//위치 확인해서 옮기는 용으로 사용하려했으나 필요없을듯?
    {
        VisualElement target = evt.target as VisualElement;
        if (target == null || target.name != "visuallistLine") return; // 직접 만든 visuallist가 맞는지 확인
        // textContainerContent 위치도 필요하다면 여기서 확인 (evt.target.parent 로 접근 가능)
        if (target.parent != null)
        {
            Debug.Log($"textContainerContent worldBound: {target.parent.worldBound}");
            Debug.Log($"textContainerContent layout: {target.parent.layout}");
        }
    }


    private Dialogue[] StartEvent(string title_str)//현재 가지고있는 아이템의 dialogue를 가져옴
    {
        Debug.Log("추리 이벤트 실행됨!");
        Dialogue[] tempdialogue = DatabaseManager.instance.theParser.Parse(title_str);//일지 파일 명으로 변경
        Debug.Log(tempdialogue.Length);
        DatabaseManager.instance.dialogueDic.Clear();
        Dictionary<int, Dialogue> trace_dic = new Dictionary<int, Dialogue>();//매번 할 필요없을거같긷한데 나중에 수정해도 될듯
        List<int> csv_id_list = new List<int>();
        bool is_problem = false;
        foreach (var dialogue in tempdialogue)
        {
            Debug.Log("dialogue id" + dialogue.id + "dialogue text" + dialogue.context[0]);
            trace_dic.Add(int.Parse(dialogue.id), dialogue);
            csv_id_list.Add(int.Parse(dialogue.id));
            if (dialogue.problem.Length >0)
            {
                Debug.Log("문제 길이" + dialogue.problem.Length);
                //Debug.Log("히든 정답" + dialogue.hidden_answer);
                is_problem = true;
            }
        }



        List<Dialogue> templist = new List<Dialogue>();
        foreach (var i in info_keys)//현재 가지고 있는 키에서 문자를 받을수있음
        {
            bool check = csv_id_list.Contains(i);
            print("chapter :" + InventoryManager.Instance.GetInfo_(i).news.chapter);
            check=check&&(InventoryManager.Instance.GetInfo_(i).news.chapter == currentChapterNum);//이 부분 들어가면 내용에도 chapter 동일해야 들어감
            if (!check)
            {
                Debug.Log("스크립트에 키 값이 없음 continue");
                continue;
            }
            templist.Add(trace_dic[i]);//아마 키가 달라서? 
        }
        if (is_problem)
        {
            Debug.Log("문제 파일 입니다");
            //Debug.Log()
            templist.Clear();
            templist.Add(trace_dic[mesteryEventNum]);
        }
        else
        {
            Debug.Log("일반 파일 입니다");
        }

        Dialogue[] dialogues = templist.ToArray();
        Debug.Log("제목"+title_str+"dialogue length" + dialogues.Length);
        for (int i = 0; i < dialogues.Length; i++)
        {
            Debug.Log("제목" + title_str + "LowIndex = " + dialogues[i].context.Length);
        }

        return dialogues;
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (is_drag)
        {
            //Debug.Log("드래그중");

            Vector2 currentMousePosition = evt.position;
            Vector2 mouseDelta = currentMousePosition - startMousePosition;
            if (dragGhost != null)
            {
                //Debug.Log("고스트 움직이는중");
                // 고스트는 절대 위치를 사용하므로, 마우스 델타만큼 직접 이동
                Vector2 newGhostPosition = visualElement.WorldToLocal(startMousePosition) + mouseDelta;

                dragGhost.style.left = newGhostPosition.x;
                dragGhost.style.top = newGhostPosition.y;

                //Debug.Log(string.Format("고스트 left {0} top {1}", dragGhost.style.left, dragGhost.style.top));
            }
        }
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        if (!is_sentence) return;
        is_drag = true;
        Debug.Log("클릭위치" + evt.position);
        //evt.
        if (dragGhost == null)//고스트( 오브젝트 생성)
        {
            dragGhost = new Label(dragGhostName);
            dragGhost.style.fontSize = visualElement.resolvedStyle.fontSize;
            dragGhost.style.color = UnityEngine.Color.red;
            dragGhost.style.unityFont = visualElement.resolvedStyle.unityFont;
            dragGhost.style.unityFontStyleAndWeight = visualElement.resolvedStyle.unityFontStyleAndWeight;
            dragGhost.style.position = Position.Absolute; // 절대 위치 사용
            dragGhost.style.opacity = 0.7f; // 반투명하게
            dragGhost.pickingMode = PickingMode.Ignore; // 고스트는 이벤트 받지 않음
            dragGhost.style.fontSize = 25;
            // 초기 위치 설정 


            visualElement.Add(dragGhost); // 루트에 추가하여 다른 UI 위에 표시
        }
        Vector2 ghostStartPosition = evt.position; // 스크린/월드 좌표


        ghostStartPosition = visualElement.WorldToLocal(ghostStartPosition); // 루트 요소 기준 로컬 좌표로 변환
        dragGhost.style.translate = new Translate(Length.Percent(-50), Length.Percent(-50));//가운데 정렬
        dragGhost.style.left = ghostStartPosition.x;
        dragGhost.style.top = ghostStartPosition.y;

        startMousePosition = evt.position;
    }

    private void LoadMestery(PointerDownEvent evt)//데이터를 미리 정해놔야할듯?
    {
        //textContainerContent.Clear();
        ishome = false;
        textContainerContent.style.display = DisplayStyle.None;
        mesteryContainer.style.display = DisplayStyle.Flex;
        DiaryContentSet();
        MesterySystem();
        SetDiaryTextProblem();
    }
    private void DiaryContentSet()
    {

        //Dialogue[] dialyDialogue = StartEvent("다이어리 내용");
        SetDiaryText(ref textContainer, StartEvent("Reasoning_0.001"));


    }

    private void MesterySystem()
    {
        inven_button_parent.visible = true;
        diary_button_parent.visible = true;
        var root = GetComponent<UIDocument>().rootVisualElement;
        var panel = root.Q<VisualElement>("TracerNotePanel");//사용안함
        panel.style.scale = new Vector2(1, 1);
        var tracer = root.Q<VisualElement>("TracerNote");//여기 하위에 오브젝트 배치
        var content = root.Q<VisualElement>("TracerContent");

        var title_element = tracer.Q<Label>("Title");
        var charactor_image = tracer.Q<VisualElement>("TracerImage");
        content.Clear();

        //content.style.flexDirection = FlexDirection.Column;
        //content.style.flexWrap = Wrap.NoWrap;
        content.style.flexGrow = 1;
        content.style.flexShrink = 0;

        VisualElement visuallist = new VisualElement();
        //Inventory.Info info = InventoryManager.Instance.GetInfo_(info_keys[num]);
        Debug.Log("inven =" + InventoryManager.Instance.GetInfo_(info_keys[num]).context+" ||||"
            +"correction Num" + InventoryManager.Instance.GetInfo_(info_keys[num]).news.chapter??"0");

        bool is_same_chapter = InventoryManager.Instance.GetInfo_(info_keys[num]).news.chapter==currentChapterNum ? true : false;
        string textContent = InventoryManager.Instance.GetInfo_(info_keys[num]).context ?? "";
        string[] keys = InventoryManager.Instance.GetInfo_(info_keys[num]).keywords ?? new string[0];
        string[] parts = Regex.Split(textContent, @"(\n)");

        title_element.text = InventoryManager.Instance.GetInfo_(info_keys[num]).news.image_name;
        charactor_image.style.backgroundImage = new StyleBackground(InventoryManager.Instance.GetInfo_(info_keys[num]).news.image);
        foreach (string part in parts)
        {
            VisualElement lineContainer = new VisualElement();

            lineContainer.style.flexDirection = FlexDirection.Row;
            lineContainer.style.flexWrap = Wrap.Wrap;//이게 들어가야 줄 이상한거 없어짐
            //float estimatedLineHeightInPixels = 50f;
            //lineContainer.style.minHeight = new StyleLength(new Length(estimatedLineHeightInPixels, LengthUnit.Pixel));
            //string[] _part = part.Split(' ');
            string pattern = @"""([^""]*)""|([^""\s]+)";
            var matches = Regex.Matches(part, pattern);
            string[] _part = matches.Cast<Match>().Select(m => m.Value).ToList().ToArray();
            //string[] _part = Regex.Replace(part, @"(?<![""])\s+(?![^""]*[""])", "");
            foreach (string p in _part)//여기 드래그 관련 내용들은 csv가 아닌 수집품의 내용 관련으로 갈것임 지금 해당내용은 테스트용이라고 생각하는것이 좋음 띄워 쓰기 관련은 인벤토리(수집품) 추리시 발생
            {
                //여기를 수정해야할듯?
                bool check = false;
                string key = "";
                foreach (string k in keys)
                {
                    Debug.Log("Key = " + k);
                    check = keys.Length > 0 && p != null && k != null && p.Contains(k);

                    if (check)
                    {
                        key = k;
                        break;
                    }

                }

                string p_str = p + " ";
                Debug.Log($"분리 후: [{p_str}] check[{check}]"); //지금 분리도 안 되는거같은데
                var textelement = new TextElement { text = p_str, name = "textelement" };
                textelement.style.whiteSpace = WhiteSpace.PreWrap;
                lineContainer.Add(textelement);
                if (check&&is_same_chapter)
                {

                    textelement.RegisterCallback<PointerDownEvent>(evt =>
                    {
                        is_sentence = true;
                        dragGhostName = key;
                    });//여기에 문구 변경
                    textelement.RegisterCallback<PointerMoveEvent>(evt =>
                    { //Debug.Log("label 드래그 확인 문구");
                    });
                    textelement.RegisterCallback<PointerUpEvent>(evt =>
                    { //Debug.Log("label 클릭 놓은 확인 문구"); 
                    });
                    textelement.AddToClassList("draggable");
                }
                else
                {
                    textelement.AddToClassList("sentence");
                }
            }

            content.Add(lineContainer);
        }


        //drop_area.style.color = UnityEngine.Color.white;

        textContainer.style.flexGrow = 0;

        textContainer.AddToClassList("diary-left");
        //tracer.AddToClassList("test1");
        tracer.AddToClassList("test2-2");
        tracer.style.marginRight = Length.Percent(2);
    }

    public void InvenLeftButtonEvent()
    {
        num--;
        if (num < 0)
        {
            num = 0;
        }
        MesterySystem();
        Debug.Log("왼쪽 버튼 눌림");
    }

    public void InvenRightButtonEvent()
    {
        num++;
        if (inventoryScripable.inventory.Count <= num)
        {
            num = inventoryScripable.inventory.Count - 1;
        }
        MesterySystem();
        Debug.Log("오른쪽 버튼 눌림");
    }

    private async void EndMeystery()
    {
        Debug.Log("end");
        await OpenRoom(0.7f, GetComponent<UIDocument>().rootVisualElement);
    }
}

