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
        public int eventIdx;
        public bool used;
        public bool triggerEnabled;
        public Vector2 targetPosition;

        public static implicit operator JsonData(EventTriggerData data)
        {
            return new JsonData {
                eventIdx = data.eventIdx,
                used = data.used,
                triggerEnabled = data.triggerEnabled,
                targetPosition = data.targetPosition
            };
        }
    }

    [DisableInInspector] public string guid;

    [SerializeField] private bool enableOnAwake = false;
    
    public int eventIdx;
    public bool used;
    public bool triggerEnabled;
    public Vector2 targetPosition;

    public static EventTriggerData currentEventTriggerData = null;

    public void Init()
    {
        eventIdx = 0;
        used = false;
        triggerEnabled = enableOnAwake;
        targetPosition = Vector2.zero;
    }
    public void Init(JsonData eventTriggerData)
    {
        eventIdx = eventTriggerData.eventIdx;
        used = eventTriggerData.used;
        triggerEnabled = eventTriggerData.triggerEnabled;
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
            eventIdx = 0;
            used = false;
            triggerEnabled = enableOnAwake;
            targetPosition = Vector2.zero;
        }
    }
#endif
}
