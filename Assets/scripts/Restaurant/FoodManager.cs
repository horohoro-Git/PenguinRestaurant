using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static FoodMachine;

public static class FoodManager
{
   // public static Food food;
    
    static Queue<Food> foodsPooling = new Queue<Food>(500);
/*    static Queue<Food> deActivatedfoods = new Queue<Food>();
    static HashSet<Food> activatedfoods = new HashSet<Food>(1000);
*/
    static Queue<PackageFood> packagedPooling = new Queue<PackageFood>();
   /* static Queue<PackageFood> deActivatedPackageFoods = new Queue<PackageFood>();
    static HashSet<PackageFood> activatedPackageFoods = new HashSet<PackageFood>(200);*/

    //직원 먹이용 
    static Queue<Food> rewardsPooling = new Queue<Food>();
   /* static Queue<Food> deActovatedRewards = new Queue<Food>();
    static HashSet<Food> activatedRewards = new HashSet<Food>(100);*/

    static Queue<RewardsBox> rewardsBoxesPooling = new Queue<RewardsBox>();
   /* static Queue<RewardsBox> deActovatedRewardsBox = new Queue<RewardsBox>();
    static HashSet<RewardsBox> activatedRewardsBox = new HashSet<RewardsBox>(5);*/
    static Food foodCaching;
    static PackageFood packageFoodCaching;
    // Start is called before the first frame update
    public  static GameObject foodCollects;
    public static void NewFood(Food food,int count, bool rewards = false)
    {
        if (foodCollects == null)
        {
            foodCollects = new GameObject(); 
            foodCollects.name = "foodCollects";
            foodCollects.transform.position = Vector3.zero;
        }
        for (int i = 0; i < count; i++)
        {
            if (foodCaching == null) foodCaching = food;
            Food f = GameObject.Instantiate(food, foodCollects.transform);
            f.name = rewards ? "RewardFood" : "Food";
            f.foodIndex = i;
            f.gameObject.SetActive(false);
            if (!rewards) foodsPooling.Enqueue(f); // deActivatedfoods.Enqueue(f);
            else rewardsPooling.Enqueue(f); //deActovatedRewards.Enqueue(f);
        }
    }

    public static Food GetFood(Mesh meshFilter, MachineType machineType, bool rewards = false)
    {
        Queue<Food> targetPool = rewards ? rewardsPooling : foodsPooling;  // 풀을 선택
        if(targetPool.Count > 0)
        {
            Food f = targetPool.Dequeue();
            f.meshFilter.mesh = meshFilter;
            f.parentType = machineType;
            f.gameObject.SetActive(true);
            return f;


        }
        else
        {
            for(int i=0;i<10; i++)
            {
                Food newFood = GameObject.Instantiate(foodCaching, foodCollects.transform);
                newFood.name = "NewFood";
                newFood.gameObject.SetActive(false);
                targetPool.Enqueue(newFood);
            }

            Food f = targetPool.Dequeue();
            f.meshFilter.mesh = meshFilter;
            f.parentType = machineType;
            f.gameObject.SetActive(true);

            return f;
        }
    }

    public static void EatFood(Food food, bool rewards = false)
    {
        food.Resets();
        Queue<Food> targetPool = rewards ? rewardsPooling : foodsPooling;
        food.gameObject.SetActive(false);
        targetPool.Enqueue(food);
    }

    public static void NewPackageBox(PackageFood box, int num)
    {
        for (int i = 0; i < num; i++)
        {
            if (packageFoodCaching == null) packageFoodCaching = box;
            PackageFood f = GameObject.Instantiate(box, foodCollects.transform);
            f.foodIndex = i;
            f.gameObject.SetActive(false);
           // deActivatedPackageFoods.Enqueue(f);
            packagedPooling.Enqueue(f);
        }

    }

    public static PackageFood GetPackageBox()
    {
        if(packagedPooling.Count > 0)
        {
            PackageFood packageFood = packagedPooling.Dequeue();
            //  activatedPackageFoods.Add(packageFood);
            packageFood.gameObject.SetActive(true);
            return packageFood;
        }
        else
        {
            PackageFood packageFood = GameObject.Instantiate(packageFoodCaching, foodCollects.transform);

            return packageFood;
        }
     
    }

    public static void RemovePackageBox(PackageFood remove)
    {
        Stack<Food> temps = new Stack<Food>();
        for (int i = 0; i < remove.packageTrans.Length; i++)
        {
            Food food = remove.packageTrans[i].GetComponentInChildren<Food>();
            temps.Push(food);
        }
        while (temps.Count > 0)
        {
            Food food = temps.Pop();
            food.transform.SetParent(foodCollects.transform);
            food.transform.position = Vector3.zero;
            EatFood(food);
        }

        remove.gameObject.SetActive(false);
        packagedPooling.Enqueue(remove); 
      //  activatedPackageFoods.Remove(remove);
       // deActivatedPackageFoods.Enqueue(remove);
    }



    //직원 보상 관리
    //

    //보상 상자 인스턴스화
    public static void NewRewardsBox(RewardsBox rewardsBox, int count)
    {
      
        for (int i = 0; i < count; i++)
        {
            RewardsBox r = GameObject.Instantiate(rewardsBox, foodCollects.transform);
            r.boxIndex = i;
            r.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            r.transform.position = Vector3.zero;
            r.gameObject.SetActive(false);
            //deActovatedRewardsBox.Enqueue(r);
            rewardsBoxesPooling.Enqueue(r);
        }
    }


    //보상 상자 가져오기
    public static RewardsBox GetRewardsBox()
    {
        RewardsBox r = rewardsBoxesPooling.Dequeue();
        r.destroyed = false;
      //  r.meshFilter.mesh = meshFilter;
       // f.parentType = machineType;
      //  r.gameObject.SetActive(true);
     //   activatedRewardsBox.Add(r);
        return r;
    }

    //보상 상자 제거
    public static void RemoveRewardsBox(RewardsBox rewardsBox)
    {
      //  activatedRewardsBox.Remove(rewardsBox);
       // deActovatedRewardsBox.Enqueue(rewardsBox);
        rewardsBox.gameObject.SetActive(false);
        rewardsBoxesPooling.Enqueue(rewardsBox);
    }
}
