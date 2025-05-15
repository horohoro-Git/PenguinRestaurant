//using SRF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using static AssetLoader;

public class AllAnimals
{
    public List<Animal> activateAnimals = new List<Animal>();
    public Queue<Animal> deactivateAnimals = new Queue<Animal>();
    
}

public class AnimalManager : MonoBehaviour
{
    public enum Mode
    {
        GameMode,
        DebugMode
    }

    public Mode mode;

    public List<Employee> employeeControllers = new List<Employee>();
    public List<Customer> customerControllers = new List<Customer>();
    Queue<Employee> deactivateEmployeeControllers = new Queue<Employee>();
    Queue<Customer> deactivateCustomerControllers = new Queue<Customer>();


    public List<Shadow> activateShadows = new List<Shadow>();
    public Queue<Shadow> deactivateShadows = new Queue<Shadow>();

    public List<SliderController> activateSliders = new List<SliderController>();
    public Queue<SliderController> deactivateSliders = new Queue<SliderController>();

    public static Dictionary<int, AnimalStruct> animalStructs = new Dictionary<int, AnimalStruct>();
    public static Dictionary<int, int> gatchaTiers = new Dictionary<int, int>();

    [SerializeField]
    Transform employeeStart;

    [SerializeField]
    Transform customerStart;

    public Employee employeeController;
    public Customer customerController;

    public GameObject addanimalController;
    public List<Animal> animals = new List<Animal>();
    //public List<Animal> newAnimals = new List<Animal>();
    Dictionary<int, AllAnimals> allAnimals = new Dictionary<int, AllAnimals>();
    //  List<AllAnimals> newAllAnimals = new List<AllAnimals>();
    // [SerializeField] List<Shadow> shadows = new List<Shadow>();
    public Shadow shadow;
    public SliderController levelSlider;// = SliderController();

    public Dictionary<int, Queue<Rolling>> gatchStack = new Dictionary<int, Queue<Rolling>>();
    public List<Rolling> gatchaList = new List<Rolling>();

    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
    public float height;
    public float distanceSize;

    GameObject animalParent;

    [SerializeField]
    GameObject eatParticle;

   // GameObject shadowParent;
    public static GameObject SubUIParent;

    [NonSerialized]
    public Queue<Employee> employeeTasks = new Queue<Employee>();
    [NonSerialized]
    public Queue<Customer> customerTasks = new Queue<Customer>();

    public Coroutine animalActionCoroutine;

