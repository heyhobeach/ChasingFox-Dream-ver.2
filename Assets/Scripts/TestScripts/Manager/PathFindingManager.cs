using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;


#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    public bool showNodes;
    public bool showPath;

    public Vector2 correctionPos;
    public Vector2Int bottomLeft, topRight, startPos;
    // public bool allowDiagonal, dontCrossCorner;
    public PathFinding.Node[,] NodeArray;
    public bool isLoad;

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
        NodeArray = new PathFinding.Node[sizeX, sizeY];

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

                NodeArray[i, j] = new PathFinding.Node {
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

    public void PathFind(Vector3 startPos, Vector3 targetPos, ref JobHandle jobHandle, ref PathFinding pathFinding)
    {
        if(NodeArray == default || isLoad)
        {
            // Debug.Log("Node Array not set");
            isLoad = true;
            NodeArray = default;
            return;
        }

        var tmeps = new Vector2Int((int)(startPos.x - bottomLeft.x), (int)(startPos.y - bottomLeft.y));
        var tmept = new Vector2Int((int)(targetPos.x - bottomLeft.x), (int)(targetPos.y - bottomLeft.y));
        if (tmeps.x >= 0 && tmeps.x < NodeArray.GetLength(0) &&
            tmeps.y >= 0 && tmeps.y < NodeArray.GetLength(1) &&
            tmept.x >= 0 && tmept.x < NodeArray.GetLength(0) &&
            tmept.y >= 0 && tmept.y < NodeArray.GetLength(1))
        {
            pathFinding = new() { 
                startPosV3 = startPos, 
                targetPosV3 = targetPos, 
                bottomLeft = bottomLeft,
                topRight = topRight,
                NodeArray = ToNativeArray(NodeArray, Allocator.TempJob),
                OpenList = new NativeList<PathFinding.Node>(Allocator.TempJob),
                ClosedList = new NativeList<PathFinding.Node>(Allocator.TempJob),
                FinalNodeList = new NativeList<PathFinding.Node>(Allocator.TempJob),
                isLoad = new NativeArray<bool>(1, Allocator.TempJob),
                Height = NodeArray.GetLength(1), 
                Width = NodeArray.GetLength(0) 
            };
            jobHandle = pathFinding.Schedule();
        }

        return;
    }

    public static NativeArray<PathFinding.Node> ToNativeArray(PathFinding.Node[,] nodeArray, Allocator allocator)
    {
        if (nodeArray == null)
            throw new ArgumentNullException(nameof(nodeArray));

        int width = nodeArray.GetLength(0);
        int height = nodeArray.GetLength(1);
        int length = width * height;
        NativeArray<PathFinding.Node> nativeArray = new NativeArray<PathFinding.Node>(length, allocator);

        // 행 우선 순서: x 값이 빠르게 변화하고, 이후 y 값이 증가
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = x + y * width;
                nativeArray[index] = nodeArray[x, y];
            }
        }

        return nativeArray;
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
        // if(showPath)
        // {
        //     Gizmos.color = Color.white;
        //     Node prv = null;
        //     if(FinalNodeList == null) return;
        //     foreach(var node in FinalNodeList)
        //     {
        //         if(prv != null) Gizmos.DrawLine(new Vector3(prv.x+correctionPos.x, prv.y+correctionPos.y), new Vector3(node.x+correctionPos.x, node.y+correctionPos.y));
        //         prv = node;
        //     }
        // }
    }
#endif
}


