using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct PathFinding : IJob
{
    [System.Serializable]
    public struct Node
    {
        public bool isWall;
        public bool isRoad;
        public bool isPoint;
        public bool isplatform;
        public Vector2Int ParentNode;

        public int x, y, G, H;

        public int F { get { return G + H; } }

        public static bool operator ==(Node node1, Node node2) => node1.x == node2.x && node1.y == node2.y;
        public static bool operator !=(Node node1, Node node2) => !(node1 == node2);

        public override bool Equals(object obj)
        {
            if (!(obj is Node))
                return false;
            Node other = (Node)obj;
            return x == other.x && y == other.y;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + x.GetHashCode();
                hash -= hash * 23 + y.GetHashCode();
                return hash;
            }
        }
    }

    public Vector2Int bottomLeft;
    public Vector2Int topRight;
    public Vector2Int startPos;
    public NativeArray<Node> NodeArray;
    [ReadOnly] public int Width, Height;

    Node StartNode, TargetNode, CurNode;
    public NativeList<Node> OpenList, ClosedList;    
    public NativeList<Node> FinalNodeList;

    [ReadOnly] public Vector3 startPosV3, targetPosV3;
    private bool isPointSearching;

    private int GetIndex(int x, int y) => y * Width + x;

    private void FindPath(Vector3 startPosV3, Vector3 targetPosV3)
    {
        Vector2Int startPos = new Vector2Int((int)startPosV3.x, (int)startPosV3.y);
        Vector2Int targetPos = new Vector2Int((int)targetPosV3.x, (int)targetPosV3.y);
        StartNode = NodeArray[GetIndex(startPos.x - bottomLeft.x, startPos.y - bottomLeft.y)];
        TargetNode = NodeArray[GetIndex(targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y)];
        int tempNum = 0;
        Node tempNode = NodeArray[GetIndex(TargetNode.x - bottomLeft.x, TargetNode.y - bottomLeft.y+tempNum)];
        while(!tempNode.isRoad)
        {
            tempNum--;
            tempNode = NodeArray[GetIndex(TargetNode.x - bottomLeft.x, TargetNode.y - bottomLeft.y+tempNum)];
        }
        TargetNode = NodeArray[GetIndex(TargetNode.x - bottomLeft.x, TargetNode.y - bottomLeft.y + tempNum)];
        if(StartNode.isplatform) isPointSearching = true;

        OpenList.Add(StartNode);

        while (OpenList.Length > 0)
        {
            // ��������Ʈ �� ���� F�� �۰� F�� ���ٸ� H�� ���� �� ������� �ϰ� ��������Ʈ���� ��������Ʈ�� �ű��
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Length; i++)
            {
                if (OpenList[i].F <= CurNode.F)
                {
                    CurNode = OpenList[i];
                }
            }

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);

            if (CurNode == TargetNode)
            {
                TargetNode.ParentNode.x = CurNode.x;
                TargetNode.ParentNode.y = CurNode.y;
                Node TargetCurNode = TargetNode;
                int count = 0;

                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    int i = GetIndex(TargetCurNode.ParentNode.x - bottomLeft.x, TargetCurNode.ParentNode.y - bottomLeft.y);
                    if(i >= 0 && i < NodeArray.Length) TargetCurNode = NodeArray[i];
                    if(count++ >= 2000)
                    {
                        Debug.LogError("Loop erorr");
                        break;
                    }
                }

                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();
                Debug.Log("Path find success");
                return;
            }


            if(isPointSearching)
            {
                OpenListAdd(CurNode.x + 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y - 1);
                OpenListAdd(CurNode.x + 1, CurNode.y - 1);
            }
            OpenListAdd(CurNode.x - 1, CurNode.y);
            // OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            // OpenListAdd(CurNode.x, CurNode.y - 1);
        }
        Debug.LogError("Path find failure erorr");
        return;
    }

    private void OpenListAdd(int checkX, int checkY)
    {
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !NodeArray[GetIndex(checkX - bottomLeft.x, checkY - bottomLeft.y)].isWall && !ClosedList.Contains(NodeArray[GetIndex(checkX - bottomLeft.x, checkY - bottomLeft.y)]))
        {
            if (NodeArray[GetIndex(checkX - bottomLeft.x, checkY - bottomLeft.y)].isRoad)
            {
                Node NeighborNode = NodeArray[GetIndex(checkX - bottomLeft.x, checkY - bottomLeft.y)];
                int MoveCost = CurNode.G + (NeighborNode.isPoint ? -35 : CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);
                if(NeighborNode.isPoint) 
                {
                    if(isPointSearching) isPointSearching = false;
                    else isPointSearching = true;
                }
                if(NeighborNode.isPoint || MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
                {
                    NeighborNode.G = MoveCost;
                    NeighborNode.H = NeighborNode.isPoint ? 0 : (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                    NeighborNode.ParentNode.x = CurNode.x;
                    NeighborNode.ParentNode.y = CurNode.y;

                    OpenList.Add(NeighborNode);
                    NodeArray[GetIndex(checkX - bottomLeft.x, checkY - bottomLeft.y)] = NeighborNode;
                }
            }
        }
    }

    public void Execute()
    {
        FindPath(startPosV3, targetPosV3);
    }
}
