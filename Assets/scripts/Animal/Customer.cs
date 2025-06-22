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

            //foodsAnimalsWant = foodsAnimals;
            /*minOrder = gameObject.GetComponentInParent<Animal>().minOrder;
            maxOrder = gameObject.GetComponentInParent<Animal>().maxOrder;
            likeFood = gameObject.GetComponentInParent<Animal>().likeFood;
            hateFood = gameObject.GetComponentInParent<Animal>().hateFood;
            hateFood = 0;*/
         //   Debug.Log("hate" + hateFood);
          /*  if (foodsAnimals.burger)
            {
                FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
                ClearPlate(foodStack);
                foodStack.needFoodNum = UnityEngine.Random.Range(animalStruct.min_order, animalStruct.max_order);
                foodStack.type = MachineType.BurgerMachine;
                foodStacks.Add(foodStack);
            }

            if (foodsAnimals.coke)
            {
                FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
                ClearPlate(foodStack);
                foodStack.needFoodNum = UnityEngine.Random.Range(animalStruct.min_order, animalStruct.max_order);
                foodStack.type = MachineType.CokeMachine;
                foodStacks.Add(foodStack);
            }

            if (foodsAnimals.coffee)
            {
                FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
                ClearPlate(foodStack);
                foodStack.needFoodNum = UnityEngine.Random.Range(animalStruct.min_order, animalStruct.max_order);
                foodStack.type = MachineType.CoffeeMachine;
                foodStacks.Add(foodStack);
            }

            if (foodsAnimals.donut)
            {
                FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
                ClearPlate(foodStack);
                foodStack.needFoodNum = UnityEngine.Random.Range(animalStruct.min_order, animalStruct.max_order);
                foodStack.type = MachineType.DonutMachine;

                foodStacks.Add(foodStack);
            }*/
        }
    }

    void Order()
    {
        for (int i = foodStacks.Count - 1; i >= 0; i--)
        {
            FoodStack foodStack = foodStacks[i];
            foodStacks.RemoveAt(i);
            FoodStackManager.FM.RemoveFoodStack(foodStack);

        }
        if (foodsAnimalsWant.burger)
        {
            FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
            // ClearPlate(foodStack);
            foodStack.needFoodNum = UnityEngine.Random.Range(animalStruct.min_order, animalStruct.max_order);
            foodStack.type = MachineType.BurgerMachine;
            foodStacks.Add(foodStack);
        }

        if (foodsAnimalsWant.coke)
        {
            FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
            //ClearPlate(foodStack);
            foodStack.needFoodNum = UnityEngine.Random.Range(animalStruct.min_order, animalStruct.max_order);
            foodStack.type = MachineType.CokeMachine;
            foodStacks.Add(foodStack);
        }

        if (foodsAnimalsWant.coffee)
        {
            FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
            //   ClearPlate(foodStack);
            foodStack.needFoodNum = UnityEngine.Random.Range(animalStruct.min_order, animalStruct.max_order);
            foodStack.type = MachineType.CoffeeMachine;
            foodStacks.Add(foodStack);
        }

        if (foodsAnimalsWant.donut)
        {
            FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
            //  ClearPlate(foodStack);
            foodStack.needFoodNum = UnityEngine.Random.Range(animalStruct.min_order, animalStruct.max_order);
            foodStack.type = MachineType.DonutMachine;

            foodStacks.Add(foodStack);
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



                                CustomerPlayAction(target, tableList[i], j);
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

    public void CustomerPlayAction(Vector3 position, Table table, int index)
    {
        Customer_Table(position, table, index, App.GlobalToken).Forget();
  
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
            int queueindex = 0;

            for(int i = position.Length - 1; i >= 0; i--)
            {
                if(position[i].controller == null)
                {
                    queueindex = i;
                }
                else
                {
                    break;
                }
            }
            Vector3 pos = position[queueindex].transforms.position;

            int currentPoint = queueindex;// position.Length - 1;
            position[queueindex].controller = this;
            Stack<Vector3> moveTargets = await CalculateNodes_Async(pos, false, cancellationToken);

            await Customer_Move(moveTargets, pos, true, cancellationToken: cancellationToken);
            modelTrans.rotation = position[currentPoint].transforms.rotation;

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

                while(true)
                {
                    if (Vector3.Distance(trans.position, p) < 0.001f) break;

                    animator.SetInteger("state", 1);
                    animal.PlayAnimation(AnimationKeys.Walk);
                    trans.position = Vector3.MoveTowards(trans.position, p, animalStruct.speed * Time.deltaTime);
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                }
            }

                //요구 사항 표시
            counter.customer = this;
            animator.SetInteger("state", 0);
            // PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
            animal.PlayAnimation(AnimationKeys.Idle);
            //음식 주문
            Order();
            GameInstance.GameIns.uiManager.UpdateOrder(this, counter.counterType);

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
                    int animalTier = AnimalManager.gatchaTiers[animalStruct.id];
                    animalTier = (animalTier - 1) * 2;
                    animalTier = animalTier == 0 ? 1 : animalTier;
                    BigInteger price = v.foodPrice * animalTier * AnimalManager.gatchaValues;
                    int leftover = (int)(price % 100);
                    price /= 100;
                    foodPrices += price;
                    GameInstance.GameIns.restaurantManager.restaurantCurrency.leftover += leftover;
                    if (GameInstance.GameIns.restaurantManager.restaurantCurrency.leftover >= 100)
                    {
                        GameInstance.GameIns.restaurantManager.restaurantCurrency.leftover -= 100;
                        foodPrices += 1;
                    }

                    int tip = UnityEngine.Random.Range(1, 11);
                    if (tip == 1) tipNum++;
                    foodNum++;
                }

            }

            stringBuilder = Utility.GetFormattedMoney(foodPrices, stringBuilder);

            FloatingCost fc = GameInstance.GameIns.restaurantManager.GetFloatingCost();
         
            fc.rectTransform.position = InputManger.cachingCamera.WorldToScreenPoint(trans.position) + Vector3.up * 50;
            fc.text.text = stringBuilder.ToString();
            fc.height = 100;
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

            tables = tables
                    .OrderBy(fm => (fm.transforms.position - trans.position).magnitude) // 음식 개수 내림차순
                    .ToList();

            while (true)
            {
                await UniTask.Delay(500, cancellationToken: cancellationToken);

                foreach (Table t in tables)
                {
                    if (t.isDirty == false)
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

                        counter.customer = null;
                        position[0].controller = null;

                        if (index == -1)
                        {
                            float min = 9999;
                            Seat selectedSeat = null;
                            for (int j = 0; j < t.seats.Length; j++)
                            {
                                float cur = Vector3.Distance(trans.position, t.seats[j].transform.position);
                                if (min > cur)
                                {
                                    min = cur;
                                    selectedSeat = t.seats[j];
                                }
                            }

                            selectedSeat.animal = this;
                            t.numberOfFoods += foodNum;
                        }
                        else
                        {
                            int nextIndex = index + 2;
                            nextIndex = nextIndex > 3 ? nextIndex - 4 : nextIndex;
                            t.seats[nextIndex].animal = this;
                            t.numberOfFoods += foodNum;
                         
                        }

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
    
    async UniTask Customer_Move(Stack<Vector3> n, Vector3 loc, bool standInline = false, QueuePoint point = null, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
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
                while(true)
                {
                    if (App.restaurantTimeScale == 1)
                    {
                        if (!standInline && reCalculate)
                        {
                            Debug.Log("reCalculate");
                            return;
                        }
                        if (trans == null || !trans)
                        {
                            Debug.LogError("No Trans Find");
                            await UniTask.NextFrame();
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
                        trans.position = Vector3.MoveTowards(trans.position, target, animalStruct.speed * Time.deltaTime);
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

            Vector3 newLoc = loc;
            newLoc.y = 0;

            while (true)
            {
                if (App.restaurantTimeScale == 1)
                {
                    animator.SetInteger("state", 1);
                    animal.PlayAnimation(AnimationKeys.Walk);
                    trans.position = Vector3.MoveTowards(trans.position, newLoc, animalStruct.speed * Time.deltaTime);
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

    async UniTask Customer_Table(Vector3 position, Table table, int index, CancellationToken cancellationToken = default)
    {
        try
        {
            Vector3 tablePos = table.transforms.position;
            float offsetZ = index % 2 == 0 ? 1 : 0;
            float offsetSize = index / 2 == 0 ? 1 : -1;
            float offsetX = index % 2 == 1 ? 1 : 0;
            Vector3 t = new Vector3(tablePos.x + offsetX * offsetSize, tablePos.y, tablePos.z + offsetZ * offsetSize);
            t.y = 0.5f;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Stack<Vector3> moveTargets = await CalculateNodes_Async(position, false, cancellationToken);
                if (moveTargets != null && moveTargets.Count > 0)
                // if (moveNode != null)
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
                            continue;
                        }
                                                
                        modelTrans.rotation = table.seats[index].transform.rotation;
                    }
                  

                    seatIndex = index;
                    tb = table;
                    //customerAction = CustomerAction.CustomerTable;
                    //  await UniTask.Delay(200, cancellationToken: cancellationToken);
                    await Utility.CustomUniTaskDelay(0.2f, cancellationToken);

                    for (int i = VisualizingFoodStack.Count - 1; i >= 0; i--)
                    {
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

                    //식사
                   
                   // PlayAnim(animal.animationDic["Eat"], "Eat");
                    GameObject particle = ParticleManager.CreateParticle();
                    
                    while (table.foodStacks[0].foodStack.Count > 0)
                    {
                    GoUp:
                        /* for (int i = 0; i < 100; i++)
                         {
                             int timer = (int)(10 * animalStruct.eat_speed);
                             await UniTask.Delay(timer);
                             if (table.foodStacks[0].foodStack.Count == 0) break;
                         }*/
                        Food f = null;
                        if (table.placedFoods[seatIndex] == null)
                        {
                            if (table.foodStacks[0].foodStack.Count == 0) break;
                            //먹을 음식을 가까이 올려놓기
                            f = table.foodStacks[0].foodStack.Pop();

                            table.placedFoods[seatIndex] = f.gameObject;

                            //animal.audioSource.clip = GameInstance.GameIns.gameSoundManager.ThrowSound();
                            //   animal.audioSource.Play();
                            SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.ThrowSound(), 0.4f, 100, 5, trans.position);
                            f.transforms.DOJump(t, 1, 1, 0.2f);
                            //await UniTask.Delay(300, cancellationToken: cancellationToken);
                            await Utility.CustomUniTaskDelay(0.3f, cancellationToken);
                        }
                        else
                        {
                            f = table.placedFoods[seatIndex].GetComponent<Food>();
                        }
                        float tm = RestaurantManager.restaurantTimer;
                        bool stealing = false;
                        bool stolen = false;
                        for (int i = 0; i < 100; i++)
                        {
                            
                            if (!table.hasProblem)
                            {
                                float timer = animalStruct.eat_speed;
                                if (RestaurantManager.restaurantTimer >= tm + timer / 10)
                                {
                                    tm = RestaurantManager.restaurantTimer;

                                    SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.Eat(), 0.05f, 100, 5, trans.position);
                                    animator.SetTrigger(AnimationKeys.eat);
                                    // animator.SetInteger("state", 2);
                                    animal.PlayTriggerAnimation(AnimationKeys.Eat);
                                }
                                //await UniTask.NextFrame(cancellationToken: cancellationToken); //Utility.CustomUniTaskDelay(timer, cancellationToken);
                                //await UniTask.Delay((int)(timer * 10), cancellationToken: cancellationToken);
                                await Utility.CustomUniTaskDelay(timer / 100, cancellationToken);
                            }
                            else
                            {
                                while(table.hasProblem)
                                {
                                    if(table.stealing && !stealing)
                                    {
                                        stealing = true;
                                        SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.Angry(), 0.1f, 100, 5, trans.position);
                                        if(cancellationTokenSource != null) cancellationTokenSource.Cancel();
                                        cancellationTokenSource = new CancellationTokenSource();
                                        EmoteTimer(0, AnimationKeys.Sad, true, cancellationTokenSource.Token).Forget();
                                        FloatingEmote(4002);
                                        if(GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation > 0)
                                        {
                                            GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation--;
                                            GameInstance.GameIns.restaurantManager.CalculateSpawnTimer();
                                        }
                                    }

                                 /*   int remains = 0;
                                    for(int k  = 0; k < table.placedFoods.Length; k++)
                                    {
                                        if(table.placedFoods[k] != null) remains++;
                                    }
                                    remains += table.foodStacks[0].foodStack.Count;*/
                                    if (table.stolen)
                                    {
                                        SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.Sad(), 0.1f, 100, 5, trans.position);
                                      
                                        if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
                                        cancellationTokenSource = new CancellationTokenSource();
                                        EmoteTimer(0, AnimationKeys.Trauma, true, cancellationTokenSource.Token).Forget();

                                        FloatingEmote(4003);

                                        await Utility.CustomUniTaskDelay(3f, cancellationToken);
                                       // await UniTask.Delay(3000, cancellationToken: cancellationToken);
                                        customerState = CustomerState.Table;
                                        animator.SetInteger("state", 0);
                                        animal.PlayAnimation(AnimationKeys.Idle);
                                        table.seats[index].animal = null;
                                        customerCallback?.Invoke(this);
                                        return;
                                    }
                                    else
                                    {
                                        await Utility.CustomUniTaskDelay(0.2f, cancellationToken);
                                       // await UniTask.Delay(200, cancellationToken: cancellationToken);
                                    }

                                }

                                Emote(false, AnimationKeys.Normal);

                                for (int j =0; j< table.seats.Length; j++)
                                {
                                    if (table.seats[j].animal == null)
                                    {
                                        if(table.placedFoods[j] != null)
                                        {
                                            table.placedFoods[seatIndex] = table.placedFoods[j];
                                            table.placedFoods[j] = null;
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
                        particle.gameObject.transform.position = mousePoint.position;

                        particle.GetComponent<ParticleSystem>().Play();

                       // await UniTask.Delay(200, cancellationToken: cancellationToken);
                        await Utility.CustomUniTaskDelay(0.2f, cancellationToken);

                    }
                    customerState = CustomerState.Table;
                    await Utility.CustomUniTaskDelay(0.5f, cancellationToken);
                   // await UniTask.Delay(500, cancellationToken: cancellationToken);
                    ParticleManager.ClearParticle(particle);
                    SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.Happy(), 0.1f, 100, 5, trans.position);
                    int reputation = Random.Range(0, 10);
                    if(reputation < 5)
                    {
                        GameInstance.GameIns.restaurantManager.restaurantCurrency.reputation += 1;
                        GameInstance.GameIns.restaurantManager.CalculateSpawnTimer();
                        GameInstance.GameIns.restaurantManager.restaurantCurrency.changed = true;
                    }
                    FloatingEmote(4000);

                    if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
                    cancellationTokenSource = new CancellationTokenSource();
                    EmoteTimer(5f, AnimationKeys.Happy, false, cancellationTokenSource.Token).Forget();
                  //  await UniTask.Delay(3000, cancellationToken: cancellationToken);
                    await Utility.CustomUniTaskDelay(3f, cancellationToken);
                    animator.SetInteger("state", 0);
                    animal.PlayAnimation(AnimationKeys.Idle);
                    //PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
                    table.seats[index].animal = null;
                    customerCallback?.Invoke(this);
                  //  GameInstance.GameIns.animalManager.AttacCustomerTask(this);
                }
                else
                {
                    Debug.Log("Wait" + position);
                    //customerAction = CustomerAction.ProcessWait;
                }
                return;
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
        e.rectTransform.position = InputManger.cachingCamera.WorldToScreenPoint(trans.position) + Vector3.up * 50;
        e.image.sprite = GameInstance.GameIns.restaurantManager.emoteSprites[key];
        e.height = 100;
        e.Emotion();
    }
}
