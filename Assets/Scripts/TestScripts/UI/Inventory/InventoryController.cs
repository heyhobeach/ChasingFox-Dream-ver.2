using UnityEngine;

public class InventoryController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public InventoryScripable inventorydata;
    public GameObject list;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    public void NewsInventorySet()
    {
        //inventorydata=InventoryManager.Instance.GetInventoryAll();
        //Debug.Log("invenData +" + inventorydata.inventory.Count);
    }
    public void TraceInventorySet()
    {
        inventorydata = null;
    }
}
