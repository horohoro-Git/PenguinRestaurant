using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FoodMachine : MonoBehaviour
{

    public MachineType machineType;

    public Mesh mesh;
    public Food food;
    public FoodStack foodStack = new FoodStack();
    //int maxNum = 10;
    public float cookingTimer = 3f;
    public Transform foodTransform;
    public Transform workingSpot;
    public AnimalController employee;
    public FoodStack[] canBringTheFoods;
    AudioSource audioSource;
    //  public PlayData playData;
    public RestaurantParam restaurantParam;
    public LevelData levelData;
    public int id;
    public int level=1;
    public MachineData mData;
    public MachineLevelStruct machineLevelStruct;
    public Transform transforms;
    public Transform modelTrans;

    public Food TestFood;
    public MachineData machineData {
        get { return mData; }

        set { mData = value;
        //    level = mData.level;
        }
    }

    private float foodHight;
    [NonSerialized]
    public int getNum;
    private void Awake()
    {
        transforms = transform;
     //   foodStack.foodStack.Capacity = 40000;
    }
    // Start is called before the first frame update
    void Start()
    {        
        audioSource = GetComponent<AudioSource>();
       
        foodStack.type = machineType;
        Cooking();
        //for(int i=0; i<200; i++) FoodManager.NewFood(Instantiate(food));
       
    }
    public void Set(int id)
    {
        Debug.Log("FoodMachine" + id);
        this.id = id;
        restaurantParam = GameInstance.GameIns.restaurantManager.restaurantparams[id + 1];
        levelData = GameInstance.GameIns.restaurantManager.levelData[id + 1];
        machineLevelStruct = GameInstance.GameIns.restaurantManager.machineLevelData[machineType][levelData.level];
    }

    public void Cooking()
    {
        CookingFood().Forget();
      // StartCoroutine(Cook());
    }


    async UniTask CookingFood()
    {
        await UniTask.Delay(500);

        while (true)
        {
            if (foodStack.foodStack.Count >= machineLevelStruct.max_height)
            {
                await UniTask.Delay(200);
                continue;
            }

            for(int i =0; i<100; i++)
            {
                int timer = (int)(machineLevelStruct.cooking_time * 10);
                await UniTask.Delay(timer);
            }
            Food f = FoodManager.GetFood(mesh, machineType);
            f.parentType = machineType;
            if (machineType == MachineType.BurgerMachine) foodHight = 0.7f;
            else if (machineType == MachineType.CokeMachine) foodHight = 1f;
            else if (machineType == MachineType.CoffeeMachine) foodHight = 1.2f;
            else if (machineType == MachineType.DonutMachine) foodHight = 0.5f;
            Vector3 addheight = GameInstance.GetVector3(0, (foodStack.foodStack.Count) * foodHight, 0);
            f.transforms.position = foodTransform.position + addheight;
            f.foodPrice = machineLevelStruct.sale_proceed;
            foodStack.foodStack.Push(f);
        }
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
                    Food f = FoodManager.GetFood(mesh, machineType);
                    f.parentType = machineType;

                    if (machineType == MachineType.BurgerMachine) foodHight = 0.7f;
                    else if (machineType == MachineType.CokeMachine) foodHight = 1f;
                    else if (machineType == MachineType.CoffeeMachine) foodHight = 1.2f;
                    else if (machineType == MachineType.DonutMachine) foodHight = 0.5f;
                    Vector3 addheight = GameInstance.GetVector3(0, (foodStack.foodStack.Count) * foodHight, 0);
                    f.transforms.position = foodTransform.position + addheight;
                    f.foodPrice = machineLevelStruct.sale_proceed;
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
}
