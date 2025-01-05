using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Inventory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    protected class Info
    {
        public string name;
        public string context;
        public Sprite image;
    }
    protected Dictionary<int, Info> inventory;

    private void Awake()
    {
        inventory = new Dictionary<int, Info>();
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
            //Debug.Log(string.Format("수집품 추가 완료+{0} : {1},{2}", collection.id, inventory[collection.id].name, inventory[collection.id].context));
        }
    }
    //public Info GetInfo(int id)
    //{
    //    if (inventory.ContainsKey(id))
    //    {
    //        return inventory[id];
    //    }
    //}

}



