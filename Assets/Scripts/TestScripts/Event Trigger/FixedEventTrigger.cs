using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedEventTrigger : EventTrigger, IBaseController
{
    /// <summary>
    /// 콜라이더 진입 시 이벤트 자동 트리거 여부
    /// </summary>
    public bool autoTrigger;
    /// <summary>
    /// autoTrigger가 false 시 트리거를 작동시키게 할 KeyCode
    /// <para>후에 키 설정 도입 시 GetButtonDown기반 (string)으로 변경 필요</para>
    /// </summary>
    public KeyCode keyCode;
    public QTE_Prerequisites prerequisites;
    
    private Action _onDown;
    public Action onDown { get => _onDown; set => throw new NotImplementedException(); }
    private Action _onUp;
    public Action onUp { get => _onUp; set => throw new NotImplementedException(); }

    /// <summary>
    /// 이벤트 작동부
    /// </summary>
    public new void Controller()
    {
        if(Input.GetButtonDown("Cancel")) ServiceLocator.Get<GameManager>().Pause();
        
        if(ServiceLocator.Get<GameManager>().isPaused) return;
        if(eventIdx >= eventLists.Length)
        {
            SystemManager.Instance.UpdateDataForEventTrigger(null, 0);
            PopupManager.Instance.RestartButtonEnable(true);
            targetPosition = Vector2.zero;
            eventIdx = 0;
            if(limit) used = true;
            ((IBaseController)this).RemoveController();
            return;
        }

        while(eventIdx < eventLists.Length && !eventLock &&
            (eventLists[eventIdx].enterPrerequisites == null || eventLists[eventIdx].enterPrerequisites.isSatisfied) &&
            (eventLists[eventIdx].keyCode == KeyCode.None || Input.GetKeyDown(eventLists[eventIdx].keyCode)))
        {
            SystemManager.Instance.UpdateDataForEventTrigger(eventTriggerData.guid, eventIdx);
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
        if(limit ? used : false) return;
        ((IBaseController)this).AddController();
        PopupManager.Instance.RestartButtonEnable(false);
    }
    public override void OnTrigger(int idx)
    {
        if(limit ? used : false) return;
        eventIdx = idx;
        ((IBaseController)this).AddController();
        PopupManager.Instance.RestartButtonEnable(false);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if((autoTrigger ? false : !Input.GetKeyDown(keyCode)) || !collider.CompareTag(targetTag) || (prerequisites != null && !prerequisites.isSatisfied)) return;
        targetPosition = collider.transform.position;
        OnTrigger();
    }
}
