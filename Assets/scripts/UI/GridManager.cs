using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
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

    bool[] grids = new bool[250 * 250];
    float cellSize = 2.5f;
    Vector2 gridOffsets = new Vector2(0.75f, 1.25f);
    private void Awake()
    {
        GameInstance.GameIns.gridManager = this;
    }
    private void Start()
    {
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

    public bool SelectLine(Vector3 startPos, PlaceController go, bool checkArea)
    {
       
       
        Vector2 gridOffset = new Vector2(0.75f, 1.25f);
        int cellX = Mathf.FloorToInt((startPos.x - gridOffset.x + cellSize * 0.5f) / cellSize);
        int cellY = Mathf.FloorToInt((startPos.z - gridOffset.y + cellSize * 0.5f) / cellSize);
      
    
       

        float halfSize = cellSize * 0.5f;
        float centerX = cellX * cellSize + gridOffset.x;
        float centerZ = cellY * cellSize + gridOffset.y;
        if (x == centerX && y == centerZ) return checkArea;
        x = centerX; y = centerZ;

    /*    if (currentLineRender != null)
            RemoveLine(currentLineRender);

        currentLineRender = GetLine();
        Vector3[] corners = new Vector3[5];
        corners[0] = new Vector3(centerX - halfSize , 11.2f, centerZ - halfSize); 
        corners[1] = new Vector3(centerX + halfSize, 11.2f, centerZ - halfSize); 
        corners[2] = new Vector3(centerX + halfSize , 11.2f, centerZ + halfSize);
        corners[3] = new Vector3(centerX - halfSize, 11.2f, centerZ + halfSize); 
        corners[4] = corners[0]; 

        currentLineRender.positionCount = 5;
        currentLineRender.SetPositions(corners);
        currentLineRender.startWidth = currentLineRender.endWidth = 0.2f;
        currentLineRender.useWorldSpace = true;*/


       
       // int floorCellX = Mathf.FloorToInt((startPos.x - gridOffsets.x + (cellSize + 4.6f) * 0.5f) / cellSize);
      //  int floorCellY = Mathf.FloorToInt((startPos.z - gridOffsets.y + (4.6f + cellSize) * 0.5f) / cellSize);

       // go.transform.position = new Vector3(floorCellX * cellSize + 0.6f, 0, floorCellY * cellSize + 1.25f);
        go.transform.position = new Vector3(cellX * cellSize + go.offsetVector.x , 0, cellY * cellSize + go.offsetVector.y);
        //        bool[] blocks = new bool[MoveCalculator.GetBlocks.Length];
        // Array.Copy(MoveCalculator.GetBlocks, blocks, blocks.Length);

        return CheckObject(go);
    }


    public bool CheckObject(PlaceController go)
    {
        RemoveCell();
        CalculatorScale calculatorScale = GameIns.calculatorScale;
        int ch = 0;

        //   colliders.Clear();
        if (go.colliders.Count == 0) go.GetComponentsInChildren(true, go.colliders);
        foreach (BoxCollider boxCollider in go.colliders)
        {
            if (boxCollider.gameObject.layer == 17)
            {
                Vector3 size = GameInstance.GetVector3(calculatorScale.distanceSize * 3f, calculatorScale.distanceSize * 3f, calculatorScale.distanceSize * 3f);

                float x = Mathf.FloorToInt(boxCollider.transform.position.x / cellSize) * cellSize;
                float z = Mathf.FloorToInt(boxCollider.transform.position.z / cellSize) * cellSize;

                Vector2 vector2 = new Vector2(x, z);
                int gridX = Mathf.FloorToInt((x - calculatorScale.minX) / 2.5f);
                int gridY = Mathf.FloorToInt((z - calculatorScale.minY) / 2.5f);
                if (!cellDic.ContainsKey(vector2))
                {
                    MaterialBlockController cell_GO = GetCell();

                    if ((gridX + gridY * GameIns.calculatorScale.sizeX) < grids.Length && !grids[MoveCalculator.GetIndex(gridX, gridY)])
                    {
                        bool check = Physics.CheckBox(boxCollider.gameObject.transform.position, size, Quaternion.Euler(0, 0, 0), 1 << 6 | 1 << 7 | 1 << 8 | 1 << 16);

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
                        cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                        cell_GO.Set(1);
                        cellDic[vector2] = cell_GO;
                        ch++;
                    }
                }
                else
                {
                    if (cellDic[vector2].GetColorParam() == 0)
                    {
                        if ((gridX + gridY * GameIns.calculatorScale.sizeX) < grids.Length && !grids[MoveCalculator.GetIndex(gridX, gridY)])
                        {
                            bool check = Physics.CheckBox(boxCollider.gameObject.transform.position, size, Quaternion.Euler(0, 0, 0), 1 << 6 | 1 << 7 | 1 << 8 | 1 << 16);
                            if (check)
                            {
                                cellDic[vector2].Set(1);
                                ch++;
                            }
                        }
                        else
                        {
                            cellDic[vector2].Set(1);
                            ch++;
                        }
                    }
                }
            }
        }
        if (ch == 0)
        {
            go.canPlace = true;
            return true;
        }
        else
        {
            go.canPlace = false;
            return false;
        }
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

    public void ApplyGird()
    {
        foreach (var c in cellDic)
        {
            int gridX = Mathf.FloorToInt((c.Key.x - GameIns.calculatorScale.minX) / 2.5f);
            int gridY = Mathf.FloorToInt((c.Key.y - GameIns.calculatorScale.minY) / 2.5f);
            int index = MoveCalculator.GetIndex(gridX, gridY);
            if (index < grids.Length) grids[index] = true;
            c.Value.gameObject.SetActive(false);
            cellQueue.Enqueue(c.Value);
        }
        cellDic.Clear();
    }
    public void RemoveCell()
    {
        foreach (var c in cellDic)
        {
            c.Value.gameObject.SetActive(false);
            cellQueue.Enqueue(c.Value);
        }
        cellDic.Clear();
    }
}
