using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : BaseController
{
    public string targetTag;
    public EventList[] eventLists;
    private int eventIdx = 0;
    private bool used = true;

    public override void Controller()
    {
        if(eventIdx < eventLists.Length && (eventLists[eventIdx].keyCode == KeyCode.None || Input.GetKeyDown(eventLists[eventIdx].keyCode))) eventLists[eventIdx++].action.Invoke();
        if(eventIdx >= eventLists.Length) RemoveController();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(used && !collider.CompareTag(targetTag)) return;
        AddController();
        used = false;
    }

    [Serializable]
    public class EventList
    {
        public KeyCode keyCode;
        public UnityEvent action;
    }
}
