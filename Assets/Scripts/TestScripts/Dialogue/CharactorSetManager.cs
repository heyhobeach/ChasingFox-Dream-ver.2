using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SetCharImage : MonoBehaviour
{
    /// <summary>
    /// 연결이 풀릴경우 public 선언때문 같음 따라서 reset하고 다시 재 연결 필요함
    /// </summary>
    public Image main_charactor;
    public Image sub_charactor;

    [SerializeField] private Sprite main_sprite;
    [SerializeField] private Sprite sub_sprite;

    [SerializeField] private CharImageData mainData;
    [SerializeField] private CharImageData subData; 

    ScriptableObject[] char_image_list;
    // Start is called before the first frame update
    void Start()
    {
        char_image_list = GetComponent<CharactorImageList>().ImageList;
        mainData=main_charactor.transform.GetComponent<CharImageData>();
        subData=sub_charactor.transform.GetComponent<CharImageData>();
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

        mainData.ImageData = (ScriptorbleObjectTest)char_image_list[0];//상황에 맞게 인덱스 번호 수정필요
        subData.ImageData = (ScriptorbleObjectTest)char_image_list[1];
        main_charactor.sprite = mainData.ImageData.sprite;
        sub_charactor.sprite = subData.ImageData.sprite;
        //main_charactor.sprite = main_sprite;
        //sub_charactor.sprite = sub_sprite;
    }
}
