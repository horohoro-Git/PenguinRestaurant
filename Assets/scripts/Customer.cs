using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
#if HAS_DOTWEEN
using DG.Tweening;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using UnityEngine;

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

    int currentWaypointIndex;
    bool hasMoney;
    float eatingTimer;

    private float speed;
    private float eatSpeed;
    private int minOrder;
    private int maxOrder;

    string animalWaitActions;

    private int likeFood;
    private int hateFood;
    List<Node> nodes = new List<Node>();

    bool bStart = false;
    int moveStart = 0;

    bool animalMove = false;
    float actionTimer = 0;
    Node moveNode;
    bool doOnce = false;
    CustomerAction customerAction = CustomerAction.NONE;
    QueuePoint[] Qpositions;
    Counter ct;
    Table tb;
    int seatIndex;
    int animalEat;

    public AnimalStruct animalStruct { get; set; }

    public List<FoodStack> foodStacks = new List<FoodStack>();
    Transform transforms;

    [NonSerialized]
    public AnimalSpawner animalSpawner;

    [NonSerialized]
    public int customerIndex;


    public Animal currentAnimal;
    private void Awake()
    {
        transforms = transform;
        currentWaypointIndex = 5;
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
        if (r == 0) animalWaitActions = "Idle_A";
        else if (r == 1) animalWaitActions = "Bounce";
        else animalWaitActions = "LookAround";

        hasMoney = true;
        currentWaypointIndex = 5;
 
        if (foodsAnimals.spawnerType == AnimalSpawner.SpawnerType.Delivery)
        {
            WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;

            for (int i = 0; i < workSpaceManager.counters.Count; i++)
            {
                if (workSpaceManager.counters[i].counterType == Counter.CounterType.Delivery)
                {
                    workSpaceManager.counters[i].customer = this;
                    transforms.position = workSpaceManager.counters[i].transforms.position;
             
                    FoodStack foodStack = FoodStackManager.FM.GetFoodStack();
                    ClearPlate(foodStack);
                    foodStack.needFoodNum = 8;
                    foodStack.type = MachineType.PackingTable;
                    foodStacks.Add(foodStack);
                    GameInstance.GameIns.uiManager.UpdateOrder(this, Counter.CounterType.Delivery);
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
                            if (tableList[i].seats[j].customer == this)
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
            endList = workSpaceManager.endPoints;
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
            }
            busy = true;
           
            CustomerPlayAction(endList[n].transforms.position);
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
        customerAction = CustomerAction.CustomerCounter;
        Customer_Counter(position, counter, App.GlobalToken).Forget();
        //StartCoroutine(CustomerWalkToCounter(position, counter));
    }

    public void CustomerPlayAction(Vector3 position, Table table, int index)
    {
        Customer_Table(position, table, index, App.GlobalToken).Forget();
  
    }

    public void CustomerPlayAction(Vector3 position)
    {
      //  Debug.Log("A");
        Customer_GoHome(position, App.GlobalToken).Forget();
    }

    async UniTask Customer_Wait(CancellationToken cancellationToken = default)
    {
        try
        {
            await UniTask.Delay(200, cancellationToken: cancellationToken);
            GameInstance.GameIns.animalManager.AttacCustomerTask(this);
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
            for (int i = position.Length - 1; i >= 0; i--)
            {

                while (position[i].controller != null)
                {
                    animator.SetInteger("state", 0);
                    PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
                    await UniTask.Delay(200, cancellationToken: cancellationToken);
                }

                if (position[i].controller == null && i > 0 && position[i - 1].controller == null) continue;

                Vector3 pos = position[i].transforms.position;
                if (i + 1 <= position.Length - 1)
                {
                    if (position[i + 1].controller == this) position[i + 1].controller = null;
                }
                position[i].controller = this;
                moveNode = await CalculateNodes_Async(pos, cancellationToken);

                await Customer_Move(moveNode, pos, true, cancellationToken);
                if (reCalculate) reCalculate = false;
                modelTrans.rotation = position[i].transforms.rotation;
                animatedAnimal.transforms.rotation = position[i].transforms.rotation;

                await UniTask.Delay(200, cancellationToken: cancellationToken);
            }

            //요구 사항 표시
            counter.customer = this;
            animator.SetInteger("state", 0);
            PlayAnim(animal.animationDic["Idle_A"], "Idle_A");

            //음식 주문
            Order();
            GameInstance.GameIns.uiManager.UpdateOrder(this, counter.counterType);

            //받은 음식 체크
            while (true)
            {
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
                await UniTask.Delay(200, cancellationToken: cancellationToken);
            }

            GameInstance.GameIns.uiManager.UpdateOrder(this, counter.counterType);

            //비용 지불
            float foodPrices = 0;
            int tipNum = 0;
            int foodNum = 0;
            for (int j = 0; j < foodStacks.Count; j++)
            {
                for (int k = 0; k < foodStacks[j].foodStack.Count; k++)
                {
                    //         foodPrices += foodStacks[j].foodStack[k].foodPrice;
                    int tip = UnityEngine.Random.Range(1, 11);
                    if (tip == 1) tipNum++;

                    foodNum++;
                }
            }

            if (hasMoney)
            {
                GameInstance.GameIns.restaurantManager.playerData.money += foodPrices;
                GameInstance.GameIns.restaurantManager.playerData.fishesNum += tipNum;
                GameInstance.GameIns.uiManager.UpdateMoneyText(GameInstance.GameIns.restaurantManager.playerData.money);

                SaveLoadManager.Save(SaveLoadManager.SaveState.ONLY_SAVE_PLAYERDATA);
                hasMoney = false;
            }

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
                        for (int j = 0; j < t.seats.Length; j++)
                        {
                            if ((t.seats[j].customer == null || t.seats[j].customer == this))
                            {
                                counter.customer = null;
                                position[0].controller = null;
                                t.seats[j].customer = this;
                                t.numberOfFoods += foodNum;
                                customerState = CustomerState.Counter;
                                busy = false;
                                GameInstance.GameIns.animalManager.AttacCustomerTask(this);
                                return;
                            }
                        }
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
    async UniTask Customer_Move(Node n, Vector3 loc = new Vector3(), bool standInline = false, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            Node a = n.parentNode == null ? n : n.parentNode;
            Stack<Node> stack = new Stack<Node>();
            while (a != null)
            {
                if (a.parentNode != null)
                {
                    stack.Push(a);

                }

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
                    if (!standInline && reCalculate)
                    {
                        return;
                    }
                    if (trans == null || !trans)
                    {
                        await UniTask.NextFrame();
                        return;
                    }
                    Debug.DrawLine(trans.position, target, Color.red, 0.1f);
                    //  moveCTS.Token.ThrowIfCancellationRequested();
                    animator.SetInteger("state", 1);
                    PlayAnim(animal.animationDic["Run"], "Run");
                    cur = (target - trans.position).magnitude;
                    Vector3 dir = (target - trans.position).normalized;
                    trans.position = Vector3.MoveTowards(trans.position, target, animalStruct.speed * Time.deltaTime);
                    float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                    modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                    animatedAnimal.transforms.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                }
            }

            if (standInline)
            {
                float cu2 = (loc - trans.position).magnitude;

                while (cu2 > 0.1f)
                {
                    if (trans == null || !trans)
                    {
                        await UniTask.NextFrame();
                        return;
                    }
                    Debug.DrawLine(trans.position, loc, Color.red, 0.1f);
                    //  moveCTS.Token.ThrowIfCancellationRequested();
                    animator.SetInteger("state", 1);
                    PlayAnim(animal.animationDic["Run"], "Run");
                    cu2 = (loc - trans.position).magnitude;
                    Vector3 dir = (loc - trans.position).normalized;
                    trans.position = Vector3.MoveTowards(trans.position, loc, animalStruct.speed * Time.deltaTime);

                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                }
            }
            animator.SetInteger("state", 0);
            PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
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
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                moveNode = await CalculateNodes_Async(position, cancellationToken);

                if (moveNode != null)
                {
                    if (moveNode.r == 100 && moveNode.c == 100) moveNode = null;
                    else
                    {
                        await Customer_Move(moveNode, cancellationToken: cancellationToken);
                        if (reCalculate)
                        {
                            reCalculate = false;
                            continue;
                        }
                        modelTrans.rotation = table.seats[index].transforms.rotation;
                        animatedAnimal.transforms.rotation = modelTrans.rotation;
                    }
                    seatIndex = index;
                    tb = table;
                    customerAction = CustomerAction.CustomerTable;
                    await UniTask.Delay(200, cancellationToken: cancellationToken);

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
                        Vector3 t = table.transforms.position + GameInstance.GetVector3(0, 0.7f + table.foodStacks[0].foodStack.Count, 0);
#if HAS_DOTWEEN
                        f.transforms.DOJump(t, r, 1, 0.2f);
#endif
                        table.foodStacks[0].foodStack.Push(f);
                        await UniTask.Delay(300, cancellationToken: cancellationToken);
                    }

                    //식사
                    animator.SetInteger("state", 2);
                    PlayAnim(animal.animationDic["Eat"], "Eat");
                    GameObject particle = ParticleManager.CreateParticle();
                    while (table.foodStacks[0].foodStack.Count > 0)
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            int timer = (int)(10 * animalStruct.eat_speed);
                            await UniTask.Delay(timer);
                            if (table.foodStacks[0].foodStack.Count == 0) break;
                        }
                        if (table.foodStacks[0].foodStack.Count == 0) break;
                        Food f = table.foodStacks[0].foodStack.Pop();
                        FoodManager.EatFood(f);
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

                                float x = UnityEngine.Random.Range(-1f, 1f);
                                float z = UnityEngine.Random.Range(-1f, 1f);
                                go.transforms.position = table.up.position + GameInstance.GetVector3(x, 0, z);
                            }
                        }
                        particle.gameObject.transform.position = mousePoint.position;

                        particle.GetComponent<ParticleSystem>().Play();

                    }
                    customerState = CustomerState.Table;
                    await UniTask.Delay(500, cancellationToken: cancellationToken);
                    ParticleManager.ClearParticle(particle);
                    animator.SetInteger("state", 0);
                    PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
                    table.seats[index].customer = null;
                    GameInstance.GameIns.animalManager.AttacCustomerTask(this);
                }
                else
                {
                    Debug.Log("Wait" + position);
                    customerAction = CustomerAction.ProcessWait;
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
                moveNode = await CalculateNodes_Async(position, cancellationToken);
                if (moveNode != null)
                {
                    if (moveNode.r == 100 && moveNode.c == 100) moveNode = null;
                    else
                    {
                        customerAction = CustomerAction.CustomerGoToHome;
                        await Customer_Move(moveNode, cancellationToken: cancellationToken);
                        if (reCalculate)
                        {
                            reCalculate = false;
                            continue;
                        }
                    }

                    bStart = false;
                    busy = false;
                    customerAction = CustomerAction.NONE;

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
            customerAction = CustomerAction.NONE;
            GameInstance.GameIns.animalManager.AttacCustomerTask(this);
        }
    }
    void CustomerCounter_Start(QueuePoint[] position, Counter counter)
    {
        if(currentWaypointIndex < 0)
        {
            if (GameInstance.GameIns.restaurantManager.customerDebug) Debug.Log("손님 카운터 주문");
            modelTrans.rotation = position[0].transforms.rotation;
            animatedAnimal.transforms.rotation = modelTrans.rotation;
            counter.customer = this;
            PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
            animator.SetInteger("state", 0);

            GameInstance.GameIns.uiManager.UpdateOrder(this, counter.counterType);
            currentWaypointIndex = 5;
            bStart = true;
        }
        else
        {
            if (counter.queuePoints[currentWaypointIndex].controller == this || counter.queuePoints[currentWaypointIndex].controller == null)
            {
                if(GameInstance.GameIns.restaurantManager.customerDebug) Debug.Log("손님 카운터로 이동");
                if (currentWaypointIndex + 1 < position.Length)
                {
                    counter.queuePoints[currentWaypointIndex + 1].controller = null;
                }
                //counter.queuePoints[currentWaypointIndex].controller = this;
                //moveCoord = CalculateCoords(position[currentWaypointIndex].transforms.position);
                //animalMove = true;
                //currentWaypointIndex--;
                int testIndex = currentWaypointIndex;
                while (currentWaypointIndex >= 0)
                {
                    if (currentWaypointIndex > 0 && (counter.queuePoints[currentWaypointIndex - 1].controller == this || counter.queuePoints[currentWaypointIndex - 1].controller == null))
                    {
                        currentWaypointIndex--;
                        //if (currentWaypointIndex == 0)
                        //{
                        //    counter.queuePoints[currentWaypointIndex].controller = this;
                        //    moveCoord = CalculateCoords(position[currentWaypointIndex].transforms.position);
                        //    animalMove = true;
                        //    currentWaypointIndex--;
                        //    break;
                        //}
                    }
                    else
                    {
                        counter.queuePoints[currentWaypointIndex].controller = this;
                        moveNode = CalculateNodes(position[currentWaypointIndex].transforms.position);
                        animalMove = true;
                        currentWaypointIndex--;
                        break;

                    }
                }
            }
            else
            {
                if (GameInstance.GameIns.restaurantManager.customerDebug) Debug.Log("손님 카운터 대기");
                PlayAnim(animal.animationDic[animalWaitActions], animalWaitActions);
                modelTrans.rotation = position[currentWaypointIndex].transforms.rotation;
                animatedAnimal.transforms.rotation = modelTrans.rotation;
                animator.SetInteger("state", 0);
            }
        }
    }
    void CustomerCounter_Rotate(QueuePoint[] position, Counter counter)
    {
        bool check = true;
        for (int j = 0; j < foodStacks.Count; j++)
        {
            if (foodStacks[j].needFoodNum != foodStacks[j].foodStack.Count)
            {
                check = false;
                break;
            }
        }
        if (check)
        {
            if (GameInstance.GameIns.restaurantManager.customerDebug) Debug.Log("손님 카운터 주문 완료");
            doOnce = true;
        }
    }
    void CustomerCounter_Update(QueuePoint[] position, Counter counter)
    {
       
        if (hasMoney)
        {
            float foodPrices = 0;
            int tipNum = 0;
           /* for (int j = 0; j < foodStacks.Count; j++)
            {
                for (int k = 0; k < foodStacks[j].foodStack.Count; k++)
                {
                    foodPrices += foodStacks[j].foodStack[k].foodPrice;
                    int tip = UnityEngine.Random.Range(1, 11);
                    if (tip == 1) tipNum++;
                }
            }*/

            GameInstance.GameIns.restaurantManager.playerData.money += foodPrices;
            GameInstance.GameIns.restaurantManager.playerData.fishesNum += tipNum;
            GameInstance.GameIns.uiManager.UpdateMoneyText(GameInstance.GameIns.restaurantManager.playerData.money);
            GameInstance.GameIns.uiManager.fishText.text = GameInstance.GameIns.restaurantManager.playerData.fishesNum.ToString();
            SaveLoadManager.Save(SaveLoadManager.SaveState.ONLY_SAVE_PLAYERDATA);//
            hasMoney = false;
            if (GameInstance.GameIns.restaurantManager.customerDebug) Debug.Log("손님 카운터 비용 지불");
        }
        else
        {
            actionTimer += Time.deltaTime;
            if(actionTimer > 0.5f)
            {
                actionTimer = 0;
                List<Table> tables = GameInstance.GameIns.workSpaceManager.tables;

                for (int i = 0; i < tables.Count - 1; i++)
                {
                    int min = i;
                    for (int j = i + 1; j < tables.Count; j++)
                    {
                        float m1 = (tables[j].transforms.position - trans.position).magnitude;
                        float m2 = (tables[min].transforms.position - trans.position).magnitude;
                        if (m1 < m2)
                        {
                            min = j;
                        }
                    }
                    if (min != i)
                    {
                        Table temp = tables[min];
                        tables[min] = tables[i];
                        tables[i] = temp;
                    }
                }

                foreach (Table t in tables)
                {
                    if (t.isDirty == false)
                    {
                        for (int j = 0; j < t.seats.Length; j++)
                        {
                            if ((t.seats[j].customer == null || t.seats[j].customer == this))
                            {
                                if (GameInstance.GameIns.restaurantManager.customerDebug) Debug.Log("손님 카운터 좌석 발견");
                                animalSpawner.RemoveWaitingCustomer(this);
                                counter.customer = null;
                                position[0].controller = null;
                                customerState = CustomerState.Counter;
                                customerAction = CustomerAction.NONE;
                                t.seats[j].customer = this;
                                doOnce = false;
                                bStart = false;
                                busy = false;
                                
                                GameInstance.GameIns.animalManager.AttacCustomerTask(this);
                                return;
                            }
                        }
                    }
                }
            }
            
        }
    }

    void CustomerTable_Start(Node node, Table table, int index)
    {
        bStart = true;
        animalMove = true;
        if (GameInstance.GameIns.restaurantManager.customerDebug) Debug.Log("손님 테이블 이동");
    }
    void CustomerTable_Rotate(Node node, Table table, int index)
    {
        doOnce = true;
        modelTrans.rotation = table.seats[index].transforms.rotation;
        animatedAnimal.transforms.rotation = modelTrans.rotation;
        if (GameInstance.GameIns.restaurantManager.customerDebug) Debug.Log("손님 테이블 회전");
    }
    void CustomerTable_Update(Node node, Table table, int index)
    {
        if(actionTimer < 0.1f)
        {
            actionTimer += Time.deltaTime;
        }
        else
        {
            actionTimer = 0;
            if (VisualizingFoodStack.Count > 0)
            {
                if (GameInstance.GameIns.restaurantManager.customerDebug) Debug.Log("손님 테이블 음식 쌓기");
                Food f = VisualizingFoodStack[VisualizingFoodStack.Count - 1];
                VisualizingFoodStack.Remove(f);
             /*   for (int i = 0; i < foodStacks.Count; i++)
                {
                    if (foodStacks[i].type == f.parentType)
                    {
                        foodStacks[i].foodStack.Remove(f);
                        break;
                    }
                }*/
                float r = UnityEngine.Random.Range(1, 2.5f);
#if HAS_DOTWEEN
                f.transforms.DOJump(table.transforms.position + GameInstance.GetVector3(0, 0.7f + table.foodStacks[0].foodStack.Count, 0), r, 1, 0.2f);
#endif
                table.foodStacks[0].foodStack.Push(f);
                audioSource.Play();
            }
            else
            {
                for (int i = foodStacks.Count - 1; i >= 0; i--)
                {
                    if (foodStacks[i].foodStack.Count == 0)
                    {
                        foodStacks.RemoveAt(i);
                    }
                }
                animalEat = 1;
                if (GameInstance.GameIns.restaurantManager.customerDebug) Debug.Log("손님 테이블 가진 음식 없음");
            }
        }
    }
    void CustomerTable_Eating(Node node, Table table, int index)
    {
        //  if (int i = 0; i < table.foodStacks.Count; i++)
        {
            if (table.foodStacks[0].foodStack.Count > 0)
            {
                eatingTimer += Time.deltaTime;
                animator.SetInteger("state", 2);

                PlayAnim(animal.animationDic["Eat"], "Eat");
                if ((eatingTimer >= animal.eatSpeed))//10 / eatSpeed))
                {
                    if (GameInstance.GameIns.restaurantManager.customerDebug) Debug.Log("손님 테이블 음식을 먹음");
                    //Food f = table.foodStacks[0].foodStack[table.foodStacks[0].foodStack.Count - 1];
                  /*  table.foodStacks[0].foodStack.Remove(f);
                    table.numberOfGarbage++;
                    eatingTimer = 0;
                    FoodManager.EatFood(f);*/

                    data.animal_eating_speed = 0.417f;

                    GameObject particle = ParticleManager.CreateParticle();
                    particle.gameObject.transform.position = mousePoint.position;
                    
                    //
                    particle.GetComponent<ParticleSystem>().Play();
                }

            }
            else
            {
                eatingTimer = 0;
                animator.SetInteger("state", 0);
                PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
                animalEat = 2;
                if (GameInstance.GameIns.restaurantManager.customerDebug) Debug.Log("손님 테이블 식사 완료");
                //CustomerTable_Garbage(coord, table, index);
            }
        }
    }

    void CustomerTable_Garbage(Node node, Table table, int index)
    {
        if(actionTimer > 0.5f)
        {
            table.seats[index].customer = null;

            bool bCustomerExist = true;
            for (int i = 0; i < table.seats.Length; i++)
            {
                if (table.seats[i].customer != null) bCustomerExist = false;
            }

            if (bCustomerExist)
            {
                table.numberOfGarbage = table.numberOfGarbage > 10 ? 10 : table.numberOfGarbage;
                table.isDirty = true;

                for (int i = 0; i < table.numberOfGarbage; i++)
                {
                    Garbage go = GarbageManager.CreateGarbage();
                    go.transforms.SetParent(table.trashPlate.transforms);
                    table.garbageList.Add(go);

                    float x = UnityEngine.Random.Range(-1f, 1f);
                    float z = UnityEngine.Random.Range(-1f, 1f);
                    go.transforms.position = table.up.position + GameInstance.GetVector3(x, 0, z);
                }
            }
            if (GameInstance.GameIns.restaurantManager.customerDebug) Debug.Log("손님 테이블 쓰레기 생성");
            customerState = CustomerState.Table;
            busy = false;
            bStart = false;
            doOnce = false;
            animalEat = 0;
            customerAction = CustomerAction.ProcessWait;
            waitTimer = 0.2f;
            actionTimer = 0;
          //  GameInstance.GameIns.animalManager.AttacCustomerTask(this);
        }
        else
        {
            actionTimer += Time.deltaTime;
        }
    }

    void CustomerGoHome_Start(Node node)
    {
        if (GameInstance.GameIns.restaurantManager.customerDebug) Debug.Log("손님 집 이동중");
        bStart = true;
        animalMove = true;
    }
    void CustomerGoHome_Update(Node node)
    {
        if (GameInstance.GameIns.restaurantManager.customerDebug) Debug.Log("손님 집 이동 완료");
        bStart = false;
        busy = false;
        customerAction = CustomerAction.NONE;
        GameInstance.GameIns.animalManager.DespawnCustomer(this);
    }

        IEnumerator CustomerWait()
    {
        while (coroutineTimer <= 1f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }
        coroutineTimer = 0f;
        busy = false;
        GameInstance.GameIns.animalManager.AttacCustomerTask(this);
    }

    IEnumerator CustomerWalkToCounter(QueuePoint[] position, Counter counter)
    {
        //animationInstancing.PlayAnimation(animal.animationDic["Idle_A"]);
        int i = currentWaypointIndex;

        while (i >= 0)
        {
            int j = 0;
            Node node;
            while (true)
            {
                if (position[i].controller == null || position[i].controller == this)
                {
                    if (i + 1 < position.Length) position[i + 1].controller = null;
                    position[i].controller = this;

                    node = CalculateNodes(position[i].transforms.position);

                    if (node.r != 100 && node.c != 100)
                    {
                        yield return StartCoroutine(AnimalMove(node, true));
                      
                      //  animal.animationDic.FirstOrDefault(x=>x.Value == )
                       // animationInstancing.PlayAnimation(animal.animationDic["Run"]);
                    }
                    else
                    {
                        yield return null;
                    }

                    if (i != 0 && position[i - 1].controller != null)
                    {
                        if (j != i)
                        {
                            j = i;
                            //animator.SetInteger("state", animalWaitActions);
                        }
                    }
                    yield return null;
                    break;
                }
                else
                {
                    PlayAnim(animal.animationDic[animalWaitActions], animalWaitActions);
                    animator.SetInteger("state", 0);
                    yield return null;
                }
            }

            if (i == 0)
            {
                counter.customer = this;
                PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
                animator.SetInteger("state", 0);
                
                GameInstance.GameIns.uiManager.UpdateOrder(this, counter.counterType);
            }
            i--;
            currentWaypointIndex = i;
            yield return null;
        }

        while (true)
        {
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
        for (int j = 0; j < foodStacks.Count; j++)
        {
            for (int k = 0; k < foodStacks[j].foodStack.Count; k++)
            {
       //         foodPrices += foodStacks[j].foodStack[k].foodPrice;
                int tip = UnityEngine.Random.Range(1, 11);
                if (tip == 1) tipNum++;
            }
        }

        if (hasMoney)
        {
            GameInstance.GameIns.restaurantManager.playerData.money += foodPrices;
            GameInstance.GameIns.restaurantManager.playerData.fishesNum += tipNum;
            GameInstance.GameIns.uiManager.UpdateMoneyText(GameInstance.GameIns.restaurantManager.playerData.money);

            SaveLoadManager.Save(SaveLoadManager.SaveState.ONLY_SAVE_PLAYERDATA);
            hasMoney = false;
        }

        List<Table> tables = GameInstance.GameIns.workSpaceManager.tables;

        while (true)
        {
            while (coroutineTimer <= 0.5f)
            {
                coroutineTimer += Time.deltaTime;
                yield return null;
            }
            coroutineTimer = 0f;

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
                            t.seats[j].customer = this;
                            customerState = CustomerState.Counter;
                            busy = false;
                            
                            yield break;
                        }
                    }
                }
            }
        }
    }

    IEnumerator CustomerWalkToTable(Node node, Table table, int index)
    {
        yield return StartCoroutine(AnimalMove(node));

        modelTrans.rotation = table.seats[index].transforms.rotation;
        animatedAnimal.transforms.rotation = modelTrans.rotation;

        while (VisualizingFoodStack.Count > 0)
        {
            Food f = VisualizingFoodStack[VisualizingFoodStack.Count - 1];
            VisualizingFoodStack.Remove(f);
            for (int i = 0; i < foodStacks.Count; i++)
            {
                if (foodStacks[i].type == f.parentType)
                {
                    //foodStacks[i].foodStack.Remove(f);
                    break;
                }
            }

            float r = UnityEngine.Random.Range(1, 2.5f);
#if HAS_DOTWEEN
            f.transforms.DOJump(table.transforms.position + GameInstance.GetVector3(0, 0.7f + table.foodStacks[0].foodStack.Count, 0), r, 1, 0.2f);
#endif
            //   table.foodStacks[0].foodStack.Add(f);
            audioSource.Play();

            yield return GetWaitTimer.WaitTimer.GetTimer(100); //new WaitForSeconds(0.1f);
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

                while (true)
                {
                    eatingTimer += Time.deltaTime;
                    eatSpeed = gameObject.GetComponentInParent<Animal>().eatSpeed;

                    if (eatingTimer >= 10 / eatSpeed)
                    {
                        break;
                    }
                    yield return null;
                }

                data.animal_eating_speed = 0.417f;
                animator.SetInteger("state", 2);
                PlayAnim(animal.animationDic["Eat"], "Eat");
                GameObject particle = ParticleManager.CreateParticle();
                particle.gameObject.transform.position = mousePoint.position;
                particle.GetComponent<ParticleSystem>().Play();
            
                yield return new WaitForSeconds(data.animal_eating_speed);

                if (table.foodStacks.Count <= i) break;
                int foodIndex = table.foodStacks[i].foodStack.Count - 1;
                if (foodIndex < 0) break;

           //     Food f = table.foodStacks[i].foodStack[foodIndex];
            //    table.foodStacks[i].foodStack.Remove(f);
                table.numberOfGarbage++;
               // FoodManager.EatFood(f);

                ParticleManager.ClearParticle(particle);
                animator.SetInteger("state", 0);
                PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
                eatingTimer = 0;
            }
        }

        while (coroutineTimer <= 0.5f)
        {
            coroutineTimer += Time.deltaTime;
            yield return null;
        }

        coroutineTimer = 0;

        table.seats[index].customer = null;

        bool bCustomerExist = true;
        for (int i = 0; i < table.seats.Length; i++)
        {
            if (table.seats[i].customer != null) bCustomerExist = false;
        }

        if (bCustomerExist)
        {
            table.numberOfGarbage = table.numberOfGarbage > 10 ? 10 : table.numberOfGarbage;
            table.isDirty = true;

            for (int i = 0; i < table.numberOfGarbage; i++)
            {
                Garbage go = GarbageManager.CreateGarbage();
                go.transforms.SetParent(table.trashPlate.transforms);
                table.garbageList.Add(go);

                float x = UnityEngine.Random.Range(-1f, 1f);
                float z = UnityEngine.Random.Range(-1f, 1f);
                go.transforms.position = table.up.position + GameInstance.GetVector3(x, 0, z);
            }
        }

        customerState = CustomerState.Table;
        busy = false;
        
    }

    IEnumerator CustomerGoHome(Node node)
    {
        if (node != null)
        {
            yield return StartCoroutine(AnimalMove(node));
        }

        
        GameInstance.GameIns.animalManager.DespawnCustomer(this);
    }
    private static readonly WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

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
            targetUpdate = AnimalMovement(Vector3.zero, ref animalMoveIndex, moveLocations);
            dirUpdate = (targetUpdate - trans.position);
            dirUpdate.Normalize();
            newMagnitudeUpdate = (targetUpdate - trans.position).magnitude;
            moveStart = 2;
        }
        else
        {
            if (!bContinue)
            {
                PlayAnim(animal.animationDic[animation_Idle_A], animation_Idle_A);
                animator.SetInteger("state", 0);
            }

            animalMoveIndex = 0;
            animalMove = false;
            moveStart = 0;
        }
        // Debug.Log(moveStart);
    }
    private void AnimalMove_Update2(Node moveNode)
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

          /*  targetUpdate = AnimalMovement(Vector3.zero, ref animalMoveIndex, moveLocations);
            dirUpdate = (targetUpdate - trans.position);
            dirUpdate.Normalize();
            newMagnitudeUpdate = (targetUpdate - trans.position).magnitude;
*/
            trans.Translate(dirUpdate * Time.fixedDeltaTime * 5, Space.World);
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
         //       Debug.Log(newLoc);
                //   Debug.Log(loc);
                //   Debug.Log(di);
            }


            loc = newLoc;
            //   return;
        }
    }
    IEnumerator AnimalMove(Node node, bool continuous = false)
    {
        if (node != null)
        {
            PlayAnim(animal.animationDic["Run"], "Run");
            animator.SetInteger("state", 1);
            // List<Coord> coords = GetCalculatedList(coord);
            nodes.Clear();
            GetCalculatedList(ref nodes, node);
            int i = 0;
            Vector3 currentNode = trans.position;

            while (i < nodes.Count)
            {
                Vector3 target = AnimalMovement(node, ref i, nodes);
                Vector3 dirs = (target - trans.position).normalized;
                while (true)
                {
                    float magnitude = (target - trans.position).magnitude;
                    if (magnitude < 0.1f)
                    {
                        trans.position = target;
                        break;
                    }
                  
                    //speed = gameObject.GetComponentInParent<Animal>().speed;
                    trans.Translate(dirs * Time.fixedDeltaTime * 5, Space.World);

                    float angle = Mathf.Atan2(dirs.x, dirs.z) * Mathf.Rad2Deg;
                    modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                    animatedAnimal.transforms.rotation = modelTrans.rotation;
                    yield return waitForFixedUpdate;
                }
                i++;
            }

            if (!continuous)
            {
                animator.SetInteger("state", 0);
                PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
            }
        }
    }
}
