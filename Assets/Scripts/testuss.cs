using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static Inventory;
using static UnityEditor.Recorder.OutputPath;

public class UI_DynamicText : MonoBehaviour
{

    VisualElement visualElement;//메인 부분
    VisualElement dragGhost = null;
    VisualElement textContainer;//일기
    VisualElement textContainerContent;//일기의 내용을 담고 있음

    VisualElement button_parent;//버튼들을 담고있는 객체
    Button left_button;
    Button right_button;

    VisualElement diary;

    VisualElement drop_area;
    string dragGhostName = "";

    private List<int> info_keys = new List<int>();
    bool is_drag = false;

    bool is_sentence = false;

    Vector2 startMousePosition;
    InventoryScripable inventoryScripable;

    int num = 0;

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

        button_parent = root.Q<VisualElement>("button_parent");
        left_button = root.Q<Button>("left_button");
        right_button = root.Q<Button>("right_button");
        left_button.clickable.clicked += LeftButtonEvent;
        right_button.clickable.clicked += RightButtonEvent;
        button_parent.visible = false;

        var text1 = new TextElement { text = "클릭 이벤트 가능한 문장 1 ", name = "sentence1" };
        var clickableText = new TextElement { text = "클릭 이벤트 가능한 문장 2", name = "clickableWord" };
        var text2 = new TextElement { text = "클릭 이벤트 가능한 문장 3", name = "sentence2" };

        // 클릭 이벤트 추가
        List<TextElement> textList = new List<TextElement>();//이벤트가 들어가야하는 내용들
        textList.Add(text1); textList.Add(text2); textList.Add(clickableText);
        SetDiaryText(ref textContainer,StartEvent("테스트파일2"));
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



        drop_area = visualElement.Q<VisualElement>("drop-area");
        
        drop_area.RegisterCallback<PointerUpEvent>(evt =>
        {
            if (is_sentence)
            {
                var querylabel = dragGhost.Query<Label>().Build();
                //querylabel.First().text//오브젝트 내용 적혀있음
                Debug.Log("테스트 위치에 놓았습니다 내용 =>" + querylabel.First().text);
            }
        });

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

    private void Clickable_clicked()
    {
        throw new NotImplementedException();
    }