    public Action<Customer> customerCallback = (customer) => { customer.FindCustomerActions(); };
    public Action<Employee> employeeCallback = (employee) => {
        if (!employee.pause && !employee.busy)
        {
            employee.busy = true;
            employee.FindEmployeeWorks();
        }
    };
    private void Awake()
    {
        WorkSpaceManager workSpaceManager = GetComponent<WorkSpaceManager>();
        GameInstance.GameIns.animalManager = this;
        GameInstance.GameIns.workSpaceManager = workSpaceManager;
        GameInstance.GameIns.calculatorScale.minX = minX;
        GameInstance.GameIns.calculatorScale.minY = minY;
        GameInstance.GameIns.calculatorScale.maxX = maxX;
        GameInstance.GameIns.calculatorScale.maxY = maxY;
        GameInstance.GameIns.calculatorScale.height = height;
        GameInstance.GameIns.calculatorScale.distanceSize = distanceSize;

        animalParent = new GameObject();
        animalParent.transform.position = new Vector3(100, 100, 100);
        animalParent.name = "animalRoot";

        SubUIParent = new GameObject();
        SubUIParent.AddComponent<Canvas>();
        SubUIParent.name = "subui";


        /* for (int i = 0; i < 8; i++)
         {
             AllAnimals aa = new AllAnimals();
             aa.activateAnimals = new List<Animal>();
             aa.deactivateAnimals = new Queue<Animal>();
             allAnimals.Add(aa);
         }*/
        /*  Vector3 localScale = new Vector3(1.5f, 1.5f, 1.5f);
          for (int j = 0; j < 10; j++)
          {
              Animal newAnimal = Instantiate(AssetLoader.loadedAssets["NewPenguin"], animalParent.transform).GetComponent<Animal>();
              newAnimal.transform.localScale = localScale;//new Vector3(1.5f, 1.5f, 1.5f);
              newAnimal.GetAnimationInstancing();
              if(allAnimals.ContainsKey(10))
              {
                  allAnimals[10].deactivateAnimals.Enqueue(newAnimal);
              }
              else
              {
                  allAnimals[10] = new AllAnimals();
                  allAnimals[10].activateAnimals = new List<Animal>();
                  allAnimals[10].deactivateAnimals = new Queue<Animal>();
                  allAnimals[10].deactivateAnimals.Enqueue(newAnimal);
              }

          }
          for (int j = 100; j < 106; j++)
          {
              for(int i=0; i<10; i++)
              {
                  Animal newAnimal = Instantiate(AssetLoader.loadedAssets[AssetLoader.itemAssetKeys[j].Name], animalParent.transform).GetComponent<Animal>();
                  newAnimal.transform.localScale = localScale;//new Vector3(1.5f, 1.5f, 1.5f);
                  newAnimal.GetAnimationInstancing();
                  if (allAnimals.ContainsKey(j))
                  {
                      allAnimals[j].deactivateAnimals.Enqueue(newAnimal);
                  }
                  else
                  {
                      allAnimals[j] = new AllAnimals();
                      allAnimals[j].activateAnimals = new List<Animal>();
                      allAnimals[j].deactivateAnimals = new Queue<Animal>();
                      allAnimals[j].deactivateAnimals.Enqueue(newAnimal);
                  }
              }
          }*/

        //foreach (var animal in animals) // *** List 기반으로 초기화
        //{
        //    AllAnimals aa = new AllAnimals();
        //    for (int j = 0; j < 100; j++)
        //    {
        //        Animal newAnimal = Instantiate(animal, animalParent.transform);
        //        newAnimal.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        //        newAnimal.GetAnimationInstancing();
        //        // AnimationInstancing()
        //        // newAnimal.gameObject.SetActive(false);                
        //        aa.deactivateAnimals.Enqueue(newAnimal);

        //    }
        //    allAnimals.Add(aa); // *** 새로 생성된 AllAnimals를 추가
        //}


        //동물 그림자 생성
     /*   foreach (var shadow in shadows)
        {
            for (int i = 0; i < 50; i++)
            {
                Shadow instancedShadow = Instantiate(shadow, shadowParent.transform);

                instancedShadow.gameObject.SetActive(false);
                deactivateShadows.Enqueue(instancedShadow);
            }
        }

        //레벨 슬라이더 생성
        for (int i = 0; i < 10; i++)
        {
            SliderController instancedSlider = Instantiate(slider, sliderParent.transform);

            instancedSlider.gameObject.SetActive(false);
            deactivateSliders.Enqueue(instancedSlider);
        }*/

    }



