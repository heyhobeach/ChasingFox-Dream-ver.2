using Collection;
using System;
using UnityEngine;

public class ButtonInfo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Tuple<int,Inventory.Info> button_info;
    public Tuple<int,Inventory.News> button_news;


    public void SetSelectNum()
    {
        InventoryController.select_num=this.button_info.Item1;
    }

    //public void Setnewsnum()// 여기 내용이 왜 없지?
}
