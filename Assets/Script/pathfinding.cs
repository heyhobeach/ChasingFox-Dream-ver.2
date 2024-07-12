using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Node
{
    public bool isWall;
    public bool isRoad;
    public bool isPoint;
    public bool isplatform;
    public Node ParentNode;

    public int x, y, G, H;
    //public int x, y, G, H;

    public int F { get { return G + H; } }
    public Node(bool _isWall, bool _isRoad, bool _isPoint, bool _isPlatform, int _x, int _y) { 
        isWall = _isWall; 
        isRoad = _isRoad; 
        isPoint = _isPoint; 
        isplatform = _isPlatform;
        if (isplatform)
        {
            isRoad = true;
        }
        x = _x; 
        y = _y; 
    }
}
public partial class Dumy : MonoBehaviour
{
    public Rigidbody2D rg2d;

    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    public List<Node> FinalNodeList;
    public bool allowDiagonal, dontCrossCorner;

    int sizeX, sizeY;
    Node[,] NodeArray;
    Node StartNode, TargetNode, CurNode;
    List<Node> OpenList, ClosedList;
    List<string> NodeDistanceList;

    // bool PointCheck = false;

    public Transform _startPos, _targetPos;


    public void test()
    {
        Debug.Log("hello");
    }

    public void PathFinding()
    {
        // NodeArray�� ũ�� �����ְ�, isWall, x, y ����
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        NodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)//��κ� �ѹ��� ������
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;//�̰� ������ �⺻������ ���������ִٰ� ����
                bool isRoad = false;
                bool isPoint = false;
                bool isplatform = false;
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x, j + bottomLeft.y), 0.4f))
                {
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;
                    if (col.gameObject.layer == LayerMask.NameToLayer("Ground")) isRoad = true;
                    if (col.gameObject.layer == LayerMask.NameToLayer("Point")) isPoint = true;
                    if (col.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform")) isplatform = true;
                }

                NodeArray[i, j] = new Node(isWall, isRoad, isPoint, isplatform, i + bottomLeft.x, j + bottomLeft.y);
            }
        }


        //Debug.Log(string.Format("{0},{1}", startPos.x - bottomLeft.x, startPos.y - bottomLeft.y));//-bottomleft.x,bottomleft.y�� ����� �ѹ��� �̷����� �� �̷����°� ����
        //Debug.Log(string.Format("{0},{1}", targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y));//���⵵ ��ģ����
        // ���۰� �� ���, ��������Ʈ�� ��������Ʈ, ����������Ʈ �ʱ�ȭ
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];//���� �� �κ� ������ �߱��� �۵����� ������ ����
        
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];
        //if (!NodeArray[TargetNode.x - bottomLeft.x, TargetNode.y - bottomLeft.y].isRoad)
        //{
        //    Debug.Log("��ĳ ã�� ��������");
        //}
        int tempNum = 0;
        while(!NodeArray[TargetNode.x - bottomLeft.x, TargetNode.y - bottomLeft.y+tempNum].isRoad)
        {
            tempNum--;
            // Debug.Log(string.Format("{0}ĭ ",tempNum));
        }
        TargetNode = NodeArray[TargetNode.x - bottomLeft.x, TargetNode.y - bottomLeft.y + tempNum];

        //Debug.Log((int)_startPos.position.x + ":" + (int)_startPos.position.y);
        //Debug.Log((int)_targetPos.position.x + ":" + (int)_targetPos.position.y);
        //StartNode = NodeArray[(int)_startPos.position.x,(int)_startPos.position.y];
        //TargetNode = NodeArray[(int)_targetPos.position.x, (int)_targetPos.position.y];
        //StartNode = NodeArray[-7,-4];
        //TargetNode = NodeArray[3, -4];

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();
        NodeDistanceList = new List<string>();

        // int check = 0;

        while (OpenList.Count > 0)
        {
            
            //Debug.Log("while" + check);
            //check++;
            // ��������Ʈ �� ���� F�� �۰� F�� ���ٸ� H�� ���� �� ������� �ϰ� ��������Ʈ���� ��������Ʈ�� �ű��
            CurNode = OpenList[0];
            // string leftright = "";
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H)
                {//����ġ ����ؼ� �ִ� �������� �κ�
                    CurNode = OpenList[i];
                    
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
                        // Debug.Log("���� �ݺ�");
                        return;
                    }

                    //target.parentnode.isplatform�߰�
                    if (TargetCurNode.isplatform||TargetCurNode.ParentNode.isplatform)
                    {
                        if (targetPos.y > startPos.y)//����
                        {
                            if ((//Debug.Log("���� ��� ����Ʈ");
                                NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x - 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform
                                && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x - 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint
                                && !TargetCurNode.isPoint))
                            {//��
                                
                                Node temp;
                                temp = TargetCurNode.ParentNode;
                                NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y].ParentNode = temp;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y];
                            }
                            if ((//NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint||//��

                                NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform
                                && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint
                                && !TargetCurNode.isPoint))
                            {//��
                                //Debug.Log("���� ��� ����Ʈ");
                                Node temp;
                                temp = TargetCurNode.ParentNode;
                                NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y].ParentNode = temp;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y];
                            }

                            if (NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y - 1].isplatform
                               && NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y - 1].isPoint
                               && !TargetCurNode.ParentNode.isPoint)

                            {
                                //
                                //Debug.Log("�����ϴ� ����Ʈ");
                                Node temp;
                                temp = TargetCurNode.ParentNode;
                                NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y - 1].ParentNode = temp;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y - 1];
                            }

                            if (NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y - 1].isplatform
       && NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y - 1].isPoint
       && !TargetCurNode.ParentNode.isPoint)

                            {
                                //Debug.Log("�����ϴ� ����Ʈ");
                                Node temp;
                                temp = TargetCurNode.ParentNode;
                                NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y - 1].ParentNode = temp;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y - 1];
                            }



                        }
                        else if (targetPos.y < startPos.y)//�Ʒ� 
                        {
                            //Debug.Log(string.Format("{0},{1}//{2},{3}", TargetCurNode.x, TargetCurNode.y, NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x, TargetCurNode.ParentNode.y - bottomLeft.y ].x, NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x, TargetCurNode.ParentNode.y - bottomLeft.y ].y));
                            if ((//NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint||//��

                                NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1].isplatform
                                && NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1].isPoint
                                && !TargetCurNode.ParentNode.isPoint))
                            {//��
                               // Debug.Log("���� ��� ����Ʈ");
                                int sub = TargetCurNode.ParentNode.x-NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1].x ;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1];

                                for(int i = 0; i < sub; i++)
                                {
                                    NodeArray[TargetCurNode.x - bottomLeft.x - 1 + i, TargetCurNode.y - bottomLeft.y + 1].ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x + i, TargetCurNode.y - bottomLeft.y + 1];
                                }
                            }

                            if ((//NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint||//��

                                NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y + 1].isplatform
                                && NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y + 1].isPoint
                                && !TargetCurNode.ParentNode.isPoint))
                            {//��
                                //Debug.Log(NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y].x + "," + NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1].y);
                                // Debug.Log("���� ��� ����Ʈ");
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
                                //
                                //Debug.Log("�����ϴ� ����Ʈ");
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y];
                                //NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x-1, TargetCurNode.ParentNode.y - bottomLeft.y-1 ].ParentNode = NodeArray;
                                Debug.Log(FinalNodeList.Contains(NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y - 1]));

                                //TargetCurNode.ParentNode = NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x - 1, TargetCurNode.ParentNode.y - bottomLeft.y - 1];
                                //for(int i=1;FinalNodeList.Contains(NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + i, TargetCurNode.ParentNode.y - bottomLeft.y - 1]);i++)
                                //{
                                //    nodear
                                //}
                                //TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x -1, TargetCurNode.y - bottomLeft.y-1];
                            }

                            if (NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y - 1].isplatform
       && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y - 1].isPoint
        && !TargetCurNode.isPoint)

                            {
                                // Debug.Log("���� �ϴ� ����Ʈ");

                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y];                                               
                            }




                        }
                    }
                    FinalNodeList.Add(TargetCurNode);
                    //NodeDistanceList.Add(leftright);
                    TargetCurNode = TargetCurNode.ParentNode;//�θ� �����κ�
                }

                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();
                //NodeDistanceList.Add("none");
                //NodeDistanceList.Reverse();
                //int cnt = FinalNodeList.Count;
                //for (int i = 0; i < cnt; i++)
                //{
                //    print(i + "��°�� " + FinalNodeList[i].x + ", " + FinalNodeList[i].y + NodeDistanceList[i]); 
                //}
                //    
                //Debug.Log(cnt);
                return;
            }


            // �֢آע�
            if (allowDiagonal)
            {
                OpenListAdd(CurNode.x + 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y - 1);
                OpenListAdd(CurNode.x + 1, CurNode.y - 1);
            }

            // �� �� �� ��
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
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
    }

    void OpenListAdd(int checkX, int checkY)
    {
       // Debug.Log("test");
        // �����¿� ������ ����� �ʰ�, ���� �ƴϸ鼭, ��������Ʈ�� ���ٸ�
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            // �밢�� ����, �� ���̷� ��� �ȵ�
            if (allowDiagonal) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall && NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

            // �ڳʸ� �������� ���� ������, �̵� �߿� �������� ��ֹ��� ������ �ȵ�
            if (dontCrossCorner) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall || NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

            if (NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isRoad)//checkx�� checky���� �˾ƾ���
            {
                Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
                int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);


                // �̵������ �̿����G���� �۰ų� �Ǵ� ��������Ʈ�� �̿���尡 ���ٸ� G, H, ParentNode�� ���� �� ��������Ʈ�� �߰�
                if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
                {
                    NeighborNode.G = MoveCost;
                    NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                    NeighborNode.ParentNode = CurNode;

                    OpenList.Add(NeighborNode);
                }
            }


            // �̿���忡 �ְ�, ������ 10, �밢���� 14���

        }
    }

    void OnDrawGizmos()
    {
        if (FinalNodeList.Count != 0) for (int i = 0; i < FinalNodeList.Count - 1; i++)
                Gizmos.DrawLine(new Vector2(FinalNodeList[i].x, FinalNodeList[i].y), new Vector2(FinalNodeList[i + 1].x, FinalNodeList[i + 1].y));
    }

    IEnumerator cMove()
    {
        for (int i = 1; i < FinalNodeList.Count; )
        {
            // Debug.Log("�̵���");
            if (attacking)
            {
                break;
            }
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(FinalNodeList[i].x, FinalNodeList[i].y + 1), Time.deltaTime);
            // Debug.Log("�̵�����");
            if(transform.position== new Vector3(FinalNodeList[i].x, FinalNodeList[i].y + 1, transform.position.z))
            {
                // Debug.Log("����");
                i++;
            }
            //transform.position = new Vector2(FinalNodeList[i].x, FinalNodeList[i].y + 1);
            //transform.position = Vector3.Lerp(transform.position, new Vector2(FinalNodeList[i].x, FinalNodeList[i].y + 1), Time.deltaTime);
            //yield return null;
            yield return null;
        }
    }


    //private void Update()
    //{
    //    startPos.x = (int)_startPos.position.x;
    //    startPos.y = (int)_startPos.position.y;
    //    targetPos.x = (int)_targetPos.position.x;
    //    targetPos.y = (int)_targetPos.position.y;
    //    if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        FinalNodeList.Clear();
    //
    //        PathFinding();
    //        //Debug.Log(NodeArray[8 - bottomLeft.x, -2 - bottomLeft.y].ParentNode.x+"," +NodeArray[8 - bottomLeft.x, -2 - bottomLeft.y].ParentNode.y);    
    //    }
    //    //if (Input.GetKeyDown(KeyCode.A))
    //    //{
    //    //    StartCoroutine(cMove());
    //    //}
    //}
}
