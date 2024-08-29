using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Attack : ActionNode
    {
        public float aimmingTime;
        public float delayTime;
        bool canAttack;
        float time;
        Vector2 aimPos;
        
        protected override void OnEnd() => blackboard.thisUnit.ShootingAni(false);

        protected override void OnStart()
        {
            if(blackboard.thisUnit.isAttacking) return;
            time = 0;
            aimPos = blackboard.target.position;
            canAttack = blackboard.thisUnit.AttackCheck(aimPos);
            blackboard.thisUnit.Move(blackboard.thisUnit.transform.position);
            blackboard.thisUnit.ShootingAni(true);
        }

        protected override NodeState OnUpdate()
        {
            if(!canAttack || blackboard.thisUnit.isAttacking) return NodeState.Failure;
            if(time < aimmingTime)
            {
                time += Time.deltaTime;
                if(blackboard.thisUnit.AttackCheck(blackboard.target.position))
                {
                    aimPos = blackboard.target.position;
                    blackboard.thisUnit.shootingAnimationController.targetPosition = aimPos;
                }
                return NodeState.Running;
            }
            else if(time < aimmingTime+delayTime)
            {
                time += Time.deltaTime;
                return NodeState.Running;
            }
            else
            {
                switch(blackboard.thisUnit.Attack(aimPos))
                {
                    case true: return NodeState.Success;
                    case false: return NodeState.Failure;
                }
            }
        }
    }
}
