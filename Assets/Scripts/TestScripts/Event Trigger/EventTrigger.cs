using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(BoxCollider2D))]
public class EventTrigger : MonoBehaviour
{
    protected EventTriggerData eventTriggerData;
    /// <summary>
    /// 이벤트를 작동시킬 대상의 태그
    /// </summary>
    public string targetTag;

    /// <summary>
    /// <para>이벤트 작동 횟수 제한 여부</para>
    /// <para>True == 1회 제한</para>
    /// <para>False == 무제한</para>
    /// </summary>
    public bool limit;
    public EventList[] eventLists;
    protected int eventIdx = 0;
    public bool used { get => eventTriggerData.used; set => eventTriggerData.used = value; }
    protected bool eventLock;
    private Action action;

    /// <summary>
    /// 이벤트 작동부
    /// </summary>
    public void Controller()
    {
        if(eventLock) return;
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
            if(limit) used = true;
            eventIdx = 0;
            action = null;
        }
    }

    /// <summary>
    /// 트리거를 작동시키는 메서드
    /// </summary>
    public void OnTrigger()
    {
        if(limit ? used : false) return;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if((limit ? used : false) || !collider.CompareTag(targetTag)) return;
        action = Controller;
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if(!collider.CompareTag(targetTag)) return;
        action = null;
    }

    void Awake()
    {
        var path = $"ScriptableObject Datas/{SceneManager.GetActiveScene().name}_{gameObject.name}";
        eventTriggerData = Resources.Load<EventTriggerData>(path);
        if(!eventTriggerData)
        {
            EventTriggerData asset = ScriptableObject.CreateInstance<EventTriggerData>();
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(asset, "Assets/Resources/" + path + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            eventTriggerData = asset;
        }
        GetComponent<BoxCollider2D>().isTrigger = true;
        if(GameManager.Instance) GameManager.Instance.eventTriggers.Add(this);
    }

    private void Update()
    {
        if(action != null) action.Invoke();
    }

    protected IEnumerator LockTime(float lockTime)
    {
        eventLock = true;
        yield return new WaitForSecondsRealtime(lockTime);
        eventLock = false;
    }
}
