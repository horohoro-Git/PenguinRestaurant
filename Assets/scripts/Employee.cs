using Cysharp.Threading.Tasks;
#if HAS_DOTWEEN
using DG.Tweening;
#endif
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;
using static GameInstance;
public class Employee : AnimalController
{
    enum EmployeeActions
    {
        NONE,
        ProcessWait,
        EmployeeWait,
        EmployeePatrol,
        EmployeeFoodMachine,
        EmployeeCounter,
        EmployeePackingTable,
        EmployeeNowPacking,
        EmployeeGetPackageFoods,
        EmployeeDeliveryFoods,
        EmployeeServing,
        EmployeeTable,
        EmployeeTrashcan,
        EmployeeGetRewards


    }

    EmployeeActions employeeActions = EmployeeActions.NONE;

    public Dictionary<MachineType, FoodStack> foodStacks = new Dictionary<MachineType, FoodStack>();

    public EmployeeState employeeState;
    public RewardingType rewardingType;

    public RewardsBox reward;
    public int maxWeight = 3;

    public Stack<Garbage> garbageList = new Stack<Garbage>();
    public GameObject garbage;

    private EmployeeData employeeData;
    public EmployeeData EmployeeData { get { return employeeData; } set { employeeData = value; if (ui != null) ui.UpdateLevel(employeeData.level); } }

    EmployeeLevelStruct LevelStruct;
    public EmployeeLevelStruct employeeLevel { get { return LevelStruct; } set { LevelStruct = value; if (ui != null) ui.UpdateLevel(LevelStruct.level); } }
    int exp;
    public int EXP
    {
        get { return exp; }
        set
        {
            if (value != 0)
            {
                int a = Mathf.Abs(exp - value);
                exp = value;
                if (ui != null) ui.UpdateEXP(a);
            }
            else
            {
                exp = 0;
                if (ui != null) ui.ClearEXP();
            }
        }
    }

    // 직원 고용
    float x = 0;
    float z = 0;
    float length = 0;
    float elapsedTime = 0;
    Vector3 startPoint;
    Vector3 endPoint;
    Vector3 dir2;
    Vector3 controlVector;

    Counter deliveryCounter;
    public bool debuging;
    int step = 0;

    Vector3 target = new Vector3();
    List<FoodStack> stacks = new List<FoodStack>();
    List<Counter> counterList = new List<Counter>();
    List<PackingTable> packingTables = new List<PackingTable>();
    List<Table> tableList = new List<Table>();
    List<FoodMachine> finalMachineList = new List<FoodMachine>();
    //List<FoodStack> foodStacks = new List<FoodStack>();
    List<FoodStack> newFoodStacks = new List<FoodStack>();
    List<FoodMachine> foodMachineList = new List<FoodMachine>();
    List<TrashCan> trashCanList = new List<TrashCan>();

    //private IEnumerator pooledCoroutine;

    List<Node> nodes = new List<Node>();

    bool bStart = false;
    int moveStart = 0;

    bool animalMove = false;
    float actionTimer = 0;
    Node moveNode;
    bool doOnce = false;
    Table tb;
    TrashCan tc;
    bool gettingRewards;
    Vector3 lastPos;
    [NonSerialized]
    public bool falling = false;


    private void Awake()
    {
        
        // pooledCoroutine = EmployeeWait();
        //openCoords.Capacity = 200;
        //closedCoords.Capacity = 200;
        //  foodStacks.Capacity = 2;
        stacks.Capacity = 10;
        counterList.Capacity = 2;
        packingTables.Capacity = 2;
        tableList.Capacity = 10;
        finalMachineList.Capacity = 8;
        newFoodStacks.Capacity = 10;
        foodMachineList.Capacity = 8;
        trashCanList.Capacity = 1;
        nodes.Capacity = 100;
    }

    private void Start()
    {
        foodStacks[MachineType.BurgerMachine] = new FoodStack(MachineType.BurgerMachine, 0, false, 0);
        foodStacks[MachineType.CokeMachine] = new FoodStack(MachineType.CokeMachine, 0, false, 0);
        foodStacks[MachineType.CoffeeMachine] = new FoodStack(MachineType.CoffeeMachine, 0, false, 0);
        foodStacks[MachineType.DonutMachine] = new FoodStack(MachineType.DonutMachine, 0, false, 0);
        foodStacks[MachineType.PackingTable] = new FoodStack(MachineType.PackingTable, 0, true, 0);
    }
  
    //  public GameObject testObject;
    Vector3 prePosition = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        /*if (prePosition != trans.position)
        {
            if (headPoint != null)
            {
                //Debug.Log("WW");
                for (int i = 0; i < foodStacks.Count; i++)
                {
                    for (int j = 0; j < foodStacks[i].foodStack.Count; j++)
                    {
                        foodStacks[i].foodStack[j].transforms.position = headPoint.position + GameInstance.GetVector3(0, (j) * 0.7f, 0);
                    }
                }
                for (int i = 0; i < garbageList.Count; i++)
                {
                    garbageList[i].transforms.position = headPoint.position + GameInstance.GetVector3(0, 0.5f * i, 0);
                }
            }
        }
        else
        {
            prePosition = trans.position;
        }*/

        /*   if (!falling)
           {
               if (!animalMove)
               {
                   reCalculate = false;
                   //  Debug.Log(employeeActions);
                   switch (employeeActions)
                   {
                       case EmployeeActions.NONE:
                           break;

                       case EmployeeActions.ProcessWait:
                           Wait();
                           break;
                       case EmployeeActions.EmployeeWait:
                           if (!bStart)
                           {

                               EmployeeWait_Start();
                           }
                           else EmployeeWait_Update();
                           break;
                       case EmployeeActions.EmployeePatrol:
                           if (!bStart)
                           {
                               EmployeePatrol_Start();

                           }
                           else EmployeePatrol_Update();
                           break;
                       case EmployeeActions.EmployeeFoodMachine:
                           if (!bStart)
                           {
                               EmployeeFoodMachine_Start(moveNode, fm);
                           }
                           else if (!doOnce)
                           {
                               EmployeeFoodMachine_Rotate(moveNode, fm);
                           }
                           else
                           {
                               EmployeeFoodMachine_Update(moveNode, fm);
                           }
                           break;
                       case EmployeeActions.EmployeeCounter:
                           if (!bStart)
                           {
                               EmployeeCounter_Start(moveNode, ct);
                           }
                           else if (!doOnce)
                           {

                               EmployeeCounter_Rotate(moveNode, ct);
                           }
                           else
                           {

                               EmployeeCounter_Update(moveNode, ct);
                           }
                           break;
                       case EmployeeActions.EmployeeServing:
                           if (!bStart)
                           {
                               EmployeeServing_Start(moveNode, ct);
                           }
                           else if (!doOnce)
                           {
                               EmployeeServing_Rotate(moveNode, ct);

                           }
                           else if (!wait)
                           {
                               EmployeeServing_Update(moveNode, ct);
                           }
                           else
                           {
                               WaitTimer(1, ct);
                           }
                           break;
                       case EmployeeActions.EmployeeTable:
                           if (!bStart) EmployeeTable_Start(moveNode, tb);
                           else if (!doOnce) EmployeeTable_Rotate(moveNode, tb);
                           else EmployeeTable_Update(moveNode, tb);
                           break;
                       case EmployeeActions.EmployeeTrashcan:
                           if (!bStart) EmployeeTrashCan_Start(moveNode, tc);
                           else if (!doOnce) EmployeeTrashCan_Rotate(moveNode, tc);
                           else EmployeeTrashCan_Update(moveNode, tc);
                           break;
                       case EmployeeActions.EmployeeGetRewards:
                           if (!bStart) Reward_Start(moveNode);
                           else if (!doOnce) Reward_Rotate(moveNode);
                           else Reward_Update(moveNode);
                           break;
                   }
               }
           }*/
    }
    private void FixedUpdate()
    {
        /*        if (!knockback)
                {
                    if (reCalculate)
                    {
                        bStart = false;
                        doOnce = false;
                        animalMove = false;
                        moveStart = 0;

                        moveNode = CalculateNodes(lastPos);
                        reCalculate = false;
                    }
                    else
                    {
                        if (animalMove)
                        {
                            if (moveStart == 0)
                            {
                                AnimalMove_Start(moveNode, tb, gettingRewards);
                            }
                            else if (moveStart == 1)
                            {
                                AnimalMove_Update(moveNode, tb, gettingRewards);
                            }
                            else if (moveStart == 2)
                            {
                                AnimalMove_Update2(moveNode, tb, gettingRewards);
                            }
                        }
                    }
                }
                else
                {
                    if (knockbackCO == null) knockbackCO = StartCoroutine(Knockback());
                }*/
    }

    //WaitForFixedUpdate fix = new WaitForFixedUpdate();
   
    public void StartFalling(bool onlyFirst)
    {
        if (onlyFirst)
        {
            if (!CalculateFallingStrength()) return;
        }
        Falling(App.GlobalToken).Forget();
    }

    bool CalculateFallingStrength()
    {
        falling = true;
        elapsedTime = 0f;

        x = 0;
        z = 0;
        length = 0;

        /* Vector3 loc = GameIns.inputManager.cameraRange.position;
         int playerX = (int)((loc.x - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
         int playerY = (int)((loc.z - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);
         while(true)
         {
             int xx = (int)UnityEngine.Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
             int zz = (int)UnityEngine.Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
             length = (int)UnityEngine.Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
             if (Utility.ValidCheck(playerY + zz, playerX + xx))
             {
                 if (!MoveCalculator.GetBlocks[playerY + zz, playerX + xx])
                 {
                     //   float r = GameInstance.GameIns.calculatorScale.minY + node.r * GameInstance.GameIns.calculatorScale.distanceSize;
                     //   float c = GameInstance.GameIns.calculatorScale.minX + node.c * GameInstance.GameIns.calculatorScale.distanceSize;
                     int tx = (int)(GameIns.calculatorScale.minX + (playerX + xx) * GameIns.calculatorScale.distanceSize);
                     int ty = (int)(GameIns.calculatorScale.minY + (playerY + zz) * GameIns.calculatorScale.distanceSize);
                     startPoint = trans.position; //start.position;

                     endPoint = new Vector3(loc.x, 0, loc.z);
                     dir2 = new Vector3(xx, 0, zz).normalized;

                     endPoint += dir2 * length;
                     if(Utility.ValidCheck((int)endPoint.z, (int)endPoint.x) && !MoveCalculator.GetBlocks[(int)endPoint.z, (int)endPoint.x])
                     {

                         controlVector = (startPoint + endPoint) / RestaurantMgr.weight + Vector3.up * RestaurantMgr.height;

                         RestaurantMgr.flyingEndPoints.Add(endPoint);
                     }
                     return true;
                 }

             }

         }*/

        int num = InputManger.spawnDetects.Count;

        if (num == 0) return false;
        else
        {
            int rand = UnityEngine.Random.Range(0, num);
            Vector3 selectedPosition = InputManger.spawnDetects[rand];
            startPoint = trans.position;
            Transform t = GameInstance.GameIns.inputManager.cameraRange;

            endPoint = selectedPosition;// GameInstance.GetVector3(t.position.x, 0, t.position.z);
            dir2 = (endPoint - startPoint).normalized;
            //endPoint += dir2 * dirs.magnitude;
            float distance = Vector3.Distance(startPoint, endPoint);
            controlVector = (startPoint + endPoint) / RestaurantMgr.weight + Vector3.up * RestaurantMgr.height;
            return true;
        }

        /*  if (Physics.CheckSphere(GameInstance.GameIns.inputManager.cameraRange.position, Camera.main.orthographicSize / 4f, 1))
          {
              while (true)
              {
                  x = UnityEngine.Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
                  z = UnityEngine.Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
                  length = UnityEngine.Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
                  //  length = Random.Range(-10,10);
                  Vector3 test = GameInstance.GetVector3(GameInstance.GameIns.inputManager.cameraRange.position.x, 0, GameInstance.GameIns.inputManager.cameraRange.position.z);
                  Vector3 n = GameInstance.GetVector3(x, 0, z).normalized;
                  Vector3 ta = test + n * length;

                  if (Physics.Raycast(ta + Vector3.up * 5, Vector3.down, 5))
                  {
                      //   Debug.DrawLine(ta + Vector3.up * 10, test + Vector3.down * 100f, Color.red, 100);

                      if (Physics.CheckBox(ta, GameInstance.GetVector3(0.6f, 0.6f, 0.6f), Quaternion.identity, 1 << 6 | 1 << 7 | 1 << 8))
                      {

                      }
                      else break;


                  }
              }
              startPoint = trans.position; //start.position;

              endPoint = GameInstance.GetVector3(GameInstance.GameIns.inputManager.cameraRange.position.x, 0, GameInstance.GameIns.inputManager.cameraRange.position.z);
              dir2 = new Vector3(x, 0, z).normalized;

              endPoint += dir2 * length;
              controlVector = (startPoint + endPoint) / RestaurantMgr.weight + Vector3.up * RestaurantMgr.height;

              RestaurantMgr.flyingEndPoints.Add(endPoint);
              return true;
          }
          return false;*/
    }
    // busy = false;
    //Debug.DrawLine(trans.position, targetLoc, Color.red, 5f);
    async UniTask Falling(CancellationToken cancellationToken = default)
    {
        PlayAnim(animal.animationDic["Fly"], "Fly");
        animator.SetInteger("state", 3); //보이지 않는 내부 모델의 애니메이션
                                         //controlVector = (startPoint + endPoint) / RestaurantMgr.weight + Vector3.up * RestaurantMgr.height;
        
        while (true)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / RestaurantMgr.duration);

            Vector3 origin = trans.position;
            Vector3 targetLoc = CalculateBezierPoint(t, startPoint, controlVector, endPoint); //배지어 곡선의 t에 해당하는 위치 계산
