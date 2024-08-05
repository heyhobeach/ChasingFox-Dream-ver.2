using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    [CreateAssetMenu(fileName = "SequenceNode", menuName = "ScriptableObjects/BehaviorTree/SequenceNode")]
    public class SequenceNode : FlowControlNode
    {
        public enum SequenceType { SEQUENCE, RANDOM }
        public SequenceType sequenceType;

        public override NodeState OnUpdate()
        {
            if(runningNode == null || nodeState == NodeState.FAILUE) Reset();
            nodeState = runningNode.Current.OnUpdate();
            if(nodeState == NodeState.SECCESS) runningNode.MoveNext();
            return nodeState;
        }

        public override void Reset()
        {
            base.Reset();
            if(sequenceType == SequenceType.RANDOM) Shuffle(behaviorNodes);
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
