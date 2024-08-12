using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEditor;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class DecoratorNode : BehaviourNode
    {
        public BehaviourNode child;

        public override BehaviourNode Clone()
        {
            var node = Instantiate(this);
            node.child = child.Clone();
            clone = node;
            return node;
        }

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
        public override List<BehaviourNode> GetChildren()
        {
            List<BehaviourNode> temp = new();
            if(child != null) temp.Add(child);
            
            return temp;
        }
    }
}