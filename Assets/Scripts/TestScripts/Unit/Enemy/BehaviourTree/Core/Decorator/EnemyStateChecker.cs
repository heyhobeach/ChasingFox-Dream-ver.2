using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

public class EnemyStateChecker : DecoratorNode
{
    public Blackboard.Enemy_State.StateCase stateCase;

    protected override void OnEnd() { }

    protected override void OnStart() { }

    protected override NodeState OnUpdate()
    {
        if(blackboard.enemy_state.stateCase == stateCase && blackboard.enemy_state.recognition) return child.Update();
        return NodeState.Failure;
    }
}
