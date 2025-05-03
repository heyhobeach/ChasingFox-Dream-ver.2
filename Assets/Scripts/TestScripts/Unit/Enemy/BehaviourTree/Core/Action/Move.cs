using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

namespace BehaviourTree
{
    public class Move : ActionNode
    {
        public float reloadTime = 1;
        float startTime = 0;
        private bool isRunning;
        private Vector2 moveDir;
        private PathFinding pathFinding;
        private JobHandle jobHandle;

        private void OnDestroy() => Dispose();

        protected override void OnEnd()
        {
            if(state == NodeState.Failure) blackboard.thisUnit.SetAni(false);
        }

        protected override void OnStart() => blackboard.thisUnit.SetAni(false);

        protected override NodeState OnUpdate()
        {
            if(!isRunning) startTime += ServiceLocator.Get<GameManager>().ingameDeltaTime;
            if(!isRunning && blackboard.target != null && (startTime >= reloadTime || blackboard.FinalNodeList == default))
            {
                if(startTime >= reloadTime) startTime = 0;
                GetPathAsync();
                return NodeState.Running;
            }
            if(blackboard.FinalNodeList == default) 
            {
                return NodeState.Running;
            }
            if(blackboard.FinalNodeList.Count <= blackboard.nodeIdx) 
            {
                blackboard.nodeIdx = blackboard.FinalNodeList.Count - 1;
                return NodeState.Success;
            }
            var tempDir = new Vector3(blackboard.FinalNodeList[blackboard.nodeIdx].x+ServiceLocator.Get<GameManager>().correctionPos.x, blackboard.FinalNodeList[blackboard.nodeIdx].y);
            moveDir = tempDir - blackboard.thisUnit.transform.position;
            moveDir = moveDir.normalized;
            if(!blackboard.thisUnit.Move(tempDir)) return NodeState.Failure;
            if((blackboard.thisUnit.transform.position - tempDir).magnitude < 0.1f) blackboard.nodeIdx++;
            return NodeState.Running;
        }

        private async Awaitable WaitHandle()
        {
            if(jobHandle.Equals(default(JobHandle)))
            {
                Dispose();
                return;
            }
            while(!jobHandle.IsCompleted) await Awaitable.NextFrameAsync();
            jobHandle.Complete();
            var output = pathFinding.FinalNodeList;
            try 
            { 
                List<PathFinding.Node> nodes = output.ToList(); 
                if(nodes != null && nodes.Count > 1)
                {
                    blackboard.FinalNodeList = nodes;
                    blackboard.nodeIdx = 0;
                    while(blackboard.nodeIdx+1 < nodes.Count && (new Vector3(nodes[blackboard.nodeIdx].x, nodes[blackboard.nodeIdx].y)-blackboard.thisUnit.transform.position).magnitude > (new Vector3(nodes[blackboard.nodeIdx+1].x, nodes[blackboard.nodeIdx+1].y)-blackboard.thisUnit.transform.position).magnitude) blackboard.nodeIdx++;
                    if((new Vector2(nodes[blackboard.nodeIdx].x, nodes[blackboard.nodeIdx].y)-moveDir).x > 0.3f) blackboard.nodeIdx++;
                }
            }
            catch(Exception e) { Debug.LogError(e); }
            finally { Dispose(); }
        }

        [MesageTarget] public async void GetPathAsync()
        {
            if(isRunning) return;
            isRunning = true;
            jobHandle = default;
            pathFinding = default;
            try
            {
                Vector2 startPos = blackboard.thisUnit.transform.position;
                var targetPos = blackboard.target.position;
                ServiceLocator.Get<GameManager>().PathFind(startPos, targetPos, ref jobHandle, ref pathFinding);
                await WaitHandle();
            }
            catch (Exception e) 
            { 
                Debug.LogException(e);
                
                Dispose();
            }
        }

        [MesageTarget] private void Dispose()
        {
            if(!jobHandle.IsCompleted) jobHandle.Complete();
            isRunning = false;
            if(pathFinding.OpenList.IsCreated) pathFinding.OpenList.Dispose();
            if(pathFinding.ClosedList.IsCreated) pathFinding.ClosedList.Dispose();
            if(pathFinding.NodeArray.IsCreated) pathFinding.NodeArray.Dispose();
            if(pathFinding.FinalNodeList.IsCreated) pathFinding.FinalNodeList.Dispose();
            if(pathFinding.isLoad.IsCreated && ServiceLocator.Get<GameManager>() != null) ServiceLocator.Get<GameManager>().isLoad = pathFinding.isLoad[0];
            if(pathFinding.isLoad.IsCreated) pathFinding.isLoad.Dispose();
        }
    }
}
