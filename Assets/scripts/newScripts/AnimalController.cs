using Cysharp.Threading.Tasks;
using System;

//using Palmmedia.ReportGenerator.Core.Common;
using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;


//using TMPro.EditorUtilities;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static FoodMachine;
using AnimIns = AnimationInstancing.AnimationInstancing;
//using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
/// <summary>
/// using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
/// </summary>
//using static UnityEngine.GraphicsBuffer;

public class AnimalController : MonoBehaviour
{
    /*  public enum PlayType
      {
          Employee,
          Customer
      }*/
    /*  public enum AnimalType
      {
          Penguin,
          Cat
      }*/
    /* public enum RewardingType
     {
         None,
         Wait,
         Walk,
         Eat
     }
 */

    /*    public enum EmployeeState
        {
            Wait,
            Counter,
            Serving,
            FoodMachine,
            Table,
            TrashCan
        }
    */
    /* public enum CustomerState
     {
         Walk,
         Counter,
         Table
     }*/
    
    public SliderController ui;
    public Transform trans;// { get; set; }
    public Transform modelTrans;

    [HideInInspector]
    public PlayType playType;

    public Animal animal;// { get; set; }
    public Animator animator;
    public Transform headPoint;
    public Transform mousePoint;
 //   public int maxWeight = 3;
    public bool busy { get; set; }

    public AnimalData data;
    public List<AnimalData> animalDatas1 = new List<AnimalData>();
    //public FoodsAnimalsWant foodsAnimalsWant;

    public GameObject eatParticle;

    public AudioSource audioSource;
    public float coroutineTimer = 0;
#if HAS_ANIMATION_INSTANCING
    AnimIns animIns;
    public AnimIns animationInstancing { get; set; }
#endif
    public Shadow shadow;
    public const string animation_Idle_A = "Idle_A";
    public const string animation_Run = "Run";
    public const string animation_Fly = "Fly";
    public const string animation_LookAround = "lookAround";
    public const string animation_Peck = "Peck";
    public const string animation_Munch = "Munch";
    public const string animation_Bounce = "Bounce";
    //  private EmployeeData eData;
    //    public EmployeeData employeeData { get { return eData; } set { eData = value; if(ui != null) ui.UpdateLevel(eData.level); } }
    public int id;
    Stack<Node> stack = new Stack<Node>(50);
    public bool knockback = false;
    public Vector3 kockbackVector;
    public bool reCalculate = false;
    public bool bWait = false;
    [HideInInspector]
    public float waitTimer;

    public MinHeap<Node> openLists = new MinHeap<Node>(100);
    public HashSet<Node> closedLists = new HashSet<Node>(100);

    RestaurantManager restaurantManager;
    public RestaurantManager RestaurantMgr { get {
            if (restaurantManager == null) restaurantManager = GameInstance.GameIns.restaurantManager;
            return restaurantManager;
        }}
  
    public virtual void SetDefault()
    {
        animator = null;
        trans = null;
        animal = null;
        modelTrans = null;
        mousePoint = null;
        headPoint = null;
      //  shadow = null;
        animationInstancing = null;
    }
   /* public void PlayAnim(int id, string name)
    {
#if HAS_ANIMATION_INSTANCING
        if (animationInstancing.GetCurrentAnimationInfo() != null)
        {
            if (animationInstancing.GetCurrentAnimationInfo().animationName != name)
            {
                animationInstancing.PlayAnimation(id);
            }
        }
        else
        {
            animationInstancing.PlayAnimation(id);
        }
#endif
    }*/
   
 /*   public void Setup(FoodsAnimalsWant foodsAnimals)
    {
        if (foodsAnimals.spawnerType == AnimalSpawner.SpawnerType.Delivery)
        {
            GameInstance GameInstance = new GameInstance();
            WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager; 
            for(int i=0; i<workSpaceManager.counters.Count; i++)
            {
                if(workSpaceManager.counters[i].counterType == Counter.CounterType.Delivery)
                {
                    workSpaceManager.counters[i].customer = this;
                    transform.position = workSpaceManager.counters[i].transform.position;
                    FoodStack foodStack = new FoodStack();
                    foodStack.needFoodNum = 8;
                    foodStack.type = FoodMachine.MachineType.PackingTable;
                    foodStacks.Add(foodStack);
                    GameInstance.uiManager.UpdateOrder(this, Counter.CounterType.Delivery);
                }
            }
        }
        else
        {
            foodsAnimalsWant = foodsAnimals;
            if (foodsAnimals.burger)
            {
                FoodStack foodStack = new FoodStack();
                foodStack.needFoodNum = Random.Range(4, 8);
                //foodStack.needFoodNum = 6;
                foodStack.type = FoodMachine.MachineType.BurgerMachine;
                foodStacks.Add(foodStack);
            }
            if (foodsAnimals.coke)
            {
                FoodStack foodStack = new FoodStack();
                foodStack.needFoodNum = Random.Range(4, 8);
                //foodStack.needFoodNum = 5;
                foodStack.type = FoodMachine.MachineType.CokeMachine;
                foodStacks.Add(foodStack);
            }
            if (foodsAnimals.coffee)
            {
                FoodStack foodStack = new FoodStack();
                foodStack.needFoodNum = Random.Range(4, 8);
                foodStack.type = FoodMachine.MachineType.CoffeeMachine;
                foodStacks.Add(foodStack);
            }
            if (foodsAnimals.donut)
            {
                FoodStack foodStack = new FoodStack();
                foodStack.needFoodNum = Random.Range(4, 8);
                foodStack.type = FoodMachine.MachineType.DonutMachine;
                foodStacks.Add(foodStack);
            }

      
        }
    }*/

    // Update is called once per frame
    // public bool tests;
    /*  public void FindCustomerActions()
      {
          Vector3 target = new Vector3();
          WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;
          if (customerState != CustomerState.Table)
          {
              if (customerState == CustomerState.Walk)
              {
                  busy = true;
                  customerState = CustomerState.Counter;

                  CustomerPlayAction(workSpaceManager.counters[(int)foodsAnimalsWant.spawnerType].queuePoints, workSpaceManager.counters[(int)foodsAnimalsWant.spawnerType]);
                  return;
              }
              else if (customerState == CustomerState.Counter)
              {
                  List<Table> tableList = new List<Table>();
                  for (int i = 0; i < workSpaceManager.tables.Count; i++) tableList.Add(workSpaceManager.tables[i]);
                  tableList.Sort(delegate (Table a, Table b) { return (a.gameObject.transform.position - trans.position).magnitude.CompareTo((b.gameObject.transform.position - trans.position).magnitude); });
                  if (tableList.Count > 0)
                  {

                      foreach (Table t in tableList)
                      {
                          if (t.isDirty == false)
                          {
                              for (int i = 0; i < t.seats.Length; i++)
                              {
                                  if (t.seats[i].customer == null || t.seats[i].customer == this)
                                  {
                                      busy = true;
                                      customerState = CustomerState.Table;
                                      t.seats[i].customer = this;
                                      target = t.seats[i].transform.position;
                                      CustomerPlayAction(target, t, i);
                                      return;
                                  }
                              }
                          }
                      }
                  }
              }
              busy = true;
              CustomerPlayAction();
          }
          else
          {
              List<GameObject> endList = new List<GameObject>();
              for (int i = 0; i < workSpaceManager.endPoint.Count; i++) endList.Add(workSpaceManager.endPoint[i]);
              endList.Sort(delegate (GameObject a, GameObject b) { return (a.transform.position - trans.position).magnitude.CompareTo((b.transform.position - trans.position).magnitude); });
              busy = true;
              CustomerPlayAction(endList[0].transform.position);
          }
      }*/
    /* public void CustomerPlayAction()
     {
         StartCoroutine(CustomerWait());
     }*/
    /*  public void CustomerPlayAction(QueuePoint[] position, Counter counter)
      {
          List<Table> tableList = new List<Table>();
       //   for (int i = 0; i < GameInstance.GameIns.workSpaceManager.tables.Count; i++) tableList.Add(GameInstance.GameIns.workSpaceManager.tables[i]);
          StartCoroutine(CustomerWalkToCounter(position, counter));
      }*/

    /*   public void CustomerPlayAction(Vector3 position, Table table, int index)
       {
           Coord coord = CalculateCoords(position);
           if (coord != null)
           {
               if (coord.r == 100 && coord.c == 100) coord = null;
               StartCoroutine(CustomerWalkToTable(coord, table, index));
           }
           else
           {
               table.seats[index].customer = null;
               StartCoroutine(CustomerWait());
           }
       }*/