    private void Start()
    {
        GameInstance.GameIns.calculatorScale.distanceSize = 0.4f;
        MoveCalculator.CheckArea(GameInstance.GameIns.calculatorScale);

        animalStructs = SaveLoadSystem.LoadAnimalsData();
        gatchaTiers = SaveLoadSystem.LoadTier();
        for (int i = 0; i < 10; i++)
        {
            Employee controller = Instantiate(employeeController, animalParent.transform).GetComponentInChildren<Employee>();
            controller.gameObject.SetActive(false);
            deactivateEmployeeControllers.Enqueue(controller);
        }

        // 손님 초기화
        for (int i = 0; i < 20; i++)
        {
            Customer controller = Instantiate(customerController, animalParent.transform).GetComponentInChildren<Customer>();
            controller.customerIndex = i;
            controller.gameObject.SetActive(false);
            deactivateCustomerControllers.Enqueue(controller);
        }

        //직원 펭귄
        allAnimals[10] = new AllAnimals();
        allAnimals[10].activateAnimals = new List<Animal>();
        allAnimals[10].deactivateAnimals = new Queue<Animal>();
        for (int i = 0; i < 8; i++)
        {
            Animal newAnimal = Instantiate(AssetLoader.loadedAssets["PenguinEmployee"], animalParent.transform).GetComponent<Animal>();
           // newAnimal.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
           // newAnimal.GetComponentInChildren<LODController>().animator.enabled = false;
            allAnimals[10].deactivateAnimals.Enqueue(newAnimal);
        }

        //동물들
        foreach (var data in AssetLoader.animals)
        {
            if (data.Value.is_customer)
            {
                allAnimals[data.Key] = new AllAnimals();
                allAnimals[data.Key].activateAnimals = new List<Animal>();
                allAnimals[data.Key].deactivateAnimals = new Queue<Animal>();

                for (int j = 0; j < 30; j++)
                {
                    Animal newAnimal = Instantiate(AssetLoader.loadedAssets[AssetLoader.itemAssetKeys[data.Key].Name], animalParent.transform).GetComponent<Animal>();
                    allAnimals[data.Key].deactivateAnimals.Enqueue(newAnimal);
                }
            }
        }


        //그림자 생성
        for (int i = 0; i < 50; i++)
        {
            Shadow instancedShadow = Instantiate(shadow, SubUIParent.transform);
            instancedShadow.Setup();
            //    instancedShadow.Set
            instancedShadow.transform.position = new Vector3(100, 100, 100);
            instancedShadow.gameObject.SetActive(false);
            deactivateShadows.Enqueue(instancedShadow);
        }

        //레벨 슬라이더 생성
        for (int i = 0; i < 10; i++)
        {
            SliderController instancedSlider = Instantiate(levelSlider, SubUIParent.transform);
            instancedSlider.Setup();
            instancedSlider.transform.position = new Vector3(100, 100, 100);
            instancedSlider.Activate(false);
            instancedSlider.gameObject.SetActive(false);
            deactivateSliders.Enqueue(instancedSlider);
        }

    }

    //float timer = 0f;
   /* private void Update()
    {
        timer += Time.deltaTime;

        if(timer > 0.5f)
        {
            if (employeeTasks.TryDequeue(out Employee employeeTask)) employeeTask.FindEmployeeWorks();
            if (customerTasks.TryDequeue(out Customer customerTask)) customerTask.FindCustomerActions();
            timer = 0f;
           // AnimalRoutine_Update();
        }
    }*/
    void AnimalRoutine_Update()
    {
        //  Debug.Log("AnimalRoutineStart");
     //   while (true)
        {
            //   Debug.Log("KK");
           // yield return GetWaitTimer.WaitTimer.GetTimer(500);
            //yield return new WaitForSecondsRealtime(0.5f);
            for (int i = 0; i < employeeControllers.Count; i++)
            {

                if (!employeeControllers[i].busy)
                {
                    employeeControllers[i].busy = true;
                    //employeeControllers[i].Work();
                    employeeControllers[i].FindEmployeeWorks();
                }
                //else if (employeeControllers[i].success == 1)
                //{
                //    employeeControllers[i].StopC();
                //}
                //else if (employeeControllers[i].success == 2)
                //{
                //    employeeControllers[i].Release();
                //}
            }
            // new WaitForSeconds(0.1f);

            for (int i = 0; i < customerControllers.Count; i++)
            {
                if (!customerControllers[i].busy && customerControllers[i].foodsAnimalsWant.spawnerType != AnimalSpawner.SpawnerType.Delivery)
                {
                    customerControllers[i].busy = true;
                    customerControllers[i].FindCustomerActions();
                }
            }
        }
    }

