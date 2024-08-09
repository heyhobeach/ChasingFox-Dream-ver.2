using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class DebugLogNode : ActionNode
    {
        public string message;

        protected override void OnEnd()
        {
            Debug.Log("OnEnd : " + message);
        }

        protected override void OnStart()
        {
            Debug.Log("OnStart : " + message);
        }

        protected override NodeState OnUpdate()
        {
            Debug.Log("OnUpdate : " + message);
            return NodeState.SUCCESS;
        }
    }
}