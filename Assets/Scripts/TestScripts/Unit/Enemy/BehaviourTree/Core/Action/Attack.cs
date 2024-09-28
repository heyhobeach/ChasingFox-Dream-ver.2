using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Attack : ActionNode
    {
        public float aimingTime;
        public float delayTime;
        bool canAttack;
        bool isAttacking;
        float time;
        Vector2 aimPos;
        
        protected override void OnEnd() => blackboard.thisUnit.SetAni(false);

        protected override void OnStart()
        {
            time = 0;
            isAttacking = false;
            aimPos = blackboard.target.position;
            canAttack = blackboard.thisUnit.AttackCheck(aimPos);
            blackboard.thisUnit.SetAni(true);
        }

        protected override NodeState OnUpdate()
        {
            if(isAttacking && !blackboard.thisUnit.isAttacking) return NodeState.Success;
            if(!canAttack) return NodeState.Failure;
            blackboard.thisUnit.Move(blackboard.thisUnit.transform.position);
            if(time < aimingTime)
            {
                time += Time.deltaTime;
                if(blackboard.thisUnit.AttackCheck(blackboard.target.position))
                {
                    aimPos = blackboard.target.position;
                    if(blackboard.thisUnit.shootingAnimationController != null) blackboard.thisUnit.shootingAnimationController.targetPosition = aimPos;
                }
            }
            else if(time < aimingTime+delayTime)
            {
                time += Time.deltaTime;
            }
            else if(!isAttacking && time >= aimingTime+delayTime)
            {
                blackboard.thisUnit.Attack(aimPos);
                isAttacking = true;
            }
            return NodeState.Running;
        }
    }
}