    IEnumerator AnimalRoutine()
    {
      //  Debug.Log("AnimalRoutineStart");
        while (true)
        {
            //   Debug.Log("KK");
            yield return GetWaitTimer.WaitTimer.GetTimer(500);
            //yield return new WaitForSecondsRealtime(0.5f);
            for (int i=0; i<employeeControllers.Count; i++)
            {
                
                if (!employeeControllers[i].busy)
                {
                    employeeControllers[i].busy = true;
                    employeeControllers[i].Work();
                  //  employeeControllers[i].FindEmployeeWorks();
                }
                //else if (employeeControllers[i].success == 1)
                //{
                //    employeeControllers[i].StopC();
                //}
                //else if (employeeControllers[i].success == 2)
                //{
                //    employeeControllers[i].Release();
                //}
            }
           // new WaitForSeconds(0.1f);

            //for (int i = 0; i < customerControllers.Count; i++)
            //{
            //    if (!customerControllers[i].busy && customerControllers[i].foodsAnimalsWant.spawnerType != AnimalSpawner.SpawnerType.Delivery)
            //    {
            //        customerControllers[i].busy = true;
            //        customerControllers[i].FindCustomerActions();
            //    }
            //}
        }
    }

    public Employee SpawnEmployee()
    {
        int type = 10;

        if (deactivateCustomerControllers.Count > 0 && allAnimals[type].deactivateAnimals.Count > 0)
        {
            Employee employee = deactivateEmployeeControllers.Dequeue();
            employee.gameObject.SetActive(true);
            employeeControllers.Add(employee);
            employee.animalStruct = animalStructs[10];
            Animal animal = SpawnAnimal(employee, type);
            employee.id = employeeControllers.Count;
          //  employee.ui =  //animal.GetComponentInChildren<SliderController>();
            //employee.headPoint = animal.GetComponentInChildren<Head>().gameObject.transform;
            employee.mousePoint = animal.GetComponentInChildren<Mouse>().gameObject.transform;

            //employee.animator = animal.GetComponentInChildren<Animator>();

            SliderController slider = deactivateSliders.Dequeue();
            activateSliders.Add(slider);
            slider.gameObject.SetActive(true);
            slider.Activate(true);
            slider.transform.SetParent(GameInstance.GameIns.applianceUIManager.EmployeeStatusUI.transform);
            slider.model = animal.trans;
            employee.ui = slider;


            employee.eatParticle = eatParticle;
            employee.transform.localPosition = Vector3.zero;
            employee.transform.localRotation = Quaternion.identity;
            employee.employeeCallback -= EmployeeAction;
            employee.employeeCallback += EmployeeAction;
            return employee;
        }
        return null;
    }

    public Customer SpawnCustomer(FoodsAnimalsWant foodsAnimalsWant, bool onlyOrder = false)
    { 
       
        Dictionary<int, AnimalStruct> valuePairs = animalStructs;

        List<int> ints = new List<int>();
        foreach(var i in gatchaTiers)
        {
            ints.Add(i.Key);
        }
        int randomNum = UnityEngine.Random.Range(0, ints.Count);
        int res = ints[randomNum];

        if (deactivateCustomerControllers.Count > 0 && allAnimals[res].deactivateAnimals.Count > 0)
        {
            Customer customer = deactivateCustomerControllers.Dequeue();
            customer.gameObject.SetActive(true);
            customer.SetDefault();
            customer.animalStruct = animalStructs[res];
            //  Customer customer = new GameObject().AddComponent<Customer>();
            if (!onlyOrder)
            {
                Animal animal = SpawnAnimal(customer, res); //SpawnAnimal(customer, res);
                
                customer.mousePoint = animal.GetComponentInChildren<Mouse>().gameObject.transform;

                customer.eatParticle = eatParticle;
                customer.animal = animal;
                customer.customerCallback -= CustomerAction;
                customer.customerCallback += CustomerAction;
            }
        
            customer.Setup(foodsAnimalsWant);
            customerControllers.Add(customer);
            customer.transform.localPosition = Vector3.zero;
            customer.transform.localRotation = Quaternion.identity;
            if (onlyOrder)
            {
                customer.busy = true;
                
            }
           // else AttacCustomerTask(customer);
            return customer;
        }
        return null;
    }

