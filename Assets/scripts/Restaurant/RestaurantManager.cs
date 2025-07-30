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
using Vector2 = UnityEngine.Vector2;
using Random = UnityEngine.Random;

public class RestaurantManager : MonoBehaviour
{
    [NonSerialized] public int[] employeeHire = { 0, 700, 1200, 2000, 3000, 5000, 8000, 12000 };
    
    public GameObject[] levels;
    public NextTarget[] levelGuides;
    NextTargetData[] nextTargetDatas = new NextTargetData[100];
    [NonSerialized] public int level = 0;
    public bool allLevelUp;
    [NonSerialized] public int fishNum = 0;
    public bool employable;
    public int restaurantValue = 0;

    public PlayerStruct playerStruct = new PlayerStruct(0, 0f, 0, 0, null, 0);
    public List<EmployeeLevelStruct> employeeData = new List<EmployeeLevelStruct>();
    public List<RestaurantParam> restaurantparams = new List<RestaurantParam>();

    public Dictionary<int, LevelData> levelData = new Dictionary<int, LevelData>();
    public CombineDatas combineDatas;
 
    public List<EmployeeData> employeeDatas = new List<EmployeeData>();
    public EmployeeData currentEmployeeData;
    public Dictionary<MachineType, List<MachineData>> upgradeMachineDic = new Dictionary<MachineType, List<MachineData>>();

    [NonSerialized] public List<Vector3> flyingEndPoints = new List<Vector3>();

    public List<FoodStack> foodStacks = new List<FoodStack>();

    public bool employeeDebug;
    public bool customerDebug;

    public Dictionary<int, GoodsStruct> goodsStruct = new Dictionary<int, GoodsStruct>();

    public static GameObject trayObjects;
    public static GameObject emoteObjects;
    public Queue<GameObject> trays = new Queue<GameObject>();
    public Door door;
    public DoorController doorPreview;
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

    [NonSerialized] public VendingMachineData vendingData;
    [NonSerialized] public TrashData trashData;

    public static float restaurantTimer;
    public static int spawnTimer { get; set; }
    public static int spawnerNum;
    [NonSerialized] public int moneyChangedSoundKey;
    [NonSerialized] public int fishChangedSoundKey;
    Coroutine changingMoneyCoroutine;
    Coroutine changingFishCoroutine;
    public MiniGame miniGame;

    public Emote emote;
    public Queue<Emote> emotes = new Queue<Emote>();
    public List<int> emoteSpriteKeys = new List<int>();
    public Dictionary<int, Sprite> emoteSprites = new Dictionary<int, Sprite>();

    public FloatingCost floatingCost;
    public Queue<FloatingCost> floatingCosts = new Queue<FloatingCost>();

    public GameObject fishIcon;
    public Queue<GameObject> fishRewardIconQueue = new Queue<GameObject>();
    public RectTransform fishIconCanvas;

