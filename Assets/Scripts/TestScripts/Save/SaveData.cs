using System;
using System.IO;
using UnityEngine;

// [CreateAssetMenu(fileName = "SaveData", menuName = "Scriptable Objects/SaveData")]
// public class SaveData : ScriptableObject
public class SaveData
{
    public DateTime createdTime;
    
    public string chapter;
    public int chapterIdx;

    public MapData[] mapDatas;
    public EventTriggerData[] eventTriggerDatas;

    public int eventTriggerInstanceID;
    public int eventIdx;

    public int karma;

    // TODO : Add inventory data

    public void Init(DateTime date, SaveData saveData)
    {
        createdTime = date;
        chapter = saveData.chapter;
        chapterIdx = saveData.chapterIdx;
        mapDatas = saveData.mapDatas;
        eventTriggerDatas = saveData.eventTriggerDatas;
        eventTriggerInstanceID = saveData.eventTriggerInstanceID;
        eventIdx = saveData.eventIdx;
        karma = saveData.karma;
    }
}
