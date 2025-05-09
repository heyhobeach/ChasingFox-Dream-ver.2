using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif


[RequireComponent(typeof(BoxCollider2D))]
public class EventTrigger : MonoBehaviour
{
    [SerializeField, DisableInInspector] private EventTriggerData _eventTriggerData;
    public EventTriggerData eventTriggerData
    {
        get
        {
            if(_eventTriggerData == null) Init();
            return _eventTriggerData;
        }
    }

    [SerializeField] protected GameObject[] triggerActionObjects;
    [SerializeField] protected bool endObjectEnabled = false;

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
    public EventList[] eventLists;
    protected int eventIdx { get => eventTriggerData.eventIdx; set => eventTriggerData.eventIdx = value; }
    public bool used { get => eventTriggerData.used; set => eventTriggerData.used = value; }
    public bool triggerEnabled { get => eventTriggerData.triggerEnabled; set => eventTriggerData.triggerEnabled = value; }
    protected bool eventLock;
    private Action action;

    /// <summary>
    /// 이벤트 작동부
    /// </summary>
    public virtual void Controller()
    {
        if(eventIdx >= eventLists.Length)
        {
            EventTriggerData.currentEventTriggerData = null;
            foreach(var go in triggerActionObjects) go.SetActive(endObjectEnabled);
            if(limit) used = true;
            eventIdx = 0;
            action = null;
            return;
        }
        while(eventIdx < eventLists.Length && !eventLock &&
            (eventLists[eventIdx].enterPrerequisites == null || eventLists[eventIdx].enterPrerequisites.isSatisfied) &&
            (eventLists[eventIdx].keyCode == KeyCode.None || Input.GetKeyDown(eventLists[eventIdx].keyCode)))
        {
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
        if(!triggerEnabled || limit ? used : false) return;
        action = Controller;
        EventTriggerData.currentEventTriggerData = eventTriggerData;
        foreach(var go in triggerActionObjects) go.SetActive(true);
    }
    public virtual void OnTrigger(bool triggerEnabled)
    {
        this.triggerEnabled = true;
        if(!triggerEnabled) return;
        if(!triggerEnabled || limit ? used : false) return;
        action = Controller;
        EventTriggerData.currentEventTriggerData = eventTriggerData;
        foreach(var go in triggerActionObjects) go.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(!collider.CompareTag(targetTag) || collider.GetComponent<UnitBase>()?.UnitState != UnitState.Default) return;
        targetPosition = collider.transform.position;
        OnTrigger();
    }
    protected virtual void OnTriggerExit2D(Collider2D collider)
    {
        if(!collider.CompareTag(targetTag)) return;
        action = null;
    }

    public void DataReset() => eventTriggerData.Init();
    public void Init(EventTriggerData.JsonData data)
    {
        if(data != null) eventTriggerData.Init(data);
        GetComponent<BoxCollider2D>().enabled = true;
    }
    private void Init()
    {
        var path = $"ScriptableObject Datas/{gameObject.scene.name}/EventTrigger";
        var fileName = gameObject.name.Replace('/', '_');
        _eventTriggerData = Resources.Load<EventTriggerData>($"{path}/{fileName}");
        Debug.Log($"EventTriggerData: {_eventTriggerData}");
        if(_eventTriggerData == null)
        {
            Debug.Log($"EventTriggerData not found. Creating new one at {path}");
            EventTriggerData asset = ScriptableObject.CreateInstance<EventTriggerData>();
#if UNITY_EDITOR
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory( $"Assets/Resources/{path}");
                Debug.Log($"Created directory: {path}");
            }
            asset.guid = Guid.NewGuid().ToString();
            AssetDatabase.CreateAsset(asset, $"Assets/Resources/{path}/{fileName}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            _eventTriggerData = asset;
            eventTriggerData.Init();
        }

        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<BoxCollider2D>().enabled = false;
        ServiceLocator.Get<GameManager>().eventTriggers.Add(this);
    }
    private void Awake() => Init();

    private void Update() => action?.Invoke();

    protected IEnumerator LockTime(QTE_Prerequisites prerequisites)
    {
        eventLock = true;
        yield return new WaitUntil(() => prerequisites.isSatisfied);
        eventLock = false;
    }
}
