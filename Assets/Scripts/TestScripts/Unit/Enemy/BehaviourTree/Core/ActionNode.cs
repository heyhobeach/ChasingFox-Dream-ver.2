using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class ActionNode : BehaviourNode
    {
#if UNITY_EDITOR
        public override void AddChild(BehaviourNode child) { }
        public override void RemoveChild(BehaviourNode child) { }
#endif
        public override List<BehaviourNode> GetChildren() => new();
    }
}