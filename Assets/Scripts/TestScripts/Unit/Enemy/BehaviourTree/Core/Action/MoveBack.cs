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
        private IEnumerator waitting;
        private PathFinding pathFinding;
        private JobHandle jobHandle;
        private List<Vector2Int> backList;
        private NodeComparer nodeComparer;

        private void OnDestroy() => Dispose();

        protected override void OnEnd()
        {
            // blackboard.thisUnit.Move(blackboard.thisUnit.transform.position);
        }

        protected override void OnStart() => blackboard.thisUnit.SetAni(false);

        protected override NodeState OnUpdate()
        {
            if(waitting != null) waitting.MoveNext();
            if(!isRunning) startTime += Time.deltaTime;
            if(!isRunning && blackboard.target != null && (startTime >= reloadTime || blackboard.FinalNodeList == default))
            {
                if(startTime >= reloadTime) startTime = 0;
                GetBackPathAsync();
                return NodeState.Success;
            }
            if(blackboard.FinalNodeList == default) 
            {
                return NodeState.Running;
            }
            if(blackboard.FinalNodeList.Count <= blackboard.nodeIdx) 
            {
                blackboard.nodeIdx = 0;
                return NodeState.Failure;
            }
            var tempDir = new Vector3(blackboard.FinalNodeList[blackboard.nodeIdx].x+GameManager.Instance.correctionPos.x, blackboard.FinalNodeList[blackboard.nodeIdx].y);
            moveDir = tempDir - blackboard.thisUnit.transform.position;
            moveDir = moveDir.normalized;
            blackboard.thisUnit.Move(tempDir);
            if((blackboard.thisUnit.transform.position - tempDir).magnitude < 0.1f) blackboard.nodeIdx++;
            return NodeState.Success;
        }

        private IEnumerator WaitHandle()
        {
            if(jobHandle.Equals(default(JobHandle)))
            {
                Dispose();
                yield break;
            }
            while(!jobHandle.IsCompleted) yield return null;
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

        [MesageTarget] public void GetBackPathAsync()
        {
            if(isRunning) return;
            isRunning = true;
            jobHandle = default;
            pathFinding = default;
            try
            {
                Vector2 startPos = Vector2.zero;
                if(blackboard.FinalNodeList != null && blackboard.FinalNodeList.Count > 0)
                {
                    if(blackboard.FinalNodeList.Count-1 < blackboard.nodeIdx) blackboard.nodeIdx = blackboard.FinalNodeList.Count-1;
                    if(blackboard.nodeIdx < 0) blackboard.nodeIdx = 0;
                    startPos = new Vector2(blackboard.FinalNodeList[blackboard.nodeIdx].x, blackboard.FinalNodeList[blackboard.nodeIdx].y);
                }
                else startPos = blackboard.thisUnit.transform.position;

                var targetPos = GetBackPos(startPos);
                GameManager.Instance.PathFind(startPos, targetPos, ref jobHandle, ref pathFinding);
                waitting = WaitHandle();
                waitting.MoveNext();
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
            nodeComparer = new();
            nodeComparer.startPos = new Vector2Int((int)startPos.x, (int)startPos.y);
            nodeComparer.targetPos = new Vector2Int((int)blackboard.target.position.x, (int)blackboard.target.position.y);
            backList.Add(nodeComparer.startPos);

            int min = 1;
            int max = 5;
            for(int x=(int)startPos.x-max; x<(int)startPos.x-min; x++)
            {
                if(NodeCheck(x, (int)startPos.y)) backList.Add(new Vector2Int(x, (int)startPos.y));
            }
            for(int x=(int)startPos.x+min; x<(int)startPos.x+max; x++)
            {
                if(NodeCheck(x, (int)startPos.y)) backList.Add(new Vector2Int(x, (int)startPos.y));
            }

            max = 10;
            for(int y=(int)startPos.y-max; y<(int)startPos.y+max; y++)
            {
                if(NodeCheck((int)startPos.x-max, y)) backList.Add(new Vector2Int((int)startPos.x-max, y));
            }
            for(int y=(int)startPos.y-max; y<(int)startPos.y+max; y++)
            {
                if(NodeCheck((int)startPos.x+max, y)) backList.Add(new Vector2Int((int)startPos.x+max, y));
            }

            var arr = backList.ToArray();
            Array.Sort(arr, nodeComparer);

            return arr[0];
        }
        private bool NodeCheck(int x, int y)
        {
            try
            {
                if(GameManager.Instance.NodeArray[x - GameManager.Instance.bottomLeft.x, y - GameManager.Instance.bottomLeft.y].isRoad) return true;
                else return false;
            }
            catch { return false; }
        }

        [MesageTarget] private void Dispose()
        {
            if(!jobHandle.IsCompleted) jobHandle.Complete();
            isRunning = false;
            waitting = null;
            if(pathFinding.isLoad.IsCreated) GameManager.Instance.isLoad = pathFinding.isLoad[0];
            if(pathFinding.OpenList.IsCreated) pathFinding.OpenList.Dispose();
            if(pathFinding.ClosedList.IsCreated) pathFinding.ClosedList.Dispose();
            if(pathFinding.NodeArray.IsCreated) pathFinding.NodeArray.Dispose();
            if(pathFinding.FinalNodeList.IsCreated) pathFinding.FinalNodeList.Dispose();
            if(pathFinding.isLoad.IsCreated) pathFinding.isLoad.Dispose();
        }

        private class NodeComparer : IComparer<Vector2Int>
        {
            public Vector2Int startPos;
            public Vector2Int targetPos;

            public int Compare(Vector2Int item1, Vector2Int item2)
            {
                if(Mathf.Sign(targetPos.x-item1.x) != Mathf.Sign(targetPos.x-item2.x))
                {
                    if(Mathf.Sign(targetPos.x-item1.x) != Mathf.Sign(targetPos.x-startPos.x)) return -1;
                    else return 1;
                }

                var pos1 = new Vector2Int(Mathf.Abs(item1.x-startPos.x), Mathf.Abs(item1.y-startPos.y));
                var pos2 = new Vector2Int(Mathf.Abs(item2.x-startPos.x), Mathf.Abs(item2.y-startPos.y));

                if(pos1.y - pos2.y != 0) return (int)Mathf.Sign(pos1.y - pos2.y);
                if(pos1.x - pos2.x != 0) return (int)-Mathf.Sign(pos1.x - pos2.x);

                return 0;
            }
        }
    }
}
