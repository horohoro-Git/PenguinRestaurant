using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using static Utility;
//한글



public struct NodeStruct
{
    public int r, c;
    public int H, G, F;
    public bool blocked;
    public NodeStruct(int X, int Y)
    {
        r = Y;
        c = X;
        H = 0;
        G = 0;
        F = 0;
        blocked = false;
    }

}

public struct CalculatorScale
{
    public int sizeX;
    public int sizeY;
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
    public float distanceSize;
    public float height;

}

public class MoveCalculator
{

    int[] moveX = { 1, -1, 0, 0, 1, 1, -1, -1 };
    int[] moveY = { 0, 0, 1, -1, 1, -1, 1, -1 };
   // static CalculatorScale calculatorScale = new CalculatorScale();
    public Node result;
    Node[,] nodes = new Node[200, 200];
   // NodeStruct[,] nodeStructs = new NodeStruct[200, 200];
    //List<Coord> defaults = new List<Coord>(1000);
    GameInstance gameInstance;
    static bool[,] blockedAreas = new bool[200, 200];
    NativeArray<NodeStruct> nodeStructs; //= new NativeArray<NodeStruct>(40000, Allocator.Persistent);

    Queue<Node> usingNodes = new Queue<Node>(1000);
    public MoveCalculator()
    {
       /* for (int i = 0; i < 200; i++)
        {
            for (int j = 0; j < 200; j++)
            {
                int index = i * 200 + j;
                nodeStructs[index] = new NodeStruct(j,i);
               // nodeStructs[i, j] = new NodeStruct(j,i);
                //nodes[i, j] = new Node(i, j);
              //  defaultCoords[i,j] = new Coord(i,j);
            }
        }*/
        gameInstance = GameInstance.GameIns;
    }

    public void Init()
    {
        //   Reset();
        //  coords = new Coord[calculatorScale.sizeY, calculatorScale.sizeX];

        //    coords = defaultCoords;

       /* for (int i = 0; i < gameInstance.calculatorScale.sizeY; i++)
        {
            for (int j = 0; j < gameInstance.calculatorScale.sizeX; j++)
            {
                coords[i, j].Init(blockedAreas[i, j]);
            }
        }*/

    }

    public static void CheckArea(CalculatorScale calculatorScale)
    {
       // GameInstance.GameIns.calculatorScale = calculatorScale;
        float maxX = calculatorScale.maxX;
        float minX = calculatorScale.minX;
        float maxY = calculatorScale.maxY;
        float minY = calculatorScale.minY;
        float distanceSize = calculatorScale.distanceSize;

        int calculateScaleX = (int)((maxX - minX) / distanceSize);
        int calculateScaleY = (int)((maxY - minY) / distanceSize);
        GameInstance.GameIns.calculatorScale.sizeX = calculateScaleX;
        GameInstance.GameIns.calculatorScale.sizeY = calculateScaleY;
        //  blockedAreas = new bool[calculateScaleY ,calculateScaleX];
        for (int i = 0; i < calculateScaleY; i++)
        {
            for (int j = 0; j < calculateScaleX; j++)
            {
                blockedAreas[i, j] = false;
            }
        }
        for (int i = 0; i < calculateScaleY; i++)
        {
            for (int j = 0; j < calculateScaleX; j++)
            {
                float r = minY + i * distanceSize;
                float c = minX + j * distanceSize;
                Vector3 worldPoint = (Vector3.right * (c) + Vector3.forward * (r));

                Vector3 size = GameInstance.GetVector3(distanceSize, distanceSize, distanceSize);

                bool check = Physics.CheckBox(worldPoint, size, Quaternion.Euler(0, 0, 0), 1 << 6 | 1 << 7);
                //bool isWall = Physics.CheckBox(worldPoint,vector, Quaternion.Euler(0, 0, 0), LayerMask.GetMask("wall"));
                if (check)
                {
                    blockedAreas[i, j] = true;
                }
            }
        }
       int xx = 0;
        int yy = 0;
        if(GameInstance.GameIns != null && GameInstance.GameIns.inputManager != null && GameInstance.GameIns.inputManager.cameraRange != null) CheckHirable(GameInstance.GameIns.inputManager.cameraRange.position, ref xx, ref yy, true);
    }

