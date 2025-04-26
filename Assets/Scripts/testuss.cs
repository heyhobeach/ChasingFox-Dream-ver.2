
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text.RegularExpressions;

using UnityEditor.Rendering;

using UnityEngine;

using UnityEngine.UIElements;


public class UI_DynamicText : MonoBehaviour
{

    VisualElement visualElement;//메인 부분
    VisualElement dragGhost = null;
    VisualElement textContainer;//일기
    VisualElement textContainerContent;//일기의 내용을 담고 있음

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

    private List<int> info_keys = new List<int>();
    bool is_drag = false;

    bool is_sentence = false;

    Vector2 startMousePosition;
    InventoryScripable inventoryScripable;

    int num = 0;

    string SPLIT_COMMAND_PASER = @"[""!,]";

    string[] contentContextArray;
    int contentContextArrayIndex = 0;
    List<string>answerTexts=new List<string>();
    List<string>originText=new List<string>();

    private void OnEnable()
    {


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
        visualElement.Add(diary);


        ButtonSet(root);


        var text1 = new TextElement { text = "클릭 이벤트 가능한 문장 1 ", name = "sentence1" };
        var clickableText = new TextElement { text = "클릭 이벤트 가능한 문장 2", name = "clickableWord" };
        var text2 = new TextElement { text = "클릭 이벤트 가능한 문장 3", name = "sentence2" };

        // 클릭 이벤트 추가
        List<TextElement> textList = new List<TextElement>();//이벤트가 들어가야하는 내용들
        textList.Add(text1); textList.Add(text2); textList.Add(clickableText);
        SetDiaryText(ref textContainer, StartEvent("테스트파일2"));
        //ui toolkit에서 제공하는 함수로 이벤트 등록에 사용됨
        foreach (var text in textList)
        {
            text.RegisterCallback<PointerUpEvent>(even =>
            {
                Debug.Log("hello");
            });
        }
        clickableText.RegisterCallback<PointerUpEvent>(evt =>
        {
            Debug.Log("클릭된 텍스트: 클릭 가능한 텍스트");


        });



        //drop_area = visualElement.Q<VisualElement>("drop-area");
        //
        //drop_area.RegisterCallback<PointerUpEvent>(evt =>
        //{
        //    if (is_sentence)
        //    {
        //        var querylabel = dragGhost.Query<Label>().Build();
        //        //querylabel.First().text//오브젝트 내용 적혀있음
        //        Debug.Log("테스트 위치에 놓았습니다 내용 =>" + querylabel.First().text);
        //    }
        //});

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
        diary_button_parent.visible = false;
    }

