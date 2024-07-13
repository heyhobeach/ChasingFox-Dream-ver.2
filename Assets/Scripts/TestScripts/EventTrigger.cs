using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : BaseController
{
    public string targetTag;
    public EventList[] eventLists;
    private int eventIdx = 0;
    private bool used = true;
    private bool eventLock;

    public override void Controller()
    {
        if(eventLock) return;
        if(eventIdx < eventLists.Length && (eventLists[eventIdx].keyCode == KeyCode.None || Input.GetKeyDown(eventLists[eventIdx].keyCode)))
        {
            eventLists[eventIdx].action.Invoke();
            StartCoroutine(LockTime(eventLists[eventIdx].lockTime));
            eventIdx++;
        }
        else if(eventIdx >= eventLists.Length) RemoveController();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(used && !collider.CompareTag(targetTag)) return;
        AddController();
        used = false;
        // UnityEventTools.AddVoidPersistentListener(this, "", "");
    }

    private IEnumerator LockTime(float lockTime)
    {
        eventLock = true;
        yield return new WaitForSecondsRealtime(lockTime);
        eventLock = false;
    }

    [Serializable]
    public class EventList
    {
        public KeyCode keyCode;
        public UnityEvent action;
        public float lockTime;
    }
}