    void ResetNodes()
    {
        while(usingNodes.Count > 0)
        {
            Node node = usingNodes.Dequeue();

            NodePool.ReturnNode(node);  
        }
    }
    // static Node returnCoord100 = new Node(100, 100);
    public Node AStarAlgorithm(Vector3 startVector, Vector3 endVector, MinHeap<Node> openList, HashSet<Node> closedList) //(Coord start, Coord end)
    {
        ResetNodes();
       /* nodeStructs = new NativeArray<NodeStruct>(40000, Allocator.Persistent);

        for (int i = 0; i < 200; i++)
        {
            for (int j = 0; j < 200; j++)
            {
                int index = i * 200 + j;
                nodeStructs[index] = new NodeStruct(j, i);
                // nodeStructs[i, j] = new NodeStruct(j,i);
                //nodes[i, j] = new Node(i, j);
                //  defaultCoords[i,j] = new Coord(i,j);
            }
        }*/


        int playerX = (int)((startVector.x - gameInstance.calculatorScale.minX) / gameInstance.calculatorScale.distanceSize);
        int playerY = (int)((startVector.z - gameInstance.calculatorScale.minY) / gameInstance.calculatorScale.distanceSize);
        int targetX = (int)((endVector.x - gameInstance.calculatorScale.minX) / gameInstance.calculatorScale.distanceSize);
        int targetY = (int)((endVector.z - gameInstance.calculatorScale.minY) / gameInstance.calculatorScale.distanceSize);

        bool playerValidCheck = ValidCheck(playerX, playerY);
        bool targetValidCheck = ValidCheck(targetX, targetY);
        if (!(playerValidCheck && targetValidCheck))
        {
          //  if (!playerValidCheck) Debug.Log("PlayerInvalid");
        //    if (!targetValidCheck) Debug.Log("targetInvalid");
            return null;
        }

       /* int indexStart = playerY * 200 + playerX;
        int indexEnd = targetY * 200 + targetX;
        NodeStruct startNode = nodeStructs[indexStart];
        NodeStruct endNode = nodeStructs[indexEnd];
*/
        //  Node start = nodes[playerY, playerX];
        //  Node end = nodes[targetY, targetX];

        Node start = NodePool.GetNode(playerY, playerX);
        usingNodes.Enqueue(start);
        Node end = NodePool.GetNode(targetY, targetX);
        usingNodes.Enqueue(end);
        if (playerY == targetY && playerX == targetX)
        {
            //   Node already = returnCoord100;// new Coord(100, 100);
            Node already = NodePool.GetNode(100,100);
            usingNodes.Enqueue(already);
          //     Debug.Log("alreadyEnd");
            return already;
        }
        
        if (blockedAreas[targetY, targetX])
        {
            //Debug.Log("EndBlocked");
            return null;
        }


        openList.Add(start);
        while (openList.Count > 0)
        {
            Node node = openList.PopMin(); //F가 최소인 값 추출
            
            closedList.Add(node);

            for (int i = 0; i < 8; i++)  //현재 노드 기준 8방향
            {
                int r = node.r + moveY[i];
                int c = node.c + moveX[i];

                if (ValidCheck(r, c))   //크기 체크
                {
                    Node neighbor = NodePool.GetNode(r,c);// nodes[r, c];
                    usingNodes.Enqueue(neighbor);
                    if (closedList.Contains(neighbor) == false && !blockedAreas[r,c])
                    {
                        if (end.r == r && end.c == c)
                        {

                            end.parentNode = node;  //목적지 탐색 완료
                            return end;
                        }


                        //A*
                        /*bool containedInOpenList = openList.Contains(neighbor);
                        int g = node.parentNode.G + GetDistance(node.parentNode, neighbor);
                        if(g <neighbor.G || !containedInOpenList)
                        {
                            neighbor.G = g;
                            neighbor.parentNode = node;
                            neighbor.H = GetDistance(neighbor, end);
                            neighbor.ResetF();
                            if (!containedInOpenList)
                            {
                                openList.Add(neighbor);
                            }
                        }*/



                        //Theta*
                        bool hasLineOfSight = LineOfSight(node.parentNode, neighbor, 0.5f);  //직선 거리 확인
                        bool containedInOpenList = openList.Contains(neighbor);
                        if (hasLineOfSight)
                        {
                            int g = node.parentNode.G + GetDistance(node.parentNode, neighbor);
                            if (g < neighbor.G || !containedInOpenList)
                            {
                                neighbor.G = g;

                                neighbor.parentNode = node.parentNode;
                            }
                        }
                        else
                        {
                            int g = node.G + GetDistance(node, neighbor);
                            if (g < neighbor.G || !containedInOpenList)
                            {
                                neighbor.G = g;

                                neighbor.parentNode = node;
                            }
                        }
                        neighbor.H = GetDistance(neighbor, end);
                       // neighbor.ResetF();
                        if (!containedInOpenList)
                        {
                            openList.Add(neighbor);
                        }
                    }
                }

            }
        }
        Debug.Log("NotFound");
        return null;
    }
    CancellationTokenSource cts = new();