    private void SetDiaryText(ref VisualElement textContainer, Dialogue[] dialogues)//diary csv (현재는 테스트파일2) 데이터를 가져와서 사용하는 부분
    {
        List<VisualElement> textList = new List<VisualElement>();
        //Dialogue[] dialogues = StartEvent(fileName);



        VisualElement visuallist = new VisualElement();
        visuallist.style.flexWrap = Wrap.Wrap;
        //int num = 0;//사용안되고 있다고 나옴 아마 위에 새로 선언한 num변수 때문인듯?
        bool isProblemCSV = false;
        for (int i = 0; i < dialogues.Length; i++)
        {

            for(int rowIndex = 0; rowIndex < dialogues[i].context.Length; rowIndex++)
            {
                string contentContext = "";
                contentContext=dialogues[i].context[rowIndex];
                string answerText = "";
                //string[] parts = Regex.Split(dialogues[i].context[rowIndex], @"<br\s*/?>");//나눈 문장들 들어 있음
                if (dialogues[i].problem.Length > 0)//해당 부분없으면 다른 csv파일상에서 접근시 문제 생김
                {
                    isProblemCSV = true;
                    if (dialogues[i].problem[rowIndex].Length <= 1)//문제 빈칸일때
                    {
                        //Debug.Log("문제 부분 작동중 이지만 빈칸임" + dialogues[i].problem[rowIndex][0]);
                    }
                    else//문제에 내용있을때
                    {
                        Debug.Log("문제 부분 작동 " + dialogues[i].problem[rowIndex]+"정답은 "+ dialogues[i].correct_answer[rowIndex]);
                        contentContext = dialogues[i].problem[rowIndex];
                        answerText = dialogues[i].correct_answer[rowIndex];
                    }

                }
                else
                {
                    isProblemCSV = false;
                }

                //Regex.Replace()
                string[] parts = Regex.Split(contentContext, @"<br\s*/?>");//나눈 문장들 들어 있음
                string[] keys = InventoryManager.Instance.GetInfo_(info_keys[i]).keywords;
                List<List<string>> tags = new List<List<string>>();//___으로 변환 되어있는 내용에서 해당 번째가 person인지 destination인지 확인용
                int iter = 0;
                foreach (string part in parts)//br기준,사실상 없는거나 마찬가지
                {
                    Debug.Log("part is "+part);
                    string[] _part;


                    if (isProblemCSV)//여기 부분은 추리가 있는 csv부분
                    {
                        float estimatedLineHeightInPixels = 50f;
                        visuallist.style.minHeight = new StyleLength(new Length(estimatedLineHeightInPixels, LengthUnit.Pixel));
                        Debug.Log("part = " + part);
                        _part = Regex.Split(part, "(!(?:[a-zA-Z]+))");
                        MatchCollection matches = Regex.Matches(part, "(!([a-zA-Z]+))");//순서는 여기가 맞음, !XXXX패턴있는거 순서 찾는중
                        tags.Add(new List<string>());   
                        foreach (var match in matches)
                        {
                            Debug.Log(iter + "번째" + "매치된것은" + match.ToString());
                            tags[0].Add(match.ToString());
         
                        }
                        iter++;
                    }
                    else
                    {
                        _part = part.Split(' ');
                    }
                    VisualElement problemElement = new VisualElement();
                    var textelement = new TextElement { text = part, name = "textelement" };

                    foreach (string p in _part)//여기 드래그 관련 내용들은 csv가 아닌 수집품의 내용 관련으로 갈것임 지금 해당내용은 테스트용이라고 생각하는것이 좋음 띄워 쓰기 관련은 인벤토리(수집품) 추리시 발생, 스페이스 기준
                    {
                        bool check = (keys.Length > 0 ? part.ContainsAny(keys[0]) : false);
                        //int a = part.IndexOf(keys[0]);
                        Debug.Log($"분리 후: [{p}] check[{check}]"); //지금 분리도 안 되는거같은데
                        if (!isProblemCSV)
                        {
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
                        else
                        {
                            Debug.Log("p =" + p);
                            //string temp = Regex.Replace(p, "(!([a-zA-Z]+))", "_____");
                            //Debug.Log("temp = " + temp);
                            //string[] tempArray = Regex.Split(temp, "_____");
                            if(Regex.IsMatch(p, "(!([a-zA-Z]+))"))
                            {
                                string replaceString = Regex.Replace(p, "(!([a-zA-Z]+))", "____");
                                Debug.Log(string.Format("before = {0} after {1}", p,replaceString));
                                TextElement underbarElement = new TextElement { text = replaceString, name = "underbarTextElement"};
                                //underbarElement.style.whiteSpace = WhiteSpace.PreWrap;
                                underbarElement.AddToClassList("dropArea");
                                underbarElement.RegisterCallback<PointerUpEvent>(evt =>
                                {
                                    if (is_sentence)
                                    {
                                        var querylabel = dragGhost.Query<TextElement>().Build();
                                        //querylabel.First().text//오브젝트 내용 적혀있음
                                        Debug.Log("문제 위치에 놓았습니다 내용 =>" + querylabel.First().text);
                                        underbarElement.text=querylabel.First().text;   
                                    }
                                });
                                //textelement = underbarElement;
                                underbarElement.style.whiteSpace = WhiteSpace.PreWrap;
                                problemElement.Add(underbarElement);
                            }
                            else
                            {
                                //띄워쓰기 기준으로 분리 해야함 안 그러면 너무 길어짐
                                string[] pSplits = p.Split(" ");
                                foreach(string s in pSplits)
                                {
                                    TextElement tElement = new TextElement { text = s, name = "TextElement" };
                                    tElement.style.whiteSpace = WhiteSpace.PreWrap;
                                    tElement.AddToClassList("sentence");
                                    //textelement.text = p;
                                    //tElement.style.whiteSpace = WhiteSpace.PreWrap;
                                    problemElement.Add(tElement);
                                }
                                //TextElement tElement = new TextElement { text = p, name = "TextElement" };
                                //tElement.AddToClassList("sentence");
                                ////textelement.text = p;
                                ////tElement.style.whiteSpace = WhiteSpace.PreWrap;
                                //problemElement.Add(tElement);
                            }

                   


                            problemElement.style.flexDirection = FlexDirection.Row;
                            problemElement.style.flexWrap = Wrap.Wrap;
                            visuallist.Add(problemElement);
                            //textelement.text = temp;    
                        }


                    }

                    //Debug.Log("text = " + textelement.text);
                    //
                    //textelement.style.width = Length.Percent(100);
                }

                int first = 0;
                foreach(var tag in tags)
                {
                    int second = 0;

                    foreach (var text in tag)
                    {
                        Debug.Log(string.Format("{0} {1}tag 내용 = {2}",first,second,text) );
                        second++;
                    }
                    first++;
                }

                //Debug.Log(visuallist.childCount);    
                textList.Add(visuallist);


                visuallist.style.flexDirection = FlexDirection.Column;
                visuallist.style.width = Length.Percent(100);
                textList[i].style.flexDirection = FlexDirection.Column;
                textContainerContent.style.flexDirection = FlexDirection.Column;
                var t = visuallist.Query<TextElement>().Build();
                visuallist = new VisualElement();
            }
        }
        //Debug.Log("textlist count is " + textList.Count);//textelement개수 확인용
        for(int number = 0; number < textList.Count; number++)
        {
            //if (number == 0)
            //{
            //    textList[number].Q<TextElement>().text = "가장 처음입니다 테스트용입니다";//이런식으로 변경
            //}
            textContainerContent.Add(textList[number]);
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
            bool check=csv_id_list.Contains(i);
            if (!check)
            {
                Debug.Log("스크립트에 키 값이 없음 continue");
                continue;
            }
            templist.Add(trace_dic[i]);//아마 키가 달라서? 
        }
        Dialogue[] dialogues = templist.ToArray();

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
        dragGhost.style.left = ghostStartPosition.x;
        dragGhost.style.top = ghostStartPosition.y;
        Debug.Log(string.Format("{0} 넓기 {1}높이",dragGhost.style.width,dragGhost.style.height));
        startMousePosition = evt.position;
    }

    private void LoadMestery(PointerDownEvent evt)//데이터를 미리 정해놔야할듯?
    {
        textContainerContent.Clear();
        DiaryContentSet();
        MesterySystem();
    }
    private void DiaryContentSet()
    {

        //Dialogue[] dialyDialogue = StartEvent("다이어리 내용");
        SetDiaryText(ref textContainer, StartEvent("다이어리 내용"));


    } 

    private void MesterySystem()
    {
        button_parent.visible = true;
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
                bool check = (keys.Length > 0 ? p.ContainsAny(keys[0]) : false);
                string p_str = p + " ";
                Debug.Log($"분리 후: [{p_str}] check[{check}]"); //지금 분리도 안 되는거같은데
                var textelement = new TextElement { text = p_str, name = "textelement" };
                textelement.style.whiteSpace = WhiteSpace.PreWrap;
                lineContainer.Add(textelement);
                if (check)
                {
                    
                    textelement.RegisterCallback<PointerDownEvent>(evt => { 
                        is_sentence = true;
                        dragGhostName = keys[0];
                    });//여기에 문구 변경
                    textelement.RegisterCallback<PointerMoveEvent>(evt => { //Debug.Log("label 드래그 확인 문구");
                                                                            });
                    textelement.RegisterCallback<PointerUpEvent>(evt => { //Debug.Log("label 클릭 놓은 확인 문구"); 
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


        drop_area.style.color = UnityEngine.Color.white;

        textContainer.style.flexGrow = 0;

        textContainer.AddToClassList("diary-left");
        //tracer.AddToClassList("test1");
        tracer.AddToClassList("test2-2");
    }

    public void LeftButtonEvent()
    {
        num--;
        if (num < 0)
        {
            num = 0;
        }
        MesterySystem();
        Debug.Log("왼쪽 버튼 눌림");
    }

    public void RightButtonEvent()
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

