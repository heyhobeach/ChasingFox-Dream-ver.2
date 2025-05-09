using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedEventTrigger : EventTrigger, IBaseController
{
    private Action _onDown;
    public Action onDown { get => _onDown; set => throw new NotImplementedException(); }
    private Action _onUp;
    public Action onUp { get => _onUp; set => throw new NotImplementedException(); }

    /// <summary>
    /// 이벤트 작동부
    /// </summary>
    public override void Controller()
    {
        if(Input.GetButtonDown("Cancel")) ServiceLocator.Get<GameManager>().Pause();
        
        if(ServiceLocator.Get<GameManager>().isPaused) return;
        if(eventIdx >= eventLists.Length)
        {
            EventTriggerData.currentEventTriggerData = null;
            PopupManager.Instance.RestartButtonEnable(true);
            targetPosition = Vector2.zero;
            eventIdx = 0;
            if(limit) used = true;
            ((IBaseController)this).RemoveController();
            foreach(var go in triggerActionObjects) go.SetActive(endObjectEnabled);
            return;
        }

        while(eventIdx < eventLists.Length && !eventLock &&
            (eventLists[eventIdx].enterPrerequisites == null || eventLists[eventIdx].enterPrerequisites.isSatisfied) &&
            (eventLists[eventIdx].keyCode == KeyCode.None || Input.GetKeyDown(eventLists[eventIdx].keyCode)))
        {
            eventLists[eventIdx].action?.Invoke();
            if(eventLists[eventIdx].exitPrerequisites != null) StartCoroutine(LockTime(eventLists[eventIdx].exitPrerequisites));
            eventIdx++;
        }
    }

    /// <summary>
    /// 트리거를 작동시키는 메서드
    /// </summary>
    public override void OnTrigger()
    {
        if(!triggerEnabled || limit ? used : false) return;
        EventTriggerData.currentEventTriggerData = eventTriggerData;
        ((IBaseController)this).AddController();
        PopupManager.Instance.RestartButtonEnable(false);
        foreach(var go in triggerActionObjects) go.SetActive(true);
    }
    public override void OnTrigger(bool triggerEnabled)
    {
        this.triggerEnabled = true;
        if(!triggerEnabled) return;
        if(!triggerEnabled || limit ? used : false) return;
        EventTriggerData.currentEventTriggerData = eventTriggerData;
        ((IBaseController)this).AddController();
        PopupManager.Instance.RestartButtonEnable(false);
        foreach(var go in triggerActionObjects) go.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if((autoTrigger ? false : !Input.GetKeyDown(keyCode)) 
            || !collider.CompareTag(targetTag) 
            || (prerequisites != null && !prerequisites.isSatisfied)
            || collider.GetComponent<UnitBase>()?.UnitState != UnitState.Default) return;
        targetPosition = collider.transform.position;
        OnTrigger();
    }
}
