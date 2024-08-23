using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class BehaviourNode : ScriptableObject
    {
        public enum NodeState { Running, Failure, Success }
        [HideInInspector] public NodeState state = NodeState.Running;
        [HideInInspector] public bool isStarted = false;
        [SerializeField, DisableInspector] public string guid;
        [HideInInspector] public Vector2 positon;
        [HideInInspector] public Blackboard blackboard;
        [HideInInspector] public static Dictionary<(int, string), BehaviourNode> clone = new();
        [SerializeField, TextArea] public string description;

        public NodeState Update()
        {
            if(!isStarted)
            {
                OnStart(); 
                isStarted = true;
            }
            state = OnUpdate();
            if(state == NodeState.Failure || state == NodeState.Success) 
            {
                OnEnd(); 
                isStarted = false;
            }

            return state;
        }

        public virtual BehaviourNode Clone()
        {
            var node = Instantiate(this);
            if(clone.ContainsKey((blackboard.thisUnit.GetInstanceID(), guid))) clone[(blackboard.thisUnit.GetInstanceID(), guid)] = node;
            else clone.Add((blackboard.thisUnit.GetInstanceID(), guid), node);
            return node;
        }

        protected abstract void OnStart();
        protected abstract NodeState OnUpdate();
        protected abstract void OnEnd();

        public abstract void AddChild(BehaviourNode child);
        public abstract void RemoveChild(BehaviourNode child);
        public abstract List<BehaviourNode> GetChildren();
    }
}