    /* public void CustomerPlayAction(Vector3 position)
     {
         Coord coord = CalculateCoords(position);
         if(coord!=null)
         {
             if (coord.r == 100 && coord.c == 100) coord = null;
             StartCoroutine(CustomerGoHome(coord));
         }
         else
         {
             StartCoroutine(CustomerWait());
         }
     }*/

    /*  IEnumerator CustomerWait()
      {
          yield return new WaitForSeconds(1f);
          busy = false;
      }*/


    //손님이 카운터로 가서 음식을 주문하고 받음
    /* IEnumerator CustomerWalkToCounter(QueuePoint[] position, Counter counter)
     {
         int i = position.Length - 1;

         while (i >= 0)
         {
             Coord coord;
             while (true)
             {
                 if (position[i].controller == null || position[i].controller == this)
                 {
                     if (i + 1 < position.Length) position[i + 1].controller = null;
                     position[i].controller = this;

                     coord = CalculateCoords(position[i].gameObject.transform.position);
                     yield return StartCoroutine(AnimalMovement(coord));
                     break;
                 }
                 yield return null;
                 animator.SetInteger("state", 0);
             }

             if (i == 0)
             {
                 counter.customer = this;
                 animator.SetInteger("state", 0);
                 GameInstance.GameIns.uiManager.UpdateOrder(this, counter.counterType);
             }
             i--;
             yield return null;
         }


         while (true)
         {
             //주문량을 충족함

             bool check = true;
             for (int j = 0; j < foodStacks.Count; j++)
             {
                 if (foodStacks[j].needFoodNum != foodStacks[j].foodStack.Count)
                 {
                     check = false;
                     break;
                 }
             }
             if (check) break;
             yield return null;
         }

         float foodPrices = 0;
         int tipNum = 0;
         for(int j=0; j<foodStacks.Count; j++)
         {
             for(int k = 0; k< foodStacks[j].foodStack.Count;k++)
             {
                 foodPrices += foodStacks[j].foodStack[k].foodPrice;
                 int tip = Random.Range(1, 11);
                 if (tip == 1) tipNum++;
             }
         }

         /////////돈을 표현 할 에셋 추가////////


         GameInstance.GameIns.restaurantManager.playerData.money += foodPrices;
         GameInstance.GameIns.restaurantManager.playerData.fishesNum += tipNum;
         GameInstance.GameIns.uiManager.UpdateMoneyText(GameInstance.GameIns.restaurantManager.playerData.money);

         SaveLoadManager.Save(SaveLoadManager.SaveState.ONLY_SAVE_PLAYERDATA);

         ///////////////////////////////////////
         List<Table> tables = GameInstance.GameIns.workSpaceManager.tables;

         while (true)
         {
             yield return new WaitForSeconds(0.5f);

             foreach (Table t in tables)
             {
                 if (t.isDirty == false)
                 {
                     for (int j = 0; j < t.seats.Length; j++)
                     {
                         if ((t.seats[j].customer == null || t.seats[j].customer == this))
                         {
                             counter.customer = null;
                             position[0].controller = null;

                             busy = false;
                             yield break;
                         }
                     }
                 }
             }
         }
     }*/

    //손님이 테이블로 가서 음식을 먹음
    /*  IEnumerator CustomerWalkToTable(Coord coord, Table table, int index)
      {
          yield return StartCoroutine(AnimalMovement(coord));

          modelTrans.rotation = table.seats[index].transform.rotation;

          for (int i = 0; i < foodStacks.Count; i++)
          {
              while (foodStacks[i].foodStack.Count > 0)
              {
                  Food f = foodStacks[i].foodStack[foodStacks[i].foodStack.Count - 1];
                  foodStacks[i].foodStack.Remove(f);
                  float r = Random.Range(1, 2.5f);

                  if (table.foodStacks.Count > 0)
                  {
                      bool check = false;
                      for (int j = 0; j < table.foodStacks.Count; j++)
                      {
                          if (table.foodStacks[j].type == foodStacks[i].type)
                          {
                              check = true;
                          }
                      }
                      if (check == false)
                      {
                          FoodStack foodStack = new FoodStack();
                          foodStack.type = foodStacks[i].type;
                          table.foodStacks.Add(foodStack);
                      }
                  }
                  else
                  {
                      FoodStack foodStack = new FoodStack();
                      foodStack.type = foodStacks[i].type;
                      table.foodStacks.Add(foodStack);
                  }
                  for (int j = 0; j < table.foodStacks.Count; j++)
                  {
                      if (table.foodStacks[j].type == foodStacks[i].type)
                      {
                          f.transform.DOJump(table.transform.position + new Vector3(0, 0.7f * table.foodStacks[j].foodStack.Count, 0), r, 1, 0.2f);
                          table.foodStacks[j].foodStack.Add(f);
                          audioSource.Play();
                      }
                  }

                  yield return new WaitForSeconds(0.1f);
              }
          }

          for (int i = foodStacks.Count - 1; i >= 0; i--)
          {
              if (foodStacks[i].foodStack.Count == 0)
              {
                  foodStacks.RemoveAt(i);
              }
          }

          for (int i = 0; i < table.foodStacks.Count; i++)
          {

              while (table.foodStacks.Count > i)
              {

                  if (table.foodStacks[i].foodStack.Count <= 0) break;
                  data.animal_eating_speed = 0.417f;
                  animator.SetInteger("state", 2);

                  GameObject particle = ParticleManager.CreateParticle();
                  particle.gameObject.transform.position = mousePoint.position;
                  particle.GetComponent<ParticleSystem>().Play();

                  yield return new WaitForSeconds(data.animal_eating_speed);


                  if (table.foodStacks.Count <= i) break;
                  int foodIndex = table.foodStacks[i].foodStack.Count - 1;
                  if (foodIndex < 0) break;
                  Food f = table.foodStacks[i].foodStack[foodIndex];
                  table.foodStacks[i].foodStack.Remove(f);
                  table.numberOfGarbage++;
                  FoodManager.EatFood(f);

                  ParticleManager.ClearParticle(particle);
                  animator.SetInteger("state", 0);

                  yield return new WaitForSeconds(5f);
              }
          }


          for (int i = table.foodStacks.Count - 1; i >= 0; i--)
          {
              if (table.foodStacks[i].foodStack.Count == 0)
              {
                  table.foodStacks.RemoveAt(i);
              }
          }
          animator.SetInteger("state", 0);

          yield return new WaitForSeconds(0.5f);


          //GameObject go = Instantiate(table.garbage);
          ////////쓰레기를 표현 할 에셋 추가, Garbage클래스 오브젝트 풀링////////
          table.seats[index].customer = null;

          bool bCustomerExist = true;
          for (int i = 0; i < table.seats.Length; i++)
          {
              if (table.seats[i].customer != null) bCustomerExist = false;
          }
          if (bCustomerExist)
          {
              int n = table.numberOfGarbage;
              table.isDirty = true;

              for (int i = 0; i < n; i++)
              {
                  Garbage go = GarbageManager.CreateGarbage();
                  go.transform.SetParent(table.trashPlate.transform);
                  table.garbageList.Add(go);
                  float x = Random.Range(-1f, 1f);
                  float z = Random.Range(-1f, 1f);

                  go.transform.position = table.up.position + new Vector3(x, 0, z);
              }

          }
          //////////////////////////////////////////////////////////////////////
          busy = false;
      }*/

    //손님이 집으로 감
    /*  IEnumerator CustomerGoHome(Coord coord)
      {
          yield return StartCoroutine(AnimalMovement(coord));
          GameInstance.GameIns.animalManager.DeSpawnAnimal(this, playType);
      }*/

