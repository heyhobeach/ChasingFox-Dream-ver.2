using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BehaviourTree
{
    public class SendMesage : ActionNode
    {
        public List<Mesage> mesages = new();

        protected override void OnEnd() { }

        protected override void OnStart() { }

        protected override NodeState OnUpdate()
        {
            try 
            {
                // var node = BehaviourNode.clone[(blackboard.thisUnit.GetInstanceID(), (unityEvent.GetPersistentTarget(0) as BehaviourNode).guid)];
                // if(node) node.GetType().GetMethod(unityEvent.GetPersistentMethodName(0)).Invoke(node, null);
                foreach(var mesage in mesages)
                {
                    if(mesage.GetName.Equals("None")) continue;
                    var node = BehaviourNode.clone[(blackboard.thisUnit.GetInstanceID(), mesage.GetGUID)];
                    if(node) node.GetType().GetMethod(mesage.GetName).Invoke(node, null);
                }
                return NodeState.Success;
            }
            catch (TargetException e)
            { 
                Debug.LogError(e.Message);
                return NodeState.Failure;
            }
        }

        [Serializable]
        public class Mesage
        {
            [SerializeField] BehaviourNode targetNode;
            [SerializeField] string methodName;
            [SerializeField] int methodIdx;

            public string GetGUID { get => targetNode.guid; }
            public string GetName { get => methodName; }
        }
    }
}
