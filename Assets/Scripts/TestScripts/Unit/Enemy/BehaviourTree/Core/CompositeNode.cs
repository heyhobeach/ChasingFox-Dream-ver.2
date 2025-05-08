using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BehaviourTree
{
    public abstract class CompositeNode : BehaviourNode
    {
        [DisableInInspector] public List<BehaviourNode> children = new();

        public override BehaviourNode Clone()
        {
            var node = Instantiate(this);
            node.children = children.ConvertAll(n => n.Clone());
            if(clone.ContainsKey((blackboard.thisUnit.GetInstanceID(), guid))) clone[(blackboard.thisUnit.GetInstanceID(), guid)] = node;
            else clone.Add((blackboard.thisUnit.GetInstanceID(), guid), node);
            return node;
        }

#if UNITY_EDITOR
        public override void AddChild(BehaviourNode child)
        {
            Undo.RecordObject(this, "BehaviourTree AddChild");
            this.children.Add(child);
            EditorUtility.SetDirty(this);
        }
        public override void RemoveChild(BehaviourNode child)
        {
            Undo.RecordObject(this, "BehaviourTree RemoveChild");
            this.children.Remove(child);
            EditorUtility.SetDirty(this);
        }
#endif
        public override List<BehaviourNode> GetChildren() => children;
    }
}