    /* public void FindEmployeeWorks()
     {
         Vector3 target = new Vector3();
         WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;

         if (employeeState != EmployeeState.TrashCan)
         {
             bool isPackaged = false;
             int foodNum = 0;
             for (int i = 0; i < foodStacks.Count; i++)
             {
                 foodNum += foodStacks[i].foodStack.Count;
                 isPackaged = foodStacks[i].packaged;
             }

             if (foodNum > 0)
             {
                 if (!isPackaged)
                 {
                     //음식을 카운터로
                     List<FoodStack> stacks = new List<FoodStack>();
                     List<Counter> counterList = new List<Counter>();
                     for (int i = 0; i < workSpaceManager.counters.Count; i++)
                     {
                         for (int j = 0; j < foodStacks.Count; j++)
                         {
                             for (int k = 0; k < workSpaceManager.counters[i].foodStacks.Count; k++)
                             {
                                 if (foodStacks[j].type == workSpaceManager.counters[i].foodStacks[k].type)
                                 {
                                     counterList.Add(workSpaceManager.counters[i]);
                                 }
                             }
                         }
                     }
                     for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
                     {
                         for (int j = 0; j < foodStacks.Count; j++)
                         {
                             for (int k = 0; k < workSpaceManager.packingTables[i].foodStacks.Count; k++)
                             {
                                 if (foodStacks[j].type == workSpaceManager.packingTables[i].foodStacks[k].type)
                                 {
                                     counterList.Add(workSpaceManager.packingTables[i]);
                                 }
                             }
                         }
                     }

                     counterList.Sort(delegate (Counter a, Counter b)
                     {

                         int aa = 0;
                         int bb = 0;
                         for (int i = 0; i < a.foodStacks.Count; i++)
                         {
                             for (int k = 0; k < foodStacks.Count; k++)
                             {
                                 if (a.foodStacks[i].type == foodStacks[k].type)
                                 {
                                     aa = i;

                                 }
                             }

                         }

                         for (int i = 0; i < b.foodStacks.Count; i++)
                         {
                             for (int k = 0; k < foodStacks.Count; k++)
                             {
                                 if (b.foodStacks[i].type == foodStacks[k].type)
                                 {
                                     bb = i;

                                 }
                             }

                         }
                         return (a.foodStacks[aa].foodStack.Count.CompareTo(b.foodStacks[bb].foodStack.Count));
                     });

                     if (counterList.Count > 0)
                     {
                         foreach (Counter counter in counterList)
                         {
                             if (counter.counterType == Counter.CounterType.Delivery || counter.counterType == Counter.CounterType.None)
                             {
                                 if (counter.counterType == Counter.CounterType.None)
                                 {
                                     PackingTable pt = (PackingTable)counter;

                                     employeeState = EmployeeState.Counter;
                                     target = pt.workingSpot_SmallTables.position;

                                     Work(target, pt, 0);

                                     return;
                                 }
                             }
                             else
                             {
                                 employeeState = EmployeeState.Counter;
                                 target = counter.workingSpot_SmallTables.position;
                                 Work(target, counter, false);
                                 return;
                             }
                         }
                     }
                     //기다린다
                     employeeState = EmployeeState.Wait;
                     Work();
                     return;
                 }
                 else
                 {
                   *//*  for(int i=0; i<workSpaceManager.packingTables.Count; i++)
                     {
                         if (workSpaceManager.packingTables[i].counterType == Counter.CounterType.Delivery)
                         {
                             employeeState = EmployeeState.Serving;
                             target = workSpaceManager.packingTables[i].workingSpot_SmallTables.position;

                             Work(target, workSpaceManager.packingTables[i], 3);
                             return;

                         }
                     }*//*
                 }
                 //음식을 포장테이블로
               *//*  List<FoodStack> stacks2 = new List<FoodStack>();
                 List<PackingTable> counterList2 = new List<PackingTable>();
                 for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
                 {
                     for (int j = 0; j < foodStacks.Count; j++)
                     {
                         for (int k = 0; k < workSpaceManager.packingTables[i].foodStacks.Count; k++)
                         {
                             if (foodStacks[j].type == workSpaceManager.packingTables[i].foodStacks[k].type)
                             {
                                 counterList2.Add(workSpaceManager.packingTables[i]);
                             }
                         }
                     }
                 }

                 if (counterList2.Count > 0)
                 {
                     employeeState = EmployeeState.Counter;
                     target = counterList2[0].workingSpot_SmallTables.position;
                     Work(target, counterList2[0], 0);
                     return;
                 }
 *//*
             }
             else
             {
                 if(reward != null)
                 {
                     if (reward.foods.Count > 0)
                     {
                         Reward(reward.transform.position);
                         return;
                     }
                 }

                 //음식을 서빙
                 List<Counter> counterList = new List<Counter>();
                 for (int i = 0; i < workSpaceManager.counters.Count; i++) counterList.Add(workSpaceManager.counters[i]);
                 counterList.Sort(delegate (Counter a, Counter b) { return (a.gameObject.transform.position - trans.position).magnitude.CompareTo((b.gameObject.transform.position - trans.position).magnitude); });
                 if (counterList.Count > 0)
                 {
                     foreach (Counter counter in counterList)
                     {
                         if (counter.customer && (counter.employee == null || counter.employee == this))
                         {
                             for (int i = 0; i < counter.customer.foodStacks.Count; i++)
                             {
                                 for (int j = 0; j < counter.foodStacks.Count; j++)
                                 {
                                     if (counter.customer.foodStacks[i].type == counter.foodStacks[j].type && counter.customer.foodStacks[i].foodStack.Count < counter.customer.foodStacks[i].needFoodNum && counter.foodStacks[j].foodStack.Count > 0)
                                     {
                                         counter.employee = this;
                                         employeeState = EmployeeState.Serving;
                                         target = counter.workingSpot.position;

                                         Work(target, counter, true);
                                         return;
                                     }
                                 }
                             }
                         }
                     }
                 }

                 //배달
                 List<PackingTable> packingTables = new List<PackingTable>();
                 for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
                 {
                     if (workSpaceManager.packingTables[i].counterType == Counter.CounterType.Delivery && (workSpaceManager.packingTables[i].employee == null || workSpaceManager.packingTables[i].employee == this) && workSpaceManager.packingTables[i].customer != null)
                     {
                         if (workSpaceManager.packingTables[i].packageStack.foodStack.Count >= workSpaceManager.packingTables[i].customer.foodStacks[0].needFoodNum)
                         {
                             workSpaceManager.packingTables[i].employee = this;
                             Work(workSpaceManager.packingTables[i].workingSpot.position, workSpaceManager.packingTables[i], 3);
                             return;
                         }
                     }

                 }

                 //테이블을 청소
                 List<Table> tableList = new List<Table>();
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
                             Work(target, table);
                             return;
                         }
                     }
                 }

                 // 손님 우선 조달

                 List<FoodMachine> finalMachineList = new List<FoodMachine>();
                 List<FoodStack> foodStacks = new List<FoodStack>();
                 for (int i = 0; i < workSpaceManager.counters.Count; i++)
                 {
                     for (int j = 0; j < workSpaceManager.counters[i].foodStacks.Count; j++) foodStacks.Add(workSpaceManager.counters[i].foodStacks[j]);
                 }

                 for (int i = 0; i < workSpaceManager.counters.Count; i++)
                 {
                     if (workSpaceManager.counters[i].customer != null)
                     {
                         AnimalController customer = workSpaceManager.counters[i].customer;
                         //if(customer.food1 > customer.foodList.Count)
                         for (int j = 0; j < customer.foodStacks.Count; j++)
                         {
                             for (int k = 0; k < workSpaceManager.foodMachines.Count; k++)
                             {
                                 if (customer.foodStacks[j].type == workSpaceManager.foodMachines[k].machineType && workSpaceManager.foodMachines[k].foodStack.foodStack.Count + customer.foodStacks[j].foodStack.Count >= customer.foodStacks[j].needFoodNum && (workSpaceManager.foodMachines[k].employee == null || workSpaceManager.foodMachines[k].employee == this) && customer.foodStacks[j].needFoodNum > customer.foodStacks[j].foodStack.Count)
                                 {
                                     finalMachineList.Add(workSpaceManager.foodMachines[k]);
                                 }
                             }

                             if (finalMachineList.Count > 0)
                             {
                                 finalMachineList.Sort(delegate (FoodMachine a, FoodMachine b) { return (a.gameObject.transform.position - trans.position).magnitude.CompareTo((a.gameObject.transform.position - trans.position).magnitude); });
                                 finalMachineList[0].employee = this;
                                 employeeState = EmployeeState.FoodMachine;
                                 target = finalMachineList[0].workingSpot.position;
                                 Work(target, finalMachineList[0]);
                                 return;
                             }
                         }
                     }
                 }


                 //포장된 음식을 배달 테이블로 이동
                 for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
                 {
                     if (workSpaceManager.packingTables[i].counterType == Counter.CounterType.None && workSpaceManager.packingTables[i].packageStack.foodStack.Count > 0 && (workSpaceManager.packingTables[i].employeeAssistant == null || workSpaceManager.packingTables[i].employeeAssistant == this))
                     {
                         for (int j = 0; i < workSpaceManager.packingTables.Count; j++)
                         {
                             if(i != j && workSpaceManager.packingTables[j].counterType == Counter.CounterType.Delivery)
                             {
                                 workSpaceManager.packingTables[i].employeeAssistant = this;
                                 employeeState = EmployeeState.FoodMachine;
                                 target = workSpaceManager.packingTables[i].endTrans.position;
                                 Work(target, workSpaceManager.packingTables[i], 2);
                                 return;
                             }
                         }
                     }
                 }



                 //포장테이블 포장

                 List<FoodStack> newFoodStacks = new List<FoodStack>();
                 int n = 0;
                 for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
                 {
                     if ((workSpaceManager.packingTables[i].employee == null || workSpaceManager.packingTables[i].employee == this) && workSpaceManager.packingTables[i].counterType == Counter.CounterType.None)
                     {
                         n += workSpaceManager.packingTables[i].packingNumber;
                         for (int j = 0; j < workSpaceManager.packingTables[i].foodStacks.Count; j++)
                         {
                             newFoodStacks.Add(workSpaceManager.packingTables[i].foodStacks[j]);
                         }
                     }
                 }


                 for (int i = 0; i < newFoodStacks.Count; i++) n += newFoodStacks[i].foodStack.Count;

                 if (n >= 4)
                 {


                     newFoodStacks.Sort(delegate (FoodStack a, FoodStack b) { return a.foodStack.Count.CompareTo(b.foodStack.Count); });


                     for (int j = 0; j < newFoodStacks.Count; j++)
                     {
                         for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
                         {
                             for (int k = 0; k < workSpaceManager.packingTables[i].foodStacks.Count; k++)
                             {
                                 if (newFoodStacks[j].type == workSpaceManager.packingTables[i].foodStacks[k].type && (workSpaceManager.packingTables[i].employee == null || workSpaceManager.packingTables[i].employee == this))
                                 {
                                     workSpaceManager.packingTables[i].employee = this;
                                     employeeState = EmployeeState.FoodMachine;
                                     target = workSpaceManager.packingTables[i].packingTrans.position;
                                     Work(target, workSpaceManager.packingTables[i], 1);

                                     return;
                                 }
                             }
                         }
                     }
                 }







                 //일반 조달
                 foodStacks.Clear();
                 for (int i=0;i<workSpaceManager.packingTables.Count;i++)
                 {
                     for (int j = 0; j < workSpaceManager.packingTables[i].foodStacks.Count; j++)
                     {
                         foodStacks.Add(workSpaceManager.packingTables[i].foodStacks[j]);
                     }
                 }

                 for(int i=0; i<workSpaceManager.counters.Count; i++)
                 {
                     for(int j=0; j < workSpaceManager.counters[i].foodStacks.Count; j++)
                     {
                         foodStacks.Add(workSpaceManager.counters[i].foodStacks[j]);
                     }
                 }


                 foodStacks.Sort(delegate (FoodStack a, FoodStack b) { return (a.foodStack.Count.CompareTo(b.foodStack.Count)); });

                 List<FoodMachine> foodMachineList = new List<FoodMachine>();
                 for (int i = 0; i < workSpaceManager.foodMachines.Count; i++) foodMachineList.Add(workSpaceManager.foodMachines[i]);

                 finalMachineList.Clear();


                 for (int i = 0; i < foodStacks.Count; i++)
                 {
                     for (int j = 0; j < foodMachineList.Count; j++)
                     {
                         if (foodMachineList[j].machineType == foodStacks[i].type && foodMachineList[j].foodStack.foodStack.Count > 0 && (foodMachineList[j].employee == null || foodMachineList[j].employee == this))
                         {
                             finalMachineList.Add(foodMachineList[j]);
                         }
                     }
                     if (finalMachineList.Count > 0) break;
                 }
                 finalMachineList.Sort(delegate (FoodMachine a, FoodMachine b) { return a.foodStack.foodStack.Count.CompareTo((b.foodStack.foodStack.Count)); });
                 finalMachineList.Reverse();
                 if (finalMachineList.Count > 0)
                 {
                     foreach (FoodMachine machine in finalMachineList)
                     {
                         if (machine.foodStack.foodStack.Count > 0 && (machine.employee == null || machine.employee == this))
                         {
                             machine.employee = this;
                             employeeState = EmployeeState.FoodMachine;
                             target = machine.workingSpot.position;
                             Work(target, machine);
                             return;
                         }
                     }
                 }




                 *//*if (workSpaceManager.packingTables[i].foodStack[j].f
              * oodStack.Count >= 8)
             {
                 employeeState = EmployeeState.FoodMachine;
                 target = workSpaceManager.packingTables[i].packingTrans.position;
                 Work(target, workSpaceManager.packingTables[i], 1);
                 return;
             }*/
    /*  foodStacks.Clear();

      for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
      {
          for (int j = 0; j < workSpaceManager.packingTables[i].foodStack.Count; j++)
          {
              foodStacks.Add(workSpaceManager.packingTables[i].foodStack[j]);


          }
      }
      foodStacks.Sort(delegate (FoodStack a, FoodStack b) { return a.foodStack.Count.CompareTo(b.foodStack.Count); });

      //포장테이블 음식 조달

      for (int j = 0; j < foodStacks.Count; j++)
      {
          for (int i = 0; i < workSpaceManager.foodMachines.Count; i++)
          {

                  if (foodStacks[j].type == workSpaceManager.foodMachines[i].foodStack.type)
                  {
                      employeeState = EmployeeState.FoodMachine;
                      target = workSpaceManager.foodMachines[i].workingSpot.position;
                      Work(target, workSpaceManager.foodMachines[i]);
                      return;
                  }

          }
      }*/

