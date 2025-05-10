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
                    animal.PlayAnimation(AnimationKeys.Idle);
                    //PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
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
                    animal.PlayAnimation(AnimationKeys.Walk);
                   // PlayAnim(animal.animationDic["Run"], "Run");
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
                    animal.PlayAnimation(AnimationKeys.Walk);
                   // PlayAnim(animal.animationDic["Run"], "Run");
                    cu2 = (loc - trans.position).magnitude;
                    Vector3 dir = (loc - trans.position).normalized;
                    trans.position = Vector3.MoveTowards(trans.position, loc, animalStruct.speed * Time.deltaTime);

                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                }
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
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
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
                    animal.PlayAnimation(AnimationKeys.Eat);
                   // PlayAnim(animal.animationDic["Eat"], "Eat");
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
                    animal.PlayAnimation(AnimationKeys.Idle);
                    //PlayAnim(animal.animationDic["Idle_A"], "Idle_A");
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
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
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
}
