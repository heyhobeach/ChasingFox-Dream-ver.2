using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    // Start is called before the first frame update

    string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";//정규식 from chat gpt
    string SPLIT_COMMAND_PASER = @"[""!,]";//명령어 분리 정규식
    string SPLIT_NUM = @"([^1-9]{1,})";//공백 분리 정규식
    string[] row;
    //string[] command;
    string[] testarr;
    //public int start = 0, end = 0;
    //테스트

    string splitsign(string text)
    {
        if (row[1]=="") return "";
        //Debug.Log(text);
        if (text[0] == '\"')
        {
            string newtext=text.Substring(1, text.Length - 2);
            return newtext;
        }
        return text;
    }
    public Dialogue[] Parse(string _CSVFileName)
    {
        int chap = 1;
        List<Dialogue> dialoguesList = new List<Dialogue>();
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName);//csv파일 로드

        if(csvData == null)
        {
            Debug.Log(_CSVFileName+"파일 못 불러옴");
            return null;
        }
        else
        {
            Debug.Log("파일 불러옴");
        }

        string[] data =csvData.text.Split(new char[] { '\n' });//공백분리 split('\n')

        //Debug.Log("last index " + DatabaseManager.instance.lastIndex+"data length"+(data.Length-1));
        for(int i=1+DatabaseManager.instance.lastIndex;i<data.Length-1;)//ID 1번 부터 위에는 다른거라서 필요없음
        {
            int command_num = 0;
            List<string> commandList = new List<string>();
            List<string> testarr = new List<string>();
            row = Regex.Split(data[i], SPLIT_RE);
            //Debug.Log(i + "번째 파싱중");//에러 발견 위치용 디버깅
            //foreach (var txt in row)
            //{
            //    Debug.Log(txt);
            //}
            Dialogue dialogue = new Dialogue();//
            //Debug.Log("i번째 " + dialoguesList.Count);
            //dialogue.command = new string[10][];
            dialogue.name = row[2];//
            dialogue.id = row[0];
            dialogue.image_name = row[5];
            dialogue.dir = row[6];
            //command = Regex.Split(row[4], SPLIT_COMMAND_PASER);
            //command = spaceremove(command);
            

            
            //dialogue.command[command_num++] = command;
            bool isEnd = false;
            //Debug.Log(data[i]);

            List<string> contextList = new List<string>();
            do//동일 id에서 대화 창 변경 한 경우 표시
            {
                //Debug.Log("ID Check" + row[0]+"Context Text" + row[3]);
                commandList.Add(row[4]);
                //testarr.Add(row[5]);//메모 넣는부분
                //dialogue.command[command_num++] = command;
                row[3] = splitsign(row[3]);//id가 null이면 content를 ""을 리턴
                contextList.Add(row[3]);//content
                if (row[3].ToString() == "")//대화가 끝난 경우 대화창 공백
                {
                    isEnd = true;
                    //Debug.Log(string.Format("content =>null"));//대화 끝난거 확인용 debug
                    contextList.RemoveAt(contextList.Count-1);//마지막에 삽인 되어있는 공백 칸 제거용
                }

                if (++i < data.Length-1)
                {
                    row = Regex.Split(data[i], SPLIT_RE);//do while들어와서 csv 분리 못 한 경우 분리
                }
                else
                {
                    break;//이거 없으면 
                }

            } while (row[0].ToString() == "");
            dialogue.command = commandList.ToArray();

            foreach(var coms in contextList)
            {
                //Debug.Log("대사 " + coms);
            }
            dialogue.context = contextList.ToArray();

            dialoguesList.Add(dialogue); //대화 내용 리스트에 삽입
            if (isEnd)
            {
                //Debug.Log("종료");
                //Debug.Log("dialogue list" + dialoguesList.Count);
                //Debug.Log(dialoguesList[dialoguesList.Count - 1].id);//id번호 확인
                DatabaseManager.instance.endLine = int.Parse(dialoguesList[dialoguesList.Count - 1].id); //지금 parser가 어디까지 나올지 모르겠음//end라인까지 끊김//이후 start와 end수정해야함
                DatabaseManager.instance.indexList.Add(int.Parse(dialoguesList[dialoguesList.Count - 1].id));
                //break;
            }

            //Debug.Log(row[1]);

        }
        //DatabaseManager.instance.endLine = int.Parse(row[0]);//마지막 id 가져오기 위함
        return dialoguesList.ToArray();
    }
    private int EndDialogue(int endIndex)//대화 종료 시점을 int 형으로 마지막 대화 부분을 넘겨줄 것임
    {
        if (row[0].ToString() == "")
        {
            //dialoub
            Debug.Log("null");
        }
        return endIndex;
    }





    private void Start()
    {
        //Parse("테스트파일");
    }
}