    /*  for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
      {
          for (int j = 0; j < workSpaceManager.packingTables[i].foodStack.Count; j++)
          {
              for (int k = 0; k < workSpaceManager.foodMachines.Count; k++)
              {
                  if (workSpaceManager.packingTables[i].foodStack[j].type == workSpaceManager.foodMachines[k].machineType)
                  {
                      FoodMachine machine = workSpaceManager.foodMachines[k];
                      machine.employee = this;
                      employeeState = EmployeeState.FoodMachine;
                      target = machine.workingSpot.position;
                      Work(target, machine);
                      return;
                  }

              }
          }
      }
*//*


                //workSpaceManager.foodMachines

                //foodStacks.

                //기다리기
                employeeState = EmployeeState.Wait;
                Work();
            }
        }
        else
        {
            //쓰레기를 버림
            List<TrashCan> trashCanList = new List<TrashCan>();
            for (int i = 0; i < workSpaceManager.trashCans.Count; i++) trashCanList.Add(workSpaceManager.trashCans[i]);
            trashCanList.Sort(delegate (TrashCan a, TrashCan b) { return (a.gameObject.transform.position - trans.position).magnitude.CompareTo((b.gameObject.transform.position - trans.position).magnitude); });

            foreach (TrashCan c in trashCanList)
            {

                Work(c.throwPos.position, c);
                return;
            }
        }
    }*/
    /* public void Work()
     {
         StartCoroutine(EmployeeWait());
     }*/
    /*   public void Work(Vector3 position, PackingTable packingTable, int i)
       {
           Coord coord = CalculateCoords(position);
           if(coord != null)
           {
               if (coord.r == 100 && coord.c == 100) coord = null;

               if(i==0) StartCoroutine(EmployeePackingTable(coord, packingTable));
               else if(i==1) StartCoroutine(EmployeeNowPacking(coord, packingTable));
               else if(i==2) StartCoroutine(EmployeeGetPackagedFoods(coord, packingTable));
               else if(i==3) StartCoroutine(EmployeeDeliveryFoods(coord, packingTable));
           }
           else
           {
               if (i == 1 || i == 3) packingTable.employee = null;
               StartCoroutine(EmployeeWait());
           }
       }*/

    /*   public void Work(Vector3 position, FoodMachine foodMachine)
       {
           Coord coord = CalculateCoords(position);
           if(coord != null)
           {
               if (coord.r == 100 && coord.c == 100) coord = null;
               StartCoroutine(EmployeeFoodMachine(coord, foodMachine));
           }
           else
           {
               foodMachine.employee = null;
               StartCoroutine(EmployeeWait());
           }
       }*/
    /* public void Work(Vector3 position, Counter counter, bool serving)
     {
         Coord coord = CalculateCoords(position);
         if(coord != null)
         {
             if (coord.r == 100 && coord.c == 100) coord = null;

             if (!serving) StartCoroutine(EmployeeCounter(coord, counter));
             else StartCoroutine(EmployeeServing(coord, counter));
         }
         else
         {
             if (serving) counter.employee = null;
             StartCoroutine(EmployeeWait());
         }
     }*/
    /* public void Work(Vector3 position, Table table)
     {
         Coord coord = CalculateCoords(position);
         if (coord != null)
         {
             if (coord.r == 100 && coord.c == 100) coord = null;
             else StartCoroutine(EmployeeTable(coord, table));
         }
         else
         {
             table.employeeContoller = null;
             StartCoroutine(EmployeeWait());
         }
     }*/
    /* public void Work(Vector3 position, TrashCan trash)
     {
         Coord coord = CalculateCoords(position);
         if (coord != null)
         {
             if (coord.r == 100 && coord.c == 100) coord = null;
             StartCoroutine(EmployeeTrashCan(coord, trash));
         }
         else
         {
             StartCoroutine(EmployeeWait());
         }

     }*/

