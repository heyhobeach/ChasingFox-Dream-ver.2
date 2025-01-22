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
    public EventTriggerData eventTriggerData;
    /// <summary>
    /// 이벤트를 작동시킬 대상의 태그
    /// </summary>
    public string targetTag;
    private Vector2 _targetPosition;
    public Vector2 targetPosition
    {
        get => _targetPosition;
        protected set => _targetPosition = value;
    }

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
        if(eventIdx >= eventLists.Length)
        {
            if(limit) used = true;
            eventIdx = 0;
            SystemManager.Instance.UpdateDataForEventTrigger(0, 0);
            action = null;
            return;
        }
        if(eventIdx < eventLists.Length && 
            (eventLists[eventIdx].enterPrerequisites == null || eventLists[eventIdx].enterPrerequisites.isSatisfied) &&
            (eventLists[eventIdx].keyCode == KeyCode.None || Input.GetKeyDown(eventLists[eventIdx].keyCode)))
        {
            SystemManager.Instance.UpdateDataForEventTrigger(GetInstanceID(), eventIdx);
            eventLists[eventIdx].action?.Invoke();
            if(eventLists[eventIdx].exitPrerequisites != null) StartCoroutine(LockTime(eventLists[eventIdx].exitPrerequisites));
            eventIdx++;
        }
    }

    /// <summary>
    /// 트리거를 작동시키는 메서드
    /// </summary>
    public virtual void OnTrigger()
    {
        if(limit ? used : false) return;
        action = Controller;
    }
    public virtual void OnTrigger(int idx)
    {
        if(limit ? used : false) return;
#if UNITY_EDITOR
        return;
#endif
        eventIdx = idx;
        action = Controller;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(!collider.CompareTag(targetTag)) return;
        targetPosition = collider.transform.position;
        OnTrigger();
    }
    protected virtual void OnTriggerExit2D(Collider2D collider)
    {
        if(!collider.CompareTag(targetTag)) return;
        targetPosition = Vector2.zero;
        action = null;
    }

    public void Init(EventTriggerData.JsonData data) => eventTriggerData.Init(data);

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

    protected IEnumerator LockTime(QTE_Prerequisites prerequisites)
    {
        eventLock = true;
        yield return new WaitUntil(() => prerequisites.isSatisfied);
        eventLock = false;
    }
}