    Animal SpawnAnimal(AnimalController controller, int type)
    {
        Animal animal = null;
        if (allAnimals.ContainsKey(type))
        {
            animal = allAnimals[type].deactivateAnimals.Dequeue();
            if (allAnimals[type].deactivateAnimals.Count == 0) Debug.Log("Fail SpawnAnimal (Reason : No DeActivatedAnimals)");
        }
        else
        {
            allAnimals[type] = new AllAnimals();
            allAnimals[type].activateAnimals = new List<Animal>();
            allAnimals[type].deactivateAnimals = new Queue<Animal>();
            allAnimals[type].deactivateAnimals.Enqueue(Instantiate(AssetLoader.loadedAssets[AssetLoader.itemAssetKeys[type].Name]).GetComponent<Animal>());
            animal = Instantiate(AssetLoader.loadedAssets[AssetLoader.itemAssetKeys[type].Name]).GetComponent<Animal>();
        }

        animal.gameObject.SetActive(true);
        if (controller.trans == null) controller.trans = animal.trans;
        else
        {
            Debug.Log("컨트롤러가 이미 값을 갖고 있음" + controller.trans);
            controller.animal = null;
            controller.trans = null;
            controller.trans = animal.trans;
        }
        controller.modelTrans = animal.modelTrans;
        controller.transform.SetParent(animal.transform);
        controller.animator = animal.GetComponentInChildren<LODController>().animator;
        controller.animator.enabled = true;
        controller.animal = animal;
#if HAS_ANIMATION_INSTANCING
        controller.animationInstancing = animal.GetComponentInChildren<AnimationInstancing.AnimationInstancing>();
       // animal.GetComponentInChildren<AnimationInstancing.AnimationInstancing>().activePlease = true;
#endif
        // animal.GetComponentInChildren<AnimationInstancing.AnimationInstancing>().PlayAnimation(animal.animationDic["Idle_A"]);
       // controller.animatedAnimal = animal.GetComponentInChildren<AnimatedAnimal>();
        controller.headPoint = animal.GetComponentInChildren<Head>().transform;
        allAnimals[type].activateAnimals.Add(animal);
        
        Shadow shadow = deactivateShadows.Dequeue();
        activateShadows.Add(shadow);
        shadow.gameObject.SetActive(true);
        //shadow.transform.SetParent(GameInstance.GameIns.applianceUIManager.AnimalShadowUI.transform);
        shadow.SetSize(animal.transform.localScale,0,0);
        shadow.model = animal.modelTrans;
        controller.shadow = shadow;
        return animal;
    }

    Vector3 localScale = new Vector3(1.5f, 1.5f, 1.5f);
    public void AddNewAnimal(Animal newAnimal,int id, AnimalStruct animalStruct) // *** 동적 동물 추가 메서드
    {
        if (animals.Contains(newAnimal)) // *** 이미 리스트에 포함된 경우
        {
            return; // *** 메서드 종료
        }
        animals.Add(newAnimal); // *** 새로운 동물을 List에 추가        

       // AllAnimals newAllAnimals = new AllAnimals(); // *** 새로운 AllAnimals 생성
       Queue<Animal> animalQueue = new Queue<Animal>();
        for (int i = 0; i < 30; i++)
        {
            Animal animal = Instantiate(newAnimal, animalParent.transform);
            animal.transform.localScale = localScale; //new Vector3(1.5f, 1.5f, 1.5f);
            animal.GetAnimationInstancing();
            //  animal.gameObject.SetActive(false);
            animalQueue.Enqueue(animal);
          //  allAnimals[id].deactivateAnimals.Enqueue(animal);            
        }
        allAnimals[id].deactivateAnimals = animalQueue;
       // allAnimals.Add(newAllAnimals); // *** 새로운 AllAnimals를 리스트에 추가
    }


    public void DespawnEmployee(Employee employee)
    {
        foreach (var stack in employee.foodStacks)
        {
            FoodStackManager.FM.RemoveFoodStack(stack.Value);
        }
        employee.foodStacks.Clear();
        employee.gameObject.SetActive(false);
        employee.employeeState = EmployeeState.Wait;
        employee.busy = false;
        employee.employeeCallback -= EmployeeAction;
        employeeControllers.Remove(employee);
        int type = 0;
        DespawnAnimal(employee, type);

      
   
        deactivateEmployeeControllers.Enqueue(employee);
    }

