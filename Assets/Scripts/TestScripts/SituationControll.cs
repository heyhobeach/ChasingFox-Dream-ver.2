using Collection;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SituationControll : MonoBehaviour
{

    public NewsScriptorble[] wingman_news;

    public enum Situation
    {
        InventoryBox,
        Wingman,
        door
    }

    private bool iskeydown = false;

    public Situation situation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        iskeydown = false;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            iskeydown = true;
        }
        if (iskeydown)
        {
            ObjectEvent();
            iskeydown = false;
        }

    }

    public void ObjectEvent()
    {
        switch (situation)
        {
            case Situation.InventoryBox:
                Debug.Log("inventoryBox");
                PopInventory();
                break;
            case Situation.Wingman:
                Debug.Log("Wingman");
                Receive();
                break;
            case Situation.door:
                Debug.Log("door");
                break;
        }
    }


    public void PopInventory()
    {
        GameManager.Instance.InventoryEnable();
    }



    public void Receive()
    {
        if (wingman_news == null)
        {
            Debug.LogError("윙맨 뉴스 없음 뉴스 넣어주세요");
            return;
        }
        Debug.Log("현재 챕터"+DatabaseManager.instance.chapter);
        foreach(var i in wingman_news)
        {
            Debug.Log(i.name);
        }
        Debug.Log("Receive");
    }


    /// <summary>
    /// 저장된 정보 받아서 씬을 로딩 할 예정
    /// </summary>
    public void Door()
    {
        //SceneManager.LoadScene();
    }
}
