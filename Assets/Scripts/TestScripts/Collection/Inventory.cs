using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class Inventory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public class Info
    {
        public string name;
        public string context;
        public Sprite image;
    }
    public Dictionary<int, Info> inventory;
    public event Action<int, Info> OnItemAdded;
    public int invenCount;

    private new void Awake()
    {
        //base.Awake();
        inventory = new Dictionary<int, Info>();

    }
    public void testFunc1()
    {
        Debug.Log("inventory 싱글톤 테스트");
    }

    private Info SetInfoStruct(Collection.CollectionScriptorble collection)
    {
        Info info=new Info();
        info.name = collection.name;
        info.context = collection._context;
        info.image = collection.image;
        return info;
    }
    public void AddInventory(Collection.CollectionScriptorble collection)
    {
        if (!inventory.ContainsKey(collection.id))
        {
            inventory.Add(collection.id, SetInfoStruct(collection));
            invenCount = inventory.Count;
            Debug.Log("inventory 개수" + inventory.Count);
            //Debug.Log(string.Format("수집품 추가 완료+{0} : {1},{2}", collection.id, inventory[collection.id].name, inventory[collection.id].context));
        }
    }
    protected Info GetInfo(int id)
    {
        if (inventory.ContainsKey(id))
        {
            return inventory[id];
        }
        else
        {
            return null;
        }
    }

}



