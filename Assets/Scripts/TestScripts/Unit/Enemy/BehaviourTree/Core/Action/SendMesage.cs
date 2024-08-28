using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourTree
{
    public class SendMesage : ActionNode
    {
        public List<Mesage> mesages = new();

        private List<MethodInfo> actions;
        private BehaviourNode targetNode;

        protected override void OnEnd() { }

        protected override void OnStart() { }

        protected override NodeState OnUpdate()
        {
            foreach(var action in actions) action.Invoke(targetNode, null);
            return NodeState.Success;
        }

        private void OnEnable()
        {
            if(BehaviourNode.clone == null || blackboard.thisUnit == null) return;
            actions = new();
            foreach(var mesage in mesages)
            {
                if(mesage.GetName.Equals("None")) continue;
                try 
                {
                    targetNode = BehaviourNode.clone[(blackboard.thisUnit.GetInstanceID(), mesage.GetGUID)];
                    if(targetNode) actions.Add(targetNode.GetType().GetMethod(mesage.GetName)); 
                }
                catch (Exception e)
                { 
                    Debug.LogError(e.Message);
                    continue;
                }
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
