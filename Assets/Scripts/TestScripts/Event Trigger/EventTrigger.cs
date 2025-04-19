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
    [SerializeField, DisableInspector] private EventTriggerData _eventTriggerData;
    public EventTriggerData eventTriggerData
    {
        get
        {
            if(_eventTriggerData == null) Init();
            return _eventTriggerData;
        }
    }
    /// <summary>
    /// 이벤트를 작동시킬 대상의 태그
    /// </summary>
    public string targetTag;
    public Vector2 targetPosition
    {
        get => eventTriggerData.targetPosition;
        protected set => eventTriggerData.targetPosition = value;
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
    public virtual void Controller()
    {
        if(eventIdx >= eventLists.Length)
        {
            if(limit) used = true;
            eventIdx = 0;
            SystemManager.Instance.UpdateDataForEventTrigger(null, 0);
            action = null;
            return;
        }
        while(eventIdx < eventLists.Length && !eventLock &&
            (eventLists[eventIdx].enterPrerequisites == null || eventLists[eventIdx].enterPrerequisites.isSatisfied) &&
            (eventLists[eventIdx].keyCode == KeyCode.None || Input.GetKeyDown(eventLists[eventIdx].keyCode)))
        {
            SystemManager.Instance.UpdateDataForEventTrigger(eventTriggerData.guid, eventIdx);
            try { eventLists[eventIdx].action?.Invoke(); }
            catch (Exception e) { Debug.LogError(e); }
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
        action = null;
    }

    public void Init(EventTriggerData.JsonData data)
    {
        eventTriggerData.Init(data);
        GetComponent<BoxCollider2D>().enabled = true;
        gameObject.SetActive(eventTriggerData.isEneable);
    }

    private void Init()
    {
        var path = $"ScriptableObject Datas/{SceneManager.GetActiveScene().name}_{gameObject.name}";
        _eventTriggerData = Resources.Load<EventTriggerData>(path);
        if(_eventTriggerData == null)
        {
            EventTriggerData asset = ScriptableObject.CreateInstance<EventTriggerData>();
#if UNITY_EDITOR
            asset.guid = Guid.NewGuid().ToString();
            AssetDatabase.CreateAsset(asset, "Assets/Resources/" + path + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            _eventTriggerData = asset;
            eventTriggerData.Init(gameObject.activeSelf);
        }

        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    void Awake()
    {
        Init();
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

    private void OnEnable() => eventTriggerData.isEneable = true;
    private void OnDisable() => eventTriggerData.isEneable = false;
}
