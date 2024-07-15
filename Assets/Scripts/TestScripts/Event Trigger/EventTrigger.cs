using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : BaseController
{
    public string targetTag;
    public bool autoTrigger;
    public KeyCode keyCode;
    public EventList[] eventLists;
    private int eventIdx = 0;
    private bool used = false;
    private bool eventLock;

    public override void Controller()
    {
        if(eventLock) return;
        if(eventIdx < eventLists.Length && 
            (eventLists[eventIdx].prerequisites == null || eventLists[eventIdx].prerequisites.isSatisfySatisfy) &&
            (eventLists[eventIdx].keyCode == KeyCode.None || Input.GetKeyDown(eventLists[eventIdx].keyCode)))
        {
            if(eventLists[eventIdx].action != null) eventLists[eventIdx].action.Invoke();
            StartCoroutine(LockTime(eventLists[eventIdx].lockTime));
            eventIdx++;
        }
        else if(eventIdx >= eventLists.Length) RemoveController();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(used || (autoTrigger ? false : !Input.GetKeyDown(keyCode)) || !collider.CompareTag(targetTag)) return;
        AddController();
        used = false;
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        if(used || (autoTrigger ? false : !Input.GetKeyDown(keyCode)) || !collider.CompareTag(targetTag)) return;
        AddController();
        used = false;
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
        public QTE_Prerequisites prerequisites;
        public KeyCode keyCode;
        public UnityEvent action;
        public float lockTime;
    }
}