    public void DespawnCustomer(Customer customer, bool onlyOrder = false)
    {



        customer.customerState = CustomerState.Walk;
        customer.busy = false;

        customerControllers.Remove(customer);
        deactivateCustomerControllers.Enqueue(customer);

        if (!onlyOrder)
        {
            customer.animalSpawner.RemoveWaitingCustomer(customer);
            customer.foodStacks.Clear();
            customer.trans.localPosition = GameInstance.GetVector3(0, 0, 0);
            customer.gameObject.SetActive(false);
            customer.trans.position = GameInstance.GetVector3(100, 100, 100);
            customer.animator.enabled = false;
            customer.customerCallback -= CustomerAction;
            //  int t = (int)customer.animalType;
            int t = customer.animalStruct.id;
            DespawnAnimal(customer, t);

        }
        else
        {
         /*   foreach (var stack in customer.foodStacks)
            {
                foreach (var food in stack.foodStack)
                {
                    FoodManager.RemovePackageBox((PackageFood)food);
                }
            }*/

        }

        foreach (var stack in customer.foodStacks)
        {
            FoodStackManager.FM.RemoveFoodStack(stack);
        }
        customer.SetDefault();
        // deactivateCustomerControllers.Enqueue(customer);
    }

    void DespawnAnimal(AnimalController controller, int type)
    {
        Animal al = controller.GetComponentInParent<Animal>();
        allAnimals[type].activateAnimals.Remove(al);
        al.gameObject.SetActive(false);
        controller.transform.SetParent(animalParent.transform);
        allAnimals[type].deactivateAnimals.Enqueue(al);

        Shadow s = controller.shadow;
        s.model = null;
        s.transform.position = SubUIParent.transform.position;
        s.gameObject.SetActive(false);
        activateShadows.Remove(s);
        deactivateShadows.Enqueue(s);
      
        //Destroy(this);
    }

    public void UpdateEmployeeUpgrade(Employee animal)
    {
       // animal.employeeLevel = GameInstance.GameIns.restaurantManager.employeeDatas[animal.id - 1];
    }

    public void AttachEmployeeTask(Employee employee)
    {
        employeeCallback.Invoke(employee);
        //employeeTasks.Enqueue(employee);
    }
    public void AttacCustomerTask(Customer customer)
    {
        customerCallback.Invoke(customer);
       // customerTasks.Enqueue(employee);
    }

    public void NewGatchaAnimals()
    {
        //도박용 동물들
        GatcharManager gatcharManager = GameInstance.GameIns.gatcharManager;
        for (int i = 0; i < gatcharManager.gameObjects.Length; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Rolling rolling = Instantiate(gatcharManager.gameObjects[i], animalParent.transform).GetComponent<Rolling>();
                rolling.gameObject.SetActive(false);
                if (gatchStack.ContainsKey(rolling.type))
                {
                    gatchStack[rolling.type].Enqueue(rolling);
                }
                else
                {
                    gatchStack[rolling.type] = new Queue<Rolling>();
                }
           // Debug.Log(rolling.type);
            }
        }
    }
    public Rolling GetGatchaAnimal(MapType type)
    {
        int rands = 0;
        switch (type)
        {
            case MapType.town:
                {
                    rands = UnityEngine.Random.Range(100, 106);
                    break;
                }
        
        }

        Rolling rolling = gatchStack[rands].Dequeue();
        rolling.gameObject.SetActive(true);
        return rolling;
    }

    public void RemoveGatchaAnimal(Rolling r)
    {
        r.gameObject.SetActive(false);
        gatchStack[r.type].Enqueue(r);
        
    }

    public void CustomerAction(Customer customer)
    {
        customer.FindCustomerActions();
    }
    public void EmployeeAction(Employee employee)
    {
        if (!employee.pause && !employee.busy)
        {
            employee.busy = true;
            employee.FindEmployeeWorks();
        }
    }
}
