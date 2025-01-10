using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable, CreateAssetMenu(menuName = "ScriptableObjectDatas/EventTriggerData")]
public class EventTriggerData : ScriptableObject
{
    public string path;
    public bool used;

    public void Init()
    {
        used = false;
    }
    public void Init(EventTriggerData eventTriggerData)
    {
        used = eventTriggerData.used;
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
        }
    }
#endif
}