    /*  IEnumerator EmployeeWait()
      {
          animator.SetInteger("state", 2);
          yield return new WaitForSeconds(1f);
          busy = false;
      }*/

    //제조된 음식 가져가기
    /*   IEnumerator EmployeeFoodMachine(Coord coord, FoodMachine foodMachine)
       {
           yield return StartCoroutine(AnimalMovement(coord));

           modelTrans.rotation = foodMachine.workingSpot.rotation;

           while (foodMachine.foodStack.foodStack.Count > 0)
           {
               int weight = 0;
               for (int i = 0; i < foodStacks.Count; i++) { weight += foodStacks[i].foodStack.Count; }
               if (weight > maxWeight) break;
               Food f = foodMachine.foodStack.foodStack[foodMachine.foodStack.foodStack.Count - 1];
               foodMachine.foodStack.foodStack.Remove(f);
               float r = Random.Range(1, 2.5f);
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
                   FoodStack foodStack = new FoodStack();
                   foodStack.type = f.parentType;
                   foodStacks.Add(foodStack);
               }

               int n = 0;
               for (n = 0; n < foodStacks.Count; n++)
               {
                   if (foodStacks[n].type == f.parentType)
                   {

                       break;
                   }
               }
               FoodStack fs = foodStacks[n];                       


               f.transform.DOJump(headPoint.position + new Vector3(0, 0.7f * fs.foodStack.Count, 0), r, 1, 0.2f).OnComplete(() =>
               {
                   f.transform.position = headPoint.position + new Vector3(0, 0.7f * fs.foodStack.Count, 0);
                   //    foodList.Add(f);
                   audioSource.Play();

                   fs.foodStack.Add(f);
               });

               yield return new WaitForSeconds(0.1f);
           }
           yield return new WaitForSeconds(0.5f);
           foodMachine.employee = null;
           busy = false;
       }*/

    //음식 카운터로 조달 하기
    /*IEnumerator EmployeeCounter(Coord coord, Counter counter)
    {
        yield return StartCoroutine(AnimalMovement(coord));

        modelTrans.rotation = counter.workingSpot_SmallTables.rotation;

        for (int i = 0; i < foodStacks.Count; i++)
        {
            for (int j = 0; j < counter.foodStacks.Count; j++)
            {
                if (foodStacks[i].type == counter.foodStacks[j].type)
                {
                    while (foodStacks[i].foodStack.Count > 0)
                    {
                        Food f = foodStacks[i].foodStack[foodStacks[i].foodStack.Count - 1];
                        foodStacks[i].foodStack.Remove(f);
                        float r = Random.Range(1, 2.5f);
                        f.transform.DOJump(counter.stackPoints[j].position + new Vector3(0, 0.7f * counter.foodStacks[j].foodStack.Count, 0), r, 1, 0.2f);
                        counter.foodStacks[j].foodStack.Add(f);
                        audioSource.Play();
                        yield return new WaitForSeconds(0.2f);
                    }
                }
            }
        }

        for (int i = foodStacks.Count - 1; i >= 0; i--)
        {
            if (foodStacks[i].foodStack.Count == 0)
            {
                foodStacks.RemoveAt(i);
            }
        }

        yield return new WaitForSeconds(0.3f);
        busy = false;
    }*/

    /* IEnumerator EmployeePackingTable(Coord coord, PackingTable packingTable)
     {
         yield return StartCoroutine(AnimalMovement(coord));

         modelTrans.rotation = packingTable.workingSpot_SmallTables.rotation;

         for (int i = 0; i < foodStacks.Count; i++)
         {
             for (int j = 0; j < packingTable.foodStacks.Count; j++)
             {
                 if (foodStacks[i].type == packingTable.foodStacks[j].type)
                 {
                     while (foodStacks[i].foodStack.Count > 0)
                     {

                         Food f = foodStacks[i].foodStack[foodStacks[i].foodStack.Count - 1];
                         foodStacks[i].foodStack.Remove(f);
                         float r = Random.Range(1, 2.5f);
                         f.transform.DOJump(packingTable.stackPoints[j].position + new Vector3(0, 0.7f * packingTable.foodStacks[j].foodStack.Count, 0), r, 1, 0.2f);
                         packingTable.foodStacks[j].foodStack.Add(f);
                         audioSource.Play();
                         yield return new WaitForSeconds(0.2f);

                     }
                 }
             }
         }

         for (int i = foodStacks.Count - 1; i >= 0; i--)
         {
             if (foodStacks[i].foodStack.Count == 0)
             {
                 foodStacks.RemoveAt(i);
             }
         }

         yield return new WaitForSeconds(0.3f);
         busy = false;
     }
 */
    /* IEnumerator EmployeeNowPacking(Coord coord, PackingTable packingTable)
     {
         yield return StartCoroutine(AnimalMovement(coord));

         modelTrans.rotation = packingTable.packingTrans.rotation;

         HamburgerPackaging hamburgerPackaging = packingTable.GetComponent<HamburgerPackaging>();



         int num = 0;
         foreach (FoodStack p in packingTable.foodStacks)
         {
             num += p.foodStack.Count;
         }

         List<FoodStack> list = new List<FoodStack>();
         while(num > 0)
         {
             for(int i=0; i<packingTable.foodStacks.Count; i++)
             {
                 list.Add(packingTable.foodStacks[i]);
             }
             list.Sort(delegate (FoodStack a, FoodStack b) { return a.foodStack.Count.CompareTo(b.foodStack.Count); });
             list.Reverse();
             Food f = list[0].foodStack[list[0].foodStack.Count - 1];

             list[0].foodStack.Remove(f);

             yield return hamburgerPackaging.MoveFood(f);
             yield return new WaitForSeconds(0.2f);
             num = 0;
             foreach (FoodStack p in packingTable.foodStacks)
             {
                 num += p.foodStack.Count;
             }
         }


        *//* while(packingTable.f1.foodStack.Count > 0)
         {
             hamburgerPackaging.MoveHamburger();
             yield return new WaitForSeconds(0.5f);
         }*//*

         yield return new WaitForSeconds(0.3f);
         packingTable.employee = null;
         busy = false;
     }*/

    /*  IEnumerator EmployeeGetPackagedFoods(Coord coord, PackingTable packingTable)
      {
          yield return StartCoroutine(AnimalMovement(coord));

          modelTrans.rotation = packingTable.packingTrans.rotation;

          while (packingTable.packageStack.foodStack.Count > 0)
          {

              Food f = packingTable.packageStack.foodStack[packingTable.packageStack.foodStack.Count - 1];
              packingTable.packageStack.foodStack.Remove(f);
              float r = Random.Range(1, 2.5f);
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
                  FoodStack foodStack = new FoodStack();
                  foodStack.type = f.parentType;
                  foodStacks.Add(foodStack);
              }

              int n = 0;
              for (n = 0; n < foodStacks.Count; n++)
              {
                  if (foodStacks[n].type == f.parentType)
                  {

                      break;
                  }
              }

              FoodStack fs = foodStacks[n];
              f.transform.DOJump(headPoint.position + new Vector3(0, 0.7f * fs.foodStack.Count, 0), r, 1, 0.2f).OnComplete(() =>
              {
                  f.transform.position = headPoint.position + new Vector3(0, 0.7f * fs.foodStack.Count, 0);
                  //    foodList.Add(f);
                  audioSource.Play();

                  fs.foodStack.Add(f);
              });

              yield return new WaitForSeconds(0.1f);

          }

          GameInstance gameIns = new GameInstance();
          WorkSpaceManager workSpaceManager = gameIns.GameIns.workSpaceManager;

          for (int i = 0; i < workSpaceManager.packingTables.Count; i++) {
              if (workSpaceManager.packingTables[i].counterType == Counter.CounterType.Delivery)
              {

                  yield return StartCoroutine(AnimalMovement(CalculateCoords(workSpaceManager.packingTables[i].workingSpot_SmallTables.position)));




                  for (int j = 0; j < foodStacks.Count; j++)
                  {
                      if (foodStacks[j].type == FoodMachine.MachineType.PackingTable)
                      {
                          while (foodStacks[j].foodStack.Count > 0)
                          {
                              float r = Random.Range(1, 2.5f);
                              Food f = foodStacks[j].foodStack[foodStacks[j].foodStack.Count - 1];
                              foodStacks[j].foodStack.Remove(f);

                              f.transform.DOJump(workSpaceManager.packingTables[i].smallTable.position + new Vector3(0, 0.7f * workSpaceManager.packingTables[i].packageStack.foodStack.Count, 0), r, 1, 0.2f).OnComplete(() =>
                              {
                                  f.transform.position = workSpaceManager.packingTables[i].smallTable.position + new Vector3(0, 0.7f * workSpaceManager.packingTables[i].packageStack.foodStack.Count, 0);
                                  //    foodList.Add(f);
                                  audioSource.Play();

                                  workSpaceManager.packingTables[i].packageStack.foodStack.Add(f);
                              });

                              yield return new WaitForSeconds(0.1f);
                          }
                      }
                  }

                  for (int k = foodStacks.Count - 1; k >= 0; k--)
                  {
                      if (foodStacks[k].foodStack.Count == 0) foodStacks.RemoveAt(k);
                  }

                  //  yield return StartCoroutine(AnimalMovement(CalculateCoords()));
                  yield return new WaitForSeconds(0.3f);
                  packingTable.employeeAssistant = null;
                  busy = false;

                  break;
              }
          }

      }*/


