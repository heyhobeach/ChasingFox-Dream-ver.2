using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Events;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    public string targetTag;
    public bool autoTrigger;
    public KeyCode keyCode;
    public EventList[] eventLists;
    private int eventIdx = 0;
    private bool used = false;
    private bool eventLock;

    public void Controller()
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
        else if(eventIdx >= eventLists.Length) used = true;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(used || (autoTrigger ? false : !Input.GetKeyDown(keyCode)) || !collider.CompareTag(targetTag)) return;
        Controller();
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        if(used || (autoTrigger ? false : !Input.GetKeyDown(keyCode)) || !collider.CompareTag(targetTag)) return;
        Controller();
    }

    private IEnumerator LockTime(float lockTime)
    {
        eventLock = true;
        yield return new WaitForSecondsRealtime(lockTime);
        eventLock = false;
    }
}
