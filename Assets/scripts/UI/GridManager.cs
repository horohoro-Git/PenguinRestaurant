using CryingSnow.FastFoodRush;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static GameInstance;
public class GridManager : MonoBehaviour
{
    public List<GridView> gridView = new List<GridView>();
    public MaterialBlockController cell;
    public LineRenderer lineRenderer;
    public GameObject selectObject;
    public Material red;
    public Material green;

    Queue<GameObject> selectQueue = new Queue<GameObject>();
    Queue<GameObject> activateSelect = new Queue<GameObject>();
    Queue<LineRenderer> lineQueue = new Queue<LineRenderer>();
    Queue<LineRenderer> activatedLineQueue = new Queue<LineRenderer>();
    Queue<MaterialBlockController> cellQueue = new Queue<MaterialBlockController>();
  //  Queue<GameObject> activatedCellQueue = new Queue<GameObject>();
    LineRenderer currentLineRender;
    GameObject currentSelector;
    Dictionary<Vector2, MaterialBlockController> cellDic = new Dictionary<Vector2, MaterialBlockController>();
    Dictionary<Vector2, MaterialBlockController> cellDic_Table = new Dictionary<Vector2, MaterialBlockController>();
    
    float x;
    float y;

    [NonSerialized] public bool[] grids = new bool[400 * 400];
    [NonSerialized] public int[] counterGrids = new int[400 * 400];
    [NonSerialized] public int[] tableGrids = new int[400 * 400];
    [NonSerialized] public int[] tableCenterGrids = new int[400 * 400];
    [NonSerialized] public int[] trashCanGrids = new int[400 * 400];
    float cellSize = 2.5f;
    Vector2 gridOffsets = new Vector2(0.75f, 1.25f);

