using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    [CreateAssetMenu(fileName = "SelectorNode", menuName = "ScriptableObjects/BehaviorTree/SelectorNode")]
    public class SelectorNode : FlowControlNode
    {
        public enum SelectorType { SEQUENCE, RANDOM }
        public SelectorType selectorType;

        public override NodeState OnUpdate()
        {
            if(runningNode.Current == null || nodeState == NodeState.SECCESS) Reset();
            while(runningNode.Current != null && 
                    (nodeState = runningNode.Current.OnUpdate()) == NodeState.FAILUE) runningNode.MoveNext();
            return nodeState;
        }

        public override void Reset()
        {
            base.Reset();
            if(selectorType == SelectorType.RANDOM) Shuffle(behaviorNodes);
            runningNode.Reset();
            runningNode.MoveNext();
        }

        public override void OnStart()
        {
            throw new System.NotImplementedException();
        }

        public override void OnEnd()
        {
            throw new System.NotImplementedException();
        }
    }
}
