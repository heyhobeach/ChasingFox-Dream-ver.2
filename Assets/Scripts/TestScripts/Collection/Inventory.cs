using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class Inventory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public class Info//이 부분 의존성 주입으로 수정 예정
    {
        public News news;
        //public string _name;
        public string context;
        //public Sprite image;
        public Info(News news,string context)
        {
            this.news = news;
            this.news.image = news.image;
            this.news.image_name = news.image_name;
            this.context = context;
        }
    }

    public class News
    {
        public Sprite image;
        public string image_name;
    }
    public  Dictionary<int, Info> invenDic;//static으로 해결은 가능한데
    public Dictionary<int, News> newsDic;//static으로 해결은 가능한데
    InventoryScripable invendata;

    public int invenCount;
    public int newsCount;

    private void Awake()
    {
        invendata = Resources.Load("Inventory") as InventoryScripable;
        newsDic = new Dictionary<int, News>();
        invenDic = new Dictionary<int, Info>();
    }

    private Info SetInfoStruct(Collection.CollectionScriptorble collection)
    {

        News news = new News
        {
            image = collection.image,
            image_name = collection._name
        };


        Info info=new Info(news,collection._context);
        Debug.Log(string.Format("{0},{1},{2}", info.news.image_name, info.context, "collection"));
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
    public void AddInventory(Collection.CollectionScriptorble collection)//흔적
    {
        Debug.Log("AddInventory");
        if (invendata == null)
        {
            Debug.LogError("InvendataNull");
            invendata = Resources.Load("Inventory") as InventoryScripable;
        }


        if (!invenDic.ContainsKey(collection.id))
        {
            if (!collection.is_collect)
            {
                Debug.Log("수집 하지 않는 수집품");
                return;
            }
            invenDic.Add(collection.id, SetInfoStruct(collection));

            invendata.inventory = invenDic;//이 부분은 결국 scriptorble에 저장하기 위함 아닌가? 굳이 필요한가? 구조도 추가가 아닌 덮어쓰기 처럼 보이는데
            invenCount = invendata.inventory.Count;
        }
        else
        {
            Debug.Log("이미 있는 데이터");
        }
    }
    public void AddNews(Collection.NewsScriptorble collection)//뉴스
    {
        if (invendata == null)
        {
            invendata = Resources.Load("Inventory") as InventoryScripable;
        }
        else
        {
            Debug.Log(invenDic.Count + "뉴스 개수");
        }
        if (!newsDic.ContainsKey(collection.id))    
        {
            newsDic.Add(collection.id, SetInfoStruct(collection));
            invendata.news= newsDic;//이 부분은 결국 scriptorble에 저장하기 위함 아닌가? 굳이 필요한가? 구조도 추가가 아닌 덮어쓰기 처럼 보이는데
            newsCount = invendata.news.Count;
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
    //protected Info GetInfo()//필요없는 부분처럼 보임
    //{
    //    Info info = new Info();
    //    return info;
    //}

}



