using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Attack : ActionNode
    {
        public float aimingTime;
        public float delayTime;
        public int attackCount = 1;
        int attackIndex = 0;
        bool canAttack;
        // bool isAttacking;
        float time;
        Vector2 aimPos;
        Rigidbody2D targetRigidbody;
        
        protected override void OnEnd() => blackboard.thisUnit.SetAni(false);

        protected override void OnStart()
        {
            time = 0;
            attackIndex = 0;
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
            if(!canAttack) return NodeState.Failure;
            time += ServiceLocator.Get<GameManager>().ingameDeltaTime;
            if(time < aimingTime)
            {
                if(targetRigidbody) aimPos = targetRigidbody.worldCenterOfMass;
                else aimPos = blackboard.target.position;
                canAttack = blackboard.thisUnit.AttackCheck(aimPos);
                if(blackboard.thisUnit.shootingAnimationController != null) blackboard.thisUnit.shootingAnimationController.targetPosition = aimPos;
            }
            else if(time >= aimingTime+(delayTime*attackIndex)) 
            {
                blackboard.thisUnit.Attack(aimPos);
                attackIndex++;
                if(attackIndex >= attackCount) return NodeState.Success;
            }
            return NodeState.Running;
        }
    }
}