    /*  IEnumerator EmployeeDeliveryFoods(Coord coord, PackingTable packingTable)
      {
          yield return StartCoroutine(AnimalMovement(coord));
          modelTrans.rotation = packingTable.workingSpot.rotation;
          int num = 0;
          int a = -1;
          while (packingTable.customer.foodStacks[0].needFoodNum > num)
          {
              Food food = packingTable.packageStack.foodStack[packingTable.packageStack.foodStack.Count - 1];
              packingTable.packageStack.foodStack.Remove(food);


              for (int i = 0; i < foodStacks.Count; i++)
              {
                  if (foodStacks[i].type == FoodMachine.MachineType.PackingTable)
                  {
                      a = i;
                  }
              }
              if (a == -1)
              {
                  FoodStack fs = new FoodStack();
                  fs.type = FoodMachine.MachineType.PackingTable;
                  foodStacks.Add(fs);
                  for (int i = 0; i < foodStacks.Count; i++)
                  {
                      if (foodStacks[i].type == FoodMachine.MachineType.PackingTable)
                      {
                          a = i;
                      }
                  }
              }


              float r = Random.Range(1, 2.5f);

              food.transform.DOJump(headPoint.position + new Vector3(0, 0.7f * foodStacks[a].foodStack.Count, 0), r, 1, 0.2f).OnComplete(() =>
              {
                  food.transform.position = headPoint.position + new Vector3(0, 0.7f * foodStacks[a].foodStack.Count, 0);
                  //    foodList.Add(f);
                  audioSource.Play();

                  foodStacks[0].foodStack.Add(food);
                  num++;

              });

              yield return new WaitForSeconds(0.2f);
          }

          //배달 준비 완료


          animator.SetInteger("state", 3);

          float timerY = 0;
          while (timerY <= 1.5f)
          {

              trans.Translate(Vector3.up * 6f * Time.deltaTime);

              timerY += Time.deltaTime;
              yield return null;
          }
          yield return new WaitForSeconds(0.4f);
          float timerXZ = 0;
          while (timerXZ <= 3f)
          {

              trans.Translate(modelTrans.forward * 10f * Time.deltaTime);

              timerXZ += Time.deltaTime;
              yield return null;
          }

          while (foodStacks[a].foodStack.Count > 0)
          {

              Food f = foodStacks[a].foodStack[foodStacks[a].foodStack.Count - 1];
              foodStacks[a].foodStack.Remove(f);
              packingTable.customer.foodStacks[0].foodStack.Add(f);
          }

          *//*     for(int i= foodStacks.Count - 1; i <= 0; i--)
               {
                   if (foodStacks[i].foodStack.Count == 0)
                   {
                       foodStacks.RemoveAt(i);
                   }
               }*//*

          packingTable.customer = null;
          packingTable.employee = null;
          yield return new WaitForSeconds(5f);
          float X = 0;
          float Z = 0;

          bool check = false;
          while (!check)
          {
              X = Random.Range(-18, 22);
              Z = Random.Range(-22, 21);
              if (Physics.CheckBox(new Vector3(X, 0, Z), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1 << 6 | 1 << 7 | 1 << 8))
              {
                  check = false;
              }
              else
              {
                  break;
              }

              yield return null;
          }


          Vector3 targetLoc = new Vector3(X, 0, Z);


          while (true)
          {
              Vector3 currnetLoc = new Vector3(trans.position.x, 0, trans.position.z);
              Vector3 dir = (targetLoc - currnetLoc).normalized;
              trans.Translate(dir * 7f * Time.deltaTime, Space.World);
              float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
              modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);

              if ((currnetLoc - targetLoc).magnitude < 0.5f) break;
              yield return null;
          }
          yield return new WaitForSeconds(0.4f);
          while (true)
          {
              trans.position = new Vector3(trans.position.x, trans.position.y - 6f * Time.deltaTime, trans.position.z);

              if (trans.position.y < 0)
              {
                  trans.position = new Vector3(trans.position.x, 0, trans.position.z);

                  break;
              }

              yield return null;
          }
          animator.SetInteger("state", 0);
          yield return new WaitForSeconds(0.5f);

          busy = false;
      }*/

    //음식 손님에게 서빙하기
    /* IEnumerator EmployeeServing(Coord coord, Counter counter)
     {
         yield return StartCoroutine(AnimalMovement(coord));

         modelTrans.rotation = counter.workingSpot.rotation;

         for (int i = 0; i < counter.customer.foodStacks.Count; i++)
         {
             for (int j = 0; j < counter.foodStacks.Count; j++)
             {
                 if (counter.customer.foodStacks[i].type == counter.foodStacks[j].type && counter.customer.foodStacks[i].foodStack.Count < counter.customer.foodStacks[i].needFoodNum && counter.foodStacks[j].foodStack.Count > 0)
                 {
                     while (counter.foodStacks[j].foodStack.Count > 0)
                     {
                         if (counter.customer.foodStacks[i].foodStack.Count >= counter.customer.foodStacks[i].needFoodNum) break;
                         Food f = counter.foodStacks[j].foodStack[counter.foodStacks[j].foodStack.Count - 1];
                         counter.foodStacks[j].foodStack.Remove(f);
                         float r = Random.Range(1, 2.5f);
                         f.transform.DOJump(counter.customer.headPoint.position + new Vector3(0, 0.7f * counter.customer.foodStacks[i].foodStack.Count, 0), r, 1, 0.2f).OnComplete(() =>
                         {
                             counter.customer.foodStacks[i].foodStack.Add(f);
                             audioSource.Play();
                             GameInstance.GameIns.uiManager.UpdateOrder(counter.customer, counter.counterType);
                         });

                         yield return new WaitForSeconds(0.5f);
                     }
                 }
             }
         }
         *//*
                 if (counter.customer.foodStacks[0].foodStack.Count < counter.customer.foodStacks[0].needFoodNum)
                 {
                     for (int j = 0; j < counter.foodStacks.Count; j++)
                     {
                         if (counter.customer.foodStacks[0].type == counter.foodStacks[j].type && counter.customer.foodStacks[0].foodStack.Count < counter.customer.foodStacks[0].needFoodNum && counter.foodStacks[j].foodStack.Count > 0)
                         {
                             while (counter.foodStacks[j].foodStack.Count > 0)
                             {
                                 if (counter.customer.foodStacks[0].foodStack.Count >= counter.customer.foodStacks[0].needFoodNum) break;
                                 Food f = counter.foodStacks[j].foodStack[counter.foodStacks[j].foodStack.Count - 1];
                                 counter.foodStacks[j].foodStack.Remove(f);
                                 float r = Random.Range(1, 2.5f);
                                 f.transform.DOJump(counter.customer.headPoint.position + new Vector3(0, 0.7f * counter.customer.foodStacks[0].foodStack.Count, 0), r, 1, 0.2f).OnComplete(() =>
                                 {
                                     counter.customer.foodStacks[0].foodStack.Add(f);
                                     audioSource.Play();
                                     GameInstance.GameIns.uiManager.UpdateOrder(counter.customer);
                                 });

                                 yield return new WaitForSeconds(0.5f);
                             }
                         }
                     }
                 }
                 else if(counter.customer.foodStacks[1].foodStack.Count < counter.customer.foodStacks[1].needFoodNum)
                 {
                     for (int j = 0; j < counter.foodStacks.Count; j++)
                     {
                         if (counter.customer.foodStacks[1].type == counter.foodStacks[j].type && counter.customer.foodStacks[1].foodStack.Count < counter.customer.foodStacks[1].needFoodNum && counter.foodStacks[j].foodStack.Count > 0)
                         {
                             while (counter.foodStacks[j].foodStack.Count > 0)
                             {
                                 if (counter.customer.foodStacks[1].foodStack.Count >= counter.customer.foodStacks[1].needFoodNum) break;
                                 Food f = counter.foodStacks[j].foodStack[counter.foodStacks[j].foodStack.Count - 1];
                                 counter.foodStacks[j].foodStack.Remove(f);
                                 float r = Random.Range(1, 2.5f);
                                 f.transform.DOJump(counter.customer.headPoint.position + new Vector3(0, 0.7f * counter.customer.foodStacks[1].foodStack.Count, 0), r, 1, 0.2f).OnComplete(() =>
                                 {
                                     counter.customer.foodStacks[1].foodStack.Add(f);
                                     audioSource.Play();
                                     GameInstance.GameIns.uiManager.UpdateOrder(counter.customer);
                                 });

                                 yield return new WaitForSeconds(0.5f);
                             }
                         }
                     }
                 }*//*
         yield return new WaitForSeconds(1f);
         counter.employee = null;
         busy = false;
     }*/