    public async UniTask<Node> AStarAlgorithm_Async(Vector3 startVector, Vector3 endVector, MinHeap<Node> openList, HashSet<Node> closedList, CancellationToken cancellation = default) //(Coord start, Coord end)
    {

        try
        {
            cancellation.ThrowIfCancellationRequested();
            int playerX = Mathf.RoundToInt((startVector.x - gameInstance.calculatorScale.minX) / gameInstance.calculatorScale.distanceSize);
            int playerY = Mathf.RoundToInt((startVector.z - gameInstance.calculatorScale.minY) / gameInstance.calculatorScale.distanceSize);
            int targetX = Mathf.RoundToInt((endVector.x - gameInstance.calculatorScale.minX) / gameInstance.calculatorScale.distanceSize);
            int targetY = Mathf.RoundToInt((endVector.z - gameInstance.calculatorScale.minY) / gameInstance.calculatorScale.distanceSize);
            /*  int playerX = (int)((startVector.x - gameInstance.calculatorScale.minX) / gameInstance.calculatorScale.distanceSize);
              int playerY = (int)((startVector.z - gameInstance.calculatorScale.minY) / gameInstance.calculatorScale.distanceSize);
              int targetX = (int)((endVector.x - gameInstance.calculatorScale.minX) / gameInstance.calculatorScale.distanceSize);
              int targetY = (int)((endVector.z - gameInstance.calculatorScale.minY) / gameInstance.calculatorScale.distanceSize);*/

            bool playerValidCheck = ValidCheck(playerX, playerY);
            bool targetValidCheck = ValidCheck(targetX, targetY);

            if (!(playerValidCheck && targetValidCheck))
            {
                Debug.Log("AA");
            }

            return await UniTask.RunOnThreadPool<Node>(() =>
            {
                // ResetNodes();

                cancellation.ThrowIfCancellationRequested();
                if (!(playerValidCheck && targetValidCheck))
                {
                    return new Node(100, 100);
                }

                int indexStart = playerY * 200 + playerX;
                int indexEnd = targetY * 200 + targetX;

                Node start = new Node(playerY, playerX); // NodePool.GetNode(playerY, playerX);
                                                         //    usingNodes.Enqueue(start);
                Node end = new Node(targetY, targetX);//NodePool.GetNode(targetY, targetX);
                                                      //   usingNodes.Enqueue(end);
                if (playerY == targetY && playerX == targetX)
                {
                    //   Node already = returnCoord100;// new Coord(100, 100);
                    Node already = new Node(100, 100); //NodePool.GetNode(100, 100);
                                                       //     usingNodes.Enqueue(already);
                                                       //     Debug.Log("alreadyEnd");
                    return already;
                }

                if (blockedAreas[targetY, targetX])
                {
                    //Debug.Log("EndBlocked");
                    return null;
                }
                //  start.parentNode = start;   

                openList.Add(start);
                while (openList.Count > 0)
                {
                    Node node = openList.PopMin(); //F가 최소인 값 추출
                    closedList.Add(node);

                    for (int i = 0; i < 8; i++)  //현재 노드 기준 8방향
                    {
                        int r = node.r + moveY[i];
                        int c = node.c + moveX[i];

                        if (ValidCheck(r, c))   //크기 체크
                        {
                            Node neighbor = new Node(r, c); //NodePool.GetNode(r, c);// nodes[r, c];
                                                            // usingNodes.Enqueue(neighbor);
                            if (closedList.Contains(neighbor) == false && !blockedAreas[r, c])
                            {
                                if (end.r == r && end.c == c)
                                {
                                    end.parentNode = node;  //목적지 탐색 완료
                                    return end;
                                }


                                //A*
                                /*              try
                                              {
                                                  bool containedInOpenLists = openList.Contains(neighbor);
                                              }
                                              catch (Exception e)
                                              {
                                                  await UniTask.SwitchToMainThread();
                                                  Debug.Log(e);
                                                  throw;
                                              }*/

                                /*  bool containedInOpenList = openList.Contains(neighbor);
                                  int g = node.G + GetDistance(node, neighbor);
                                  if (g < neighbor.G || !containedInOpenList)
                                  {
                                      neighbor.G = g;
                                      neighbor.parentNode = node;
                                      neighbor.H = GetDistance(neighbor, end);
                                      neighbor.ResetF();
                                      if (!containedInOpenList)
                                      {
                                          openList.Add(neighbor);
                                      }
                                  }*/


                                //Theta*
                                bool hasLineOfSight = LineOfSight(node.parentNode, neighbor, 0.5f);  //직선 거리 확인
                                bool containedInOpenList = openList.Contains(neighbor);
                                if (hasLineOfSight)
                                {
                                    int g = node.parentNode.G + GetDistance(node.parentNode, neighbor);
                                    if (g < neighbor.G || !containedInOpenList)
                                    {
                                        neighbor.G = g;

                                        neighbor.parentNode = node.parentNode;
                                    }
                                }
                                else
                                {
                                    int g = node.G + GetDistance(node, neighbor);
                                    if (g < neighbor.G || !containedInOpenList)
                                    {
                                        neighbor.G = g;

                                        neighbor.parentNode = node;
                                    }
                                }
                                neighbor.H = GetDistance(neighbor, end);
                                // neighbor.ResetF();
                                if (!containedInOpenList)
                                {
                                    openList.Add(neighbor);
                                }
                            }
                        }

                    }
                }
                return null;
            }, cancellationToken: cancellation);
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("calculator was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in calculator: {ex.Message}");
            throw;
        }
    }
    public static int FastAbs(int x) => x < 0 ? -x : x;

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = FastAbs(nodeA.c - nodeB.c);
        int dstY = FastAbs(nodeA.r - nodeB.r);

