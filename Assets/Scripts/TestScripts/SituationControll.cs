using UnityEngine;

public class SituationControll : MonoBehaviour
{

    public enum Situation
    {
        InventoryBox,
        stage
    }

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
        ObjectEvent();
    }

    public void ObjectEvent()
    {
        switch (situation)
        {
            case Situation.InventoryBox:
                Debug.Log("inventoryBox");
                break;
            case Situation.stage:
                Debug.Log("stage");
                break;
        }
    }
}
