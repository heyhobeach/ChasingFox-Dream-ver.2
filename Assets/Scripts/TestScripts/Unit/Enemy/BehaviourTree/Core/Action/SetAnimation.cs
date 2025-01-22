using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class SetAnimation : ActionNode
    {
        protected override void OnEnd() { }
        protected override void OnStart() => blackboard.thisUnit.SetAni(false);

        protected override NodeState OnUpdate()
        {
            // blackboard.thisUnit.anim.s
            return NodeState.Success;
        }
    }
}
