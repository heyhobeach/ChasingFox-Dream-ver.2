using System;
using System.IO;
using UnityEngine;

// [CreateAssetMenu(fileName = "SaveData", menuName = "Scriptable Objects/SaveData")]
// public class SaveData : ScriptableObject
[Serializable]
public class SaveData
{
    public DateTime createdTime;
    
    public string chapter;

    public int karma => playerData.karma;
    public int stageIdx;
    public string currentEventTriggerDataGuid;
    public Inventory.Info[] inventoryData;
    public PlayerData.JsonData playerData;
    public MapData.JsonData[] mapData;
    public EventTriggerData.JsonData[] eventTriggerData;

    public void Init(DateTime date, SaveData saveData)
    {
        createdTime = date;
        stageIdx = saveData.stageIdx;
        currentEventTriggerDataGuid = saveData.currentEventTriggerDataGuid;
        inventoryData = saveData.inventoryData;
        chapter = saveData.chapter;
        playerData = saveData.playerData;
        mapData = saveData.mapData;
        eventTriggerData = saveData.eventTriggerData;
    }
}
