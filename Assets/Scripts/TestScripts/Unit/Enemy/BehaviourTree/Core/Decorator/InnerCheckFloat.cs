using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

public class InnerCheckFloat : DecoratorNode
{
    public float targetFlaot;

    protected override void OnEnd() { }

    protected override void OnStart() { }
    protected override NodeState OnUpdate()
    {
        if(targetFlaot > (blackboard.thisUnit.transform.position-blackboard.target.position).magnitude) return child.Update();
        else return NodeState.Failure;
    }
}
