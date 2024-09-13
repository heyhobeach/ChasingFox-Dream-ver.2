using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public List<EnemyUnit> enemyUnits = new List<EnemyUnit>();
    [HideInInspector] public EdgeCollider2D edgeCollider2D;
    private int enemyCount = 0;

    void Awake()
    {
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
        StartCoroutine(EndDeley());
    }

    IEnumerator EndDeley()
    {
        yield return new WaitForSeconds(1);
        // gameObject.SetActive(false);
        foreach (EnemyUnit unit in enemyUnits) unit.gameObject.SetActive(false);
    }
}
