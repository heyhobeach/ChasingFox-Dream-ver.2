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

        public string[] keywords;

        //public Sprite image;
        public Info(News news,string context, string[] keywords)
        {
            this.news = news;
            this.news.image = news.image;
            this.news.image_name = news.image_name;
            this.context = context;
            this.keywords = keywords;
            this.news.id=news.id;   
        }
    }

    public class News
    {
        public Sprite image;
        public string image_name;
        public int id;
    }
    public  Dictionary<int, Info> invenDic;//static으로 해결은 가능한데
    public Dictionary<int, News> newsDic;//static으로 해결은 가능한데
    InventoryScripable invendata;

    /// <summary>
    /// 테스트 종료후 삭제하세요, 데이터 미리 넣어서 테스트 하기 용도입니다 추후 인벤토리 저장 기능이 생기면 필요없습니다
    /// </summary>
    [SerializeField]
    private Collection.CollectionScriptorble[] tempcollections;

    public int invenCount;
    public int newsCount;

    private void Awake()
    {
        invendata = Resources.Load("Inventory") as InventoryScripable;
        newsDic = new Dictionary<int, News>();
        invenDic = new Dictionary<int, Info>();
        //Info testInfo = new Info { "이름",};
        //AddInventory()

        
        foreach(var collection in tempcollections)//테스트 이후 삭제하세요 미리 설정한 데이터를 삽입 하는 용입니다 추후 인벤토리 저장 기능이 생기면 필요없습니다
        {
            AddInventory(collection);
        }
    }

    private Info SetInfoStruct(Collection.CollectionScriptorble collection)
    {

        News news = new News
        {
            image = collection.image,
            image_name = collection._name,
            id=collection.id
        };


        Info info=new Info(news,collection._context,collection.keywords);
        Debug.Log(string.Format("{0},{1},{2}", info.news.image_name, info.context, "collection"));
        return info;
    }

    private News SetInfoStruct(Collection.NewsScriptorble news)
    {
        News _news=new News();
        _news.image = news.image;
        _news.image_name=news.image_name;
        _news.id=news.id;
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
            Debug.Log("id추가"+SetInfoStruct(collection).news.id);
            AddInventory(SetInfoStruct(collection));
            //invenDic.Add(collection.id, SetInfoStruct(collection));
            //
            //invendata.inventory = invenDic;//이 부분은 결국 scriptorble에 저장하기 위함 아닌가? 굳이 필요한가? 구조도 추가가 아닌 덮어쓰기 처럼 보이는데
            //invenCount = invendata.inventory.Count;
        }
        else
        {
            Debug.Log("이미 있는 데이터");
        }
    }

    public void AddInventory(Info collectionInfo)//흔적
    {
        //Debug.Log("AddInventory");
        //if (invendata == null)
        //{
        //    Debug.LogError("InvendataNull");
        //    invendata = Resources.Load("Inventory") as InventoryScripable;
        //}
        //CollectionInteractive collectionInteractive = new GameObject("CollectionInteractive").AddComponent<CollectionInteractive>();    
        //if (collectionInteractive == null)
        //{
        //    Debug.Log("collectionInteractive null");
        //}
        //else
        //{
        //    collectionInteractive.CallCollectionPopup(collection);
        //}


        if (!invenDic.ContainsKey(collectionInfo.news.id))
        {
            invenDic.Add(collectionInfo.news.id, collectionInfo);

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



