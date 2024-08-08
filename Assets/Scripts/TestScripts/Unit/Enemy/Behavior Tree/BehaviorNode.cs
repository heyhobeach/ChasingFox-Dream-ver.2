using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BehaviorTree
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), true)]
    public abstract class BehaviorNode : ScriptableObject
    {
        public enum NodeState { FAILUE, RUNNING, SECCESS }
        public NodeState state;

        public abstract void OnStart();
        public abstract NodeState OnUpdate();
        public abstract void OnEnd();
    }
}
