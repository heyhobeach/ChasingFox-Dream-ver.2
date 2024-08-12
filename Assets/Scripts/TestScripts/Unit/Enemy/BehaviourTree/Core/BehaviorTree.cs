using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BehaviourTree
{
    [CreateAssetMenu(fileName = "BehaviourTree", menuName = "BehaviourTree/BehaviourTree", order = 1)]
    public class BehaviourTree : ScriptableObject
    {
        public BehaviourNode rootNode;
        public BehaviourNode.NodeState nodeState;
        public List<BehaviourNode> nodes = new();
        public Blackboard blackboard = new();
        [HideInInspector] public Vector2 viewPos = Vector2.zero;
        [HideInInspector] public Vector3 viewScale = Vector3.one;

        public BehaviourNode.NodeState Update() => rootNode.Update();

#if UNITY_EDITOR
        public BehaviourNode CreateNode(Type type)
        {
            var node = ScriptableObject.CreateInstance(type) as BehaviourNode;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            
            Undo.RecordObject(this, "BehaviourTree CreateNode");
            nodes.Add(node);

            Undo.RegisterCreatedObjectUndo(node, "BehaviourTree CreateNode");
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
            return node;
        }
        public void DeleteNode(BehaviourNode node)
        {
            Undo.RecordObject(this, "BehaviourTree DeleteNode");
            nodes.Remove(node);
            Undo.DestroyObjectImmediate(node);
            // AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(BehaviourNode parent, BehaviourNode child) => parent.AddChild(child);
        public void RemoveChild(BehaviourNode parent, BehaviourNode child) => parent.RemoveChild(child);
#endif

        public List<BehaviourNode> GetChildren(BehaviourNode parent) => parent.GetChildren();

        public void Traverse(BehaviourNode node, Action<BehaviourNode> visiter)
        {
            if(node)
            {
                visiter.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((n) => Traverse(n, visiter));
            }
        }

        public BehaviourTree Clone()
        {
            var tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new();
            Traverse(tree.rootNode, (n) => tree.nodes.Add(n));
            return tree;
        }

        public void Bind() => Traverse(rootNode, (n) => n.blackboard = blackboard);
    }
}