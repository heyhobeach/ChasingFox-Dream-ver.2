using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace BehaviourTree
{
    public class Move : ActionNode
    {
        public float reloadTime = 1;
        float startTime = 0;
        private bool isRunning;

        protected override void OnEnd() { }

        protected override void OnStart() { }

        protected override NodeState OnUpdate()
        {
            if(!isRunning) startTime += Time.deltaTime;
            if(!isRunning && blackboard.target != null && (startTime >= reloadTime || blackboard.FinalNodeList == null))
            {
                if(startTime >= reloadTime) startTime = 0;
                GetPathAsync();
                return NodeState.Running;
            }
            if(blackboard.FinalNodeList == null || blackboard.FinalNodeList.Count <= blackboard.nodeIdx) 
            {
                blackboard.nodeIdx = 0;
                return NodeState.Failure;
            }
            var b = blackboard.thisUnit.Move(new Vector2(blackboard.FinalNodeList[blackboard.nodeIdx].x, blackboard.FinalNodeList[blackboard.nodeIdx].y + 1));
            if((blackboard.thisUnit.transform.position - new Vector3(blackboard.FinalNodeList[blackboard.nodeIdx].x, blackboard.FinalNodeList[blackboard.nodeIdx].y + 1, blackboard.thisUnit.transform.position.z)).magnitude < 0.1f) blackboard.nodeIdx++;
            switch(b)
            {
                case true: return NodeState.Success;
                case false: return NodeState.Failure;
            }
        }

        private async void GetPathAsync()
        {
            if(isRunning) return;
            isRunning = true;
            try
            {
                var startPos = blackboard.thisUnit.transform.position + (Vector3.down * (blackboard.thisUnit.BoxSizeY+0.1f));
                var targetPos = blackboard.target.position;
                List<GameManager.Node> nodes = null;
                await Task.Run(() => {
                    nodes = GameManager.Instance.PathFinding(startPos, targetPos);
                    if(nodes != null) blackboard.FinalNodeList = nodes;
                });
                if(nodes != null && nodes.Count > 1)
                {
                    blackboard.FinalNodeList = nodes;
                    blackboard.nodeIdx = 0;
                    while((new Vector3(nodes[blackboard.nodeIdx].x, nodes[blackboard.nodeIdx].y)-blackboard.thisUnit.transform.position).magnitude > (new Vector3(nodes[blackboard.nodeIdx+1].x, nodes[blackboard.nodeIdx+1].y)-blackboard.thisUnit.transform.position).magnitude) blackboard.nodeIdx++;
                    if((new Vector3(nodes[blackboard.nodeIdx].x, nodes[blackboard.nodeIdx].y)-blackboard.thisUnit.transform.position).magnitude < 1) blackboard.nodeIdx++;
                }
                isRunning = false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                isRunning = false;
            }
        }
    }
}
