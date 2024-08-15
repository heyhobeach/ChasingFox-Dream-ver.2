using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Attack : ActionNode
    {
        protected override void OnEnd() { }

        protected override void OnStart() { }

        protected override NodeState OnUpdate()
        {
            var b = blackboard.thisUnit.Attack(blackboard.target.position);
            switch(b)
            {
                case true: return NodeState.Success;
                case false: return NodeState.Failure;
            }
        }
    }
}
