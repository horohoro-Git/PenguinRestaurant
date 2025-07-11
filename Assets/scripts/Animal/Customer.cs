using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
#if HAS_DOTWEEN
using DG.Tweening;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using System.Text;
using System.Data.SqlTypes;
// 한글

public class Customer : AnimalController
{
    enum CustomerAction
    {
        NONE,
        ProcessWait,
        CustomerCounter,
        CustomerTable,
        CustomerGoToHome

    }
    [NonSerialized]
    public AnimalType animalType;
    [NonSerialized]
    public CustomerState customerState;
    [NonSerialized]
    public FoodsAnimalsWant foodsAnimalsWant;

 //   int currentWaypointIndex;
    bool hasMoney;
    float eatingTimer;

    private float speed;
    private float eatSpeed;
    private int minOrder;
    private int maxOrder;

 
    private int likeFood;
    private int hateFood;
    List<Node> nodes = new List<Node>();

  //  bool bStart = false;

 //   CustomerAction customerAction = CustomerAction.NONE;
    QueuePoint[] Qpositions;
    Counter ct;
    Table tb;
    int seatIndex;
    int animalEat;
    [NonSerialized] public bool bOrder;
    public Action<Customer> customerCallback;
   

    public List<FoodStack> foodStacks = new List<FoodStack>();
    Transform transforms;

    [NonSerialized]
    public AnimalSpawner animalSpawner;

    [NonSerialized]
    public int customerIndex;

    public Animal currentAnimal;

    public GameObject endPoint;

    CancellationTokenSource cancellationTokenSource;

    StringBuilder stringBuilder = new StringBuilder();

    private void Awake()
    {
        transforms = transform;
   //     currentWaypointIndex = 5;
        nodes.Capacity = 100;
        //openCoords.Capacity = 200;
        //closedCoords.Capacity = 200;
    }
    private void Start()
    {
    }

    //  FoodStack foodStack = new FoodStack();


    public void Setup(FoodsAnimalsWant foodsAnimals)
    {
        int r = UnityEngine.Random.Range(0, 3);
      /*  if (r == 0) animalWaitActions = "Idle_A";
        else if (r == 1) animalWaitActions = "Bounce";
        else animalWaitActions = "LookAround";*/

        hasMoney = true;
  //      currentWaypointIndex = 5;
 
        if (foodsAnimals.spawnerType == AnimalSpawner.SpawnerType.Delivery)
        {
            WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;

            for (int i = 0; i < workSpaceManager.counters.Count; i++)
            {
                if (workSpaceManager.counters[i].counterType == CounterType.Delivery)
                {
                    workSpaceManager.counters[i].customer = this;
                    transforms.position = workSpaceManager.counters[i].transforms.position;
             
                    FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
                    ClearPlate(foodStack);
                    foodStack.needFoodNum = 8;
                    foodStack.type = MachineType.PackingTable;
                    foodStacks.Add(foodStack);
                    GameInstance.GameIns.uiManager.UpdateOrder(this, CounterType.Delivery);
                }
            }
        }
        else
        {
            foodsAnimalsWant = foodsAnimals;

        }
    }

    void Order()
    {
        for (int i = foodStacks.Count - 1; i >= 0; i--)
        {
            FoodStack foodStack = foodStacks[i];
            foodStacks.RemoveAt(i);

            foreach(var food in foodStack.foodStack)
            {
                FoodManager.EatFood(food);
            }
            foodStack.foodStack.Clear();
       
            FoodStackManager.FM.RemoveFoodStack(foodStack);

        }
        int maxNum = animalStruct.max_order + 1 + (animalPersonality == AnimalPersonality.Foodie ? 2 : 0) - (animalPersonality == AnimalPersonality.LightEater ? (animalStruct.max_order - animalStruct.min_order > 2 ? 2 : animalStruct.max_order - animalStruct.min_order) : 0);
       // if(maxNum <= 1) Debug.LogError("MaxNMum " + maxNum);
        if (foodsAnimalsWant.burger)
        {
            FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
            // ClearPlate(foodStack);
            foodStack.needFoodNum = UnityEngine.Random.Range(animalStruct.min_order, maxNum);
            foodStack.type = MachineType.BurgerMachine;
            foodStacks.Add(foodStack);


     
        }

        if (foodsAnimalsWant.coke)
        {
            FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
            //ClearPlate(foodStack);
            foodStack.needFoodNum = UnityEngine.Random.Range(animalStruct.min_order, maxNum);
            foodStack.type = MachineType.CokeMachine;
            foodStacks.Add(foodStack);
        }

        if (foodsAnimalsWant.coffee)
        {
            FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
            //   ClearPlate(foodStack);
            foodStack.needFoodNum = UnityEngine.Random.Range(animalStruct.min_order, maxNum);
            foodStack.type = MachineType.CoffeeMachine;
            foodStacks.Add(foodStack);
        }

        if (foodsAnimalsWant.donut)
        {
            FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
            //  ClearPlate(foodStack);
            foodStack.needFoodNum = UnityEngine.Random.Range(animalStruct.min_order, maxNum);
            foodStack.type = MachineType.DonutMachine;

            foodStacks.Add(foodStack);
        }



        for (int i = 0; i < foodStacks.Count; i++)
        {
            int s = foodStacks[i].needFoodNum - foodStacks[i].foodStack.Count;
            if (s < 1) Debug.LogError("FoodStack Error " + s);
        }
    }
    public override int GetHashCode() => customerIndex.GetHashCode();

    public override bool Equals(object obj)
    {
        if (obj is not Customer other) return false;
        return customerIndex == other.customerIndex;
    }

    public List<Food> VisualizingFoodStack = new List<Food>();

    private void Update()
    {
       /* if (headPoint != null)
        {
            for (int i = 0; i < VisualizingFoodStack.Count; i++)
            {
                Vector3 addHeight = GameInstance.GetVector3(0, i * 0.7f, 0);
                VisualizingFoodStack[i].transforms.position =
                    headPoint.position + addHeight;// new Vector3(0, i * 0.7f, 0);
            }
        }*/
       /* if(!animalMove)
        {
            switch (customerAction)
            {
                case CustomerAction.NONE: break;
                case CustomerAction.ProcessWait:
                    Wait();
                    break;
                case CustomerAction.CustomerCounter:
                    if(!bStart) CustomerCounter_Start(Qpositions, ct);
                    else if(!doOnce) CustomerCounter_Rotate(Qpositions, ct);
                    else CustomerCounter_Update(Qpositions, ct);
                    break;
                case CustomerAction.CustomerTable:
                    if (!bStart) CustomerTable_Start(moveNode, tb, seatIndex);
                    else if(!doOnce) CustomerTable_Rotate(moveNode, tb, seatIndex);
                    else if(animalEat == 0) CustomerTable_Update(moveNode, tb, seatIndex);
                    else if(animalEat == 1) CustomerTable_Eating(moveNode, tb, seatIndex);
                    else if(animalEat == 2) CustomerTable_Garbage(moveNode, tb, seatIndex);
                    break;
                case CustomerAction.CustomerGoToHome:
                    if(!bStart) CustomerGoHome_Start(moveNode);
                    else CustomerGoHome_Update(moveNode);
                    
                    break;
            }

        }*/

    }

    private void FixedUpdate()
    {
       /* if(animalMove)
        {
            if(moveStart==0)
            {
                AnimalMove_Start(moveNode);
            }
            else if (moveStart == 1)
            {
                AnimalMove_Update(moveNode);
            }
            else if (moveStart == 2)
            {
                AnimalMove_Update2(moveNode);
            }
        }*/
    }

