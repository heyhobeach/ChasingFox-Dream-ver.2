using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

public class SequenceNode : CompositeNode
{
    int crurent;

    protected override void OnEnd() {}

    protected override void OnStart() => crurent = 0;

    protected override NodeState OnUpdate()
    {
        switch(children[crurent].Update())
        {
            case NodeState.RUNNING: return NodeState.RUNNING;
            case NodeState.FAILURE: return NodeState.FAILURE;
            case NodeState.SUCCESS: crurent++; break;
        }
        return crurent == children.Count ? NodeState.SUCCESS : NodeState.RUNNING;
    }
}
