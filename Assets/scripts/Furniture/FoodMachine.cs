using CryingSnow.FastFoodRush;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.CustomPlugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
public class FoodMachine : Furniture
{

    public MachineType machineType;

    public Mesh foodMesh;
    public FoodStack foodStack = new FoodStack();
    //int maxNum = 10;
    public float cookingTimer = 3f;
    public Transform foodTransform;
    public Transform workingSpot;
    public AnimalController employee;
    public FoodStack[] canBringTheFoods;

    Stack<Food> hiddenStack = new Stack<Food>();

    protected AudioSource audioSource;
    public AudioSource foodCreateAudio;
    //  public PlayData playData;
    public RestaurantParam restaurantParam;
    public LevelData levelData;
    public int level=1;
    public MachineData mData;
    public MachineLevelData machineLevelData;
    public Transform transforms;
    public Transform modelTrans;
    protected bool noFishes;
    public MachineData machineData {
        get { return mData; }

        set { mData = value;
        //    level = mData.level;
        }
    }
    public static bool isQuitting = false;
    private float foodHight;

    public float trayOffset;
    [NonSerialized]
    public int getNum;

    [NonSerialized]
    public GameObject tray;

    Action<float> cookingAction;
    Action cookingFinishAction;
    public Func<UniTask> cookingFinishFunc;

    public float height;
    protected  bool canCooking = false;

    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    [NonSerialized] public FuelGage fuelGage;
    public int soundClip;

    StringBuilder sb = new StringBuilder();
    [NonSerialized] public bool bActivated;