    Vector3 target = new Vector3();
    List<Table> tableList = new List<Table>();
    List<EndPoint> endList = new List<EndPoint>();
    public void FindCustomerActions()
    {
        WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;

        if (customerState != CustomerState.Table)
        {
            if (customerState == CustomerState.Walk)
            {
                busy = true;

                CustomerPlayAction(
                    workSpaceManager.counters[(int)foodsAnimalsWant.spawnerType].queuePoints,
                    workSpaceManager.counters[(int)foodsAnimalsWant.spawnerType]
                );

                return;
            }
            else if (customerState == CustomerState.Counter)
            {
                //테이블 정렬 거리 오름차순
                tableList = workSpaceManager.tables;
                for(int i=0; i<tableList.Count-1; i++)
                {
                    int min = i;
                    for(int j=i+1; j<tableList.Count;j++)
                    {
                        float m1 = (tableList[j].transforms.position - trans.position).magnitude;
                        float m2 = (tableList[min].transforms.position - trans.position).magnitude;
                        if(m1 < m2)
                        {
                            min = j;  
                        }
                    }
                    if (min != i)
                    {
                        Table temp = tableList[min];
                        tableList[min] = tableList[i];
                        tableList[i] = temp;
                    }
                }

                for(int i=0; i<tableList.Count; i++)
                {
                    if(!tableList[i].isDirty)
                    {
                        for (int j = 0; j < tableList[i].seats.Length; j++)
                        {
                            if (tableList[i].seats[j].animal == this)
                            {
                        
                                busy = true;
                               // tableList[i].seats[j].customer = this;
                                target = tableList[i].seats[j].transforms.position;



                                CustomerPlayAction(tableList[i], j);
                                return;
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
           /* endList = workSpaceManager.endPoints;
            int n = 0;
            float min = float.MaxValue;
            for(int i=0;i<endList.Count; i++)
            {
                float other = (endList[i].transforms.position - trans.position).magnitude;

                if (other < min)
                {
                    min = other;
                    n = i;
                }
            }*/
            busy = true;

            CustomerPlayAction(endPoint.transform.position);//endList[n].transforms.position);
        }
    }

    public void CustomerPlayAction()
    {
        Customer_Wait(App.GlobalToken).Forget();
       // StartCoroutine(CustomerWait());
    }

    public void CustomerPlayAction(QueuePoint[] position, Counter counter)
    {
     /*   Qpositions = position;*/
        ct = counter;
       // customerAction = CustomerAction.CustomerCounter;
        Customer_Counter(position, counter, App.GlobalToken).Forget();
        //StartCoroutine(CustomerWalkToCounter(position, counter));
    }

    public void CustomerPlayAction(Table table, int index)
    {
        Customer_Table(table, index, App.GlobalToken).Forget();
  
    }

    public void CustomerPlayAction(Vector3 position)
    {
    
        Customer_GoHome(position, App.GlobalToken).Forget();
    }

    async UniTask Customer_Wait(CancellationToken cancellationToken = default)
    {
        try
        {
          //  await UniTask.Delay(200, cancellationToken: cancellationToken);
            await Utility.CustomUniTaskDelay(0.2f, cancellationToken);
         //   GameInstance.GameIns.animalManager.AttacCustomerTask(this);
            customerCallback?.Invoke(this);
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Customer_Wait task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Customer_Wait: {ex.Message}");
            throw;
        }
    }

    async UniTask Customer_Counter(QueuePoint[] position, Counter counter, CancellationToken cancellationToken = default)
    {
        try
        {
            counter.customers.Add(this);
            int queueindex = -1;
        MoveReturn:
            //await UniTask.Delay(250, cancellationToken:cancellationToken);
            await UniTask.NextFrame(cancellationToken: cancellationToken);
            reCalculate = false;
            bOrder = false;

            if(queueindex == -1)
            {
                queueindex = position.Length - 1;
                for (int i = queueindex; i >= 0; i--)
                {
                    if (position[i].controller == null || position[i].controller == this)
                    {
                        queueindex = i;
                    }
                    else
                    {
                        break;
                    }
                }
            }
           
            Vector3 pos = position[queueindex].transforms.position;
            if (!(trans.position == pos && queueindex == 0))
            {
                int currentPoint = queueindex;// position.Length - 1;
                position[queueindex].controller = this;

                Stack<Vector3> moveTargets = await CalculateNodes_Async(pos, false, cancellationToken);
                if (moveTargets != null && moveTargets.Count > 0)
                {
                    Vector3 test = moveTargets.Peek();
                    if (trans.position != pos || !(test.x == 100 && test.z == 100))
                    {
                        await Customer_Move(moveTargets, pos, true, cancellationToken: cancellationToken);
                        modelTrans.rotation = position[currentPoint].transforms.rotation;
                        if (reCalculate)
                        {
                            reCalculate = false;
                            await UniTask.NextFrame(cancellationToken: cancellationToken);
                            goto MoveReturn;
                        }
                    }
                    
                }
                else
                {
                    await Customer_Move(moveTargets, pos, true, cancellationToken: cancellationToken);
                    modelTrans.rotation = position[currentPoint].transforms.rotation;
                }
                for (int i = queueindex; i >= 0; i--)
                {
                    while (position[i].controller != null && position[i].controller != this)
                    {
                        animator.SetInteger("state", 0);
                        animal.PlayAnimation(AnimationKeys.Idle);

                        await UniTask.Delay(200, cancellationToken: cancellationToken);
                    }
                    Vector3 p = position[i].transforms.position;
                    position[currentPoint].controller = null;
                    currentPoint = i;
                    position[currentPoint].controller = this;

                    while (true)
                    {
                        if (Vector3.Distance(trans.position, p) < 0.001f) break;

                        animator.SetInteger("state", 1);
                        animal.PlayAnimation(AnimationKeys.Walk);
                        float subSpeed = animalPersonality == AnimalPersonality.Relaxed ? 0.8f : (animalPersonality == AnimalPersonality.Impatient ? 1.2f : 1);
                        trans.position = Vector3.MoveTowards(trans.position, p, animalStruct.speed * Time.deltaTime * subSpeed);
                        await UniTask.NextFrame(cancellationToken: cancellationToken);
                    }
                }

                animator.SetInteger("state", 0);
                animal.PlayAnimation(AnimationKeys.Idle);
            }
         
            //요구 사항 표시
            if (foodStacks.Count == 0)
            {
                counter.customer = this;
                //음식 주문
                Order();
                GameInstance.GameIns.uiManager.UpdateOrder(this, counter.counterType);
            }

            bOrder = true;
            //받은 음식 체크
            while (true)
            {
                bool check = true;
                for (int j = 0; j < foodStacks.Count; j++)
                {
                    if (j >= foodStacks.Count) continue;
                    if (foodStacks[j].needFoodNum != foodStacks[j].foodStack.Count)
                    {
                        check = false;
                        break;
                    }
                }
                if (check) break;

                await UniTask.Delay(200, cancellationToken: cancellationToken);
            }

            GameInstance.GameIns.uiManager.UpdateOrder(this, counter.counterType);

            //비용 지불
            BigInteger foodPrices = 0;
            int tipNum = 0;
            int foodNum = 0;
            for (int j = 0; j < foodStacks.Count; j++)
            {
                foreach (var v in foodStacks[j].foodStack)
                {
                    (int, List<int>) animalTier = AnimalManager.gatchaTiers[animalStruct.id];
                    animalTier.Item1 = (animalTier.Item1 - 1) * 2;
                    animalTier.Item1 = animalTier.Item1 == 0 ? 1 : animalTier.Item1;
                    BigInteger price = v.foodPrice * animalTier.Item1 * AnimalManager.gatchaValues;
                    int leftover = (int)(price % 100);
                    price /= 100;
                    foodPrices += price;
                    GameInstance.GameIns.restaurantManager.restaurantCurrency.leftover += leftover;
                    if (GameInstance.GameIns.restaurantManager.restaurantCurrency.leftover >= 100)
                    {
                        GameInstance.GameIns.restaurantManager.restaurantCurrency.leftover -= 100;
                        foodPrices += 1;
                    }

                    int tip = Random.Range(1, 11);
                    if (tip == 1) tipNum++;
                    foodNum++;
                }

            }

            if (!GameInstance.GameIns.restaurantManager.miniGame.activate)
            {
                GameInstance.GameIns.restaurantManager.restaurantCurrency.minigameStack += foodNum;

                int r = Random.Range(0, GameInstance.GameIns.restaurantManager.restaurantCurrency.minigameStack);

                if(r > 50)
                {
                    GameInstance.GameIns.restaurantManager.restaurantCurrency.minigameStack = 0;
                    GameInstance.GameIns.restaurantManager.miniGame.activate = true;
                    GameInstance.GameIns.restaurantManager.OpenMiniGame();
                }
            }
            
            stringBuilder = Utility.GetFormattedMoney(foodPrices, stringBuilder);

            FloatingCost fc = GameInstance.GameIns.restaurantManager.GetFloatingCost();

            fc.rectTransform.position = trans.position + Vector3.up * 4; //InputManger.cachingCamera.WorldToScreenPoint(trans.position) + Vector3.up * 50;
            fc.text.text = stringBuilder.ToString();
            fc.height = 3;
            fc.Floating();

            SoundManager.Instance.PlayAudioWithKey(GameInstance.GameIns.uISoundManager.Money(), 0.2f, GameInstance.GameIns.restaurantManager.moneyChangedSoundKey);
            GameInstance.GameIns.restaurantManager.GetMoney(foodPrices.ToString());

            GameInstance.GameIns.restaurantManager.GetFish(GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes, tipNum);
            GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes += tipNum;
            GameInstance.GameIns.restaurantManager.restaurantCurrency.changed = true;
            //   GameInstance.GameIns.uiManager.UpdateMoneyText(GameInstance.GameIns.restaurantManager.restaurantCurrency.money);

            hasMoney = false;

            await UniTask.Delay(500, cancellationToken: cancellationToken);

            List<Table> tables = GameInstance.GameIns.workSpaceManager.tables.ToList();
            //foodMachine  거리 오름차순 개수 내림차순

            for (int i = foodStacks.Count - 1; i >= 0; i--)
            {
                FoodStack foodStack = foodStacks[i];
                foodStacks.RemoveAt(i);
                foodStack.foodStack.Clear();
                FoodStackManager.FM.RemoveFoodStack(foodStack);
            }

            while (true)
            {
                tables = tables
                    .OrderBy(fm => (fm.transforms.position - trans.position).magnitude) // 음식 개수 내림차순
                    .ToList();
                await UniTask.Delay(500, cancellationToken: cancellationToken);

                foreach (Table t in tables)
                {
                    if (t.isDirty == false && t.placed)
                    {
                        int index = -1;
                        int c = 0;
                        for(int j = 0; j < t.seats.Length; j++)
                        {
                            if (t.seats[j].animal != null)
                            {
                                index = j;
                                c++;
                            }
                        }
                        if (c >= 2) continue;

                        float min = 9999;
                        Seat selectedSeat = null;
                        for (int j = 0; j < t.seats.Length; j++)
                        {
                            if (t.seats[j].isDisEnabled == false && t.seats[j].animal == null)
                            {
                                float cur = Vector3.Distance(trans.position, t.seats[j].transform.position);
                                if (min > cur)
                                {
                                    min = cur;
                                    selectedSeat = t.seats[j];
                                }
                            }
                        }

                        if (selectedSeat != null)
                        {
                            selectedSeat.animal = this;
                            t.numberOfFoods += foodNum;
                            counter.customer = null;
                            position[0].controller = null;
                        }
                        else continue;

                        customerState = CustomerState.Counter;
                        busy = false;
                        customerCallback?.Invoke(this);
                        return;
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Customer_Counter task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Customer_Counter: {ex.Message}");
            throw;
        }
    }
    
    async UniTask Customer_Move(Stack<Vector3> n, Vector3 loc, bool standInLine = false, QueuePoint point = null, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            float subSpeed = animalPersonality == AnimalPersonality.Relaxed ? 0.8f : (animalPersonality == AnimalPersonality.Impatient ? 1.2f : 1);
            float speed = standInLine ? 5 : animalStruct.speed * subSpeed; 
            if (n != null && n.Count > 0)
            {
                n.Pop();

                await UniTask.NextFrame(cancellationToken: cancellationToken);
                while (n.Count > 0)
                {
                    if (trans == null || !trans)
                    {
                        Debug.Log("No Trans");
                        await UniTask.NextFrame();
                        return;
                    }
                    Vector3 target = n.Pop();
                    float cur = (target - trans.position).magnitude;
                    while (true)
                    {
                        if (App.restaurantTimeScale == 1)
                        {
                            //if (!standInline && reCalculate)
                            if (reCalculate)
                            {
                                return;
                            }

                            if (Vector3.Distance(trans.position, target) <= 0.01f) break;

                            Debug.DrawLine(trans.position, target, Color.red, 0.1f);
                            //  moveCTS.Token.ThrowIfCancellationRequested();
                            animator.SetInteger("state", 1);
                            animal.PlayAnimation(AnimationKeys.Walk);
                            // PlayAnim(animal.animationDic["Run"], "Run");
                            cur = (target - trans.position).magnitude;
                            Vector3 dir = (target - trans.position).normalized;

                            trans.position = Vector3.MoveTowards(trans.position, target, speed * Time.deltaTime);
                            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                            modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                        }
                        await UniTask.NextFrame(cancellationToken: cancellationToken);
                    }

                    /*while (cur > 0.1f)
                    {
                        if(standInline && point != null)
                        {
                            if(point.controller == null)
                            {
                                reCalculate = true;
                            }
                        }


                        if (!standInline && reCalculate)
                        {
                            Debug.Log("reCalculate");
                            return;
                        }
                        if (trans == null || !trans)
                        {
                            Debug.Log("No Trans");
                            await UniTask.NextFrame();
                            return;
                        }
                        Debug.DrawLine(trans.position, target, Color.red, 0.1f);
                        //  moveCTS.Token.ThrowIfCancellationRequested();
                        animator.SetInteger("state", 1);
                        animal.PlayAnimation(AnimationKeys.Walk);
                       // PlayAnim(animal.animationDic["Run"], "Run");
                        cur = (target - trans.position).magnitude;
                        Vector3 dir = (target - trans.position).normalized;
                        trans.position = Vector3.MoveTowards(trans.position, target, animalStruct.speed * Time.deltaTime);
                        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                        modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                       // animatedAnimal.transforms.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                        await UniTask.NextFrame(cancellationToken: cancellationToken);
                    }*/
                }
            }
            Vector3 newLoc = loc;
            newLoc.y = 0;

            while (true)
            {
                if (App.restaurantTimeScale == 1)
                {
                    if (reCalculate)
                    {
                        return;
                    }
                    animator.SetInteger("state", 1);
                    animal.PlayAnimation(AnimationKeys.Walk);
                    trans.position = Vector3.MoveTowards(trans.position, newLoc, speed * Time.deltaTime);
                    if (Vector3.Distance(trans.position, newLoc) <= 0.01f) break;
                    Vector3 dir = newLoc - trans.position;
                    float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                    modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                }
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }


            animator.SetInteger("state", 0);
            animal.PlayAnimation(AnimationKeys.Idle);
            //PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Customer_Move task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Customer_Move: {ex.Message}");
            throw;
        }
    }

    async UniTask Customer_Table(Table table, int index, CancellationToken cancellationToken = default)
    {
        try
        {
            table.animals.Add(this);

        StartTable:
            while(!table.placed) await Utility.CustomUniTaskDelay(0.2f, cancellationToken);
            if (table.seats[index].isDisEnabled)
            {
                while (true)
                {
                    await UniTask.Delay(500, cancellationToken: cancellationToken);
                    for (int i = 0; i < 4; i++)
                    {
                        if (i != index)
                        {
                            if (!table.seats[i].isDisEnabled && table.seats[i].animal == null)
                            {
                                table.seats[index].animal = null;
                                table.seats[i].animal = this;
                                table.animals.Remove(this);

                                if (table.placedFoods[index] != null)
                                {
                                    table.placedFoods[i] = table.placedFoods[index];
                                    table.placedFoods[index] = null;

                                    Vector3 t = table.transforms.position;

                                    Vector3 F = -table.seats[i].transform.forward;
                                    t += F;
                                    // t += Z;
                                    t.y = 0.5f;
                                    table.placedFoods[i].transform.DOJump(t, 1, 1, 0.2f);
                                }

                                Emote(false, AnimationKeys.Normal);
                                customerState = CustomerState.Counter;
                                busy = false;
                                customerCallback?.Invoke(this);
                                return;
                            }
                        }
                    }

                    if (table.placedFoods[index] != null)
                    {
                        table.placedFoods[index].transform.SetParent(FoodManager.foodCollects.transform);
                        VisualizingFoodStack.Add(table.placedFoods[index].GetComponent<Food>());
                    }
                    for (int i = 0; i < table.placedFoods.Length; i++)
                    {
                        if (i != index && table.placedFoods[i] != null && table.seats[i].animal == null)
                        {
                            table.placedFoods[i].transform.SetParent(FoodManager.foodCollects.transform);
                            VisualizingFoodStack.Add(table.placedFoods[i].GetComponent<Food>());
                        }
                    }

                    while (table.foodStacks[0].foodStack.Count > 0)
                    {
                        Food f = table.foodStacks[0].foodStack.Pop();
                        f.transform.SetParent(FoodManager.foodCollects.transform);
                        VisualizingFoodStack.Add(f);
                    }

                    List<Table> tables = GameInstance.GameIns.workSpaceManager.tables
                        .OrderBy(fm => (fm.transforms.position - trans.position).magnitude) // 음식 개수 내림차순
                        .ToList();
                    await UniTask.Delay(500, cancellationToken: cancellationToken);

                    foreach (Table tb in tables)
                    {
                        if (tb == table || !tb.placed) continue;

                        if (tb.isDirty == false)
                        {
                            int i = -1;
                            int c = 0;
                            for (int j = 0; j < tb.seats.Length; j++)
                            {
                                if (tb.seats[j].animal != null)
                                {
                                    i = j;
                                    c++;
                                }
                            }
                            if (c >= 2) continue;



                            float min = 9999;
                            Seat selectedSeat = null;
                            for (int j = 0; j < tb.seats.Length; j++)
                            {
                                if (tb.seats[j].isDisEnabled == false && tb.seats[j].animal == null)
                                {
                                    float cur = Vector3.Distance(trans.position, tb.seats[j].transform.position);
                                    if (min > cur)
                                    {
                                        min = cur;
                                        selectedSeat = tb.seats[j];
                                    }
                                }
                            }

                            if (selectedSeat != null)
                            {
                                selectedSeat.animal = this;
                                tb.numberOfGarbage += table.numberOfGarbage;
                                tb.numberOfFoods += table.numberOfFoods;
                                tb.numberOfGarbage = tb.numberOfGarbage > 10 ? 10 : tb.numberOfGarbage;
                                table.numberOfFoods = 0;
                                table.numberOfGarbage = 0;

                            }
                            else continue;

                            table.seats[index].animal = null;
                            table.animals.Remove(this);
                            customerState = CustomerState.Counter;
                            busy = false;
                            customerCallback?.Invoke(this);

                            return;
                        }
                    }
                }
            }
            Emote(false, AnimationKeys.Normal);
            reCalculate = false;
            Vector3 position = table.seats[index].transform.position;
            Stack<Vector3> moveTargets = await CalculateNodes_Async(position, false, cancellationToken);
            await Customer_Move(moveTargets, position, cancellationToken: cancellationToken);
            if (reCalculate)
            {
                while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                reCalculate = false;

                goto StartTable;
            }
            await UniTask.DelayFrame(3, cancellationToken: cancellationToken);
            modelTrans.rotation = table.seats[index].transform.rotation;
            bool stealing = false;
        Stealing:
            while (true)
            {
                if (!table.placed)
                {
                    await Utility.CustomUniTaskDelay(0.2f, cancellationToken);
                    goto StartTable;
                }

                seatIndex = index;
                tb = table;
                await Utility.CustomUniTaskDelay(0.2f, cancellationToken);

                Vector3 t = table.transforms.position;

                Vector3 F = -table.seats[seatIndex].transform.forward;
                t += F;
                t.y = 0.5f;

                for (int i = VisualizingFoodStack.Count - 1; i >= 0; i--)
                {
                    modelTrans.rotation = table.seats[index].transform.rotation;
                    if (!table.placed || table.seats[index].isDisEnabled)
                    {

                        goto StartTable;
                    }

                    Food f = VisualizingFoodStack[i];
                    VisualizingFoodStack.RemoveAt(i);

                    for (int j = 0; j < foodStacks.Count; j++)
                    {
                        if (f.parentType == foodStacks[j].type)
                        {
                            foodStacks[j].foodStack.Pop();
                        }
                    }
                    f.Release();
                    float r = UnityEngine.Random.Range(1, 2.5f);
                    Vector3 pos = table.transforms.position + GameInstance.GetVector3(0, 0.7f + table.foodStacks[0].foodStack.Count, 0);
#if HAS_DOTWEEN
                    f.transforms.DOJump(pos, r, 1, 0.2f);
#endif
                    table.foodStacks[0].foodStack.Push(f);
                    // await UniTask.Delay(300, cancellationToken: cancellationToken);
                    await Utility.CustomUniTaskDelay(0.3f, cancellationToken);
                }

                if (table.tableState == TableState.HasTrouble)
                {
                    await UniTask.Delay(200, cancellationToken: cancellationToken);
                    goto Stealing;
                }
                else if (table.tableState == TableState.Stealing)
                {
                    if (!stealing)
                    {
                        stealing = true;
                        SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.Angry(), 0.1f, 100, 5, trans.position);
                        if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
                        cancellationTokenSource = new CancellationTokenSource();
                        EmoteTimer(0, AnimationKeys.Sad, true, cancellationTokenSource.Token).Forget();
                        FloatingEmote(4002);
                    }
                    await UniTask.Delay(200, cancellationToken: cancellationToken);
                    goto Stealing;
                }
                else if (table.tableState == TableState.Stolen)
                {
                    table.tableState = TableState.None;
                    SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.Sad(), 0.1f, 100, 5, trans.position);

                    if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
                    cancellationTokenSource = new CancellationTokenSource();
                    EmoteTimer(0, AnimationKeys.Trauma, true, cancellationTokenSource.Token).Forget();

                    FloatingEmote(4003);
                    if (GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation > 0)
                    {
                        GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation--;
                        GameInstance.GameIns.uiManager.reputation.text = GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation.ToString();
                        GameInstance.GameIns.restaurantManager.restaurantCurrency.changed = true;
                        GameInstance.GameIns.restaurantManager.CalculateSpawnTimer();
                    }
                    await Utility.CustomUniTaskDelay(3f, cancellationToken);
                    // await UniTask.Delay(3000, cancellationToken: cancellationToken);
                    customerState = CustomerState.Table;
                    animator.SetInteger("state", 0);
                    animal.PlayAnimation(AnimationKeys.Idle);
                    table.animals.Remove(this);
                    table.seats[index].animal = null;
                    customerCallback?.Invoke(this);
                    return;
                }
                else
                {
                    if(stealing)
                    {
                        stealing = false;
                        Emote(false, AnimationKeys.Normal);
                    }
                }

                if (table.placedFoods[seatIndex] == null)
                {
                    //음식 가져오기 주변좌석 우선
                    for (int i = 0; i < table.placedFoods.Length; i++)
                    {
                        if (seatIndex != i && table.seats[i].animal == null && table.placedFoods[i] != null)
                        {
                            table.placedFoods[seatIndex] = table.placedFoods[i];
                            table.placedFoods[i] = null;
                            SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.ThrowSound(), 0.4f, 100, 5, trans.position);
                            table.placedFoods[seatIndex].transform.DOJump(t, 1, 1, 0.2f);
                            await Utility.CustomUniTaskDelay(0.3f, cancellationToken);
                            break;
                        }
                    }

                    if (table.placedFoods[seatIndex] == null && table.foodStacks[0].foodStack.Count > 0)
                    {
                        Food f = table.foodStacks[0].foodStack.Pop();
                        table.placedFoods[seatIndex] = f.gameObject;
                        SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.ThrowSound(), 0.4f, 100, 5, trans.position);
                        table.placedFoods[seatIndex].transform.DOJump(t, 1, 1, 0.2f);
                        await Utility.CustomUniTaskDelay(0.3f, cancellationToken);
                    }
                    else if (table.placedFoods[seatIndex] == null)
                    {
                        // 음식을 다 먹음
                        int reputation = Random.Range(0, 10);
                        int targetRep = animalPersonality == AnimalPersonality.Loyal ? 10 : (animalPersonality == AnimalPersonality.HardToPlease ? 5 : 8);

                        if (reputation < targetRep)
                        {
                            int gradeUp = Random.Range(0, 100);

                            if (gradeUp == 0)
                            {
                                //등급업
                                // if (AnimalManager.gatchaTiers[id].Item1 < 4)
                                {
                                    int tier = AnimalManager.gatchaTiers[animalStruct.id].Item1;
                                    (int, List<int>) tmp = AnimalManager.gatchaTiers[animalStruct.id];
                                    int personality = Random.Range(0, 7);
                                    bool success = tmp.Item1 < 4 ? true : false;
                                    if (success) tmp.Item1++;
                                    tmp.Item2[personality] = 1;
                                    AnimalManager.gatchaTiers[animalStruct.id] = tmp;

                                    if (success)
                                    {
                                        GameInstance.GameIns.gatcharManager.popup.SetActive(true);
                                        SoundManager.Instance.PlayAudio(GameInstance.GameIns.gatchaSoundManager.GradeUp(), 0.4f);

                                        GameInstance.GameIns.gatcharManager.popup_TierUp.SetActive(true);

                                        AnimalStruct asset = AssetLoader.animals[animalStruct.id];
                                        string n = asset.asset_name + "_Sprite";
                                        GameInstance.GameIns.gatcharManager.TierUpAnimalImage.sprite = AssetLoader.loadedSprites[n];
                                        if (tier == 2) GameInstance.GameIns.gatcharManager.backGlow.color = Color.blue;
                                        else if (tier == 3) GameInstance.GameIns.gatcharManager.backGlow.color = new Color(0.5f, 0f, 0.5f);
                                        else if (tier == 4) GameInstance.GameIns.gatcharManager.backGlow.color = Color.yellow;
                                        GameInstance.GameIns.gatcharManager.SetPrice();
                                        SaveLoadSystem.SaveGatchaAnimalsData();

                                        GameInstance.GameIns.gatcharManager.CheckGameClear();

                                    }
                                }
                            }
                            else if (gradeUp == 1)
                            {
                                //새로운 손님 추가
                                if (App.CurrentLevel == 0)
                                {
                                    int randomCustomer = Random.Range(100, 106);
                                    if (!AnimalManager.gatchaTiers.ContainsKey(randomCustomer))
                                    {
                                        GameInstance.GameIns.gatcharManager.popup.SetActive(true);
                                        SoundManager.Instance.PlayAudio(GameInstance.GameIns.gatchaSoundManager.Unlock(), 0.4f);
                                        (int, List<int>) tmp = (1, new List<int>());
                                        int personality = Random.Range(0, 7);
                                        for (int i = 0; i < 7; i++) tmp.Item2.Add(0);
                                        tmp.Item2[personality] = 1;
                                        AnimalManager.gatchaTiers[randomCustomer] = tmp;
                                        AnimalStruct asset = AssetLoader.animals[randomCustomer];
                                        string n = asset.asset_name + "_Sprite";
                                        GameInstance.GameIns.gatcharManager.NewAnimalImage.sprite = AssetLoader.loadedSprites[n];// this.sprites[pair.Key];
                                        GameInstance.GameIns.gatcharManager.popup_NewCustomer.SetActive(true);
                                        AnimalManager.animalStructs[randomCustomer] = asset;
                                        GameInstance.GameIns.gatcharManager.SetPrice();
                                        SaveLoadSystem.SaveGatchaAnimalsData();

                                        GameInstance.GameIns.gatcharManager.CheckGameClear();
                                    }
                                }
                            }

                            SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.Happy(), 0.1f, 100, 5, trans.position);
                            if (GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation < 100)
                            {
                                GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation += 1;
                                GameInstance.GameIns.uiManager.reputation.text = GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation.ToString();
                                GameInstance.GameIns.restaurantManager.CalculateSpawnTimer();
                                GameInstance.GameIns.restaurantManager.restaurantCurrency.changed = true;
                            }
                            FloatingEmote(4000);
                            if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
                            cancellationTokenSource = new CancellationTokenSource();
                            EmoteTimer(5f, AnimationKeys.Happy, false, cancellationTokenSource.Token).Forget();
                        }

                        await Utility.CustomUniTaskDelay(3f, cancellationToken);
                        animator.SetInteger("state", 0);
                        animal.PlayAnimation(AnimationKeys.Idle);
                        table.animals.Remove(this);
                        table.seats[index].animal = null;
                        busy = false;
                        customerState = CustomerState.Table;
                        customerCallback?.Invoke(this);
                        return;
                    }
                    else
                    {
                        await Utility.CustomUniTaskDelay(0.2f, cancellationToken);
                        goto Stealing;
                    }
                }
                else
                {
                    float tm = RestaurantManager.restaurantTimer;
                    //음식 먹기
                    for (int i = 1; i <= 100; i++)
                    {
                        if(!table.placed || table.seats[seatIndex].isDisEnabled || table.tableState != TableState.None)
                        {
                            goto StartTable;
                        }
                        float timer = animalStruct.eat_speed;
                        if (i != 0 && i % 10 == 0)
                        {
                            tm = RestaurantManager.restaurantTimer;

                            SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.Eat(), 0.05f, 100, 5, trans.position);
                            animator.SetTrigger(AnimationKeys.eat);
                            animal.PlayTriggerAnimation(AnimationKeys.Eat);

                            Eat eat = ParticleManager.CreateParticle(ParticleType.Eating).GetComponent<Eat>();
                            eat.transform.position = table.placedFoods[seatIndex].transform.position;
                            eat.PlayEating();
                        }

                        await Utility.CustomUniTaskDelay(timer / 100, cancellationToken);
                    }

                    //먹음

                    FoodManager.EatFood(table.placedFoods[seatIndex].GetComponent<Food>());
                    table.placedFoods[seatIndex] = null;

                    table.numberOfFoods--;
                    table.numberOfFoods = table.numberOfFoods < 0 ? 0 : table.numberOfFoods;
                    table.numberOfGarbage++;

                    //쓰레기 생성
                    if (table.foodStacks[0].foodStack.Count == 0 && table.numberOfFoods == 0)
                    {
                        table.numberOfGarbage = table.numberOfGarbage > 10 ? 10 : table.numberOfGarbage;
                        table.isDirty = true;

                        for (int i = 0; i < table.numberOfGarbage; i++)
                        {
                            Garbage go = GarbageManager.CreateGarbage();
                            go.transforms.SetParent(table.trashPlate.transforms);
                            table.garbageList.Add(go);

                            float x = Random.Range(-1f, 1f);
                            float z = Random.Range(-1f, 1f);
                            go.transforms.position = table.up.position + GameInstance.GetVector3(x, 0, z);
                        }
                    }
                 
                    await UniTask.Delay(200, cancellationToken: cancellationToken);
                }

                /*

             while (true)
             {
                 Vector3 position = table.seats[index].transform.position;

                 Stack<Vector3> moveTargets = await CalculateNodes_Async(position, false, cancellationToken);
                 if (moveTargets != null && moveTargets.Count > 0)
                 {
                     Vector3 test = moveTargets.Peek();
                     if (test.z == 100 && test.x == 100)
                     {
                         // moveNode = null;
                     }
                     else
                     {
                         await Customer_Move(moveTargets, position, cancellationToken: cancellationToken);
                         if (reCalculate)
                         {
                             while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                             reCalculate = false;

                             if(table.seats[index].isDisEnabled)
                             {
                                 goto DisableSeat;
                             }                           
                             continue;
                         }
                         await UniTask.DelayFrame(3, cancellationToken: cancellationToken);
                         modelTrans.rotation = table.seats[index].transform.rotation;
                     }

                     seatIndex = index;
                     tb = table;
                     await Utility.CustomUniTaskDelay(0.2f, cancellationToken);

                     Vector3 t = table.transforms.position;

                     Vector3 F = -table.seats[seatIndex].transform.forward;
                     t += F;
                     t.y = 0.5f;

                     for (int i = VisualizingFoodStack.Count - 1; i >= 0; i--)
                     {
                         modelTrans.rotation = table.seats[index].transform.rotation;
                         if (!table.placed)
                         {

                             goto StartTable;
                         }
                         if (table.seats[index].isDisEnabled)
                         {
                             goto DisableSeat;
                         }
                         Food f = VisualizingFoodStack[i];
                         VisualizingFoodStack.RemoveAt(i);

                         for (int j = 0; j < foodStacks.Count; j++)
                         {
                             if (f.parentType == foodStacks[j].type)
                             {
                                 foodStacks[j].foodStack.Pop();
                             }
                         }
                         f.Release();
                         float r = UnityEngine.Random.Range(1, 2.5f);
                         Vector3 pos = table.transforms.position + GameInstance.GetVector3(0, 0.7f + table.foodStacks[0].foodStack.Count, 0);
 #if HAS_DOTWEEN
                         f.transforms.DOJump(pos, r, 1, 0.2f);
 #endif
                         table.foodStacks[0].foodStack.Push(f);
                         // await UniTask.Delay(300, cancellationToken: cancellationToken);
                         await Utility.CustomUniTaskDelay(0.3f, cancellationToken);
                     }


                     if (table.foodStacks[0].foodStack.Count == 0 && table.placedFoods[seatIndex] == null && !table.hasProblem)
                     {
                         bool none = true;
                         for (int i = 0; i < table.placedFoods.Length; i++)
                         {
                             if (i != seatIndex && table.placedFoods[i] != null && table.seats[i].animal == null)
                             {                               
                                 table.placedFoods[seatIndex] = table.placedFoods[i];
                                 table.placedFoods[i] = null;
                                 SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.ThrowSound(), 0.4f, 100, 5, trans.position);
                                 table.placedFoods[seatIndex].transform.DOJump(t, 1, 1, 0.2f);
                                 none = false;
                                 await Utility.CustomUniTaskDelay(0.3f, cancellationToken);
                                 break;
                             }
                         }
                         if (none)
                         {
                             SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.Sad(), 0.1f, 100, 5, trans.position);

                             if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
                             cancellationTokenSource = new CancellationTokenSource();
                             EmoteTimer(0, AnimationKeys.Trauma, true, cancellationTokenSource.Token).Forget();

                             FloatingEmote(4003);
                             if (GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation > 0)
                             {
                                 GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation--;
                                 GameInstance.GameIns.uiManager.reputation.text = GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation.ToString();
                                 GameInstance.GameIns.restaurantManager.restaurantCurrency.changed = true;
                                 GameInstance.GameIns.restaurantManager.CalculateSpawnTimer();
                             }
                             await Utility.CustomUniTaskDelay(3f, cancellationToken);
                             // await UniTask.Delay(3000, cancellationToken: cancellationToken);
                             customerState = CustomerState.Table;
                             animator.SetInteger("state", 0);
                             animal.PlayAnimation(AnimationKeys.Idle);
                             table.animals.Remove(this);
                             table.seats[index].animal = null;
                             customerCallback?.Invoke(this);
                             return;
                         }
                         else
                         {
                             if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
                             cancellationTokenSource = new CancellationTokenSource();
                             Emote(false, AnimationKeys.Normal);
                         }
                     }
                     //식사
                     if (!table.stolen)
                     {
                         while (table.foodStacks[0].foodStack.Count > 0 || table.placedFoods[seatIndex] != null || table.hasProblem)
                         {
                         GoUp:
                             if (!table.placed)
                             {
                                 goto StartTable;
                             }
                             if (table.seats[index].isDisEnabled)
                             {
                                 goto DisableSeat;
                             }
                             modelTrans.rotation = table.seats[index].transform.rotation;


                             Food f = null;

                             if (!table.hasProblem)
                             {
                                 if (table.placedFoods[seatIndex] == null)
                                 {
                                     bool getFood = false;
                                     for(int i = 0; i < table.seats.Length; i++)
                                     {
                                         if(seatIndex != i && table.seats[i].animal == null && table.placedFoods[i] != null)
                                         {
                                             table.placedFoods[seatIndex] = table.placedFoods[i];
                                             table.placedFoods[i] = null;
                                             SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.ThrowSound(), 0.4f, 100, 5, trans.position);
                                             table.placedFoods[seatIndex].transform.DOJump(t, 1, 1, 0.2f);
                                             getFood = true;
                                             await Utility.CustomUniTaskDelay(0.3f, cancellationToken);
                                             break;
                                         }
                                     }

                                     if (!getFood)
                                     {
                                         if (table.foodStacks[0].foodStack.Count == 0) break;
                                         //먹을 음식을 가까이 올려놓기
                                         f = table.foodStacks[0].foodStack.Pop();

                                         table.placedFoods[seatIndex] = f.gameObject;

                                         SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.ThrowSound(), 0.4f, 100, 5, trans.position);
                                         f.transforms.DOJump(t, 1, 1, 0.2f);
                                         //await UniTask.Delay(300, cancellationToken: cancellationToken);
                                         await Utility.CustomUniTaskDelay(0.3f, cancellationToken);
                                     }
                                 }
                                 else
                                 {
                                     f = table.placedFoods[seatIndex].GetComponent<Food>();
                                 }
                             }
                             else
                             {
                                 if(table.stealing && !stealing)
                                 {
                                     stealing = true;
                                     SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.Angry(), 0.1f, 100, 5, trans.position);
                                     if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
                                     cancellationTokenSource = new CancellationTokenSource();
                                     EmoteTimer(0, AnimationKeys.Sad, true, cancellationTokenSource.Token).Forget();
                                     FloatingEmote(4002);
                                 }
                             }
                             //음식을 빼앗길 때
                             if (f == null)
                             {
                                 while(table.stealing)
                                 {
                                     await UniTask.Delay(200, cancellationToken: cancellationToken);
                                 }
                                 goto Stealing;
                             }
                             float tm = RestaurantManager.restaurantTimer;
                             for (int i = 1; i <= 100; i++)
                             {
                                 if (!table.placed || table.seats[seatIndex].isDisEnabled)
                                 {
                                     animator.SetInteger(AnimationKeys.state, 0);
                                     animal.PlayAnimation(AnimationKeys.Idle);
                                     goto StartTable;
                                 }

                                 if (!table.hasProblem)
                                 {
                                     float timer = animalStruct.eat_speed;
                                     // if (RestaurantManager.restaurantTimer >= tm + timer / 10)
                                     if (i != 0 && i % 10 == 0)
                                     {
                                         tm = RestaurantManager.restaurantTimer;

                                         SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.Eat(), 0.05f, 100, 5, trans.position);
                                         animator.SetTrigger(AnimationKeys.eat);
                                         animal.PlayTriggerAnimation(AnimationKeys.Eat);

                                         Eat eat = ParticleManager.CreateParticle(ParticleType.Eating).GetComponent<Eat>();
                                         eat.transform.position = f.transforms.position;
                                         eat.PlayEating();
                                     }

                                     await Utility.CustomUniTaskDelay(timer / 100, cancellationToken);
                                 }
                                 else
                                 {
                                     while (table.hasProblem)
                                     {
                                         if (!table.placed || table.seats[seatIndex].isDisEnabled)
                                         {

                                     *//*        animator.SetInteger(AnimationKeys.state, 0);
                                             animal.PlayAnimation(AnimationKeys.Idle);*//*
                                             goto StartTable;
                                         }

                                         if (table.stealing && !stealing)
                                         {
                                             stealing = true;
                                             SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.Angry(), 0.1f, 100, 5, trans.position);
                                             if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
                                             cancellationTokenSource = new CancellationTokenSource();
                                             EmoteTimer(0, AnimationKeys.Sad, true, cancellationTokenSource.Token).Forget();
                                             FloatingEmote(4002);

                                         }

                                         if (table.stolen)
                                         {
                                             SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.Sad(), 0.1f, 100, 5, trans.position);

                                             if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
                                             cancellationTokenSource = new CancellationTokenSource();
                                             EmoteTimer(0, AnimationKeys.Trauma, true, cancellationTokenSource.Token).Forget();

                                             FloatingEmote(4003);
                                             if (GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation > 0)
                                             {
                                                 GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation--;
                                                 GameInstance.GameIns.uiManager.reputation.text = GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation.ToString();
                                                 GameInstance.GameIns.restaurantManager.restaurantCurrency.changed = true;
                                                 GameInstance.GameIns.restaurantManager.CalculateSpawnTimer();
                                             }
                                             await Utility.CustomUniTaskDelay(3f, cancellationToken);
                                             // await UniTask.Delay(3000, cancellationToken: cancellationToken);
                                             customerState = CustomerState.Table;
                                             animator.SetInteger("state", 0);
                                             animal.PlayAnimation(AnimationKeys.Idle);
                                             table.animals.Remove(this);
                                             table.seats[index].animal = null;
                                             customerCallback?.Invoke(this);
                                             return;
                                         }
                                         else
                                         {
                                             Debug.Log("AA");
                                             await Utility.CustomUniTaskDelay(0.2f, cancellationToken);
                                             // await UniTask.Delay(200, cancellationToken: cancellationToken);
                                         }

                                     }

                                     await UniTask.Delay(200, cancellationToken: cancellationToken);
                                     Debug.Log("BB");
                                     Emote(false, AnimationKeys.Normal);
                                     animator.SetInteger("state", 0);
                                     animal.PlayAnimation(AnimationKeys.Idle);

                                     for (int j = 0; j < table.seats.Length; j++)
                                     {
                                         if (table.seats[j].animal == null)
                                         {
                                             if (table.placedFoods[j] != null)
                                             {
                                                 if (!table.placed || table.seats[seatIndex].isDisEnabled)
                                                 {
                                                     animator.SetInteger(AnimationKeys.state, 0);
                                                     animal.PlayAnimation(AnimationKeys.Idle);
                                                     goto StartTable;
                                                 }

                                                 table.placedFoods[seatIndex] = table.placedFoods[j];
                                                 table.placedFoods[j] = null;
                                                 table.placedFoods[seatIndex].transform.DOKill();
                                                 table.placedFoods[seatIndex].transform.DOJump(t, 1, 1, 0.2f);

                                                 // await UniTask.Delay(300, cancellationToken: cancellationToken);
                                                 await Utility.CustomUniTaskDelay(0.3f, cancellationToken);
                                                 goto GoUp;

                                             }
                                         }
                                     }

                                 }
                                 //if (table.foodStacks[0].foodStack.Count == 0) break;

                             }
                             FoodManager.EatFood(f);
                             table.placedFoods[seatIndex] = null;

                             table.numberOfFoods--;
                             table.numberOfFoods = table.numberOfFoods < 0 ? 0 : table.numberOfFoods;
                             table.numberOfGarbage++;



                             if (table.foodStacks[0].foodStack.Count == 0 && table.numberOfFoods == 0)
                             {
                                 table.numberOfGarbage = table.numberOfGarbage > 10 ? 10 : table.numberOfGarbage;
                                 table.isDirty = true;

                                 for (int i = 0; i < table.numberOfGarbage; i++)
                                 {
                                     Garbage go = GarbageManager.CreateGarbage();
                                     go.transforms.SetParent(table.trashPlate.transforms);
                                     table.garbageList.Add(go);

                                     float x = Random.Range(-1f, 1f);
                                     float z = Random.Range(-1f, 1f);
                                     go.transforms.position = table.up.position + GameInstance.GetVector3(x, 0, z);
                                 }
                             }
                             else
                             {
                                 continue;
                             }

                             await Utility.CustomUniTaskDelay(0.2f, cancellationToken);
                         }

                         customerState = CustomerState.Table;
                         await Utility.CustomUniTaskDelay(0.5f, cancellationToken);

                         int reputation = Random.Range(0, 10);
                         int targetRep = animalPersonality == AnimalPersonality.Loyal ? 10 : (animalPersonality == AnimalPersonality.HardToPlease ? 5 : 8);

                         if (reputation < targetRep)
                         {
                             int gradeUp = Random.Range(0, 100);

                             if (gradeUp == 0)
                             {
                                 //등급업
                                 // if (AnimalManager.gatchaTiers[id].Item1 < 4)
                                 {
                                     int tier = AnimalManager.gatchaTiers[animalStruct.id].Item1;
                                     (int, List<int>) tmp = AnimalManager.gatchaTiers[animalStruct.id];
                                     int personality = Random.Range(0, 7);
                                     bool success = tmp.Item1 < 4 ? true : false;
                                     if (success) tmp.Item1++;
                                     tmp.Item2[personality] = 1;
                                     AnimalManager.gatchaTiers[animalStruct.id] = tmp;

                                     if (success)
                                     {
                                         GameInstance.GameIns.gatcharManager.popup.SetActive(true);
                                         SoundManager.Instance.PlayAudio(GameInstance.GameIns.gatchaSoundManager.GradeUp(), 0.4f);

                                         GameInstance.GameIns.gatcharManager.popup_TierUp.SetActive(true);

                                         AnimalStruct asset = AssetLoader.animals[animalStruct.id];
                                         string n = asset.asset_name + "_Sprite";
                                         GameInstance.GameIns.gatcharManager.TierUpAnimalImage.sprite = AssetLoader.loadedSprites[n];
                                         if (tier == 2) GameInstance.GameIns.gatcharManager.backGlow.color = Color.blue;
                                         else if (tier == 3) GameInstance.GameIns.gatcharManager.backGlow.color = new Color(0.5f, 0f, 0.5f);
                                         else if (tier == 4) GameInstance.GameIns.gatcharManager.backGlow.color = Color.yellow;
                                         GameInstance.GameIns.gatcharManager.SetPrice();
                                         SaveLoadSystem.SaveGatchaAnimalsData();

                                         GameInstance.GameIns.gatcharManager.CheckGameClear();

                                     }
                                 }
                             }
                             else if (gradeUp == 1)
                             {
                                 //새로운 손님 추가
                                 if (App.CurrentLevel == 0)
                                 {
                                     int randomCustomer = Random.Range(100, 106);
                                     if (!AnimalManager.gatchaTiers.ContainsKey(randomCustomer))
                                     {
                                         GameInstance.GameIns.gatcharManager.popup.SetActive(true);
                                         SoundManager.Instance.PlayAudio(GameInstance.GameIns.gatchaSoundManager.Unlock(), 0.4f);
                                         (int, List<int>) tmp = (1, new List<int>());
                                         int personality = Random.Range(0, 7);
                                         for (int i = 0; i < 7; i++) tmp.Item2.Add(0);
                                         tmp.Item2[personality] = 1;
                                         AnimalManager.gatchaTiers[randomCustomer] = tmp;
                                         AnimalStruct asset = AssetLoader.animals[randomCustomer];
                                         string n = asset.asset_name + "_Sprite";
                                         GameInstance.GameIns.gatcharManager.NewAnimalImage.sprite = AssetLoader.loadedSprites[n];// this.sprites[pair.Key];
                                         GameInstance.GameIns.gatcharManager.popup_NewCustomer.SetActive(true);
                                         AnimalManager.animalStructs[randomCustomer] = asset;
                                         GameInstance.GameIns.gatcharManager.SetPrice();
                                         SaveLoadSystem.SaveGatchaAnimalsData();

                                         GameInstance.GameIns.gatcharManager.CheckGameClear();
                                     }
                                 }
                             }

                             SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.Happy(), 0.1f, 100, 5, trans.position);
                             if (GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation < 100)
                             {
                                 GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation += 1;
                                 GameInstance.GameIns.uiManager.reputation.text = GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation.ToString();
                                 GameInstance.GameIns.restaurantManager.CalculateSpawnTimer();
                                 GameInstance.GameIns.restaurantManager.restaurantCurrency.changed = true;
                             }
                             FloatingEmote(4000);
                             if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
                             cancellationTokenSource = new CancellationTokenSource();
                             EmoteTimer(5f, AnimationKeys.Happy, false, cancellationTokenSource.Token).Forget();
                         }

                         await Utility.CustomUniTaskDelay(3f, cancellationToken);
                         animator.SetInteger("state", 0);
                         animal.PlayAnimation(AnimationKeys.Idle);
                         //PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
                         table.animals.Remove(this);
                         table.seats[index].animal = null;
                         customerCallback?.Invoke(this);
                         //  GameInstance.GameIns.animalManager.AttacCustomerTask(this);
                     }
                     else
                     {
                         SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.Sad(), 0.1f, 100, 5, trans.position);

                         if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
                         cancellationTokenSource = new CancellationTokenSource();
                         EmoteTimer(0, AnimationKeys.Trauma, true, cancellationTokenSource.Token).Forget();

                         FloatingEmote(4003);
                         if (GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation > 0)
                         {
                             GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation--;
                             GameInstance.GameIns.uiManager.reputation.text = GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation.ToString();
                             GameInstance.GameIns.restaurantManager.restaurantCurrency.changed = true;
                             GameInstance.GameIns.restaurantManager.CalculateSpawnTimer();
                         }
                         await Utility.CustomUniTaskDelay(3f, cancellationToken);
                         // await UniTask.Delay(3000, cancellationToken: cancellationToken);
                         customerState = CustomerState.Table;
                         animator.SetInteger("state", 0);
                         animal.PlayAnimation(AnimationKeys.Idle);
                         table.animals.Remove(this);
                         table.seats[index].animal = null;
                         customerCallback?.Invoke(this);
                         return;
                     }

                     await UniTask.NextFrame(cancellationToken: cancellationToken);
                 }
                 else
                 {
                     await Customer_Move(moveTargets, position, cancellationToken: cancellationToken);
                     continue;
                 }
                 return;
             }*/
            }
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Customer_Table task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Customer_Table: {ex.Message}");
            throw;
        }
    }

    async UniTask Customer_GoHome(Vector3 position, CancellationToken cancellationToken = default)
    {
        try
        {
            while (true)
            {
                reCalculate = false;
                cancellationToken.ThrowIfCancellationRequested();
                Stack<Vector3> moveTargets = await CalculateNodes_Async(position, false, cancellationToken);
                if (moveTargets != null && moveTargets.Count > 0)
                //    if (moveNode != null)
                {
                    Vector3 test = moveTargets.Peek();
                    if (test.z == 100 && test.x == 100)
                    {
                        // moveNode = null;
                    }
                    else
                    {
                    //    customerAction = CustomerAction.CustomerGoToHome;
                        await Customer_Move(moveTargets, position, cancellationToken: cancellationToken);
                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }
                    }

             //       bStart = false;
                    busy = false;
              //      customerAction = CustomerAction.NONE;
                    GameInstance.GameIns.animalManager.DespawnCustomer(this);
                }
                else
                {
                    await Customer_Move(moveTargets, position, cancellationToken: cancellationToken);
                    continue;
                }
                return;
            }
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Customer_GoHome task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Customer_GoHome: {ex.Message}");
            throw;
        }
    }

