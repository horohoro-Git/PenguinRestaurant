using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using static SaveLoadManager;
using static GameInstance;
using System;
using CryingSnow.FastFoodRush;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;
using System.Threading;
using static UnityEngine.UI.Image;
using UnityEngine.InputSystem;
using System.Text;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class RestaurantManager : MonoBehaviour
{
    [NonSerialized] public int[] employeeHire = { 0, 700, 1500, 2500, 4000, 6000, 9000, 15000 };
    
    public GameObject[] levels;
    public NextTarget[] levelGuides;
    NextTargetData[] nextTargetDatas = new NextTargetData[100];
    public AudioSource purchase;
    [NonSerialized] public int level = 0;
    public bool allLevelUp;
    [NonSerialized] public int fishNum = 0;
    public bool employable;
    public int restaurantValue = 0;

  //  public PlayerData playerData { get; set; }
    public PlayerStruct playerStruct = new PlayerStruct(0, 0f, 0, 0, null, 0);
    public List<EmployeeLevelStruct> employeeData = new List<EmployeeLevelStruct>();
    public List<RestaurantParam> restaurantparams = new List<RestaurantParam>();

    public Dictionary<int, LevelData> levelData = new Dictionary<int, LevelData>();
    public CombineDatas combineDatas;
  //  public Dictionary<MachineType, Dictionary<int, MachineLevelStruct>> machineLevelData = new Dictionary<MachineType, Dictionary<int, MachineLevelStruct>>();

    public List<EmployeeData> employeeDatas = new List<EmployeeData>();
    public EmployeeData currentEmployeeData;
    public Dictionary<MachineType, List<MachineData>> upgradeMachineDic = new Dictionary<MachineType, List<MachineData>>();

    [NonSerialized] public List<Vector3> flyingEndPoints = new List<Vector3>();

    public List<FoodStack> foodStacks = new List<FoodStack>();

    public bool employeeDebug;
    public bool customerDebug;

    public Dictionary<int, GoodsStruct> goodsStruct = new Dictionary<int, GoodsStruct>();

    public static GameObject trayObjects;
    public Queue<GameObject> trays = new Queue<GameObject>();
    public Door door;
    public RestaurantCurrency restaurantCurrency;
    public RestaurantData restaurantData;
    public Employees employees;

    CancellationTokenSource cancellationTokenSource;

    [NonSerialized] public bool machineLevelDataChanged;
    public Dictionary<MachineType, MachineLevelData> machineLevelData = new Dictionary<MachineType, MachineLevelData>();
    public List<GameObject> expandables = new List<GameObject>();
    public List<GameObject> removables = new List<GameObject>();
    public List<ParticleSystem> extensionParticles = new List<ParticleSystem>();
    StringBuilder moneyString = new StringBuilder();

    public FuelGage fuelGage;
    public Queue<FuelGage> fuelGages = new Queue<FuelGage>();

    public VendingMachineData vendingData;

    public static float restaurantTimer;

  //  public Dictionary<WorkSpaceType, int> workSpaces = new Dictionary<WorkSpaceType, int>(); 
 //   public Dictionary<MachineType, MachineLevelStruct>
    // Start is called before the first frame update
    private void Awake()
    {
      
        door = Instantiate(door);
        door.gameObject.SetActive(false);
        trayObjects = new GameObject();
        trayObjects.name = "trayObjects";
        trayObjects.transform.position = Vector3.zero;
        GameInstance.GameIns.restaurantManager = this;
        /*   var burgerupgrade_resources = Resources.Load<TextAsset>("burger_upgrade_table");

           // List<MachineData> burgerdata = JsonConvert.DeserializeObject<List<MachineData>>(burgerupgrade_resources.text);
           List<MachineData> burgerdata = JsonConvert.DeserializeObject<List<MachineData>>(burgerupgrade_resources.text);
           upgradeMachineDic.Add(FoodMachine.MachineType.BurgerMachine, burgerdata);

           var cokeupgrade_resources = Resources.Load<TextAsset>("coke_upgrade_table");
           List<MachineData> cokedata = JsonConvert.DeserializeObject<List<MachineData>>(cokeupgrade_resources.text);
           upgradeMachineDic.Add(FoodMachine.MachineType.CokeMachine, cokedata);

           var coffeeupgrade_resources = Resources.Load<TextAsset>("coffee_upgrade_table");
           List<MachineData> coffeedata = JsonConvert.DeserializeObject<List<MachineData>>(coffeeupgrade_resources.text);
           upgradeMachineDic.Add(FoodMachine.MachineType.CoffeeMachine, coffeedata);

           var donutupgrade_resources = Resources.Load<TextAsset>("donut_upgrade_table");
           List<MachineData> donutdata = JsonConvert.DeserializeObject<List<MachineData>>(donutupgrade_resources.text);
           upgradeMachineDic.Add(FoodMachine.MachineType.DonutMachine, donutdata);

           var employeeupgrade_resources = Resources.Load<TextAsset>("employee_upgrade_table");
           employeeDatas = JsonConvert.DeserializeObject<List<EmployeeData>>(employeeupgrade_resources.text);

           GameInstance.GameIns.restaurantManager = this;
           combineDatas = SaveLoadManager.LoadGame();
   */

        //combineDatas.playerData.money = 100000; //테스트 용

      //  playerData = 
      //  playerData.money = 10000;

        goodsStruct = AssetLoader.goods;
        // playerData = combineDatas.playerData;
        //NewTrays();
      
        //   UpgradePenguin(combineDatas.playerData.level, true, null);
        
        NewTrays();


        machineLevelData = SaveLoadSystem.LoadFoodMachineStats();
    }
    void Start()
    {
        for (int i = 0; i < 20; i++)
        {

            FuelGage fg = Instantiate(fuelGage, AnimalManager.SubUIParent.transform);
            fg.transform.position = new Vector3(100, 100, 100);
            fg.gameObject.SetActive(false);
            fuelGages.Enqueue(fg);
        }
        restaurantData = SaveLoadSystem.LoadRestaurantData();
        restaurantCurrency = SaveLoadSystem.LoadRestaurantCurrency();
        restaurantparams = SaveLoadSystem.LoadRestaurantBuildingData();
        vendingData = SaveLoadSystem.LoadVendingMachineData();
        employees = SaveLoadSystem.LoadEmployees();

        restaurantCurrency.fishes += 100;
        restaurantCurrency.Money += BigInteger.Parse("316000");// 10000;

        moneyString = Utility.GetFormattedMoney(restaurantCurrency.Money, moneyString);
        GameInstance.GameIns.uiManager.moneyText.text = moneyString.ToString();
        GameInstance.GameIns.uiManager.fishText.text = restaurantCurrency.fishes.ToString();

        // restaurantCurrency.money = 100000;
        //   restaurantCurrency.fishes = 1000;

        customerDebug = true;


        GameIns.store.NewGoods(goodsStruct);

        AutoSave(App.GlobalToken).Forget();
    }

    private void Update()
    {
        restaurantTimer += Time.deltaTime * App.restaurantTimeScale;
    }

    async UniTask AutoSave(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            // await UniTask.Delay(30000, cancellationToken: cancellationToken);

            await Utility.CustomUniTaskDelay(30f, cancellationToken);

            if(employees.changed)
            {
                employees.changed = false;
                SaveLoadSystem.SaveEmployees(employees);
            }
            if(restaurantCurrency.changed)
            {
                restaurantCurrency.changed = false;
                SaveLoadSystem.SaveRestaurantCurrency(restaurantCurrency);
            }
            if(restaurantData.changed)
            {
                restaurantData.changed = false;
                SaveLoadSystem.SaveRestaurantData(restaurantData);
            }
            if(machineLevelDataChanged)
            {
                machineLevelDataChanged = false;
                SaveLoadSystem.SaveFoodMachineStats(machineLevelData);
            }
            if (vendingData.changed)
            {
                vendingData.changed = false;
                SaveLoadSystem.SaveVendingMachineData(vendingData);
            }
        }
    }

    public void Extension()
    {
        int extensionLevel = restaurantData.extension_level;

      
        expandables[extensionLevel].SetActive(true);
        removables[extensionLevel].SetActive(false);
        extensionParticles[extensionLevel].Play();
        purchase.clip = GameIns.gameSoundManager.Extension();
        purchase.volume = 0.2f;
        purchase.Play();
        restaurantData.extension_level++;
        restaurantData.changed = true;

        if (door.removeWall != null)
        {
            if (!door.removeWall.gameObject.activeSelf)
            {
                door.transform.SetParent(door.transform.parent.parent);
                Transform doorTransform = door.transform;
                Vector3 origin = doorTransform.position;
                Vector3 forward = doorTransform.forward;
                float angle = 30f;
                Debug.DrawRay(origin + Vector3.up, -forward * float.MaxValue, Color.red, 5);
                if (Physics.Raycast(origin + Vector3.up, -forward, out var hit0, float.MaxValue, 1 << 16 | 1 << 19))
                {
                    GameObject h = hit0.collider.gameObject;
                    h.SetActive(false);
                    door.transform.position = h.transform.position - Vector3.up * h.transform.position.y;
                    door.transform.rotation = h.transform.rotation * Quaternion.Euler(0, -90, 0);
                    door.transform.SetParent(h.transform.parent);
                    door.removeWall = h;
                    StartCoroutine(door.OpenDoor());
                    goto Escape;
                }
                Vector3 leftDir = Quaternion.AngleAxis(-angle, doorTransform.up) * forward;
                Debug.DrawRay(origin + Vector3.up, -leftDir * float.MaxValue, Color.green, 5);
                if (Physics.Raycast(origin + Vector3.up, -leftDir, out var hit1, float.MaxValue, 1 << 16 | 1 << 19))
                {
                    GameObject h = hit1.collider.gameObject;
                    h.SetActive(false);
                    door.transform.position = h.transform.position - Vector3.up * h.transform.position.y;
                    door.transform.rotation = h.transform.rotation * Quaternion.Euler(0, -90, 0);
                    door.transform.SetParent(h.transform.parent);
                    door.removeWall = h;
                    StartCoroutine(door.OpenDoor());
                    goto Escape;
                }

                Vector3 rightDir = Quaternion.AngleAxis(angle, doorTransform.up) * forward;
                Debug.DrawRay(origin + Vector3.up, -rightDir * float.MaxValue, Color.blue, 5);
                if (Physics.Raycast(origin + Vector3.up, -rightDir, out var hit2, float.MaxValue, 1 << 16 | 1 << 19))
                {
                    GameObject h = hit2.collider.gameObject;
                    h.SetActive(false);
                    door.transform.position = h.transform.position - Vector3.up * h.transform.position.y;
                    door.transform.rotation = h.transform.rotation * Quaternion.Euler(0, -90, 0);
                    door.transform.SetParent(h.transform.parent);
                    door.removeWall = h;
                    StartCoroutine(door.OpenDoor());
                    goto Escape;
                }
            }
        }
        Escape: 
        MoveCalculator.CheckArea(GameIns.calculatorScale, true);
        SaveLoadSystem.SaveRestaurantBuildingData();
    }

    public void LevelUp(bool load = false, bool checkArea = true)
    {
        //   if (saveLoadManager.isLoading == false)
        {
            if (!load)
            {
                if (restaurantCurrency.Money >= levelGuides[level].money)
                {
                    restaurantCurrency.Money -= levelGuides[level].money;
                }
                else return;
               // GameInstance.GameIns.uiManager.UpdateMoneyText(restaurantCurrency.money);
                purchase.Play();
            }
        }

    //    restaurantparams[level].unlock = true;
       // combineDatas.playData[level].unlock = true;
        WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;
        if (levelGuides[level].type == MachineType.BurgerMachine && workSpaceManager.unlocks[0] == 0) workSpaceManager.unlocks[0]++;
        if (levelGuides[level].type == MachineType.CokeMachine && workSpaceManager.unlocks[0] == 1) workSpaceManager.unlocks[0]++;
        if (levelGuides[level].type == MachineType.CoffeeMachine && workSpaceManager.unlocks[1] == 0) workSpaceManager.unlocks[1]++;
        if (levelGuides[level].type == MachineType.DonutMachine && workSpaceManager.unlocks[1] == 1) workSpaceManager.unlocks[1]++;


        switch (levelGuides[level].workSpaceType)
        {
            case WorkSpaceType.Counter:
                {
                    workSpaceManager.counters.Add(levels[level].gameObject.GetComponent<Counter>());
                    break;
                }
            case WorkSpaceType.Table:
                {
                    workSpaceManager.tables.Add(levels[level].gameObject.GetComponent<Table>());
                    break;
                }
            case WorkSpaceType.FoodMachine:
                {
                    if (levels[level].gameObject.TryGetComponent<FoodMachine>(out FoodMachine foodMachine))
                    {
                        SetMachineLayer(foodMachine.modelTrans.gameObject, App.GlobalToken).Forget();
                        workSpaceManager.foodMachines.Add(foodMachine);
                        foodMachine.restaurantParam = restaurantparams[level];
                     //   foodMachine.Set(level);
                       

                   /*     for (int i = 0; i < levelData.Count; i++)
                        {
                            if (levelData[i].id == restaurantparams[level].id)
                            {
                                foodMachine.level = levelData[i].level;

                                List<MachineData> md = upgradeMachineDic[foodMachine.machineType];
                                foodMachine.machineData = md[foodMachine.level - 1];
                                //     UpgradeMachine(foodMachine);
                                break;
                            }
                        }*/
                    }
                    break;
                }
        }

        levelGuides[level++].gameObject.SetActive(false);

    //    Debug.Log(level);
        GameObject expandObject = levels[level - 1];
        expandObject.SetActive(true);

        if (expandObject.TryGetComponent<PackingTable>(out PackingTable packingTable))
            workSpaceManager.packingTables.Add(packingTable);

        if (expandObject.TryGetComponent<ScaleUp>(out ScaleUp scaleUp)) scaleUp.ObjectEnabled(load);
        if (expandObject.TryGetComponent<TableScaleUp>(out TableScaleUp tableScaleUp))
        {
            tableScaleUp.ObjectEnabled(load);
        }
        if (levelGuides.Length > level) levelGuides[level].gameObject.SetActive(true);
        if (checkArea)
        {
            MoveCalculator.CheckArea(GameInstance.GameIns.calculatorScale, true);

            for (int i = 0; i < GameInstance.GameIns.animalManager.employeeControllers.Count; i++)
            {
                Employee e = GameInstance.GameIns.animalManager.employeeControllers[i];
                e.reCalculate = true;
                int posX = Mathf.FloorToInt((e.trans.position.x - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
                int posZ = Mathf.FloorToInt((e.trans.position.z - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);
                if (!e.falling && Utility.ValidCheck(posZ, posX) && MoveCalculator.GetBlocks[MoveCalculator.GetIndex(posX, posZ)]) AnimalCollision(e, posX, posZ, App.GlobalToken).Forget();
           
            }
            for (int i = 0; i < GameInstance.GameIns.animalManager.customerControllers.Count; i++)
            {
                Customer c = GameIns.animalManager.customerControllers[i];
                if (c.trans)
                {
                    c.reCalculate = true;
                    int posX = Mathf.FloorToInt((c.trans.position.x - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
                    int posZ = Mathf.FloorToInt((c.trans.position.z - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);
                    if (Utility.ValidCheck(posZ, posX) && MoveCalculator.GetBlocks[MoveCalculator.GetIndex(posX, posZ)]) AnimalCollision(c, posX, posZ, App.GlobalToken).Forget();
                }

            }
        }
        if (!load)
        {
           // level++;
            if ((employees.num < 8 && employeeHire[employees.num] <= GetRestaurantValue()))
            {
                GameInstance.GameIns.applianceUIManager.UnlockHire(true);
            }
          //  SaveLoadSystem.SaveRestaurantData(restaurantparams);
      //      SaveLoadManager.Save(SaveLoadManager.SaveState.ONLY_SAVE_PLAYERDATA);
       //     SaveLoadManager.Save(SaveLoadManager.SaveState.ONLY_SAVE_UPGRADE);

            //for (int i = 0; i < GameInstance.GameIns.animalManager.employeeControllers.Count; i++)
            //{
            //    if (GameInstance.GameIns.animalManager.employeeControllers[i].restartCoroutine != null)
            //    {
            //        GameInstance.GameIns.animalManager.employeeControllers[i].RestartAction();
            //    }
            //}
            //for (int i = 0; i < GameInstance.GameIns.animalManager.customerControllers.Count; i++)
            //{
            //    if (GameInstance.GameIns.animalManager.customerControllers[i].restartCoroutine != null)
            //    {
            //        GameInstance.GameIns.animalManager.customerControllers[i].RestartAction();
            //    }
            //}
        }
    }

    public void ApplyPlaced(Furniture furniture)
    {
        StartCoroutine(ApplyPlacedNextFrame(furniture));    
    }

    IEnumerator ApplyPlacedNextFrame(Furniture furniture)
    {
        yield return null;
        MoveCalculator.CheckAreaWithBounds(GameInstance.GameIns.calculatorScale, furniture.GetComponentInChildren<Collider>(), true);
        //  MoveCalculator.CheckArea(GameInstance.GameIns.calculatorScale);
        for (int i = 0; i < GameInstance.GameIns.animalManager.employeeControllers.Count; i++)
        {
            Employee e = GameInstance.GameIns.animalManager.employeeControllers[i];
            e.reCalculate = true;
            int posX = Mathf.FloorToInt((e.trans.position.x - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
            int posZ = Mathf.FloorToInt((e.trans.position.z - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);
            if (!e.falling && Utility.ValidCheck(posZ, posX) && MoveCalculator.GetBlocks[MoveCalculator.GetIndex(posX, posZ)]) AnimalCollision(e, posX, posZ, App.GlobalToken).Forget();

        }
        for (int i = 0; i < GameInstance.GameIns.animalManager.customerControllers.Count; i++)
        {
            Customer c = GameIns.animalManager.customerControllers[i];
            if (c.trans)
            {
                c.reCalculate = true;
                int posX = Mathf.FloorToInt((c.trans.position.x - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
                int posZ = Mathf.FloorToInt((c.trans.position.z - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);
                if (Utility.ValidCheck(posZ, posX) && MoveCalculator.GetBlocks[MoveCalculator.GetIndex(posX, posZ)]) AnimalCollision(c, posX, posZ, App.GlobalToken).Forget();
            }
        }
    }
    async UniTask AnimalCollision(AnimalController animal, int x, int z, CancellationToken cancellationToken = default)
    {
        animal.bWait = true;
        int finalX = x;
        int finalZ = z;
       
        await UniTask.RunOnThreadPool(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            while (true)
            {
                if (Utility.ValidCheck(finalZ, finalX))
                {
                    if(!Utility.ValidCheckWithCharacterSize(finalX, finalZ, MoveCalculator.moveX, MoveCalculator.moveY))
                    //if (MoveCalculator.GetBlocks[MoveCalculator.GetIndex(finalX, finalZ)])
                    {
                        //i//nt width = MoveCalculator.GetBlocks.GetLength(1);
                      // // int height = MoveCalculator.GetBlocks.GetLength(0);
                      //  bool[,] visited = new bool[height, width];
                        bool[] visited = new bool[MoveCalculator.GetBlocks.Length];
                        Queue<(int z, int x)> queue = new();
                        queue.Enqueue((finalZ, finalX));
                        visited[MoveCalculator.GetIndex(finalX, finalZ)] = true;
                      //  visited[finalZ, finalX] = true;
                        int[] dx = { 0, 1, 0, -1 };
                        int[] dz = { 1, 0, -1, 0 };

                        while (queue.Count > 0)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            var (currentZ, currentX) = queue.Dequeue();
                            finalX = currentX;
                            finalZ = currentZ;
                            if (Utility.ValidCheckWithCharacterSize(currentX, currentZ, MoveCalculator.moveX, MoveCalculator.moveY))
                             //   if (!MoveCalculator.GetBlocks[MoveCalculator.GetIndex(currentX, currentZ)])
                            {
                                break;
                            }
                        
                            for (int i = 0; i < 4; i++)
                            {
                                int nextX = currentX + dx[i];
                                int nextZ = currentZ + dz[i];

                                if (nextX >= 0 && nextZ >= 0 && nextX < GameIns.calculatorScale.sizeX && nextZ < GameIns.calculatorScale.sizeY)
                                {
                                    if (Utility.ValidCheck(nextZ, nextX) && !visited[MoveCalculator.GetIndex(nextX, nextZ)])
                                    {
                                        visited[MoveCalculator.GetIndex(nextX, nextZ)] = true;
                                        queue.Enqueue((nextZ, nextX));
                                    }
                                }
                            }
                            Thread.Sleep(1);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        });
        float worldX = GameIns.calculatorScale.minX + finalX * (GameIns.calculatorScale.distanceSize);
        float worldZ = GameIns.calculatorScale.minY + finalZ * (GameIns.calculatorScale.distanceSize);
        Vector3 targetPos = new Vector3(worldX, 0, worldZ);
        Vector3 st = animal.trans.position;
        float t = restaurantTimer + 0.2f;
        animal.animator.SetInteger("state", 0);
        animal.animal.PlayAnimation(AnimationKeys.Idle);// (animal.animal.animationDic["Idle_A"], "Idle_A");
        while (restaurantTimer < t)
        {
            float progress = restaurantTimer / t; // 0 ~ 1
            animal.trans.position = Vector3.Lerp(st, targetPos, progress);

        //    t += Time.deltaTime;
            await UniTask.NextFrame(cancellationToken);
        }
        animal.trans.position = targetPos;

        Debug.Log(x + " " + z + " " + finalX + " " + finalZ + " " + st + " " + targetPos);
        animal.bWait = false;
    }

    async UniTask SetMachineLayer(GameObject go, CancellationToken cancellationToken = default)
    {
     //   await UniTask.Delay(1000);
        await Utility.CustomUniTaskDelay(1, cancellationToken);
       // yield return GetWaitTimer.WaitTimer.GetTimer(1000);
        go.layer = LayerMask.NameToLayer("RestaurantObject");
    }

    public async UniTask LoadRestaurant(CancellationToken cancellationToken = default)
    {
        try
        {
            await UniTask.NextFrame(cancellationToken: cancellationToken);

            for (int i = 0; i < restaurantData.extension_level; i++)
            {
                expandables[i].SetActive(true);
                removables[i].SetActive(false);
                GameIns.store.require.Add(1101 + i);
                GameIns.store.Extended(1101 + i);
            }
            if (door.setup)
            {
                door.gameObject.SetActive(true);
                Collider[] overlaps = Physics.OverlapBox(door.transform.position, new Vector3(0.1f, 1f, 0.1f), Quaternion.identity, (1 << 16) | (1 << 19));

                if (overlaps.Length > 0)
                {
                    GameObject h = overlaps[0].gameObject;
                    h.SetActive(false);
                    door.removeWall = h;
                    door.transform.SetParent(h.transform.parent);
                }
            }

            for (int i = 0; i < restaurantparams.Count; i++)
            {
                PlaceController placeController = GameIns.store.goodsPreviewDic[restaurantparams[i].id + 1000];
                Furniture furniture = GameIns.store.goodsDic[restaurantparams[i].id].Dequeue().GetComponent<Furniture>();
                furniture.spawned = true;
                furniture.gameObject.SetActive(true);
                Vector3 pos = restaurantparams[i].position;
                Vector3 localPos = restaurantparams[i].localPos;
                Quaternion rot = restaurantparams[i].rotation;
                furniture.rotateLevel = restaurantparams[i].level;
                furniture.id = restaurantparams[i].id;
                furniture.transform.position = pos;
                furniture.originPos = pos;
                switch (furniture.spaceType)
                {
                    case WorkSpaceType.Counter:
                        Transform counterOffset = furniture.GetComponent<Counter>().offset;
                        counterOffset.transform.localPosition = localPos;
                        counterOffset.transform.rotation = rot;
                        Debug.Log(rot);
                        break;
                    case WorkSpaceType.Trashcan:
                        Transform trashCanOffset = furniture.GetComponent<TrashCan>().offset;
                        trashCanOffset.transform.localPosition = localPos;
                        trashCanOffset.transform.rotation = rot;
                        break;
                    case WorkSpaceType.FoodMachine:
                        FoodMachine fm = furniture.GetComponent<FoodMachine>();
                        fm.Set(true);
                        furniture.transform.rotation = rot;
                        break;
                    default:
                        furniture.transform.rotation = rot;
                        break;
                }
                placeController.transform.position = furniture.transform.position;
                placeController.offset.transform.rotation = Quaternion.Euler(0, placeController.rotates[restaurantparams[i].level], 0);
                placeController.offset.transform.localPosition = placeController.rotateOffsets[restaurantparams[i].level];
                GameInstance.GameIns.gridManager.CheckObject(placeController);
                GameInstance.GameIns.gridManager.ApplyGird();

                GameIns.store.require.Add(restaurantparams[i].id);
                await UniTask.NextFrame(cancellationToken: cancellationToken);  
            }

            await UniTask.NextFrame(cancellationToken: cancellationToken);
            MoveCalculator.CheckArea(GameIns.calculatorScale, true);

            GameIns.store.StoreUpdate();

            await LoadEmployees(cancellationToken);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
     
    }

    async UniTask LoadEmployees(CancellationToken cancellationToken = default)
    {
        try
        {
            await UniTask.NextFrame(cancellationToken: cancellationToken);
            int num = employees.employeeLevelDatas.Count;
            for (int i = 0; i < num; i++)
            {
                Employee animal = GameInstance.GameIns.animalManager.SpawnEmployee();
                bool check = false;
                float X = 0;
                float Z = 0;
                while (!check)
                {
                    X = UnityEngine.Random.Range(-18, 22);
                    Z = UnityEngine.Random.Range(-22, 21);
                    Vector3 center = GameInstance.GetVector3(X, 0, Z);
                    Vector3 halfExtents = GameInstance.GetVector3(0.5f, 0.5f, 0.5f);
                    if (Physics.CheckBox(center, halfExtents, Quaternion.identity, 1 << 6 | 1 << 7 | 1 << 8 | 1 << 16 | 1 << 19 | 1 << 21))
                    {
                        check = false;
                    }
                    else
                    {
                        check = true;
                    }
                }
                animal.employeeLevelData = employees.employeeLevelDatas[i];
                animal.trans.position = GameInstance.GetVector3(X, 0, Z);
                animal.EXP = employees.employeeLevelDatas[i].exp;
                animal.employeeLevel = AssetLoader.employees_levels[employees.employeeLevelDatas[i].level];
                animal.employeeCallback?.Invoke(animal);

                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }

            if ((num < 8 && employeeHire[num] <= GetRestaurantValue()))
            {
                GameInstance.GameIns.applianceUIManager.UnlockHire(true);
            }
            else GameInstance.GameIns.applianceUIManager.UnlockHire(false);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void UpgradeMachine(FoodMachine foodMachine)
    {
        if (foodMachine.machineData.upgrade_cost <= restaurantCurrency.Money)
        {
            restaurantCurrency.Money -= foodMachine.machineData.upgrade_cost;
            List<MachineData> machineUpgrades = upgradeMachineDic[foodMachine.machineType];
            foodMachine.machineData = machineUpgrades[foodMachine.level - 1];

            if ((employees.num < 8 && employeeHire[employees.num] <= GetRestaurantValue()))
            {
                GameInstance.GameIns.applianceUIManager.UnlockHire(true);
            }
            else
            {
                GameInstance.GameIns.applianceUIManager.UnlockHire(false);
            }

            //   SaveLoadManager.Save(SaveState.)
        }
    }
    public void HireEmployee()
    {
        Debug.Log(employees.num < 8);
        Debug.Log(employeeHire[employees.num] + " " + GetRestaurantValue());
        Debug.Log(GameIns.inputManager.CheckHire());

        if (employees.num < 8 && employeeHire[employees.num] <= GetRestaurantValue() && GameIns.inputManager.CheckHire())
        {
            employees.num++;

            //SaveLoadManager.Save(SaveLoadManager.SaveState.ONLY_SAVE_PLAYERDATA);//
            Debug.Log("A");
            EmployeeNum();
        }
       // EmployeeNum();
    }

    private bool isFishUpdating = false;

    public void GetFish()
    {
        if (isFishUpdating) return; // 중복 호출 방지
        isFishUpdating = true;

        float fillAmount = GameInstance.GameIns.applianceUIManager.rewardChest_Fill.GetComponent<UnityEngine.UI.Image>().fillAmount;
        int trashcanFish = Mathf.FloorToInt(fillAmount * 20); // 소수점 절삭

        // Update fishesNum
        int oldFishesNum = restaurantCurrency.fishes;
        restaurantCurrency.fishes += trashcanFish;

        // Update UI
        GameInstance.GameIns.uiManager.fishText.text = restaurantCurrency.fishes.ToString();

        // Reset fillAmount
        GameInstance.GameIns.applianceUIManager.rewardChest_Fill.GetComponent<UnityEngine.UI.Image>().fillAmount = 0;

        isFishUpdating = false;
    }


    public void EmployeeNum()
    {
        for (int i = 0; i < 8; i++)
        {
            Employee animal = GameInstance.GameIns.animalManager.SpawnEmployee();
            EmployeeLevelData levelData = new EmployeeLevelData(1, 0, 100);
            employees.employeeLevelDatas.Add(levelData);
            animal.employeeLevelData = levelData;

            //  animal.EmployeeData = employeeDatas[combineDatas.employeeData[animal.id - 1].level - 1];
            animal.employeeLevel = AssetLoader.employees_levels[1];
            animal.animal.audioSource.clip = GameIns.gameSoundManager.Quack();
            animal.animal.audioSource.volume = 0.2f;
            animal.animal.audioSource.Play();
            // animal.EmployeeData = new EmployeeData(1, 1, 1, 1, 1);
            //   Vector3 targetPos = GameInstance.GameIns.inputManager.cameraRange.position;
            //animal.busy = true;
            Vector3 screenPos;
#if UNITY_ANDROID || UNITY_IOS
                screenPos = Touchscreen.current.touches[0].position.ReadValue();
#else
            screenPos = Mouse.current.position.ReadValue();
#endif
            animal.trans.position = Camera.main.ScreenToWorldPoint(screenPos);

            if ((employees.num < 8 && employeeHire[employees.num] <= GetRestaurantValue()))
            {
                GameInstance.GameIns.applianceUIManager.UnlockHire(true);
            }
            else
            {
                GameInstance.GameIns.applianceUIManager.UnlockHire(false);
            }

            animal.StartFalling(true);
            SaveLoadSystem.SaveEmployees(employees);
        }
    }


    public void UseFish()
    {
        restaurantCurrency.fishes--;
     //   restaurantCurrency.fishesNum_InBox++;
        //SaveLoadManager.PlayerStateSave();
        GameInstance.GameIns.uiManager.fishText.text = restaurantCurrency.fishes.ToString();
    }


    public Transform start;
    public Transform end;
    [Range(1f, 100f)]
    public float duration;
    [Range(1f, 40f)]
    public float height;
    [Range(1f, 4f)]
    public float weight;
    [Range(1f, 4f)]
    public float weight2;
    public void testCode()
    {
        float x = 0;
        float z = 0;
        float length = 0;
        for (int i = 0; i < 100; i++)
        {
            x = UnityEngine.Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
            z = UnityEngine.Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
            length = UnityEngine.Random.Range(-Camera.main.orthographicSize / 2.5f, Camera.main.orthographicSize / 2.5f);
            Vector3 test = GameInstance.GetVector3(GameInstance.GameIns.inputManager.cameraRange.position.x, 0, GameInstance.GameIns.inputManager.cameraRange.position.z);
            Vector3 n = GameInstance.GetVector3(x, 0, z).normalized;
            Vector3 ta = test + n * length;

            if (Physics.Raycast(ta + Vector3.up * 5, Vector3.down, 10))
            {
                Debug.DrawLine(ta + Vector3.up * 10, ta + Vector3.down * 100f, Color.red, 100);


            }

        }
    }

    IEnumerator movePenguin(AnimalController animal)
    {
      //  testCode();
        animal.animator.SetInteger("state", 3);
        Vector3 startPoint = animal.trans.position; //start.position;

        float x = 0;
        float z = 0;
        float length = 0;
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
                Debug.DrawLine(ta + Vector3.up * 10, test + Vector3.down * 100f, Color.red, 100);

                if (Physics.CheckBox(ta, GameInstance.GetVector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1 << 6 | 1 << 7 | 1 << 8))
                {

                }
                else break;
            }
            yield return null;
        }


        Vector3 endPoint = GameInstance.GetVector3(GameInstance.GameIns.inputManager.cameraRange.position.x, 0, GameInstance.GameIns.inputManager.cameraRange.position.z);
        Vector3 dir2 = GameInstance.GetVector3(x, 0, z).normalized;

        endPoint += dir2 * length;
        bool bstart = true;
        Vector3 controlVector = (startPoint + endPoint) / weight + Vector3.up * height;

        float elapsedTime = 0f;

        while (bstart)
        {

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            Vector3 origin = animal.trans.position;
            // Vector3 targetLoc = Vector3.Lerp(startPoint, endPoint, t);// CalculateBezierPoint(t, startPoint, controlVector, endPoint);
            Vector3 targetLoc = CalculateBezierPoint(t, startPoint, controlVector, endPoint);

            Vector3 dir = targetLoc - origin;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            animal.modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Debug.DrawLine(animal.trans.position, targetLoc, Color.red, 5f);
            animal.trans.position = targetLoc;


            if (t >= 1.0f)
            {
                animal.trans.position = endPoint;
                bstart = false;
            }

            yield return null;
        }

        animal.animator.SetInteger("state", 0);
        animal.busy = false;

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

    public void UpgradePenguin(int level, bool isLoad, Employee animalController)
    {
        if (AssetLoader.employees_levels.ContainsKey(level))
        {
         //   EmployeeData nextEmployeeData = employeeDatas[level];
            //     currentEmployeeData = employeeDatas[level];
            //   playerData.level = level;
            EmployeeLevelStruct employeeLevel = AssetLoader.employees_levels[level];
            if (!isLoad)
            {
               // GameInstance.GameIns.animalManager.UpdateEmployeeUpgrade(animalController);
                
                animalController.employeeLevel = employeeLevel;
                //  GameInstance.GameIns.applianceUIManager.ShowPenguinUpgradeInfo(animalController, true);
                //combineDatas.employeeData[]
                //SaveLoadManager.EmployeeLevelSave(true);

                if ((employees.num < 8 && employeeHire[employees.num] <= GetRestaurantValue()))
                {
                    GameInstance.GameIns.applianceUIManager.UnlockHire(true);
                }
                else
                {
                    GameInstance.GameIns.applianceUIManager.UnlockHire(false);
                }
            }
            else
            {
                animalController.employeeLevel = employeeLevel;
                //GameInstance.GameIns.animalManager.UpdateEmployeeUpgrade(animalController);
            }

        }
    }

    public void UpgradeFoodMachine(FoodMachine foodMachine)
    {
        if (restaurantCurrency.Money >= foodMachine.machineLevelData.Price_Value)
        {
            GameIns.uiManager.audioSource.clip = GameIns.uISoundManager.Money();
            GameIns.uiManager.audioSource.volume = 0.2f;
            GameIns.uiManager.audioSource.Play();
            restaurantCurrency.Money -= foodMachine.machineLevelData.Price_Value;
            GetMoney((-foodMachine.machineLevelData.Price_Value).ToSafeString()); 
            MachineType type = foodMachine.machineType;
        
            int currentLevel = foodMachine.machineLevelData.level;

            //foodMachine.machineLevelStruct = //GameInstance.GameIns.restaurantManager.machineLevelData[type][currentLevel + 1];

           // GameInstance.GameIns.applianceUIManager.ShowApplianceInfo(foodMachine);


            if ((employees.num < 8 && employeeHire[employees.num] <= GetRestaurantValue()))
            {
                GameInstance.GameIns.applianceUIManager.UnlockHire(true);
            }
            else
            {
                GameInstance.GameIns.applianceUIManager.UnlockHire(false);
            }
            //   SaveLoadManager.Save(SaveState.ONLY_SAVE_UPGRADE);//
            //  SaveLoadManager.Save(SaveState.ONLY_SAVE_PLAYERDATA);//
        }
    }

    public void AddFuel(FoodMachine foodMachine, int amount)
    {
        GameIns.uiManager.audioSource.clip = GameIns.uISoundManager.Fish();
        GameIns.uiManager.audioSource.volume = 0.2f;
        GameIns.uiManager.audioSource.Play();
        GetFish(-amount);
        restaurantCurrency.changed = true;
        foodMachine.machineLevelData.fishes += amount;
        machineLevelDataChanged = true; 
    }

    public float GetRestaurantValue()
    {
        WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;
        float val = 0;
        /*   for(int i=0; i< combineDatas.playData.Count; i++)
           {
               if (combineDatas.playData[i].unlock)
               {
                   int type = combineDatas.playData[i].type;
                   switch(type)
                   {
                       case 0:
                           val += 50;
                           break;
                       case 1:
                           val += 100;
                           break;
                       case 2:
                           val += 150;
                           break;
                       case 3:
                           val += 200;
                           break;
                   }
               }
           }*/
        for (int i = 0; i < restaurantparams.Count; i++)
        {
        //    if (restaurantparams[i].unlock)
            {
                /*   int type = restaurantparams[i].type;
                   switch (type)
                   {
                       case 0:
                           val += 50;
                           break;
                       case 1:
                           val += 100;
                           break;
                       case 2:
                           val += 150;
                           break;
                       case 3:
                           val += 200;
                           break;
                   }*/
                val += 100;
            }
        }
        for (int i=0; i < workSpaceManager.foodMachines.Count; i++)
        {
            MachineType machineType = workSpaceManager.foodMachines[i].machineType;
            int l = workSpaceManager.foodMachines[i].machineLevelData.level;
            //  List<MachineData> machindb = machineLevelData[machineType]; //upgradeMachineDic[machineType];
         //   Debug.Log(machineType + " " + l);

    //        int proceeds = machineLevelData[machineType][l].sale_proceed; //machindb[workSpaceManager.foodMachines[i].machineLevelStruct.level - 1].sale_proceeds;
      //      int maxheight = machineLevelData[machineType][l].max_height; //machindb[workSpaceManager.foodMachines[i].machineLevelStruct.level - 1].food_production_max_height;
      //      float speed = machineLevelData[machineType][l].cooking_time; //machindb[workSpaceManager.foodMachines[i].machineLevelStruct.level - 1].food_production_speed;

            val += 100 * (l - 1);
        }

        for(int i=0; i< GameInstance.GameIns.animalManager.employeeControllers.Count; i++)
        {
            Employee newAnimalController = GameInstance.GameIns.animalManager.employeeControllers[i];
            //    float action_speed = newAnimalController.employeeLevel.//.action_speed;
            // float move_speed = newAnimalController.employeeLevel.move_speed;
            // float max_holds = newAnimalController.employeeLevel.max_weight;
            //   val += move_speed * max_holds * 15.625f;
            val += 100 * (newAnimalController.employeeLevel.level - 1); 
        }
       // Debug.Log(val);
        return val;
    }

    private void OnApplicationQuit()
    {
        // SaveLoadManager.Save(SaveState.ALL_SAVES);
        ///if (restaurantCurrency == null) Debug.Log("LL");
        SaveLoadSystem.SaveRestaurantCurrency(restaurantCurrency);
        SaveLoadSystem.SaveEmployees(employees);
        SaveLoadSystem.SaveRestaurantData(restaurantData);
        SaveLoadSystem.SaveFoodMachineStats(machineLevelData);
        SaveLoadSystem.SaveVendingMachineData(vendingData);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveLoadSystem.SaveRestaurantCurrency(restaurantCurrency);
            SaveLoadSystem.SaveEmployees(employees);
            SaveLoadSystem.SaveRestaurantData(restaurantData);
            //     SaveLoadManager.Save(SaveState.ALL_SAVES);
            SaveLoadSystem.SaveVendingMachineData(vendingData);
        }
    }

    public void NewTrays()
    {
        for(int i=0; i < 20; i++)
        {
            GameObject tray = Instantiate(AssetLoader.loadedAssets[AssetLoader.itemAssetKeys[1000].ID], trayObjects.transform);
            tray.SetActive(false);
            trays.Enqueue(tray);
        }
    }
    public GameObject GetTray()
    { 
        GameObject tray = trays.Count > 0 ? trays.Dequeue() : Instantiate(AssetLoader.loadedAssets[AssetLoader.itemAssetKeys[1000].ID], trayObjects.transform);
        tray.SetActive(true);
        return tray;
    }

    public void TrayPool(GameObject tray)
    {
        tray.SetActive(false);
        trays.Enqueue(tray);
    }

    public FuelGage GetGage()
    {
        FuelGage fuel = fuelGages.Dequeue();
        return fuel;
    }

    public void GetFish(int addFish, bool animate = true)
    {
        int before = restaurantCurrency.fishes;
        restaurantCurrency.fishes += addFish;
        if (animate)
        {
            StartCoroutine(ChangingFishes(before, addFish));
        }
        else
        {
            GameIns.uiManager.fishText.text = restaurantCurrency.fishes.ToString();
        }
    }

    IEnumerator ChangingFishes(int before, int changed)
    {
        float f = 0;
        if (changed > 0)
        {
            while(f <= 0.5f)
            {
                int test = before + (int)(changed * (f * 2 * 1000) / 1000);
                GameIns.uiManager.fishText.text = test.ToString();
                f += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        else
        {
            while (f <= 0.5f)
            {
                int test = (before) + (int)(changed * (f * 2 * 1000) / 1000);
                GameIns.uiManager.fishText.text = test.ToString();
                f += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        GameIns.uiManager.fishText.text = restaurantCurrency.fishes.ToString();
        if (GameIns.uiManager.audioSource.clip == GameIns.uISoundManager.Fish())
        {
            GameIns.uiManager.audioSource.Stop();
        }
    }

    public void GetMoney(string addMoney, bool animate = true)
    {
        BigInteger bigInteger = BigInteger.Parse(addMoney);
        BigInteger before = restaurantCurrency.Money;
        restaurantCurrency.money += bigInteger;

        if(animate)
        {
            StartCoroutine(ChangingMoney(before, bigInteger));
        }
        else
        {
            moneyString = Utility.GetFormattedMoney(restaurantCurrency.Money, moneyString);
            GameIns.uiManager.moneyText.text = moneyString.ToString();
        }
    }

    IEnumerator ChangingMoney(BigInteger before, BigInteger changed)
    {
        float f = 0;
    //    Debug.LogWarning(restaurantCurrency.money);
        if(changed > 0)
        {
            while (f <= 0.5f)
            {
                BigInteger test = before + (changed * (BigInteger)(f * 2 * 1000)) / 1000;
                moneyString = Utility.GetFormattedMoney(test, moneyString);
                GameIns.uiManager.moneyText.text = moneyString.ToString();
                f += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        else
        {
            while (f <= 0.5f)
            {
                BigInteger test = (before - changed) + (changed * (BigInteger)(f * 2 * 1000)) / 1000;
                Debug.Log(test.ToString());
                moneyString = Utility.GetFormattedMoney(test, moneyString);
                GameIns.uiManager.moneyText.text = moneyString.ToString();
                f += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    
        moneyString = Utility.GetFormattedMoney(restaurantCurrency.Money, moneyString);
        GameIns.uiManager.moneyText.text = moneyString.ToString();
        if(GameIns.uiManager.audioSource.clip == GameIns.uISoundManager.Money())
        {
            GameIns.uiManager.audioSource.Stop();
        }
    }
}
