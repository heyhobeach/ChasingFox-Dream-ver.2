using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable, CreateAssetMenu(menuName = "ScriptableObjectDatas/MapData")]
public class MapData : ScriptableObject
{
    [Serializable]
    public class JsonData
    {
        public bool used;
        public bool cleared;
        public Vector3 position;

        public static implicit operator JsonData(MapData data)
        {
            return new JsonData {
                used = data.used,
                cleared = data.cleared,
                position = data.position
            };
        }
    }

    [DisableInInspector] public string guid;
    public Vector3 position;
    public bool used;
    public bool cleared;

    public void Init()
    {
        position = Vector3.zero;
        used = false;
        cleared = false;
    }

    public void Init(JsonData mapData)
    {
        position = mapData.position;
        used = mapData.used;
        cleared = mapData.cleared;
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
        }
    }
#endif
}