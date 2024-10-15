using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;


public class SetCharImage : MonoBehaviour
{
    /// <summary>
    /// 연결이 풀릴경우 public 선언때문 같음 따라서 reset하고 다시 재 연결 필요함
    /// </summary>
    public Image main_charactor;
    public Image sub_charactor;

     private CharImageData mainData;
    private CharImageData subData; 

    ScriptableObject[] char_image_list;

    public string color_code = "5C5C5C"; 
    // Start is called before the first frame update
    void Start()
    {
        char_image_list = GetComponent<CharactorImageList>().ImageList;
        mainData=main_charactor.transform.GetComponent<CharImageData>();
        subData=sub_charactor.transform.GetComponent<CharImageData>();
        color_code = "#" + color_code;
    }

    // Update is called once per frame
    void Update()
    {
        if (main_charactor==null)
        {
            Debug.Log("main_charactor null");
        }
        else
        {
            Debug.Log(main_charactor.transform.name);
        }
       if (sub_charactor == null)
        {
            Debug.Log("sub_charactor null");
        }
        else
        {
            Debug.Log(sub_charactor.gameObject.transform.name);
        }



        //ChangeImage();
        SetDarkImage();
        //main_charactor.sprite = main_sprite;
        //sub_charactor.sprite = sub_sprite;
    }
    /// <summary>
    /// 대사 불러올때 한번 호출 예정 지금은 테스트 때문에 update에서 호출중
    /// </summary>
    public void ChangeImage(string image_name, string image_dir)
    {
        Debug.Log(image_dir);
        Sprite image_sprite = null;
        image_name = string.Format("illustration\\{0}", image_name);
        image_sprite = Resources.Load<Sprite>(image_name); 
        if(image_name == null)
        {
            Debug.Log("Image Null");
            throw new Exception("이미지가 없습니다. 이름 확인 혹은 파일 다시 확인 해 주세요");
        }
        if (image_dir == "left")//여기서 문제 지금 왼쪽 오른쪽 텍스트가 구별 안되는중
        {
            Debug.Log("왼쪽");
            main_charactor.sprite = image_sprite;
        }
        else
        {
            Debug.Log("오른쪽");
            sub_charactor.sprite = image_sprite;
        }
        //ScriptorbleObjectTest temp;
        //for(int i=0;i<char_image_list.Length;i++)
        //{
        //    temp = (ScriptorbleObjectTest)char_image_list[i]; 
        //    if (temp.char_name== "zizel_cheerless")
        //    {
        //        mainData.ImageData = (ScriptorbleObjectTest)char_image_list[0];//상황에 맞게 인덱스 번호 수정필요
        //    }
        //    if(temp.char_name== "human_coffee")
        //    {
        //        subData.ImageData = (ScriptorbleObjectTest)char_image_list[1];
        //    }
        //}
        //mainData.ImageData = (ScriptorbleObjectTest)char_image_list[0];//상황에 맞게 인덱스 번호 수정필요
        //subData.ImageData = (ScriptorbleObjectTest)char_image_list[1];

    }

    public void SetDarkImage()
    {
        Color color;

        ColorUtility.TryParseHtmlString(color_code, out color);
        //Debug.Log("color code " + color);
        sub_charactor.color = color;
    }

}

