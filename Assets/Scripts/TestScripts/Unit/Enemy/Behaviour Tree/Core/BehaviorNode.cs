using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.ComponentModel;

namespace BehaviourTree
{
    public abstract class BehaviourNode : ScriptableObject
    {
        public enum NodeState { RUNNING, FAILURE, SUCCESS }
        [HideInInspector] public NodeState state = NodeState.RUNNING;
        [HideInInspector] public bool isStarted = false;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 positon;
        [HideInInspector] public Blackboard blackboard;
        [TextArea] public string description;

        public NodeState Update()
        {
            if(!isStarted)
            {
                OnStart(); 
                isStarted = true;
            }
            state = OnUpdate();
            if(state == NodeState.FAILURE || state == NodeState.SUCCESS) 
            {
                OnEnd(); 
                isStarted = false;
            }

            return state;
        }

        public virtual BehaviourNode Clone() => Instantiate(this);

        protected abstract void OnStart();
        protected abstract NodeState OnUpdate();
        protected abstract void OnEnd();

        public abstract void AddChild(BehaviourNode child);
        public abstract void RemoveChild(BehaviourNode child);
        public abstract List<BehaviourNode> GetChildren();
    }
}
