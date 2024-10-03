using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{    
    public class SequenceNode : CompositeNode
    {
        int crurent;

        protected override void OnEnd() {}

        protected override void OnStart() => crurent = 0;

        protected override NodeState OnUpdate()
        {
            do
            {
                switch(children[crurent].Update())
                {
                case NodeState.Running: return NodeState.Running;
                case NodeState.Failure: return NodeState.Failure;
                case NodeState.Success: crurent++; break;
                }
            } while(crurent < children.Count);
            return NodeState.Success;
        }
    }
}
