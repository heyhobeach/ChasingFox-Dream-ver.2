using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_DynamicText : MonoBehaviour
{
    private void OnEnable()
    {
        // UIDocument에서 rootVisualElement 가져오기
        var root = GetComponent<UIDocument>().rootVisualElement;

        // textContainer 가져오기
        var textContainer = root.Q<VisualElement>("textContainer");

        // 기존 요소 제거 (초기화)
        //textContainer.Clear();

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
        clickableText.RegisterCallback<PointerUpEvent>(evt =>
        {
            Debug.Log("클릭된 텍스트: 클릭 가능한 텍스트");
            StartEvent();
        });

        // 컨테이너에 추가
        textContainer.Add(text1);
        textContainer.Add(clickableText);
        textContainer.Add(text2);
    }

    private void StartEvent()
    {
        Debug.Log("추리 이벤트 실행됨!");
        //DatabaseManager.instance
        //DialogueParser parser = new DialogueParser();
        //parser.Parse("테스트파일");
        Dialogue[] tempdialogue = DatabaseManager.instance.theParser.Parse("테스트파일2");//일지 파일 명으로 변경
        Debug.Log("tempdialogue length" + tempdialogue.Length);
        //Dictionary<int, Dialogue> dialogueDic = new Dictionary<int, Dialogue>();
        DatabaseManager.instance.dialogueDic.Clear();
        Debug.Log("before dialogue dic lenght"+DatabaseManager.instance.dialogueDic.Count);    
        for (int i = 0; i < tempdialogue.Length; i++)
        {
            
            DatabaseManager.instance.dialogueDic.Add(i + 1, tempdialogue[i]);
        }
        Debug.Log("after dialogue dic lenght" + DatabaseManager.instance.dialogueDic.Count);
        Dialogue[] dialogues = DatabaseManager.instance.GetDialogues(0, 5);

        foreach (var i in dialogues)
        {
            Debug.Log("dialogues" + i.context[0]);
        }
    }


}