    public static GameObject gridObjects;
    private void Awake()
    {
        gridObjects = new GameObject();
        gridObjects.name = "GridObjects";
        gridObjects.transform.position = Vector3.zero;
        red = Instantiate(red);
        green = Instantiate(green);
        GameInstance.GameIns.gridManager = this;
    }
    private void Start()
    {
        CreateCells();
        CreateLines();
       // Debug.Log(worldPoint);
      /*  int x = Mathf.FloorToInt((-3 - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
        int y = Mathf.FloorToInt((-10 - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);
        int gridCellX = Mathf.FloorToInt((x - 0.7f + 2.5f * 0.5f));
        int gridCellY = Mathf.FloorToInt((y - 1.2f + 2.5f * 0.5f));
        Debug.Log(x + " " + y + " " + gridCellX + " " + gridCellY);*/
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
            GameObject select = Instantiate(selectObject, gridObjects.transform);
            select.transform.position = Vector3.zero;
            select.gameObject.SetActive(false);
            selectQueue.Enqueue(select);
        }
    }

    void CreateCells()
    {
        for (int i = 0; i < 30; i++)
        {
            MaterialBlockController c = Instantiate(cell, gridObjects.transform);
            c.transform.position = Vector3.zero;
            c.gameObject.SetActive(false);
            cellQueue.Enqueue(c);
        }
    }

    GameObject GetSelect()
    {
        if (selectQueue.Count > 0)
        {
            GameObject select = selectQueue.Dequeue();
            select.SetActive(true);
            return select;
        }
        else
        {
            GameObject select = Instantiate(selectObject, gridObjects.transform);
            return select;
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



    public bool SelectLine(Vector3 startPos, PlaceController go, bool checkArea, WorkSpaceType workType)
    {
       
        
        Vector2 gridOffset = new Vector2(0.75f, 1.25f);
        int cellX = Mathf.FloorToInt((startPos.x - gridOffset.x + cellSize * 0.5f) / cellSize);
        int cellY = Mathf.FloorToInt((startPos.z - gridOffset.y + cellSize * 0.5f) / cellSize);
      
        float halfSize = cellSize * 0.5f;
        float centerX = cellX * cellSize + gridOffset.x;
        float centerZ = cellY * cellSize + gridOffset.y;
        if (x == centerX && y == centerZ) return checkArea;
        x = centerX; y = centerZ;

        RemoveSelect();

        SoundManager.Instance.PlayAudio(GameIns.uISoundManager.FurniturePlace(), 0.05f);
      
        //RemoveLine();

        currentSelector = GetSelect();
        float offsetX = centerX - 4.5f;
        float offsetY = centerZ - 4.5f;
        // currentLineRender = GetLine();
        /* Vector3[] corners = new Vector3[5];
         float offsetX = centerX - 4.5f;
         float offsetY = centerZ - 4.5f;
         corners[0] = new Vector3(offsetX - halfSize, 11.2f, offsetY - halfSize);
         corners[1] = new Vector3(offsetX + halfSize, 11.2f, offsetY - halfSize);
         corners[2] = new Vector3(offsetX + halfSize, 11.2f, offsetY + halfSize);
         corners[3] = new Vector3(offsetX - halfSize, 11.2f, offsetY + halfSize);
         corners[4] = corners[0];*/

        /*   currentLineRender.positionCount = 5;
           currentLineRender.SetPositions(corners);
           currentLineRender.startWidth = currentLineRender.endWidth = 0.2f;
           currentLineRender.useWorldSpace = true;*/

        currentSelector.transform.position = new Vector3(offsetX, 11.2f, offsetY);

        // int floorCellX = Mathf.FloorToInt((startPos.x - gridOffsets.x + (cellSize + 4.6f) * 0.5f) / cellSize);
        //  int floorCellY = Mathf.FloorToInt((startPos.z - gridOffsets.y + (4.6f + cellSize) * 0.5f) / cellSize);

        // go.transform.position = new Vector3(floorCellX * cellSize + 0.6f, 0, floorCellY * cellSize + 1.25f);
        go.transform.position = new Vector3(cellX * cellSize + go.offsetVector.x , 0, cellY * cellSize + go.offsetVector.y);
        //        bool[] blocks = new bool[MoveCalculator.GetBlocks.Length];
        // Array.Copy(MoveCalculator.GetBlocks, blocks, blocks.Length);

        return CheckObject(go, workType);
    }

    public void ReCalculate(PlaceController go, bool isTable = false)
    {
        RemoveCell();
        RemoveSelect();
      //  RemoveLine();
        Vector2 gridOffset = new Vector2(0.75f, 1.25f);
        int cellX = Mathf.FloorToInt((go.transform.position.x - gridOffset.x + cellSize * 0.5f) / cellSize);
        int cellY = Mathf.FloorToInt((go.transform.position.z - gridOffset.y + cellSize * 0.5f) / cellSize);




        float halfSize = cellSize * 0.5f;
        float centerX = cellX * cellSize + gridOffset.x;
        float centerZ = cellY * cellSize + gridOffset.y;

        currentSelector = GetSelect();

    //    currentLineRender = GetLine();
       // Vector3[] corners = new Vector3[5];
        float offsetX = centerX - 4.5f;
        float offsetY = centerZ - 4.5f;
      
        currentSelector.transform.position = new Vector3(offsetX, 11.2f, offsetY);

        CalculatorScale calculatorScale = GameIns.calculatorScale;
        if (go.colliders.Count == 0) go.GetComponentsInChildren(true, go.colliders);
        foreach (BoxCollider boxCollider in go.colliders)
        {
            if (boxCollider.gameObject.layer == 17)
            {
                Vector3 size = GameInstance.GetVector3(calculatorScale.distanceSize * 3f, calculatorScale.distanceSize * 3f, calculatorScale.distanceSize * 3f);
                float x = Mathf.FloorToInt(boxCollider.transform.position.x / cellSize) * cellSize;
                float z = Mathf.FloorToInt(boxCollider.transform.position.z / cellSize) * cellSize;
                Vector2 vector2 = new Vector2(x, z);
                int gridX = Mathf.FloorToInt((x - calculatorScale.minX) / cellSize);
                int gridY = Mathf.FloorToInt((z - calculatorScale.minY) / cellSize);
                if (!cellDic.ContainsKey(vector2))
                {
                    MaterialBlockController cell_GO = GetCell();

                    bool check = Physics.CheckBox(boxCollider.gameObject.transform.position, size, Quaternion.Euler(0, 0, 0), 1 << 6 | 1 << 7 | 1 << 8 | 1 << 16 | 1 << 19 | 1 << 21);

                    cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                    cell_GO.Set(0, green);

                    cellDic[vector2] = cell_GO;
                    go.temp.Add(vector2);

                    if (go.storeGoods.goods.type == WorkSpaceType.Table)
                    {
                        float xx = Mathf.FloorToInt(go.transform.position.x / cellSize) * cellSize;
                        float zz = Mathf.FloorToInt(go.transform.position.z / cellSize) * cellSize;
                        if (x == xx && z == zz)
                        {
                            go.tempTableCenter.Add(vector2);
                        }
                        else
                        {
                            go.tempTable.Add(vector2);
                        }
                    }
                }
            }

        }

        if (go.storeGoods.goods.type == WorkSpaceType.Trashcan)
        {
            float x = Mathf.FloorToInt(go.model.transform.position.x / cellSize) * cellSize;
            float z = Mathf.FloorToInt(go.model.transform.position.z / cellSize) * cellSize;
            Vector2 vector2 = new Vector2(x, z);
            int gridX = Mathf.FloorToInt((x - calculatorScale.minX) / cellSize);
            int gridY = Mathf.FloorToInt((z - calculatorScale.minY) / cellSize);

            if (!cellDic.ContainsKey(vector2))
            {
                MaterialBlockController cell_GO = GetCell();

                bool check = Physics.CheckBox(go.model.gameObject.transform.position, new Vector3(0.5f,0.5f,0.5f), Quaternion.Euler(0, 0, 0), 1 << 6 | 1 << 7 | 1 << 8 | 1 << 16 | 1 << 19 | 1 << 21);

                cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                cell_GO.Set(0, green);

                cellDic[vector2] = cell_GO;
                go.temp.Add(vector2);

                if (go.storeGoods.goods.type == WorkSpaceType.Table)
                {
                    float xx = Mathf.FloorToInt(go.model.transform.position.x / cellSize) * cellSize;
                    float zz = Mathf.FloorToInt(go.model.transform.position.z / cellSize) * cellSize;
                    if (x == xx && z == zz)
                    {
                        go.tempTableCenter.Add(vector2);
                    }
                    else
                    {
                        go.tempTable.Add(vector2);
                    }
                }
            }

        }

        go.canPlace = true;
        foreach (var c in go.temp)
        {
            int gridX = Mathf.FloorToInt((c.x - GameIns.calculatorScale.minX) / 2.5f);
            int gridY = Mathf.FloorToInt((c.y - GameIns.calculatorScale.minY) / 2.5f);
            int index = MoveCalculator.GetIndex(gridX, gridY);
            if (index < grids.Length) grids[index] = false;
        }

        foreach (var t in go.tempTable)
        {
            int gridX = Mathf.FloorToInt((t.x - GameIns.calculatorScale.minX) / 2.5f);
            int gridY = Mathf.FloorToInt((t.y - GameIns.calculatorScale.minY) / 2.5f);
            int index = MoveCalculator.GetIndex(gridX, gridY);
            if (index < tableGrids.Length) tableGrids[index] -= 1;
        }
        foreach(var t in go.tempTableCenter)
        {
            int gridX = Mathf.FloorToInt((t.x - GameIns.calculatorScale.minX) / 2.5f);
            int gridY = Mathf.FloorToInt((t.y - GameIns.calculatorScale.minY) / 2.5f);
            int index = MoveCalculator.GetIndex(gridX, gridY);
            if (index < tableCenterGrids.Length) tableCenterGrids[index] -= 1;
        }
        if (go.storeGoods.goods.type == WorkSpaceType.Trashcan)
        {
            float x2 = Mathf.FloorToInt(go.model.transform.position.x / cellSize) * cellSize;
            float z2 = Mathf.FloorToInt(go.model.transform.position.z / cellSize) * cellSize;
            int gridX2 = Mathf.FloorToInt((x2 - GameIns.calculatorScale.minX) / 2.5f);
            int gridY2 = Mathf.FloorToInt((z2 - GameIns.calculatorScale.minY) / 2.5f);
            int index2 = MoveCalculator.GetIndex(gridX2, gridY2);

            if (index2 < tableCenterGrids.Length)
            {
                Debug.Log(index2);
                tableCenterGrids[index2] -= 1;
                if (index2 < grids.Length) grids[index2] = false;

            }
        }
    }

    public void Revert(PlaceController go)
    {
        foreach (var c in go.temp)
        {
            int gridX = Mathf.FloorToInt((c.x - GameIns.calculatorScale.minX) / 2.5f);
            int gridY = Mathf.FloorToInt((c.y - GameIns.calculatorScale.minY) / 2.5f);
            int index = MoveCalculator.GetIndex(gridX, gridY);
            if (index < grids.Length) grids[index] = true;
        }

        if (go.storeGoods.goods.type == WorkSpaceType.Table)
        {
            foreach (var t in go.tempTable)
            {
                int gridX = Mathf.FloorToInt((t.x - GameIns.calculatorScale.minX) / 2.5f);
                int gridY = Mathf.FloorToInt((t.y - GameIns.calculatorScale.minY) / 2.5f);
                int index = MoveCalculator.GetIndex(gridX, gridY);
                if (index < tableGrids.Length) tableGrids[index] += 1;

            }

            foreach (var t in go.tempTableCenter)
            {
                int gridX = Mathf.FloorToInt((t.x - GameIns.calculatorScale.minX) / 2.5f);
                int gridY = Mathf.FloorToInt((t.y - GameIns.calculatorScale.minY) / 2.5f);
                int index = MoveCalculator.GetIndex(gridX, gridY);
                if (index < tableCenterGrids.Length) tableCenterGrids[index] += 1;
            }
        }


        if (go.storeGoods.goods.type == WorkSpaceType.Trashcan)
        {
            float x2 = Mathf.FloorToInt(go.model.transform.position.x / cellSize) * cellSize;
            float z2 = Mathf.FloorToInt(go.model.transform.position.z / cellSize) * cellSize;
            int gridX2 = Mathf.FloorToInt((x2 - GameIns.calculatorScale.minX) / 2.5f);
            int gridY2 = Mathf.FloorToInt((z2 - GameIns.calculatorScale.minY) / 2.5f);
            int index2 = MoveCalculator.GetIndex(gridX2, gridY2);

            if (index2 < tableCenterGrids.Length)
            {
                tableCenterGrids[index2] += 1;
            }
        }


        go.temp.Clear();
        go.tempTable.Clear();
        go.tempTableCenter.Clear();
    }

    public bool CheckObject(PlaceController go, WorkSpaceType workType)
    {
       // go.temp.Clear();
        RemoveCell();
        CalculatorScale calculatorScale = GameIns.calculatorScale;
        int ch = 0;

        //   colliders.Clear();
        if (go.colliders.Count == 0) go.GetComponentsInChildren(true, go.colliders);
        foreach (BoxCollider boxCollider in go.colliders)
        {
            if (boxCollider.gameObject.layer == 17)
            {
                Vector3 size = GameInstance.GetVector3(calculatorScale.distanceSize * 2.5f, calculatorScale.distanceSize * 2.5f, calculatorScale.distanceSize * 2.5f);

                float x = Mathf.FloorToInt(boxCollider.transform.position.x / cellSize) * cellSize;
                float z = Mathf.FloorToInt(boxCollider.transform.position.z / cellSize) * cellSize;

                Vector2 vector2 = new Vector2(x, z);
                int gridX = Mathf.FloorToInt((x - calculatorScale.minX) / 2.5f);
                int gridY = Mathf.FloorToInt((z - calculatorScale.minY) / 2.5f);
                if (workType == WorkSpaceType.Table)
                {
                    if (!cellDic.ContainsKey(vector2))
                    {
                        MaterialBlockController cell_GO = GetCell();

                        if ((gridX + gridY * GameIns.calculatorScale.sizeX) < grids.Length && (gridX + gridY * GameIns.calculatorScale.sizeX) >= 0 && !grids[MoveCalculator.GetIndex(gridX, gridY)])
                        {
                            if (tableCenterGrids[MoveCalculator.GetIndex(gridX, gridY)] > 0)
                            {
                                //설치된 다른 테이블 중앙 체크
                                float xx = Mathf.FloorToInt(go.transform.position.x / cellSize) * cellSize;
                                float zz = Mathf.FloorToInt(go.transform.position.z / cellSize) * cellSize;

                                cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                                if (!(xx == x && zz == z))
                                {
                                    cell_GO.Set(0, green);

                                }
                                else
                                {
                                    cell_GO.Set(1, red);
                                    ch++;
                                }
                                cellDic[vector2] = cell_GO;
                            }
                            else
                            {
                                bool check = Physics.CheckBox(boxCollider.gameObject.transform.position, new Vector3(1.25f, 1.25f, 1.25f), Quaternion.Euler(0, 0, 0), 1 << 7 | 1 << 8 | 1 << 16 | 1 << 19 | 1 << 21);
                                cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                                if (check)
                                {
                                    cell_GO.Set(1, red);
                                    ch++;
                                }
                                else
                                {
                                    cell_GO.Set(0, green);
                                }
                                cellDic[vector2] = cell_GO;
                            }
                        }
                        else if ((gridX + gridY * GameIns.calculatorScale.sizeX) < grids.Length && (gridX + gridY * GameIns.calculatorScale.sizeX) >= 0)
                        {
                            if (tableGrids[MoveCalculator.GetIndex(gridX, gridY)] > 0)
                            {
                                //겹칠 수 있는 테이블의 외각
                                if (tableCenterGrids[MoveCalculator.GetIndex(gridX, gridY)] > 0)
                                {
                                    float xx = Mathf.FloorToInt(go.transform.position.x / cellSize) * cellSize;
                                    float zz = Mathf.FloorToInt(go.transform.position.z / cellSize) * cellSize;
                                    cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                                    if ((xx == x && zz == z))
                                    {
                                        cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                                        cell_GO.Set(1, red);
                                        ch++;
                                        cellDic[vector2] = cell_GO;
                                    }
                                    else
                                    {
                                        cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                                        cell_GO.Set(0, green);
                                        cellDic[vector2] = cell_GO;
                                    }
                                }
                                else
                                {
                                    cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                                    cell_GO.Set(0, green);
                                    cellDic[vector2] = cell_GO;
                                }
                            }
                            else if (tableCenterGrids[MoveCalculator.GetIndex(gridX, gridY)] > 0)
                            {
                                //다른 테이블과 완전히 겹치는지 체크
                                float xx = Mathf.FloorToInt(go.transform.position.x / cellSize) * cellSize;
                                float zz = Mathf.FloorToInt(go.transform.position.z / cellSize) * cellSize;
                                cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                                if (!(xx == x && zz == z))
                                {
                                    cell_GO.Set(0, green);
                                }
                                else
                                {
                                    cell_GO.Set(1, red);
                                    ch++;
                                }
                                cellDic[vector2] = cell_GO;
                            }
                            else
                            {
                                cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                                cell_GO.Set(1, red);
                                ch++;
                                cellDic[vector2] = cell_GO;
                            }
                        }
                        else
                        {
                            cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                            cell_GO.Set(1, red);
                            ch++;
                            cellDic[vector2] = cell_GO;
                        }

                    }
                }
                else if (workType == WorkSpaceType.Trashcan)
                {
                    if (!cellDic.ContainsKey(vector2))
                    {
                        MaterialBlockController cell_GO = GetCell();

                        if ((gridX + gridY * GameIns.calculatorScale.sizeX) < grids.Length && (gridX + gridY * GameIns.calculatorScale.sizeX) >= 0 && !grids[MoveCalculator.GetIndex(gridX, gridY)] && trashCanGrids[MoveCalculator.GetIndex(gridX, gridY)] == 0)
                        {
                            bool check = Physics.CheckBox(boxCollider.gameObject.transform.position, new Vector3(1.25f, 1.25f, 1.25f), Quaternion.Euler(0, 0, 0), 1 << 6 | 1 << 7 | 1 << 8 | 1 << 16 | 1 << 19 | 1 << 21);

                            cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                            if (check)
                            {
                                cell_GO.Set(1, red);
                                ch++;
                            }
                            else
                            {
                                cell_GO.Set(0, green);
                            }
                            cellDic[vector2] = cell_GO;
                        }
                        else
                        {
                            cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                            cell_GO.Set(1, red);
                            cellDic[vector2] = cell_GO;
                            ch++;
                        }
                    }
                    else
                    {
                        if (cellDic[vector2].GetColorParam() == 0)
                        {
                            if ((gridX + gridY * GameIns.calculatorScale.sizeX) < grids.Length && (gridX + gridY * GameIns.calculatorScale.sizeX) >= 0 && !grids[MoveCalculator.GetIndex(gridX, gridY)] && trashCanGrids[MoveCalculator.GetIndex(gridX,gridY)] == 0)
                            {
                                bool check = Physics.CheckBox(boxCollider.gameObject.transform.position, new Vector3(1.25f, 1.25f, 1.25f), Quaternion.Euler(0, 0, 0), 1 << 6 | 1 << 7 | 1 << 8 | 1 << 16);
                                if (check)
                                {
                                    cellDic[vector2].Set(1, red);
                                    ch++;
                                }
                            }
                            else
                            {
                                cellDic[vector2].Set(1, red);
                                ch++;
                            }
                        }
                    }

                    float x2 = Mathf.FloorToInt(go.model.transform.position.x / cellSize) * cellSize;
                    float z2 = Mathf.FloorToInt(go.model.transform.position.z / cellSize) * cellSize;

                    Vector2 vector22 = new Vector2(x2, z2);
                    int gridX2 = Mathf.FloorToInt((x2 - calculatorScale.minX) / 2.5f);
                    int gridY2 = Mathf.FloorToInt((z2 - calculatorScale.minY) / 2.5f);
                    Debug.Log(trashCanGrids[MoveCalculator.GetIndex(gridX2, gridY2)]);
                    if (!cellDic.ContainsKey(vector22))
                    {
                        MaterialBlockController cell_GO2 = GetCell();

                        if ((gridX2 + gridY2 * GameIns.calculatorScale.sizeX) < grids.Length && (gridX2 + gridY2 * GameIns.calculatorScale.sizeX) >= 0 && !grids[MoveCalculator.GetIndex(gridX2, gridY2)] && trashCanGrids[MoveCalculator.GetIndex(gridX2, gridY2)] == 0)
                        {
                            bool check = Physics.CheckBox(go.model.gameObject.transform.position, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(0, 0, 0), 1 << 6 | 1 << 7 | 1 << 8 | 1 << 16 | 1 << 19 | 1 << 21);

                            cell_GO2.transform.position = new Vector3(x2 + gridOffsets.x, 0.2f, z2 + gridOffsets.y);
                            if (check)
                            {
                                cell_GO2.Set(1, red);
                                ch++;
                            }
                            else
                            {
                                cell_GO2.Set(0, green);
                            }
                            cellDic[vector22] = cell_GO2;
                        }
                        else
                        {
                            Debug.Log("BB " + trashCanGrids[MoveCalculator.GetIndex(gridX2, gridY2)]);
                            cell_GO2.transform.position = new Vector3(x2 + gridOffsets.x, 0.2f, z2 + gridOffsets.y);
                            cell_GO2.Set(1, red);
                            cellDic[vector22] = cell_GO2;
                            ch++;
                        }
                    }
                    else
                    {
                        if (cellDic[vector22].GetColorParam() == 0)
                        {
                            if (!((gridX2 + gridY2 * GameIns.calculatorScale.sizeX) < grids.Length && (gridX2 + gridY2 * GameIns.calculatorScale.sizeX) >= 0 && !grids[MoveCalculator.GetIndex(gridX2, gridY2)] && trashCanGrids[MoveCalculator.GetIndex(gridX2, gridY2)] == 0))
                            {
                                cellDic[vector22].Set(1, red);
                                ch++;
                            }
                          
                        }
                    }
                }
                else
                {
                    if (!cellDic.ContainsKey(vector2))
                    {
                        MaterialBlockController cell_GO = GetCell();

                        if ((gridX + gridY * GameIns.calculatorScale.sizeX) < grids.Length && (gridX + gridY * GameIns.calculatorScale.sizeX) >= 0 && !grids[MoveCalculator.GetIndex(gridX, gridY)])
                        {
                            bool check = Physics.CheckBox(boxCollider.gameObject.transform.position, new Vector3(1.25f, 1.25f, 1.25f), Quaternion.Euler(0, 0, 0), 1 << 6 | 1 << 7 | 1 << 8 | 1 << 16 | 1 << 19 | 1 << 21);

                            cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                            if (check)
                            {
                                cell_GO.Set(1, red);
                                ch++;
                            }
                            else
                            {
                                cell_GO.Set(0, green);
                            }
                            cellDic[vector2] = cell_GO;
                        }
                        else
                        {
                            cell_GO.transform.position = new Vector3(x + gridOffsets.x, 0.2f, z + gridOffsets.y);
                            cell_GO.Set(1, red);
                            cellDic[vector2] = cell_GO;
                            ch++;
                        }
                    }
                    else
                    {
                        if (cellDic[vector2].GetColorParam() == 0)
                        {
                            if ((gridX + gridY * GameIns.calculatorScale.sizeX) < grids.Length && (gridX + gridY * GameIns.calculatorScale.sizeX) >= 0 && !grids[MoveCalculator.GetIndex(gridX, gridY)])
                            {
                                bool check = Physics.CheckBox(boxCollider.gameObject.transform.position, new Vector3(1.25f, 1.25f, 1.25f), Quaternion.Euler(0, 0, 0), 1 << 6 | 1 << 7 | 1 << 8 | 1 << 16);
                                if (check)
                                {
                                    cellDic[vector2].Set(1, red);
                                    ch++;
                                }
                            }
                            else
                            {
                                cellDic[vector2].Set(1, red);
                                ch++;
                            }
                        }
                    }
                }
            }
        }

      /*  if (workType == WorkSpaceType.Trashcan)
        {
            float x2 = Mathf.FloorToInt(go.transform.position.x / cellSize) * cellSize;
            float z2 = Mathf.FloorToInt(go.transform.position.z / cellSize) * cellSize;
            if ((gridX2 + gridY2 * GameIns.calculatorScale.sizeX) < grids.Length && (gridX2 + gridY2 * GameIns.calculatorScale.sizeX) >= 0 && !grids[MoveCalculator.GetIndex(gridX2, gridY2)])
            {
            }
        }*/
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
    public void RemoveSelect()
    {
        if (currentSelector != null)
        {
            currentSelector.SetActive(false);
            selectQueue.Enqueue(currentSelector);
            currentSelector = null;
        }

    }
    public void RemoveLine()
    {
        if(currentLineRender != null)
        {
            currentLineRender.gameObject.SetActive(false);
            lineQueue.Enqueue(currentLineRender);
            currentLineRender = null;
        }
     
    }

    public void ApplyGird(PlaceController placeController, Vector3 position, WorkSpaceType workType)
    {
        foreach (var c in cellDic)
        {
            int gridX = Mathf.FloorToInt((c.Key.x - GameIns.calculatorScale.minX) / 2.5f);
            int gridY = Mathf.FloorToInt((c.Key.y - GameIns.calculatorScale.minY) / 2.5f);
            int index = MoveCalculator.GetIndex(gridX, gridY);
            if (index < grids.Length) grids[index] = true;
            if (workType == WorkSpaceType.Table)
            {
                if (index < tableGrids.Length)
                {
                    float x = Mathf.FloorToInt(position.x / cellSize) * cellSize;
                    float z = Mathf.FloorToInt(position.z / cellSize) * cellSize;
                    if (x == c.Key.x && z == c.Key.y)
                    {
                        tableCenterGrids[index] += 1;
                    }
                    else
                    {
                        tableGrids[index] += 1;
                    }
                }

            }
     
            c.Value.gameObject.SetActive(false);
            cellQueue.Enqueue(c.Value);

        }
        cellDic.Clear();

        if (workType == WorkSpaceType.Trashcan)
        {
            float x2 = Mathf.FloorToInt(placeController.model.transform.position.x / cellSize) * cellSize;
            float z2 = Mathf.FloorToInt(placeController.model.transform.position.z / cellSize) * cellSize;
            int gridX2 = Mathf.FloorToInt((x2 - GameIns.calculatorScale.minX) / 2.5f);
            int gridY2 = Mathf.FloorToInt((z2 - GameIns.calculatorScale.minY) / 2.5f);
            int index2 = MoveCalculator.GetIndex(gridX2, gridY2);

            if (index2 < tableCenterGrids.Length)
            {
                tableCenterGrids[index2] += 1;
            }
            Debug.Log(index2);
        }


        placeController.temp.Clear();
        if(workType == WorkSpaceType.Table)
        {
            placeController.tempTable.Clear();
            placeController.tempTableCenter.Clear();
        }
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

    public void VisibleGrid(bool isVisible)
    {
        for(int i=0; i< gridView.Count; i++)
        {
            if(gridView[i].gameObject.activeSelf)
            {
                for (int j = 0; j < gridView[i].views.Count; j++)
                {
                    gridView[i].views[j].SetActive(isVisible);
                }
            }
        }
    }
}
