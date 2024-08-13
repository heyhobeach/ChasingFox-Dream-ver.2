using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace BehaviourTree
{
    public class SendMesege : ActionNode
    {
        public UnityEvent unityEvent;

        protected override void OnEnd() { }

        protected override void OnStart() { }

        protected override NodeState OnUpdate()
        {
            try 
            {
                var node = BehaviourNode.clone[(blackboard.thisUnit.GetInstanceID(), (unityEvent.GetPersistentTarget(0) as BehaviourNode).guid)];
                if(node) node.GetType().GetMethod(unityEvent.GetPersistentMethodName(0)).Invoke(node, null);
                return NodeState.Success;
            }
            catch (TargetException e)
            { 
                Debug.LogError(e.Message);
                return NodeState.Failure;
            }
        }
    }
}
