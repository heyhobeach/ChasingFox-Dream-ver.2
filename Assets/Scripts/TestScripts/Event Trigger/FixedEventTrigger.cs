using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedEventTrigger : EventTrigger, IBaseController
{
    public bool autoTrigger;
    public KeyCode keyCode;

    public new void Controller()
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
        if(eventIdx >= eventLists.Length)
        {
            ((IBaseController)this).RemoveController();
            eventIdx = 0;
            PageManger.Instance.AddClearList(eventTriggerData);
        }
    }

    public new void OnTrigger()
    {
        if(limit ? used : false) return;
        used = true;
        ((IBaseController)this).AddController();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if((limit ? used : false) || (autoTrigger ? false : !Input.GetKeyDown(keyCode)) || !collider.CompareTag(targetTag)) return;
        used = true;
        ((IBaseController)this).AddController();
    }

    private IEnumerator LockTime(float lockTime)
    {
        eventLock = true;
        yield return new WaitForSecondsRealtime(lockTime);
        eventLock = false;
    }
}
