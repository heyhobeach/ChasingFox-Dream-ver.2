using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public BehaviourTree.BehaviourTree behaviorTree;

    void Start()
    {
        behaviorTree = behaviorTree.Clone();
        behaviorTree.Bind();
    }
    void Update()
    {
        behaviorTree.Update();
    }
}
