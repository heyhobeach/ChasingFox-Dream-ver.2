using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PageManger : MonoBehaviour
{
    // Start is called before the first frame update
    //���� �ʱ�ȭ�� �� �ص� �ɰ� ������ �Ʒ��� ���� �ʱ�ȭ ���
    //private static Lazy<SceneManger> instance = new Lazy<SceneManger>(() => new SceneManger());
    //public static SceneManger Instance { get { return instance.Value; } }   

    //������ : ������ ���� ������� �ذ��ؼ� �̱��� ����� ���ϼ� ������?

    private static PageManger instance;
    public static PageManger Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PageManger();
            }
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RoadRetry()//
    {
        SceneManager.LoadScene("SampleScene");
    }
}
