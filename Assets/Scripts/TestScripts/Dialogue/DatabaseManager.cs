using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static DatabaseManager instance;//나중에 싱글턴 될듯

    [DisableInspector] public string csv_FileName;
    enum Lang//언어 설정
    {
        KOR,
        ENG
    }
    [SerializeField]
    Lang eLang = Lang.KOR;

    public int chapter = 0;//추후 0으로 수정 필요 이유 튜토리얼이 0부터 시작하기에

    Dictionary<int, Dialogue> dialogueDic =new Dictionary<int, Dialogue>();
    DialogueParser theParser;

    public int endLine = 0, startLine = 0;//start = 0 end 대화 끝 시점 => start = end , end = 대화 끝 반복(짜야함) 딕셔너리 접근용 수
    public int lastIndex = 0;

    public List<int> indexList;

    public static bool isFinish = false;

    private void Awake()
    {
        //indexList.Add(0);
        if (instance == null)
        {
            instance = this;
            theParser = GetComponent<DialogueParser>();
            //if(csv_FileName.Equals("")) csv_FileName = "테스트파일";
            csv_FileName= string.Format("{0}\\{1}\\Chapter{2}\\Chapter{3}", "FindTest", CheckLangugea(eLang), chapter,chapter);
            //Dialogue[] dialogues =theParser.Parse(csv_FileName);//여기서 지금 대화 모든 내용을 다 파싱 한 상태//주석 풀고 사용 하면 됨
            csv_FileName = "테스트파일";//테스트용 csv파일 주석 풀면 실행됨
            Dialogue[] dialogues =theParser.Parse(csv_FileName);//여기서 지금 대화 모든 내용을 다 파싱 한 상태//주석 풀고 사용 하면 됨
            for (int i =0;i<dialogues.Length;i++)
            {
                dialogueDic.Add(i +1, dialogues[i]);
            }
            isFinish= true;
            Debug.Log("디셔너리 크기 확인"+dialogueDic.Count);//크기 65//파싱이 잘 못 되어있음
        }
    }
    string CheckLangugea(Lang eLang)    
    {
        switch (eLang)
        {
            case Lang.KOR:
                return "KOR";
            case Lang.ENG:
                return "ENG";
        }
        return null;

    }

    public Dialogue[] GetDialogues(int startNum ,int endNum)
    {
        //Debug.Log("파서");
        List<Dialogue> dialoguesList = new List<Dialogue>();

        for(int i = 0; i <= endNum - startNum; i++)//1과 3 사이 내용을 가져 오려고 그러는것
        {
            
            if (startNum + i < 1)
            {
                //Debug.Log("넘김");
                //Debug.Log(string.Format("넘김 {0}||{1}||{2}", startNum, endNum, i));
                continue;
            }
            else
            {
                //Debug.Log("else문");
            }//Debug.Log("리스트 삽입");
            dialoguesList.Add(dialogueDic[startNum + i]);
        }
        Debug.Log("GetDialogues 호출");
        return dialoguesList.ToArray();
    }
}
