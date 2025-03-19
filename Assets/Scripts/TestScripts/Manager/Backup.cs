using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public partial class Backup : MonoBehaviour
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

        public static bool operator ==(Node node1, Node node2) => node1.Equals(node2);
        public static bool operator !=(Node node1, Node node2) => !node1.Equals(node2);

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

    public bool showNodes;
    public bool showPath;

    public Vector2 correctionPos;
    public Vector2Int bottomLeft, topRight, startPos;
    // public bool allowDiagonal, dontCrossCorner;
    Node StartNode, TargetNode, CurNode;
    List<Node> OpenList, ClosedList;
    private List<Node> FinalNodeList;
    public Node[,] NodeArray;
    private bool isPointSearching;
    private bool isLoad;

    public IEnumerator MapSearchStart()
    {
        while(true)
        {
            yield return new WaitUntil(() => isLoad);
            MapSearch();
            isLoad = false;
        }
    }
    public void MapSearch()
    {
        // NodeArray�� ũ�� �����ְ�, isWall, x, y ����
        int sizeX = topRight.x - bottomLeft.x + 1;
        int sizeY = topRight.y - bottomLeft.y + 1;
        NodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)//��κ� �ѹ��� ������
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;//�̰� ������ �⺻������ ���������ִٰ� ����
                bool isRoad = false;
                bool isPoint = false;
                bool isplatform = false;
                var temp = Physics2D.OverlapBoxAll(new Vector2(i + bottomLeft.x + correctionPos.x, j + bottomLeft.y + correctionPos.y), Vector2.one*0.9f, 0);
                foreach (Collider2D col in temp)
                {
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;
                    if (col.gameObject.layer == LayerMask.NameToLayer("Ground")) isRoad = true;
                    if (col.gameObject.layer == LayerMask.NameToLayer("Point")) isPoint = true;
                    if (col.gameObject.layer == LayerMask.NameToLayer("EnemyPlatform")) 
                    {
                        isRoad = true;
                        isplatform = true;
                    }
                }
                if(!isWall) isWall = !isRoad && !isplatform;
                if (isplatform || isPoint) isRoad = true;

                NodeArray[i, j] = new Node {
                    isWall = isWall, 
                    isRoad = isRoad, 
                    isPoint = isPoint, 
                    isplatform = isplatform,
                    x = i + bottomLeft.x,
                    y = j + bottomLeft.y,
                    ParentNode = new Vector2Int(int.MinValue, int.MinValue)
                };
            }
        }
    }
    public List<Node> PathFinding(Vector3 startPosV3, Vector3 targetPosV3)
    {
        if(NodeArray == null || isLoad)
        {
            // Debug.Log("Node Array not set");
            isLoad = true;
            NodeArray = null;
            return new List<Node>();
        }
        lock (this)
        {
        Vector2Int startPos = new Vector2Int((int)startPosV3.x, (int)startPosV3.y);
        Vector2Int targetPos = new Vector2Int((int)targetPosV3.x, (int)targetPosV3.y);
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];//���� �� �κ� ������ �߱��� �۵����� ������ ����
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];
        int tempNum = 0;
        Node tempNode = NodeArray[TargetNode.x - bottomLeft.x, TargetNode.y - bottomLeft.y+tempNum];
        while(!tempNode.isRoad)
        {
            tempNum--;
            tempNode = NodeArray[TargetNode.x - bottomLeft.x, TargetNode.y - bottomLeft.y+tempNum];
        }
        // Debug.Log("Path Setting");
        TargetNode = NodeArray[TargetNode.x - bottomLeft.x, TargetNode.y - bottomLeft.y + tempNum];
        if(StartNode.isplatform) isPointSearching = true;

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();


        while (OpenList.Count > 0)
        {
            // ��������Ʈ �� ���� F�� �۰� F�� ���ٸ� H�� ���� �� ������� �ϰ� ��������Ʈ���� ��������Ʈ�� �ű��
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
            {
                if (OpenList[i].F <= CurNode.F)
                {
                    CurNode = OpenList[i];
                }
            }

            // Debug.Log("Finding");

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);

            // ������
            if (CurNode == TargetNode)//���� ����
            {
                TargetNode.ParentNode.x = CurNode.x;
                TargetNode.ParentNode.y = CurNode.y;
                Node TargetCurNode = TargetNode;
                int count = 0;
                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x, TargetCurNode.ParentNode.y - bottomLeft.y];//�θ� �����κ�
                    if(count++ >= 2000)
                    {
                        Debug.LogError("Loop Erorr");
                        break;
                    }
                }

                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                return FinalNodeList;
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
        
        Debug.LogError("Path lost Erorr");
        return null;
        }
    }

    private void OpenListAdd(int checkX, int checkY)
    {
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            if (NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isRoad)
            {
                Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
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
                    NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y] = NeighborNode;
                }
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if(showNodes)
        {
            if(NodeArray == null) return;
            foreach(var node in NodeArray)
            {
                Gizmos.color = Color.green;
                if(node.isRoad) Gizmos.DrawWireCube(new Vector3(node.x+correctionPos.x, node.y+correctionPos.y), Vector3.one*0.95f);
                Gizmos.color = Color.blue;
                if(node.isplatform) Gizmos.DrawWireCube(new Vector3(node.x+correctionPos.x, node.y+correctionPos.y), Vector3.one*0.95f);
                Gizmos.color = Color.red;
                if(node.isWall) Gizmos.DrawWireCube(new Vector3(node.x+correctionPos.x, node.y+correctionPos.y), Vector3.one*0.95f);
                Gizmos.color = Color.blue;
                if(node.isPoint) Gizmos.DrawWireCube(new Vector3(node.x+correctionPos.x, node.y+correctionPos.y), Vector3.one*0.85f);

                Handles.Label(new Vector3(node.x+correctionPos.x, node.y+correctionPos.y), "G : " + node.G +  "\nH : " + node.H +  "\nF : " + node.F);
            }
        }
        if(showPath)
        {
            Gizmos.color = Color.white;
            Node prv = default;
            if(FinalNodeList == null) return;
            foreach(var node in FinalNodeList)
            {
                if(prv != default) Gizmos.DrawLine(new Vector3(prv.x+correctionPos.x, prv.y+correctionPos.y), new Vector3(node.x+correctionPos.x, node.y+correctionPos.y));
                prv = node;
            }
        }
    }
#endif
}


