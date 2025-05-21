using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStackManager
{
    static FoodStackManager instance = new FoodStackManager();

    public static FoodStackManager FM { get { return instance; } }

    Queue<FoodStack> deactivatedStack = new Queue<FoodStack>();
    List<FoodStack> activatedStack = new List<FoodStack>();

    //r = new FoodStackManager();
    // Start is called before the first frame update
  
    FoodStackManager()
    {
      
        activatedStack.Capacity = 1000;
        for(int i =0; i<100; i++)
        {
            deactivatedStack.Enqueue(new FoodStack());
        }
    }

    public FoodStack GetFoodStack()
    {
        FoodStack newStack = deactivatedStack.Dequeue();
        activatedStack.Add(newStack);
        return newStack;
    }
    public void RemoveFoodStack(FoodStack stack)
    {
        activatedStack.Remove(stack);
        deactivatedStack.Enqueue(stack);
    }
}
