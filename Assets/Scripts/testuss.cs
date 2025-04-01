using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_DynamicText : MonoBehaviour
{

    private List<int> info_keys=new List<int>();
    private void OnEnable()
    {
        InventoryScripable inventoryScripable = InventoryManager.Instance.GetInventoryAll();//인벤토리에서 데이터를 가져옴
        Dictionary<int, Inventory.Info>.KeyCollection keys  = InventoryManager.Instance.GetinventoryKeys();//키를 가져오기 위한 변수


        foreach(var key in keys)//현재 수집 되어있는 key를 가져옴
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
        var textContainer = root.Q<VisualElement>("textContainer");

        // 기존 요소 제거 (초기화)
        textContainer.Clear();

        // 새로운 텍스트 요소 추가
        //Dialogue[] dialogues=InteractionEvent.Instance.GetDialogue();
        //
        //foreach(var i in dialogues)
        //{
        //    Debug.Log(i);
        //}
        
        var text1 = new TextElement { text = "클릭 이벤트 가능한 문장 1 ", name = "sentence1" };
        var clickableText = new TextElement { text = "클릭 이벤트 가능한 문장 2", name = "clickableWord"};
        var text2 = new TextElement { text = "클릭 이벤트 가능한 문장 3", name = "sentence2" };

        // 클릭 이벤트 추가
        List<TextElement> textList = new List<TextElement>();//이벤트가 들어가야하는 내용들
        textList.Add(text1); textList.Add(text2) ; textList.Add(clickableText);
        SetDiaryText(ref textContainer);
        //ui toolkit에서 제공하는 함수로 이벤트 등록에 사용됨
        foreach(var text in textList)
        {
            text.RegisterCallback<PointerUpEvent>(even=>{ 
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

        foreach(var text in textList)
        {
            textContainer.Add(text);
        }


        // 컨테이너에 추가
        //textContainer.Add(text1);
        //textContainer.Add(clickableText);
        //textContainer.Add(text2);
        VisualElement visualElement = root.Q<VisualElement>("VisualElement");
        VisualElement sentence_container=root.Q<VisualElement>("sentence-container");
        var draggable_label= sentence_container.Query<Label>().Class("draggable").Build();

        //해당 내용을 델리게이트로 구성해보는 방식으로
        visualElement.RegisterCallback<PointerDownEvent>(evt => { Debug.Log("패널 클릭 누름 문구 확인"); });
        visualElement.RegisterCallback<PointerMoveEvent>(evt => { Debug.Log("패널 드래그 확인 문구"); });
        visualElement.RegisterCallback<PointerUpEvent>(evt => { Debug.Log("패널 클릭 놓은 확인 문구"); });

        foreach (var dlable in draggable_label)
        {
            dlable.RegisterCallback<PointerDownEvent>(evt => { Debug.Log("label 클릭 누름 문구 확인"); });
            dlable.RegisterCallback<PointerMoveEvent>(evt => { Debug.Log("label 드래그 확인 문구"); });
            dlable.RegisterCallback<PointerUpEvent>(evt => { Debug.Log("label 클릭 놓은 확인 문구"); });
            Debug.Log("드래그 라벨" + dlable.text);
        }

        foreach(var text in textContainer.hierarchy.Children())
        {
            //if(text is TextElement label)
            //{
            //    Debug.Log("text=>" + label.text);
            //    string[]str=label.text.Split(" ");
            //    foreach(var splitstr in str)
            //    {
            //        Debug.Log("split ->" + splitstr);
            //    }
            //}

        }


    }

    private void SetDiaryText(ref VisualElement textContainer)//데이터를 가져와서 사용하는 부분
    {
        List<TextElement> textList = new List<TextElement>();
        Dialogue[] dialogues = StartEvent();
        textContainer.Clear();
        int num = 0;
        foreach (var dialogue in dialogues)
        {
            string[] parts = Regex.Split(dialogue.context[0], @"<br\s*/?>");//나눈 문장들 들어 있음
            Debug.Log("parts count" + parts.Length);
            foreach (string part in parts)
            {
                Debug.Log($"분리 후: [{part}]");
                textList.Add(new TextElement { text = part, name = "textelement" });
                textList[num++].AddToClassList("sentence");//클래스 부여 하는 부분
            }
        }
        for (int i = 0; i < textList.Count; i++)
        {

            //while (true) { }//입력 대기 부분
            textContainer.Add(textList[i]);
            
        }
    }

    //Awaitable 

    private Dialogue[] StartEvent()//클릭시 호출될 함수, csv에서 가져오는방식
    {
        Debug.Log("추리 이벤트 실행됨!");
        Dialogue[] tempdialogue = DatabaseManager.instance.theParser.Parse("테스트파일2");//일지 파일 명으로 변경
        DatabaseManager.instance.dialogueDic.Clear();
        Dictionary<int,Dialogue> trace_dic = new Dictionary<int, Dialogue>();
        foreach (var dialogue in tempdialogue)
        {
            Debug.Log("dialogue id" + dialogue.id +"dialogue text" + dialogue.context[0]);
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
        List<Dialogue> templist=new List<Dialogue>();
        foreach (var i in info_keys)//현재 가지고 있는 키에서 문자를 받을수있음
        {
            //Debug.Log("key id=>"+i);
            Debug.Log("getKey value" + trace_dic[i].context[0]);
            //Dialogue dia = trace_dic[i];
            templist.Add(trace_dic[i]);
        }

        //Dialogue[] dialogues = DatabaseManager.instance.GetDialogues(0, 5);
        Dialogue[] dialogues = templist.ToArray();
        //Dialogue[] dialogues=new Dialogue[100]
        return dialogues;
    }


}