    private void Awake()
    {
        transforms = transform;
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void OnEnable()
    {
        if (spawned && App.GlobalToken != null)
        {
            Invoke("LateStart", 0.5f);
            if (fuelGage != null)
            {
                fuelGage.gameObject.SetActive(true);
            }
           
            /*  PlaceTray();
          //    if (canCooking)
              {
                  cancellationTokenSource = new CancellationTokenSource();
                  CookingFood(cancellationTokenSource.Token).Forget();
              }
              if (GameInstance.GameIns.workSpaceManager)
              {
                  GameInstance.GameIns.workSpaceManager.foodMachines.Add(this);
              }*/
        }
       /* tray = GameInstance.GameIns.restaurantManager.GetTray();
        Vector3 target = new Vector3(transforms.position.x, transforms.position.y, transforms.position.z) + transforms.forward * 3;
        tray.transform.position = target;*/
    }

    public virtual void OnDisable()
    {
        if (isQuitting) return;
        if (!Application.isPlaying) return;
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();

        if(fuelGage != null)
        {
            fuelGage.gameObject.SetActive(false);
        }
        bActivated = false;
        if (tray != null && GameInstance.GameIns.app != null && App.GlobalToken != null)
        {
            DespawnTray(App.GlobalToken).Forget();
        }
    }
    // Start is called before the first frame update
    public override void Start()
    {
        GameInstance.GameIns.workSpaceManager.unlockFoods[(int)machineType - 1] = true;
        GameInstance.GameIns.workSpaceManager.foodMachines.Add(this);
        if (!spawned)
        {
            Invoke("LateStart", 0.5f);
        
        }
        base.Start();
        
        foodStack.type = machineType;
        
      //  Cooking();
        //for(int i=0; i<200; i++) FoodManager.NewFood(Instantiate(food));
    }

    void LateStart()
    {
        PlaceTray();
        if(!canCooking)
        {
            canCooking = true;
            Cooking();
        }
        else
        {
            cancellationTokenSource = new CancellationTokenSource();
            CookingFood(cancellationTokenSource.Token).Forget();
        }
     
    }

    public void PlaceTray()
    {
        if(tray == null) tray = GameInstance.GameIns.restaurantManager.GetTray();
        tray.SetActive(true);
        tray.transform.rotation = transforms.rotation;
        Vector3 target = new Vector3(transforms.position.x, 10, transforms.position.z) + transforms.forward * (3 + trayOffset);
        tray.transform.position = target;
        tray.transform.localScale = new Vector3(1, 1, 1);
        SpawnTray(App.GlobalToken).Forget();
    }

    async UniTask SpawnTray(CancellationToken cancellationToken = default)
    {
        try
        {
            while (hiddenStack.Count > 0)
            {
                Food food = hiddenStack.Pop();
              //  food.gameObject.SetActive(true);
                foodStack.foodStack.Push(food);
                food.transform.position = foodTransform.position + Vector3.up * (foodStack.foodStack.Count - 1) * height;
            }

            Vector3 targetLoc = new Vector3(transforms.position.x, 10, transforms.position.z) + transforms.forward * (3 + trayOffset);
            tray.transform.position = targetLoc;
            float origin = 10;
            float t = 0;
            float f = 0;
            while (f <= 0.2f)
            {
                float r = Mathf.Lerp(origin, t, f * 5);
                Vector3 v = new Vector3(targetLoc.x, r, targetLoc.z);
                tray.transform.position = v;
                f += Time.unscaledDeltaTime;
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }
            tray.transform.position = new Vector3(transforms.position.x, transforms.position.y, transforms.position.z) + transforms.forward * (3 + trayOffset);
            Vector3 target = new Vector3(2.4f,2.4f,2.4f);
            Vector3 tt = new Vector3(2f,2f,2f);

            Vector3 current = tray.transform.localScale;

            
            SoundManager.Instance.PlayAudio3D(GameInstance.GameIns.gameSoundManager.DropPlate(), 0.2f, 100, 5, tray.transform.position);

            f = 0;
            while (f <= 0.2f)
            {
                tray.transform.localScale = Vector3.Lerp(current, target, f * 5);
                f += Time.unscaledDeltaTime;
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }
            f = 0;
            Vector3 cur = tray.transform.localScale;
            while (f <= 0.1f)
            {
                tray.transform.localScale = Vector3.Lerp(cur, tt, f * 10);
                f += Time.unscaledDeltaTime;
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }
            tray.transform.localScale = tt;

            foreach(var v in foodStack.foodStack)
            {
                v.gameObject.SetActive(true);
            }

            bActivated = true;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

       
      
    }
    async UniTask DespawnTray(CancellationToken cancellationToken = default)
    {
        try
        {
            while (foodStack.foodStack.Count > 0)
            {
                Food food = foodStack.foodStack.Pop();
                food.gameObject.SetActive(false);
                hiddenStack.Push(food); 
            }
            getNum = 0;
          
            Vector3 target = Vector3.zero;

            Vector3 current = tray.transform.localScale;
            float f = 0;
            while (f <= 0.2f)
            {
                tray.transform.localScale = Vector3.Lerp(current, target, f * 5);
                f += Time.unscaledDeltaTime;
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }
            tray.transform.localScale = Vector3.zero;
            tray.SetActive(false);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }


    public void Set(bool load)
    {
        Dictionary<MachineType, MachineLevelData> levelDatas = GameInstance.GameIns.restaurantManager.machineLevelData;

        if (levelDatas.ContainsKey(machineType))
        {
            this.machineLevelData = levelDatas[machineType];
           
        }
        else
        {
            this.machineLevelData = AssetLoader.machines_levels[(int)machineType];
            levelDatas[machineType] = this.machineLevelData;
            int level = this.machineLevelData.level;
            int id = this.machineLevelData.id;
            int index = id + (level >= 41 ? 41 : level >= 31 ? 31 : 1);

            MachineLevelOffset offset = AssetLoader.machineLevelOffsets[index];

            BigInteger upgradePrice = this.machineLevelData.Price_Value + Mathf.FloorToInt(Mathf.Pow((level - 1), offset.price_pow) * offset.price_mul);
            sb = Utility.GetFormattedMoney(upgradePrice, sb);
            this.machineLevelData.calculatedPrice = sb.ToString();
            BigInteger salePrice = Utility.StringToBigInteger(this.machineLevelData.sale_proceed);
            if (offset.sale_div == 0) salePrice += Mathf.FloorToInt(Mathf.Pow((level - 1), offset.sale_pow) * offset.sale_mul);
            else salePrice += Mathf.FloorToInt(Mathf.Pow((level - 1), offset.sale_pow) * offset.sale_mul) / Mathf.FloorToInt((level - 1) * 0.07f);
            this.machineLevelData.calculatedSales = salePrice;

            this.machineLevelData.calculatedCookingTimer = this.machineLevelData.cooking_time - (level - 1) * offset.reduce_timer;
            if (this.machineLevelData.calculatedCookingTimer < 3f) this.machineLevelData.calculatedCookingTimer = 3f;
            this.machineLevelData.calculatedHeight = this.machineLevelData.max_height + Mathf.FloorToInt((level - 1) * offset.increase_height);

        }
        if (!load)
        {
            GameInstance.GameIns.restaurantManager.machineLevelDataChanged = true;
        }
       /* if (!load)
        {
            this.machineLevelData.fishes += 10;
        }*/
        // foodStack = FoodStackManager.FM.GetFoodStack();
        // foodStack.type = machineType;
        if (fuelGage == null)
        {
            fuelGage = GameInstance.GameIns.restaurantManager.GetGage();
            fuelGage.gameObject.SetActive(true);
            fuelGage.foodMachine = this;
            fuelGage.UpdateGage(this, machineLevelData.fishes, true);
            if (machineLevelData.fishes > 0) fuelGage.ShowGage(true);
            //    fuelGage.foreground.fillAmount = (float)10 / 100;
            //fuelGage.UpdateGage(machineLevelData.fishes);
        }

        GameInstance.GameIns.restaurantManager.machineLevelDataChanged = true;

    }

    public void Cooking()
    {
        switch(machineType)
        {
            case MachineType.None:
                break;
            case MachineType.BurgerMachine:
                cookingAction += GetComponent<BurgerMachine>().GetPatty;
                cookingFinishAction += GetComponent<BurgerMachine>().Done;
                break;
            case MachineType.CokeMachine:
                cookingAction += GetComponent<CokeMachine>().Shake;
                cookingFinishAction += GetComponent<CokeMachine>().Done;
                break;
            case MachineType.CoffeeMachine:
                cookingAction += GetComponent<CoffeeMachine>().Shake;
                cookingFinishAction += GetComponent<CoffeeMachine>().Done;
                break;
            case MachineType.DonutMachine:
                cookingAction += GetComponent<DonutMachine>().FryDonut;
                cookingFinishAction += GetComponent<DonutMachine>().Done;
                break;
        }
        canCooking = true;
        cancellationTokenSource = new CancellationTokenSource();
        CookingFood(cancellationTokenSource.Token).Forget();
      // StartCoroutine(Cook());
    }


    async UniTask CookingFood(CancellationToken cancellationToken = default)
    {
        try
        {
            await Utility.CustomUniTaskDelay(0.5f, cancellationToken);  
            //await UniTask.Delay(500, cancellationToken: cancellationToken);

            while (true)
            {
                if (machineLevelData == null || foodStack.foodStack.Count >= machineLevelData.calculatedHeight)
                {
                    await Utility.CustomUniTaskDelay(0.2f, cancellationToken);
                  //  await UniTask.Delay(200, cancellationToken: cancellationToken);
                    continue;
                }

                if (machineLevelData != null && machineLevelData.fishes >= 1 )
                {
                    machineLevelData.fishes -= 1;
                    fuelGage.UpdateGage(this, -1, false);
                    if (fuelGage) fuelGage.ShowGage(false);
                    noFishes = true;
                }
                else
                {
                    if(fuelGage) fuelGage.ShowGage(true);
                    noFishes = false;
                  //  await Utility.CustomUniTaskDelay(0.2f, cancellationToken);
                   // await UniTask.Delay(200, cancellationToken: cancellationToken);
                  //  continue;
                }

                audioSource.clip = GameInstance.GameIns.gameSoundManager.MachineSound(soundClip);
                audioSource.volume = 0.1f;
                audioSource.loop = true;
                audioSource.spatialBlend = 1;
                audioSource.maxDistance = SoundManager.Instance.max;
                audioSource.minDistance = SoundManager.Instance.min;
                audioSource.Play();

                float cookingTimer = machineLevelData.cooking_time * (noFishes == true ? 1 : 2);

                cookingAction?.Invoke(cookingTimer);

                await Utility.CustomUniTaskDelay(cookingTimer, cancellationToken);
            //    await UniTask.Delay((int)(cookingTimer * 1000), cancellationToken: cancellationToken);

                cookingFinishAction?.Invoke();
                await Utility.CustomUniTaskDelay(0.6f, cancellationToken);
               // await UniTask.Delay(600, cancellationToken: cancellationToken);

            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }


    public void CreatedFood(Food food)
    {
        food.foodPrice = machineLevelData.calculatedSales;

    }

    //float coroutineTimer = 0;
    IEnumerator Cook()
    {
       // int a = 0;
        yield return new WaitForSeconds(0.5f);
       // bool on = false;
        while (true)
        {
            //pooling version
            //if (!on)
            // if (mData.food_production_max_height > foodStack.foodStack.Count)
            {
                // if (!audioSource.isPlaying) audioSource.Play();
                // yield return new WaitForSeconds(cookingTimer);
              //  a++;

               // for (int i = 0; i < 1000; i++)
                {
                    //yield return new WaitForSecondsRealtime(cookingTimer);
                    //    coroutineTimer = 0;
                    yield return new WaitForSeconds(cookingTimer);
                    Food f = FoodManager.GetFood(foodMesh, machineType);
                    f.parentType = machineType;

                    if (machineType == MachineType.BurgerMachine) foodHight = 0.7f;
                    else if (machineType == MachineType.CokeMachine) foodHight = 1f;
                    else if (machineType == MachineType.CoffeeMachine) foodHight = 1.2f;
                    else if (machineType == MachineType.DonutMachine) foodHight = 0.5f;
                    Vector3 addheight = GameInstance.GetVector3(0, (foodStack.foodStack.Count) * foodHight, 0);
                    f.transforms.position = foodTransform.position + addheight;
                 //   f.foodPrice = machineLevelData.sale_proceed;
                    foodStack.foodStack.Push(f);
                  //  if(a==10) yield break;
                    //    if (foodStack.foodStack.Count == 10000) on = true;
                }
            }
         /*   else
            {
                //for (int i = 0; i < 50; i++)
                {
                    //    if (audioSource.isPlaying) audioSource.Stop();
                    // yield return null;
                    //      Food food = foodStack.foodStack[foodStack.foodStack.Count - 1];
                    //      foodStack.foodStack.RemoveAt(foodStack.foodStack.Count - 1);
                    //     FoodManager.EatFood(food);

                    //    if (foodStack.foodStack.Count == 0) on = false;
                }
                // FoodManager.NewFood(food);
            }*/
            // yield return null;
            //yield return null;
            /* if (!on)
             // if (mData.food_production_max_height > foodStack.foodStack.Count)
             {
                 for (int i = 0; i < 1000; i++)
                 {
                     //yield return new WaitForSecondsRealtime(cookingTimer);
                     //  coroutineTimer = 0;
                     Food f = Instantiate(TestFood);
                     f.parentType = machineType;

                     if (machineType == MachineType.BurgerMachine) foodHight = 0.7f;
                     else if (machineType == MachineType.CokeMachine) foodHight = 1f;
                     else if (machineType == MachineType.CoffeeMachine) foodHight = 1.2f;
                     else if (machineType == MachineType.DonutMachine) foodHight = 0.5f;
                     Vector3 addheight = GameInstance.GetVector3(0, foodStack.foodStack.Count * foodHight, 0);
                     f.transform.position = foodTransform.position + addheight;
                     f.foodPrice = mData.sale_proceeds;

                     foodStack.foodStack.Add(f);
                     if (foodStack.foodStack.Count == 10000) on = true;
                 }
             }
             else
             {
                 // for (int i = 0; i < 50; i++)
                 {
                     //    if (audioSource.isPlaying) audioSource.Stop();
                     // yield return null;
                     //   Food food = foodStack.foodStack[foodStack.foodStack.Count - 1];
                     //  foodStack.foodStack.RemoveAt(foodStack.foodStack.Count - 1);
                     //   Destroy(food.gameObject);
                     // FoodManager.EatFood(food);

                     //   if (foodStack.foodStack.Count == 0) on = false;
                 }
                 // FoodManager.NewFood(food);
             }*/
           // yield return null;
           
           // yield return GetWaitTimer.WaitTimer.GetTimer(1000);
            

        }


    }
    private void OnApplicationQuit()
    {
        isQuitting = true;
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }


    public void MachineSound(bool pause)
    {
        if(pause)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.UnPause();

        }
    }
}
