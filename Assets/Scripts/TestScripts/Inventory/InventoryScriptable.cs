using System.Collections.Generic;
using UnityEngine;
using static Inventory;

[CreateAssetMenu(fileName = "InventoryScripable", menuName = "Scriptable Objects/Inventory")]
public class InventoryScripable : ScriptableObject
{
    public Dictionary<int, Info> inventory;
    public Dictionary<int, News> news;
}
