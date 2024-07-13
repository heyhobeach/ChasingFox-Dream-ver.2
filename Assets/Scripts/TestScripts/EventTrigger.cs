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

    public override void Controller()
    {
        if(eventLists[eventIdx].keyCode == KeyCode.None || Input.GetKeyDown(eventLists[eventIdx].keyCode)) eventLists[eventIdx++].action.Invoke();
        if(eventIdx >= eventLists.Length) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(!collider.CompareTag(targetTag)) return;
        AddController();
    }

    [Serializable]
    public class EventList
    {
        public KeyCode keyCode;
        public UnityEvent action;
    }
}
