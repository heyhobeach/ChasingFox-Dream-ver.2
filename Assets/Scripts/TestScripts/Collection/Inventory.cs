using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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
    public  Dictionary<int, Info> invenDic;//static으로 해결은 가능한데
    public Dictionary<int, News> newsDic;//static으로 해결은 가능한데
    //public Dictionary<int, int> inventory2;
    // public  event Action<int, Info> OnItemAdded;//static으로 해결은 가능한데
    InventoryScripable invendata;
    //InventoryScripable newsdata;

    public int invenCount;
    public int newsCount;

    private void Awake()
    {
        invendata = Resources.Load("Inventory") as InventoryScripable;
        newsDic = new Dictionary<int, News>();
        invenDic = new Dictionary<int, Info>();
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
        Debug.Log(string.Format("{0},{1},{2}", info.image_name, info.context, "collection"));
        return info;
    }

    private News SetInfoStruct(Collection.NewsScriptorble news)
    {
        News _news=new News();
        _news.image = news.image;
        _news.image_name=news.image_name;
        Debug.Log(string.Format("{0},{1}", _news.image_name, "news"));
        return _news;
    }
    public void AddInventory(Collection.CollectionScriptorble collection)
    {
        Debug.Log("AddInventory");
        if (invendata == null)
        {
            invendata = Resources.Load("Inventory") as InventoryScripable;
            //inventory = new Dictionary<int, Info>();
        }
        if (!invenDic.ContainsKey(collection.id))
        {

            if (!collection.is_collect)
            {
                Debug.Log("수집 하지 않는 수집품");
                return;
            }
            invenDic.Add(collection.id, SetInfoStruct(collection));

            invendata.inventory = invenDic;
            invenCount = invendata.inventory.Count;
            Debug.Log("inventory 흔적 개수" + invendata.inventory.Count);
            Debug.Log(string.Format("흔적 추가 완료+{0} : {1},{2}",   collection.id, invenDic[collection.id].image_name, invenDic[collection.id].context));
        }
    }
    public void AddNews(Collection.NewsScriptorble collection)
    {
        if (invendata == null)
        {
            invendata = Resources.Load("Inventory") as InventoryScripable;
            //newsDic = new Dictionary<int, News>();
        }
        else
        {
            Debug.Log(invenDic.Count + "뉴스 개수");
        }
        if (!newsDic.ContainsKey(collection.id))    
        {

            //if (!collection.is_collect)
            //{
            //    return;
            //}
            newsDic.Add(collection.id, SetInfoStruct(collection));
            invendata.news= newsDic;
            newsCount = invendata.news.Count;
            Debug.Log("inventory 뉴스 개수" + newsDic.Count);
            Debug.Log(string.Format("뉴스 추가 완료+{0} : {1},{2}", collection.id, invenDic[collection.id].image, invenDic[collection.id].image_name));
        }
    }

    //추후 void> int
    public void GetcurrentChaper()
    {

    }

    //여기를 나중에 collection배열을 주고 이후에 현재 챕터를 가져와야함

    public Info GetInfo(int id)
    {
        if (invenDic.ContainsKey(id))
        {
            return invenDic[id];
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