          if (dstX > dstY)
              return 14 * dstY + 10 * (dstX - dstY);
          return 14 * dstX + 10 * (dstY - dstX);

       // return Mathf.Sqrt(dstX * dstX + dstY * dstY); //유클리드
    }


    int ReturnDiff(int diffX, int diffY)
    {
        if (diffX == diffY) return 14 * diffX;

        int s = diffY > diffX ? diffX : diffY;
        int b = diffY > diffX ? diffY : diffX;

        int p = s * 14;
        p += (b - s) * 10;


        return p;
    }

   
    Vector3 vSize= new Vector3(0.6f,0.6f,0.6f);
    bool LineOfSight(Node start, Node end, float radius)
    {
        if (start == null || end == null) return false;

        /* Vector3 startPos = GameInstance.GetVector3(
             start.c * gameInstance.calculatorScale.distanceSize + gameInstance.calculatorScale.minX,
             0,
             start.r * gameInstance.calculatorScale.distanceSize + gameInstance.calculatorScale.minY
         );
         Vector3 endPos = GameInstance.GetVector3(
             end.c * gameInstance.calculatorScale.distanceSize + gameInstance.calculatorScale.minX,
             0,
             end.r * gameInstance.calculatorScale.distanceSize + gameInstance.calculatorScale.minY
         );*/

        int s1 = start.c;
        int s2 = start.r;

        int e1 = end.c;
        int e2 = end.r;

      /*  Vector3 startPos = GameInstance.GetVector3(
           start.c,
           0,
           start.r
       );
        Vector3 endPos = GameInstance.GetVector3(
            end.c,
            0,
            end.r
        );*/

        int dx = Mathf.Abs(s1 - e1);
        int dy = Mathf.Abs(s2 - e2);

        int sx = s1 < e1 ? 1 : -1;
        int sy = s2 < e2 ? 1 : -1;
        int err = dx - dy;
        while (true)
        {
            if(ValidCheck(s2,s1))
            {
               
                if (blockedAreas[s2, s1])
                    return false; // 장애물 있음

                if (s1 == e1 && s2 == e2)
                    break;

                int d2 = 2 * err;
                if (d2 > -dy) { err -= dy; s1 += sx; }
                if (d2 < dx) { err += dx; s2 += sy; }

            }
            else
            {
                return false;
            }
         
        }

        return true;
        /*    Vector3 direction = (endPos - startPos); // 시작점에서 끝점으로 가는 방향
            float distance = Vector3.Distance(startPos, endPos); // 시작점과 끝점 간 거리
            if (distance > Mathf.Epsilon)
            {
                direction.x /= distance;
                direction.y /= distance;
                direction.z /= distance;
            }
            // SphereCast를 사용하여 충돌 검사
            return !Physics.BoxCast(startPos, vSize, direction, Quaternion.identity, distance, 1 << 6 | 1 << 7);*/ //!Physics.SphereCast(startPos, radius, direction, out RaycastHit hit, distance, 1 << 6 | 1 << 7);
    }

    public void ResetThisGrids()
    {
        while (usingNodes.Count > 0)
        {
            NodePool.ReturnNode(usingNodes.Dequeue());
        }
      //  if (nodeStructs.IsCreated) nodeStructs.Dispose();
    }
    
    public void RemoveGridData()
    {
        //ResetThisGrids();
    }

    public static bool[,] GetBlocks => blockedAreas;
    
}


