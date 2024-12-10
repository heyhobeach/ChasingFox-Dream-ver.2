using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable, CreateAssetMenu(menuName = "ScriptableObjectDatas/MapData")]
public class MapData : ScriptableObject
{
    public Vector3 position;
    public bool used;
    public bool cleared;
    public PlayerData playerData;

    public void Init()
    {
        position = Vector3.zero;
        used = false;
        playerData = CreateInstance<PlayerData>();
        playerData.Init();
    }

#if UNITY_EDITOR
    void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
    {
        if(playModeStateChange == PlayModeStateChange.ExitingPlayMode)
        {
            used = false;
            cleared = false;
            position = Vector3.zero;
            playerData = CreateInstance<PlayerData>();
            playerData.Init();
        }
    }
#endif
}