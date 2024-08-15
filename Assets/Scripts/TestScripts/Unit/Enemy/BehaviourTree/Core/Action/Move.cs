using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Move : ActionNode
    {
        public float reloadTime = 1;
        float startTime = 0;
        int idx = 0;

        protected override void OnEnd() { }

        protected override void OnStart() { }

        protected override NodeState OnUpdate()
        {
            startTime += Time.deltaTime;
            if(blackboard.target != null && (startTime >= reloadTime || blackboard.FinalNodeList == null))
            {
                if(startTime >= reloadTime) startTime = 0;
                blackboard.FinalNodeList = GameManager.Instance.PathFinding(blackboard.thisUnit.transform.position + (Vector3.down * 0.5f), blackboard.target.position);
                idx = 0;
            }
            if(blackboard.FinalNodeList == null || blackboard.FinalNodeList.Count <= idx) 
            {
                idx = 0;
                return NodeState.Failure;
            }
            var b = blackboard.thisUnit.Move(new Vector2(blackboard.FinalNodeList[idx].x, blackboard.FinalNodeList[idx].y + 1));
            if(blackboard.thisUnit.transform.position== new Vector3(blackboard.FinalNodeList[idx].x, blackboard.FinalNodeList[idx].y + 1, blackboard.thisUnit.transform.position.z)) idx++;
            switch(b)
            {
                case true: return NodeState.Success;
                case false: return NodeState.Failure;
            }
        }
    }
}
