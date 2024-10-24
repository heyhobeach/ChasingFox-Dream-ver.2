using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
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
    }
    /// <summary>
    /// 대사 불러올때 한번 호출 예정 지금은 테스트 때문에 update에서 호출중
    /// </summary>
    public void ChangeImage(string image_name, string image_dir,bool is_disable)
    {

        Sprite image_sprite = null;//이미지 받아오는 변수
        Debug.Log("위치" + image_dir);
        image_name = string.Format("illustration\\{0}", image_name);
        image_sprite = Resources.Load<Sprite>(image_name);
        Image target = null;
        Image other = null;
        if(image_name == null)
        {
            Debug.Log("Image Null");
            throw new Exception("이미지가 없습니다. 이름 확인 혹은 파일 다시 확인 해 주세요");
        }
        Debug.Log(image_dir);
        if (image_dir.Equals("left"))//여기서 문제 지금 왼쪽 오른쪽 텍스트가 구별 안되는중
        {
            //Debug.Log(image_dir+"left");
            target = main_charactor;
            other = sub_charactor;
        }
        else
        {
            //Debug.Log(image_dir+"right");
            target = sub_charactor;
            other = main_charactor;
        }
        target.sprite = image_sprite;
        SetDarkImage(other);
        //SetDisable(other);
        SetWhiteImage(target);
        if(is_disable)
        {
            Debug.Log("투명");
            //other.color = Color.clear;
            SetDisable( other);
        }

    }

    public void SetDarkImage(Image charactor)
    {
        Color color;

        ColorUtility.TryParseHtmlString(color_code, out color);
        charactor.color = color;
    }

    public void SetWhiteImage(Image charactor)
    {
        charactor.color = Color.white;
    }

    public void SetDisable( Image charactor)//지금 두번 호출 중임
    {
        Debug.Log("setdisable");
        charactor.color = Color.clear;
    }

}

