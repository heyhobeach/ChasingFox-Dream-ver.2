using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable, CreateAssetMenu(menuName = "ScriptableObjectDatas/EventTriggerData")]
public class EventTriggerData : ScriptableObject
{
    [Serializable]
    public class JsonData
    {
        public string guid;
        public int eventIdx;
        public bool used;
        public bool isEneable;
        public Vector2 targetPosition;

        public static implicit operator JsonData(EventTriggerData data)
        {
            return new JsonData {
                guid = data.guid,
                eventIdx = data.eventIdx,
                used = data.used,
                isEneable = data.isEneable,
                targetPosition = data.targetPosition
            };
        }
    }

    [DisableInInspector] public string guid;
    public int eventIdx;
    public bool used;
    public bool isEneable;
    public Vector2 targetPosition;

    public static EventTriggerData currentEventTriggerData = null;

    public void Init(bool enabled)
    {
        eventIdx = 0;
        used = false;
        isEneable = enabled;
        targetPosition = Vector2.zero;
    }
    public void Init(JsonData eventTriggerData)
    {
        guid = eventTriggerData.guid;
        eventIdx = eventTriggerData.eventIdx;
        used = eventTriggerData.used;
        isEneable = eventTriggerData.isEneable;
        targetPosition = eventTriggerData.targetPosition;
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
            isEneable = false;
            targetPosition = Vector2.zero;
        }
    }
#endif
}
