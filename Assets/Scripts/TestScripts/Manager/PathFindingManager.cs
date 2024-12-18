using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Node
    {
        public bool isWall;
        public bool isRoad;
        public bool isPoint;
        public bool isplatform;
        public Node ParentNode;

        public int x, y, G, H;

        public int F { get { return G + H; } }
        public Node(bool _isWall, bool _isRoad, bool _isPoint, bool _isPlatform, int _x, int _y) { 
            isWall = _isWall; 
            isRoad = _isRoad; 
            isPoint = _isPoint; 
            isplatform = _isPlatform;
            if (isplatform || isPoint)
            {
                isRoad = true;
            }
            x = _x; 
            y = _y; 
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
    List<string> NodeDistanceList;
    public Node[,] NodeArray;
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
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x + correctionPos.x, j + bottomLeft.y + correctionPos.y), 0.4f))
                {
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;
                    if (col.gameObject.layer == LayerMask.NameToLayer("Ground")) isRoad = true;
                    if (col.gameObject.layer == LayerMask.NameToLayer("Point")) isPoint = true;
                    if (col.gameObject.layer == LayerMask.NameToLayer("EnemyPlatform")) isplatform = true;
                }
                if(!isWall) isWall = !isRoad && !isplatform;

                NodeArray[i, j] = new Node(isWall, isRoad, isPoint, isplatform, i + bottomLeft.x, j + bottomLeft.y);
            }
        }
    }
    public List<Node> PathFinding(Vector3 startPosV3, Vector3 targetPosV3)
    {
        if(NodeArray == null || isLoad)
        {
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
        while(!NodeArray[TargetNode.x - bottomLeft.x, TargetNode.y - bottomLeft.y+tempNum].isRoad)
        {
            tempNum--;
        }
        TargetNode = NodeArray[TargetNode.x - bottomLeft.x, TargetNode.y - bottomLeft.y + tempNum];

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();
        NodeDistanceList = new List<string>();

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

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);

            // ������
            if (CurNode == TargetNode)//���� ����
            {
                Node TargetCurNode = TargetNode;
                int _cnt = 0;
                // Debug.Log("���κ�");
                while (TargetCurNode != StartNode)//�̰� �������� ������ �׻� ���� �ݺ��� �׷��ٸ� startnode�� �������
                {
                    _cnt++;
                    if (_cnt > 2000)//Ȥ�ó� ���� �ݺ��� ��� ���� 
                    {
                        Debug.LogError("Path Cant Find Erorr");
                        return null;
                    }

                    if (TargetCurNode.isplatform||TargetCurNode.ParentNode.isplatform)
                    {
                        if (targetPos.y > startPos.y)//����
                        {
                            if ((
                                NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x - 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform
                                && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x - 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint
                                && !TargetCurNode.isPoint))
                            {//��
                                Node temp;
                                temp = TargetCurNode.ParentNode;
                                NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y].ParentNode = temp;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y];
                            }
                            if ((
                                NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform
                                && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint
                                && !TargetCurNode.isPoint))
                            {//��
                                Node temp;
                                temp = TargetCurNode.ParentNode;
                                NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y].ParentNode = temp;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y];
                            }
                            if (NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y - 1].isplatform
                               && NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y - 1].isPoint
                               && !TargetCurNode.ParentNode.isPoint)
                            {
                                Node temp;
                                temp = TargetCurNode.ParentNode;
                                NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y - 1].ParentNode = temp;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y - 1];
                            }
                            if (NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y - 1].isplatform
       && NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y - 1].isPoint
       && !TargetCurNode.ParentNode.isPoint)
                            {
                                Node temp;
                                temp = TargetCurNode.ParentNode;
                                NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y - 1].ParentNode = temp;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y - 1];
                            }
                        }
                        else if (targetPos.y < startPos.y)//�Ʒ� 
                        {
                            if ((
                                NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1].isplatform
                                && NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1].isPoint
                                && !TargetCurNode.ParentNode.isPoint))
                            {
                                int sub = TargetCurNode.ParentNode.x-NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1].x ;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1];

                                for(int i = 0; i < sub; i++)
                                {
                                    NodeArray[TargetCurNode.x - bottomLeft.x - 1 + i, TargetCurNode.y - bottomLeft.y + 1].ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x + i, TargetCurNode.y - bottomLeft.y + 1];
                                }
                            }
                            if ((
                                NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y + 1].isplatform
                                && NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y + 1].isPoint
                                && !TargetCurNode.ParentNode.isPoint))
                            {
                                int sub = NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y + 1].x - TargetCurNode.ParentNode.x;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y + 1];
                                
                                for(int i = 0; i < sub; i++)
                                {
                                    NodeArray[TargetCurNode.x - bottomLeft.x + 1-i, TargetCurNode.y - bottomLeft.y + 1].ParentNode= NodeArray[TargetCurNode.x - bottomLeft.x -i, TargetCurNode.y - bottomLeft.y + 1];
                                }
                            }
                            if (NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x - 1, TargetCurNode.ParentNode.y - bottomLeft.y - 1].isplatform//parent���� x �÷��� ����
                               && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x - 1, TargetCurNode.ParentNode.y - bottomLeft.y - 1].isPoint
                               && !TargetCurNode.isPoint)
                            {
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y];
                            }
                            if (NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y - 1].isplatform
       && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y - 1].isPoint
        && !TargetCurNode.isPoint)
                            {
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y];                                               
                            }
                        }
                    }
                    
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;//�θ� �����κ�
                }

                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();
                return FinalNodeList;
            }


            // �֢آע�
            // if (allowDiagonal)
            // {
            // }
            OpenListAdd(CurNode.x + 1, CurNode.y + 1);
            OpenListAdd(CurNode.x - 1, CurNode.y + 1);
            OpenListAdd(CurNode.x - 1, CurNode.y - 1);
            OpenListAdd(CurNode.x + 1, CurNode.y - 1);

            // �� �� �� ��
            OpenListAdd(CurNode.x - 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
        }
        // Debug.Log("���� �� ��ǥ ��� ��ġ" + (TargetNode.y));
        if (OpenList.Count == 0)
        {

            //Debug.Log(CurNode.x-bottomLeft.x);
            if (CurNode == TargetNode)
            {
                Debug.Log("üũ");
            }
        }
        Debug.LogError("Path lost Erorr");
        return null;
        }
    }

    private void OpenListAdd(int checkX, int checkY)
    {
        // �����¿� ������ ����� �ʰ�, ���� �ƴϸ鼭, ��������Ʈ�� ���ٸ�
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            // �밢�� ����, �� ���̷� ��� �ȵ�
            // if (allowDiagonal) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall && NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

            // �ڳʸ� �������� ���� ������, �̵� �߿� �������� ��ֹ��� ������ �ȵ�
            // if (dontCrossCorner) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall || NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

            if (NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isRoad)//checkx�� checky���� �˾ƾ���
            {
                Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
                int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);
                if(NeighborNode.isPoint) MoveCost = CurNode.G - 28;

                // �̵������ �̿����G���� �۰ų� �Ǵ� ��������Ʈ�� �̿���尡 ���ٸ� G, H, ParentNode�� ���� �� ��������Ʈ�� �߰�
                if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
                {
                    NeighborNode.G = MoveCost;
                    NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                    NeighborNode.ParentNode = CurNode;

                    OpenList.Add(NeighborNode);
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
            Node prv = null;
            if(FinalNodeList == null) return;
            foreach(var node in FinalNodeList)
            {
                if(prv != null) Gizmos.DrawLine(new Vector3(prv.x+correctionPos.x, prv.y+correctionPos.y), new Vector3(node.x+correctionPos.x, node.y+correctionPos.y));
                prv = node;
            }
        }
    }
#endif
}


