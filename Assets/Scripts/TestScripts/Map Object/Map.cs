using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public List<EnemyUnit> enemyUnits = new List<EnemyUnit>();
    private int enemyCount = 0;

    void Start()
    {
        foreach (EnemyUnit unit in enemyUnits) unit.gameObject.SetActive(false);
        enemyCount= enemyUnits.Count;
    }

    public void OnStart()
    {
        foreach (EnemyUnit unit in enemyUnits) unit.gameObject.SetActive(true);
    }
}
