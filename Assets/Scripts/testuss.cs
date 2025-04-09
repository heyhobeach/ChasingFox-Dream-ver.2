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
using static UnityEditor.Recorder.OutputPath;

public class UI_DynamicText : MonoBehaviour
{

    VisualElement visualElement;//메인 부분
    VisualElement dragGhost = null;
    VisualElement textContainer;
    VisualElement textContainerContent;

    VisualElement diary;

    VisualElement drop_area;

    private List<int> info_keys = new List<int>();
    bool is_drag = false;

    bool is_sentence = false;

    Vector2 startMousePosition;
    InventoryScripable inventoryScripable;


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

        // 기존 요소 제거 (초기화)
        //textContainer.Clear();

        // 새로운 텍스트 요소 추가
        //Dialogue[] dialogues=InteractionEvent.Instance.GetDialogue();
        //
        //foreach(var i in dialogues)
        //{
        //    Debug.Log(i);
        //}

        var text1 = new TextElement { text = "클릭 이벤트 가능한 문장 1 ", name = "sentence1" };
        var clickableText = new TextElement { text = "클릭 이벤트 가능한 문장 2", name = "clickableWord" };
        var text2 = new TextElement { text = "클릭 이벤트 가능한 문장 3", name = "sentence2" };

        // 클릭 이벤트 추가
        List<TextElement> textList = new List<TextElement>();//이벤트가 들어가야하는 내용들
        textList.Add(text1); textList.Add(text2); textList.Add(clickableText);
        SetDiaryText(ref textContainer);
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
            //textList.Clear();//초기화 
            //Dialogue[] dialogues=StartEvent();
            //foreach (var dialogue in dialogues)
            //{
            //    string[] parts = Regex.Split(dialogue.context[0], @"<br\s*/?>");
            //    foreach(string part in parts)
            //    {
            //        Debug.Log($"분리 후: [{part}]");
            //        textList.Add(new TextElement { text = part, name = "textelement" });
            //    }
            //}
            //for (int i = 0; i < textList.Count; i++)
            //{
            //    textContainer.Add(textList[i]);
            //}


        });

        //foreach(var text in textList)
        //{
        //    textContainer.Add(text);
        //}


        // 컨테이너에 추가
        //textContainer.Add(text1);
        //textContainer.Add(clickableText);
        //textContainer.Add(text2);

        //VisualElement sentence_container = root.Q<VisualElement>("sentence-container");
        //var draggable_label = sentence_container.Query<Label>().Class("draggable").Build();

        drop_area = visualElement.Q<VisualElement>("drop-area");

        drop_area.RegisterCallback<PointerUpEvent>(evt =>
        {
            if (is_sentence)
            {
                var querylabel = dragGhost.Query<Label>().Build();
                //querylabel.First().text//오브젝트 내용 적혀있음
                Debug.Log("위치에 놓았습니다 내용 =>" + querylabel.First().text);
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

        //var tracer = root.Q<VisualElement>("TracerNote");
        //tracer.AddToClassList("test2-2");
        var panel = root.Q<VisualElement>("TracerNotePanel");
        panel.style.scale = new Vector2(0, 0);
        //panel.style.bottom = 100;
        //panel.RemoveFromHierarchy();
        //panel.Clear();


        //foreach (var dlable in draggable_label)
        //{
        //    dlable.RegisterCallback<PointerDownEvent>(evt => { is_sentence = true; });
        //    dlable.RegisterCallback<PointerMoveEvent>(evt => { Debug.Log("label 드래그 확인 문구"); });
        //    dlable.RegisterCallback<PointerUpEvent>(evt => { Debug.Log("label 클릭 놓은 확인 문구"); });
        //    Debug.Log("드래그 라벨" + dlable.text);
        //}



        var clickable = textContainer.Query<TextElement>().Class("clickable").Build();
        foreach (var i in clickable)
        {
            Debug.Log("clickable" + i.name);
            i.RegisterCallback<PointerDownEvent>(LoadMestery);
        }
        textContainer.style.width = Length.Percent(50);
        textContainer.style.alignSelf = Align.Center;

    }

    private void SetDiaryText(ref VisualElement textContainer)//diary csv (현재는 테스트파일2) 데이터를 가져와서 사용하는 부분
    {
        List<VisualElement> textList = new List<VisualElement>();
        Dialogue[] dialogues = StartEvent();
        //textContainer.Clear();
        VisualElement visuallist = new VisualElement();
        int num = 0;
        //foreach (var dialogue in dialogues)
        for (int i = 0; i < dialogues.Length; i++)
        {
            string[] parts = Regex.Split(dialogues[i].context[0], @"<br\s*/?>");//나눈 문장들 들어 있음
            Debug.Log("parts count" + parts.Length);
            string[] keys = InventoryManager.Instance.GetInfo_(info_keys[i]).keywords;


            //Debug.Log("key size="+keys.Length);
            if (keys.Length > 0)//키가 있다면
            {
                Debug.Log("keys =>" + keys[0]);
                //str += string.Format("키워드 {0}", InventoryManager.Instance.GetInfo_(i).keywords[0]);
            }
            else
            {
                Debug.Log("null");
            }


            foreach (string part in parts)
            {
                string[] _part = part.Split(' ');
                foreach (string p in _part)//여기 드래그 관련 내용들은 csv가 아닌 수집품의 내용 관련으로 갈것임 지금 해당내용은 테스트용이라고 생각하는것이 좋음 띄워 쓰기 관련은 인벤토리(수집품) 추리시 발생
                {
                    bool check = (keys.Length > 0 ? part.ContainsAny(keys[0]) : false);
                    //int a = part.IndexOf(keys[0]);
                    Debug.Log($"분리 후: [{p}] check[{check}]"); //지금 분리도 안 되는거같은데

                    var textelement = new TextElement { text = p, name = "textelement" };
                    visuallist.Add(textelement);
                    if (check)
                    {
                        //textelement.RegisterCallback<PointerDownEvent>(evt => { is_sentence = true; });
                        //textelement.RegisterCallback<PointerMoveEvent>(evt => { Debug.Log("label 드래그 확인 문구"); });
                        //textelement.RegisterCallback<PointerUpEvent>(evt => { Debug.Log("label 클릭 놓은 확인 문구"); });
                        //textelement.AddToClassList("draggable");
                        textelement.AddToClassList("clickable");
                        textelement.RegisterCallback<PointerDownEvent>(LoadMestery);
                    }
                    else
                    {
                        textelement.AddToClassList("sentence");
                    }
                    //visuallist.AddToClassList("clickable");
                }


                //(check ? textList[num++].AddToClassList("sentence") : textList[num++].AddToClassList("sentence"));
                //textList[num++].AddToClassList("sentence");//클래스 부여 하는 부분
            }
            textList.Add(visuallist);
            //textList[i].Add(visuallist);
            //visuallist.Add(tex);
            visuallist.style.flexDirection = FlexDirection.Row;
            //visualElement.Add(textList[i]);
            textContainerContent.Add(textList[i]);
            var t = visuallist.Query<TextElement>().Build();
            foreach (var k in t)
            {
                Debug.Log($"textlist [{i}] => {k.text}");
            }
            visuallist = new VisualElement();


        }
        //for (int i = 0; i < textList.Count; i++)
        //{
        //
        //    //while (true) { }//입력 대기 부분
        //    textList[i].style.flexGrow = 3;
        //    //아래는 출력용
        //    var t = textList[i].Query<TextElement>().Build();
        //    foreach(var k in t)
        //    {
        //        Debug.Log($"textlist [{i}] => {k.text}");
        //    }
        //
        //    //textContainer.Add(textList[i]);
        //    //textContainer.style.flexDirection = FlexDirection.Row;
        //    //visualElement.Add(visuallist[i]);
        //}
    }

    //Awaitable 

    private Dialogue[] StartEvent()//현재 가지고있는 아이템의 dialogue를 가져옴
    {
        Debug.Log("추리 이벤트 실행됨!");
        Dialogue[] tempdialogue = DatabaseManager.instance.theParser.Parse("테스트파일2");//일지 파일 명으로 변경
        DatabaseManager.instance.dialogueDic.Clear();
        Dictionary<int, Dialogue> trace_dic = new Dictionary<int, Dialogue>();//매번 할 필요없을거같긷한데 나중에 수정해도 될듯
        foreach (var dialogue in tempdialogue)
        {
            Debug.Log("dialogue id" + dialogue.id + "dialogue text" + dialogue.context[0]);
            trace_dic.Add(int.Parse(dialogue.id), dialogue);
        }
        //for (int i = 0; i < tempdialogue.Length; i++)//문자열에서 아이템 찾는 부분이었음
        //{
        //    if (tempdialogue[i].context[0].Contains("item"))//수집품 요구하는 문장
        //    {
        //        MatchCollection matches=Regex.Matches(tempdialogue[i].context[0], "<item(.*?)>");  
        //        foreach(Match match in matches)
        //        {
        //            string str = match.Groups[1].Value; //매칭 된 숫자들 나옴
        //            //Debug.Log(str);//숫자는 가져옴
        //        }
        //        //Debug.Log(i + "번째에 아이템이 있음"+"위치는 "+ tempdialogue[i].context[0].IndexOf("item"));//item위치 찾기 위함
        //    }
        //    Debug.Log(string.Format("id =>{0}",i+1));
        //
        //    DatabaseManager.instance.dialogueDic.Add(i + 1, tempdialogue[i]);
        //}
        List<Dialogue> templist = new List<Dialogue>();
        foreach (var i in info_keys)//현재 가지고 있는 키에서 문자를 받을수있음
        {
            //Debug.Log("key id=>"+i);

            //string str = string.Format("getKey value{0}", trace_dic[i].context[0]);
            //string[]keys =InventoryManager.Instance.GetInfo_(i).keywords;
            //
            //
            ////Debug.Log("key size="+keys.Length);
            //if (keys.Length>0)//키가 있다면
            //{
            //    //Debug.Log("keys =>" + keys[0]);
            //    str += string.Format("키워드 {0}", InventoryManager.Instance.GetInfo_(i).keywords[0]);
            //}
            //else
            //{
            //    Debug.Log("null");
            //}
            //Debug.Log(str);
            //if (InventoryManager.Instance.GetInfo_(i).keywords != null)//여기 라인이 계속 에러, keywords 가 null인듯
            //{
            //    str += string.Format("{0}", InventoryManager.Instance.GetInfo_(i).keywords[0]);
            //}


            //inventoryScripable.inventory[i].context.
            //Dialogue dia = trace_dic[i];
            templist.Add(trace_dic[i]);
        }

        //Dialogue[] dialogues = DatabaseManager.instance.GetDialogues(0, 5);
        Dialogue[] dialogues = templist.ToArray();
        //Dialogue[] dialogues=new Dialogue[100]
        return dialogues;
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (is_drag)
        {
            Debug.Log("드래그중");

            Vector2 currentMousePosition = evt.position;
            Vector2 mouseDelta = currentMousePosition - startMousePosition;
            if (dragGhost != null)
            {
                //Debug.Log("고스트 움직이는중");
                // 고스트는 절대 위치를 사용하므로, 마우스 델타만큼 직접 이동
                Vector2 newGhostPosition = visualElement.WorldToLocal(startMousePosition) + mouseDelta;

                dragGhost.style.left = newGhostPosition.x;
                dragGhost.style.top = newGhostPosition.y;

                Debug.Log(string.Format("고스트 left {0} top {1}", dragGhost.style.left, dragGhost.style.top));
            }
        }
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        if (!is_sentence) return;
        is_drag = true;
        Debug.Log("클릭위치" + evt.position);
        if (dragGhost == null)//고스트( 오브젝트 생성)
        {
            dragGhost = new Label("visualElement 고스트");
            dragGhost.style.fontSize = visualElement.resolvedStyle.fontSize;
            dragGhost.style.color = UnityEngine.Color.red;
            dragGhost.style.unityFont = visualElement.resolvedStyle.unityFont;
            dragGhost.style.unityFontStyleAndWeight = visualElement.resolvedStyle.unityFontStyleAndWeight;
            dragGhost.style.position = Position.Absolute; // 절대 위치 사용
            dragGhost.style.opacity = 0.7f; // 반투명하게
            dragGhost.pickingMode = PickingMode.Ignore; // 고스트는 이벤트 받지 않음

            // 초기 위치 설정 


            visualElement.Add(dragGhost); // 루트에 추가하여 다른 UI 위에 표시
        }
        Vector2 ghostStartPosition = evt.position; // 스크린/월드 좌표

        ghostStartPosition = visualElement.WorldToLocal(ghostStartPosition); // 루트 요소 기준 로컬 좌표로 변환
        dragGhost.style.left = ghostStartPosition.x;
        dragGhost.style.top = ghostStartPosition.y;
        startMousePosition = evt.position;
    }

    private void LoadMestery(PointerDownEvent evt)
    {
        Debug.Log("Mestery Load");
        var root = GetComponent<UIDocument>().rootVisualElement;
        var panel = root.Q<VisualElement>("TracerNotePanel");
        panel.style.scale = new Vector2(1, 1);
        var tracer = root.Q<VisualElement>("TracerNote");//여기 하위에 오브젝트 배치
        tracer.AddToClassList("test2-2");

        //테스트용 이제 이렇게 텍스트를 배치해야함
        int i = 0;
        VisualElement visuallist = new VisualElement();
        foreach (var inven in inventoryScripable.inventory)
        {
            Debug.Log("inven =" + inven.Value.context);
            string[] parts = Regex.Split(inven.Value.context, @"<br\s*/?>");
            string[] keys = InventoryManager.Instance.GetInfo_(info_keys[i++]).keywords;


            foreach (string part in parts)
            {
                string[] _part = part.Split(' ');
                foreach (string p in _part)//여기 드래그 관련 내용들은 csv가 아닌 수집품의 내용 관련으로 갈것임 지금 해당내용은 테스트용이라고 생각하는것이 좋음 띄워 쓰기 관련은 인벤토리(수집품) 추리시 발생
                {
                    bool check = (keys.Length > 0 ? part.ContainsAny(keys[0]) : false);
                    //int a = part.IndexOf(keys[0]);
                    Debug.Log($"분리 후: [{p}] check[{check}]"); //지금 분리도 안 되는거같은데

                    var textelement = new TextElement { text = p, name = "textelement" };
                    visuallist.Add(textelement);
                    if (check)
                    {
                        textelement.RegisterCallback<PointerDownEvent>(evt => { is_sentence = true; });
                        textelement.RegisterCallback<PointerMoveEvent>(evt => { Debug.Log("label 드래그 확인 문구"); });
                        textelement.RegisterCallback<PointerUpEvent>(evt => { Debug.Log("label 클릭 놓은 확인 문구"); });
                        textelement.AddToClassList("draggable");
                        //textelement.AddToClassList("clickable");
                        //textelement.RegisterCallback<PointerDownEvent>(LoadMestery);
                    }
                    else
                    {
                        textelement.AddToClassList("sentence");
                    }
                    //visuallist.AddToClassList("clickable");
                    //visuallist.Add(textelement);
                }

                //var sample = new TextElement { text = inven.Value.context, name = "sentence1" };
                //sample.AddToClassList("sentence");
                //sample.style.fontSize = 40;
                tracer.Add(visuallist);
                //visualElement = new VisualElement();
            }

        }
        drop_area.RemoveFromHierarchy();
        panel.Add(drop_area);
        drop_area.style.color = UnityEngine.Color.white;
        var element = panel.Q<VisualElement>("TracerNotePanel");
        element.style.width = Length.Percent(100);
        textContainer.style.flexGrow = 0;
        textContainer.RemoveFromHierarchy();
        panel.Add(textContainer);
        textContainer.AddToClassList("left-diary");
    }
}

