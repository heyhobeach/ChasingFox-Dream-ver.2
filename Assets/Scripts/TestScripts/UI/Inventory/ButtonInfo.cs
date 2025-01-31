using Collection;
using System;
using UnityEngine;

public class ButtonInfo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Tuple<int,Inventory.Info> button_info;
    public Tuple<int,Inventory.News> button_news;
    GameObject gobj;

    private void Awake()
    {
        gobj = this.transform.parent.transform.parent.transform.parent.transform.parent.transform.parent.gameObject;//is_trace 변수 가져오려고
        //Debug.Log("gobj"+ gobj.name);
    }

    public void SetSelectNum()
    {
        if (gobj.GetComponent<InventoryController>().is_trace)
        {
            InventoryController.select_num = this.button_info.Item1;
        }
        else
        {
            InventoryController.select_num = this.button_news.Item1;
        }

    }

    //public void Setnewsnum()
}
