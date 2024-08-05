using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class EnemyController : MonoBehaviour
{
    public BehaviorNode behaviorNode;

    void Update()
    {
        behaviorNode.OnUpdate();
    }
}
