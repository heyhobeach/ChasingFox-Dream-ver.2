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
        var dir = blackboard.target.transform.position - blackboard.thisUnit.transform.position;
        if(Mathf.Abs(dir.x) < targetFlaot && dir.y < 3 && dir.y > -2) return child.Update();
        else return NodeState.Failure;
    }
}
