using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameInstance;
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
public enum LOD_Type
{
    LOD0,
    LOD1
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
    public bool is_customer;
    public readonly int ID => id;

    public readonly string Name => name;

    public AnimalStruct(int id, string name, string asset_name, int tier, float speed, float eat_speed, int min_order, int max_order, bool is_customer)
    {
        this.id = id;
        this.name = name;
        this.asset_name = asset_name;
        this.tier = tier;
        this.speed = speed;
        this.eat_speed = eat_speed;
        this.min_order = min_order;
        this.max_order = max_order;
        this.is_customer = is_customer;
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

    public static bool CheckHirable(Vector3 center, ref int x, ref int y, bool check, bool forcedCheck = false)
    {
        Vector3 loc = center;

        int cameraPosX = (int)((loc.x - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
        int cameraPosZ = (int)((loc.z - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);

        if (x == cameraPosX && y == cameraPosZ && !forcedCheck) return check;
        x = cameraPosX; y = cameraPosZ;

        InputManger.spawnDetects.Clear();


        float tileSize = GameIns.calculatorScale.distanceSize;

        Camera cam = null;
        if (InputManger.cachingCamera == null) cam = Camera.main;
        else cam = InputManger.cachingCamera;
        float radiusInGrid = (cam.orthographicSize / 2.5f) / tileSize;
        float radiusSqr = radiusInGrid * radiusInGrid;

        int minX = Mathf.FloorToInt(cameraPosX - radiusInGrid);
        int maxX = Mathf.CeilToInt(cameraPosX + radiusInGrid);
        int minY = Mathf.FloorToInt(cameraPosZ - radiusInGrid);
        int maxY = Mathf.CeilToInt(cameraPosZ + radiusInGrid);

        for (int xx = minX; xx <= maxX; xx++)
        {
            for (int yy = minY; yy <= maxY; yy++)
            {
                float dx = xx - cameraPosX;
                float dy = yy - cameraPosZ;

                if (dx * dx + dy * dy <= radiusSqr)
                {
                    if (!ValidCheck(yy, xx)) continue;

                    bool isBlocked = MoveCalculator.GetBlocks[yy, xx];

                    float worldX = GameIns.calculatorScale.minX + xx * tileSize;
                    float worldZ = GameIns.calculatorScale.minY + yy * tileSize;
                    Vector3 worldPos = new Vector3(worldX, 0, worldZ);

                    Color debugColor = isBlocked ? Color.red : Color.green;
                    Debug.DrawRay(worldPos, Vector3.up * 0.5f, debugColor, 0.1f);
                    if (!isBlocked) InputManger.spawnDetects.Add(worldPos);
                }
            }
        }

        if (InputManger.spawnDetects.Count > 0)
        {
            return true;
           // GameIns.applianceUIManager.UnlockHire(true);
        }
        else
        {
            return false;
         //   GameIns.applianceUIManager.UnlockHire(false);
        }
    }
}

public interface ITableID<K>
{
    K ID { get; }
    public string Name { get; }
}

