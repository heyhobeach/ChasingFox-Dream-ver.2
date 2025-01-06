using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

//싱글톤으로 구현할지 연결을 시킬지 생각해봐야함 
public class InventoryManager : Inventory

{
    Inventory inven;
    // Start is called once before the first execution of Update after the MonoBehaviour is created\
    private void Awake()
    {

    }

    public void setitem()
    {

    }

    private void OnEnable()
    {
        OnItemAdded += setInven;
    }
    private void OnDisable()
    {
        OnItemAdded -= setInven;
    }

    public void setInven(int id,Info _info)
    {
        Debug.Log("before add inven - manager");
        inventory.Add(id, _info);
        //inventory.Add(id, _info);
        Debug.Log("add inventory - manager");
    }

    public void testFunc2()
    {
        Debug.Log("inventorymanager 싱글톤 테스트");
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

    //public Dictionary<int,Info> getall()
    //{
    //    return inventory;
    //}
    //
    public Info GetInfo(int id)
    {
        return inventory[id];
    }
}
