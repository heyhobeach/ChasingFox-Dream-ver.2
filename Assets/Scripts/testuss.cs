using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_DynamicText : MonoBehaviour
{
    private void OnEnable()
    {
        InventoryScripable inventoryScripable = InventoryManager.Instance.GetInventoryAll();//인벤토리에서 데이터를 가져옴
        if (inventoryScripable.inventory != null)
        {
            Debug.Log(inventoryScripable);
            foreach (var item in inventoryScripable.inventory)
            {
                Debug.Log(item.Value.context);
            }
        }
        else
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
        
        var text1 = new TextElement { text = "이 문장에서 ", name = "sentence1" };
        var clickableText = new TextElement { text = "변경 테스트", name = "clickableWord"};
        var text2 = new TextElement { text = "를 눌러보세요.", name = "sentence2" };

        // 클릭 이벤트 추가
        List<TextElement> textList = new List<TextElement>();

        SetDiaryText(ref textContainer);
        //ui toolkit에서 제공하는 함수로 이벤트 등록에 사용됨
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


        // 컨테이너에 추가
        //textContainer.Add(text1);
        textContainer.Add(clickableText);
        //textContainer.Add(text2);

    }

    private void SetDiaryText(ref VisualElement textContainer)//데이터를 가져와서 사용하는 부분
    {
        List<TextElement> textList = new List<TextElement>();
        Dialogue[] dialogues = StartEvent();
        foreach (var dialogue in dialogues)
        {
            string[] parts = Regex.Split(dialogue.context[0], @"<br\s*/?>");//나눈 문장들 들어 있음
            Debug.Log("parts count" + parts.Length);
            foreach (string part in parts)
            {
                Debug.Log($"분리 후: [{part}]");
                textList.Add(new TextElement { text = part, name = "textelement" });
            }
        }
        for (int i = 0; i < textList.Count; i++)
        {
            textContainer.Add(textList[i]);
        }
    }

    private Dialogue[] StartEvent()//클릭시 호출될 함수
    {
        Debug.Log("추리 이벤트 실행됨!");
        Dialogue[] tempdialogue = DatabaseManager.instance.theParser.Parse("테스트파일2");//일지 파일 명으로 변경
        DatabaseManager.instance.dialogueDic.Clear();   
        for (int i = 0; i < tempdialogue.Length; i++)
        {
            if (tempdialogue[i].context[0].Contains("item"))//수집품 요구하는 문장
            {
                MatchCollection matches=Regex.Matches(tempdialogue[i].context[0], "<item(.*?)>");  
                foreach(Match match in matches)
                {
                    string str = match.Groups[1].Value; //매칭 된 숫자들 나옴
                    //Debug.Log(str);//숫자는 가져옴
                }
                //Debug.Log(i + "번째에 아이템이 있음"+"위치는 "+ tempdialogue[i].context[0].IndexOf("item"));//item위치 찾기 위함
            }
            DatabaseManager.instance.dialogueDic.Add(i + 1, tempdialogue[i]);
        }
        Dialogue[] dialogues = DatabaseManager.instance.GetDialogues(0, 5);
        return dialogues;
    }


}
