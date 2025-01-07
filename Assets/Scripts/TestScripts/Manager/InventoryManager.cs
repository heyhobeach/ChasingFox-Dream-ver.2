using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

//싱글톤으로 구현할지 연결을 시킬지 생각해봐야함 
public class InventoryManager : Inventory

{
    public Inventory inven;
    new public Dictionary<int, Info> inventory;
    InventoryScripable invendata;
    // Start is called once before the first execution of Update after the MonoBehaviour is created\
    private void Awake()
    {
        inventory = new Dictionary<int, Info>();

        invendata = Resources.Load("Inventory") as InventoryScripable;

    }


    public void showinven()
    {
        Debug.Log("수집품 목록 확인 함수");
        if (invenCount > 0)
        {
            Debug.Log("수집품 존재");
            for (int i = 0; i < invenCount; i++)
            {
                Debug.Log(string.Format("수집품 목록확인+{0} : {1},{2}", i, inventory[i].name, inventory[i].context));
            }
        }
        else
        {
            Debug.Log("수집품 없음");
        }
    }



     public Info GetInfo_(int id)
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
}
