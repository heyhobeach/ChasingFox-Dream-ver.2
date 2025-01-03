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
    
    private Action _onDown;
    public Action onDown { get => _onDown; set => throw new NotImplementedException(); }
    private Action _onUp;
    public Action onUp { get => _onUp; set => throw new NotImplementedException(); }

    /// <summary>
    /// 이벤트 작동부
    /// </summary>
    public new void Controller()
    {
        if(eventLock || GameManager.Instance.isPaused) return;

        if(Input.GetButtonDown("Cancel")) GameManager.Instance.Pause();

        if(eventIdx < eventLists.Length && 
            (eventLists[eventIdx].prerequisites == null || eventLists[eventIdx].prerequisites.isSatisfied) &&
            (eventLists[eventIdx].keyCode == KeyCode.None || Input.GetKeyDown(eventLists[eventIdx].keyCode)))
        {
            eventLists[eventIdx].action?.Invoke();
            if(eventLists[eventIdx].lockTime > 0) StartCoroutine(LockTime(eventLists[eventIdx].lockTime));
            eventIdx++;
        }
        if(eventIdx >= eventLists.Length)
        {
            ((IBaseController)this).RemoveController();
            eventIdx = 0;
            if(limit) used = true;
        }
    }

    /// <summary>
    /// 트리거를 작동시키는 메서드
    /// </summary>
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
}