    public ChangableWall[] changableWalls;
    private void Awake()
    {
        moneyChangedSoundKey = 100011;
        fishChangedSoundKey = 100012;

        door = Instantiate(door);
        door.id = 2000;
        door.gameObject.SetActive(false);
        doorPreview = Instantiate(doorPreview);
        doorPreview.gameObject.SetActive(false);
        trayObjects = new GameObject();
        trayObjects.name = "trayObjects";
        emoteObjects = new GameObject();
        emoteObjects.AddComponent<Canvas>().renderMode = RenderMode.WorldSpace; //RenderMode.ScreenSpaceOverlay;
        RectTransform r = emoteObjects.GetComponent<RectTransform>();
        r.sizeDelta = new Vector2(1080, 1920);
        emoteObjects.name = "emotes";
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
        ApplyLoadedFoodMachineStat();
    }
    void Start()
    {
        for(int i=0; i<emoteSpriteKeys.Count; i++)
        {
            emoteSprites[emoteSpriteKeys[i]] = AssetLoader.loadedAtlases["UI"].GetSprite(AssetLoader.spriteAssetKeys[emoteSpriteKeys[i]].ID);
        }
        for(int i=0; i< 30; i++)
        {
            Emote e = Instantiate(emote, emoteObjects.transform);
            e.gameObject.SetActive(false);
            emotes.Enqueue(e);
        }
        for(int i=0; i<10; i++)
        {
            FloatingCost fc = Instantiate(floatingCost, emoteObjects.transform);
            fc.gameObject.SetActive(false);
            floatingCosts.Enqueue(fc);
        }


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
        miniGame = SaveLoadSystem.LoadMiniGameStatus();
        trashData = SaveLoadSystem.LoadTrashData();



       // restaurantCurrency.fishes += 100;
     //   restaurantCurrency.Money += BigInteger.Parse("316000");// 10000;

        moneyString = Utility.GetFormattedMoney(restaurantCurrency.Money, moneyString);
        GameInstance.GameIns.uiManager.moneyText.text = moneyString.ToString();
        GameInstance.GameIns.uiManager.fishText.text = restaurantCurrency.fishes.ToString();

        customerDebug = true;


        GameIns.store.NewGoods(goodsStruct);

        AutoSave(App.GlobalToken).Forget();

        for (int i = 0; i < 1000; i++)
        {
            GameObject fishIconObject = Instantiate(fishIcon, fishIconCanvas.transform);
            fishIconObject.SetActive(false);
            fishRewardIconQueue.Enqueue(fishIconObject);
        }
    }

    private void Update()
    {
        restaurantTimer += Time.deltaTime * App.restaurantTimeScale;
    }