//  Debug.DrawLine(trans.position, targetLoc, Color.red, 5f);
            Vector3 dir = targetLoc - origin;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;     //회전 각도 계산
            modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            trans.position = targetLoc;
            int posX = (int)((endPoint.x - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
            int posZ = (int)((endPoint.z - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);

            if (MoveCalculator.GetBlocks[posZ, posX]) FindSafetyZone(endPoint);
            
            await UniTask.NextFrame(cancellationToken: cancellationToken);
            if (t >= 1.0f)
            {
                trans.position = endPoint;
                break;
            }
        }
        await UniTask.NextFrame(cancellationToken: cancellationToken);
        PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
        animator.SetInteger("state", 0);
        falling = false;    // 낙하 종료
        GameIns.animalManager.AttachEmployeeTask(this);
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * p0; // 시작점
        point += 2 * u * t * p1; // 제어점
        point += tt * p2; // 끝점

        return point;
    }

    void FindSafetyZone(Vector3 endPosition)
    {
        int posX = (int)((endPosition.x - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
        int posZ = (int)((endPosition.z - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);

        int arrZ = MoveCalculator.GetBlocks.GetLength(0);
        int arrX = MoveCalculator.GetBlocks.GetLength(1);
        int finalX = posX;
        int finalZ = posZ;
        bool[,] visit = new bool[arrZ, arrX];
        int[] dirX = new int[4] { 0, 1, 0, -1 };
        int[] dirZ = new int[4] { 1, 0, -1, 0 };
        Queue<(int x, int z)> q = new();
        q.Enqueue((posX, posZ));

        while(q.Count > 0)
        {
            var(currentX, currentZ) = q.Dequeue();
            finalX = currentX;
            finalZ = currentZ;
            if (!MoveCalculator.GetBlocks[finalZ, finalX]) break;

            for(int i = 0; i<4; i++)
            {
                int nextX = currentX + dirX[i];
                int nextZ = currentZ + dirZ[i];
                if(Utility.ValidCheck(nextZ,nextX) && !visit[nextZ, nextX])
                {
                    visit[nextZ, nextX] = true;
                    q.Enqueue((nextX, nextZ));
                }
            }
        }

        float worldX = GameIns.calculatorScale.minX + finalX * GameIns.calculatorScale.distanceSize;
        float worldZ = GameIns.calculatorScale.minY + finalZ * GameIns.calculatorScale.distanceSize;
        Vector3 target = new Vector3(worldX, 0, worldZ);

        endPoint = target;
        float distance = Vector3.Distance(startPoint, endPoint);
        controlVector = (startPoint + endPoint) / RestaurantMgr.weight + Vector3.up * RestaurantMgr.height;
    }

    public void FindEmployeeWorks()
    {
        WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;

        if (employeeState != EmployeeState.TrashCan)
        {
            bool isPackaged = false;
            int foodNum = 0;
            MachineType type = MachineType.None;
            foreach (var stack in foodStacks)
            {
                foodNum += stack.Value.foodStack.Count;
                isPackaged = stack.Value.packaged;
                type = stack.Value.type;
                if (foodNum > 0) break;
            }
           
            if (foodNum > 0)
            {

                //if (employeeState == EmployeeState.FoodMachine)
                {
                    // if (!isPackaged)
                    {
                        //counterList  개수 거리 오름차순
                        int cn = workSpaceManager.counters.Count + workSpaceManager.packingTables.Count;
                        if (cn != counterList.Count)
                        {
                            counterList.Clear();
                            counterList.AddRange(workSpaceManager.counters);
                         //   counterList.AddRange(workSpaceManager.packingTables);
                        }
                        counterList = counterList.OrderBy(cl => (cl.transforms.position - trans.position).magnitude).ToList();
                        /*    for (int i = 0; i < counterList.Count - 1; i++)
                            {
                                int min = i;
                                for (int j = i + 1; j < counterList.Count; j++)
                                {
                                    float m1 = (counterList[j].transforms.position - trans.position).magnitude;
                                    float m2 = (counterList[min].transforms.position - trans.position).magnitude;
                                    if (m1 < m2)
                                    {
                                        min = j;
                                    }
                                }
                                if (min != i)
                                {
                                    Counter temp = counterList[i];
                                    counterList[i] = counterList[min];
                                    counterList[min] = temp;
                                }
                            }
                            for (int i = 0; i < counterList.Count - 1; i++)
                            {
                                int min = i;
                                for (int j = i + 1; j < counterList.Count; j++)
                                {
                                    int jj = int.MaxValue;
                                    int mm = int.MaxValue;
                                    for (int k = 0; k < counterList[j].foodStacks.Count; k++)
                                    {
                                        jj = counterList[j].foodStacks[k].foodStack.Count < jj ? counterList[j].foodStacks[k].foodStack.Count : jj;
                                    }
                                    for (int k = 0; k < counterList[min].foodStacks.Count; k++)
                                    {
                                        mm = counterList[min].foodStacks[k].foodStack.Count < mm ? counterList[min].foodStacks[k].foodStack.Count : mm;
                                    }

                                    if (jj < mm)
                                    {
                                        min = j;
                                    }
                                }
                                if (min != i)
                                {
                                    Counter temp = counterList[min];
                                    counterList[min] = counterList[i];
                                    counterList[i] = temp;
                                }
                            }*/


                        /*    for (int i = 0; i < counterList.Count; i++)
                            {
                                if (counterList[i].customer != null)
                                {
                                    for (int j = 0; j < foodStacks.Count; j++)
                                    {
                                        for (int k = 0; k < counterList[i].foodStacks.Count; k++)
                                        {
                                            MachineType targetType = counterList[i].foodStacks[k].type;
                                            if (targetType == type)
                                            {
                                                for (int l = 0; l < counterList[i].customer.foodStacks.Count; l++)
                                                {
                                                    if (counterList[i].customer.foodStacks[l].type == targetType && counterList[i].foodStacks[k].foodStack.Count + counterList[i].customer.foodStacks[l].foodStack.Count < counterList[i].customer.foodStacks[l].needFoodNum)
                                                    {
                                        //                Debug.Log("CounterType :" + counterList[i].customer.foodStacks[l].type + " " + counterList[i].foodStacks[k].type);
                                                        employeeState = EmployeeState.Counter;
                                                        target = counterList[i].workingSpot_SmallTables[k].transforms.position;

                                                        Work(target, counterList[i], false);

                                                        return;
                                                    }
                                                }

                                            }
                                        }
                                    }

                                }
                            }*/

                        // for (int a = 0; a < stacks.Count; a++)
                        {
                            //음식을 카운터로
                            /*       for (int j = 0; j < counterList.Count; j++)
                                   {
                                       for (int k = 0; k < counterList[j].foodStacks.Count; k++)
                                       {
                                           if (type == counterList[j].foodStacks[k].type)
                                           {
                                               employeeState = EmployeeState.Counter;
                                               target = counterList[j].workingSpot_SmallTables[k].transforms.position;

                                               Work(target, counterList[j], false);

                                               return;
                                           }
                                       }
                                   }*/
                        }
                        if (deliveryCounter)
                        {
                            for (int j = 0; j < counterList.Count; j++)
                            {
                                if (counterList[j] == deliveryCounter)
                                {
                                    for (int k = 0; k < counterList[j].foodStacks.Count; k++)
                                    {
                                        if (counterList[j].foodStacks[k].type == type && counterList[j].foodStacks[k].foodStack.Count + foodNum <= 20)
                                        {
                                            employeeState = EmployeeState.Counter;
                                            target = counterList[j].workingSpot_SmallTables[k].transforms.position;

                                            deliveryCounter = null;
                                            Work(target, counterList[j], false);

                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < counterList.Count; j++)
                            {
                                for (int k = 0; k < counterList[j].foodStacks.Count; k++)
                                {
                                    if (counterList[j].foodStacks[k].type == type && counterList[j].foodStacks[k].foodStack.Count + foodNum <= 20)
                                    {
                                        employeeState = EmployeeState.Counter;
                                        target = counterList[j].workingSpot_SmallTables[k].transforms.position;

                                        Work(target, counterList[j], false);

                                        return;
                                    }
                                }
                            }
                        }



                        //기다린다
                        Debug.Log("FailWait");
                        employeeState = EmployeeState.Wait;
                        Work();
                        return;
                    }
                }
            }
            else
            {
                if (reward != null)
                {

                    if (reward.foods.Count > 0)
                    {
                        //  restartCoroutine += () => { Reward(reward.transform.position); };
                        Reward(reward.transforms.position);
                        return;
                    }
                }
                packingTables = workSpaceManager.packingTables.ToList();
                //tableList 거리 오름차순
                tableList = workSpaceManager.tables.ToList();
                for (int i = 0; i < tableList.Count - 1; i++)
                {
                    int min = i;
                    for (int j = i + 1; j < tableList.Count; j++)
                    {
                        float m1 = (tableList[j].transforms.position - trans.position).magnitude;
                        float m2 = (tableList[min].transforms.position - trans.position).magnitude;
                        if (m1 < m2)
                        {
                            min = j;
                        }
                    }
                    if (min != i)
                    {
                        Table temp = tableList[i];
                        tableList[i] = tableList[min];
                        tableList[min] = temp;
                    }
                }

                //counterList  거리 오름차순
                /*  int cn = workSpaceManager.counters.Count + workSpaceManager.packingTables.Count;
                  if (cn != counterList.Count)
                  {
                      counterList.Clear();
                      counterList.AddRange(workSpaceManager.counters);
                      counterList.AddRange(workSpaceManager.packingTables);
                  }*/
                /*    for (int i = 0; i < counterList.Count - 1; i++)
                    {
                        int min = i;
                        for(int j= i+1; j< counterList.Count; j++)
                        {
                            float m1 = (counterList[j].transforms.position - trans.position).magnitude;
                            float m2 = (counterList[min].transforms.position - trans.position).magnitude;
                            if (m1 < m2)
                            {
                                min = j;
                            }
                        }
                        if(min != i)
                        {
                            Counter temp = counterList[i];
                            counterList[i] = counterList[min];
                            counterList[min] = temp;
                        }
                    }*/
                counterList.Clear();
                counterList.AddRange(workSpaceManager.counters);
                counterList.AddRange(workSpaceManager.packingTables);
                counterList = counterList
      .OrderBy(cl => (cl.transforms.position - trans.position).magnitude)
      .ToList();
            
                //foodMachine  거리 오름차순 개수 내림차순
                foodMachineList = workSpaceManager.foodMachines.ToList();
                foodMachineList = foodMachineList
    .OrderByDescending(fm => fm.foodStack.foodStack.Count) // 음식 개수 내림차순
    .ThenBy(fm => (fm.transforms.position - trans.position).magnitude) // 거리 오름차순
    .ToList();

       


                //stack 수량 오름차순
                stacks = GameInstance.GameIns.restaurantManager.foodStacks;
                for (int i = 0; i < stacks.Count - 1; i++)
                {
                    int min = i;
                    for (int j = i + 1; j < stacks.Count; j++)
                    {
                        int m1 = stacks[j].foodStack.Count;
                        int m2 = stacks[min].foodStack.Count;
                        if (m1 < m2)
                        {
                            min = j;
                        }
                        else if (m1 == m2)
                        {
                            float mm1 = 0;
                            float mm2 = 0;
                            for (int k = 0; k < foodMachineList.Count; k++)
                            {
                                if (foodMachineList[k].machineType == stacks[j].type)
                                {
                                    mm1 = (foodMachineList[k].transforms.position - trans.position).magnitude;

                                }
                                if (foodMachineList[k].machineType == stacks[min].type)
                                {

                                    mm2 = (foodMachineList[k].transforms.position - trans.position).magnitude;
                                }
                            }

                            if (mm1 < mm2)
                            {
                                min = j;
                            }
                        }
                    }
                    if (min != i)
                    {
                        FoodStack temp = stacks[i];
                        stacks[i] = stacks[min];
                        stacks[min] = temp;
                    }
                }

                //음식을 서빙
                for (int i = 0; i < counterList.Count; i++)
                {
                    if (counterList[i].customer && (counterList[i].employee == null || counterList[i].employee == this) && counterList[i].counterType != Counter.CounterType.Delivery)
                    {
                        int tmp = 0;
                        for (int j = 0; j < counterList[i].customer.foodStacks.Count; j++)
                        {
                            for (int k = 0; k < counterList[i].foodStacks.Count; k++)
                            {
                                if (counterList[i].customer.foodStacks[j].needFoodNum == 0) continue;
                                if (counterList[i].customer.foodStacks[j].type != counterList[i].foodStacks[k].type) continue;
                                if (counterList[i].customer.foodStacks[j].foodStack.Count < counterList[i].customer.foodStacks[j].needFoodNum && counterList[i].foodStacks[k].foodStack.Count + counterList[i].customer.foodStacks[j].foodStack.Count >= counterList[i].customer.foodStacks[j].needFoodNum)
                                {
                                    tmp++;
                                }
                                else
                                {
                                    tmp--;
                                }
                            }
                        }

                        if (tmp > 0)
                        {
                            counterList[i].employee = this;
                            target = counterList[i].workingSpot.position;
                            Work(target, counterList[i], true);
                            return;
                        }
                    }
                }



                //배달
                for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
                {
                    int d = i;
                    if (workSpaceManager.packingTables[i].counterType == Counter.CounterType.Delivery && (workSpaceManager.packingTables[i].employee == null || workSpaceManager.packingTables[i].employee == this) && workSpaceManager.packingTables[i].customer != null)
                    {
                        if (workSpaceManager.packingTables[i].packageStack.foodStack.Count >= workSpaceManager.packingTables[i].customer.foodStacks[0].needFoodNum)
                        {
                            workSpaceManager.packingTables[i].employee = this;
                            // restartCoroutine += () => { Work(workSpaceManager.packingTables[d].workingSpot.position, workSpaceManager.packingTables[d], 3); };
                            Work(workSpaceManager.packingTables[i].workingSpot.position, workSpaceManager.packingTables[i]);
                            return;
                        }
                    }
                }

                //테이블을 청소


                for (int i = 0; i < tableList.Count; i++)
                {
                    if (tableList[i].isDirty && !tableList[i].interacting && (tableList[i].employeeContoller == null || tableList[i].employeeContoller == this))
                    {
                        tableList[i].employeeContoller = this;
                        employeeState = EmployeeState.Table;
                        target = tableList[i].cleanSeat.position;

                        Work(target, tableList[i]);
                        return;
                    }
                }


                /*     tableList.Clear();
                     for (int i = 0; i < workSpaceManager.tables.Count; i++) tableList.Add(workSpaceManager.tables[i]);
                     tableList.Sort(delegate (Table a, Table b) { return (a.gameObject.transform.position - trans.position).magnitude.CompareTo((b.gameObject.transform.position - trans.position).magnitude); });

                     if (tableList.Count > 0)
                     {
                         foreach (Table table in tableList)
                         {
                             if (table.isDirty && !table.interacting && (table.employeeContoller == null || table.employeeContoller == this))
                             {
                                 table.employeeContoller = this;
                                 employeeState = EmployeeState.Table;
                                 target = table.cleanSeat.position;
                             //    restartCoroutine += () => { Work(target, table); };
                                 Work(target, table);
                                 return;
                             }
                         }
                     }*/

                // 손님 우선 조달
                for (int i = 0; i < counterList.Count; i++)
                {
                    if (counterList[i].customer && counterList[i].counterType != Counter.CounterType.Delivery)
                    {
                        for (int j = 0; j < counterList[i].customer.foodStacks.Count; j++)
                        {

                            Customer customer = counterList[i].customer;
                            for (int k = 0; k < foodMachineList.Count; k++)
                            {
                                //  int tmp = 0;
                                for (int l = 0; l < counterList[i].foodStacks.Count; l++)
                                {
                                    int tmp = 20 - counterList[i].foodStacks[l].foodStack.Count;
                                    tmp = tmp > employeeLevel.max_weight ? employeeLevel.max_weight : tmp;
                                    if (counterList[i].customer.foodStacks[j].type != foodMachineList[k].machineType) continue;
                                    /*if (foodMachineList[k].foodStack.foodStack.Count + counterList[i].foodStacks[l].foodStack.Count + customer.foodStacks[j].foodStack.Count >= customer.foodStacks[j].needFoodNum &&
                                   (foodMachineList[k].employee == null || foodMachineList[k].employee == this) && counterList[i].foodStacks[l].type == counterList[i].customer.foodStacks[j].type &&
                                   customer.foodStacks[j].needFoodNum > customer.foodStacks[j].foodStack.Count + counterList[i].foodStacks[l].foodStack.Count)*/
                                    if ((foodMachineList[k].employee == null || foodMachineList[k].employee == this) && counterList[i].foodStacks[l].type == counterList[i].customer.foodStacks[j].type &&
                                   customer.foodStacks[j].needFoodNum > customer.foodStacks[j].foodStack.Count + counterList[i].foodStacks[l].foodStack.Count && foodMachineList[k].foodStack.foodStack.Count > 0)
                                    {
                                        foodMachineList[k].employee = this;
                                        foodMachineList[k].getNum = tmp;
                                        employeeState = EmployeeState.FoodMachine;
                                        target = foodMachineList[k].workingSpot.position;
                                        deliveryCounter = counterList[i];
                                        //  counterList[i].foodStacks[l].getNum = tmp;
                                        Work(target, foodMachineList[k]);
                                        return;
                                    }
                                    /*  else if(foodMachineList[k].foodStack.foodStack.Count + counterList[i].foodStacks[l].foodStack.Count + customer.foodStacks[j].foodStack.Count < customer.foodStacks[j].needFoodNum &&
                                     (foodMachineList[k].employee == null || foodMachineList[k].employee == this) && counterList[i].foodStacks[l].type == counterList[i].customer.foodStacks[j].type &&
                                     customer.foodStacks[j].needFoodNum > customer.foodStacks[j].foodStack.Count + counterList[i].foodStacks[l].foodStack.Count)
                                      {
                                          tmp--;
                                          goto ExitLoops;
                                      }*/
                                }
                                /*
                                        if (tmp > 0)
                                        {
                                            foodMachineList[k].employee = this;
                                            employeeState = EmployeeState.FoodMachine;
                                            target = foodMachineList[k].workingSpot.position;
                                            deliveryCounter = counterList[i];
                                            Work(target, foodMachineList[k]);
                                        }
                                        return;*/
                            }

                            //  ExitLoops:
                            //       break;
                        }

                    }
                }






                //포장된 음식을 배달 테이블로 이동
                for (int i = 0; i < packingTables.Count; i++)
                {
                    if (packingTables[i].counterType == Counter.CounterType.None && packingTables[i].packageStack.foodStack.Count > 0 && (packingTables[i].employeeAssistant == null || packingTables[i].employeeAssistant == this))
                    {
                        for (int j = 0; j < packingTables.Count; j++)
                        {
                            if (i != j && packingTables[j].counterType == Counter.CounterType.Delivery && packingTables[j].employee == null)
                            {
                                packingTables[i].employeeAssistant = this;
                                packingTables[j].employee = this;
                                employeeState = EmployeeState.FoodMachine;
                                target = packingTables[i].endTrans.position;
                                //    restartCoroutine += () => { Work(target, workSpaceManager.packingTables[i], 2); };
                                Work(target, packingTables[i], packingTables[j].workingSpot.position, packingTables[j]);
                                return;
                            }
                        }
                    }
                }



                //포장테이블 포장


                for (int i = 0; i < packingTables.Count; i++)
                {
                    if ((packingTables[i].employee == null || packingTables[i].employee == this) && packingTables[i].counterType == Counter.CounterType.None)
                    {
                        int n = 0;
                        for (int j = 0; j < packingTables[i].foodStacks.Count; j++)
                        {
                            n += packingTables[i].foodStacks[j].foodStack.Count;
                        }
                     //   n += packingTables[i].packingNumber;
                        //   int tmp = workSpaceManager.packingTables[i].packageStack.foodStack.Count + 1;

                        if (n >= 4)
                        {
                            packingTables[i].employee = this;
                            employeeState = EmployeeState.FoodMachine;
                            target = packingTables[i].packingTrans.position;
                            //         restartCoroutine += () => { Work(target, workSpaceManager.packingTables[i], 1); };
                            Work(target, packingTables[i], 1);

                            return;

                        }

                    }
                }



                List<FoodStack> fStacks = new List<FoodStack>();
                for (int i = 0; i < counterList.Count; i++)
                {
                    for (int j = 0; j < counterList[i].foodStacks.Count; j++)
                    {
                        fStacks.Add(counterList[i].foodStacks[j]);
                    }
                }

                fStacks.Sort((a, b) => (a.foodStack.Count + a.getNum).CompareTo(b.foodStack.Count + b.getNum));

                //일반 조달
                for (int i = 0; i < fStacks.Count; i++)
                {
                    for (int j = 0; j < foodMachineList.Count; j++)
                    {
                        if (fStacks[i].type != MachineType.PackingTable && fStacks[i].type == foodMachineList[j].machineType &&
                          foodMachineList[j].foodStack.foodStack.Count > 0 && (foodMachineList[j].employee == null || foodMachineList[j].employee == this))
                        {
                            int tmp = 20 - fStacks[i].foodStack.Count;
                            for (int k = 0; k < counterList.Count; k++)
                            {
                                for (int l = 0; l < counterList[k].foodStacks.Count; l++)
                                {
                                    tmp = 20 - fStacks[i].foodStack.Count;
                                    tmp = tmp > employeeLevel.max_weight ? employeeLevel.max_weight : tmp;
                                    if (fStacks[i].id == counterList[k].foodStacks[l].id && tmp > 0)
                                    {
                                        fStacks[i].getNum = tmp;
                                        deliveryCounter = counterList[k];
                                        foodMachineList[j].employee = this;
                                        employeeState = EmployeeState.FoodMachine;
                                        target = foodMachineList[j].workingSpot.position;
                                        foodMachineList[j].getNum = tmp;
                                        Work(target, foodMachineList[j]);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }


                /* for (int i=0; i<stacks.Count; i++)
                 {
                     for (int j=0; j<foodMachineList.Count; j++)
                     {
                         if (stacks[i].id <=4 && stacks[i].type == foodMachineList[j].machineType && 
                             foodMachineList[j].foodStack.foodStack.Count > 0 && (foodMachineList[j].employee == null || foodMachineList[j].employee == this))
                         {
                             for (int k=0; k<counterList.Count; k++)
                             {
                                 for(int l =0; l < counterList[k].foodStacks.Count; l++)
                                 {
                                     if (counterList[k].foodStacks[l].type == foodMachineList[j].machineType)
                                     {
                                         foodMachineList[j].employee = this;
                                         employeeState = EmployeeState.FoodMachine;
                                         target = foodMachineList[j].workingSpot.position;

                                         Work(target, foodMachineList[j]);
                                         return;
                                     }
                                 }
                             }

                         }
                     }
                 }*/


                //   Debug.Log("AAA");
                //기다리기
                employeeState = EmployeeState.Wait;

                //   restartCoroutine += () => { Work(); };
                Work();
            }
        }
        else
        {
            //쓰레기를 버림
            // List<TrashCan> trashCanList = new List<TrashCan>();
            /* trashCanList.Clear();
             for (int i = 0; i < workSpaceManager.trashCans.Count; i++) trashCanList.Add(workSpaceManager.trashCans[i]);
             trashCanList.Sort(delegate (TrashCan a, TrashCan b) { return (a.gameObject.transform.position - trans.position).magnitude.CompareTo((b.gameObject.transform.position - trans.position).magnitude); });

             foreach (TrashCan c in trashCanList)
             {
                // restartCoroutine += () => { Work(c.throwPos.position, c); };
                 Work(c.throwPos.position, c);
                 return;
             }*/
            trashCanList = workSpaceManager.trashCans;
            for (int i = 0; i < trashCanList.Count - 1; i++)
            {
                int min = i;
                for (int j = i + 1; j < trashCanList.Count - 1; j++)
                {
                    float m1 = (trashCanList[j].transforms.position - trans.position).magnitude;
                    float m2 = (trashCanList[min].transforms.position - trans.position).magnitude;
                    if (m1 < m2)
                    {
                        min = j;
                    }
                }
                if (min != i)
                {
                    TrashCan temp = trashCanList[min];
                    trashCanList[min] = trashCanList[i];
                    trashCanList[i] = temp;
                }
            }


            //  for(int i=0; i<trashCanList.Count; i++)
            {
                Work(trashCanList[0].throwPos.position, trashCanList[0]);
                return;
            }
        }
    }
    // = EmployeeWait;
    Coroutine waitC;

    public void Work()
    {
        int r = UnityEngine.Random.Range(2, 3);

        Employee_Wait(r, App.GlobalToken).Forget();

    }
    static Node tempDeliveryFoodsNode = new Node(1, 1);
    public void Work(Vector3 position, PackingTable packingTable, int i)
    {
        Employee_Packing(position, packingTable, i, App.GlobalToken).Forget();


    }
    public void Work(Vector3 position, PackingTable packingTable, Vector3 targetPos, PackingTable targetTable)
    {
        Employee_PackMove(position, packingTable, targetPos, targetTable, App.GlobalToken).Forget();
    }
    public void Work(Vector3 position, PackingTable packingTable)
    {
        Employee_Delivery(position, packingTable, App.GlobalToken).Forget();
    }

    public void Work(Vector3 position, FoodMachine foodMachine)
    {
        Employee_FoodMachine(position, foodMachine, App.GlobalToken).Forget();

    }

    public void Work(Vector3 position, Counter counter, bool serving)
    {
        if (!serving) Employee_Counter(position, counter, App.GlobalToken).Forget();
        else Employee_Serving(position, counter, App.GlobalToken).Forget();
    }

    public void Work(Vector3 position, Table table)
    {
        Employee_Table(position, table, App.GlobalToken).Forget();
    }

    public void Work(Vector3 position, TrashCan trash)
    {
        Employee_Trashcan(position, trash, App.GlobalToken).Forget();
    }

    static WaitForSeconds zerodotfive = new WaitForSeconds(0.5f);
    float coroutineTimer2 = 0;
    public int success;
    public Coroutine employeeCoroutine;

    void Wait()
    {
        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
        }
        else
        {
            waitTimer = 0f;
            employeeActions = EmployeeActions.NONE;
            GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
        }
    }

    async UniTask Employee_Wait(int r, CancellationToken cancellationToken = default)
    {

        employeeState = EmployeeState.Wait;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            r = 2;
            if (r == 2)
            {
                animator.SetInteger("state", 2);
                PlayAnim(animal.animationDic[animation_LookAround], animation_LookAround);
                await UniTask.Delay(500, cancellationToken: cancellationToken);
            }
            else
            {
                await Employee_Patrol(cancellationToken);
            }

            await UniTask.SwitchToMainThread(cancellationToken);
            GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Employee_Wait task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Employee_Wait: {ex.Message}");
            throw;
        }

    }


    async UniTask Employee_Patrol(CancellationToken cancellationToken = default)
    {
        try
        {
            while (true)
            {
                Vector3 target;
                float x = UnityEngine.Random.Range(-1f, 1f);
                float y = UnityEngine.Random.Range(-1f, 1f);
                Vector3 v3 = GameInstance.GetVector3(x, 0, y);
                float magnitude = v3.magnitude;
                if (magnitude > 0f)
                {
                    v3 = v3 / magnitude;
                }
                float speed = UnityEngine.Random.Range(employeeLevel.move_speed, employeeLevel.move_speed * 2);
                if (trans == null || !trans)
                {
                    await UniTask.NextFrame();
                    return;
                }
                target = trans.position + v3 * speed;

                bool interruptCheck = Physics.CheckBox(target, GameInstance.GetVector3(0.6f, 0.6f, 0.6f), Quaternion.identity, 1 << 6 | 1 << 7);
                bool validCheck = Physics.CheckBox(target, GameInstance.GetVector3(0.6f, 0.6f, 0.6f), Quaternion.identity, 1);

                if (validCheck && !interruptCheck)
                {
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                    moveNode = await CalculateNodes_Async(target, cancellationToken);
                    await UniTask.SwitchToMainThread(cancellationToken: cancellationToken);
                    if (moveNode != null)
                    {
                        if (moveNode.c == 100 && moveNode.r == 100)
                        {
                            moveNode = null;
                            animator.SetInteger("state", 2);
                            PlayAnim(animal.animationDic[animation_LookAround], animation_LookAround);
                        }
                        else
                        {
                            await Employee_Move(moveNode, cancellationToken);
                            if (reCalculate)
                            {
                                while(bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                                reCalculate = false;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        animator.SetInteger("state", 2);
                        PlayAnim(animal.animationDic[animation_LookAround], animation_LookAround);
                    }
                    await UniTask.Delay(500, cancellationToken: cancellationToken);
                    return;
                }
                //    return;
            }
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Employee_Patrol task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Employee_Patrol: {ex.Message}");
            throw;
        }
    }


    async UniTask Employee_FoodMachine(Vector3 position, FoodMachine foodMachine, CancellationToken cancellationToken = default)
    {
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                lastPos = position;
                moveNode = await CalculateNodes_Async(position, cancellationToken);
                if (moveNode != null)
                {
                    if (moveNode.r == 100 && moveNode.c == 100)
                    {
                        moveNode = null;
                        Debug.Log("100Find");
                    }
                    else
                    {
                        employeeActions = EmployeeActions.EmployeeFoodMachine;
                        await Employee_Move(moveNode, cancellationToken);

                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;

                            continue;
                        }

                        modelTrans.rotation = foodMachine.workingSpot.rotation;
                        animatedAnimal.transforms.rotation = foodMachine.workingSpot.rotation;
                    }
                    FoodStack foodStack = null;

                    if (foodMachine.foodStack.foodStack.Count > 0)
                    {
                        foodStack = foodStacks[foodMachine.machineType];
                        while (true)
                        {
                            int currentStackCount = foodStack.foodStack.Count; // 음식 개수 저장
                            if (currentStackCount >= employeeLevel.max_weight || foodMachine.foodStack.foodStack.Count == 0)
                            {
                                foodMachine.employee = null;
                                employeeActions = EmployeeActions.ProcessWait;
                                if (debuging) Debug.Log("직원 조리기구 종료");
                                await UniTask.Delay(500, cancellationToken: cancellationToken);
                                GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
                                return;
                            }
                            Food f = foodMachine.foodStack.foodStack.Pop();

                            if (debuging) Debug.Log("직원 조리기구에서 음식을 가져가는 중");

                            MachineType t = foodMachine.machineType;
                            // FoodStack fs = foodStacks[n];

                            Vector3 targetPosition = headPoint.position + GameInstance.GetVector3(0, 0.7f * (currentStackCount + 1), 0); // 목적지 저장
                            float r = UnityEngine.Random.Range(1, 2.5f);
#if HAS_DOTWEEN
                            DOTween.Kill(f.transforms); //Tween 제거
                            f.transforms.DOJump(targetPosition, r, 1, 0.2f).OnComplete(() =>
                              OnFoodStackComplete(f, targetPosition, foodStack, audioSource, foodStack.foodStack.Count, headPoint));
#endif
                            await UniTask.Delay(300, cancellationToken: cancellationToken);
                        }
                    }
                    //await UniTask.Delay(200);
                    //GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
                }
                else
                {
                    foodMachine.employee = null;
                    if (debuging) Debug.Log("Fail FoodMachine");
                    Work();
                }
                Debug.Log("Error");
                return;
            }
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Employee_FoodMachine task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Employee_FoodMachine: {ex.Message}");
            throw;
        }
    }


    async UniTask Employee_Counter(Vector3 position, Counter counter, CancellationToken cancellationToken = default)
    {
        try
        {
            while (true)
            {
                lastPos = position;
                cancellationToken.ThrowIfCancellationRequested();
                moveNode = await CalculateNodes_Async(position, cancellationToken);
                if (moveNode != null)
                {
                    if (moveNode.r == 100 && moveNode.c == 100)
                    {
                        // Work();
                        moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveNode, cancellationToken);
                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }
                        modelTrans.rotation = counter.workingSpot.rotation;
                        animatedAnimal.transforms.rotation = counter.workingSpot.rotation;
                    }
                    employeeActions = EmployeeActions.EmployeeCounter;


                    if (debuging) Debug.Log("직원 카운터에 음식을 내려놓는 중");


                    FoodStack foodStack = null;

                    for (int i = 0; i < counter.foodStacks.Count; i++)
                    {
                        MachineType t = counter.foodStacks[i].type;
                        foodStack = foodStacks[t];
                  
                        while (foodStack.foodStack.Count > 0)
                        {
                            int index = counter.foodStacks[i].foodStack.Count;
                            Food f = foodStack.foodStack.Pop();
                            f.Release();
                            float r = UnityEngine.Random.Range(1, 2.5f);
                            int currentStackCount = foodStacks[t].foodStack.Count; // 음식 개수 저장
                            Vector3 targetPosition = headPoint.position + GameInstance.GetVector3(0, 0.7f * (currentStackCount + 1), 0); // 목적지
#if HAS_DOTWEEN
                            DOTween.Kill(f.transforms); //Tween 제거
                            f.transforms.DOJump(counter.stackPoints[i].position + GameInstance.GetVector3(0, 0.7f * counter.foodStacks[i].foodStack.Count, 0), r, 1, 0.2f);
#endif
                            counter.foodStacks[i].foodStack.Push(f);
                            counter.foodStacks[i].getNum = counter.foodStacks[i].getNum - 1 < 0 ? 0 : counter.foodStacks[i].getNum - 1;

                            if (debuging) Debug.Log(counter.foodStacks[i].getNum);

                            await UniTask.Delay(300, cancellationToken: cancellationToken);
                        }
                    }
                    if (debuging) Debug.Log("직원 카운터 종료");

                    GameInstance.GameIns.animalManager.AttachEmployeeTask(this);

                }
                else
                {
                    if (debuging) Debug.Log("Error");
                    // Work();
                    await Employee_Wait(0, cancellationToken);
                }
                return;
            }
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Employee_Counter task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Employee_Counter: {ex.Message}");
            throw;
        }
    }

    async UniTask Employee_Serving(Vector3 position, Counter counter, CancellationToken cancellationToken = default)
    {
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                lastPos = position;
                moveNode = await CalculateNodes_Async(position, cancellationToken);
                if (moveNode != null)
                {
                    if (moveNode.r == 100 && moveNode.c == 100)
                    {
                        //   counter.employee = null;
                        //  Work();
                        moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveNode, cancellationToken);
                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }
                        modelTrans.rotation = counter.workingSpot.rotation;
                        animatedAnimal.transforms.rotation = counter.workingSpot.rotation;
                    }

                    employeeActions = EmployeeActions.EmployeeServing;

                    int index = 0;

                    for (int i = 0; i < counter.foodStacks.Count; i++)
                    {
                        MachineType t = counter.foodStacks[i].type;

                        if (counter.customer != null)
                        {
                            for (int j = 0; j < counter.customer.foodStacks.Count; j++)
                            {
                                FoodStack foodStack = counter.foodStacks[i];
                                if (t == counter.customer.foodStacks[j].type)
                                {
                                    index += counter.customer.foodStacks[j].foodStack.Count;
                                    while (counter.customer.foodStacks[j].needFoodNum > counter.customer.foodStacks[j].foodStack.Count && foodStack.foodStack.Count > 0)
                                    {
                                        Food f = foodStack.foodStack.Pop();
                                        float r = UnityEngine.Random.Range(1, 2.5f);
                                        int currentStackCount = foodStacks[t].foodStack.Count; // 음식 개수 저장
                                        Vector3 pos = counter.customer.headPoint.position + GameInstance.GetVector3(0, 0.7f * index, 0);
#if HAS_DOTWEEN
                                        DOTween.Kill(f.transforms);
                                        f.transforms.DOJump(pos, r, 1, 0.2f).OnComplete(() =>
                                        OnFoodStackComplete(f, pos, counter.customer.foodStacks[j], audioSource, index, counter.customer.headPoint));
#endif
                                        counter.customer.VisualizingFoodStack.Add(f);
                                        index++;
                                        await UniTask.Delay(300, cancellationToken: cancellationToken);
                                        if (debuging) Debug.Log("Find");
                                    }
                                }
                            }
                        }
                    }

                    await UniTask.Delay(500, cancellationToken: cancellationToken);
                    GameInstance.GameIns.animalManager.AttachEmployeeTask(this);

                }
                else
                {
                    counter.employee = null;
                    Work();
                }
                return;
            }
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Employee_Serving task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Employee_Serving: {ex.Message}");
            throw;
        }
    }

    async UniTask Employee_Table(Vector3 position, Table table, CancellationToken cancellationToken = default)
    {
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                lastPos = position;
                moveNode = await CalculateNodes_Async(position, cancellationToken);
                if (moveNode != null)
                {
                    if (moveNode.r == 100 && moveNode.c == 100)
                    {
                        moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveNode, cancellationToken);

                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;

                            if(table.interacting)
                            {
                                animator.SetInteger("state", 0);
                                PlayAnim(animal.animationDic[animation_Idle_A], animation_Idle_A);
                                await UniTask.Delay(500);
                                GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
                                return;
                            }

                            continue;
                        }

                        modelTrans.LookAt(table.transforms);
                        animatedAnimal.transforms.LookAt(table.transforms);
                    }

                    await UniTask.Delay(200, cancellationToken: cancellationToken);

                    while (table.numberOfGarbage > 0)
                    {
                        Garbage garbage = table.garbageList[table.numberOfGarbage - 1];
                        table.garbageList.RemoveAt(table.numberOfGarbage - 1);
                        table.numberOfGarbage--;
                        Vector3 pos = headPoint.position + GameInstance.GetVector3(0, 0.5f * garbageList.Count, 0);
                        float r = UnityEngine.Random.Range(1, 2.5f);
                        int index = garbageList.Count;
#if HAS_DOTWEEN
                        DOTween.Kill(garbage.transforms);
                        garbage.transforms.DOJump(pos, r, 1, 0.2f).OnComplete(() =>
                        OnGarbageStackComplete(garbage, pos, garbageList, audioSource, index, headPoint));
#endif
                        await UniTask.Delay(300, cancellationToken: cancellationToken);
                    }

                    table.isDirty = false;
                    tb = table;
                    employeeActions = EmployeeActions.EmployeeTable;
                    table.employeeContoller = null;
                    employeeState = EmployeeState.TrashCan;
                    await UniTask.Delay(500, cancellationToken: cancellationToken);
                    GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
                }
                else
                {
                    table.employeeContoller = null;
                    Work();
                }
                return;
            }
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Employee_Table task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Employee_Table: {ex.Message}");
            throw;
        }
    }

    async UniTask Employee_Trashcan(Vector3 position, TrashCan trash, CancellationToken cancellationToken = default)
    {
        try
        {
            while (true)
            {
                moveNode = await CalculateNodes_Async(position, cancellationToken);
                if (moveNode != null)
                {
                    if (moveNode.r == 100 && moveNode.c == 100)
                    {
                        moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveNode, cancellationToken);

                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }

                        modelTrans.LookAt(trash.transforms);
                        animatedAnimal.transforms.LookAt(trash.transforms);
                    }

                    await UniTask.Delay(200, cancellationToken: cancellationToken);
                    while (garbageList.Count > 0)
                    {
                        Garbage garbage = garbageList.Pop();
                        garbage.Release();
                        Vector3 pos = trash.transforms.position;
                        float r = UnityEngine.Random.Range(1, 2.5f);
                        int index = garbageList.Count;
#if HAS_DOTWEEN
                        DOTween.Kill(garbage.transforms);
                        garbage.transforms.DOJump(pos, r, 1, 0.2f).OnComplete(() =>
                        OnGarbageClearComplete(garbage, audioSource));
#endif
                        await UniTask.Delay(300, cancellationToken: cancellationToken);
                    }
                    await UniTask.Delay(500, cancellationToken: cancellationToken);
                    employeeState = EmployeeState.Wait;
                    GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
                }
                else
                {
                    Work();
                }
                return;
            }
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Employee_Trashcan task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Employee_Trashcan: {ex.Message}");
            throw;
        }
    }

    async UniTask Employee_Reward(Vector3 position, CancellationToken cancellationToken = default)
    {
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                moveNode = await CalculateNodes_Async(position, cancellationToken);

                if (moveNode != null)
                {
                    if (moveNode.r == 100 && moveNode.c == 100)
                    {
                        moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveNode, cancellationToken);

                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }

                        // modelTrans.LookAt(trash.transforms);
                        //animatedAnimal.transforms.LookAt(trash.transforms);
                    }

                    await UniTask.Delay(200, cancellationToken: cancellationToken);
                    while (reward.foods.Count > 0)
                    {
                        PlayAnim(animal.animationDic[animation_Peck], animation_Peck);
                        animator.SetInteger("state", 4);
                        await UniTask.Delay(500, cancellationToken: cancellationToken);
                        Food go = reward.foods.Pop();
                        reward.EatFish(go);
                        EXP += 20;
                        GameInstance.GameIns.restaurantManager.playerData.fishesNum_InBox--;

                        ui.GainExperience(); //경험치바 채워주기
                                             //yield return new WaitForSeconds(0.5f);
                        if (EXP >= 100)
                        {
                            EXP = 0;
                            GameInstance.GameIns.restaurantManager.UpgradePenguin(employeeLevel.level + 1, false, this); //펭귄의 레벨 업 및 능력치 변경하기
                        }
                    }
                    GameInstance.GameIns.applianceUIManager.ResetSchadule(reward);
                    reward = null;
                    employeeState = EmployeeState.Wait;
                    GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
                }
                return;
            }
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Employee_Reward task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Employee_Reward: {ex.Message}");
            throw;
        }
    }

    async UniTask Employee_Packing(Vector3 position, PackingTable packingTable, int index, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                moveNode = await CalculateNodes_Async(position, cancellationToken);
                if (moveNode != null)
                {
                    if (moveNode.r == 100 && moveNode.c == 100)
                    {
                        moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveNode, cancellationToken);
                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }
                        modelTrans.rotation = packingTable.workingSpot.rotation;
                        animatedAnimal.transforms.rotation = packingTable.workingSpot.rotation;
                    }

                    await UniTask.Delay(200, cancellationToken: cancellationToken);

                    while (true)
                    {
                        if (packingTable.packingNumber == 4)
                        {
                            packingTable.packingNumber = 0;

                            await UniTask.Delay(200, cancellationToken: cancellationToken);

                            PackageFood packageFood = packingTable.packageFood;


                            float r = UnityEngine.Random.Range(1, 2.5f);
#if HAS_DOTWEEN
                            packageFood.transforms.DOJump(packingTable.packageStackTrans.position, r, 1, 0.2f);
#endif
                            packingTable.packageStack.foodStack.Push(packageFood);
                            await UniTask.Delay(300, cancellationToken: cancellationToken);

                            packingTable.SpawnBox();
                        }

                        try
                        {
                            int sum = 0;
                            for (int i = 0; i < packingTable.foodStacks.Count; i++) sum += packingTable.foodStacks[i].foodStack.Count;

                            if (sum < 4) break;
                        }
                        catch (OperationCanceledException)
                        {
                            // 작업이 취소되었을 때의 정리 코드
                            Debug.Log("Employee_packing task was cancelled");
                            throw; // 필요에 따라 rethrow 또는 생략
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error in Employee_packing: {ex.Message}");
                            throw;
                        }

                        await UniTask.Delay(200, cancellationToken: cancellationToken);
                        //포장
                        while (true)
                        {
                            try
                            {
                                List<int> randomIndexes = Enumerable.Range(0, packingTable.foodStacks.Count).ToList();
                                randomIndexes = randomIndexes.OrderBy(i => UnityEngine.Random.value).ToList();
                                FoodStack fs = null;
                                int max = 0;
                                for (int i = 0; i < randomIndexes.Count; i++)
                                {
                                    if (max < packingTable.foodStacks[randomIndexes[i]].foodStack.Count)
                                    {
                                        max = packingTable.foodStacks[randomIndexes[i]].foodStack.Count;
                                        fs = packingTable.foodStacks[randomIndexes[i]];
                                    }
                                }
                                if (fs != null)
                                {
                                    Food f = fs.foodStack.Pop();
                                    float r = UnityEngine.Random.Range(1, 2.5f);
                                    Transform t = packingTable.packageFood.packageTrans[packingTable.packingNumber];
#if HAS_DOTWEEN
                                    f.DOKill();
                                    f.transforms.DOJump(packingTable.packageFood.packageTrans[packingTable.packingNumber].position, r, 1, 0.2f).OnComplete(() =>
                                    CompleteMove(f, t, t.position, packingTable));
#endif
                                    audioSource.Play();
                                    await UniTask.Delay(300, cancellationToken: cancellationToken);
                                    if (packingTable.packingNumber == 4) break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                // 작업이 취소되었을 때의 정리 코드
                                Debug.Log("Employee_packing task was cancelled");
                                throw; // 필요에 따라 rethrow 또는 생략
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError($"Error in Employee_packing: {ex.Message}");
                                throw;
                            }
                        }
                        await UniTask.Delay(300, cancellationToken: cancellationToken);
                        }

                    packingTable.employee = null;
                    await UniTask.Delay(500, cancellationToken: cancellationToken);
                    GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
                }
                else
                {
                    packingTable.employee = null;
                    Work();

                }
                return;
            }
            catch (OperationCanceledException)
            {
                // 작업이 취소되었을 때의 정리 코드
                Debug.Log("Employee_packing task was cancelled");
                throw; // 필요에 따라 rethrow 또는 생략
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in Employee_packing: {ex.Message}");
                throw;
            }

            
        }
  
    }

    async UniTask Employee_PackMove(Vector3 position, PackingTable packingTable, Vector3 targetPos, PackingTable targetTable, CancellationToken cancellationToken = default)
    {
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                moveNode = await CalculateNodes_Async(position, cancellationToken);

                if (moveNode != null)
                {
                    if (moveNode.r == 100 && moveNode.c == 100)
                    {
                        moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveNode, cancellationToken);

                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }
                        modelTrans.rotation = packingTable.endTrans.rotation;
                        animatedAnimal.transforms.rotation = packingTable.endTrans.rotation;
                    }

                    await UniTask.Delay(200, cancellationToken: cancellationToken);

                    while (packingTable.packageStack.foodStack.Count > 0)
                    {
                        int currentStackCount = foodStacks[MachineType.PackingTable].foodStack.Count;
                        PackageFood f = (PackageFood)packingTable.packageStack.foodStack.Pop();
                        Vector3 targetPosition = headPoint.position + GameInstance.GetVector3(0, 0.7f * (currentStackCount + 1), 0); // 목적지 저장
                        float r = UnityEngine.Random.Range(1, 2.5f);
#if HAS_DOTWEEN
                        DOTween.Kill(f.transforms); //Tween 제거
                        f.transforms.DOJump(targetPosition, r, 1, 0.2f).OnComplete(() =>
                          OnFoodStackComplete(f, targetPosition, foodStacks[MachineType.PackingTable], audioSource, foodStacks[MachineType.PackingTable].foodStack.Count, headPoint));
#endif
                        await UniTask.Delay(300, cancellationToken: cancellationToken);
                    }

                    await UniTask.Delay(500, cancellationToken: cancellationToken);
                    packingTable.employeeAssistant = null;
                    //카운터로 배달

                    while (true)
                    {
                        moveNode = await CalculateNodes_Async(targetPos, cancellationToken);
                        if (moveNode != null)
                        {
                            if (moveNode.r == 100 && moveNode.c == 100)
                            {
                                moveNode = null;
                            }
                            else
                            {
                                await Employee_Move(moveNode, cancellationToken);

                                if (reCalculate)
                                {
                                    reCalculate = false;
                                    continue;
                                }
                                modelTrans.rotation = targetTable.workingSpot.rotation;
                                animatedAnimal.transforms.rotation = targetTable.workingSpot.rotation;
                            }
                            await UniTask.Delay(200, cancellationToken: cancellationToken);
                           

                            int foodNum = foodStacks[MachineType.PackingTable].foodStack.Count;
                            if (targetTable.customer != null)
                            {
                                //넘은 음식 카운터 배분
                                if(foodNum > targetTable.customer.foodStacks[0].needFoodNum)
                                {
                                    while(foodStacks[MachineType.PackingTable].foodStack.Count > targetTable.customer.foodStacks[0].needFoodNum)
                                    {
                                        PackageFood f = (PackageFood)foodStacks[MachineType.PackingTable].foodStack.Pop();
                                        f.Release();
                                        Vector3 targetPosition = targetTable.smallTable2.position + GameInstance.GetVector3(0, 0.7f * (targetTable.packageStack.foodStack.Count), 0);
                                        float r = UnityEngine.Random.Range(1, 2.5f);
#if HAS_DOTWEEN
                                        f.transforms.DOKill();
                                        f.transforms.DOJump(targetPosition, r, 1, 0.2f);
#endif
                                        targetTable.packageStack.foodStack.Push(f);

                                        await UniTask.Delay(300, cancellationToken: cancellationToken);
                                    }
                                    packingTable.employeeAssistant = null;
                                    await Employee_Fly(targetTable, cancellationToken);


                                    return;
                                }
                                else if(targetTable.customer.foodStacks[0].needFoodNum <= foodNum + targetTable.packageStack.foodStack.Count)
                                {
                                    //모자란 음식 카운터에서 채우기
                                    while (foodStacks[MachineType.PackingTable].foodStack.Count < targetTable.customer.foodStacks[0].needFoodNum)
                                    {
                                        PackageFood f = (PackageFood)targetTable.packageStack.foodStack.Pop();
          
                                        int currentStackCount = foodStacks[MachineType.PackingTable].foodStack.Count;
                                      
                                        Vector3 targetPosition = headPoint.position + GameInstance.GetVector3(0, 0.7f * (currentStackCount + 1), 0); // 목적지 저장
                                        float r = UnityEngine.Random.Range(1, 2.5f);

#if HAS_DOTWEEN
                                        DOTween.Kill(f.transforms); //Tween 제거
                                        f.transforms.DOJump(targetPosition, r, 1, 0.2f).OnComplete(() =>
                                          OnFoodStackComplete(f, targetPosition, foodStacks[MachineType.PackingTable], audioSource, foodStacks[MachineType.PackingTable].foodStack.Count, headPoint));
#endif
                                        await UniTask.Delay(300, cancellationToken: cancellationToken);
                                    }
                                    packingTable.employeeAssistant = null;
                                    await Employee_Fly(targetTable, cancellationToken);
                                    return;
                                }
                    

                            }

                            while (foodStacks[MachineType.PackingTable].foodStack.Count > 0)
                            {
                                PackageFood f = (PackageFood)foodStacks[MachineType.PackingTable].foodStack.Pop();
                                f.Release();
                                Vector3 targetPosition = targetTable.smallTable2.position + GameInstance.GetVector3(0, 0.7f * (targetTable.packageStack.foodStack.Count), 0);
                                float r = UnityEngine.Random.Range(1, 2.5f);
#if HAS_DOTWEEN
                                f.transforms.DOKill();
                                f.transforms.DOJump(targetPosition, r, 1, 0.2f);
#endif
                                targetTable.packageStack.foodStack.Push(f);

                                await UniTask.Delay(300, cancellationToken: cancellationToken);
                            }
                            await UniTask.Delay(200, cancellationToken: cancellationToken);
                            packingTable.employeeAssistant = null;
                            targetTable.employee = null;
                            GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
                        }
                        else
                        {
                            packingTable.employeeAssistant = null;
                            targetTable.employee = null;
                            Work();
                        }


                        return;
                    }
                }
                else
                {
                    packingTable.employeeAssistant = null;
                    Work();
                }


                return;
            }
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Employee_PackMove task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Employee_PackMove: {ex.Message}");
            throw;
        }
    }

    async UniTask Employee_Delivery(Vector3 position, PackingTable packingTable, CancellationToken cancellationToken = default)
    {
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                moveNode = await CalculateNodes_Async(position, cancellationToken);
                if (moveNode != null)
                {
                    if (moveNode.r == 100 && moveNode.c == 100)
                    {
                        moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveNode, cancellationToken);
                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }
                        modelTrans.rotation = packingTable.workingSpot.rotation;
                        animatedAnimal.transforms.rotation = packingTable.workingSpot.rotation;
                    }

                    await UniTask.Delay(200, cancellationToken: cancellationToken);

                    while (foodStacks[MachineType.PackingTable].foodStack.Count < packingTable.customer.foodStacks[0].needFoodNum)
                    {
                        PackageFood f = (PackageFood)packingTable.packageStack.foodStack.Pop();
                        int currentStackCount = foodStacks[MachineType.PackingTable].foodStack.Count;
                        Vector3 targetPosition = headPoint.position + GameInstance.GetVector3(0, 0.7f * (currentStackCount + 1), 0); // 목적지 저장
                        float r = UnityEngine.Random.Range(1, 2.5f);
#if HAS_DOTWEEN
                        DOTween.Kill(f.transforms); //Tween 제거
                        f.transforms.DOJump(targetPosition, r, 1, 0.2f).OnComplete(() =>
                          OnFoodStackComplete(f, targetPosition, foodStacks[MachineType.PackingTable], audioSource, foodStacks[MachineType.PackingTable].foodStack.Count, headPoint));
#endif
                        await UniTask.Delay(300, cancellationToken: cancellationToken);
                    }
                    await UniTask.Delay(500, cancellationToken: cancellationToken);


                    await Employee_Fly(packingTable, cancellationToken);
                }
                else
                {
                    packingTable.employee = null;
                    Work();
                }

                return;
            }
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Employee_Delivery task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Employee_Delivery: {ex.Message}");
            throw;
        }
    }
    async UniTask Employee_Move(Node n, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            Node a = n.parentNode == null ? n : n.parentNode;
            Stack<Node> stack = new Stack<Node>();
            while (a != null)
            {
                stack.Push(a);
                a = a.parentNode;
                await UniTask.NextFrame();
            }
            stack.Pop();
            await UniTask.NextFrame(cancellationToken: cancellationToken);
            while (stack.Count > 0)
            {
                if (trans == null || !trans)
                {
                    await UniTask.NextFrame();
                    return;
                }
                Node node = stack.Pop();
                float r = GameInstance.GameIns.calculatorScale.minY + node.r * GameInstance.GameIns.calculatorScale.distanceSize;
                float c = GameInstance.GameIns.calculatorScale.minX + node.c * GameInstance.GameIns.calculatorScale.distanceSize;
                Vector3 target = new Vector3(c, 0, r);
                float cur = (target - trans.position).magnitude;

                while (cur > 0.1f)
                {
                    if (reCalculate)
                    {
                        return;
                    }

                    if (trans == null || !trans)
                    {
                        await UniTask.NextFrame();
                        return;
                    }
                    cancellationToken.ThrowIfCancellationRequested();
                    animator.SetInteger("state", 1);
                    PlayAnim(animal.animationDic[animation_Run], animation_Run);
                    cur = (target - trans.position).magnitude;
                    Vector3 dir = (target - trans.position).normalized;
                    trans.position = Vector3.MoveTowards(trans.position, target, employeeLevel.move_speed * Time.deltaTime);
                    float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                    modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                    animatedAnimal.transforms.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                    await UniTask.NextFrame();
                    if (debuging) Debug.Log("이동 중");
                }
            }

            animator.SetInteger("state", 0);
            PlayAnim(animal.animationDic[animation_Idle_A], animation_Idle_A);
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Employee_Move task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Employee_Move: {ex.Message}");
            throw;
        }
    }

    async UniTask Employee_Fly(PackingTable packingTable, CancellationToken cancellationToken = default)
    {
        try
        {
            animator.SetInteger("state", 3);
            PlayAnim(animal.animationDic["Fly"], "Fly");
            while (trans.position.y < 6)
            {
                trans.Translate(Vector3.up * 6f * Time.deltaTime);

                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }
            await UniTask.Delay(200, cancellationToken: cancellationToken);
            float a = 0;
            while (a < 3)
            {
                trans.Translate(modelTrans.forward * 10f * Time.deltaTime);
                a += Time.deltaTime;
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }

            await UniTask.Delay(200, cancellationToken: cancellationToken);

            while (foodStacks[MachineType.PackingTable].foodStack.Count > 0)
            {
                PackageFood f = (PackageFood)foodStacks[MachineType.PackingTable].foodStack.Pop();
                packingTable.customer.foodStacks[0].foodStack.Push(f);
                FoodManager.RemovePackageBox(f);
            }
            //packingTable.customer = null;

            await UniTask.Delay(5000, cancellationToken: cancellationToken);

            bool check = false;
            int moveX = 0;
            int moveZ = 0;
            while (!check)
            {
                moveX = UnityEngine.Random.Range(-18, 22);
                moveZ = UnityEngine.Random.Range(-22, 21);
                if (Physics.CheckBox(GameInstance.GetVector3(moveX, 0, moveZ), GameInstance.GetVector3(0.6f, 0.6f, 0.6f), Quaternion.identity, 1 << 6 | 1 << 7 | 1 << 8))
                {
                    check = false;
                }
                else
                {
                    break;
                }
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }

            await UniTask.NextFrame(cancellationToken: cancellationToken);

            Vector3 targetLoc = GameInstance.GetVector3(moveX, 0, moveZ);
            while (true)
            {
                Vector3 currnetLoc = GameInstance.GetVector3(trans.position.x, 0, trans.position.z);
                Vector3 dir = (targetLoc - currnetLoc).normalized;
                trans.Translate(dir * 7f * Time.deltaTime);
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);

                if ((currnetLoc - targetLoc).magnitude < 0.5f) break;
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }

            await UniTask.Delay(400, cancellationToken: cancellationToken);
            while (true)
            {
                trans.position = GameInstance.GetVector3(trans.position.x, trans.position.y - 6f * Time.deltaTime, trans.position.z);

                if (trans.position.y < 0)
                {
                    trans.position = GameInstance.GetVector3(trans.position.x, 0, trans.position.z);

                    break;
                }

                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }

            animator.SetInteger("state", 0);
            PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
         
            if (packingTable.employee == this) packingTable.employee = null;
            GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Employee_Fly task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Employee_Fly: {ex.Message}");
            throw;
        }
    }
    void CompleteMove(Food f, Transform fixedTrans, Vector3 lasLoc, PackingTable p)
    {
        f.transforms.SetParent(fixedTrans);
        f.transforms.position = lasLoc;
        p.packingNumber++;
      //  p.packageStack.foodStack.Push(f);
    }
    void EmployeeWait_Start()
    {
        if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 대기중");
        bStart = true;
        animator.SetInteger("state", 2);
        PlayAnim(animal.animationDic[animation_LookAround], animation_LookAround);
    }
    void EmployeeWait_Update()
    {
        actionTimer += Time.deltaTime;
        //0.5초 대기
        if (actionTimer > 0.5f)
        {
            actionTimer = 0;
            bStart = false;
            employeeActions = EmployeeActions.ProcessWait;
            busy = false;
            waitTimer = 0.5f;
           // GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
        }
    }


    void EmployeePatrol_Start()
    {
       
        Vector3 target;
        float x = UnityEngine.Random.Range(-1f, 1f);
        float y = UnityEngine.Random.Range(-1f, 1f);
        Vector3 v3 = GameInstance.GetVector3(x, 0, y);
        float magnitude = v3.magnitude;
        if (magnitude > 0f)
        {
            v3 = v3 / magnitude;
        }
        float speed = UnityEngine.Random.Range(employeeData.move_speed, employeeData.move_speed * 2);
        target = trans.position + v3 * speed;

        bool interruptCheck = Physics.CheckBox(target, GameInstance.GetVector3(0.6f, 0.6f, 0.6f), Quaternion.identity, 1 << 6 | 1 << 7);
        bool validCheck = Physics.CheckBox(target, GameInstance.GetVector3(0.6f, 0.6f, 0.6f), Quaternion.identity, 1);
        if (validCheck && !interruptCheck)
        {
            bStart = true;
            animalMove = true;
            moveNode = CalculateNodes(target);
            if (moveNode != null)
            {
                if (moveNode.c == 100 && moveNode.r == 100) moveNode = null;
            }
            if(GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 패트롤 시작");
        }
    }

    void EmployeePatrol_Update()
    {
        actionTimer += Time.deltaTime;

        if (actionTimer > 0.5f)
        {
            employeeActions = EmployeeActions.ProcessWait;
            bStart = false;
            busy = false;
            if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 패트롤 종료");
            //  GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
            waitTimer = 0.5f;
        }
    }

    void EmployeeFoodMachine_Start(Node node, FoodMachine foodMachine)
    {
        bStart = true;
        animalMove = true;
        if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 조리기구로 이동");
    }
    void EmployeeFoodMachine_Rotate(Node node, FoodMachine foodMachine)
    {
        modelTrans.rotation = foodMachine.workingSpot.rotation;
        animatedAnimal.transforms.rotation = foodMachine.workingSpot.rotation;
        doOnce = true;
        if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 조리기구 방향으로 회전");
    }
    void EmployeeFoodMachine_Update(Node node, FoodMachine foodMachine)
    {
        if (foodMachine.foodStack.foodStack.Count > 0)
        {
            actionTimer += Time.deltaTime;
            if (actionTimer > employeeData.action_speed)
            {
                actionTimer = 0;

                int weight = 0;
              //  for (int i = 0; i < foodStacks.Count; i++) { weight += foodStacks[i].foodStack.Count; }
                if (weight >= employeeData.max_holds)
                {
                    foodMachine.employee = null;
                    employeeActions = EmployeeActions.ProcessWait;
                    waitTimer = 0.5f;
                    actionTimer = 0;
                    bStart = false;
                    doOnce = false;
                    busy = false;
                    if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 조리기구 종료");
                   // GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
                    return;
                }
           //     Food f = foodMachine.foodStack.foodStack[foodMachine.foodStack.foodStack.Count - 1];
       //         foodMachine.foodStack.foodStack.Remove(f);
                float r = UnityEngine.Random.Range(1, 2.5f);
                bool check = false;
                if (foodStacks.Count > 0)
                {
                    for (int j = 0; j < foodStacks.Count; j++)
                    {
           //             if (foodStacks[j].type == f.parentType)
                        {
                            check = true;
                        }
                    }
                }
                if (check == false)
                {
                    FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
                    ClearPlate(foodStack);
          //          foodStack.type = f.parentType;
          //          foodStacks.Add(foodStack);
                }

                int n = 0;
                for (n = 0; n < foodStacks.Count; n++)
                {
               //     if (foodStacks[n].type == f.parentType)
                    {

                        break;
                    }
                }
                if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 조리기구에서 음식을 가져가는 중");


             //   FoodStack fs = foodStacks[n];
             //   int currentStackCount = fs.foodStack.Count; // 음식 개수 저장
            //    Vector3 targetPosition = headPoint.position + GameInstance.GetVector3(0, 0.7f * (currentStackCount + 1), 0); // 목적지 저장

             //   DOTween.Kill(f.transforms); //Tween 제거
             //   f.transforms.DOJump(targetPosition, r, 1, 0.2f).OnComplete(() =>
             //     OnFoodStackComplete(f, targetPosition, fs, audioSource));  //음식을 해당 위치로 이동


              /*  f.transforms.DOJump(headPoint.position + GameInstance.GetVector3(0, 0.7f * (fs.foodStack.Count + 1), 0), r, 1, 0.2f).OnComplete(() =>
                {
                    fs.foodStack.Add(f);

                    f.transforms.position = headPoint.position + GameInstance.GetVector3(0, 0.7f * (fs.foodStack.Count), 0);
                    audioSource.Play();
                });*/
             

            }
        }
        else
        {
            foodMachine.employee = null;
            employeeActions = EmployeeActions.ProcessWait;
            waitTimer = 0.5f;
            actionTimer = 0;
            bStart = false;
            doOnce = false;
            busy = false;
            if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 조리기구 종료");
           // GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
        }
    }


    void EmployeeCounter_Start(Node node, Counter counter)
    {
        bStart = true;
        animalMove = true;
        if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 카운터로 이동");
    }
    void EmployeeCounter_Rotate(Node node, Counter counter)
    {
        modelTrans.rotation = counter.workingSpot.rotation;
        animatedAnimal.transforms.rotation = counter.workingSpot.rotation;
        doOnce = true;
        if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 카운터 방향으로 회전");
    }
    void EmployeeCounter_Update(Node node, Counter counter)
    {
        actionTimer += Time.deltaTime;
        if (employeeData.action_speed < actionTimer)
        {
            actionTimer = 0;
            int n = 0;
            for (int i = 0; i < foodStacks.Count; i++)
            {
            /*    n += foodStacks[i].foodStack.Count;
                if (foodStacks[i].foodStack.Count > 0)
                {
                    for (int j = 0; j < counter.foodStacks.Count; j++)
                    {
                        if (foodStacks[i].type == counter.foodStacks[j].type)
                        {
                            if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 카운터로 음식을 내려 놓음");
                            Food f = foodStacks[i].foodStack[foodStacks[i].foodStack.Count - 1];
                            foodStacks[i].foodStack.Remove(f);
                            float r = UnityEngine.Random.Range(1, 2.5f);
                            f.transforms.DOJump(counter.stackPoints[j].position + GameInstance.GetVector3(0, 0.7f * counter.foodStacks[j].foodStack.Count, 0), r, 1, employeeData.action_speed * 0.9f);
                            counter.foodStacks[j].foodStack.Add(f);
                            audioSource.Play();

                            return;
                        }
                    }
                }*/
            }
            if(n == 0)
            {
                if (foodStacks.Count > 0)
                {
                    for (int j = foodStacks.Count - 1; j >= 0; j--)
                    {
                  //      if (foodStacks[j].foodStack.Count == 0)
                        {
                          //  FoodStackManager.FM.RemoveFoodStack(foodStacks[j]);
                //            foodStacks.RemoveAt(j);
                        }
                    }
                }

                if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 카운터 종료");
                employeeActions = EmployeeActions.ProcessWait;
                bStart = false;
                doOnce = false;
                busy = false;
                actionTimer = 0;
                waitTimer = 0.2f;
               // GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
            }
        }
    }


    void EmployeeServing_Start(Node node, Counter counter)
    {
        bStart = true;
        animalMove = true;
        if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 서빙카운터 이동");
    }
    void EmployeeServing_Rotate(Node node, Counter counter)
    {
        doOnce = true;
        modelTrans.rotation = counter.workingSpot.rotation;
        animatedAnimal.transforms.rotation = modelTrans.rotation;
        if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 서빙 카운터 회전");
    }
    bool wait;

    int servToWait = 1;
    void EmployeeServing_Update(Node node, Counter counter)
    {
        if (counter.customer)
        {
            if(actionTimer < employeeData.action_speed *servToWait)
            {
                actionTimer += Time.deltaTime;
            }
            else
            {
                actionTimer = 0;
                int n = 0;
                int l = 0;
                for (int i = 0; i < counter.customer.foodStacks.Count; i++)
                {
                    l += counter.customer.foodStacks[i].needFoodNum - counter.customer.foodStacks[i].foodStack.Count;
                    for (int j = 0; j < counter.foodStacks.Count; j++)
                    {
                       
                        if (counter.customer.foodStacks[i].type == counter.foodStacks[j].type && counter.customer.foodStacks[i].foodStack.Count < counter.customer.foodStacks[i].needFoodNum && counter.foodStacks[j].foodStack.Count > 0)
                        {
                            n += counter.foodStacks[j].foodStack.Count;
                            while (counter.foodStacks[j].foodStack.Count > 0)
                            {
                              /*  if (counter.customer.foodStacks[i].foodStack.Count >= counter.customer.foodStacks[i].needFoodNum) break;
                                Food f = counter.foodStacks[j].foodStack[counter.foodStacks[j].foodStack.Count - 1];
                                counter.foodStacks[j].foodStack.Remove(f);
                                float r = UnityEngine.Random.Range(1, 2.5f);
                                if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 서빙중");
                                if(counter.customer.headPoint ==null)
                                {
                                    Debug.Log("local " + counter.customer.transform.localPosition + " world " + counter.customer.transform.position);
                                }
                                f.transforms.DOJump(counter.customer.headPoint.position + GameInstance.GetVector3(0, 0.7f * counter.customer.VisualizingFoodStack.Count, 0), r, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
                                {
                                    counter.customer.foodStacks[i].foodStack.Add(f);
                                    counter.customer.VisualizingFoodStack.Add(f);
                                    audioSource.Play();
                                    GameInstance.GameIns.uiManager.UpdateOrder(counter.customer, counter.counterType);
                                });*/

                                int p = 0;
                                for (int ff = 0; ff < counter.customer.foodStacks.Count; ff++) p += counter.customer.foodStacks[ff].needFoodNum - counter.customer.foodStacks[ff].foodStack.Count;

                                if (p == 1) servToWait = 2;
                                else servToWait = 1;
                                return;
                            }
                        }
                    }
                }

                if(n==0)
                {
                    //  wait = true;
                    if (l == 0)
                    {
                        wait = true;

                    }
                    else
                    {
                        if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 서빙 종료");
                        counter.employee = null;
                        wait = false;
                        busy = false;
                        doOnce = false;
                        bStart = false;
                        actionTimer = 0;
                        employeeActions = EmployeeActions.NONE;
                        GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
                        return;
                    }

                }
            }
        }
    }
    
    void EmployeeTable_Start(Node node, Table table)
    {
        if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 테이블 이동");
        bStart = true;
        animalMove = true;
    }
    void EmployeeTable_Rotate(Node node, Table table)
    {
        doOnce = true;
        table.interacting = true;
    }
    void EmployeeTable_Update(Node node, Table table)
    {
        if(actionTimer > 0.5f)
        {
            actionTimer = 0;
            if(table.numberOfGarbage > 0)
            {
                if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 테이블 쓰레기줍는중");
                float p = UnityEngine.Random.Range(1, 2.5f);
                Garbage go = table.garbageList[table.garbageList.Count - 1];//garbages.Pop();
                table.garbageList.Remove(go);
                table.numberOfGarbage--;
              /*  go.transforms.DOJump(headPoint.position + GameInstance.GetVector3(0, 0.5f * garbageList.Count, 0), p, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
                {
                    audioSource.Play();
                    go.transforms.position = headPoint.position + GameInstance.GetVector3(0, 0.5f * garbageList.Count, 0);
             //       garbageList.Add(go);
                });*/
            }
            else
            {
                if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 테이블 쓰레기줍기 완료");
                bStart = false;
                doOnce = false;
                employeeActions = EmployeeActions.ProcessWait;
                waitTimer = 0.5f;
                table.isDirty = false;
                table.interacting = false;
                employeeState = EmployeeState.TrashCan;
                table.employeeContoller = null;
                busy = false;
                tb = null;
               // GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
            }
        }
        else
        {
            actionTimer += Time.deltaTime;
        }

    }

    void EmployeeTrashCan_Start(Node node, TrashCan trashCan)
    {
        if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 쓰레기통 이동");
        bStart = true;
        animalMove = true;
    }
    void EmployeeTrashCan_Rotate(Node node, TrashCan trashCan)
    {
        
        if (garbageList.Count > 0)
        {
            if (actionTimer > 0.2f)
            {
                if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 쓰레기통 쓰레기 버리는중");
                actionTimer = 0;
          //      Garbage garbage = garbageList[garbageList.Count - 1];
           //     garbageList.Remove(garbage);
         //       float r = UnityEngine.Random.Range(1, 2.5f);
           //     garbage.transforms.DOJump(trashCan.transforms.position, r, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
          //      {
           //         audioSource.Play();
          //          GarbageManager.ClearGarbage(garbage);
         //       });
            }
            else
            {
                actionTimer += Time.deltaTime;
            }


        }
        else
        {
            doOnce = true;

        }
    }
    void EmployeeTrashCan_Update(Node node, TrashCan trashCan)
    {
        if (actionTimer > 0.5f)
        {
            actionTimer = 0;
            employeeState = EmployeeState.Wait;
            employeeActions = EmployeeActions.ProcessWait;
            waitTimer = 0.2f;
            bStart = false;
            doOnce = false;
            busy = false;
            if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 쓰레기통 쓰레기 버리기 완료");
          //  GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
        }
        else
        {
            actionTimer += Time.deltaTime;
        }
    }
    bool WaitTimer(float timer, Counter counter)
    {
        if(actionTimer < timer)
        {
            actionTimer += Time.deltaTime;
            return false;
        }
        else
        {
            if (GameInstance.GameIns.restaurantManager.employeeDebug) Debug.Log("직원 서빙 종료");
            counter.employee = null;
            wait = false;
            busy = false;
            doOnce = false;
            bStart = false;
            actionTimer = 0;
            employeeActions = EmployeeActions.NONE;
            GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
            return true;
        }
    }

    IEnumerator EmployeeWait()
    {
      
    //   animator.SetInteger("state", 2);

       //if(animationInstancing.GetCurrentAnimationInfo() != null)
       //{
          
       //     if (animal.animationDic[animationInstancing.GetCurrentAnimationInfo().animationName] != animal.animationDic[animation_LookAround])
       //     {
       //         animationInstancing.PlayAnimation(animal.animationDic[animation_LookAround]);
       //     }
       //}
       //else
       //{
       //     animationInstancing.PlayAnimation(animal.animationDic[animation_LookAround]);
       //     //      animationInstancing.PlayAnimation(animal.animationDic["IB"])
       // }
        

        yield return GetWaitTimer.WaitTimer.GetTimer(500); //new WaitForSeconds(0.5f);
                                                           //Invoke("Release", 0.4f);
                                                           //if(employeeCoroutine != null)
                                                           //{
                                                           //    StopCoroutine(employeeCoroutine);
                                                           //    employeeCoroutine = null;
                                                           //    busy = false;
                                                           //    Debug.Log("Stop Coroutine");
                                                           //}

        //  Debug.Log("Stop");
        //    GameInstance.GameIns.coroutneManager.StopCoroutines(ref employeeCoroutine);
        //StopCoroutine(employeeCoroutine);
        busy = false;
        //yield return GetWaitTimer.WaitTimer.GetTimer(300);
        //busy = true;
        //  GameInstance.GameIns.coroutneManager.StopCoroutines(employeeCoroutine);
        //  success = 1;

        //   Debug.Log("Finish");
        //   coroutineTimer = 0;
        //   waitC = null;

        // yield return null;
    }

    public void StopC()
    {
        GameInstance.GameIns.coroutneManager.StopCoroutines(ref employeeCoroutine);
        success = 2;
    }

    public void Release()
    {
  //      StopCoroutine(employeeCoroutine);
        success = 0;
       // Debug.Log("Im Running");
        busy = false;
        //employeeCoroutine = null;

    }

    IEnumerator EmployeeWait_Patrol()
    {
        Vector3 target;
        while (true)
        {
            float x = UnityEngine.Random.Range(-1f, 1f);
            float y = UnityEngine.Random.Range(-1f, 1f);
            Vector3 v3 = GameInstance.GetVector3(x, 0, y);
            float magnitude = v3.magnitude;
            if (magnitude > 0f)
            {
                v3 = v3 / magnitude;
            }
            float speed = UnityEngine.Random.Range(employeeData.move_speed, employeeData.move_speed * 2);
            target = trans.position + v3 * speed;

            bool interruptCheck = Physics.CheckBox(target, GameInstance.GetVector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1 << 6 | 1 << 7);
            bool validCheck = Physics.CheckBox(target, GameInstance.GetVector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1);
            if (validCheck && !interruptCheck)
            {
                break;
            }
            yield return null;
        }

        //Coord coord = CalculateCoords(target);
        //if (coord != null) {
        //    if (coord.c == 100 && coord.r == 100) coord = null;
        //    yield return StartCoroutine(AnimalMove(coord));
            
        //}

        yield return GetWaitTimer.WaitTimer.GetTimer(500);//new WaitForSeconds(0.5f);
        busy = false;
        GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
        
    }

   // FoodStack foodStack = new FoodStack();
    IEnumerator EmployeeFoodMachine(Node node, FoodMachine foodMachine)
    {
        yield return StartCoroutine(AnimalMove(node));

        modelTrans.rotation = foodMachine.workingSpot.rotation;
        animatedAnimal.transforms.rotation = foodMachine.workingSpot.rotation;

        while (foodMachine.foodStack.foodStack.Count > 0)
        {
            while (coroutineTimer2 < employeeData.action_speed)
            {
                coroutineTimer2 += Time.deltaTime;
                yield return null;
            }
            coroutineTimer2 = 0;
            //yield return new WaitForSeconds(0.1f);
            int weight = 0;
            //for (int i = 0; i < foodStacks.Count; i++) { weight += foodStacks[i].foodStack.Count; }
            if (weight >= employeeData.max_holds) break;
          /*  Food f = foodMachine.foodStack.foodStack[foodMachine.foodStack.foodStack.Count - 1];
            foodMachine.foodStack.foodStack.Remove(f);
            float r = UnityEngine.Random.Range(1, 2.5f);
            bool check = false;
            if (foodStacks.Count > 0)
            {
                for (int j = 0; j < foodStacks.Count; j++)
                {
                    if (foodStacks[j].type == f.parentType)
                    {
                        check = true;
                    }
                }
            }
            if (check == false)
            {
                FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
                ClearPlate(foodStack);
                foodStack.type = f.parentType;
                foodStacks.Add(foodStack);
            }*/

            int n = 0;
/*            for (n = 0; n < foodStacks.Count; n++)
            {
                if (foodStacks[n].type == f.parentType)
                {

                    break;
                }
            }
            FoodStack fs = foodStacks[n]; 
            int currentStackCount = fs.foodStack.Count; // 음식 개수 저장
            Vector3 targetPosition = headPoint.position + GameInstance.GetVector3(0, 0.7f * (currentStackCount + 1), 0); // 목적지 저장

            DOTween.Kill(f.transforms); //Tween 제거
            f.transforms.DOJump(targetPosition, r, 1, 0.2f).OnComplete(() =>
              OnFoodStackComplete(f, targetPosition,fs, audioSource));  //음식을 해당 위치로 이동
*/
            /*{
                *//*fs.foodStack.Add(f);
               
                f.transforms.position = headPoint.position + GameInstance.GetVector3(0, 0.7f * (fs.foodStack.Count), 0);
                //    foodList.Add(f);
                audioSource.Play();*//*
            });*/
        }

        while(coroutineTimer <= 0.5f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }

        coroutineTimer = 0;
     //   yield return new WaitForSeconds(0.5f);
        foodMachine.employee = null;
        busy = false;
        
    }
    void OnFoodStackComplete(Food food, Vector3 targetPoint, FoodStack fs, AudioSource audio, int index, Transform target)
    {
        if (!fs.foodStack.Contains(food))
        {
            fs.foodStack.Push(food);
            food.transform.position = targetPoint;
            food.Setup(target, index);
            audio.Play();
        }
    }
    void OnGarbageStackComplete(Garbage garbage, Vector3 targetPoint, Stack<Garbage> gs, AudioSource audio, int index, Transform target)
    {
        if (!gs.Contains(garbage))
        {
            gs.Push(garbage);
            garbage.transform.position = targetPoint;
            garbage.Setup(target, index);
            audio.Play();
        }
    }

    void OnGarbageClearComplete(Garbage garbage, AudioSource audio)
    {
        GarbageManager.ClearGarbage(garbage);
        audio.Play();
    }


  IEnumerator EmployeeCounter(Node node, Counter counter)
    {
        yield return StartCoroutine(AnimalMove(node));

        modelTrans.rotation = counter.transforms.rotation;
        animatedAnimal.transforms.rotation = counter.transforms.rotation;

        for (int i = 0; i < foodStacks.Count; i++)
        {
            for (int j = 0; j < counter.foodStacks.Count; j++)
            {
              /*  if (foodStacks[i].type == counter.foodStacks[j].type)
                {
                    while (foodStacks[i].foodStack.Count > 0)
                    {
                        Food f = foodStacks[i].foodStack[foodStacks[i].foodStack.Count - 1];
                        foodStacks[i].foodStack.Remove(f);
                        float r = UnityEngine.Random.Range(1, 2.5f);
                        f.transforms.DOJump(counter.stackPoints[j].position + GameInstance.GetVector3(0, 0.7f * counter.foodStacks[j].foodStack.Count, 0), r, 1, employeeData.action_speed * 0.9f);
                        counter.foodStacks[j].foodStack.Add(f);
                        audioSource.Play();
                        yield return new WaitForSeconds(employeeData.action_speed);
                    }
                }*/
            }
        }

        for (int i = foodStacks.Count - 1; i >= 0; i--)
        {
           /* if (foodStacks[i].foodStack.Count == 0)
            {
                foodStacks.RemoveAt(i);
            }*/
        }
        while (coroutineTimer <= 0.3f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }

        coroutineTimer = 0;
        //yield return new WaitForSeconds(0.3f);
        busy = false;
        
    }
    IEnumerator EmployeePackingTable(Node node, PackingTable packingTable)
    {
        yield return StartCoroutine(AnimalMove(node));

        modelTrans.rotation = packingTable.transforms.rotation;
        animatedAnimal.transforms.rotation = packingTable.transforms.rotation;
        for (int i = 0; i < foodStacks.Count; i++)
        {
            for (int j = 0; j < packingTable.foodStacks.Count; j++)
            {
               /* if (foodStacks[i].type == packingTable.foodStacks[j].type)
                {
                    while (foodStacks[i].foodStack.Count > 0)
                    {

                        Food f = foodStacks[i].foodStack[foodStacks[i].foodStack.Count - 1];
                        foodStacks[i].foodStack.Remove(f);
                        float r = UnityEngine.Random.Range(1, 2.5f);
                        f.transforms.DOJump(packingTable.stackPoints[j].position + GameInstance.GetVector3(0, 0.7f * packingTable.foodStacks[j].foodStack.Count, 0), r, 1, employeeData.action_speed * 0.9f);
                        packingTable.foodStacks[j].foodStack.Add(f);
                        audioSource.Play();
                        yield return new WaitForSeconds(employeeData.action_speed);

                    }
                }*/
            }
        }

        for (int i = foodStacks.Count - 1; i >= 0; i--)
        {
          /*  if (foodStacks[i].foodStack.Count == 0)
            {
                FoodStackManager.FM.RemoveFoodStack(foodStacks[i]);
                foodStacks.RemoveAt(i);
            }*/
        }
        while (coroutineTimer <= 0.3f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }

        coroutineTimer = 0;
     //   yield return new WaitForSeconds(0.3f);
        busy = false;
        
    }
    List<FoodStack> listEmployeeNowPacking = new List<FoodStack>();
    IEnumerator EmployeeNowPacking(Node node, PackingTable packingTable)
    {
        yield return StartCoroutine(AnimalMove(node));

        modelTrans.rotation = packingTable.packingTrans.rotation;
        animatedAnimal.transforms.rotation = modelTrans.rotation;

        HamburgerPackaging hamburgerPackaging = packingTable.GetComponent<HamburgerPackaging>();

        if (hamburgerPackaging.packingAction != null)
        {
            hamburgerPackaging.packingAction.Invoke();
            yield return new WaitForSeconds(1f); //GetWaitTimer.WaitTimer.GetTimer(1000);  //new WaitForSeconds(1f);
        }
        int num = 0;
        foreach (FoodStack p in packingTable.foodStacks)
        {
            num += p.foodStack.Count;
        }

        // List<FoodStack> list = new List<FoodStack>();
        listEmployeeNowPacking.Clear();
        while (num > 0)
        {
            for (int i = 0; i < packingTable.foodStacks.Count; i++)
            {
                listEmployeeNowPacking.Add(packingTable.foodStacks[i]);
            }
            listEmployeeNowPacking.Sort(delegate (FoodStack a, FoodStack b) { return a.foodStack.Count.CompareTo(b.foodStack.Count); });
            listEmployeeNowPacking.Reverse();
           /* Food f = listEmployeeNowPacking[0].foodStack[listEmployeeNowPacking[0].foodStack.Count - 1];

            listEmployeeNowPacking[0].foodStack.Remove(f);*/
          //  hamburgerPackaging.packingAction += () => { hamburgerPackaging.StartMove(f, false); };
          //  yield return hamburgerPackaging.MoveFood(f,true);
            yield return GetWaitTimer.WaitTimer.GetTimer(200); //new WaitForSeconds(0.2f);
            num = 0;
            foreach (FoodStack p in packingTable.foodStacks)
            {
                num += p.foodStack.Count;
            }
        }


        /* while(packingTable.f1.foodStack.Count > 0)
         {
             hamburgerPackaging.MoveHamburger();
             yield return new WaitForSeconds(0.5f);
         }*/
        while (coroutineTimer <= 0.3f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }

        coroutineTimer = 0;
    //    yield return new WaitForSeconds(0.3f);
        packingTable.employee = null;
        
        busy = false;
        GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
    }

   // FoodStack foodStackEmployeeGetPackagedFoods = new FoodStack(MachineType.PackingTable, 4, true, 1);
    IEnumerator EmployeeGetPackagedFoods(Node node, PackingTable packingTable)
    {
        if(step == 0) yield return StartCoroutine(AnimalMove(node));
        if(step == 0) step = 1;

        if(step == 1)
        {
            modelTrans.rotation = packingTable.packingTrans.rotation;
            animatedAnimal.transforms.rotation = modelTrans.rotation;

            while (packingTable.packageStack.foodStack.Count > 0)
            {
                while (coroutineTimer <= employeeData.action_speed)
                {
                    coroutineTimer += Time.deltaTime;
                    yield return null;
                }
                coroutineTimer = 0;

                int weight = 0;
             //   for (int i = 0; i < foodStacks.Count; i++) { weight += foodStacks[i].foodStack.Count; }
                if (weight >= employeeData.max_holds) break;

            /*    Food f = packingTable.packageStack.foodStack[packingTable.packageStack.foodStack.Count - 1];
                packingTable.packageStack.foodStack.Remove(f);
                float r = UnityEngine.Random.Range(1, 2.5f);
                bool check = false;
                if (foodStacks.Count > 0)
                {
                    for (int j = 0; j < foodStacks.Count; j++)
                    {
                        if (foodStacks[j].type == f.parentType)
                        {
                            check = true;
                        }
                    }
                }*/
              /*  if (check == false)
                {
                    //FoodStack foodStack = new FoodStack();
                    FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
                    ClearPlate(foodStackEmployeeGetPackagedFoods);
                    foodStack.type = f.parentType;
                    foodStacks.Add(foodStack);
                }*/

               /* int n = 0;
                for (n = 0; n < foodStacks.Count; n++)
                {
                    if (foodStacks[n].type == f.parentType)
                    {

                        break;
                    }
                }

                FoodStack fs = foodStacks[n];
                f.transforms.DOJump(headPoint.position + GameInstance.GetVector3(0, 0.7f * fs.foodStack.Count, 0), r, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
                {
                    f.transforms.position = headPoint.position + GameInstance.GetVector3(0, 0.7f * fs.foodStack.Count, 0);
                    //    foodList.Add(f);
                    audioSource.Play();

                    fs.foodStack.Add(f);
                });*/

                // yield return new WaitForSeconds(0.1f);

            }
            step = 2;
        }

        if (step == 2)
        {
            WorkSpaceManager workSpaceManager =  GameInstance.GameIns.workSpaceManager;

            for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
            {
                if (workSpaceManager.packingTables[i].counterType == Counter.CounterType.Delivery)
                {

                    yield return StartCoroutine(AnimalMove(CalculateNodes(workSpaceManager.packingTables[i].workingSpot_SmallTables[0].transforms.position)));
                    


                 /*   for (int j = 0; j < foodStacks.Count; j++)
                    {
                        if (foodStacks[j].type == MachineType.PackingTable)
                        {
                            while (foodStacks[j].foodStack.Count > 0)
                            {
                                while (coroutineTimer <= employeeData.action_speed)
                                {
                                    coroutineTimer += Time.deltaTime;
                                    yield return null;
                                }
                                coroutineTimer = 0;
                                float r = UnityEngine.Random.Range(1, 2.5f);
                                Food f = foodStacks[j].foodStack[foodStacks[j].foodStack.Count - 1];
                                foodStacks[j].foodStack.Remove(f);
                                workSpaceManager.packingTables[i].packageStack.foodStack.Add(f);
                                f.transforms.DOJump(workSpaceManager.packingTables[i].smallTable.position + GameInstance.GetVector3(0, 0.7f * workSpaceManager.packingTables[i].packageStack.foodStack.Count, 0), r, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
                                {
                                    f.transforms.position = workSpaceManager.packingTables[i].smallTable.position + GameInstance.GetVector3(0, 0.7f * workSpaceManager.packingTables[i].packageStack.foodStack.Count, 0);
                                    //    foodList.Add(f);
                                    audioSource.Play();

                                   
                                });
                            }
                        }
                    }

                    for (int k = foodStacks.Count - 1; k >= 0; k--)
                    {
                        if (foodStacks[k].foodStack.Count == 0)
                        {

                            FoodStackManager.FM.RemoveFoodStack(foodStacks[k]);
                            foodStacks.RemoveAt(k);

                        }
                    }*/
                        //  yield return StartCoroutine(AnimalMovement(CalculateCoords()));
                        // yield return new WaitForSeconds(0.3f);
                        while (coroutineTimer <= 0.3f)
                    {
                        coroutineTimer += Time.deltaTime;
                        yield return null;
                    }

                    step = 0;
                    coroutineTimer = 0;
                    packingTable.employeeAssistant = null;
                    busy = false;
                    GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
                    break;
                }
            }
        }
    }
    float timerY = 0;
    float timerXZ = 0;
    float coroutineTimer3 = 0;
   // float coroutineTimer4 = 0;
    float X = 0;
    float Z = 0;
    int foodA = -1;
    int foodNum = 0;


  //  FoodStack foodStackEmployeeDeliveryFoods = new FoodStack();
    IEnumerator EmployeeDeliveryFoods(Node node, PackingTable packingTable)
    {
        if(step == 0) yield return StartCoroutine(AnimalMove(node));
        if (step == 0) step = 1;
        if (step == 1)
        {
            modelTrans.rotation = packingTable.workingSpot.rotation;
            animatedAnimal.transforms.rotation = modelTrans.rotation;
            while (packingTable.customer.foodStacks[0].needFoodNum > foodNum)
            {
                while (coroutineTimer <= employeeData.action_speed)
                {
                    coroutineTimer += Time.deltaTime;
                    yield return null;
                }
                coroutineTimer = 0;

            /*    int weight = 0;
                for (int i = 0; i < foodStacks.Count; i++) { weight += foodStacks[i].foodStack.Count; }
                if (weight >= employeeData.max_holds) break;*/

             /*   Food food = packingTable.packageStack.foodStack[packingTable.packageStack.foodStack.Count - 1];
                packingTable.packageStack.foodStack.Remove(food);


                for (int i = 0; i < foodStacks.Count; i++)
                {
                    if (foodStacks[i].type == MachineType.PackingTable)
                    {
                        foodA = i;
                    }
                }
                if (foodA == -1)
                {
                    //FoodStack fs = new FoodStack();
                    ClearPlate(foodStackEmployeeDeliveryFoods);
                    foodStackEmployeeDeliveryFoods.type = MachineType.PackingTable;
                    foodStacks.Add(foodStackEmployeeDeliveryFoods);
                    for (int i = 0; i < foodStacks.Count; i++)
                    {
                        if (foodStacks[i].type == MachineType.PackingTable)
                        {
                            foodA = i;
                        }
                    }
                }*/

             /*   float r = UnityEngine.Random.Range(1, 2.5f);
               
                foodNum++;
                food.transforms.DOJump(headPoint.position + GameInstance.GetVector3(0, 0.7f * foodStacks[foodA].foodStack.Count, 0), r, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
                {
                    foodStacks[0].foodStack.Add(food);
                    food.transforms.position = headPoint.position + GameInstance.GetVector3(0, 0.7f * foodStacks[foodA].foodStack.Count, 0);
                    //    foodList.Add(f);
                    audioSource.Play();
                });*/
            }
            step = 2;
            foodNum = 0;
        }
        //배달 준비 완료
  
        animator.SetInteger("state", 3);
        PlayAnim(animal.animationDic["Fly"], "Fly");
        if (step == 2)
        {
            while (timerY <= 1.5f)
            {

                trans.Translate(Vector3.up * 6f * Time.deltaTime);

                timerY += Time.deltaTime;
                yield return null;
            }
            timerY = 0;
            step = 3;
        }

        if (step == 3)
        {
            while (coroutineTimer <= 0.4f)
            {
                coroutineTimer += Time.deltaTime;
                yield return null;
            }
            coroutineTimer = 0;
            step = 4;
        }
 
        if (step == 4)
        {
            while (timerXZ <= 3f)
            {
                trans.Translate(modelTrans.forward * 10f * Time.deltaTime);
                
                timerXZ += Time.deltaTime;
                yield return null;
            }
            timerXZ = 0;
            coroutineTimer = 0;
            step = 5;
        }

        if (step == 5)
        {
        /*    while (foodStacks[foodA].foodStack.Count > 0)
            {

                Food f = foodStacks[foodA].foodStack[foodStacks[foodA].foodStack.Count - 1];
                foodStacks[foodA].foodStack.Remove(f);
                packingTable.customer.foodStacks[0].foodStack.Add(f);
            }

            packingTable.customer = null;
            packingTable.employee = null;

            while (coroutineTimer <= 5f)
            {
                coroutineTimer += Time.deltaTime;
                yield return null;
            }
            for (int i = foodStacks.Count -1; i >=0; i--)
            {
                if (foodStacks[i].foodStack.Count == 0)
                {
                    FoodStackManager.FM.RemoveFoodStack(foodStacks[i]);
                    foodStacks.RemoveAt(i);
                }
            }*/
            coroutineTimer = 0;
            step = 6;
        }
        /*     for(int i= foodStacks.Count - 1; i <= 0; i--)
             {
                 if (foodStacks[i].foodStack.Count == 0)
                 {
                     foodStacks.RemoveAt(i);
                 }
             }*/

        bool check = false;
        if (step == 6)
        {
            while (!check)
            {
                X = UnityEngine.Random.Range(-18, 22);
                Z = UnityEngine.Random.Range(-22, 21);
                if (Physics.CheckBox(GameInstance.GetVector3(X, 0, Z), GameInstance.GetVector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1 << 6 | 1 << 7 | 1 << 8))
                {
                    check = false;
                }
                else
                {
                    break;
                }

                yield return null;
            }
            step = 7;
        }

        Vector3 targetLoc = GameInstance.GetVector3(X, 0, Z);
        if (step == 7)
        {
            while (true)
            {
                Vector3 currnetLoc = GameInstance.GetVector3(trans.position.x, 0, trans.position.z);
                Vector3 dir = (targetLoc - currnetLoc).normalized;
                trans.Translate(dir * 7f * Time.deltaTime, Space.World);
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);

                if ((currnetLoc - targetLoc).magnitude < 0.5f) break;
                yield return null;
            }
            step = 8;
        }
        if (step == 8)
        {
            while (coroutineTimer <= 0.4f)
            {
                coroutineTimer += Time.deltaTime;
                yield return null;
            }
            coroutineTimer = 0;
            step = 9;
        }   
        //  yield return new WaitForSeconds(0.4f);
        if (step == 9)
        {
            while (true)
            {
                trans.position = GameInstance.GetVector3(trans.position.x, trans.position.y - 6f * Time.deltaTime, trans.position.z);

                if (trans.position.y < 0)
                {
                    trans.position = GameInstance.GetVector3(trans.position.x, 0, trans.position.z);

                    break;
                }

                yield return null;
            }
            step = 10;
        }
        animator.SetInteger("state", 0);
        PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
        if(step == 10)
        {
            while (coroutineTimer <= 0.5f)
            {
                coroutineTimer += Time.deltaTime;
                yield return null;
            }
            coroutineTimer = 0;
            step = 0;
            
            busy = false;
            foodA = -1;
            GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
        }
    }

    IEnumerator EmployeeServing(Node node, Counter counter)
    {
        yield return StartCoroutine(AnimalMove(node));

        modelTrans.rotation = counter.workingSpot.rotation;
        animatedAnimal.transforms.rotation = modelTrans.rotation;
        if (counter.customer)
        {
            for (int i = 0; i < counter.customer.foodStacks.Count; i++)
            {
                for (int j = 0; j < counter.foodStacks.Count; j++)
                {
                    if (counter.customer.foodStacks[i].type == counter.foodStacks[j].type && counter.customer.foodStacks[i].foodStack.Count < counter.customer.foodStacks[i].needFoodNum && counter.foodStacks[j].foodStack.Count > 0)
                    {
                        while (counter.foodStacks[j].foodStack.Count > 0)
                        {
                            if (counter.customer.foodStacks[i].foodStack.Count >= counter.customer.foodStacks[i].needFoodNum) break;
                           // Food f = counter.foodStacks[j].foodStack[counter.foodStacks[j].foodStack.Count - 1];
                         /*   counter.foodStacks[j].foodStack.Remove(f);
                            float r = UnityEngine.Random.Range(1, 2.5f);
                            f.transforms.DOJump(counter.customer.headPoint.position + GameInstance.GetVector3(0, 0.7f * counter.customer.VisualizingFoodStack.Count, 0), r, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
                            {
                                counter.customer.foodStacks[i].foodStack.Add(f);
                                counter.customer.VisualizingFoodStack.Add(f);
                                audioSource.Play();
                                GameInstance.GameIns.uiManager.UpdateOrder(counter.customer, counter.counterType);
                            });

                            while (coroutineTimer2 <= employeeData.action_speed * 2)
                            {
                                coroutineTimer2 += Time.deltaTime;
                                yield return null;
                            }
*/
                            coroutineTimer2 = 0;
                        }
                    }
                }
            }
        }
        while (coroutineTimer <= 1f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }

        coroutineTimer = 0;
        counter.employee = null;
        
        busy = false;
    }

    IEnumerator EmployeeTable(Node node, Table table)
    {

        yield return StartCoroutine(AnimalMove(node, table));

        if (!table.interacting && table.isDirty)
        {
            // Garbage go = table.garbageInstance;
            while (table.numberOfGarbage > 0)
            {
                float p = UnityEngine.Random.Range(1, 2.5f);
                Garbage go = table.garbageList[table.garbageList.Count - 1];//garbages.Pop();
                table.garbageList.Remove(go);
                table.numberOfGarbage--;
              /*  go.transforms.DOJump(headPoint.position + GameInstance.GetVector3(0, 0.5f * garbageList.Count, 0), p, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
                {
                    audioSource.Play();
                    go.transforms.position = headPoint.position + GameInstance.GetVector3(0, 0.5f * garbageList.Count, 0);
               //     garbageList.Add(go);
                });*/
            }
            
            while(coroutineTimer <= 0.5f)
            {
                coroutineTimer += Time.deltaTime;
                yield return null;
            }
            coroutineTimer = 0;
            table.isDirty = false;
            employeeState = EmployeeState.TrashCan;
            table.employeeContoller = null;
            busy = false;
            
        }
        else
        {
            yield return GetWaitTimer.WaitTimer.GetTimer(500); //new WaitForSeconds(0.5f);
            coroutineTimer = 0;
            table.employeeContoller = null;
            busy = false;
            
        }
    }

    IEnumerator EmployeeTrashCan(Node node, TrashCan trashCan)
    {
        yield return StartCoroutine(AnimalMove(node));

        while (garbageList.Count > 0)
        {
            while (coroutineTimer2 <= employeeData.action_speed)
            {
                coroutineTimer2 += Time.deltaTime;
                yield return null;
            }
            coroutineTimer2 = 0;
         //   Garbage garbage = garbageList[garbageList.Count - 1];
          //  garbageList.Remove(garbage);
            float r = UnityEngine.Random.Range(1, 2.5f);
          //  garbage.transforms.DOJump(trashCan.transforms.position, r, 1, employeeData.action_speed * 0.9f).OnComplete(() =>
           // {
                audioSource.Play();
            //    GarbageManager.ClearGarbage(garbage);
          //  });
        }
        while (coroutineTimer <= 0.5f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }

        coroutineTimer = 0;
        employeeState = EmployeeState.Wait;
        
        busy = false;

    }

    void Reward(Vector3 position)
    {
       
        Employee_Reward(position).Forget();

      /*  Coord coord = CalculateCoords(position);
        if (coord != null)
        {
            if (coord.r == 100 && coord.c == 100) coord = null;

            StartCoroutine(EmployeeGetRewards(coord));
        }
        else
        {
         ///   
            StartCoroutine(EmployeeWait());
        }*/

    }

    void Reward_Start(Node node)
    {
        bStart = true;
        rewardingType = RewardingType.Walk;
        animalMove = true;
        gettingRewards = true;
        
    }
    void Reward_Rotate(Node node)
    {
        if(actionTimer > 0.5f)
        {
            actionTimer = 0;
            doOnce = true;
        }
        else
        {
            rewardingType = RewardingType.Eat;
            PlayAnim(animal.animationDic[animation_Peck], animation_Peck);
            animator.SetInteger("state", 4);
            actionTimer += Time.deltaTime;
        }
    }
    void Reward_Update(Node node)
    {
        if (reward.foods.Count > 0)
        {
            Food go = reward.foods.Pop();
            reward.EatFish(go);
            EXP += 20;
            GameInstance.GameIns.restaurantManager.playerData.fishesNum_InBox--;

            ui.GainExperience(); //경험치바 채워주기
                                 //yield return new WaitForSeconds(0.5f);
            if (EXP >= 100)
            {
                EXP = 0;
                //  GameInstance.GameIns.restaurantManager.combineDatas.employeeData[id - 1].level++;
                GameInstance.GameIns.restaurantManager.UpgradePenguin(employeeData.level, false, this); //펭귄의 레벨 업 및 능력치 변경하기
               
            }
            else SaveLoadManager.EmployeeLevelSave(true);
            doOnce = false;
            if(reward.foods.Count == 0)
            {
                //reward.ClearFishes();
                GameInstance.GameIns.applianceUIManager.ResetSchadule(reward);

                rewardingType = RewardingType.None;
                bStart = false;
                gettingRewards = false;
                busy = false;
                employeeActions = EmployeeActions.NONE;
                reward = null;
                GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
            }
        }
        else
        {
            reward.ClearFishes();
            doOnce = false;
            rewardingType = RewardingType.None;
            bStart = false;
            gettingRewards = false;
            busy = false;
            employeeActions = EmployeeActions.NONE;
            reward = null;
            GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
        }
    
        
    }

    IEnumerator EmployeeGetRewards(Node node)
    {
        rewardingType = RewardingType.Walk;
        //이동
        yield return StartCoroutine(AnimalMove(node, null, true));

        rewardingType = RewardingType.Eat;

        if(reward)
        {
            //행동
            while (reward.foods.Count > 0)
            {
                animator.SetInteger("state", 4);
                while (coroutineTimer <= 0.417f)
                {
                    coroutineTimer += Time.deltaTime;
                    yield return null;
                }

                Food go = reward.foods.Pop();

                reward.EatFish(go);
                Animal animal = GetComponentInParent<Animal>();
                EXP += 10;
                ui.GainExperience(); //경험치바 채워주기
                                     //yield return new WaitForSeconds(0.5f);
                if (EXP >= 100)
                {
                    EXP = 0;
                    //  GameInstance.GameIns.restaurantManager.combineDatas.employeeData[id - 1].level++;
                    GameInstance.GameIns.restaurantManager.UpgradePenguin(employeeData.level, false, this); //펭귄의 레벨 업 및 능력치 변경하기
                                                                                                        //Invoke("LevelUp", 0.5f);
                                                                                                        //   SaveLoadManager.EmployeeLevelSave(true);
                    while (coroutineTimer2 <= 0.5f)
                    {
                        coroutineTimer2 += Time.deltaTime;
                        yield return null;
                    }

                }
                else SaveLoadManager.EmployeeLevelSave(true);
                // yield return new WaitForSeconds(0.5f);
                animator.SetInteger("state", 0);

                while (coroutineTimer3 <= 0.2f)
                {
                    coroutineTimer3 += Time.deltaTime;
                    yield return null;
                }

                coroutineTimer3 = 0;
                coroutineTimer2 = 0;
                coroutineTimer = 0;
            }
        }

        reward.ClearFishes();   //상자 치우기
        SaveLoadManager.EmployeeLevelSave(true);
        SaveLoadManager.PlayerStateSave();

        // GameInstance.GameIns.inputManager.inputDisAble = false;

        reward = null;
        busy = false;
        
        rewardingType = RewardingType.None;

    }

    void LevelUp()
    {
        EXP = 0;
        Animal animal = GetComponentInParent<Animal>();
        Employee animalController = animal.GetComponentInChildren<Employee>();

        GameInstance.GameIns.restaurantManager.UpgradePenguin(GameInstance.GameIns.restaurantManager.combineDatas.employeeData[animalController.id - 1].level, false, animalController);
        SliderController sliderController = animal.GetComponentInChildren<SliderController>();
        // sliderController.
    }

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    Vector3 curNode;
    int animalMoveIndex;
    void AnimalMove_Start(Node node, Table table = null, bool rewards = false, bool bContinue = false)
    {
        if (node != null)
        {
            nodes.Clear();
            GetCalculatedList(ref nodes, node);
            InitMoveLocations();
            animalMoveIndex = 0;
            curNode = trans.position;

            PlayAnim(animal.animationDic[animation_Run], animation_Run);
            animator.SetInteger("state", 1);

            moveStart = 1;
        }
        else
        {
         //   Debug.LogError("Find");
            animalMove = false;
        }
      //  Debug.Log(moveStart);
    }

    Vector3 targetUpdate;
    Vector3 dirUpdate;
    float newMagnitudeUpdate;
    void AnimalMove_Update(Node node, Table table = null, bool rewards = false, bool bContinue = false)
    {
        if (animalMoveIndex <= moveLocations.Count - 1)
        {
            PlayAnim(animal.animationDic[animation_Run], animation_Run);
            animator.SetInteger("state", 1);

            //targetUpdate = AnimalMovement(coord, ref animalMoveIndex, coords);
            targetUpdate = AnimalMovement(Vector3.zero, ref animalMoveIndex, moveLocations);
            dirUpdate = (targetUpdate - trans.position);
            dirUpdate.Normalize();
            newMagnitudeUpdate = (targetUpdate - trans.position).magnitude;
            moveStart = 2;
        }
        else
        {
            MoveComplete();

            if (!bContinue)
            {
                PlayAnim(animal.animationDic[animation_Idle_A], animation_Idle_A);
                animator.SetInteger("state", 0);
            }

            animalMoveIndex=0;
            animalMove=false;
            moveStart = 0;
        }
      // Debug.Log(moveStart);
    }
    private void AnimalMove_Update2(Node node, Table table = null, bool rewards = false, bool bContinue = false)
    {
        float magnitude = (targetUpdate - trans.position).magnitude;
        if (magnitude < 0.1f)
        {
            trans.position = targetUpdate;
            animalMoveIndex++;
            moveStart = 1;
            return;
        }
        if (magnitude >= 0.1f)
        {
            if(table != null)
            {
                if (!table.isDirty || table.interacting)
                {
                    bStart = false;
                    doOnce = false;
                    employeeActions = EmployeeActions.NONE;
                    // table.isDirty = false;
                    employeeState = EmployeeState.Wait;
                    //employeeState = EmployeeState.TrashCan;
                    table.employeeContoller = null;
                    busy = false;
                    tb = null;
                    animalMove = false;
                    moveStart = 0;
                    GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
                    return;
                }
            }
            if(rewards)
            {
                if (reward)
                {
                    bool bStopping = false;

                    //Vector3 target = AnimalMovement(coord, ref i, coords);

                    Vector3 dirs = (targetUpdate - trans.position).normalized;
                    float newMagnitude = (targetUpdate - trans.position).magnitude;
                    float rr = GameInstance.GameIns.calculatorScale.minY + node.r * GameInstance.GameIns.calculatorScale.distanceSize;
                    float cc = GameInstance.GameIns.calculatorScale.minX + node.c * GameInstance.GameIns.calculatorScale.distanceSize;

                    float mag_r = Mathf.Abs(rr - modelTrans.position.z);
                    float mag_c = Mathf.Abs(cc - modelTrans.position.x);

                    if (mag_r <= 2f && mag_c <= 2f && mag_r + mag_c < 2.95f)
                    {
                        bStopping = true;
                       // break;
                    }
                    else if (magnitude <= 0.1f || newMagnitude < magnitude)
                    {
                        bStopping = true;
                    }

                    if(bStopping)
                    {
                        MoveComplete();
                        // bStart = false;
                        // doOnce = false;
                        animalMoveIndex = 0;
                        animalMove = false;
                        moveStart = 0;
                        PlayAnim(animal.animationDic[animation_Idle_A], animation_Idle_A);
                        animator.SetInteger("state", 0);

                        return;
                    }
                }
               
            }
            targetUpdate = AnimalMovement(Vector3.zero, ref animalMoveIndex, moveLocations);
            dirUpdate = (targetUpdate - trans.position);
            dirUpdate.Normalize();
            newMagnitudeUpdate = (targetUpdate - trans.position).magnitude;

            trans.Translate(dirUpdate * Time.fixedDeltaTime * employeeData.move_speed, Space.World);
            float angle = Mathf.Atan2(dirUpdate.x, dirUpdate.z) * Mathf.Rad2Deg;
            modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            animatedAnimal.transforms.rotation = Quaternion.AngleAxis(angle, Vector3.up);
           
        }
        else
        {
            animalMoveIndex++;
            moveStart = 1;
        }
    }

    List<Vector3> moveLocations = new List<Vector3>();

    void InitMoveLocations()
    {
        moveLocations.Clear();  
        Vector2 dir = Vector2.zero;
        Vector3 loc = trans.position;
        if (nodes.Count > 1)
        {
            for (int i = 1; i < nodes.Count; i++)
            {
                Node node = nodes[i];
                float r = GameInstance.GameIns.calculatorScale.minY + node.r * GameInstance.GameIns.calculatorScale.distanceSize;
                float c = GameInstance.GameIns.calculatorScale.minX + node.c * GameInstance.GameIns.calculatorScale.distanceSize;
                Vector3 newLoc = new Vector3(c, loc.y, r);

                Vector2 currentDir = new Vector2(node.c - nodes[i - 1].c, node.r - nodes[i - 1].r);
                if (currentDir != dir)
                {
                    Vector3 d = new Vector3(newLoc.x - loc.x, 0, newLoc.z - loc.z);
                    Vector3 di = d.normalized;
                    float dif = d.magnitude;
                    Debug.DrawLine(loc, loc + di * dif, Color.red, 100);


                    moveLocations.Add(loc);
                    moveLocations.Add(loc + di * dif);
                    dir = currentDir;
                }
                else if (nodes.Count - 1 == i)
                {
                    Vector3 d = new Vector3(newLoc.x - loc.x, 0, newLoc.z - loc.z);
                    Vector3 di = d.normalized;
                    float dif = d.magnitude;
                    moveLocations.Add(newLoc);
               //     Debug.Log(newLoc);
                    //   Debug.Log(loc);
                    //   Debug.Log(di);
                }


                loc = newLoc;
                //   return;
            }
        }
        else
        {
            for (int i = 1; i < nodes.Count; i++)
            {
                Node node = nodes[i];
                float r = GameInstance.GameIns.calculatorScale.minY + node.r * GameInstance.GameIns.calculatorScale.distanceSize;
                float c = GameInstance.GameIns.calculatorScale.minX + node.c * GameInstance.GameIns.calculatorScale.distanceSize;
                Vector3 newLoc = new Vector3(c, loc.y, r);
                moveLocations.Add(newLoc);
            }
        }
    }

    IEnumerator AnimalMove(Node node, Table table = null, bool rewards = false, bool bContinue = false)
    {
        if (node != null)
        {
            nodes.Clear();
            GetCalculatedList(ref nodes, node);
            int i = 0;
            Vector3 currentNode = trans.position;
            while (i < nodes.Count - 1)
            {
                PlayAnim(animal.animationDic[animation_Run], animation_Run);
                animator.SetInteger("state", 1);
                bool bStopping = false;

                Vector3 target = AnimalMovement(node, ref i, nodes);

                Vector3 dirs = (target - trans.position).normalized;
                float newMagnitude = (target - trans.position).magnitude;

            
                while (true)
                {
                    float magnitude = (target - trans.position).magnitude;
                    if (magnitude < 0.1f)
                    {
                        trans.position = target;
                        break;
                    }

                    if (table != null)
                    {
                        if (!table.isDirty || table.interacting)
                        {
                            bStopping = true;
                            break;
                        }
                    }

                    if (rewards)
                    {
                        if (reward)
                        {
                            float rr = GameInstance.GameIns.calculatorScale.minY + node.r * GameInstance.GameIns.calculatorScale.distanceSize;
                            float cc = GameInstance.GameIns.calculatorScale.minX + node.c * GameInstance.GameIns.calculatorScale.distanceSize;

                            float mag_r = Mathf.Abs(rr - modelTrans.position.z);
                            float mag_c = Mathf.Abs(cc - modelTrans.position.x);

                            if (mag_r <= 2f && mag_c <= 2f && mag_r + mag_c < 3)
                            {
                                bStopping = true;
                                break;
                            }
                            else if (magnitude <= 0.1f || newMagnitude < magnitude)
                            {
                                break;
                            }
                        }
                        else
                        {
                            bStopping = true;
                            break;
                        }
                    }
                    
                    if(magnitude >= 0.1f)
                    {
                        trans.Translate(dirs * Time.fixedDeltaTime * employeeData.move_speed, Space.World);
                        float angle = Mathf.Atan2(dirs.x, dirs.z) * Mathf.Rad2Deg;
                        modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                        animatedAnimal.transforms.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                        yield return waitForFixedUpdate;
                    }
                 
                }
                i++;
                if (bStopping)
                {
                    MoveComplete();
                    PlayAnim(animal.animationDic[animation_Idle_A], animation_Idle_A);
                    animator.SetInteger("state", 0);

                    //while (coroutineTimer <= 0.2f)
                    //{
                    //    coroutineTimer += Time.deltaTime;
                    //    yield return null;
                    //}
                    yield return GetWaitTimer.WaitTimer.GetTimer(200); //new WaitForSeconds(0.2f);
                    coroutineTimer = 0;

                    employeeState = EmployeeState.Wait;
                    if (table) table.employeeContoller = null;
                   // if (table) busy = false;
                    yield break;
                }
               
            }
            if (!bContinue)
            {
                MoveComplete();
                PlayAnim(animal.animationDic[animation_Idle_A], animation_Idle_A);
                animator.SetInteger("state", 0);
            }
        }
        else
        {
            Debug.Log("NULL");
        }
    }
}
