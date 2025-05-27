using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static GameInstance;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;
using System.Linq;
using UnityEngine.Windows;
using Unity.VisualScripting;

public enum CounterType
{
    None,
    FastFood,
    Donuts,
    Delivery,
    TakeOut
}

public enum WorkSpaceType
{
    None,
    Counter,
    Table,
    FoodMachine,
    Trashcan
}
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
    public WorkSpaceType type;
    public int level;
    public UnityEngine.Vector3 position;
    public UnityEngine.Vector3 localPos;
    public  Quaternion rotation;

    
    public RestaurantParam(int id, WorkSpaceType type, int level, Vector3 position, Vector3 localPos, Quaternion rotation)
    {
        this.id = id;
        this.type = type;
        this.level = level;
        this.position = position;
        this.localPos = localPos;
        this.rotation = rotation;
    }
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
    public float size_width;
    public float size_height;
    public float offset_x;
    public float offset_z;

    public readonly int ID => id;

    public readonly string Name => name;

    public AnimalStruct(int id, string name, string asset_name, int tier, float speed, float eat_speed, int min_order, int max_order, bool is_customer, float size_width, float size_height, float offset_x, float offset_z)
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
        this.size_width = size_width;
        this.size_height = size_height;
        this.offset_x = offset_x;
        this.offset_z = offset_z;
    }

}

public struct GoodsStruct : ITableID<int>
{
    public int id;
    public string asset_name;
    public string name;
    public WorkSpaceType type;
    public int price;
    public int num;
    public int require;
    public bool soldout;

    public readonly int ID => id;

    public readonly string Name => asset_name;
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
    public static string[] format = new string[5] {"", "K", "M", "B", "T"};
    public static string[] alphabet = new string[26] {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l",
                                                     "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
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

    public static bool ValidCheckWithCharacterSize(int x, int y, int[] offsetX, int[] offsetY)
    {
        float cellSize = GameIns.calculatorScale.distanceSize;
/*        int[] offsetX = { 1, 1, 0, -1, -1, -1, 0, 1 };
        int[] offsetY = { 0, -1, -1, -1, 0, 1, 1, 1 };*/

        for (int i = 0; i < 8; i++)
        {
            int targetX = x + offsetX[i];
            int targetY = y + offsetY[i];

            if (targetX >= 0 && targetX < GameIns.calculatorScale.sizeX && targetY >= 0 && targetY < GameIns.calculatorScale.sizeY)
            {
                if(MoveCalculator.GetBlocks[MoveCalculator.GetIndex(targetX, targetY)]) return false;
            }
        }

        return true;
    }

    public static bool CheckHirable(Vector3 center, ref int x, ref int y, bool check, bool forcedCheck = false)
    {
        Vector3 loc = center;

        int cameraPosX = Mathf.FloorToInt((loc.x - GameIns.calculatorScale.minX) / GameIns.calculatorScale.distanceSize);
        int cameraPosZ = Mathf.FloorToInt((loc.z - GameIns.calculatorScale.minY) / GameIns.calculatorScale.distanceSize);

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

                    bool isBlocked = MoveCalculator.GetBlockEmployee[MoveCalculator.GetIndex(xx, yy)];

                    float worldX = GameIns.calculatorScale.minX + xx * tileSize;
                    float worldZ = GameIns.calculatorScale.minY + yy * tileSize;
                    Vector3 worldPos = new Vector3(worldX, 0, worldZ);

                 //   Color debugColor = isBlocked ? Color.red : Color.green;
                   // Debug.DrawRay(worldPos, Vector3.up * 0.5f, debugColor, 0.1f);
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

    public static void GetComponentsInChildrenReUse<T>(List<T> values, GameObject targetGO)
    {
        if (values == null || targetGO == null) return;

        values.Clear(); // ���� ���� �ʱ�ȭ

        // GetComponentsInChildren�� �ڱ� �ڽŵ� �����ϹǷ� �ʿ信 ���� ���� ����
        targetGO.GetComponentsInChildren(true, values); // ��Ȱ�� ���� true
    }


    public static string GetFormattedMoney(BigInteger bigInteger)
    {
        string returnVal = "";
        returnVal = bigInteger.ToString();  
        int num = returnVal.Length;
        int sliced = (num - 1) / 3;

        int odd = (num - 1) % 3;

        string result = "";
        for(int i=0; i<= odd; i++) result += returnVal[i];
        if(num > odd + 2)
        {
            if (returnVal[odd + 1] != '0' || returnVal[odd + 2] != '0')
            {
                result += "." + returnVal[odd + 1] + returnVal[odd + 2];
            }
        }
        else if(num > odd + 1)
        {
            if (returnVal[odd + 1] != '0')
            {
                result += "." + returnVal[odd + 1];
            }

        }
        if (sliced < format.Length) result += format[sliced];
        else
        {
            int temp = sliced - 5; 
            string t = "";
            //  int test = temp % 26;
            // temp -= test;
            int test = temp;
            bool odds = false;
            while (true)
            {

                test = Mathf.RoundToInt((float)test / 26);
                if (test == 0)
                {
                    odds = false;
                    break;
                }
                if (test == 1)
                {
                    odds = true;
                    break;
                }
            }
            Debug.Log(temp);
            while(temp > 0)
            {
               
             
                int p = temp % 26;
                if (odds && temp == 1)
                {
                    Debug.Log("check");
                    t = alphabet[p - 1] + t;
                    odds = false;
                }
                else t = alphabet[p] + t;
                temp -= p;
                temp /= 26;
               /* if(temp < 26)
                {
                    break;
                }
*/
            }

          //  t += alphabet[test];

            result += t;
        }

        
        return result;
    }

    public static string test(int input)
    {
        if (input < 0) return "";

       // input += 1; 

        string result = "";

        while (input > 0)
        {
          //  input -= 1;
            int mod = input % 26;
            result = alphabet[mod] + result;
            input /= 26;
        }

        return result;
    }
}

public class RestaurantCurrency
{
    public BigInteger money;
    public int fishes;
    public int affinity;
    public int extension_level;
    public bool changed;
    public RestaurantCurrency(BigInteger money, int fishes, int affinity, int extension_level)
    {
        this.money = money;
        this.fishes = fishes;
        this.affinity = affinity;
        this.extension_level = extension_level;
    }
}

public class Employees
{
    public int num;
    public List<EmployeeLevelData> employeeLevelDatas;
    public bool changed;
    public Employees(List<EmployeeLevelData> employeeLevelDatas)
    {
        this.employeeLevelDatas = employeeLevelDatas;
        this.num = employeeLevelDatas.Count;
    }
}

public class EmployeeLevelData
{
    public int level;
    public int exp;
    public int targetEXP;
    public EmployeeLevelData(int level, int exp, int targetEXP)
    {
        this.level = level;
        this.exp = exp;
        this.targetEXP = targetEXP;
    }
}

public interface ITableID<K>
{
    K ID { get; }
    public string Name { get; }
}
public interface IObjectOffset
{
    public Transform offset { get; set; }
}

