using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackingTable : Counter
{
  //  public List<FoodStack> foodStack = new List<FoodStack>();
 //   public Transform[] stackPoints;
  //  public Transform workingSpot_SmallTables;
    public Transform packingTrans;
    public Transform endTrans;
    public Transform packageStackTrans;
    public PackageFood packageFood;

    public FoodStack f1;
    public FoodStack f2;

    public FoodStack packageStack;
    public FoodStack packagingStack;
    public List<Transform> packingTransforms = new List<Transform>();

    public int packingNumber;
    public AnimalController employeeAssistant;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        if (counterType != CounterType.Delivery)
        {
            f1 = new FoodStack();
            f1.type = MachineType.BurgerMachine;

            foodStacks.Add(f1);

            f2 = new FoodStack();
            f2.type = MachineType.CokeMachine;

            foodStacks.Add(f2);
            packageFood = FoodManager.GetPackageBox();
            packageFood.transforms.position = smallTable.position;
            packagingStack = new FoodStack();
            packageStack = new FoodStack();
            packageStack.type = MachineType.PackingTable;
            f1.id = 5; f2.id = 6;
           // packageStack.id = 7;
            GameInstance.GameIns.restaurantManager.foodStacks.Add(f1);
            GameInstance.GameIns.restaurantManager.foodStacks.Add(f2);
          //  smallTable
        }
        else
        {
           
            packageStack = new FoodStack();
            packageStack.type = MachineType.PackingTable;
            packageStack.packaged = true;
            foodStacks.Add(packageStack);
        }
      //  GameInstance.GameIns.restaurantManager.foodStacks.Add(packageStack);
      //  foodStacks.Add(packageStack);

        for(int i = 0;i < foodStacks.Count; i++)
        {
            Debug.Log("pakaging " +  foodStacks[i].type +  "  " + foodStacks[i].packaged);
        }
    }
    public void SpawnBox()
    {
        packageFood = FoodManager.GetPackageBox();
       // packageFood.transforms.SetParent(smallTable);
        packageFood.transforms.position = smallTable.position;
    }
}
