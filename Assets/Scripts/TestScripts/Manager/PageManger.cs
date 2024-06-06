using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PageManger : MonoBehaviour
{
    // Start is called before the first frame update
    //늦은 초기화는 안 해도 될것 같았음 아래는 늦은 초기화 방식
    //private static Lazy<SceneManger> instance = new Lazy<SceneManger>(() => new SceneManger());
    //public static SceneManger Instance { get { return instance.Value; } }   

    //개선점 : 의존성 주입 방식으로 해결해서 싱글톤 사용을 줄일수 있을까?

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
