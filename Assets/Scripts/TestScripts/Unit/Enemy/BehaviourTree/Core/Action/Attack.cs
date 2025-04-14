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
        Rigidbody2D targetRigidbody;
        
        protected override void OnEnd() {}

        protected override void OnStart()
        {
            time = 0;
            isAttacking = false;
            if(blackboard.target) targetRigidbody = blackboard.target.GetComponent<Rigidbody2D>();
            if(!targetRigidbody) targetRigidbody = blackboard.target.GetComponent<PlayerUnit>()?.rg;
            if(targetRigidbody) aimPos = targetRigidbody.worldCenterOfMass;
            else aimPos = blackboard.target.position;
            blackboard.thisUnit.SetFlipX(Mathf.Sign((blackboard.thisUnit.transform.position-blackboard.target.transform.position).x) > 0);
            canAttack = blackboard.thisUnit.AttackCheck(aimPos);
            blackboard.thisUnit.Move(blackboard.thisUnit.transform.position);
            blackboard.thisUnit.SetAni(true);
        }

        protected override NodeState OnUpdate()
        {
            if(isAttacking && !blackboard.thisUnit.isAttacking) return NodeState.Success;
            if(!canAttack) 
            {
                blackboard.thisUnit.SetAni(false);
                return NodeState.Failure;
            }
            if(time < aimingTime)
            {
                time += Time.deltaTime;
                if(targetRigidbody) aimPos = targetRigidbody.worldCenterOfMass;
                else aimPos = blackboard.target.position;
                canAttack = blackboard.thisUnit.AttackCheck(aimPos);
                if(blackboard.thisUnit.shootingAnimationController != null) blackboard.thisUnit.shootingAnimationController.targetPosition = aimPos;
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
