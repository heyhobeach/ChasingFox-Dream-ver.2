using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;
using UnityEngine.Events;
using Unity.VisualScripting;





#if UNITY_EDITOR
using UnityEditor;
#endif

public class Map : MonoBehaviour
{
    public List<EnemyUnit> enemyUnits = new List<EnemyUnit>();
    [HideInInspector] public EdgeCollider2D edgeCollider2D;
    private int enemyCount = 0;

    public bool enemyClear { get => enemyCount <= 0; }

    public bool used { get => mapData.used; set => mapData.used = value; }
    public bool cleared { get => mapData.cleared; set => mapData.cleared = value; }
    public Vector3 position { get => mapData.position; set => mapData.position = value; }
    public PlayerData.JsonData playerData { get => mapData.playerData; set => mapData.playerData = value; }

    [DisableInInspector] public MapData mapData;

    public float Timelimit = -1f;
    [SerializeField, DisableInInspector] private float playTime = 0f;
    public UnityAction timeoutAction;
    public List<MapEvent> mapEvents = new List<MapEvent>();
    private Queue<MapEvent> mapEventQueue;

    public void SortTimedEvents() => mapEvents.Sort();


    public void DataReset() => mapData.Init();
    public void Init(MapData.JsonData data) => mapData.Init(data);

    private void Awake()
    {
        var path = $"ScriptableObject Datas/{SceneManager.GetActiveScene().name}_{gameObject.name}";
        mapData = Resources.Load<MapData>(path);
        if(!mapData)
        {
            MapData asset = ScriptableObject.CreateInstance<MapData>();
#if UNITY_EDITOR
            asset.guid = Guid.NewGuid().ToString();
            AssetDatabase.CreateAsset(asset, "Assets/Resources/" + path + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            mapData = asset;
            mapData.Init();
        }

        enemyCount= enemyUnits.Count;
        // gameObject.SetActive(false);
        foreach (EnemyUnit unit in enemyUnits)
        {
            if(!unit.IsUnityNull()) unit.gameObject.SetActive(false);
            else Debug.LogError($"EnemyUnit not set in {gameObject.name}");
        }

        mapEventQueue = new Queue<MapEvent>(mapEvents);

        ServiceLocator.Get<GameManager>().maps.Add(this);
    }

    void Start()
    {
        foreach (EnemyUnit unit in enemyUnits)
        {
            if(!unit.IsUnityNull()) unit.onDeath += EnemyDeathAction;
            else Debug.LogError($"EnemyUnit not set in {gameObject.name}");
        }
    }

    public async void OnStart(Vector3 pos)
    {
        if(cleared) return;
        foreach (EnemyUnit unit in enemyUnits) unit.gameObject.SetActive(true);
        if(enemyCount > 0) edgeCollider2D.enabled = true;
        mapData.used = true;
        mapData.position = pos;
        mapData.playerData = ServiceLocator.Get<GameManager>().player.GetComponent<Player>().GetJsonData();
        if(Timelimit > 0) await TimeUpdate();
    }
    public void OnEnd()
    {
        if(ServiceLocator.Get<GameManager>().player.GetComponent<Player>().UnitState == UnitState.Death) return;
        edgeCollider2D.enabled = false;
        mapData.cleared = true;
        foreach (EnemyUnit unit in enemyUnits) unit.gameObject.SetActive(false);
        StartCoroutine(EndDeley());
    }

    IEnumerator EndDeley()
    {
        yield return new WaitForSeconds(3);
        // gameObject.SetActive(false);
        foreach (EnemyUnit unit in enemyUnits) unit.gameObject.SetActive(false);
    }
    
    async Awaitable TimeUpdate()
    {
        while (!mapData.cleared && playTime < Timelimit)
        {
            await Awaitable.FixedUpdateAsync();
            if(!ServiceLocator.Get<GameManager>().isPaused)
            {
                playTime += Time.fixedDeltaTime;
                if(mapEventQueue.Count > 0 && mapEventQueue.Peek().time >= playTime) mapEventQueue.Dequeue().action?.Invoke();
            }
        }
        if(!mapData.cleared) timeoutAction?.Invoke();
    }

    private void EnemyDeathAction()
    {
        enemyCount--;
        if(enemyCount <= 0) edgeCollider2D.enabled = false;
    }

    public void AddEnemyUnit(EnemyUnit unit)
    {
        enemyUnits.Add(unit);
        unit.onDeath += EnemyDeathAction;
        enemyCount++;
    }
    public void RemoveEnemyUnit(EnemyUnit unit)
    {
        enemyUnits.Remove(unit);
        unit.onDeath -= EnemyDeathAction;
        enemyCount--;
    }
}