/*using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
//한글

public class Coord
{
    public int r, c;
    public bool blocked;
    public int startPoint;
    public int endPoint;
    public int dPoint;
    public int point;

    public Coord parent = null;
    public Coord(int r, int c, int startPoint = 0, int endPoint = 0, bool blocked = false)
    {
        this.r = r;
        this.c = c;
        this.blocked = blocked;
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        dPoint = 0;
        point = 0;
    }

    public void Setup()
    {
        point = startPoint + endPoint;
    }
}


public struct CalculatorScale
{
    public int sizeX;
    public int sizeY;
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
    public float distanceSize;
    public float height;

}

public class MoveCalculator
{
    int[] moveX = { 1, -1, 0, 0, 1, 1, -1, -1 };
    int[] moveY = { 0, 0, 1, -1, 1, -1, 1, -1 };
    static CalculatorScale calculatorScale = new CalculatorScale();
    public Coord result;
    Coord[,] coords;
    static bool[,] blockedAreas;

    public void Init()
    {
        coords = new Coord[calculatorScale.sizeY, calculatorScale.sizeX];
        for (int i = 0; i < calculatorScale.sizeY; i++)
        {
            for (int j = 0; j < calculatorScale.sizeX; j++)
            {
                coords[i, j] = new Coord(i, j);
            }
        }

        for (int i = 0; i < calculatorScale.sizeY; i++)
        {
            for (int j = 0; j < calculatorScale.sizeX; j++)
            {
                if (blockedAreas[i, j]) coords[i, j].blocked = true;
            }
        }

    }

    public static void CheckArea(CalculatorScale calculatorScale)
    {
        MoveCalculator.calculatorScale = calculatorScale;
        float maxX = calculatorScale.maxX;
        float minX = calculatorScale.minX;
        float maxY = calculatorScale.maxY;
        float minY = calculatorScale.minY;
        float distanceSize = calculatorScale.distanceSize;

        int calculateScaleX = (int)((maxX - minX) / distanceSize);
        int calculateScaleY = (int)((maxY - minY) / distanceSize);
        MoveCalculator.calculatorScale.sizeX = calculateScaleX;
        MoveCalculator.calculatorScale.sizeY = calculateScaleY;
        blockedAreas = new bool[calculateScaleY, calculateScaleX];
        for (int i = 0; i < calculateScaleY; i++)
        {
            for (int j = 0; j < calculateScaleX; j++)
            {
                float r = minY + i * distanceSize;
                float c = minX + j * distanceSize;
                Vector3 worldPoint = (Vector3.right * (c) + Vector3.forward * (r));
                bool isBlock = Physics.CheckBox(worldPoint, new Vector3(distanceSize, distanceSize, distanceSize), Quaternion.Euler(0, 0, 0), LayerMask.GetMask("block"));
                bool isWall = Physics.CheckBox(worldPoint, new Vector3(distanceSize, distanceSize, distanceSize), Quaternion.Euler(0, 0, 0), LayerMask.GetMask("wall"));
                if (isBlock || isWall)
                {
                    blockedAreas[i, j] = true;
                }
            }
        }
    }

    public Coord AStarAlgorithm(Vector3 startVector, Vector3 endVector) //(Coord start, Coord end)
    {

        int playerX = (int)((startVector.x - calculatorScale.minX) / calculatorScale.distanceSize);
        int playerY = (int)((startVector.z - calculatorScale.minY) / calculatorScale.distanceSize);
        int targetX = (int)((endVector.x - calculatorScale.minX) / calculatorScale.distanceSize);
        int targetY = (int)((endVector.z - calculatorScale.minY) / calculatorScale.distanceSize);

        bool playerValidCheck = ValidCheck(playerX, playerY);
        bool targetValidCheck = ValidCheck(targetX, targetY);
        if (!(playerValidCheck && targetValidCheck))
        {
            if (!playerValidCheck) Debug.Log("PlayerInvalid");
            if (!targetValidCheck) Debug.Log("targetInvalid");
            return null;
        }

        Coord start = coords[playerY, playerX];
        Coord end = coords[targetY, targetX];

        if (playerY == targetY && playerX == targetX)
        {
            Coord already = new Coord(100, 100);
            //     Debug.Log("alreadyEnd");
            return already;
        }
        //result = start;
        //start.blocked = true;

        if (end.blocked)
        {
            //    Debug.Log("alreadyblock");
            //Debug.Log("EndBlock");
            return null;
        }
        //  Debug.Log(startVector + " " + endVector);
        List<Coord> openList = new List<Coord>();
        List<Coord> closedList = new List<Coord>();
        openList.Add(start);
        while (openList.Count > 0)
        {
            Coord coord = null;
            if (openList.Count == 1)
                coord = openList[0];
            else
            {
                int min = 99999;
                Coord minCoord = null;
                for (int i = 0; i < openList.Count; i++)
                {
                    if (min > openList[i].point)
                    {
                        min = openList[i].point;
                        minCoord = openList[i];
                    }


                }

                coord = minCoord;

            }
            if (coord == null) return null;
            // Debug.Log(coord.r + " " + coord.c);
            closedList.Add(coord);

            openList.Remove(coord);
            // Debug.Log($"{openList.Count} : {coord.r} : {coord.c}");

            List<Coord> coordSort = new List<Coord>();
            for (int i = 0; i < 8; i++)
            {
                int r = coord.r + moveY[i];
                int c = coord.c + moveX[i];

                if (ValidCheck(r, c))
                {

                    if (closedList.Contains(coords[r, c]) == false && coords[r, c].blocked == false)
                    {
                        if (end.r == r && end.c == c)
                        {
                            end.parent = coord;

                            return end;
                        }
                        int startPoint = coord.startPoint + GetDistance(coord, coords[r, c]);
                        if (startPoint < coords[r, c].startPoint || !openList.Contains(coords[r, c]))
                        {
                            coords[r, c].startPoint = startPoint;
                            coords[r, c].endPoint = GetDistance(coords[r, c], end);
                            coords[r, c].Setup();
                            coords[r, c].parent = coord;
                            if (!openList.Contains(coords[r, c])) openList.Add(coords[r, c]);
                        }
                    }
                }

            }
        }

        Debug.Log("NotFound");
        return null;
    }
    int GetDistance(Coord nodeA, Coord nodeB)
    {
        int dstX = Mathf.Abs(nodeA.c - nodeB.c);
        int dstY = Mathf.Abs(nodeA.r - nodeB.r);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }


    int ReturnDiff(int diffX, int diffY)
    {
        if (diffX == diffY) return 14 * diffX;

        int s = diffY > diffX ? diffX : diffY;
        int b = diffY > diffX ? diffY : diffX;

        int p = s * 14;
        p += (b - s) * 10;


        return p;
    }

    bool ValidCheck(int r, int c)
    {
        if (r >= 0 && r < calculatorScale.sizeY && c >= 0 && c < calculatorScale.sizeX)
        {
            return true;
        }

        return false;
    }
}
*/