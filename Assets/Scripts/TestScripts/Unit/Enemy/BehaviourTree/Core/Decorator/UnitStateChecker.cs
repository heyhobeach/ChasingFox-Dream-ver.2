using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

public class UnitStateChecker : DecoratorNode
{
    public UnitState stateCase;

    protected override void OnEnd() { }

    protected override void OnStart() { }

    protected override NodeState OnUpdate()
    {
        if(blackboard.thisUnit.UnitState == stateCase) return child.Update();
        return NodeState.Failure;
    }
}
