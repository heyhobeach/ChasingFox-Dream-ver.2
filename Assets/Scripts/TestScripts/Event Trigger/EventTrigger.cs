using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EventTrigger : MonoBehaviour
{
    public string targetTag;
    public bool autoTrigger;
    public bool limit;
    public KeyCode keyCode;
    public EventList[] eventLists;
    protected int eventIdx = 0;
    protected bool used = false;
    protected bool eventLock;
    private Action action;

    public void Controller()
    {
        if(eventLock) return;
        if(eventIdx < eventLists.Length && 
            (eventLists[eventIdx].prerequisites == null || eventLists[eventIdx].prerequisites.isSatisfySatisfy) &&
            (eventLists[eventIdx].keyCode == KeyCode.None || Input.GetKeyDown(eventLists[eventIdx].keyCode)))
        {
            if(eventLists[eventIdx].action != null) eventLists[eventIdx].action.Invoke();
            if(eventLists[eventIdx].lockTime > 0) StartCoroutine(LockTime(eventLists[eventIdx].lockTime));
            eventIdx++;
        }
        if(eventIdx >= eventLists.Length)
        {
            used = true;
            eventIdx = 0;
            action = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if((limit ? used : false) || (autoTrigger ? false : !Input.GetKeyDown(keyCode)) || !collider.CompareTag(targetTag)) return;
        action = Controller;
    }

    private void Update()
    {
        if(action != null) action.Invoke();
    }

    private IEnumerator LockTime(float lockTime)
    {
        eventLock = true;
        yield return new WaitForSecondsRealtime(lockTime);
        eventLock = false;
    }
}
