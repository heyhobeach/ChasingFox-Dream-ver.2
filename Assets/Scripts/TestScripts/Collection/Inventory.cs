using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;



public class Inventory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public class Info:News
    {
        //public string _name;
        public string context;
        //public Sprite image;
    }

    public class News
    {
        public Sprite image;
        public string image_name;
    }
    public  Dictionary<int, Info> inventory;//static으로 해결은 가능한데
    public Dictionary<int, Info> newsDic;//static으로 해결은 가능한데
    //public Dictionary<int, int> inventory2;
    public  event Action<int, Info> OnItemAdded;//static으로 해결은 가능한데
    InventoryScripable invendata;
    InventoryScripable newsdata;

    public int invenCount;
    public int newsCount;

    private void Awake()
    {

    }
     public void testFunc1()
    {
        Debug.Log("inventory 싱글톤 테스트");
    }

    private Info SetInfoStruct(Collection.CollectionScriptorble collection)
    {
        Info info=new Info();
        info.image_name = collection._name;
        info.context = collection._context;
        info.image = collection.image;
        return info;
    }
    public void AddInventory(Collection.CollectionScriptorble collection)
    {
        if (invendata == null)
        {
            invendata = Resources.Load("Inventory") as InventoryScripable;
            inventory = new Dictionary<int, Info>();
        }
        if (!inventory.ContainsKey(collection.id))
        {

            if (!collection.is_collect)
            {
                return;
            }
            inventory.Add(collection.id, SetInfoStruct(collection));

            invendata.inventory = inventory;
            invenCount = invendata.inventory.Count;
            Debug.Log("inventory 개수" + inventory.Count);
            //Debug.Log(string.Format("수집품 추가 완료+{0} : {1},{2}", collection.id, inventory[collection.id].name, inventory[collection.id].context));
        }
    }


    //추후 void> int
    public void GetcurrentChaper()
    {

    }

    //여기를 나중에 collection배열을 주고 이후에 현재 챕터를 가져와야함
    public void AddNews(Collection.CollectionScriptorble collection)
    {
        if (newsdata == null)
        {
            newsdata = Resources.Load("NewsInventory") as InventoryScripable;
            newsDic = new Dictionary<int, Info>();
        }
        if (!newsDic.ContainsKey(collection.id))
        {

            if (!collection.is_collect)
            {
                return;
            }
            newsDic.Add(collection.id, SetInfoStruct(collection));
            newsdata.inventory = newsDic;
            newsCount = newsdata.inventory.Count;
            Debug.Log("inventory 개수" + newsDic.Count);
            Debug.Log(string.Format("수집품 추가 완료+{0} : {1},{2}", collection.id, inventory[collection.id].image, inventory[collection.id].image_name));
        }
    }
    public Info GetInfo(int id)
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
    protected Info GetInfo()
    {
        Info info = new Info();
        return info;
    }

}