    // 테이블로 가서 쓰레기 줍기
    /* IEnumerator EmployeeTable(Coord coord, Table table)
     {

         yield return StartCoroutine(AnimalMovement(coord, false, table));
         if (!table.interacting)
         {
             // Garbage go = table.garbageInstance;
             while (table.numberOfGarbage > 0)
             {
                 float p = Random.Range(1, 2.5f);
                 Garbage go = table.garbageList[table.garbageList.Count - 1];//garbages.Pop();
                 table.garbageList.Remove(go);
                 table.numberOfGarbage--;
                 go.transform.DOJump(headPoint.position + new Vector3(0, 0.5f * garbageList.Count, 0), p, 1, 0.2f).OnComplete(() =>
                 {
                     audioSource.Play();
                     go.transform.position = headPoint.position + new Vector3(0, 0.5f * garbageList.Count, 0);
                     garbageList.Add(go);
                 });
             }
             yield return new WaitForSeconds(0.5f);
             table.isDirty = false;
             employeeState = EmployeeState.TrashCan;
             table.employeeContoller = null;
             busy = false;
         }
         else
         {

         }
     }*/

    //쓰레기 통에 가서 쓰레기 버리기
    /*IEnumerator EmployeeTrashCan(Coord coord, TrashCan trashCan)
    {
        yield return StartCoroutine(AnimalMovement(coord));

        while (garbageList.Count > 0)
        {
            Garbage garbage = garbageList[garbageList.Count - 1];
            garbageList.Remove(garbage);
            float r = Random.Range(1, 2.5f);
            garbage.transform.DOJump(trashCan.transform.position, r, 1, 0.2f).OnComplete(() =>
            {
                audioSource.Play();
                GarbageManager.ClearGarbage(garbage);
            });

            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        employeeState = EmployeeState.Wait;
        busy = false;

    }*/

    /* void Reward(Vector3 position)
     {
         Coord coord = CalculateCoords(position);
         if (coord != null)
         {
             if (coord.r == 100 && coord.c == 100) coord = null;

             StartCoroutine(EmployeeGetRewards(coord));
         }
         else
         {
             StartCoroutine(EmployeeWait());
         }

     }*/

    //직원이 상자로 가서 음식을 먹기 
    /* IEnumerator EmployeeGetRewards(Coord coord)
     {
         rewardingType = RewardingType.Walk;
         //이동
         yield return StartCoroutine(AnimalMovement(coord,false,null ,true));

         rewardingType = RewardingType.Eat;
         //행동
         while (reward.foods.Count > 0)
         {
             animator.SetInteger("state", 4); 
             yield return new WaitForSeconds(0.417f);

             Food go = reward.foods[reward.foods.Count - 1];

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
                 yield return new WaitForSeconds(0.5f);
             }
             else SaveLoadManager.EmployeeLevelSave(true);
            // yield return new WaitForSeconds(0.5f);
             animator.SetInteger("state", 0);
             yield return new WaitForSeconds(0.2f);
         }

         reward.ClearFishes();   //상자 치우기
        // GameInstance.GameIns.inputManager.inputDisAble = false;

         reward = null;
         busy = false;
         rewardingType = RewardingType.None;
         yield return new WaitForSeconds(0.2f);
     }*/

    /*  void LevelUp()
      {
          EXP = 0;
          Animal animal = GetComponentInParent<Animal>();
          AnimalController animalController = animal.GetComponentInChildren<AnimalController>();

          GameInstance.GameIns.restaurantManager.UpgradePenguin(GameInstance.GameIns.restaurantManager.combineDatas.employeeData[animalController.id - 1].level, false, animalController);
          SliderController sliderController = animal.GetComponentInChildren<SliderController>();
         // sliderController.
      }*/



    public void ClearPlate(FoodStack st)
    { 
        st.type = MachineType.None;
        st.foodStack.Clear();
        st.needFoodNum = 0;
        st.packaged = false;
    }
    public void RestartAction()
    {
        StopAllCoroutines();
     
    }

 
    MoveCalculator moveCalculator = new MoveCalculator();
    protected Node CalculateNodes(Vector3 position)
    {
        moveCalculator.Init();
        openLists.Clear();
        closedLists.Clear();
     
        Node node = moveCalculator.AStarAlgorithm(trans.position, position, openLists, closedLists);

        if (node != null)
        {
           return node;
        }
        else
        {
            //
            Debug.Log("XX" + position);
            return null;
        }
    }

