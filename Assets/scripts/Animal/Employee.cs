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
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;
using static GameInstance;
using static UnityEngine.Rendering.VolumeComponent;
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

//    EmployeeActions employeeActions = EmployeeActions.NONE;

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
    public bool pause;
    public int EXP
    {
        get { return exp; }
        set
        {
            if(employeeLevelData != null)
            {
                employeeLevelData.exp = value;
                GameIns.restaurantManager.employees.changed = true;
            }
            exp = value;
            ui.EXPChanged();
       
        }
    }
    public EmployeeLevelData employeeLevelData;

    // 직원 고용
    float elapsedTime = 0;
    Vector3 startPoint;
    Vector3 endPoint;
    Vector3 dir2;
    Vector3 controlVector;

    Counter deliveryCounter;
    public bool debuging;
   // int step = 0;

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

    Table tb;
    TrashCan tc;
    Vector3 lastPos;
    [NonSerialized]
    public bool falling = false;

    public Action<Employee> employeeCallback;
    private void Awake()
    {
        busy = false;
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
     /*   if(Input.GetMouseButtonDown(0))
        {
            EXP += 20;
        }*/
        if(pause && !falling)
        {
            Vector3 screenPos;

#if UNITY_ANDROID || UNITY_IOS
            screenPos = Touchscreen.current.touches[0].position.ReadValue();
#else
            screenPos = Mouse.current.position.ReadValue();
            //screenPos = Mouse.current.position.ReadValue();
#endif
            Ray ray = InputManger.cachingCamera.ScreenPointToRay(screenPos);
          
            if(Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1 << 15))
            {
                Vector3 pos = hit.point;
                trans.position = pos;
            }
/*
            float distance;
            if (groundPlane.Raycast(ray, out distance))
            {
                Vector3 mouseWorldPos = ray.GetPoint(distance);
                trans.transform.position = mouseWorldPos;
            }*/
           /* Vector3 pos = InputManger.cachingCamera.ScreenToWorldPoint(Input.mousePosition);
            pos.y = 20;
            trans.position = pos;*/

            
           // trans.position.y = 10f;
        }
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
        animal.PlayAnimation(AnimationKeys.Fly);
     //   PlayAnim(animal.animationDic["Fly"], "Fly");
        animator.SetInteger("state", 2); //보이지 않는 내부 모델의 애니메이션
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
            int posX = Mathf.FloorToInt((endPoint.x - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
            int posZ = Mathf.FloorToInt((endPoint.z - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);

            if (MoveCalculator.GetBlocks[MoveCalculator.GetIndex(posX, posZ)]) FindSafetyZone(endPoint);
            
            await UniTask.NextFrame(cancellationToken: cancellationToken);
            if (t >= 1.0f)
            {
                trans.position = endPoint;
                break;
            }
        }
        await UniTask.NextFrame(cancellationToken: cancellationToken);
        await UniTask.Delay(200,cancellationToken: cancellationToken);
        animal.PlayAnimation(AnimationKeys.Idle);
       // PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
        animator.SetInteger("state", 0);
        await UniTask.Delay(200,cancellationToken: cancellationToken);
        falling = false;    // 낙하 종료
        elapsedTime = 0;
        if (!pause)
        {
            employeeCallback?.Invoke(this);
            //GameIns.animalManager.AttachEmployeeTask(this);
        }
        else
        {
            pause = false;
            reCalculate = true;
            if(!busy) employeeCallback?.Invoke(this);
        }
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
        int posX = Mathf.FloorToInt((endPosition.x - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
        int posZ = Mathf.FloorToInt((endPosition.z - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);

      //  int arrZ = MoveCalculator.GetBlocks.GetLength(0);
      //  int arrX = MoveCalculator.GetBlocks.GetLength(1);
       // bool[] visit = MoveCalculator.GetBlocks.;
        int finalX = posX;
        int finalZ = posZ;
        bool[] visit = new bool[MoveCalculator.GetBlocks.Length];
        int[] dirX = new int[4] { 0, 1, 0, -1 };
        int[] dirZ = new int[4] { 1, 0, -1, 0 };
        Queue<(int x, int z)> q = new();
        q.Enqueue((posX, posZ));

        while(q.Count > 0)
        {
            var(currentX, currentZ) = q.Dequeue();
            finalX = currentX;
            finalZ = currentZ;
            if (!MoveCalculator.GetBlocks[MoveCalculator.GetIndex(finalX, finalZ)]) break;

            for(int i = 0; i<4; i++)
            {
                int nextX = currentX + dirX[i];
                int nextZ = currentZ + dirZ[i];
                if(Utility.ValidCheck(nextZ,nextX) && !visit[MoveCalculator.GetIndex(nextX, nextZ)])
                {
                    visit[MoveCalculator.GetIndex(nextX, nextZ)] = true;
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

        if (garbageList.Count == 0)
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
                        employeeState = EmployeeState.Wait;
                        Work();
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
                foodMachineList = workSpaceManager.foodMachines;
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
                    if (counterList[i].customer && (counterList[i].employee == null || counterList[i].employee == this) && counterList[i].counterType != CounterType.Delivery)
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
                    if (workSpaceManager.packingTables[i].counterType == CounterType.Delivery && (workSpaceManager.packingTables[i].employee == null || workSpaceManager.packingTables[i].employee == this) && workSpaceManager.packingTables[i].customer != null)
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

                        float min = 9999;
                        Seat seat = null;
                        int index = 0;
                        for(int j = 0; j< tableList[i].seats.Length; j++)
                        {
                            float dif = Vector3.Distance(trans.position, tableList[i].seats[j].transform.position);
                            if(dif < min)
                            {
                                index = j;
                                min = dif;
                                seat = tableList[i].seats[j];
                            }
                        }

                        if (seat != null)
                        {
                            target = seat.transform.position;
                            seat.animal = this;
                            Work(target, tableList[i], index);
                        }
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
                    if (counterList[i].customer && counterList[i].counterType != CounterType.Delivery)
                    {
                        for (int j = 0; j < counterList[i].customer.foodStacks.Count; j++)
                        {

                            Customer customer = counterList[i].customer;
                            for (int k = 0; k < foodMachineList.Count; k++)
                            {
                                //  int tmp = 0;
                                for (int l = 0; l < counterList[i].foodStacks.Count; l++)
                                {
                                    int tmp = 20 - (counterList[i].foodStacks[l].foodStack.Count + counterList[i].foodStacks[l].getNum);
                                    tmp = tmp > employeeLevel.max_weight ? employeeLevel.max_weight : tmp;
                                    if (counterList[i].customer.foodStacks[j].type != foodMachineList[k].machineType) continue;
                                    /*if (foodMachineList[k].foodStack.foodStack.Count + counterList[i].foodStacks[l].foodStack.Count + customer.foodStacks[j].foodStack.Count >= customer.foodStacks[j].needFoodNum &&
                                   (foodMachineList[k].employee == null || foodMachineList[k].employee == this) && counterList[i].foodStacks[l].type == counterList[i].customer.foodStacks[j].type &&
                                   customer.foodStacks[j].needFoodNum > customer.foodStacks[j].foodStack.Count + counterList[i].foodStacks[l].foodStack.Count)*/
                                    if ((foodMachineList[k].employee == null || foodMachineList[k].employee == this) && counterList[i].foodStacks[l].type == counterList[i].customer.foodStacks[j].type &&
                                   customer.foodStacks[j].needFoodNum > customer.foodStacks[j].foodStack.Count + counterList[i].foodStacks[l].foodStack.Count && foodMachineList[k].foodStack.foodStack.Count > 0)
                                    {
                                        counterList[i].foodStacks[l].getNum += tmp;
                                        foodMachineList[k].employee = this;
                                        foodMachineList[k].getNum += tmp;
                                        employeeState = EmployeeState.FoodMachine;
                                        target = foodMachineList[k].workingSpot.position;
                                        deliveryCounter = counterList[i];
                                        Vector3 t = counterList[i].workingSpot_SmallTables[l].transforms.position;
                                        //  counterList[i].foodStacks[l].getNum = tmp;
                                        Work(target, foodMachineList[k], t, counterList[i]);
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
                    if (packingTables[i].counterType == CounterType.None && packingTables[i].packageStack.foodStack.Count > 0 && (packingTables[i].employeeAssistant == null || packingTables[i].employeeAssistant == this))
                    {
                        for (int j = 0; j < packingTables.Count; j++)
                        {
                            if (i != j && packingTables[j].counterType == CounterType.Delivery && packingTables[j].employee == null)
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
                    if ((packingTables[i].employee == null || packingTables[i].employee == this) && packingTables[i].counterType ==CounterType.None)
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
                            for (int k = 0; k < counterList.Count; k++)
                            {
                                for (int l = 0; l < counterList[k].foodStacks.Count; l++)
                                {
                                    int tmp = 20 - (fStacks[i].foodStack.Count + fStacks[i].getNum);
                                    tmp = tmp > employeeLevel.max_weight ? employeeLevel.max_weight : tmp;
                                    if (fStacks[i].id == counterList[k].foodStacks[l].id && tmp > 0)
                                    {
                                        fStacks[i].getNum += tmp;
                                        deliveryCounter = counterList[k];
                                        foodMachineList[j].employee = this;
                                        employeeState = EmployeeState.FoodMachine;
                                        target = foodMachineList[j].workingSpot.position;
                                        foodMachineList[j].getNum += tmp;
                                        Vector3 t = counterList[k].workingSpot_SmallTables[l].transform.position;
                                        Work(target, foodMachineList[j], t, counterList[k]);
                                       
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
            trashCanList.Clear();
            trashCanList = workSpaceManager.trashCans;
            trashCanList = trashCanList.OrderBy(trashcan => (trashcan.transforms.position - trans.position).magnitude).ToList();
            if(trashCanList.Count > 0)
            {
                Work(trashCanList[0].throwPos.position, trashCanList[0]);
                return;

            }

            //쓰레기 통이 없음
            Work();
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

    public void Work(Vector3 position, FoodMachine foodMachine, Vector3 counterPosition, Counter counter)
    {
        Employee_FoodMachine(position, foodMachine, counterPosition, counter, App.GlobalToken).Forget();

    }

    public void Work(Vector3 position, Counter counter, bool serving)
    {
        if (!serving) Employee_Counter(position, counter, App.GlobalToken).Forget();
        else Employee_Serving(position, counter, App.GlobalToken).Forget();
    }

    public void Work(Vector3 position, Table table, int index)
    {
        Employee_Table(position, table, index, App.GlobalToken).Forget();
    }

    public void Work(Vector3 position, TrashCan trash)
    {
        Employee_Trashcan(position, trash, App.GlobalToken).Forget();
    }

    static WaitForSeconds zerodotfive = new WaitForSeconds(0.5f);
 //   float coroutineTimer2 = 0;
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
            //employeeActions = EmployeeActions.NONE;
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
                animator.SetInteger("state", 0);
                animal.PlayAnimation(AnimationKeys.Idle);
               // PlayAnim(animal.animationDic[animation_LookAround], animation_LookAround);
                await UniTask.Delay(500, cancellationToken: cancellationToken);
            }
            else
            {
                await Employee_Patrol(cancellationToken);
            }

            busy = false;
            employeeCallback?.Invoke(this);
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
                    Stack<Vector3> moveTargets = await CalculateNodes_Async(target, true, cancellationToken);
                    await UniTask.SwitchToMainThread(cancellationToken: cancellationToken);
                    if (moveTargets != null && moveTargets.Count >0)
                    {
                        Vector3 test = moveTargets.Peek();
                        if (test.x == 100 && test.z == 100)
                        {
                            animator.SetInteger("state", 2);
                            animal.PlayAnimation(AnimationKeys.Idle);
                        }
                        else
                       /* if (moveNode.c == 100 && moveNode.r == 100)
                        {
                            moveNode = null;
                            animator.SetInteger("state", 2);
                            animal.PlayAnimation(AnimationKeys.Idle);
                           // PlayAnim(animal.animationDic[animation_LookAround], animation_LookAround);
                        }
                        else*/
                        {
                            await Employee_Move(moveTargets, target, cancellationToken);
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
                        animal.PlayAnimation(AnimationKeys.Idle);
                        //PlayAnim(animal.animationDic[animation_LookAround], animation_LookAround);
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


    async UniTask Employee_FoodMachine(Vector3 position, FoodMachine foodMachine, Vector3 counterPosition, Counter counter, CancellationToken cancellationToken = default)
    {
        try
        {
            //조달
            while (true)
            {
                //조달
                cancellationToken.ThrowIfCancellationRequested();
                while(pause) await UniTask.Delay(100, cancellationToken: cancellationToken);
               
                lastPos = position;
                Stack<Vector3> moveTargets = await CalculateNodes_Async(position, true, cancellationToken);
                if (moveTargets != null && moveTargets.Count > 0)
                {
                    Vector3 test = moveTargets.Peek();
                    if (test.z == 100 && test.x == 100)
                    {
                        Debug.Log("Find");
                    }
                    else
                    {
                        //employeeActions = EmployeeActions.EmployeeFoodMachine;
                        await Employee_Move(moveTargets, position, cancellationToken);

                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            if(bResearch)
                            {
                                animator.SetInteger("state", 0);
                                animal.PlayAnimation(AnimationKeys.Idle);
                                await UniTask.Delay(300, cancellationToken: cancellationToken);
                                bResearch = false;
                           
                                busy = false;
                                employeeCallback?.Invoke(this);
                                return;
                            }
                            continue;
                        }

                        modelTrans.rotation = foodMachine.workingSpot.rotation;
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
                                if (debuging) Debug.Log("직원 조리기구 종료");
                            
                                break;
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
                            audioSource.clip = GameIns.gameSoundManager.ThrowSound();
                            audioSource.volume = 0.2f;
                            audioSource.Play();
#endif 
                            await UniTask.Delay(300, cancellationToken: cancellationToken);
                            if (pause) break;
                        }
                    }
                }
                else
                {
                    foodMachine.employee = null;
                    if (debuging) Debug.Log("Fail FoodMachine");
                    Work();
                }
                if (!pause) break;
            }

            await UniTask.Delay(300, cancellationToken: cancellationToken);

            
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                while (pause) await UniTask.Delay(100, cancellationToken: cancellationToken);
                lastPos = counterPosition;
                Stack<Vector3> moveTargets = await CalculateNodes_Async(counterPosition, true, cancellationToken);
                if (moveTargets != null && moveTargets.Count > 0)
                {
                    Vector3 test = moveTargets.Peek();
                    if (test.z == 100 && test.x == 100)
                    {
                        // moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveTargets, counterPosition, cancellationToken);
                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }
                        modelTrans.rotation = counter.workingSpot.rotation;

                    }
                    //employeeActions = EmployeeActions.EmployeeCounter;


                    if (debuging) Debug.Log("직원 카운터에 음식을 내려놓는 중");


                    FoodStack foodStack = null;

                    for (int i = 0; i < counter.foodStacks.Count; i++)
                    {
                        MachineType t = counter.foodStacks[i].type;
                        foodStack = foodStacks[t];

                        while (foodStack.foodStack.Count > 0)
                        {
                            if (counter.foodStacks == null) continue;
                            if (i >= counter.foodStacks.Count) continue;
                            if (counter.foodStacks[i].foodStack == null) continue;
                            if (foodStacks[t].foodStack == null) continue;

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
                            f.transforms.rotation = Quaternion.Euler(0,0,0);
                            counter.foodStacks[i].foodStack.Push(f);
                            counter.foodStacks[i].getNum = counter.foodStacks[i].getNum - 1 < 0 ? 0 : counter.foodStacks[i].getNum - 1;

                            if (debuging) Debug.Log(counter.foodStacks[i].getNum);
                            audioSource.clip = GameIns.gameSoundManager.ThrowSound();
                            audioSource.volume = 0.2f;
                            audioSource.Play();
                            await UniTask.Delay(300, cancellationToken: cancellationToken);

                            EXP += 1;
                            if(pause) goto Escape;
                        }
                    }
                    if (debuging) Debug.Log("직원 카운터 종료");

                    busy = false;
                    await UniTask.Delay(300, cancellationToken: cancellationToken);
                    //  FindEmployeeWorks();
                    employeeCallback?.Invoke(this);



                }
                else
                {
                    if (debuging) Debug.Log("Error");
                    // Work();
                    await Employee_Wait(0, cancellationToken);
                }
                return;
            Escape: continue;
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
                Stack<Vector3> moveTargets = await CalculateNodes_Async(position, true, cancellationToken);
                if (moveTargets != null && moveTargets.Count > 0)
                {
                    Vector3 test = moveTargets.Peek();
                    if (test.z == 100 && test.x == 100)
                    {
                        // moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveTargets, position, cancellationToken);
                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }
                        modelTrans.rotation = counter.workingSpot.rotation;
                      
                    }
                    //employeeActions = EmployeeActions.EmployeeCounter;


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

                    employeeCallback?.Invoke(this);

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
                while (pause) await UniTask.Delay(100, cancellationToken: cancellationToken);
                lastPos = position;
                Stack<Vector3> moveTargets = await CalculateNodes_Async(position, true, cancellationToken);
                if (moveTargets != null && moveTargets.Count > 0)
                {
                    Vector3 test = moveTargets.Peek();
                    if (test.z == 100 && test.x == 100)
                    {
                        // moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveTargets, position, cancellationToken);
                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }
                        modelTrans.rotation = counter.workingSpot.rotation;
                    
                    }

                    //employeeActions = EmployeeActions.EmployeeServing;

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
                                        audioSource.clip = GameIns.gameSoundManager.ThrowSound();
                                        audioSource.volume = 0.2f;
                                        audioSource.Play();
#endif
                                        counter.customer.VisualizingFoodStack.Add(f);
                                        index++;
                                        await UniTask.Delay(300, cancellationToken: cancellationToken);
                                        EXP += 2;
                                        if (debuging) Debug.Log("Find");
                                        if (pause) goto Escape;
                                    }
                                }
                            }
                        }
                    }

                    counter.employee = null;
                    busy = false;
                    await UniTask.Delay(500, cancellationToken: cancellationToken);
                    employeeCallback?.Invoke(this);

                }
                else
                {
                    counter.employee = null;
                    Work();
                }
                return;
            Escape: continue;
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

    async UniTask Employee_Table(Vector3 position, Table table, int seatIndex, CancellationToken cancellationToken = default)
    {
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                while (pause) await UniTask.Delay(100, cancellationToken: cancellationToken);
                //종료
                if (table.numberOfGarbage == 0)
                {
                    table.isDirty = false;
                    tb = table;
                    //employeeActions = EmployeeActions.EmployeeTable;
                    table.employeeContoller = null;
                    employeeState = EmployeeState.TrashCan;
                    busy = false;
                    await UniTask.Delay(500, cancellationToken: cancellationToken);
                    GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
                    return;
                }
                lastPos = position;
                Stack<Vector3> moveTargets = await CalculateNodes_Async(position, true, cancellationToken);
                if (moveTargets != null && moveTargets.Count > 0)
                {
                    Vector3 test = moveTargets.Peek();
                    if (test.z == 100 && test.x == 100)
                    {
                        // moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveTargets, position, cancellationToken);

                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;

                            if(table.interacting)
                            {
                                animator.SetInteger("state", 0);
                                animal.PlayAnimation(AnimationKeys.Idle);
                              //  PlayAnim(animal.animationDic[animation_Idle_A], animation_Idle_A);
                                busy = false;
                                await UniTask.Delay(500);
                                employeeCallback?.Invoke(this);
                                return;
                            }

                            continue;
                        }
                        modelTrans.rotation = table.seats[seatIndex].transforms.rotation;
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
                        audioSource.clip = GameIns.gameSoundManager.ThrowSound();
                        audioSource.volume = 0.2f;
                        audioSource.Play();
#endif
                        await UniTask.Delay(300, cancellationToken: cancellationToken);
                        if (pause) goto Escape;
                    }

                    table.isDirty = false;
                    tb = table;
                    //employeeActions = EmployeeActions.EmployeeTable;
                    table.employeeContoller = null;
                    employeeState = EmployeeState.TrashCan;
                    busy = false;
                    table.seats[seatIndex].animal = null;
                    await UniTask.Delay(500, cancellationToken: cancellationToken);
                    GameInstance.GameIns.animalManager.AttachEmployeeTask(this);
                }
                else
                {
                    table.employeeContoller = null;
                    Work();
                }
                return;
            Escape: continue;
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
                while (pause) await UniTask.Delay(100, cancellationToken: cancellationToken);
                Stack<Vector3> moveTargets = await CalculateNodes_Async(position, true, cancellationToken);
                if (moveTargets != null && moveTargets.Count > 0)
                {
                    Vector3 test = moveTargets.Peek();
                    if (test.z == 100 && test.x == 100)
                    {
                        // moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveTargets, position, cancellationToken);

                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }

                        modelTrans.LookAt(trash.transforms);
                       
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
                        audioSource.clip = GameIns.gameSoundManager.ThrowSound();
                        audioSource.volume = 0.2f;
                        audioSource.Play();
#endif
                        await UniTask.Delay(300, cancellationToken: cancellationToken);
                        EXP += 2;
                       
                        if (pause) goto Escape;
                    }
                    busy = false;
                    await UniTask.Delay(500, cancellationToken: cancellationToken);
                    employeeState = EmployeeState.Wait;
                    employeeCallback?.Invoke(this);
                }
                else
                {
                    Work();
                }
                return;
            Escape: continue;
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
                Stack<Vector3> moveTargets = await CalculateNodes_Async(position, true, cancellationToken);

                if (moveTargets != null && moveTargets.Count > 0)
                {
                    Vector3 test = moveTargets.Peek();
                    if (test.z == 100 && test.x == 100)
                    {
                        // moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveTargets, position, cancellationToken);

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
                        animal.PlayAnimation(AnimationKeys.Eat);
                        //PlayAnim(animal.animationDic[animation_Peck], animation_Peck);
                        animator.SetInteger("state", 4);
                        await UniTask.Delay(500, cancellationToken: cancellationToken);
                        Food go = reward.foods.Pop();
                        reward.EatFish(go);
                        EXP += 20;
                     //   GameInstance.GameIns.restaurantManager.restaurantCurrency.fishesNum_InBox--;

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
                    employeeCallback?.Invoke(this);
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
                while(pause) await UniTask.Delay(100, cancellationToken:  cancellationToken);
                Stack<Vector3> moveTargets = await CalculateNodes_Async(position, true, cancellationToken);
                if (moveTargets != null && moveTargets.Count > 0)
                {
                    Vector3 test = moveTargets.Peek();
                    if (test.z == 100 && test.x == 100)
                    {
                        // moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveTargets, position, cancellationToken);
                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }
                        modelTrans.rotation = packingTable.workingSpot.rotation;
                        
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
                                    if (pause) goto Escape;
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
                    busy = false;
                    await UniTask.Delay(500, cancellationToken: cancellationToken);
                    employeeCallback?.Invoke(this);
                }
                else
                {
                    packingTable.employee = null;
                    Work();

                }
                return;
            Escape: continue;
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
                while(pause) await UniTask.Delay(100, cancellationToken: cancellationToken);
                Stack<Vector3> moveTargets = await CalculateNodes_Async(position, true, cancellationToken);

                if (moveTargets != null && moveTargets.Count > 0)
                {
                    Vector3 test = moveTargets.Peek();
                    if (test.z == 100 && test.x == 100)
                    {
                        // moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveTargets, position, cancellationToken);

                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }
                        modelTrans.rotation = packingTable.endTrans.rotation;
                       
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
                        if (pause) break;
                    }

                    if (pause) continue;
                    await UniTask.Delay(500, cancellationToken: cancellationToken);
                    packingTable.employeeAssistant = null;
                    //카운터로 배달

                    while (true)
                    {
                        while (pause) await UniTask.Delay(100, cancellationToken: cancellationToken);
                        Stack<Vector3> moveTargetss = await CalculateNodes_Async(targetPos, true, cancellationToken);
                        if (moveTargetss != null && moveTargetss.Count > 0)
                        {
                            Vector3 tests = moveTargetss.Peek();
                            if (tests.z == 100 && tests.x == 100)
                            {
                                // moveNode = null;
                            }
                            else
                            {
                                await Employee_Move(moveTargetss, targetPos, cancellationToken);

                                if (reCalculate)
                                {
                                    while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                                    reCalculate = false;
                                    continue;
                                }
                                modelTrans.rotation = targetTable.workingSpot.rotation;
                                
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
                                        if (pause) goto Escape;
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
                                        if (pause) goto Escape;
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
                                if (pause) goto Escape;
                            }
                            busy = false;
                            await UniTask.Delay(200, cancellationToken: cancellationToken);
                            packingTable.employeeAssistant = null;
                            targetTable.employee = null;
                            employeeCallback?.Invoke(this);
                        }
                        else
                        {
                            packingTable.employeeAssistant = null;
                            targetTable.employee = null;
                            Work();
                        }


                        return;
                    Escape: continue;
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
                while(pause) await UniTask.Delay(100, cancellationToken: cancellationToken);    
                Stack<Vector3> moveTargets = await CalculateNodes_Async(position, true, cancellationToken);
                if (moveTargets != null && moveTargets.Count > 0)
                {
                    Vector3 test = moveTargets.Peek();
                    if (test.z == 100 && test.x == 100)
                    {
                       // moveNode = null;
                    }
                    else
                    {
                        await Employee_Move(moveTargets, position, cancellationToken);
                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }
                        modelTrans.rotation = packingTable.workingSpot.rotation;
                    
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
                        if (pause) goto Escape;
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
            Escape: continue;
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
    async UniTask Employee_Move(Stack<Vector3> n, Vector3 loc, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            /* Node a = n.parentNode == null ? n : n.parentNode;
             Stack<Node> stack = new Stack<Node>();
             while (a != null)
             {
                 stack.Push(a);
                 a = a.parentNode;
                 await UniTask.NextFrame();
             }*/
            n.Pop();
            await UniTask.NextFrame(cancellationToken: cancellationToken);
            while (n.Count > 0)
            {
                if (trans == null || !trans)
                {
                    await UniTask.NextFrame();
                    return;
                }
                Vector3 target = n.Pop();
             /*   float r = GameInstance.GameIns.calculatorScale.minY + node.r * GameInstance.GameIns.calculatorScale.distanceSize;
                float c = GameInstance.GameIns.calculatorScale.minX + node.c * GameInstance.GameIns.calculatorScale.distanceSize;
                Vector3 target = new Vector3(c, 0, r);*/
                float cur = (target - trans.position).magnitude;

                while(true)
                {
                    while (pause) await UniTask.Delay(100, cancellationToken: cancellationToken);
                    if (reCalculate)
                    {
                        Debug.Log("Recalculate");
                        return;
                    }

                    if (trans == null || !trans)
                    {
                        await UniTask.NextFrame();
                        return;
                    }


                    if (Vector3.Distance(trans.position, target) <= 0.01f) break;
                    cancellationToken.ThrowIfCancellationRequested();
                    animator.SetInteger("state", 1);
                    animal.PlayAnimation(AnimationKeys.Walk);
                    // PlayAnim(animal.animationDic[animation_Run], animation_Run);
                    cur = (target - trans.position).magnitude;
                    Vector3 dir = (target - trans.position).normalized;
                    trans.position = Vector3.MoveTowards(trans.position, target, employeeLevel.move_speed * Time.deltaTime);
                    float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                    modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                    // animatedAnimal.transforms.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                    await UniTask.NextFrame();
                    if (debuging) Debug.Log("이동 중");
                }
            }

            Vector3 newLoc = loc;
            newLoc.y = 0;
            while (true)
            {
                while (pause) await UniTask.Delay(100, cancellationToken: cancellationToken);
                if (reCalculate)
                {
                    Debug.Log("Recalculate");
                    return;
                }
                animator.SetInteger("state", 1);
                animal.PlayAnimation(AnimationKeys.Walk);
                trans.position = Vector3.MoveTowards(trans.position, newLoc, employeeLevel.move_speed * Time.deltaTime);
                if (Vector3.Distance(trans.position, newLoc) <= 0.01f) break;
                Vector3 dir = newLoc - trans.position;
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }


            animator.SetInteger("state", 0);
            animal.PlayAnimation(AnimationKeys.Idle);
          //  PlayAnim(animal.animationDic[animation_Idle_A], animation_Idle_A);
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
            falling = true;
            animator.SetInteger("state", 2);
            animal.PlayAnimation(AnimationKeys.Fly);
           // PlayAnim(animal.animationDic["Fly"], "Fly");
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

            busy = false;

            animator.SetInteger("state", 0);
            animal.PlayAnimation(AnimationKeys.Idle);
          //  PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
         
            if (packingTable.employee == this) packingTable.employee = null;
            employeeCallback?.Invoke(this);
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
   

    void OnFoodStackComplete(Food food, Vector3 targetPoint, FoodStack fs, AudioSource audio, int index, Transform target)
    {
        if (!fs.foodStack.Contains(food))
        {
            fs.foodStack.Push(food);
            food.transform.position = targetPoint;
            food.Setup(target, index);
           
        }
    }
    void OnGarbageStackComplete(Garbage garbage, Vector3 targetPoint, Stack<Garbage> gs, AudioSource audio, int index, Transform target)
    {
        if (!gs.Contains(garbage))
        {
            gs.Push(garbage);
            garbage.transform.position = targetPoint;
            garbage.Setup(target, index);
        }
    }

    void OnGarbageClearComplete(Garbage garbage, AudioSource audio)
    {
        GarbageManager.ClearGarbage(garbage);
    }


   // float coroutineTimer4 = 0;
 
    void Reward(Vector3 position)
    {
       
        Employee_Reward(position).Forget();

    }

    public void LevelUp()
    {
       
        employeeLevelData.level++;
        EXP = 0;
        employeeLevel = AssetLoader.employees_levels[employeeLevelData.level];
        //Animal animal = GetComponentInParent<Animal>();
       // Employee animalController = animal.GetComponentInChildren<Employee>();

       // GameInstance.GameIns.restaurantManager.UpgradePenguin(GameInstance.GameIns.restaurantManager.combineDatas.employeeData[animalController.id - 1].level, false, animalController);
        //SliderController sliderController = animal.GetComponentInChildren<SliderController>();
        // sliderController.
    }

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    Vector3 curNode;
    int animalMoveIndex;
   
    List<Vector3> moveLocations = new List<Vector3>();

    public void Dragged()
    {
        animal.audioSource.clip = GameIns.gameSoundManager.Quack();
        animal.audioSource.volume = 0.2f;
        animal.audioSource.Play();
        animator.SetInteger("state", 2);
        animal.PlayAnimation(AnimationKeys.Fly);
    }

    Coroutine draggedCoroutine;
    public void UnDragged()
    {
        falling = true;
     
        int i = 10000,j=10000;
       // Ray r = InputManger.cachingCamera.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(r, out RaycastHit hit, float.MaxValue))
        {
            Transform t = GameInstance.GameIns.inputManager.cameraRange;

            if (Utility.CheckHirable(t.position, ref i, ref j, false, true))
            {
                int num = InputManger.spawnDetects.Count;
                int rand = UnityEngine.Random.Range(0, num);

                if (num > 0)
                {
                    Vector3 selectedPosition = InputManger.spawnDetects[rand];
                    startPoint = trans.position;

                    endPoint = selectedPosition;// GameInstance.GetVector3(t.position.x, 0, t.position.z);
                    dir2 = (endPoint - startPoint).normalized;
                    //endPoint += dir2 * dirs.magnitude;
                    float distance = Vector3.Distance(startPoint, endPoint);
                    controlVector = (startPoint + endPoint) / 2 + Vector3.up * 10;
                    Falling(App.GlobalToken).Forget();
                }
                else
                {
                    Vector3 selectedPosition = Vector3.zero;
                    startPoint = trans.position;
                    endPoint = selectedPosition;
                    dir2 = (endPoint - startPoint).normalized;
                    float distance = Vector3.Distance(startPoint, endPoint);
                    controlVector = (startPoint + endPoint) / 2 + Vector3.up * 10;
                    Falling(App.GlobalToken).Forget();
                }
            }
            else
            {
                Vector3 selectedPosition = Vector3.zero;
                startPoint = trans.position;
                endPoint = selectedPosition;
                dir2 = (endPoint - startPoint).normalized;
                float distance = Vector3.Distance(startPoint, endPoint);
                controlVector = (startPoint + endPoint) / 2 + Vector3.up * 10;
                Falling(App.GlobalToken).Forget();
            }
        }
        /*  if(draggedCoroutine != null) StopCoroutine(draggedCoroutine);
          draggedCoroutine = StartCoroutine(Fall());*/
    }
    IEnumerator Fall()
    {
        float timer = 0;
      /*  Vector3 target = Vector3.zero;
        Ray r = InputManger.cachingCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(r, out RaycastHit hit, float.MaxValue, 1)) target = hit.point;*/
        while (timer <= 0.2f)
        {
         //   trans.position = Vector3.Lerp(trans.position, target, timer * 5);
            //float l = Mathf.Lerp(trans.position.y, 0, timer * 5);
            //trans.position = new Vector3(trans.position.x, l, trans.position.z);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
