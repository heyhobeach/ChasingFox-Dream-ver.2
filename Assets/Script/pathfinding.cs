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

    bool PointCheck = false;

    public Transform _startPos, _targetPos;


    public void test()
    {
        Debug.Log("hello");
    }

    public void PathFinding()
    {
        // NodeArray의 크기 정해주고, isWall, x, y 대입
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        NodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;//이거 때문에 기본적으로 지나갈수있다고 생각
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



        // 시작과 끝 노드, 열린리스트와 닫힌리스트, 마지막리스트 초기화
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];//지금 이 부분 빨간색 뜨긴함 작동에는 문제는 없음
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];
        //if (!NodeArray[TargetNode.x - bottomLeft.x, TargetNode.y - bottomLeft.y].isRoad)
        //{
        //    Debug.Log("어캐 찾누 으하하하");
        //}
        int tempNum = 0;
        while(!NodeArray[TargetNode.x - bottomLeft.x, TargetNode.y - bottomLeft.y+tempNum].isRoad)
        {
            tempNum--;
            Debug.Log(string.Format("{0}칸 ",tempNum));
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

        int check = 0;

        while (OpenList.Count > 0)
        {
            
            //Debug.Log("while" + check);
            //check++;
            // 열린리스트 중 가장 F가 작고 F가 같다면 H가 작은 걸 현재노드로 하고 열린리스트에서 닫힌리스트로 옮기기
            CurNode = OpenList[0];
            string leftright = "";
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H)
                {//가중치 계산해서 넣는 실질적인 부분
                    CurNode = OpenList[i];
                    
                }

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);


            // 마지막
            if (CurNode == TargetNode)//여기 못감
            {
                Node TargetCurNode = TargetNode;
                int _cnt = 0;
                Debug.Log("끝부분");
                while (TargetCurNode != StartNode)//이게 성립하지 않으면 항상 무한 반복중 그렇다면 startnode와 엮어야함
                {
                    
                    _cnt++;
                    if (_cnt > 2000)//혹시나 무한 반복할 경우 예외 
                    {
                        Debug.Log("무한 반복");
                        return;
                    }

                    //target.parentnode.isplatform추가
                    if (TargetCurNode.isplatform||TargetCurNode.ParentNode.isplatform)
                    {
                        if (targetPos.y > startPos.y)//위로
                        {
                            if ((//Debug.Log("좌측 상단 포인트");
                                NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x - 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform
                                && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x - 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint
                                && !TargetCurNode.isPoint))
                            {//좌
                                
                                Node temp;
                                temp = TargetCurNode.ParentNode;
                                NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y].ParentNode = temp;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y];
                            }
                            if ((//NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint||//우

                                NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform
                                && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint
                                && !TargetCurNode.isPoint))
                            {//좌
                                //Debug.Log("우측 상단 포인트");
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
                                //Debug.Log("우측하단 포인트");
                                Node temp;
                                temp = TargetCurNode.ParentNode;
                                NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y - 1].ParentNode = temp;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y - 1];
                            }

                            if (NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y - 1].isplatform
       && NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y - 1].isPoint
       && !TargetCurNode.ParentNode.isPoint)

                            {
                                //Debug.Log("좌측하단 포인트");
                                Node temp;
                                temp = TargetCurNode.ParentNode;
                                NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y - 1].ParentNode = temp;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y - 1];
                            }



                        }
                        else if (targetPos.y < startPos.y)//아래 
                        {
                            //Debug.Log(string.Format("{0},{1}//{2},{3}", TargetCurNode.x, TargetCurNode.y, NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x, TargetCurNode.ParentNode.y - bottomLeft.y ].x, NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x, TargetCurNode.ParentNode.y - bottomLeft.y ].y));
                            if ((//NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint||//우

                                NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1].isplatform
                                && NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1].isPoint
                                && !TargetCurNode.ParentNode.isPoint))
                            {//좌
                               // Debug.Log("좌측 상단 포인트");
                                int sub = TargetCurNode.ParentNode.x-NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1].x ;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1];

                                for(int i = 0; i < sub; i++)
                                {
                                    NodeArray[TargetCurNode.x - bottomLeft.x - 1 + i, TargetCurNode.y - bottomLeft.y + 1].ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x + i, TargetCurNode.y - bottomLeft.y + 1];
                                }
                            }

                            if ((//NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isplatform && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x + 1, TargetCurNode.ParentNode.y - bottomLeft.y + 1].isPoint||//우

                                NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y + 1].isplatform
                                && NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y + 1].isPoint
                                && !TargetCurNode.ParentNode.isPoint))
                            {//좌
                                //Debug.Log(NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y].x + "," + NodeArray[TargetCurNode.x - bottomLeft.x - 1, TargetCurNode.y - bottomLeft.y + 1].y);
                                Debug.Log("우측 상단 포인트");
                                int sub = NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y + 1].x - TargetCurNode.ParentNode.x;
                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y + 1];
                                

                                for(int i = 0; i < sub; i++)
                                {
                                    NodeArray[TargetCurNode.x - bottomLeft.x + 1-i, TargetCurNode.y - bottomLeft.y + 1].ParentNode= NodeArray[TargetCurNode.x - bottomLeft.x -i, TargetCurNode.y - bottomLeft.y + 1];
                                }
                            }

                            if (NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x - 1, TargetCurNode.ParentNode.y - bottomLeft.y - 1].isplatform//parent접근 x 플랫폼 문제
                               && NodeArray[TargetCurNode.ParentNode.x - bottomLeft.x - 1, TargetCurNode.ParentNode.y - bottomLeft.y - 1].isPoint
                               && !TargetCurNode.isPoint)

                            {
                                //
                                //Debug.Log("좌측하단 포인트");
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
                                Debug.Log("우측 하단 포인트");

                                TargetCurNode.ParentNode = NodeArray[TargetCurNode.x - bottomLeft.x + 1, TargetCurNode.y - bottomLeft.y];                                               
                            }




                        }
                    }
                    FinalNodeList.Add(TargetCurNode);
                    //NodeDistanceList.Add(leftright);
                    TargetCurNode = TargetCurNode.ParentNode;//부모 설정부분
                }

                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();
                //NodeDistanceList.Add("none");
                //NodeDistanceList.Reverse();
                //int cnt = FinalNodeList.Count;
                //for (int i = 0; i < cnt; i++)
                //{
                //    print(i + "번째는 " + FinalNodeList[i].x + ", " + FinalNodeList[i].y + NodeDistanceList[i]); 
                //}
                //    
                //Debug.Log(cnt);
                return;
            }


            // ↗↖↙↘
            if (allowDiagonal)
            {
                OpenListAdd(CurNode.x + 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y - 1);
                OpenListAdd(CurNode.x + 1, CurNode.y - 1);
            }

            // ↑ → ↓ ←
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
        Debug.Log("끝날 때 목표 노드 위치" + (TargetNode.y));
        if (OpenList.Count == 0)
        {

            //Debug.Log(CurNode.x-bottomLeft.x);
            if (CurNode == TargetNode)
            {
                Debug.Log("체크");
            }
        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
       // Debug.Log("test");
        // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            // 대각선 허용시, 벽 사이로 통과 안됨
            if (allowDiagonal) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall && NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

            // 코너를 가로질러 가지 않을시, 이동 중에 수직수평 장애물이 있으면 안됨
            if (dontCrossCorner) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall || NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

            if (NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isRoad)//checkx와 checky값을 알아야함
            {
                Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
                int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);


                // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
                if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
                {
                    NeighborNode.G = MoveCost;
                    NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                    NeighborNode.ParentNode = CurNode;

                    OpenList.Add(NeighborNode);
                }
            }


            // 이웃노드에 넣고, 직선은 10, 대각선은 14비용

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
            Debug.Log("이동중");
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(FinalNodeList[i].x, FinalNodeList[i].y + 1), Time.deltaTime);
            if(transform.position== new Vector3(FinalNodeList[i].x, FinalNodeList[i].y + 1, transform.position.z))
            {
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
