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

    private List<int> info_keys = new List<int>();
    bool is_drag = false;

    bool is_sentence = false;

    Vector2 startMousePosition;
    InventoryScripable inventoryScripable;

    int num = 0;

    private void OnEnable()
    {

        Dialogue[] dialyDialogue=StartEvent("테스트파일2");
        foreach(var i in dialyDialogue)
        {
            Debug.Log("dialyDialogue"+i.context[0]);
        }
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


        });



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

    private void SetDiaryText(ref VisualElement textContainer)//diary csv (현재는 테스트파일2) 데이터를 가져와서 사용하는 부분
    {
        List<VisualElement> textList = new List<VisualElement>();
        Dialogue[] dialogues = StartEvent("테스트파일2");

        VisualElement visuallist = new VisualElement();
        int num = 0;

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


            foreach (string part in parts)//br기준
            {
                string[] _part = part.Split(' ');
                var textelement = new TextElement { text = part, name = "textelement" };
                visuallist.Add(textelement);
                textelement.style.width = Length.Percent(100);
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

                }


            }
            textList.Add(visuallist);

            visuallist.style.flexDirection = FlexDirection.Column;
            visuallist.style.width = Length.Percent(100);
            //visualElement.Add(textList[i]);
            textContainerContent.Add(textList[i]);
            textList[i].style.flexDirection = FlexDirection.Column;
            textContainerContent.style.flexDirection = FlexDirection.Column;
            var t = visuallist.Query<TextElement>().Build();
            foreach (var k in t)
            {
                Debug.Log($"textlist [{i}] => {k.text}");
            }
            visuallist = new VisualElement();


        }

    }


    private Dialogue[] StartEvent(string title_str)//현재 가지고있는 아이템의 dialogue를 가져옴
    {
        Debug.Log("추리 이벤트 실행됨!");
        Dialogue[] tempdialogue = DatabaseManager.instance.theParser.Parse(title_str);//일지 파일 명으로 변경
        Debug.Log(tempdialogue.Length);
        DatabaseManager.instance.dialogueDic.Clear();
        Dictionary<int, Dialogue> trace_dic = new Dictionary<int, Dialogue>();//매번 할 필요없을거같긷한데 나중에 수정해도 될듯
        foreach (var dialogue in tempdialogue)
        {
            Debug.Log("dialogue id" + dialogue.id + "dialogue text" + dialogue.context[0]);
            trace_dic.Add(int.Parse(dialogue.id), dialogue);
        }

        List<Dialogue> templist = new List<Dialogue>();
        foreach (var i in info_keys)//현재 가지고 있는 키에서 문자를 받을수있음
        {

            templist.Add(trace_dic[i]);
        }

        Dialogue[] dialogues = templist.ToArray();
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


    private void LoadTest(PointerDownEvent evt)
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var panel = root.Q<VisualElement>("TracerNotePanel");
        panel.style.scale = new Vector2(1, 1);
        var tracer = root.Q<VisualElement>("TracerNote");//여기 하위에 오브젝트 배치

        tracer.AddToClassList("test2-2");
    }
    private void LoadMestery(PointerDownEvent evt)//데이터를 미리 정해놔야할듯?
    {
        textContainer.Clear();
        DiaryContentSet();
        MesterySystem();
    }
    private void DiaryContentSet()
    {

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

        content.style.flexDirection = FlexDirection.Column;
        content.style.flexWrap = Wrap.NoWrap;
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
            lineContainer.style.flexWrap = Wrap.Wrap;
            float estimatedLineHeightInPixels = 50f;
            lineContainer.style.minHeight = new StyleLength(new Length(estimatedLineHeightInPixels, LengthUnit.Pixel));
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
                    textelement.RegisterCallback<PointerDownEvent>(evt => { is_sentence = true; });
                    textelement.RegisterCallback<PointerMoveEvent>(evt => { Debug.Log("label 드래그 확인 문구"); });
                    textelement.RegisterCallback<PointerUpEvent>(evt => { Debug.Log("label 클릭 놓은 확인 문구"); });
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

