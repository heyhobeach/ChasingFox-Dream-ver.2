using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class SetAnimation : ActionNode
    {
        protected override void OnEnd() { }
        protected override void OnStart() { }

        protected override NodeState OnUpdate()
        {
            // blackboard.thisUnit.anim.s
            return NodeState.Success;
        }
    }
}