    async UniTask AutoSave(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            await UniTask.Delay(30000, DelayType.UnscaledDeltaTime, cancellationToken: cancellationToken);

           // await Utility.CustomUniTaskDelay(30f, cancellationToken);

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
            if(miniGame.changed)
            {
                miniGame.changed = false;
                SaveLoadSystem.SaveMiniGameStatus(miniGame);
            }
            if(trashData.changed)
            {
                trashData.changed = false;
                SaveLoadSystem.SaveTrashData(trashData);
            }
        }
    }

    public void Extension()
    {
        int extensionLevel = restaurantData.extension_level;

      
        expandables[extensionLevel].SetActive(true);
        removables[extensionLevel].SetActive(false);
        extensionParticles[extensionLevel].Play();
        SoundManager.Instance.PlayAudio(GameIns.gameSoundManager.Extension(), 0.2f);
       
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
                if (Physics.Raycast(origin + Vector3.up, -forward, out var hit0, float.MaxValue, 1 << 16 | 1 << 19))
                {
                    GameObject h = hit0.collider.gameObject;
                    h.SetActive(false);
                    door.transform.position = h.transform.position - Vector3.up * h.transform.position.y;
                    door.transform.rotation = h.transform.rotation * Quaternion.Euler(0, -90, 0);
                    door.transform.SetParent(h.transform.parent);
                    door.removeWall = h;

                //    StartCoroutine(door.OpenDoor());
                    goto Escape;
                }
                Vector3 leftDir = Quaternion.AngleAxis(-angle, doorTransform.up) * forward;
                if (Physics.Raycast(origin + Vector3.up, -leftDir, out var hit1, float.MaxValue, 1 << 16 | 1 << 19))
                {
                    GameObject h = hit1.collider.gameObject;
                    h.SetActive(false);
                    door.transform.position = h.transform.position - Vector3.up * h.transform.position.y;
                    door.transform.rotation = h.transform.rotation * Quaternion.Euler(0, -90, 0);
                    door.transform.SetParent(h.transform.parent);
                    door.removeWall = h;
               //     StartCoroutine(door.OpenDoor());
                    goto Escape;
                }

                Vector3 rightDir = Quaternion.AngleAxis(angle, doorTransform.up) * forward;
                if (Physics.Raycast(origin + Vector3.up, -rightDir, out var hit2, float.MaxValue, 1 << 16 | 1 << 19))
                {
                    GameObject h = hit2.collider.gameObject;
                    h.SetActive(false);
                    door.transform.position = h.transform.position - Vector3.up * h.transform.position.y;
                    door.transform.rotation = h.transform.rotation * Quaternion.Euler(0, -90, 0);
                    door.transform.SetParent(h.transform.parent);
                    door.removeWall = h;
                 //   StartCoroutine(door.OpenDoor());
                    goto Escape;
                }
            }
        }
        Escape:

        MeshRenderer meshRenderer = door.GetComponentInChildren<MeshRenderer>();
        Material[] materials = meshRenderer.materials;
        for (int i = 0; i < door.doorMat.Count; i++)
        {
            materials[i] = door.doorMat[i];
        }
        meshRenderer.materials = materials;
        GameInstance.GameIns.restaurantManager.ApplyPlaced(door);

        SaveLoadSystem.SaveRestaurantBuildingData();
        restaurantparams = SaveLoadSystem.LoadRestaurantBuildingData();
        int testX = -999;
        int testY = -999;
        Utility.CheckHirable(GameIns.inputManager.cameraRange.position, ref testX, ref testY, false, true);
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
              // purchase.Play();
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

    public void ApplyLoadedFoodMachineStat()
    {
        foreach (var v in machineLevelData)
        {
            int level = v.Value.level;
            int id = v.Value.id;
            int index = id + (level >= 41 ? 41 : level >= 31 ? 31 : 1);

            MachineLevelOffset offset = AssetLoader.machineLevelOffsets[index];

            BigInteger upgradePrice = v.Value.Price_Value + Mathf.FloorToInt(Mathf.Pow((level - 1), offset.price_pow) * offset.price_mul);
            moneyString = Utility.GetFormattedMoney(upgradePrice, moneyString);
            v.Value.calculatedPrice = moneyString.ToString();
            BigInteger salePrice = Utility.StringToBigInteger(v.Value.sale_proceed); 
            if(offset.sale_div == 0)  salePrice += Mathf.FloorToInt(Mathf.Pow((level - 1), offset.sale_pow) * offset.sale_mul);
            else salePrice += Mathf.FloorToInt(Mathf.Pow((level - 1), offset.sale_pow) * offset.sale_mul) / Mathf.FloorToInt((level - 1) * 0.07f);
            v.Value.calculatedSales = salePrice;

            v.Value.calculatedCookingTimer = v.Value.cooking_time - (level - 1) * offset.reduce_timer;
            if (v.Value.calculatedCookingTimer < 3f) v.Value.calculatedCookingTimer = 3f;
            v.Value.calculatedHeight = v.Value.max_height + Mathf.FloorToInt((level - 1) * offset.increase_height);

        }
    }
    IEnumerator ApplyPlacedNextFrame(Furniture furniture)
    {
        yield return null;

        SaveLoadSystem.SaveRestaurantBuildingData();
        if (furniture.spaceType == WorkSpaceType.Table)
        {
            for(int i=0;i<GameIns.workSpaceManager.tables.Count; i++)
            {
                Table t = GameIns.workSpaceManager.tables[i];
                MoveCalculator.CheckAreaWithBounds(GameInstance.GameIns.calculatorScale, t.GetComponentInChildren<Collider>(), true);
              
            }
            yield return null;
            List<Table> table = new List<Table>();
            table = GameIns.workSpaceManager.tables;
            TableUpdate(table);
        }
        else if(furniture.spaceType == WorkSpaceType.Door)
        {
            MoveCalculator.CheckArea(GameInstance.GameIns.calculatorScale, true);
        }
        else
        {
            MoveCalculator.CheckAreaWithBounds(GameInstance.GameIns.calculatorScale, furniture.GetComponentInChildren<Collider>(), true);
        }
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


        BlackConsumer b = GameIns.animalManager.blackConsumer;
        if (b != null)
        {

            b.reCalculate = true;
            int posX = Mathf.FloorToInt((b.trans.position.x - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
            int posZ = Mathf.FloorToInt((b.trans.position.z - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);
            if (Utility.ValidCheck(posZ, posX) && MoveCalculator.GetBlocks[MoveCalculator.GetIndex(posX, posZ)]) AnimalCollision(b, posX, posZ, App.GlobalToken).Forget();
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
        if(animal.animal != null) animal.animal.PlayAnimation(AnimationKeys.Idle);// (animal.animal.animationDic["Idle_A"], "Idle_A");
        while (restaurantTimer < t)
        {
            float progress = restaurantTimer / t; // 0 ~ 1
            animal.trans.position = Vector3.Lerp(st, targetPos, progress);

            await UniTask.NextFrame(cancellationToken);
        }
        animal.trans.position = targetPos;

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

            changableWalls = FindObjectsByType<ChangableWall>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            
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
                door.spawned = true;
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

            List<Table> tables = new List<Table>();
            for (int i = 0; i < restaurantparams.Count; i++)
            {
                PlaceController placeController = GameIns.store.goodsPreviewDic[restaurantparams[i].id + 1000];
                Furniture furniture = GameIns.store.goodsDic[restaurantparams[i].id].Dequeue().GetComponent<Furniture>();
                furniture.placed = true;
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
                    case WorkSpaceType.Table:
                        furniture.transform.rotation = rot; 
                        tables.Add(furniture.GetComponent<Table>());
                        break;
                    default:
                        furniture.transform.rotation = rot;
                        break;
                }           

                placeController.transform.position = furniture.transform.position;
                placeController.offset.transform.rotation = Quaternion.Euler(0, placeController.rotates[restaurantparams[i].level], 0);
                placeController.offset.transform.localPosition = placeController.rotateOffsets[restaurantparams[i].level];
                GameInstance.GameIns.gridManager.CheckObject(placeController, placeController.storeGoods.goods.type);
                GameInstance.GameIns.gridManager.ApplyGird(placeController, placeController.offset.transform.position, furniture.spaceType);
                          
                GameIns.store.require.Add(restaurantparams[i].id);
                await UniTask.NextFrame(cancellationToken: cancellationToken);  
            }

            await UniTask.NextFrame(cancellationToken: cancellationToken);
            MoveCalculator.CheckArea(GameIns.calculatorScale, true);


            await UniTask.NextFrame(cancellationToken: cancellationToken);

            TableUpdate(tables);
          

            GameIns.store.StoreUpdate();

            if (miniGame.activate)
            {
                switch (miniGame.type)
                {
                    case MiniGameType.None:
                        break;
                    case MiniGameType.Fishing:
                        GameIns.fishingManager.LoadStatus(miniGame.fishing);
                        GameIns.uiManager.fishingBtn.gameObject.SetActive(true);
                        //     OpenMiniGame(MiniGameType.Fishing);
                        break;
                }
            }
          
            GameIns.uiManager.reputation.text = restaurantCurrency.reputation.ToString();

            GameIns.applianceUIManager.rewardChest_Fill.uiImage.fillAmount = trashData.trashPoint * 0.01f; 
            if(trashData.trashPoint == 100)
            {
                GameIns.applianceUIManager.rewardChest_Fill.ChangeHighlight(true);
            }

            CalculateSpawnTimer();
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
            int num = employees.num;
            int x=0, y=0;
            Utility.CheckHirable(Vector3.zero, ref x, ref y, false, true, true);

       //     Debug.Log(InputManger.spawnDetects.Count);


            for (int i = 0; i < num; i++)
            {
                int r = Random.Range(0, InputManger.spawnDetects.Count);

                Vector3 t = InputManger.spawnDetects[r];

                Employee animal = GameIns.animalManager.SpawnEmployee();
                animal.employeeLevelData = employees.employeeLevelDatas[i];
                animal.trans.position = t;
                animal.employeeLevel = AssetLoader.employees_levels[0];
                animal.employeeLevelData.targetEXP = animal.employeeLevel.exp + Mathf.FloorToInt(Mathf.Pow((animal.employeeLevelData.level - 1), animal.employeeLevel.increase_exp_pow) * animal.employeeLevel.increase_exp_mul);
                animal.employeeLevelData.speed = animal.employeeLevel.move_speed + (animal.employeeLevelData.level - 1) * 0.2f;
                animal.employeeLevelData.max_weight = animal.employeeLevel.max_weight + Mathf.FloorToInt((animal.employeeLevelData.level - 1) / 2);
                animal.EXP = employees.employeeLevelDatas[i].exp;
                animal.ui.UpdateLevel(animal.employeeLevelData.level);
                animal.employeeCallback?.Invoke(animal);

                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }

            InputManger.spawnDetects = new List<Vector3>();
            /*      while (true)
                  {
                      int r = Random.Range(0, InputManger.spawnDetects.Count);

                      Vector3 t = InputManger.spawnDetects[r];

                      Employee animal = GameIns.animalManager.SpawnEmployee();
                      bool check = false;
                      float X = 0;
                      float Z = 0;
                      while (!check)
                      {
                          X = Random.Range(-18, 22);
                          Z = Random.Range(-22, 21);

                          float minX = GameIns.calculatorScale.minX;
                          float minY = GameIns.calculatorScale.minY;
                          float distanceSize = GameIns.calculatorScale.distanceSize;

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
                      animal.employeeLevel = AssetLoader.employees_levels[0];
                      animal.employeeLevelData.targetEXP = animal.employeeLevel.exp + Mathf.FloorToInt(Mathf.Pow((animal.employeeLevelData.level - 1), animal.employeeLevel.increase_exp_mul) * animal.employeeLevel.increase_exp_mul);
                      animal.employeeLevelData.speed = animal.employeeLevel.move_speed + (animal.employeeLevelData.level - 1) * 0.2f;
                      animal.employeeLevelData.max_weight = animal.employeeLevel.max_weight + Mathf.FloorToInt((animal.employeeLevelData.level - 1) / 2);
                      animal.EXP = employees.employeeLevelDatas[i].exp;
                      animal.ui.UpdateLevel(animal.employeeLevelData.level);
                      animal.employeeCallback?.Invoke(animal);

                      await UniTask.NextFrame(cancellationToken: cancellationToken);
                  }
      */
            GameInstance.GameIns.applianceUIManager.UnlockHire(true);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }


    public void TableUpdate(List<Table> tableList)
    {
        List<Table> tables = tableList;
        Dictionary<Vector2, int> tableData = new Dictionary<Vector2, int>();
        for (int i = 0; i < tables.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {

                int playerX = Mathf.FloorToInt(tables[i].seats[j].transform.position.x);
                int playerY = Mathf.FloorToInt(tables[i].seats[j].transform.position.z);

                Vector2 key = new Vector2(playerX, playerY);
                if (!tableData.ContainsKey(key)) tableData[key] = 1;
                else tableData[key]++;
            }
        }

        for (int i = 0; i < tables.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int playerX = Mathf.FloorToInt((tables[i].seats[j].transform.position.x - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
                int playerY = Mathf.FloorToInt((tables[i].seats[j].transform.position.z - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);
                int x = Mathf.FloorToInt(tables[i].seats[j].transform.position.x);
                int y = Mathf.FloorToInt(tables[i].seats[j].transform.position.z);
                Vector2 key = new Vector2(x, y);

                if (MoveCalculator.GetBlocks[MoveCalculator.GetIndex(playerX, playerY)])
                {
                    tables[i].seats[j].isDisEnabled = true;
                    if (tables[i].seats[j].animal != null) tables[i].seats[j].animal.reCalculate = true;
                }
                else
                {
                    
                    if (tableData.ContainsKey(key))
                    {
                        if (tableData[key] > 1)
                        {
                            tables[i].seats[j].isDisEnabled = true;
                            if (tables[i].seats[j].animal != null) tables[i].seats[j].animal.reCalculate = true;
                        }
                        else
                        {
                            tables[i].seats[j].isDisEnabled = false;
                        }
                    }
                }
            }
        }
    }

    public void UpgradeMachine(FoodMachine foodMachine)
    {
        if (foodMachine.machineData.upgrade_cost <= restaurantCurrency.Money)
        {
            restaurantCurrency.Money -= foodMachine.machineData.upgrade_cost;
            List<MachineData> machineUpgrades = upgradeMachineDic[foodMachine.machineType];
            foodMachine.machineData = machineUpgrades[foodMachine.level - 1];

            GameInstance.GameIns.applianceUIManager.UnlockHire(true);
            //   SaveLoadManager.Save(SaveState.)
        }
    }
    public void HireEmployee()
    {
        if (employees.num < 8 && employeeHire[employees.num] <= GetRestaurantValue() && GameIns.inputManager.CheckHire())
        {
            employees.num++;
            EmployeeNum();
        }
    }

    private bool isFishUpdating = false;

    public void GetReward()
    {
        if (trashData.trashPoint != 100) return;

        trashData.trashPoint = 0;
        trashData.changed = true;
        GameInstance.GameIns.applianceUIManager.rewardChest_Fill.uiImage.fillAmount = 0;
        GameIns.applianceUIManager.rewardChest.SetActive(false);
        GameIns.applianceUIManager.rewardChest_Fill.ChangeHighlight(false);
        GameIns.applianceUIManager.rewardChest_Fill.gameObject.SetActive(false);
        GameIns.applianceUIManager.clickerReward.gameObject.SetActive(true);
        GameIns.applianceUIManager.clickerReward.trashcanImage.sprite = AssetLoader.loadedSprites[AssetLoader.spriteAssetKeys[5001].ID];
        GameIns.applianceUIManager.clickerReward.StartClicker();
    }


    public void EmployeeNum()
    {
     //   for (int i = 0; i < 8; i++)
        {
            Employee animal = GameInstance.GameIns.animalManager.SpawnEmployee();
            EmployeeLevelData levelData = new EmployeeLevelData(1, 0, 5);
            employees.employeeLevelDatas.Add(levelData);
            employees.changed = true;
            animal.employeeLevel = AssetLoader.employees_levels[0];
            levelData.speed = animal.employeeLevel.move_speed;
            levelData.max_weight = animal.employeeLevel.max_weight;
            animal.employeeLevelData = levelData;

            SoundManager.Instance.PlayAudio(GameIns.gameSoundManager.Quack(), 0.2f);
           
            Vector3 screenPos;
#if UNITY_ANDROID || UNITY_IOS
                screenPos = Touchscreen.current.touches[0].position.ReadValue();
#else
            screenPos = Mouse.current.position.ReadValue();
#endif
            animal.trans.position = Camera.main.ScreenToWorldPoint(screenPos);
            GameInstance.GameIns.applianceUIManager.UnlockHire(true);

            animal.StartFalling(true);
            SaveLoadSystem.SaveEmployees(employees);
        }
    }


    public void UseFish()
    {
        restaurantCurrency.fishes--;
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
            Vector3 test = new Vector3(GameInstance.GameIns.inputManager.cameraRange.position.x, 0, GameInstance.GameIns.inputManager.cameraRange.position.z);
            Vector3 n = new Vector3(x, 0, z).normalized;
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
            Vector3 test = new Vector3(GameInstance.GameIns.inputManager.cameraRange.position.x, 0, GameInstance.GameIns.inputManager.cameraRange.position.z);
            Vector3 n = new Vector3(x, 0, z).normalized;
            Vector3 ta = test + n * length;

            if (Physics.Raycast(ta + Vector3.up * 5, Vector3.down, 5))
            {
                Debug.DrawLine(ta + Vector3.up * 10, test + Vector3.down * 100f, Color.red, 100);

                if (Physics.CheckBox(ta, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, 1 << 6 | 1 << 7 | 1 << 8))
                {

                }
                else break;
            }
            yield return null;
        }


        Vector3 endPoint = new Vector3(GameInstance.GameIns.inputManager.cameraRange.position.x, 0, GameInstance.GameIns.inputManager.cameraRange.position.z);
        Vector3 dir2 = new Vector3(x, 0, z).normalized;

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

                GameInstance.GameIns.applianceUIManager.UnlockHire(true);
            }
            else
            {
                animalController.employeeLevel = employeeLevel;
                //GameInstance.GameIns.animalManager.UpdateEmployeeUpgrade(animalController);
            }

        }
    }

    public void UpgradeFoodMachine(FoodMachine foodMachine, FurnitureInfo info)
    {
        BigInteger price = Utility.StringToBigInteger(foodMachine.machineLevelData.calculatedPrice);
        if (restaurantCurrency.Money >= price)
        {
            bool exists = SoundManager.Instance.PlayAudioWithKey(GameIns.uISoundManager.Money(), 0.2f, moneyChangedSoundKey);
       
            GetMoney((-price).ToSafeString()); 
            MachineType type = foodMachine.machineType;
        
            
            foodMachine.machineLevelData.level++;
            machineLevelDataChanged = true;

            int level = foodMachine.machineLevelData.level;
            int id = foodMachine.machineLevelData.id;
            int index = id + (level >= 41 ? 41 : level >= 31 ? 31 : 1);

            MachineLevelOffset offset = AssetLoader.machineLevelOffsets[index];

            BigInteger upgradePrice = foodMachine.machineLevelData.Price_Value + Mathf.FloorToInt(Mathf.Pow((level - 1), offset.price_pow) * offset.price_mul);
            moneyString = Utility.GetFormattedMoney(upgradePrice, moneyString);
            foodMachine.machineLevelData.calculatedPrice = moneyString.ToString();
            BigInteger salePrice = Utility.StringToBigInteger(foodMachine.machineLevelData.sale_proceed);
            if (offset.sale_div == 0) salePrice += Mathf.FloorToInt(Mathf.Pow((level - 1), offset.sale_pow) * offset.sale_mul);
            else salePrice += Mathf.FloorToInt(Mathf.Pow((level - 1), offset.sale_pow) * offset.sale_mul) / Mathf.FloorToInt((level - 1) * 0.07f);
            foodMachine.machineLevelData.calculatedSales = salePrice;

            foodMachine.machineLevelData.calculatedCookingTimer = foodMachine.machineLevelData.cooking_time - (level - 1) * offset.reduce_timer;
            if (foodMachine.machineLevelData.calculatedCookingTimer < 3f) foodMachine.machineLevelData.calculatedCookingTimer = 3f;
            foodMachine.machineLevelData.calculatedHeight = foodMachine.machineLevelData.max_height + Mathf.FloorToInt((level - 1) * offset.increase_height);

            info.UpdateInfo(foodMachine);

            GameIns.applianceUIManager.UnlockHire(true);
        }
    }

    public void AddFuel(FoodMachine foodMachine, int amount)
    {
        SoundManager.Instance.PlayAudioWithKey(GameIns.uISoundManager.Fishes(), 0.2f, fishChangedSoundKey);
        int before = restaurantCurrency.fishes;
        restaurantCurrency.fishes += -amount;
        GetFish(before, -amount);
        restaurantCurrency.changed = true;
        foodMachine.machineLevelData.fishes += amount;
        foodMachine.fuelGage.UpdateGage(foodMachine, amount, false);
        machineLevelDataChanged = true; 
    }

    public float GetRestaurantValue()
    {
        WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;
        float val = 0;
        
        val += restaurantData.extension_level * 600;
        for (int i = 0; i < restaurantparams.Count; i++)
        {
     
            val += 100;
            
        }
        for (int i=0; i < workSpaceManager.foodMachines.Count; i++)
        {
            MachineType machineType = workSpaceManager.foodMachines[i].machineType;
            int l = workSpaceManager.foodMachines[i].machineLevelData.level;
           
            val += 20 * (l - 1);
        }

        for(int i=0; i< GameInstance.GameIns.animalManager.employeeControllers.Count; i++)
        {
            Employee newAnimalController = GameInstance.GameIns.animalManager.employeeControllers[i];
          
            val += 50 * (newAnimalController.employeeLevelData.level - 1); 
        }
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
        SaveLoadSystem.SaveMiniGameStatus(miniGame);
        if(trashData != null) SaveLoadSystem.SaveTrashData(trashData);
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
            SaveLoadSystem.SaveMiniGameStatus(miniGame);
            if (trashData != null) SaveLoadSystem.SaveTrashData(trashData);
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

    public void GetFish(int before, int addFish, bool animate = true)
    {
        if (animate)
        {
            if(changingFishCoroutine != null) StopCoroutine(changingFishCoroutine);
            changingFishCoroutine = StartCoroutine(ChangingFishes(before, addFish));
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
      
        SoundManager.Instance.AudioStop(fishChangedSoundKey);
    }

    public void GetMoney(string addMoney, bool animate = true)
    {
        BigInteger bigInteger = BigInteger.Parse(addMoney);
        BigInteger before = restaurantCurrency.Money;
        restaurantCurrency.Money += bigInteger;
      //  Debug.Log(restaurantCurrency.Money.ToString());
        if(animate)
        {
            if (changingMoneyCoroutine != null) StopCoroutine(changingMoneyCoroutine);
            changingMoneyCoroutine = StartCoroutine(ChangingMoney(before, bigInteger));
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
                BigInteger test = (before) + (changed * (BigInteger)(f * 2 * 1000)) / 1000;
                moneyString = Utility.GetFormattedMoney(test, moneyString);
                GameIns.uiManager.moneyText.text = moneyString.ToString();
                f += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    
        moneyString = Utility.GetFormattedMoney(restaurantCurrency.Money, moneyString);
        GameIns.uiManager.moneyText.text = moneyString.ToString();
        SoundManager.Instance.AudioStop(moneyChangedSoundKey);
    }

    public Emote GetEmote()
    {
        Emote e = null;
        if(emotes.Count > 0)
        {
            e = emotes.Dequeue();
            e.gameObject.SetActive(true);
        }
        else
        {
            e = Instantiate(emote, emoteObjects.transform);
        }

        return e;
    }

    public void ReturnEmote(Emote e)
    {
        if (emotes.Count > 30)
        {
            Destroy(e.gameObject);
        }
        else
        {
            e.gameObject.SetActive(false);
            emotes.Enqueue(e);
        }
    }

    public FloatingCost GetFloatingCost()
    {
        FloatingCost fc = null;
        if (floatingCosts.Count > 0)
        {
            fc = floatingCosts.Dequeue();   
            fc.gameObject.SetActive(true);
        }
        else
        {
            fc = Instantiate(floatingCost, emoteObjects.transform);
        }
        return fc;
    }

    public void ReturnFloatingCost(FloatingCost fc)
    {
        if(floatingCosts.Count > 10)
        {
            Destroy(fc.gameObject);
        }
        else
        {
            fc.gameObject.SetActive(false);
            floatingCosts.Enqueue(fc);
        }
    }

    public void CalculateSpawnTimer()
    {
        if(restaurantCurrency.reputation == 0)
        {
            spawnTimer = 20000 + (int)(20000 * (spawnerNum - 1) * 0.5f);
        }
        else
        {
            float f = restaurantCurrency.reputation / 100f;

            int timer = (int)Mathf.Lerp(20000, 4000, f);
            timer = timer + (int)(timer * (spawnerNum - 1) * 0.5f);
            spawnTimer = timer;
        }

    }

    public void OpenMiniGame(MiniGameType r)
    {
        switch(r)
        {
            case MiniGameType.Fishing:
                miniGame.type = MiniGameType.Fishing;
                miniGame.changed = true;
                miniGame.activate = true;
                if (miniGame.fishing == null)
                {
                    miniGame.fishing = new Fishing(0, false);
                }

                GameIns.fishingManager.ResetFishing();
          
                if (GameIns.app.currentScene == SceneState.Restaurant) GameIns.uiManager.fishingBtn.gameObject.SetActive(true);
                break;
        }
    }

    public GameObject GetFishIcon()
    {
        GameObject f;
        if (fishRewardIconQueue.Count > 0)
        {
            f = fishRewardIconQueue.Dequeue();
            f.SetActive(true);
        }
        else
        {

            f = Instantiate(fishIcon, fishIconCanvas.transform);
        }
        return f;
    }

    public void RemoveFishIcon(GameObject icon)
    {
        icon.SetActive(false);
        fishRewardIconQueue.Enqueue(icon);
    }
}
