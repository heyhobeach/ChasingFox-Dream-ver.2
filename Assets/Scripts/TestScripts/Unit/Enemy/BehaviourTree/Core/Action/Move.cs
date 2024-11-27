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

        protected override void OnEnd() { }

        protected override void OnStart() { }

        protected override NodeState OnUpdate()
        {
            if(blackboard.target != null && (blackboard.target.transform.position-blackboard.thisUnit.transform.position).magnitude < 1f) return NodeState.Failure;
            if(!isRunning) startTime += Time.deltaTime;
            if(!isRunning && blackboard.target != null && (startTime >= reloadTime || blackboard.FinalNodeList == null))
            {
                if(startTime >= reloadTime) startTime = 0;
                GetPathAsync();
                return NodeState.Running;
            }
            if(blackboard.FinalNodeList == null || blackboard.FinalNodeList.Count <= blackboard.nodeIdx) 
            {
                if(blackboard.FinalNodeList != null && blackboard.FinalNodeList.Count > 2)
                {
                    while(blackboard.FinalNodeList != null && blackboard.FinalNodeList.Count <= blackboard.nodeIdx) blackboard.nodeIdx--;
                    // if(blackboard.nodeIdx > 0) blackboard.nodeIdx--;
                }
                return NodeState.Failure;
            }
            moveDir = new Vector3(blackboard.FinalNodeList[blackboard.nodeIdx].x+GameManager.Instance.correctionPos.x, blackboard.FinalNodeList[blackboard.nodeIdx].y+(blackboard.thisUnit.BoxSizeY*2)+1f) - blackboard.thisUnit.transform.position;
            moveDir = moveDir.normalized;
            blackboard.thisUnit.Move(new Vector2(blackboard.FinalNodeList[blackboard.nodeIdx].x+GameManager.Instance.correctionPos.x, blackboard.FinalNodeList[blackboard.nodeIdx].y+(blackboard.thisUnit.BoxSizeY*2)+1f));
            if((blackboard.thisUnit.transform.position - new Vector3(blackboard.FinalNodeList[blackboard.nodeIdx].x+GameManager.Instance.correctionPos.x, blackboard.FinalNodeList[blackboard.nodeIdx].y+(blackboard.thisUnit.BoxSizeY*2)+1f, blackboard.thisUnit.transform.position.z)).magnitude < 0.1f) blackboard.nodeIdx++;
            return NodeState.Success;
        }

        [MesageTarget] public async void GetPathAsync()
        {
            if(isRunning) return;
            isRunning = true;
            try
            {
                Vector2 startPos = Vector2.zero;
                if(blackboard.FinalNodeList != null && blackboard.FinalNodeList.Count > blackboard.nodeIdx-1)
                {
                    blackboard.nodeIdx--;
                    if(blackboard.nodeIdx < 0) blackboard.nodeIdx = 0;
                    var curNode = blackboard.FinalNodeList[blackboard.nodeIdx];
                    startPos = new Vector2(curNode.x, curNode.y);
                }
                else startPos = blackboard.thisUnit.transform.position - (Vector3.down * (blackboard.thisUnit.BoxSizeY + 0.1f));
                if(blackboard.FinalNodeList != null)
                {
                    if(blackboard.FinalNodeList.Count <= blackboard.nodeIdx) blackboard.nodeIdx = 0;
                    startPos = new Vector2(blackboard.FinalNodeList[blackboard.nodeIdx].x, blackboard.FinalNodeList[blackboard.nodeIdx].y);
                }
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
                    if((new Vector2(nodes[blackboard.nodeIdx].x, nodes[blackboard.nodeIdx].y)-moveDir).x > 0.3f) blackboard.nodeIdx++;
                }
            }
            catch (Exception e) { Debug.LogException(e); }
            finally { isRunning = false;}
        }

        public async void Backstep()
        {
            //backsteppoint =>구해주기만 하면 되나?
            //blackboart.target = backsteppoint;
        }
    }
}
