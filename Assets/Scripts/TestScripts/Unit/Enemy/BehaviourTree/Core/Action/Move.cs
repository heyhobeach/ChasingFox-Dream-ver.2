using JetBrains.Annotations;
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
        private Vector2 moveDir;

        protected override void OnEnd() => blackboard.thisUnit.Move(blackboard.thisUnit.transform.position);

        protected override void OnStart() => blackboard.thisUnit.SetAni(false);

        protected override NodeState OnUpdate()
        {
            if(blackboard.target != null && (blackboard.target.transform.position-blackboard.thisUnit.transform.position).magnitude < blackboard.thisUnit.attackDistance * 0.7f)
            {
                blackboard.thisUnit.SetFlipX(Mathf.Sign((blackboard.thisUnit.transform.position-blackboard.target.transform.position).x) > 0);
                return NodeState.Success;
            }
            if(!isRunning) startTime += Time.deltaTime;
            if(!isRunning && blackboard.target != null && (startTime >= reloadTime || blackboard.FinalNodeList == null))
            {
                // Debug.Log("Call Path");
                if(startTime >= reloadTime) startTime = 0;
                GetPathAsync();
                return NodeState.Running;
            }
            if(blackboard.FinalNodeList == null) 
            {
                // Debug.LogError("Path is Null");
                return NodeState.Failure;
            }
            if(blackboard.FinalNodeList.Count <= blackboard.nodeIdx) 
            {
                // Debug.LogError("Out of Index");
                while(blackboard.FinalNodeList.Count <= blackboard.nodeIdx) blackboard.nodeIdx--;
                return NodeState.Failure;
            }
            var tempDir = new Vector3(blackboard.FinalNodeList[blackboard.nodeIdx].x+GameManager.Instance.correctionPos.x, blackboard.FinalNodeList[blackboard.nodeIdx].y);
            moveDir = tempDir - blackboard.thisUnit.transform.position;
            moveDir = moveDir.normalized;
            blackboard.thisUnit.Move(tempDir);
            if((blackboard.thisUnit.transform.position - tempDir).magnitude < 0.1f) blackboard.nodeIdx++;
            return NodeState.Running;
        }

        [MesageTarget] public async void GetPathAsync()
        {
            if(isRunning) return;
            isRunning = true;
            try
            {
                // Debug.Log("Start Path find");
                Vector2 startPos = Vector2.zero;
                if(blackboard.FinalNodeList != null && blackboard.FinalNodeList.Count > 0)
                {
                    if(blackboard.FinalNodeList.Count-1 < blackboard.nodeIdx) blackboard.nodeIdx = blackboard.FinalNodeList.Count-1;
                    if(blackboard.nodeIdx < 0) blackboard.nodeIdx = 0;
                    startPos = new Vector2(blackboard.FinalNodeList[blackboard.nodeIdx].x, blackboard.FinalNodeList[blackboard.nodeIdx].y);
                }
                else startPos = blackboard.thisUnit.transform.position;
                var targetPos = blackboard.target.position;
                List<GameManager.Node> nodes = null;
                await Task.Run(() => {
                    // Debug.Log("Path found0");
                    nodes = GameManager.Instance.PathFinding(startPos, targetPos);
                    if(nodes != null) blackboard.FinalNodeList = nodes;
                    // Debug.Log("Path found1");
                });
                if(nodes != null && nodes.Count > 1)
                {
                    blackboard.FinalNodeList = nodes;
                    blackboard.nodeIdx = 0;
                    // Debug.Log("Path found2");
                    while(blackboard.nodeIdx+1 < nodes.Count && (new Vector3(nodes[blackboard.nodeIdx].x, nodes[blackboard.nodeIdx].y)-blackboard.thisUnit.transform.position).magnitude > (new Vector3(nodes[blackboard.nodeIdx+1].x, nodes[blackboard.nodeIdx+1].y)-blackboard.thisUnit.transform.position).magnitude) blackboard.nodeIdx++;
                    if((new Vector2(nodes[blackboard.nodeIdx].x, nodes[blackboard.nodeIdx].y)-moveDir).x > 0.3f) blackboard.nodeIdx++;
                    // Debug.Log("Path found3");
                }
            }
            catch (Exception e) { Debug.LogException(e); }
            finally { isRunning = false;}
        }

        // public async void Backstep()
        // {
        //     //backsteppoint =>구해주기만 하면 되나?
        //     //blackboart.target = backsteppoint;
        // }
    }
}
