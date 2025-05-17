using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static GameInstance;
public class GridManager : MonoBehaviour
{
    public GameObject grid;
    public MaterialBlockController cell;
    public LineRenderer lineRenderer;

    Queue<LineRenderer> lineQueue = new Queue<LineRenderer>();
    Queue<LineRenderer> activatedLineQueue = new Queue<LineRenderer>();
    Queue<MaterialBlockController> cellQueue = new Queue<MaterialBlockController>();
  //  Queue<GameObject> activatedCellQueue = new Queue<GameObject>();
    LineRenderer currentLineRender;

    Dictionary<Vector2, MaterialBlockController> cellDic = new Dictionary<Vector2, MaterialBlockController>();
    
    float x;
    float y;

    bool[] grids = new bool[400];
    List<BoxCollider> colliders = new List<BoxCollider>(100);
   
    private void Awake()
    {
        GameInstance.GameIns.gridManager = this;
    }
    private void Start()
    {
        CreateLines();
        CreateCells();

       // Debug.Log(worldPoint);
        int x = Mathf.FloorToInt((-3 - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
        int y = Mathf.FloorToInt((-10 - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);
        int gridCellX = Mathf.FloorToInt((x - 0.7f + 2.5f * 0.5f));
        int gridCellY = Mathf.FloorToInt((y - 1.2f + 2.5f * 0.5f));
        Debug.Log(x + " " + y + " " + gridCellX + " " + gridCellY);
    }

   /* private void Update()
    {
        if (InputManger.cachingCamera)
        {
            Ray ray = InputManger.cachingCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1))
            {
          //      SelectLine(hit.point);
            }
        }
    }*/

    void CreateLines()
    {
        for (int i = 0; i < 10; i++)
        {
            LineRenderer line = Instantiate(lineRenderer);
            line.gameObject.SetActive(false);
            lineQueue.Enqueue(line);
        }
    }

    void CreateCells()
    {
        for (int i = 0; i < 30; i++)
        {
            MaterialBlockController c = Instantiate(cell);
            c.gameObject.SetActive(false);
            cellQueue.Enqueue(c);
        }
    }

    LineRenderer GetLine()
    {
        if (lineQueue.Count > 0)
        {
            LineRenderer line = lineQueue.Dequeue();
            line.gameObject.SetActive(true);
            return line;
        }
        else
        {
            LineRenderer line = Instantiate(lineRenderer);
            return line;
        }
    }

    MaterialBlockController GetCell()
    {
        if(cellQueue.Count > 0)
        {
            MaterialBlockController c = cellQueue.Dequeue();
            c.gameObject.SetActive(true);
          //  activatedCellQueue.Enqueue(c);
            return c;
        }
        else
        {
            MaterialBlockController c = Instantiate(cell);
        //    activatedCellQueue.Enqueue(c);
            return c;
        }
    }

    public bool SelectLine(Vector3 startPos, GameObject go, bool checkArea)
    {
       
        float cellSize = 2.5f;
        Vector2 gridOffset = new Vector2(1.2f, 1.7f);
        int cellX = Mathf.FloorToInt((startPos.x - gridOffset.x -4.5f + cellSize * 0.5f) / cellSize);
        int cellY = Mathf.FloorToInt((startPos.z - gridOffset.y - 4.5f + cellSize * 0.5f) / cellSize);
      
    
        Vector3[] corners = new Vector3[5];

        float halfSize = cellSize * 0.5f;
        float centerX = cellX * cellSize + gridOffset.x;
        float centerZ = cellY * cellSize + gridOffset.y;
        if (x == centerX && y == centerZ) return checkArea;
        x = centerX; y = centerZ;




        if (currentLineRender != null)
            RemoveLine(currentLineRender);

        currentLineRender = GetLine();

        corners[0] = new Vector3(centerX - halfSize , 11.2f, centerZ - halfSize); 
        corners[1] = new Vector3(centerX + halfSize, 11.2f, centerZ - halfSize); 
        corners[2] = new Vector3(centerX + halfSize , 11.2f, centerZ + halfSize);
        corners[3] = new Vector3(centerX - halfSize, 11.2f, centerZ + halfSize); 
        corners[4] = corners[0]; 

        currentLineRender.positionCount = 5;
        currentLineRender.SetPositions(corners);
        currentLineRender.startWidth = currentLineRender.endWidth = 0.2f;
        currentLineRender.useWorldSpace = true;


        Vector2 gridOffsets = new Vector2(0.7f, 1.2f);
        int floorCellX = Mathf.FloorToInt((startPos.x - gridOffsets.x + cellSize * 0.5f) / cellSize);
        int floorCellY = Mathf.FloorToInt((startPos.z - gridOffsets.y + cellSize * 0.5f) / cellSize);

        go.transform.position = new Vector3(floorCellX * cellSize + 0.4f, 0, floorCellY * cellSize + 1.25f);
        //        bool[] blocks = new bool[MoveCalculator.GetBlocks.Length];
        // Array.Copy(MoveCalculator.GetBlocks, blocks, blocks.Length);

        RemoveCell();
        bool[] blockArea = MoveCalculator.GetBlocks;
        CalculatorScale calculatorScale = GameIns.calculatorScale;
        int ch = 0;

        colliders.Clear();
        go.GetComponentsInChildren(true, colliders);
        foreach (BoxCollider boxCollider in colliders)
        {
            if(boxCollider.gameObject.layer == 17)
            {
                Vector3 size = GameInstance.GetVector3(calculatorScale.distanceSize * 3f, calculatorScale.distanceSize * 3f, calculatorScale.distanceSize * 3f);
                bool check = Physics.CheckBox(boxCollider.gameObject.transform.position, size, Quaternion.Euler(0, 0, 0), 1 << 6 | 1<< 7 | 1 <<8 | 1 << 16);
            
                float x = Mathf.FloorToInt(boxCollider.transform.position.x / cellSize) * cellSize;
                float z = Mathf.FloorToInt(boxCollider.transform.position.z / cellSize) * cellSize;
          
                Vector2 vector2 = new Vector2(x, z);
                if (!cellDic.ContainsKey(vector2))
                {
                  //  Debug.Log(boxCollider.name + " " + x + " " + z);
                    MaterialBlockController cell_GO = GetCell();

                    cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                    if (check)
                    {
                        cell_GO.Set(1);
                        ch++;
                    }
                    else
                    {
                        cell_GO.Set(0);
                    
                    }
                    cellDic[vector2] = cell_GO;
                }
                else
                {
                    if (check) cellDic[vector2].Set(1);
                  
                }
            }
        }
     //   Debug.Log(ch);
        if(ch == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
        /*for (int i = 0; i < calculatorScale.sizeX; i++)
        {
            for (int j = 0; j < calculatorScale.sizeY; j++)
            {
                float r = calculatorScale.minY + j * calculatorScale.distanceSize;
                float c = calculatorScale.minX + i * calculatorScale.distanceSize;

                //실제 좌표
                Vector3 worldPoint = (Vector3.right * (c) + Vector3.forward * (r));

                Vector3 size = GameInstance.GetVector3(calculatorScale.distanceSize * 2f, calculatorScale.distanceSize * 2f, calculatorScale.distanceSize * 2f);
                Debug.DrawLine(worldPoint - size, worldPoint + size, Color.red, 1f);
                bool check = Physics.CheckBox(worldPoint, size, Quaternion.Euler(0, 0, 0), 1 << 17);
                //bool isWall = Physics.CheckBox(worldPoint,vector, Quaternion.Euler(0, 0, 0), LayerMask.GetMask("wall"));
                if (check)
                {
                    Debug.Log(worldPoint);
                    int x = Mathf.FloorToInt((c - calculatorScale.minX) / calculatorScale.distanceSize);
                    int y = Mathf.FloorToInt((r - calculatorScale.minY) / calculatorScale.distanceSize);
                      int gridCellX = Mathf.FloorToInt((c - gridOffsets.x + cellSize * 0.5f));
                      int gridCellY = Mathf.FloorToInt((r - gridOffsets.y + cellSize * 0.5f));
                   // int gridCellX = Mathf.FloorToInt((c - gridOffsets.x));
                  //  int gridCellY = Mathf.FloorToInt((r - gridOffsets.y));

                    Vector2 vector2 = new Vector2(gridCellX, gridCellY);
                    if (!cellDic.ContainsKey(vector2))
                    {
                        ch++;
                        MaterialPropertyBlock _propBlock = new MaterialPropertyBlock();
                        GameObject cell_GO = GetCell();
                        Renderer renderer = cell_GO.GetComponent<Renderer>();
                       
                        cell_GO.transform.position = new Vector3(gridCellX, 0.1f, gridCellY);
                        if (!Utility.ValidCheckWithCharacterSize(x, y, MoveCalculator.moveX, MoveCalculator.moveY))
                        {
                            //실패 붉은 색 표시
                            renderer.GetPropertyBlock(_propBlock);
                            _propBlock.SetFloat("_ColorBlend", 1);
                            renderer.SetPropertyBlock(_propBlock);

                        }
                        else
                        {
                            //성공 초록색 표시
                            renderer.GetPropertyBlock(_propBlock);
                            _propBlock.SetFloat("_ColorBlend", 0);
                            renderer.SetPropertyBlock(_propBlock);

                        }

                        cellDic[vector2] = cell_GO;
                    }

                }
            }
        }*/


    }


    public Vector3 FurniturePreview(Vector3 startPos)
    {
        float cellSize = 2.5f;
        Vector2 gridOffset = new Vector2(0, 0);
        int cellX = Mathf.FloorToInt((startPos.x - gridOffset.x + cellSize * 0.5f) / cellSize);
        int cellY = Mathf.FloorToInt((startPos.z - gridOffset.y + cellSize * 0.5f) / cellSize);
        if (x == cellX && y == cellY) return new Vector3(cellX, 0, cellY);
        x = cellX; y = cellY;

        return new Vector3(x * cellSize, 0, y * cellSize);
    }
    void RemoveLine(LineRenderer line)
    {
        line.gameObject.SetActive(false);
        lineQueue.Enqueue(line);
    }
    void RemoveCell()
    {
        foreach (var c in cellDic)
        {
            c.Value.gameObject.SetActive(false);
            cellQueue.Enqueue(c.Value);
        }
        cellDic.Clear();
    }
}
