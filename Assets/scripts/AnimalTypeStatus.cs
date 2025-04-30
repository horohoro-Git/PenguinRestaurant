using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayType
{
    Employee,
    Customer
}
public enum AnimalType
{
    Penguin = 10,
    Cat = 100,
    Crow = 101,
    Dog = 102,
    Parrot = 103,
    Rabbit = 104,
    Tortoise = 105

}
public enum RewardingType
{
    None,
    Wait,
    Walk,
    Eat
}

public enum EmployeeState
{
    Wait,
    Counter,
    Serving,
    FoodMachine,
    Table,
    TrashCan
}

public enum CustomerState
{
    Walk,
    Counter,
    Table
}

public static class AnimalTypeStatus
{
    
}