    private void DiaryFinishButtonEvent()//정답 확인
    {
        Debug.Log("일기 finish");
        List<string>strings = new List<string>();
        var container = textContainerContent.Query<TextElement>(className: "dropArea");
        string pattern = @"\s*,\s*";
        string textWithoutQuotes = answerTexts[contentContextArrayIndex].Replace("\"", "").Trim();
        string[] answer_strings = Regex.Split(textWithoutQuotes, pattern);

        int index = 0;
        foreach (var i in container.ToList())
        {
            Debug.Log("제출 답변" + i.text+"정답은 "+ answer_strings[index]);//범위 넘어갈때 문제 생김
            index++;
            strings.Add(i.text.Trim());
        }
        if (answerTexts[contentContextArrayIndex].Length <= 1)
        {
            contentContextArrayIndex++;
            SetDiaryTextProblem();
            return;
        }
        bool is_correct = false;
        if (answer_strings.SequenceEqual(strings.ToArray()))//정답일경우
        {
            is_correct = true;
            contentContextArrayIndex++;
            CorrectRespon();
        }
        else
        {
            is_correct = false;
            Debug.Log("오답");
        }
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

        //Debug.Log("dialogue length" + dialogues.Length);
        for (int i = 0; i < dialogues.Length; i++)
        {
            //Debug.Log("LowIndex = " + dialogues[i].context.Length);
            for (int rowIndex = 0; rowIndex < dialogues[i].context.Length; rowIndex++)
            {
                VisualElement visuallist = new VisualElement { name = "textList" };
                visuallist.style.flexWrap = Wrap.Wrap;
                string contentContext = "";
                contentContext = dialogues[i].context[rowIndex];


                string[] keys = InventoryManager.Instance.GetInfo_(info_keys[i]).keywords;
                if (dialogues[i].problem.Length > 0)//해당 부분없으면 다른 csv파일상에서 접근시 문제 생김
                {
                    isProblemCSV = true;
                    originText.Add(contentContext);
                    if (dialogues[i].problem[rowIndex].Length <= 1)//문제 빈칸일때
                    {
                        //Debug.Log("문제 부분 작동중 이지만 빈칸임" + dialogues[i].problem[rowIndex][0]);

                        answerTexts.Add("");
                    }
                    else//문제에 내용있을때
                    {
                        Debug.Log("문제 부분 작동 " + dialogues[i].problem[rowIndex] + "정답은 " + dialogues[i].correct_answer[rowIndex]);
                        contentContext = dialogues[i].problem[rowIndex];
                        answerTexts.Add( dialogues[i].correct_answer[rowIndex]);
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
                    SetDairyTextNormal(visuallist, contentContext, keys);
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
        //RefactoryingTest();

        for (int number = 0; number < textList.Count; number++)
        {
            textContainerContent.Add(textList[number]);
        }
        Debug.Log("answerTexts length " + answerTexts.Count +" , contentContextArray"+contentContextArray.Length);
    }

    private void CorrectRespon()
    {
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

    private void SetDairyTextNormal(VisualElement visuallist, string contentContext, string[] keys)
    {
        string[] parts = Regex.Split(contentContext, @"<br\s*/?>");//나눈 문장들 들어 있음
        //string[] keys = InventoryManager.Instance.GetInfo_(info_keys[i]).keywords;
        List<List<string>> tags = new List<List<string>>();//___으로 변환 되어있는 내용에서 해당 번째가 person인지 destination인지 확인용
        foreach (string part in parts)//br기준,사실상 없는거나 마찬가지
        {
            Debug.Log("part is " + part);
            string[] _part;
            _part = part.Split(' ');


            var textelement = new TextElement { text = part, name = "textelement" };

            foreach (string p in _part)//여기 드래그 관련 내용들은 csv가 아닌 수집품의 내용 관련으로 갈것임 지금 해당내용은 테스트용이라고 생각하는것이 좋음 띄워 쓰기 관련은 인벤토리(수집품) 추리시 발생, 스페이스 기준
            {
                bool check = (keys.Length > 0 ? part.ContainsAny(keys[0]) : false);
                //int a = part.IndexOf(keys[0]);
                Debug.Log($"분리 후: [{p}] check[{check}]"); //지금 분리도 안 되는거같은데

                if (check)
                {
                    textelement.AddToClassList("clickable");
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

        VisualElement visuallist = new VisualElement { name = "visuallistLine"};//얘의 위치를 알아야함
        //visuallist.AddToClassList("textOri");
        //visuallist.AddToClassList("textPos");
       
        visuallist.style.flexWrap = Wrap.Wrap;
        //visuallist.style.position = Position.Absolute;//아래에서 relative로 해야함

        //string[] keys = InventoryManager.Instance.GetInfo_(info_keys[contentContextArrayIndex]).keywords;
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
        //visuallist.style.translate = new Translate(0, 50);
        visuallist.RegisterCallback<GeometryChangedEvent>(VisualElementChangeEvent);
        textContainerContent.Add(visuallist);
        //visuallist.RemoveFromClassList("textPos"); 
        Debug.Log(string.Format("visualist position {0} {1} {2} ",visuallist.worldBound.position,visuallist.layout.height,visuallist.worldBound.y));
        Debug.Log(string.Format("textContainerContent position {0} {1} {2} ", textContainerContent.worldBound.position, textContainerContent.layout.height, textContainerContent.worldBound.y));

        //visuallist.style.position = Position.Relative;

    }

    public void VisualElementChangeEvent(GeometryChangedEvent evt)
    {
        VisualElement target = evt.target as VisualElement;
        if (target == null || target.name != "visuallistLine") return; // 직접 만든 visuallist가 맞는지 확인

        // 이 시점에서는 레이아웃 계산이 완료되었습니다.
        Debug.Log("--- GeometryChangedEvent 발생 ---");
        Debug.Log($"visuallist worldBound: {target.worldBound}");
        Debug.Log($"visuallist layout: {target.layout}");
        Debug.Log($"visuallist worldBound.position: {target.worldBound.position}");
        Debug.Log($"visuallist layout.position: {target.layout.position}");//이거 기준으로 해야할듯?
        Debug.Log($"visuallist layout.height: {target.layout.height}");

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
        foreach (var dialogue in tempdialogue)
        {
            Debug.Log("dialogue id" + dialogue.id + "dialogue text" + dialogue.context[0]);
            trace_dic.Add(int.Parse(dialogue.id), dialogue);
            csv_id_list.Add(int.Parse(dialogue.id));
        }

        List<Dialogue> templist = new List<Dialogue>();
        foreach (var i in info_keys)//현재 가지고 있는 키에서 문자를 받을수있음
        {
            bool check = csv_id_list.Contains(i);
            if (!check)
            {
                Debug.Log("스크립트에 키 값이 없음 continue");
                continue;
            }
            templist.Add(trace_dic[i]);//아마 키가 달라서? 
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
        textContainerContent.Clear();
        DiaryContentSet();
        MesterySystem();
        SetDiaryTextProblem();
    }
    private void DiaryContentSet()
    {

        //Dialogue[] dialyDialogue = StartEvent("다이어리 내용");
        SetDiaryText(ref textContainer, StartEvent("다이어리 내용"));


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
        content.Clear();

        //content.style.flexDirection = FlexDirection.Column;
        //content.style.flexWrap = Wrap.NoWrap;
        content.style.flexGrow = 1;
        content.style.flexShrink = 0;

        VisualElement visuallist = new VisualElement();
        Debug.Log("inven =" + InventoryManager.Instance.GetInfo_(info_keys[num]).context);
        string textContent = InventoryManager.Instance.GetInfo_(info_keys[num]).context ?? "";
        string[] keys = InventoryManager.Instance.GetInfo_(info_keys[num]).keywords ?? new string[0];
        string[] parts = Regex.Split(textContent, @"(\n)");
        foreach (string part in parts)
        {
            VisualElement lineContainer = new VisualElement();

            lineContainer.style.flexDirection = FlexDirection.Row;
            lineContainer.style.flexWrap = Wrap.Wrap;//이게 들어가야 줄 이상한거 없어짐
            //float estimatedLineHeightInPixels = 50f;
            //lineContainer.style.minHeight = new StyleLength(new Length(estimatedLineHeightInPixels, LengthUnit.Pixel));
            string[] _part = part.Split(' ');

            foreach (string p in _part)//여기 드래그 관련 내용들은 csv가 아닌 수집품의 내용 관련으로 갈것임 지금 해당내용은 테스트용이라고 생각하는것이 좋음 띄워 쓰기 관련은 인벤토리(수집품) 추리시 발생
            {
                //여기를 수정해야할듯?
                bool check = false;
                string key = "";
                foreach (string k in keys)
                {
                    Debug.Log("Key = " + k);
                    check = (keys.Length > 0 ? p.ContainsAny(k) : false);
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
                if (check)
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
}

