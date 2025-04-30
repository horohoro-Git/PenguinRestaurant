using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
//using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
//?œê?

public struct FoodsAnimalsWant
{
    public bool burger, coke, coffee, donut;
    public int burgerNum, cokeNum, coffeeNum, donutNum;
    public AnimalSpawner.SpawnerType spawnerType;
}


public class AnimalSpawner : MonoBehaviour
{
    HashSet<Customer> waitingCustomers = new HashSet<Customer>(10);
    GameInstance gameInstance;
    AnimalManager animalManager;
    public int maxCustomer = 6;
    public enum SpawnerType
    {
        None = -1,
        FastFood = 0,
        DonutShop = 1,
        Delivery = 2,
        TakeOut = 3
    }
    public SpawnerType type;

    // Start is called before the first frame update
    private void Awake()
    {
     
       
    }
    void Start()
    {
        gameInstance = GameInstance.GameIns;
        animalManager = gameInstance.animalManager;
        gameInstance.workSpaceManager.spwaners.Add(this);

        if (animalManager.mode == AnimalManager.Mode.GameMode) StartCoroutine(Spawn());
    }

    //float coroutineTimer =0;
    float coroutineTimer2 =0;

    Customer deliveryCustomer;
  
    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
           // Debug.Log(GameInstance.GameIns.restaurantManager.GetRestaurantValue());
            WorkSpaceManager workSpaceManager = gameInstance.workSpaceManager;

          
         //   yield return GetWaitTimer.WaitTimer.GetTimer(3000); //new WaitForSeconds(3);
         //    GetWaitTimer.WaitTimer.GetTimer(3000);

