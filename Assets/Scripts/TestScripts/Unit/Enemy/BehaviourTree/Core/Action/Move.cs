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
        int idx = 0;
        private bool isRunning;

        protected override void OnEnd() { }

        protected override void OnStart()
        {
            if(!isRunning && blackboard.target != null && (startTime >= reloadTime || blackboard.FinalNodeList == null)) GetPathAsync();
        }

        protected override NodeState OnUpdate()
        {
            if(!isRunning) startTime += Time.deltaTime;
            if(!isRunning && blackboard.target != null && (startTime >= reloadTime || blackboard.FinalNodeList == null))
            {
                if(startTime >= reloadTime) startTime = 0;
                GetPathAsync();
                return NodeState.Running;
            }
            if(blackboard.FinalNodeList == null || blackboard.FinalNodeList.Count <= idx) 
            {
                return NodeState.Failure;
            }
            var b = blackboard.thisUnit.Move(new Vector2(blackboard.FinalNodeList[idx].x, blackboard.FinalNodeList[idx].y + 1));
            if(blackboard.thisUnit.transform.position== new Vector3(blackboard.FinalNodeList[idx].x, blackboard.FinalNodeList[idx].y + 1, blackboard.thisUnit.transform.position.z)) idx++;
            switch(b)
            {
                case true: return NodeState.Running;
                case false: return NodeState.Failure;
            }
        }

        private async void GetPathAsync()
        {
            if(isRunning) return;
            isRunning = true;
            try
            {
                var startPos = blackboard.thisUnit.transform.position + (Vector3.down * 0.5f);
                var targetPos = blackboard.target.position;
                List<GameManager.Node> nodes = null;
                await Task.Run(() => {
                    nodes = GameManager.Instance.PathFinding(startPos, targetPos);
                    if(nodes != null) blackboard.FinalNodeList = nodes;
                });
                if(nodes != null)
                {
                    blackboard.FinalNodeList = nodes;
                    idx = 0;
                    while((new Vector3(nodes[idx].x, nodes[idx].y)-blackboard.thisUnit.transform.position).magnitude > (new Vector3(nodes[idx+1].x, nodes[idx+1].y)-blackboard.thisUnit.transform.position).magnitude) idx++;
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
