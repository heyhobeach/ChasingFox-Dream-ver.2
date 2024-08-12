using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class DebugLogNode : ActionNode
    {
        public enum LogType { Defalut, Warning, Error }
        public LogType debugLogType;
        [TextArea] public string message;

        protected override void OnEnd() { }

        protected override void OnStart()
        {
            switch(debugLogType)
            {
                case LogType.Defalut: Debug.Log(message); break;
                case LogType.Warning: Debug.LogWarning(message); break;
                case LogType.Error: Debug.LogError(message); break;
            }
        }

        protected override NodeState OnUpdate() => NodeState.Success;

    }
}