            // for(int i=0; i)
            if (waitingCustomers.Count < maxCustomer)
            {
                switch (type)
                {
                    case SpawnerType.FastFood:
                        {
                           // yield break;
                            bool burger = false;
                            bool coke = false;

                            GetFoodUnlock(out burger, out coke, 0);

                            if (burger || coke)
                            {
                                FoodsAnimalsWant foodsAnimalsWant = new FoodsAnimalsWant();
                                foodsAnimalsWant.burger = burger;
                                foodsAnimalsWant.coke = coke;
                                foodsAnimalsWant.spawnerType = SpawnerType.FastFood;
                                Customer ac = animalManager.SpawnCustomer(foodsAnimalsWant);
                                ac.animalSpawner = this;
                                ac.trans.position = transform.position;
                                waitingCustomers.Add(ac);
                                GameInstance.GameIns.animalManager.AttacCustomerTask(ac);

                                float value = GameInstance.GameIns.restaurantManager.GetRestaurantValue();
                                float timerValue = 1650 / value + 2;
                              
                                yield return new WaitForSeconds(timerValue);
                            }
                            break;
                        }
                    case SpawnerType.DonutShop:
                        {
                          //  yield break;
                            bool coffee = false;
                            bool donut = false;

                            GetFoodUnlock(out coffee, out donut, 1);

                            if (coffee || donut)
                            {
                                FoodsAnimalsWant foodsAnimalsWant = new FoodsAnimalsWant();
                                foodsAnimalsWant.coffee = coffee;
                                foodsAnimalsWant.donut = donut;
                                foodsAnimalsWant.spawnerType = SpawnerType.DonutShop;
                                Customer ac = animalManager.SpawnCustomer(foodsAnimalsWant);
                                ac.animalSpawner = this;
                                ac.trans.position = transform.position;
                                waitingCustomers.Add(ac);
                                GameInstance.GameIns.animalManager.AttacCustomerTask(ac);

                                float value = GameInstance.GameIns.restaurantManager.GetRestaurantValue();
                                float timerValue = 1650 / value + 2;
                               
                                yield return new WaitForSeconds(timerValue);
                            }
                            break;
                        }
                    case SpawnerType.TakeOut:
                        {
                            if (workSpaceManager.unlocks[2] > 0)
                            {
                                bool burger = false, coke = false, coffee = false, donut = false;
                                int rand = UnityEngine.Random.Range(0, 2);

                                if (rand == 0)
                                {
                                    int r = UnityEngine.Random.Range(0, 4);
                                    burger = r == 0 ? true : false; coke = r == 1 ? true : false; coffee = r == 2 ? true : false; donut = r == 3 ? true : false;
                                }
                                else
                                {
                                    bool[] b = new bool[4];
                                    int r = 0, rs = 0;
                                    while (r != rs)
                                    {
                                        r = UnityEngine.Random.Range(0, 4);
                                        rs = UnityEngine.Random.Range(0, 4);
                                    }
                                    b[r] = true;
                                    b[rs] = true;
                                    burger = b[0];
                                    coke = b[1];
                                    coffee = b[2];
                                    donut = b[3];
                                }
                                FoodsAnimalsWant foodsAnimalsWant = new FoodsAnimalsWant();
                                foodsAnimalsWant.burger = burger;
                                foodsAnimalsWant.coke = coke;
                                foodsAnimalsWant.coffee = coffee;
                                foodsAnimalsWant.donut = donut;
                                foodsAnimalsWant.spawnerType = SpawnerType.TakeOut;
                                animalManager.SpawnCustomer(foodsAnimalsWant);

                            }
                            break;
                        }
                    case SpawnerType.Delivery:
                        {
                            yield return GetWaitTimer.WaitTimer.GetTimer(5000);
                            if(deliveryCustomer != null)
                            {

                                while (deliveryCustomer.foodStacks[0].foodStack.Count != deliveryCustomer.foodStacks[0].needFoodNum)
                                {
                                    while(coroutineTimer2 <= 2f)
                                    {
                                        coroutineTimer2 += Time.deltaTime;
                                        yield return null;
                                    }
                                    coroutineTimer2 = 0;
                                }
                                GameInstance.GameIns.uiManager.UpdateOrder(deliveryCustomer, Counter.CounterType.Delivery);
                                GameInstance.GameIns.animalManager.DespawnCustomer(deliveryCustomer, true);
                                deliveryCustomer = null;
                            }
                            else
                            {
                                FoodsAnimalsWant foodsAnimalsWant = new FoodsAnimalsWant();
                                foodsAnimalsWant.burger = true;
                                foodsAnimalsWant.coke = true;
                                foodsAnimalsWant.spawnerType = SpawnerType.Delivery;
                               
                              
                                if (GameInstance.GameIns.workSpaceManager.packingTables.Count == 2)
                                {

                                    deliveryCustomer = animalManager.SpawnCustomer(foodsAnimalsWant, true);
                                    for (int i = 0; i < workSpaceManager.packingTables.Count; i++)
                                    {
                                        if (workSpaceManager.packingTables[i].counterType == Counter.CounterType.Delivery)
                                        {
                                            FoodStack foodStack = new FoodStack();
                                            foodStack.needFoodNum = UnityEngine.Random.Range(1,2);
                                            foodStack.type = MachineType.PackingTable;
                                            deliveryCustomer.foodStacks.Add(foodStack);

                                            workSpaceManager.packingTables[i].customer = deliveryCustomer;
                                            deliveryCustomer.transform.position = new Vector3(workSpaceManager.packingTables[i].transform.position.x, 0, workSpaceManager.packingTables[i].transform.position.z - 10);

                                            GameInstance.GameIns.uiManager.UpdateOrder(deliveryCustomer, Counter.CounterType.Delivery);
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                }
            }
           // coroutineTimer = 0;

            yield return null;
            //  yield break;
            //  yield return new WaitForSeconds(6);
        }
    }

    public void RemoveWaitingCustomer(Customer customer)
    {
        waitingCustomers.Remove(customer);
    }

    void GetFoodUnlock(out bool foodA, out bool foodB, int index)
    {
        foodA = false;
        foodB = false;
        WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;

        if (workSpaceManager.unlocks[index] == 1)
        {
            foodA = true;
        }
        if (workSpaceManager.unlocks[index] == 2)
        {
            int rand = UnityEngine.Random.Range(0, 3);

            if (rand == 0 || rand == 2) foodA = true;
            if (rand == 1 || rand == 2) foodB = true;

        }
    }
}
