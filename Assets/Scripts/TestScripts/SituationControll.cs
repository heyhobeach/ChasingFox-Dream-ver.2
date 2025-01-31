using UnityEngine;
using UnityEngine.SceneManagement;

public class SituationControll : MonoBehaviour
{

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

    }


    /// <summary>
    /// 저장된 정보 받아서 씬을 로딩 할 예정
    /// </summary>
    public void Door()
    {
        //SceneManager.LoadScene();
    }
}
