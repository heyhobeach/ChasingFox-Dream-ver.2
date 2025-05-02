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
    public class MoveBack : ActionNode
    {
        public float reloadTime = 1;
        float startTime = 0;
        private bool isRunning;
        private Vector2 moveDir;
        private PathFinding pathFinding;
        private JobHandle jobHandle;
        private List<Vector2Int> backList;
        private NodeComparer nodeComparer;

        private void OnDestroy() => Dispose();

        protected override void OnEnd() { }

        protected override void OnStart() => blackboard.thisUnit.SetAni(false);

        protected override NodeState OnUpdate()
        {
            if(!isRunning) startTime += ServiceLocator.Get<GameManager>().ingameDeltaTime;
            if(!isRunning && blackboard.target != null && (startTime >= reloadTime || blackboard.FinalNodeList == default))
            {
                if(startTime >= reloadTime) startTime = 0;
                GetBackPathAsync();
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

        [MesageTarget] public async void GetBackPathAsync()
        {
            if(isRunning) return;
            isRunning = true;
            jobHandle = default;
            pathFinding = default;
            try
            {
                Vector2 startPos = blackboard.thisUnit.transform.position;

                var targetPos = GetBackPos(startPos);
                ServiceLocator.Get<GameManager>().PathFind(startPos, targetPos, ref jobHandle, ref pathFinding);
                await WaitHandle();
            }
            catch (Exception e) 
            { 
                Debug.LogException(e);
                
                Dispose();
            }
        }
        private Vector2 GetBackPos(Vector2 startPos)
        {
            backList = new();
            Vector2Int start = new Vector2Int((int)startPos.x, (int)startPos.y);
            nodeComparer = new(start, (int)Mathf.Sign(blackboard.target.position.x - start.x));
            backList.Add(start);

            int max = 5;
            for(int x=(int)startPos.x-max; x<(int)startPos.x+max; x++)
            {
                int abs = max - (int)Mathf.Abs(startPos.x - x);
                if(NodeCheck(x, start.y) && abs < 2) backList.Add(new Vector2Int(x, start.y));
            }

            for(int y=(int)startPos.y-max; y<(int)startPos.y+max; y++)
            {
                int add = max - (int)Mathf.Abs(startPos.y - y);
                if(NodeCheck(start.x-add, y)) backList.Add(new Vector2Int(start.x-add, y));
            }
            for(int y=(int)startPos.y-max; y<(int)startPos.y+max; y++)
            {
                int add = max - (int)Mathf.Abs(startPos.y - y);
                if(NodeCheck(start.x+add, y)) backList.Add(new Vector2Int(start.x+add, y));
            }

            var arr = backList.ToArray();
            Array.Sort(arr, nodeComparer);

            if((arr[0]-start).magnitude < 2) return arr[arr.Length-1];
            else return arr[0];
        }
        private bool NodeCheck(int x, int y)
        {
            try
            {
                if(ServiceLocator.Get<GameManager>().NodeArray[x - ServiceLocator.Get<GameManager>().bottomLeft.x, y - ServiceLocator.Get<GameManager>().bottomLeft.y].isRoad) return true;
                else return false;
            }
            catch { return false; }
        }

        [MesageTarget] private void Dispose()
        {
            if(!jobHandle.IsCompleted) jobHandle.Complete();
            isRunning = false;
            if(pathFinding.isLoad.IsCreated) ServiceLocator.Get<GameManager>().isLoad = pathFinding.isLoad[0];
            if(pathFinding.OpenList.IsCreated) pathFinding.OpenList.Dispose();
            if(pathFinding.ClosedList.IsCreated) pathFinding.ClosedList.Dispose();
            if(pathFinding.NodeArray.IsCreated) pathFinding.NodeArray.Dispose();
            if(pathFinding.FinalNodeList.IsCreated) pathFinding.FinalNodeList.Dispose();
            if(pathFinding.isLoad.IsCreated) pathFinding.isLoad.Dispose();
        }

        private class NodeComparer : IComparer<Vector2Int>
        {
            private Vector2Int startPos;
            private int sign;

            public NodeComparer(Vector2Int startPos, int sign)
            {
                this.startPos = startPos;
                this.sign = sign;
            }

            public int Compare(Vector2Int item1, Vector2Int item2)
            {
                if(Mathf.Sign(startPos.x-item1.x) != Mathf.Sign(startPos.x-item2.x))
                {
                    if(sign == Mathf.Sign(startPos.x-item1.x)) return -1;
                    else return 1;
                }

                var pos1 = new Vector2Int(Mathf.Abs(item1.x-startPos.x), Mathf.Abs(item1.y-startPos.y));
                var pos2 = new Vector2Int(Mathf.Abs(item2.x-startPos.x), Mathf.Abs(item2.y-startPos.y));

                if(pos1.x - pos2.x != 0) return (int)-Mathf.Sign(pos1.x - pos2.x);
                if(pos1.y - pos2.y != 0) return (int)Mathf.Sign(pos1.y - pos2.y);

                return 0;
            }
        }
    }
}