    protected async UniTask<Stack<Vector3>> CalculateNodes_Async(Vector3 position, CancellationToken cancellationToken = default)
    {
        moveCalculator.Init();
        openLists.Clear();
        closedLists.Clear();

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (trans == null || !trans)
            {
               
                await UniTask.NextFrame(cancellationToken: cancellationToken);
                return null;    
            }

            Stack<Vector3> node = await moveCalculator.AStarAlgorithm_Async(trans.position, position, openLists, closedLists, cancellationToken);
            if (node != null)
            {
                return node;
            }
            else
            {
                return null;
            }
        }
        catch (OperationCanceledException) { return null; }
      
    }



    protected List<Node> GetCalculatedList(ref List<Node> returnNodes, Node node)
    {
    
        stack.Clear();
     
     //   GameInstance.GetNodes(ref stack, node);
     
        while (stack.Count > 0)
        {
            returnNodes.Add(stack.Pop());
        }
        return returnNodes;
    }

    protected void MoveComplete()
    {
        moveCalculator.ResetThisGrids();
    }

    private void OnApplicationQuit()
    {
       // Debug.Log("Quit");
        //moveCalculator.RemoveGridData();
    }

    /*protected Vector3 AnimalMovement(Node n, ref int i, List<Node> ns)
    {
        Vector3 size = GameInstance.GetVector3(0.6f, 0.6f, 0.6f);

        //   int i = 0;
        int a = i;
        Node node = n;
        float r = GameInstance.GameIns.calculatorScale.minY + node.r * GameInstance.GameIns.calculatorScale.distanceSize;
        float c = GameInstance.GameIns.calculatorScale.minX + node.c * GameInstance.GameIns.calculatorScale.distanceSize;
    
        Vector3 targetNode = GameInstance.GetVector3(c, GameInstance.GameIns.calculatorScale.height, r);
       // Vector3 targetNode = new Vector3(c, GameInstance.GameIns.calculatorScale.height, r);

        Vector3 updateVector = trans.position;
        float sumDis = (targetNode - updateVector).magnitude;
        updateVector = targetNode;

        bool check = false;
        Vector3 realUpdateVector = Vector3.zero; //new Vector3();
        for (int j = ns.Count - 1; j > i; j--)
        {
            Node node2 = ns[j];
            float r2 = GameInstance.GameIns.calculatorScale.minY + node2.r * GameInstance.GameIns.calculatorScale.distanceSize;
            float c2 = GameInstance.GameIns.calculatorScale.minX + node2.c * GameInstance.GameIns.calculatorScale.distanceSize;
            Vector3 targetNode2 = GameInstance.GetVector3(c2, GameInstance.GameIns.calculatorScale.height, r2); 
           // Vector3 targetNode2 = new Vector3(c2, GameInstance.GameIns.calculatorScale.height, r2); 
            sumDis += (targetNode2 - updateVector).magnitude;
            //updateVector = targetNode2;
            updateVector = targetNode2;
            float diff = (targetNode2 - trans.position).magnitude;
            Vector3 di = (targetNode2 - trans.position);
            if (diff > 0f)
            {
                di = di / diff;
            }
            //.normalized;
            Vector3 di2 = Quaternion.AngleAxis(15, Vector3.up) * di;
            Vector3 di3 = Quaternion.AngleAxis(-15, Vector3.up) * di;

            bool ch = false;
            Vector3 checkVector = trans.position;
            float mag = (checkVector - (trans.position + di * diff)).magnitude;
            float p = 1 / (mag) ;
            float f = 0;
            while (f <= 1)
            {
                Vector3 origin = Vector3.Lerp(checkVector, trans.position + di * diff,f);

                if(Physics.CheckBox(origin, size, modelTrans.rotation, 1 << 6 | 1 << 7))
                {
                    ch = true;
                    break;
                } 
                f += p;
            }

            //Debug.DrawLine(trans.position, trans.position + mag * di, Color.red, 5);
            *//*   if (!cc && !aa && !bb && !dd && !ee && !ff && !gg && !hh && !ii)
               {
                   realUpdateVector = targetNode2;
                   check = true;
                   i = j;
               }*//*
            if(!ch && ns.Count -1 == j)
           // if (!xx && !yy && !zz)
            {
                realUpdateVector = targetNode2;
                check = true;
                i = j;
                break;
            }
        
        }
        if (check)
        {
            return realUpdateVector;
        }
        else
        {
         
            Node node3 = ns[i];
            float r3 = GameInstance.GameIns.calculatorScale.minY + node3.r * GameInstance.GameIns.calculatorScale.distanceSize;
            float c3 = GameInstance.GameIns.calculatorScale.minX + node3.c * GameInstance.GameIns.calculatorScale.distanceSize;
            Vector3 targetNode3 = GameInstance.GetVector3(c3, GameInstance.GameIns.calculatorScale.height, r3);
           // Vector3 targetNode3 = new Vector3(c3, GameInstance.GameIns.calculatorScale.height, r3);
            return targetNode3;
     
        }
    }*/

    protected Vector3 AnimalMovement(Node n, ref int i, List<Node> ns)
    {
        Vector3 size = GameInstance.GetVector3(0.6f, 0.6f, 0.6f);
        Vector3 currentPos = trans.position;

        // 끝에서 시작까지 역순으로 직선 이동 가능성 확인
        /*   for (int j = ns.Count - 1; j > i; j--)
           {
               Node targetNode = ns[j];
               Vector3 targetPos = NodeToWorldPosition(targetNode);

               if (IsPathClear(currentPos, targetPos, size))
               {
                   i = j; // 다음 이동 노드 갱신
                   return targetPos;
               }
           }*/
        int iOrigin = i;
        Vector3 target = Vector3.zero;
        for(int j = i; j<ns.Count; j++)
        {
            Node targetNode = ns[j];
            Vector3 targetPos = NodeToWorldPosition(targetNode);

            if(IsPathClear(currentPos, targetPos, size))
            {
                target = targetPos;
                i = j;
            }
            else
            {
                if (i != iOrigin) return target;
            }
        }

        // 직선 이동 불가능 시 다음 노드로 이동
        Node nextNode = ns[i];
        return NodeToWorldPosition(nextNode);
    }

    // 노드 정보를 월드 좌표로 변환
    private Vector3 NodeToWorldPosition(Node node)
    {
        float x = GameInstance.GameIns.calculatorScale.minX + node.c * GameInstance.GameIns.calculatorScale.distanceSize;
        float z = GameInstance.GameIns.calculatorScale.minY + node.r * GameInstance.GameIns.calculatorScale.distanceSize;
        return GameInstance.GetVector3(x, GameInstance.GameIns.calculatorScale.height, z);
    }

    // 경로가 직선으로 연결 가능한지 확인
   /* private bool IsPathClear(Vector3 start, Vector3 end, Vector3 size)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        direction.Normalize();

        float step = 0.1f; // 검사 간격 (너무 작으면 성능 저하, 너무 크면 정확도 저하)
        float currentDistance = 0;

        while (currentDistance < distance)
        {
            Vector3 checkPoint = start + direction * currentDistance;

            if (Physics.CheckBox(checkPoint, size, modelTrans.rotation, 1 << 6 | 1 << 7))
            {
                return false; // 장애물 발견
            }

            currentDistance += step;
        }

        return true; // 경로에 장애물이 없음
    }
*/

    /*public Vector3 AnimalMovement(Vector3 coord, ref int i, List<Vector3> vectors)
    {
        Vector3 size = GameInstance.GetVector3(0.6f, 0.6f, 0.6f);
        int a = i;
        Vector3 loc = trans.position;

        for (int j = vectors.Count - 1; j >= i; j--)
        {

            Vector3 di = loc - vectors[j];

            float dif = di.magnitude;
            float s = 0;
            bool isBlock = false;
            while (s < 1)
            {
                s += 1 / dif;
                Vector3 checkLoc = Vector3.Lerp(loc, vectors[j], s);

                if (Physics.CheckBox(checkLoc, size, Quaternion.identity, 1 << 6 | 1 << 7))
                {
                    isBlock = true;
                    break;
                }
            }

            if (!isBlock)
            {
               
                i = j;

                return vectors[j];
             
            }
        }
        return vectors[i];
        *//*
                //  Vector3 targetNode = coord;
                // Vector3 targetNode = new Vector3(c, GameInstance.GameIns.calculatorScale.height, r);

                Vector3 updateVector = trans.position;
                //float sumDis = (targetNode - updateVector).magnitude;
              //  updateVector = targetNode;

                bool check = false;
                Vector3 realUpdateVector = Vector3.zero; //new Vector3();
                for (int j = vectors.Count - 1; j > i; j--)
                {
                    Vector3 node2 = vectors[j];

                    // Vector3 targetNode2 = new Vector3(c2, GameInstance.GameIns.calculatorScale.height, r2); 
                 //   sumDis += (node2 - updateVector).magnitude;
                    //updateVector = targetNode2;
                    updateVector = node2;
                    float diff = (node2 - trans.position).magnitude;
                    Vector3 di = (node2 - trans.position);
                    if (diff > 0f)
                    {
                        di = di / diff;
                    }

                    bool ch = false;
                    Vector3 checkVector = trans.position;
                    float mag = (checkVector - (trans.position + di * diff)).magnitude;
                    float p = 1 / (mag);
                    float f = 0;
                    while (f <= 1)
                    {
                        Vector3 origin = Vector3.Lerp(checkVector, trans.position + di * diff, f);

                        if (Physics.CheckBox(origin, size, modelTrans.rotation, 1 << 6 | 1 << 7))
                        {
                            ch = true;
                            break;
                        }
                        f += p;
                    }

                    if (!ch && vectors.Count - 1 == j)
                    {
                        realUpdateVector = node2;
                        check = true;
                        i = j;
                        break;
                    }

                }
                if (check)
                {
                    return realUpdateVector;
                }
                else
                {

                    Vector3 node3 = vectors[i];

                    Vector3 targetNode3 = node3;
                    // Vector3 targetNode3 = new Vector3(c3, GameInstance.GameIns.calculatorScale.height, r3);
                    return targetNode3;

                }*//*
    }*/
    public Vector3 AnimalMovement(Vector3 vector, ref int i, List<Vector3> vectors)
    {
        Vector3 size = GameInstance.GetVector3(0.6f, 0.6f, 0.6f);
        Vector3 currentPosition = trans.position;

        int iOrigin = i;
        Vector3 target = Vector3.zero;
        for (int j = i; j < vectors.Count; j++)
        {
            Vector3 targetPosition = vectors[j];
            if (IsPathClear(currentPosition, targetPosition, size))
            {
                i = j;
                target = targetPosition;
               
            }
            else
            {
                if(i != iOrigin) return target;
            }
        }
        if (i != iOrigin) return target;


        return vectors[i];
    }

    private bool IsPathClear(Vector3 start, Vector3 end, Vector3 boxSize)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        float step = 1f / distance;
        float progress = 0f;

        while (progress <= 1)
        {
            Vector3 checkPosition = Vector3.Lerp(start, end, progress);
            if (Physics.CheckBox(checkPosition, boxSize, Quaternion.identity, 1 << 6 | 1 << 7))
            {
                return false;
            }
            progress += step;
        }

        return true;
    }
}
