using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Map : MonoBehaviour
{
    public List<EnemyUnit> enemyUnits = new List<EnemyUnit>();
    [HideInInspector] public EdgeCollider2D edgeCollider2D;
    private int enemyCount = 0;

    public bool used { get => mapData.used; set => mapData.used = value; }
    public Vector3 position { get => mapData.position; set => mapData.position = value; }

    public class MapData : ScriptableObject
    {
        public Vector3 position;
        public bool used;


#if UNITY_EDITOR
        void OnEnable()
        {
            EditorApplication.playModeStateChanged += (playModeStateChange) => {
                if(playModeStateChange == PlayModeStateChange.ExitingPlayMode)
                {
                    used = false;
                    position = Vector3.zero;
                }
            };
        }
#endif
    }

    protected MapData mapData;


    void Awake()
    {
        var path = $"ScriptableObject Datas/{SceneManager.GetActiveScene().name}_{gameObject.name}";
        mapData = Resources.Load<MapData>(path);
#if UNITY_EDITOR
        if(!mapData)
        {
            var asset = ScriptableObject.CreateInstance<MapData>();
            AssetDatabase.CreateAsset(asset, "Assets/Resources/" + path + ".asset");
            AssetDatabase.SaveAssets();
            mapData = asset;
        }
#endif

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

    public void OnStart()
    {
        if(enemyCount <= 0) edgeCollider2D.enabled = false;
        // gameObject.SetActive(true);
        foreach (EnemyUnit unit in enemyUnits) unit.gameObject.SetActive(true);
    }
    public void OnEnd()
    {
        edgeCollider2D.enabled = true;
        foreach (EnemyUnit unit in enemyUnits) unit.gameObject.SetActive(false);
        PageManger.Instance.AddClearList(mapData);
        mapData.used = true;
        StartCoroutine(EndDeley());
    }

    IEnumerator EndDeley()
    {
        yield return new WaitForSeconds(1);
        // gameObject.SetActive(false);
        foreach (EnemyUnit unit in enemyUnits) unit.gameObject.SetActive(false);
    }
}
