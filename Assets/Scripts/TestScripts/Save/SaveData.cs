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
    public PlayerData.JsonData playerData;

    // TODO : Add inventory data

    public void Init(DateTime date, SaveData saveData)
    {
        createdTime = date;
        chapter = saveData.chapter;
        playerData = saveData.playerData;
    }
}
