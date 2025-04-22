using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


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

    [DisableInInspector] public MapData mapData;

    public void Reset() => mapData.Init();
    public void Init(MapData.JsonData data) => mapData.Init(data);

    void Awake()
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
            unit.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        foreach (EnemyUnit unit in enemyUnits)
        {
            unit.onDeath += () => {
                enemyCount--;
                if(enemyCount <= 0) edgeCollider2D.enabled = false;
            };
        }
    }

    public void OnStart(Vector3 pos)
    {
        if(cleared) return;
        foreach (EnemyUnit unit in enemyUnits) unit.gameObject.SetActive(true);
        if(enemyCount > 0) edgeCollider2D.enabled = true;
        mapData.used = true;
        mapData.position = pos;
        SystemManager.Instance.saveData.playerData = ServiceLocator.Get<GameManager>().player.GetComponent<Player>().DataSet();
        SystemManager.Instance.saveData.playerData.pcm = ServiceLocator.Get<GameManager>().player.GetComponent<PlayerController>().DataSet();
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
}
