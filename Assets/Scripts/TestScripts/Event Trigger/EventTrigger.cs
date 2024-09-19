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
    public string targetTag;
    public bool limit;
    public EventList[] eventLists;
    protected int eventIdx = 0;
    protected bool used { get => eventTriggerData.used; set => eventTriggerData.used = value; }
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
            PageManger.Instance.AddClearList(eventTriggerData);
        }
    }

    public void OnTrigger()
    {
        if(limit ? used : false) return;
        ((IBaseController)this).AddController();
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
