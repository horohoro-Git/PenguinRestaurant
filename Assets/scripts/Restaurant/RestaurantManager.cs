using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static GameInstance;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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
    public RestaurantOthers restaurantOthers;
    public RestaurantData restaurantData;
    public Employees employees;
    public RectTransform machineStatusCanvas;

    public List<MachineStatus> machineStatuses = new(); 

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

    public Transform start;
    public Transform end;

    //곡선 크기
    [Range(1f, 100f)]
    public float duration;
   // [Range(1f, 100f)]
    public float Height { get { if (InputManager.cachingCamera.orthographic) return InputManager.cachingCamera.orthographicSize * 2; else return 30; } }
    [Range(1f, 4f)]
    public float weight;
    [Range(1f, 4f)]
    public float weight2;


    public Tutorials tutorials;
    public Dictionary<int, List<TutorialStruct>> tutorialStructs = new();
    public static HashSet<int> tutorialKeys = new HashSet<int>();                                   //튜토리얼 이벤트 키
    public static HashSet<TutorialEventKey> tutorialEventKeys = new HashSet<TutorialEventKey>();    //튜토리얼 기능 제한 키
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

        string s = Resources.Load<TextAsset>("Tutorial").text;
        List<TutorialStruct> tutoStructs = JsonConvert.DeserializeObject<List<TutorialStruct>>(s);

        for (int i = 0; i < tutoStructs.Count; i++)
        {
            if (tutorialStructs.ContainsKey(tutoStructs[i].id))
            {
                tutorialStructs[tutoStructs[i].id].Add(tutoStructs[i]);
            }
            else
            {
                tutorialStructs[tutoStructs[i].id] = new()
                {
                    tutoStructs[i]
                };
            }
        }
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

        for (int i = 0; i < emoteSpriteKeys.Count; i++)
        {
            emoteSprites[emoteSpriteKeys[i]] = AssetLoader.loadedAtlases["UI"].GetSprite(AssetLoader.spriteAssetKeys[emoteSpriteKeys[i]].ID);
        }
        for (int i = 0; i < 30; i++)
        {
            Emote e = Instantiate(emote, emoteObjects.transform);
            e.gameObject.SetActive(false);
            emotes.Enqueue(e);
        }
        for (int i = 0; i < 10; i++)
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
        restaurantOthers = SaveLoadSystem.LoadRestaurantOthers();
        restaurantparams = SaveLoadSystem.LoadRestaurantBuildingData();
        vendingData = SaveLoadSystem.LoadVendingMachineData();
        employees = SaveLoadSystem.LoadEmployees();
        miniGame = SaveLoadSystem.LoadMiniGameStatus();
        trashData = SaveLoadSystem.LoadTrashData();
        tutorials = SaveLoadSystem.LoadTutorialData();
      
        Tutorials.TutorialUpdate(0);
       
        // restaurantCurrency.fishes += 100;
        restaurantCurrency.Money += BigInteger.Parse("10000000000");// 10000;
        restaurantCurrency.fishes += 100;
        moneyString = Utility.GetFormattedMoney(restaurantCurrency.Money, moneyString);
        GameInstance.GameIns.uiManager.moneyText.text = moneyString.ToString();
        GameInstance.GameIns.uiManager.fishText.text = restaurantCurrency.fishes.ToString();

        customerDebug = false;


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

        //조리기구 연료정보
        for (int i = 0; i < machineStatuses.Count; i++)
        {
            FoodMachine machine = machineStatuses[i].foodMachine;
            if (machine != null)
            {
                if (machine.gameObject.activeSelf)
                {
                    Vector3 pos = new Vector3(machineStatuses[i].foodMachine.transform.position.x - 6f, 10f, machineStatuses[i].foodMachine.transform.position.z - 6f);
                    machineStatuses[i].rectTransform.position = pos;
                    if(!machineStatuses[i].gameObject.activeSelf) machineStatuses[i].gameObject.SetActive(true);
                }
                else
                {
                    if (machineStatuses[i].gameObject.activeSelf) machineStatuses[i].gameObject.SetActive(false);
                }
            }
        }
    }

    async UniTask AutoSave(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            await UniTask.Delay(30000, DelayType.UnscaledDeltaTime, cancellationToken: cancellationToken);

            // await Utility.CustomUniTaskDelay(30f, cancellationToken);

            if (employees.changed)
            {
                employees.changed = false;
                SaveLoadSystem.SaveEmployees(employees);
            }
            if (restaurantCurrency.changed)
            {
                restaurantCurrency.changed = false;
                SaveLoadSystem.SaveRestaurantCurrency(restaurantCurrency);
            }
            if (restaurantData.changed)
            {
                restaurantData.changed = false;
                SaveLoadSystem.SaveRestaurantData(restaurantData);
            }
            if (machineLevelDataChanged)
            {
                machineLevelDataChanged = false;
                SaveLoadSystem.SaveFoodMachineStats(machineLevelData);
            }
            if (vendingData.changed)
            {
                vendingData.changed = false;
                SaveLoadSystem.SaveVendingMachineData(vendingData);
            }
            if (miniGame.changed)
            {
                miniGame.changed = false;
                SaveLoadSystem.SaveMiniGameStatus(miniGame);
            }
            if (trashData.changed)
            {
                trashData.changed = false;
                SaveLoadSystem.SaveTrashData(trashData);
            }
            if(restaurantOthers.changed)
            {
                restaurantOthers.changed = false;
                SaveLoadSystem.SaveRestaurantOthers(restaurantOthers);
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
              /*  float angle = 30f;
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
                }*/


                int[] angleRange = { 0, 15, -15, 30, -30, 45, -45, 60, -60, 75, -75, 90, -90, 120, -120, 150, -150, 180 };
                for (int i = 0; i < angleRange.Length; i++)
                {
                    Vector3 dir = Quaternion.AngleAxis(angleRange[i], doorTransform.up) * doorTransform.forward;
                    if (Physics.Raycast(door.transform.position + Vector3.up, dir, out RaycastHit hitWall, float.MaxValue, 1 << 16 | 1 << 19))
                    {
                        GameObject h = hitWall.collider.gameObject;

                        door.transform.position = h.transform.position - Vector3.up * h.transform.position.y;
                        door.transform.rotation = h.transform.rotation * Quaternion.Euler(0, -90, 0);

                        GameInstance.GameIns.restaurantManager.doorPreview.transform.position = door.transform.position;
                        GameInstance.GameIns.restaurantManager.doorPreview.rotateOffset.transform.rotation = door.transform.rotation * Quaternion.Euler(0, 90, 0);
                        if (GameInstance.GameIns.restaurantManager.doorPreview.CheckDoorPlacement())
                        {
                            door.transform.SetParent(h.transform.parent);
                            door.removeWall = h;
                            MoveCalculator.CheckAreaWithBounds(GameInstance.GameIns.calculatorScale, h.GetComponentInChildren<Collider>(), false);
                            h.SetActive(false);
                        }
                        break;
                    }
                }
            }
        }
        MeshRenderer meshRenderer = door.GetComponentInChildren<MeshRenderer>();
        Material[] materials = meshRenderer.materials;
        for (int i = 0; i < door.doorMat.Count; i++)
        {
            materials[i] = door.doorMat[i];
        }
        meshRenderer.materials = materials;
        MoveCalculator.CheckArea(GameInstance.GameIns.calculatorScale, true);
        GameInstance.GameIns.restaurantManager.ApplyPlaced(door, null, false);

        SaveLoadSystem.SaveRestaurantBuildingData();
        restaurantparams = SaveLoadSystem.LoadRestaurantBuildingData();
        int testX = -999;
        int testY = -999;
        Utility.CheckHirable(GameIns.inputManager.cameraRange.position, ref testX, ref testY, false, true);
    }

    public void ApplyPlaced(Furniture furniture, StoreGoods goods, bool purchased)
    {
        StartCoroutine(ApplyPlacedNextFrame(furniture, goods, purchased));
    }

    public void ApplyLoadedFoodMachineStat()
    {
        foreach (var v in machineLevelData)
        {
       //     Debug.Log(111);
            int level = v.Value.level;
            int id = v.Value.id;
            int index = id + (level >= 41 ? 41 : level >= 31 ? 31 : 1);

            MachineLevelOffset offset = AssetLoader.machineLevelOffsets[index];

            BigInteger upgradePrice = v.Value.Price_Value + Mathf.FloorToInt(Mathf.Pow((level - 1), offset.price_pow) * offset.price_mul);
            moneyString = Utility.GetFormattedMoney(upgradePrice, moneyString);
            v.Value.calculatedPrice = moneyString.ToString();
            BigInteger salePrice = Utility.StringToBigInteger(v.Value.sale_proceed);
            if (offset.sale_div == 0) salePrice += Mathf.FloorToInt(Mathf.Pow((level - 1), offset.sale_pow) * offset.sale_mul);
            else salePrice += Mathf.FloorToInt(Mathf.Pow((level - 1), offset.sale_pow) * offset.sale_mul) / Mathf.FloorToInt((level - 1) * 0.07f);
            v.Value.calculatedSales = salePrice;

            v.Value.calculatedCookingTimer = v.Value.cooking_time - (level - 1) * offset.reduce_timer;
            if (v.Value.calculatedCookingTimer < 3f) v.Value.calculatedCookingTimer = 3f;
            v.Value.calculatedHeight = v.Value.max_height + Mathf.FloorToInt((level - 1) * offset.increase_height);

        }
    }
    IEnumerator ApplyPlacedNextFrame(Furniture furniture, StoreGoods goods, bool purchased)
    {
        yield return null;
        if (!purchased && goods != null)
        {
            if (tutorialKeys.Contains(goods.goods.ID))
            {
                ((Action<TutorialEventKey>)EventManager.Publish((TutorialEventKey)goods.goods.ID))?.Invoke((TutorialEventKey)goods.goods.ID);
                if (tutorialStructs[tutorials.id][0].event_start)
                {
                    GameIns.uiManager.TutorialStart(tutorials.id, tutorials.count, tutorialStructs[tutorials.id].Count);
                }
                else
                {
                    Tutorials.TutorialUnlock(tutorialStructs[tutorials.id][0]);
                    GameIns.uiManager.TutorialEnd(true);
                }
                tutorialKeys.Remove(goods.goods.ID);
            }
            goods.Purchase();
#if UNITY_ANDROID || UNITY_IOS
                if(App.gameSettings.hapticFeedback) Handheld.Vibrate();
#endif
        }

        SaveLoadSystem.SaveRestaurantBuildingData();
        yield return null;
        if (furniture.SpaceType == WorkSpaceType.Table)
        {
            for (int i = 0; i < GameIns.workSpaceManager.tables.Count; i++)
            {
                Table t = GameIns.workSpaceManager.tables[i];
                MoveCalculator.CheckAreaWithBounds(GameInstance.GameIns.calculatorScale, t.GetComponentInChildren<Collider>(), true);

            }
            yield return null;
            List<Table> table = new List<Table>();
            table = GameIns.workSpaceManager.tables;
            TableUpdate(table);
        }
        else if (furniture.SpaceType == WorkSpaceType.Door)
        {
            MoveCalculator.CheckArea(GameInstance.GameIns.calculatorScale, true);
        }
        else
        {
            MoveCalculator.CheckAreaWithBounds(GameInstance.GameIns.calculatorScale, furniture.GetComponentInChildren<Collider>(), true);
        }

        yield return null;

        int[] collisionDirectionX = { 0, 1, 1, 0, -1, -1, -1, 0, 1 };
        int[] collisionDirectionY = { 0, 0, -1, -1, -1, 0, 1, 1, 1 };
        //  MoveCalculator.CheckArea(GameInstance.GameIns.calculatorScale);
        for (int i = 0; i < GameInstance.GameIns.animalManager.employeeControllers.Count; i++)
        {
            Employee e = GameInstance.GameIns.animalManager.employeeControllers[i];
            int collides = 0;
            for (int j = 0; j < 9; j++)
            {
                int posX = Mathf.FloorToInt((e.trans.position.x + collisionDirectionX[j] * 0.4f - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
                int posZ = Mathf.FloorToInt((e.trans.position.z + collisionDirectionY[j] * 0.4f - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);
                if (!e.falling && Utility.ValidCheck(posZ, posX) && MoveCalculator.GetBlocks[MoveCalculator.GetIndex(posX, posZ)])
                {
                    collides++;
                    AnimalCollision(e, posX, posZ, App.GlobalToken).Forget();
                    break;
                }
            }

            if(collides == 0) e.reCalculate = true;
            //     e.reCalculate = true;
        }

     

        for (int i = 0; i < GameInstance.GameIns.animalManager.customerControllers.Count; i++)
        {
            Customer c = GameIns.animalManager.customerControllers[i];
            int collides = 0;
            if (c.trans)
            {
                for(int j = 0; j < 9; j++)
                {
                    int posX = Mathf.FloorToInt((c.trans.position.x + collisionDirectionX[j] * 0.4f - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
                    int posZ = Mathf.FloorToInt((c.trans.position.z + collisionDirectionY[j] * 0.4f - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);
                    if (Utility.ValidCheck(posZ, posX) && MoveCalculator.GetBlocks[MoveCalculator.GetIndex(posX, posZ)])
                    {
                        collides++;
                        AnimalCollision(c, posX, posZ, App.GlobalToken).Forget();
                        break;
                    }
                }

            }
            if(collides == 0) c.reCalculate = true;
        }


        BlackConsumer b = GameIns.animalManager.blackConsumer;
        if (b != null)
        {
            int collides = 0;
            for (int j = 0; j < 9; j++)
            {
                int posX = Mathf.FloorToInt((b.trans.position.x + collisionDirectionX[j] * 0.4f - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
                int posZ = Mathf.FloorToInt((b.trans.position.z + collisionDirectionY[j] * 0.4f - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);
                if (Utility.ValidCheck(posZ, posX) && MoveCalculator.GetBlocks[MoveCalculator.GetIndex(posX, posZ)])
                {
                    collides++;
                    AnimalCollision(b, posX, posZ, App.GlobalToken).Forget();
                    break;
                }
            }
            if(collides == 0) b.reCalculate = true;
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
                    if (!Utility.ValidCheckWithCharacterSize(finalX, finalZ, MoveCalculator.moveX, MoveCalculator.moveY))
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
        if (animal.animal != null) animal.animal.PlayAnimation(AnimationKeys.Idle);// (animal.animal.animationDic["Idle_A"], "Idle_A");
        while (restaurantTimer < t)
        {
            float progress = restaurantTimer / t; // 0 ~ 1
            animal.trans.position = Vector3.Lerp(st, targetPos, progress);

            await UniTask.NextFrame(cancellationToken);
        }
        animal.trans.position = targetPos;

        animal.bWait = false;
        animal.reCalculate = true;
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
        //    Debug.Log(restaurantparams.Count + " num");
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
                switch (furniture.SpaceType)
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
                        fm.transform.rotation = rot;
                        break;
                    case WorkSpaceType.Table:
                        furniture.transforms.rotation = rot;
                        tables.Add(furniture.GetComponent<Table>());
                        break;
                    default:
                        furniture.transforms.rotation = rot;
                        break;
                }

                placeController.transform.position = furniture.transform.position;
                placeController.offset.transform.rotation = Quaternion.Euler(0, placeController.rotates[restaurantparams[i].level], 0);
                placeController.offset.transform.localPosition = placeController.rotateOffsets[restaurantparams[i].level];
                GameInstance.GameIns.gridManager.CheckObject(placeController, placeController.storeGoods.goods.type);
                GameInstance.GameIns.gridManager.ApplyGird(placeController, placeController.offset.transform.position, furniture.SpaceType);

                GameIns.store.require.Add(restaurantparams[i].id);
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }

            await UniTask.NextFrame(cancellationToken: cancellationToken);
            MoveCalculator.CheckArea(GameIns.calculatorScale, true);


            await UniTask.NextFrame(cancellationToken: cancellationToken);

            TableUpdate(tables);


            GameIns.store.StoreUpdate();

            Tutorials.TutorialUpdate(tutorials.id);

            if (!tutorialEventKeys.Contains(TutorialEventKey.NoFishing))
            {
                if (miniGame.activate)
                {
                    switch (miniGame.type)
                    {
                        case MiniGameType.None:
                            break;
                        case MiniGameType.Fishing:
                            GameIns.fishingManager.LoadStatus(miniGame.fishing);
                            GameIns.uiManager.fishingBtn.gameObject.SetActive(true);
                            //OpenMiniGame(MiniGameType.Fishing);
                            break;
                    }
                }
            }
            GameIns.uiManager.reputation.text = restaurantOthers.reputation.ToString();
            int sum = 0;
            foreach(var v in AnimalManager.gatchaTiers)
            {
                sum += v.Value.Item1;
            }
            GameIns.uiManager.targetText.text = $"{sum} / {AssetLoader.rules[GameIns.assetLoader.gameRegulation.id].target_num}";
            GameIns.applianceUIManager.rewardChest_Fill.uiImage.fillAmount = trashData.trashPoint * 0.01f;
            if (trashData.trashPoint == 100)
            {
                GameIns.applianceUIManager.rewardChest_Fill.ChangeHighlight(true);
            }

            CalculateSpawnTimer();
            await LoadEmployees(cancellationToken);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    async UniTask LoadEmployees(CancellationToken cancellationToken = default)
    {
        try
        {
            await UniTask.NextFrame(cancellationToken: cancellationToken);
            int num = employees.num;
            int x = 0, y = 0;
            Utility.CheckHirable(Vector3.zero, ref x, ref y, false, true, true);

            //     Debug.Log(InputManger.spawnDetects.Count);


            for (int i = 0; i < num; i++)
            {
                int r = Random.Range(0, InputManager.spawnDetects.Count);

                Vector3 t = InputManager.spawnDetects[r];

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

            InputManager.spawnDetects = new List<Vector3>();
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
            // GameInstance.GameIns.applianceUIManager.UnlockHire(true);

           
            /*
            Tutorials tutorial = GameInstance.GameIns.restaurantManager.tutorials;

            Dictionary<int, List<TutorialStruct>> tutorialStructs = GameInstance.GameIns.restaurantManager.tutorialStructs;

            List<TutorialStruct> lastTutorialStruct = new();
            foreach (var v in tutorialStructs) lastTutorialStruct = v.Value;

            if (tutorial.id <= lastTutorialStruct[0].id)
            {
                tutorial.id = lastTutorialStruct[0].id + 1;
             //   tutorial.id = 16;
                tutorial.worked = true;
                int checkTier = 0;
                foreach (var v in AnimalManager.gatchaTiers)
                {
                    (int, List<int>) f = v.Value;
                    checkTier += f.Item1;
                }

                if (checkTier == 0)
                {
                    //못 받은 고양이 손님 추가
                    GameInstance.GameIns.gatcharManager.CheckSuccess(100, 0);
                }

                SaveLoadSystem.SaveTutorialData(tutorial);
                Tutorials.TutorialUpdate(tutorial.id);
                GameInstance.GameIns.uiManager.TutorialEnd(true);
            }
            */

        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
    public void Tutorial()
    {

        if (!tutorialStructs.ContainsKey(tutorials.id))
        {
            Tutorials.TutorialUpdate(tutorials.id);
            return;
        }
        if (tutorials.worked)
        {
            if (1 == tutorialStructs[tutorials.id].Count)
            {
                tutorials.worked = false;
            }
            else
            {
                tutorials.worked = true;
            }

            TutorialStruct tuto = tutorialStructs[tutorials.id][0];
            if (tuto.event_id == TutorialEventKey.EnterRestaurant)
            {

                switch (tuto.event_type)
                {

                    case TutorialType.SpawnEnemy:
                        EnemySpawner enemySpawner = GameInstance.GameIns.restaurantManager.door.GetComponentInChildren<EnemySpawner>();
                        GameObject target = enemySpawner.SpawnEnemyTutorial();
                        Tutorials.EnemyFollowing(target, App.GlobalToken).Forget();
                        break;
                }
                tutorials.id++;
                if (1 == tutorialStructs[tutorials.id].Count) tutorials.worked = false;
                else tutorials.worked = true;

            }
            if (tuto.event_id == TutorialEventKey.EnterRestaurant || tuto.event_id == TutorialEventKey.HireEmployee || tuto.event_id == TutorialEventKey.FillFishes || tuto.event_id == TutorialEventKey.EnterFishing || tuto.event_id == TutorialEventKey.StartFishing || tuto.event_id == TutorialEventKey.ComploeteFishing || tuto.event_id == TutorialEventKey.CatchEnemy || tuto.event_id == TutorialEventKey.BuyTrashcan || tuto.event_id == TutorialEventKey.Cleaning)
            {
                AnimalSpawner[] spawners = AnimalSpawner.FindObjectsOfType<AnimalSpawner>();

                foreach (var v in spawners)
                {
                    if (v.type == AnimalSpawner.SpawnerType.FastFood)
                    {
                        v.TutorialSpawnAnimal(App.GlobalToken).Forget();
                    }
                }
            }

            if (tuto.event_id == TutorialEventKey.TrashcanMinigame)
            {
                trashData.trashPoint = 100;
                GameIns.applianceUIManager.rewardChest_Fill.ChangeHighlight(true);
                GameIns.applianceUIManager.rewardChest_Fill.uiImage.fillAmount = trashData.trashPoint * 0.01f;
                trashData.changed = true;
            }

            Tutorials.Setup(tutorials);

            if (tuto.event_id != TutorialEventKey.TutorialComplete)
            {
                if (tuto.event_start) GameIns.uiManager.TutorialStart(tutorials.id, tutorials.count, tutorialStructs[tutorials.id].Count);
                else if (tuto.event_type == TutorialType.SpawnEnemy) GameIns.uiManager.TutorialStart(tutorials.id, tutorials.count, tutorialStructs[tutorials.id].Count);
            }
            else
            {
                Tutorials.Unlock(tutorials.id);
            }
        }

    }
    /*   async UniTask TutorialAsync(CancellationToken cancellationToken)
       {  
          // await UniTask.Delay(5000, cancellationToken: cancellationToken);
           if (1 == tutorialStructs[tutorials.id].Count)
           {
               tutorials.worked = false;
           }
           else
           {
               tutorials.worked = true;
           }

           TutorialStruct tuto = tutorialStructs[tutorials.id][0];
           if(tuto.event_id == TutorialEventKey.EnterRestaurant)
           {

               switch (tuto.event_type)
               {

                   case TutorialType.SpawnEnemy:
                       EnemySpawner enemySpawner = GameInstance.GameIns.restaurantManager.door.GetComponentInChildren<EnemySpawner>();
                       GameObject target = enemySpawner.SpawnEnemyTutorial();
                       Tutorials.EnemyFollowing(target, App.GlobalToken).Forget();
                       break;
               }
               tutorials.id++;
               if (1 == tutorialStructs[tutorials.id].Count) tutorials.worked = false;
               else tutorials.worked = true;

           }

           if(tuto.event_id == TutorialEventKey.EnterFishing || tuto.event_id == TutorialEventKey.StartFishing || tuto.event_id == TutorialEventKey.ComploeteFishing)
           {
                GameIns.uiManager.fishingBtn.gameObject.SetActive(true);
           }

           if(tuto.event_id == TutorialEventKey.EnterRestaurant || tuto.event_id == TutorialEventKey.FillFishes || tuto.event_id == TutorialEventKey.EnterFishing || tuto.event_id == TutorialEventKey.StartFishing || tuto.event_id == TutorialEventKey.ComploeteFishing || tuto.event_id == TutorialEventKey.CatchEnemy || tuto.event_id == TutorialEventKey.BuyTrashcan || tuto.event_id == TutorialEventKey.Cleaning)
           {
               AnimalSpawner[] spawners = AnimalSpawner.FindObjectsOfType<AnimalSpawner>();

               foreach (var v in spawners)
               {
                   if (v.type == AnimalSpawner.SpawnerType.FastFood)
                   {
                       v.TutorialSpawnAnimal(App.GlobalToken).Forget();
                   }
               }
           }

           if(tuto.event_id == TutorialEventKey.TrashcanMinigame)
           {
               trashData.trashPoint = 100;
               GameIns.applianceUIManager.rewardChest_Fill.ChangeHighlight(true);
               GameIns.applianceUIManager.rewardChest_Fill.uiImage.fillAmount = trashData.trashPoint * 0.01f;
               trashData.changed = true;
           }

           Tutorials.Setup(tutorials);

           if (tuto.event_id != TutorialEventKey.TutorialComplete)
           {
               if (tuto.event_start) GameIns.uiManager.TutorialStart(tutorials.id, tutorials.count, tutorialStructs[tutorials.id].Count);
               else if(tuto.event_type == TutorialType.SpawnEnemy) GameIns.uiManager.TutorialStart(tutorials.id, tutorials.count, tutorialStructs[tutorials.id].Count);
           }
           else
           {
               Tutorials.Unlock(tutorials.id);
           }


           ///   if (tutorialStructs[tutorials.id][0].event_start) GameIns.uiManager.TutorialStart(tutorials.id, tutorials.count, tutorialStructs[tutorials.id].Count);
           //   else tutorialKeys.Add((int)tutorialStructs[tutorials.id][0].event_id);

       }
   */
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

    public void HireEmployee()
    {
        if (employees.num < 8 && employeeHire[employees.num] <= GetRestaurantValue() && GameIns.inputManager.CheckHire())
        {
            //테스트 GameIns.inputManager.CheckHire();
            employees.num++;
            EmployeeNum();
        }
    }

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
        if (tutorialKeys.Contains((int)TutorialEventKey.TrashcanMinigame)) GameIns.uiManager.TutorialEnd(true);
    }


    public void EmployeeNum()
    {
        //테스트   for (int i = 0; i < 8; i++)
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
            screenPos = GameIns.applianceUIManager.hireBtn.GetComponent<RectTransform>().position;
            animal.trans.position = Camera.main.ScreenToWorldPoint(screenPos);
            GameInstance.GameIns.applianceUIManager.UnlockHire(true);

            animal.StartFalling(true);
            if (tutorialKeys.Contains((int)TutorialEventKey.HireEmployee))
            {
                tutorialKeys.Remove((int)TutorialEventKey.HireEmployee);
                ((Action<TutorialEventKey>)EventManager.Publish(TutorialEventKey.HireEmployee))?.Invoke(TutorialEventKey.HireEmployee);
                GameIns.uiManager.TutorialEnd(true);
            }


            SaveLoadSystem.SaveEmployees(employees);
        }
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
            else salePrice += Mathf.FloorToInt((Mathf.Pow((level - 1), offset.sale_pow) * offset.sale_mul) / ((level - 1) * 0.07f));
            foodMachine.machineLevelData.calculatedSales = salePrice;

            foodMachine.machineLevelData.calculatedCookingTimer = foodMachine.machineLevelData.cooking_time - (level - 1) * offset.reduce_timer;
            if (foodMachine.machineLevelData.calculatedCookingTimer < 3f) foodMachine.machineLevelData.calculatedCookingTimer = 3f;
            int height = foodMachine.machineLevelData.max_height + Mathf.FloorToInt((level - 1) * offset.increase_height);
            if(height > 10) height = 10;
            foodMachine.machineLevelData.calculatedHeight = height;

            info.UpdateInfo(foodMachine);

            GameIns.applianceUIManager.UnlockHire(true);


           // Debug.Log("Money : " + restaurantCurrency.Money + " Price : " + foodMachine.machineLevelData.calculatedPrice + " p : " + Utility.StringToBigInteger(foodMachine.machineLevelData.calculatedPrice));
        }
    }

    public void AddFuel(FoodMachine foodMachine, int amount)
    {
        SoundManager.Instance.PlayAudioWithKey(GameIns.uISoundManager.Fishes(), 0.2f, fishChangedSoundKey);
        int before = restaurantCurrency.fishes;
        restaurantCurrency.fishes += -amount;
        GetFish(before, -amount);
        restaurantCurrency.changed = true;
        foodMachine.machineLevelData.Fishes += amount;
     //   foodMachine.fuelGage.UpdateGage(foodMachine, amount, false);
        machineLevelDataChanged = true;
    //    if (tutorialKeys.Contains(7000) && amount > 0) ((Action<int>)EventManager.Publish(-1, true))?.Invoke(7000);
    }

    public float GetRestaurantValue()
    {
        WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;
        float val = 0;
        if (restaurantData == null) Debug.Log("EE");
        val += restaurantData.extension_level * 600;
        for (int i = 0; i < restaurantparams.Count; i++)
        {
            val += 100;

        }
        for (int i = 0; i < workSpaceManager.foodMachines.Count; i++)
        {
            MachineType machineType = workSpaceManager.foodMachines[i].machineType;
            if(workSpaceManager.foodMachines[i].machineLevelData == null) Debug.Log("EEE");
            int l = workSpaceManager.foodMachines[i].machineLevelData.level;
            
            val += 20 * (l - 1);
        }

        for (int i = 0; i < GameInstance.GameIns.animalManager.employeeControllers.Count; i++)
        {
            Employee newAnimalController = GameInstance.GameIns.animalManager.employeeControllers[i];
            val += 50 * (newAnimalController.employeeLevelData.level - 1);
        }
        return val;
    }

    public void GameSave()
    {
        if (restaurantCurrency.changed) SaveLoadSystem.SaveRestaurantCurrency(restaurantCurrency);
        if (employees.changed) SaveLoadSystem.SaveEmployees(employees);
        if (restaurantData.changed) SaveLoadSystem.SaveRestaurantData(restaurantData);
        SaveLoadSystem.SaveFoodMachineStats(machineLevelData);
        if (vendingData.changed) SaveLoadSystem.SaveVendingMachineData(vendingData);
        if (miniGame.changed) SaveLoadSystem.SaveMiniGameStatus(miniGame);
        if (trashData != null)
        {
            if (trashData.changed) SaveLoadSystem.SaveTrashData(trashData);
        }
        if(restaurantOthers.changed) SaveLoadSystem.SaveRestaurantOthers(restaurantOthers);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            if (restaurantCurrency.changed)
            {
                restaurantCurrency.changed = false;
                SaveLoadSystem.SaveRestaurantCurrency(restaurantCurrency);
            }
            if (employees.changed)
            {
                employees.changed = false;
                SaveLoadSystem.SaveEmployees(employees);
            }
            if (restaurantData.changed)
            {
                restaurantData.changed = false;
                SaveLoadSystem.SaveRestaurantData(restaurantData);
            }
            SaveLoadSystem.SaveFoodMachineStats(machineLevelData);
            if (vendingData.changed)
            {
                vendingData.changed = false;
                SaveLoadSystem.SaveVendingMachineData(vendingData);
            }
            if (miniGame.changed)
            {
                miniGame.changed = false;
                SaveLoadSystem.SaveMiniGameStatus(miniGame);
            }
            if (trashData != null)
            {
                if (trashData.changed)
                {
                    trashData.changed = false;
                    SaveLoadSystem.SaveTrashData(trashData);
                }
            }

            if(restaurantOthers.changed)
            {
                restaurantOthers.changed = false;
                SaveLoadSystem.SaveRestaurantOthers(restaurantOthers);
            }
        }
    }

    public void NewTrays()
    {
        for (int i = 0; i < 20; i++)
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

   /* public FuelGage GetGage()
    {
        FuelGage fuel = fuelGages.Dequeue();
        return fuel;
    }*/

    public void GetFish(int before, int addFish, bool animate = true)
    {
        if (animate)
        {
            if (changingFishCoroutine != null) StopCoroutine(changingFishCoroutine);
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
        float finishTimer = Math.Abs(changed) >= 10 ? 0.5f : Math.Abs(changed) * 0.05f;
        if (changed > 0)
        {
            while (f <= finishTimer)
            {
                int test = before + (int)(changed * (f * 2 * 1000) / 1000);
                GameIns.uiManager.fishText.text = test.ToString();
                f += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        else
        {
            while (f <= finishTimer)
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
        if (animate)
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
        if (changed > 0)
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
        if (emotes.Count > 0)
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
        if (floatingCosts.Count > 10)
        {
            Destroy(fc.gameObject);
        }
        else
        {
            fc.text.text = "";
            fc.gameObject.SetActive(false);
            floatingCosts.Enqueue(fc);
        }
    }

    public void CalculateSpawnTimer()
    {
        if (restaurantOthers.reputation == 0)
        {
            spawnTimer = 20000 + (int)(20000 * (spawnerNum - 1) * 0.5f);
        }
        else
        {
            float f = restaurantOthers.reputation / 100f;

            int timer = (int)Mathf.Lerp(20000, 4000, f);
            timer = timer + (int)(timer * (spawnerNum - 1) * 0.5f);
            spawnTimer = timer;
        }

    }

    public void OpenMiniGame(MiniGameType r)
    {
        switch (r)
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

                if (App.currentScene == SceneState.Restaurant) GameIns.uiManager.fishingBtn.gameObject.SetActive(true);
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
