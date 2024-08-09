using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class ActionNode : BehaviourNode
    {
        public override void AddChild(BehaviourNode child) { }
        public override void RemoveChild(BehaviourNode child) { }
        public override List<BehaviourNode> GetChildren() => new();
    }
}