using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class FilFindTest : MonoBehaviour
{
    enum Lang
    {
        KOR,
        ENG
    }
    [SerializeField]
    Lang eLang = Lang.KOR;
    DialogueParser theParser = new DialogueParser();
    public void Find()
    {
        string chapter = "";
        for(int i = 1; i < 4; i++)
        {
            chapter = string.Format("Chapter{0}\\{1}{2}", i,"Chapter",i);
            string dir_path = string.Format("{0}\\{1}\\{2}", "FindTest",CheckLangugea(), chapter);
            //Debug.Log(dir_path);
            //DirectoryInfo di = new DirectoryInfo(dir_path);
            Dialogue[] dialogues = theParser.Parse(dir_path);
            if (dialogues == null)
            {
                Debug.Log("로드 실패");
            }
            else
            {
                Debug.Log(string.Format("{0} 불러옴", dir_path));
            }
            //foreach (FileInfo fi in di.GetFiles())
            //{
            //    print(fi);
            //    //Dialogue[] dialogues = theParser.Parse(fi);//여기서 지금 대화 모든 내용을 다 파싱 한 상태
            //}
        }


    }
    string CheckLangugea()
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

    // Start is called before the first frame update
    void Start()
    {


        Find();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
