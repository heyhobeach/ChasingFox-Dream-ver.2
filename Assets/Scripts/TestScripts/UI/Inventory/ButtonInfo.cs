using Collection;
using System;
using UnityEngine;

public class ButtonInfo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Tuple<int,Inventory.Info> button_info;


    public void SetSelectNum()
    {
        InventoryController.select_num=this.button_info.Item1;
    }
}
