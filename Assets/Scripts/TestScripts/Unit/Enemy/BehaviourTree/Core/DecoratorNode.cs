using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BehaviourTree
{
    public abstract class DecoratorNode : BehaviourNode
    {
        [DisableInInspector] public BehaviourNode child;

        public override BehaviourNode Clone()
        {
            var node = Instantiate(this);
            node.child = child.Clone();
            if(clone.ContainsKey((blackboard.thisUnit.GetInstanceID(), guid))) clone[(blackboard.thisUnit.GetInstanceID(), guid)] = node;
            else clone.Add((blackboard.thisUnit.GetInstanceID(), guid), node);
            return node;
        }

#if UNITY_EDITOR
        public override void AddChild(BehaviourNode child)
        {
            Undo.RecordObject(this, "BehaviourTree AddChild");
            this.child = child;
            EditorUtility.SetDirty(this);
        }
        public override void RemoveChild(BehaviourNode child)
        {
            Undo.RecordObject(this, "BehaviourTree RemoveChild");
            this.child = null;
            EditorUtility.SetDirty(this);
        }
#endif
        public override List<BehaviourNode> GetChildren()
        {
            List<BehaviourNode> temp = new();
            if(child != null) temp.Add(child);
            
            return temp;
        }
    }
}