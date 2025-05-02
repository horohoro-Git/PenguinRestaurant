using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MachineType
{
    None,
    BurgerMachine,
    CokeMachine,
    CoffeeMachine,
    DonutMachine,
    PackingTable
}
public enum MapType 
{
    town,
    forest,
    winter
}
public class RestaurantParam
{
    public int id;
    public int type;
    public bool unlock;

    public RestaurantParam()
    {

    }
    public RestaurantParam(int id, int type, bool unlock)
    {
        this.id = id;
        this.type = type;
        this.unlock = unlock;
    }

    public RestaurantParam(RestaurantParam restaurantParam)
    {
        this.id = restaurantParam.id;
        this.type = restaurantParam.type;
        this.unlock = restaurantParam.unlock;
    }
    /* public void Set(int id, int type, bool unlock)
     {
         this.id = id;
         this.type = type;
         this.unlock = unlock;
     }
     public RestaurantParam(RestaurantParam restaurantParam)
     {
         this.id = restaurantParam.id;
         this.type = restaurantParam.type;
         this.unlock = restaurantParam.unlock;
     }*/
}
[System.Serializable]
public class PlayerData
{
    public int level;
    public float money;
    public int fishesNum;
    public int employeeNum;
    public string time;
    public int fishesNum_InBox;

    public PlayerData(int level, float money, int fishesNum, int employeeNum, string time, int fishesNum_InBox)
    {
        this.level = level;
        this.money = money;
        this.employeeNum = employeeNum;
        this.time = time;
        this.fishesNum = fishesNum;
        this.fishesNum_InBox = fishesNum_InBox;
    }
}

public class LevelData : ITableID<int>
{
    public int id;
    public int level;
    public int exp;

    public int ID => id;

    public string Name => throw new System.NotImplementedException();

    public LevelData(int id, int level, int exp)
    {
        this.id = id;
        this.level = level;
        this.exp = exp;
    }
    public LevelData()
    {

    }
}


public struct PlayerStruct
{
    public int level;
    public float money;
    public int fishesNum;
    public int employeeNum;
    public string time;
    public int fishesNum_InBox;

    public PlayerStruct(int level, float money, int fishesNum, int employeeNum, string time, int fishesNum_InBox)
    {
        this.level = level;
        this.money = money;
        this.employeeNum = employeeNum;
        this.time = time;
        this.fishesNum = fishesNum;
        this.fishesNum_InBox = fishesNum_InBox;
    }
}

public struct EmployeeData
{
    public int level;
    public float move_speed;
    public float action_speed;
    public int max_holds;
    public int upgrade_cost;

    public EmployeeData(int level, float move_speed, float action_speed, int max_holds, int upgrade_cost)
    {
        this.level = level;
        this.move_speed = move_speed;
        this.action_speed = action_speed;
        this.max_holds = max_holds;
        this.upgrade_cost = upgrade_cost;

    }
}

public struct AnimalStruct : ITableID<int>
{
    public int id;
    public string name;
    public string asset_name;
    public int tier;
    public float speed;
    public float eat_speed;
    public int min_order;
    public int max_order;   
    public readonly int ID => id;

    public readonly string Name => name;

    public AnimalStruct(int id, string name, string asset_name, int tier, float speed, float eat_speed, int min_order, int max_order)
    {
        this.id = id;
        this.name = name;
        this.asset_name = asset_name;
        this.tier = tier;
        this.speed = speed;
        this.eat_speed = eat_speed;
        this.min_order = min_order;
        this.max_order = max_order;
    }

}
public struct ItemStruct : ITableID<int>
{
    public int id;
    public string asset_name;

   public readonly int ID => id;

    public readonly string Name => asset_name;

}
public struct MachineLevelStruct : ITableID<int>
{
    public int id;
    public int level;
    public int price;
    public int sale_proceed;
    public float cooking_time;
    public int max_height;
    public int type;
    public readonly int ID => id;

    public readonly string Name => throw new System.NotImplementedException();
}

public struct EmployeeLevelStruct : ITableID<int>
{
    public int level;
    public float move_speed;
    public int max_weight;
    public int exp;
    public int current_exp;
    public readonly int ID => level;

    public readonly string Name => throw new System.NotImplementedException();
}

public struct StringStruct : ITableID<string>
{
    public string str;

    public readonly string Name => str;

    public readonly string ID => str;

    public StringStruct(string str)
    {
        this.str = str;
    }
}




public enum AssetType
{
    None,
    Map,
    GameObject

}




public class Utility
{
    public static bool IsInsideCameraViewport(Vector2 screenPosition, Camera cam)
    {
        Vector3 viewportPos = cam.ScreenToViewportPoint(screenPosition);
        return viewportPos.x >= 0f && viewportPos.x <= 1f && viewportPos.y >= 0f && viewportPos.y <= 1f;
    }

    public static bool TryGetComponentInParent<T>(GameObject go, out T target)
    {
        target = go.GetComponentInParent<T>();

        return target != null ? true : false;
    }

    public static bool TryGetComponentInChildren<T>(GameObject go, out T target)
    {
        target = go.GetComponentInChildren<T>();

        return target != null ? true : false;
    }
    public static bool ValidCheck(int r, int c)
    {
        if (r >= 0 && r < GameInstance.GameIns.calculatorScale.sizeY && c >= 0 && c < GameInstance.GameIns.calculatorScale.sizeX)
        {
            return true;
        }

        return false;
    }
}

public interface ITableID<K>
{
    K ID { get; }
    public string Name { get; }
}
