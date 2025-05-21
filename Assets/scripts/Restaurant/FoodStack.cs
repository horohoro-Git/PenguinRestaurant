using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStack
{
    public MachineType type;
    public Stack<Food> foodStack = new Stack<Food>();
    public int needFoodNum;
    public bool packaged;
    public int getNum;
    public int id;
    
    public FoodStack()
    {

    }
    public FoodStack(MachineType type, int needFoodNum, bool packaged, int id)
    {
        this.type = type;
        this.needFoodNum = needFoodNum;
        this.packaged = packaged;
        this.id = id;
    }
}