    void Wait()
    {
        if (waitTimer > 0)
        {
            waitTimer-=Time.deltaTime;
        }
        else
        {
            waitTimer = 0;
            //customerAction = CustomerAction.NONE;
            GameInstance.GameIns.animalManager.AttachCustomerTask(this);
        }
    }

    async UniTask EmoteTimer(float delay, string emote, bool continues, CancellationToken cancellationToken = default)
    {
        try
        {
            Emote(true, emote);
            if (!continues)
            {
                await Utility.CustomUniTaskDelay(delay, cancellationToken);
            //    await UniTask.Delay(delay, cancellationToken: cancellationToken);
                Emote(false, AnimationKeys.Normal);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
    void Emote(bool bStart, string emote)
    {
        animal.CheckVisible(!bStart, true);
        // meshRenderer.enabled = true;
        animal.lodController.GetRenderer(1).enabled = bStart;
        animal.lodController.GetRenderer(2).enabled = bStart;
        //    animal.lodController.animator.SetInteger(AnimationKeys.emotion, state);
        animal.lodController.animator.SetTrigger(emote);
    }

    public override void SetDefault()
    {
        base.SetDefault();
        animalStruct = new AnimalStruct();
        eatParticle = null;
        animalSpawner = null;
        nodes.Clear();
        foodStacks.Clear();
        currentAnimal = null;
    }


    void FloatingEmote(int key)
    {
        Emote e = GameInstance.GameIns.restaurantManager.GetEmote();
        e.rectTransform.position = trans.position + Vector3.up * 4;
        e.image.sprite = GameInstance.GameIns.restaurantManager.emoteSprites[key];
        e.height = 3;
        e.Emotion(1.5f);
    }
}
