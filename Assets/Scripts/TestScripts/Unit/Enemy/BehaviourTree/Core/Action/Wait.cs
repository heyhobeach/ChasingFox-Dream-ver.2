using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Wait : ActionNode
    {
        public float waitTime;
        private float time;

        protected override void OnEnd() { }

        protected override void OnStart()
        {
            blackboard.thisUnit.SetAni(false);
            time = 0;
            blackboard.thisUnit.Move(blackboard.thisUnit.transform.position);
        }

        protected override NodeState OnUpdate()
        {
            blackboard.thisUnit.Move(blackboard.thisUnit.transform.position);
            time += Time.deltaTime;
            if (time >= waitTime) return NodeState.Success;
            else return NodeState.Running;
        }
    }
}
