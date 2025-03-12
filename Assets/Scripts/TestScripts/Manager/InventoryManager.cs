using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//싱글톤으로 구현할지 연결을 시킬지 생각해봐야함 
//지금 보변 Inventory를 상속 받았을 필요도 없어보임
public class InventoryManager : MonoBehaviour

{

    private static InventoryManager instance;
    private Inventory _inven;


    public static InventoryManager Instance {  get { return instance; } }
    InventoryScripable invendata;
    InventoryScripable Newsdata;

    // Start is called once before the first execution of Update after the MonoBehaviour is created\
    private void Awake()
    {

        invendata = Resources.Load("Inventory") as InventoryScripable;//이걸 가져오면 저장이 어떻게 되는지?
        if (instance == null)
        {
            instance = this;
        }
    }

    /// <summary>
    /// 저장되어있는 인벤토리의 정보를 스크립터블로 옮길 예정
    /// </summary>
    public void LoadInvenInfo() 
    { }

    /// <summary>
    /// 인벤토리의 정보를 저장 할 함수
    /// </summary>
    public void SaveInvenInfo()
    { }






     public Inventory.Info GetInfo_(int id)
    {
        //if(invendata != null)
        //{
        //    for (int i = 0; i < invendata.inventory.Count; i++)
        //    {
        //        Info info = invendata.inventory[i];
        //        Debug.Log("scriptorble"+info.name + info.context + info.image);
        //    }
        //}
        return invendata.inventory[id];
    }


    /// <summary>
    /// 스크립터블 오브젝트가 저장 되어있으므로 인벤데이터 사용하려 하면 invendata.inventory로 접근
    /// </summary>
    /// <returns>InventoryScripable</returns>
    public InventoryScripable GetInventoryAll()
    {
        return invendata;
    }


    /// <summary>
    /// 스크립터블 오브젝트가 저장 되어있으므로 인벤데이터 사용하려 하면 invendata.inventory로 접근 현재 사용 안함
    /// </summary>
    /// <returns>InventoryScripable</returns>
    //public InventoryScripable GetNewsDataAll()
    //{
    //    return Newsdata;
    //}
}
