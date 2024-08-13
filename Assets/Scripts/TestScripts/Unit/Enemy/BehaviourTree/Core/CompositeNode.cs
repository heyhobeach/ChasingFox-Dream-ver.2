using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class CompositeNode : BehaviourNode
    {
        public List<BehaviourNode> children = new();

        public override BehaviourNode Clone()
        {
            var node = Instantiate(this);
            node.children = children.ConvertAll(n => n.Clone());
            if(clone.ContainsKey((blackboard.thisUnit.GetInstanceID(), guid))) clone[(blackboard.thisUnit.GetInstanceID(), guid)] = node;
            else clone.Add((blackboard.thisUnit.GetInstanceID(), guid), node);
            return node;
        }

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
        public override List<BehaviourNode> GetChildren() => children;
    }
}