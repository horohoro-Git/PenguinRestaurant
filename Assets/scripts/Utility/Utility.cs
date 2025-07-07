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
using System.Text;
using JetBrains.Annotations;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

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

public enum ParticleType
{
    Eating,
    Fishing
}

public enum BlackConsumerState
{
    None,
    Spawn,
    FindingTarget,
    FoundTarget,
    MoveToTarget,
    Steal,
    Kidding,
    SubDue
}

public enum AnimalPersonality
{
    Normal,
    Foodie,
    Relaxed,
    Loyal,
    Impatient,
    LightEater,
    HardToPlease
}

public class RestaurantParam
{
    public int id;
    public WorkSpaceType type;
    public int level;
    public Vector3 position;
    public Vector3 localPos;
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

public class FoodMachineParam
{
    public MachineType type;
    public int level;
    public int fuel;
    
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

public struct MapContent
{
    public int id;
    public string map_name;
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
    public List<int> personalities;
    public readonly int ID => id;

    public readonly string Name => name;

    public AnimalStruct(int id, string name, string asset_name, int tier, float speed, float eat_speed, int min_order, int max_order, bool is_customer, float size_width, float size_height, float offset_x, float offset_z, List<int> personalities)
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
        this.personalities = personalities;
    }

}

public struct GoodsStruct : ITableID<int>
{
    public int id;
    public string asset_name;
    public string name;
    public WorkSpaceType type;
    public string price;
    public int num;
    public int require;
    public float pow;
    public string sale;
    public bool soldout;
    public string defaultPrice;
    BigInteger? price_value;
    public string Price { get { return price; } set { price = value; price_value = null; } } 
    public BigInteger Price_Value { get { if(!price_value.HasValue) price_value = Utility.StringToBigInteger(price); return price_value.Value; } }

    public readonly int ID => id;

    public readonly string Name => asset_name;
}

public struct MachineLevelOffset : ITableID<int>
{
    public int id;
    public float price_pow;
    public float price_mul;
    public float price_div;
    public float sale_pow;
    public float sale_mul;
    public float sale_div;
    public float reduce_timer;
    public float increase_height;

    public readonly int ID => id;

    public readonly string Name => throw new System.NotImplementedException();
}

public struct ItemStruct : ITableID<int>
{
    public int id;
    public string asset_name;

   public readonly int ID => id;

    public readonly string Name => asset_name;

}

public struct EmployeeLevelStruct : ITableID<int>
{
    public int id;
    public float move_speed;
    public int max_weight;
    public int exp;
    public float increase_speed;
    public float increase_weight;
    public float increase_exp_pow;
    public float increase_exp_mul;
    public readonly int ID => id;

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

public struct SoundsStruct : ITableID<int>
{
    public int id;
    public string asset_name;

    public readonly int ID => id;

    public readonly string Name => asset_name;
}


public enum AssetType
{
    None,
    Map,
    GameObject

}

public enum MiniGameType
{
    None,
    Fishing


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
        if (r >= 0 && r < GameInstance.GameIns.calculatorScale.sizeY && c >= 0 && r < 400 && c < 400 && c < GameInstance.GameIns.calculatorScale.sizeX)
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

    public static bool CheckHirable(Vector3 center, ref int x, ref int y, bool check, bool forcedCheck = false, bool justLoad = false)
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
        float radiusInGrid = justLoad == false ? (cam.orthographicSize / 2.5f) / tileSize : (22  / tileSize);
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

    public static void GetComponentsInChildrenReUse<T>(List<T> values, GameObject targetGO)
    {
        if (values == null || targetGO == null) return;

        values.Clear(); // 기존 내용 초기화

        // GetComponentsInChildren은 자기 자신도 포함하므로 필요에 따라 제외 가능
        targetGO.GetComponentsInChildren(true, values); // 비활성 포함 true
    }
    public static BigInteger StringToBigInteger(string price)
    {
        BigInteger result;
        int num = price.Length;
        int slicedNum = (num - 1) % 3;
        StringBuilder sb = new StringBuilder();

        sb.Append(price[0]);
        for (int i = 1; i <= slicedNum; i++)
        {
            if (price[i] >= '0' && price[i] <= '9') sb.Append(price[i]);
            else break;
        }
        int dotCount = 0;
        int dotCheck = num > 4 ? 4 : num;
        for (int i = 0; i < dotCheck; i++)
        {
          /*  if (price[i] >= '0' && price[i] <= '9')
            {
                sb.Append(price[i]);
            }
*/
            if (price[i] == '.')
            {
                //  sb.Append(price[i]);
                dotCount++;
                sb.Append(price[i + 1]);
                if (i + 2 < num && price[i + 2] >= '1' && price[i + 2] <= '9')
                {
                    dotCount++;
                    sb.Append(price[i + 2]);
                }
                break;
            }
        }
        int nCount = 0;
        int count = 1;
        int baseUnitOffset = 27;
        int basicPrefixCount = format.Length;
        //  int defaultOffet = 27;
        int sum = 0;
        for (int i = 0; i < format.Length; i++)
        {
            if (!string.IsNullOrEmpty(format[i]) && price[num - 1] == format[i][0])    
            {
                int n = i * 3;
                if(dotCount > 0)
                {
                    n -= dotCount;
                    dotCount = 0;
                }
                sb.Append('0', n);
                result = BigInteger.Parse(sb.ToString());
                return result;
            }

        }
        for (int i = num - 1; i >= 0; i--)
        {
            if (price[i] >= '0' && price[i] <= '9') break;

            for (int j = 0; j < alphabet.Length; j++)
            {
                if (alphabet[j][0] == price[i])
                {
                    count = j + 1;
                }
            }
            if (nCount == 0) sum += count;
            else
            {
                sum += count * nCount * 26;
            }
            nCount++;
        }
        if (nCount == 0)
        {
            result = BigInteger.Parse(sb.ToString());
            return result;
        }
        else
        {
            sum += -baseUnitOffset + basicPrefixCount;
            sum *= 3;
            sum -= dotCount;
            sb.Append('0', sum);
            result = BigInteger.Parse(sb.ToString());
            return result;
        }
    }

    public static StringBuilder GetFormattedMoney(BigInteger bigInteger, StringBuilder result)
    {
        result.Clear();
        string returnVal = "";
        returnVal = bigInteger.ToString();  
        int num = returnVal.Length;
        if (num < 4)
        {
            result.Append(returnVal);
            return result;
        }
        int sliced = (num - 1) / 3;
        int odd = (num - 1) % 3;

        for(int i=0; i<= odd; i++) result.Append(returnVal[i]);
        if(num > odd + 2)
        {
            if (returnVal[odd + 2] != '0')
            {
                result.Append(".");
                result.Append(returnVal[odd + 1]);
                result.Append(returnVal[odd + 2]);
            }
            else if (returnVal[odd + 1] != '0')
            {
                result.Append(".");
                result.Append(returnVal[odd + 1]);
            }
           
        }
        else if(num > odd + 1)
        {
            if (returnVal[odd + 1] != '0')
            {
                result.Append(".");
                result.Append(returnVal[odd + 1]);
            }

        }
        if (sliced < format.Length) result.Append(format[sliced]);
        else
        {
            int temp = sliced - 5; 
            string t = "";
           
            int test = temp;
            bool rounds = false;


            if(temp < 26)
            {
                t += "a";
                t += alphabet[temp % 26];
                result.Append(t);
                return result;
            }

            while (true)
            {

                test = Mathf.RoundToInt((float)test / 26);
                if (test == 0)
                {
                    rounds = false;
                    break;
                }
                if (test == 1)
                {
                    rounds = true;
                    break;
                }
            }

            while(temp > 0)
            {
                int p = temp % 26;
                if (rounds && temp == 1)
                {
                    t = alphabet[p - 1] + t;
                    rounds = false;
                }
                else t = alphabet[p] + t;
                temp -= p;
                temp /= 26;
            }
            result.Append(t);
        }
        return result;
    }


    public static async UniTask CustomUniTaskDelay(float delayTime, CancellationToken cancellationToken = default)
    {
        try
        {
            float target = RestaurantManager.restaurantTimer + delayTime;
            while (RestaurantManager.restaurantTimer < target)
            {
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }
        }
        catch
        {
            throw;
        }
    }

    public static IEnumerator CustomCoroutineDelay(float delayTime)
    {
        float target = RestaurantManager.restaurantTimer + delayTime;
        while (RestaurantManager.restaurantTimer < target)
        {
            yield return null;
        }
    }

    public static bool RandomSuccess(int target)
    {
        int r = Random.Range(0, 100);
        if(target >= r)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

public class GameRegulation
{
    public int id;
    public string map_name;
    // 옵션 설정들 추가

    //
    public GameRegulation(int id, string map_name)
    {
        this.id = id;
        this.map_name = map_name;
    }
}


public class RestaurantData
{
    public int id;
    public string level_name;
    public int extension_level;
    public long latest_time;
    public bool changed;

    public RestaurantData(int id, string level_name, int extension_level, long latest_time)
    {
        this.id = id;
        this.level_name = level_name;
        this.extension_level = extension_level;
        this.latest_time = latest_time;
    }
}


public class RestaurantCurrency
{
    public string money;
    public int fishes;
    public int affinity;
    public int reputation;
    public int leftover;
    public int sale_num;
    public int minigameStack;
    public bool changed;

    BigInteger? m;
    public BigInteger Money { get { if (!m.HasValue) m = BigInteger.Parse(money); return m.Value; } set { m = value; changed = true; } }

    public RestaurantCurrency(string money, int fishes, int affinity, int reputation, int leftover, int sale_num, int minigameStack)
    {
        this.money = money;
        this.fishes = fishes;
        this.affinity = affinity;
        this.reputation = reputation;
        this.leftover = leftover;
        this.sale_num = sale_num;
        this.minigameStack = minigameStack;
    }
}

public class Employees
{
    public int num;
    public List<EmployeeLevelData> employeeLevelDatas;
    public bool changed;
    public Employees(int num, List<EmployeeLevelData> employeeLevelDatas)
    {
        this.employeeLevelDatas = employeeLevelDatas;
        this.num = num;
    }
}

public class EmployeeLevelData
{
    public int level;
    public int exp;
    public int targetEXP;
    public float speed;
    public int max_weight;
    public EmployeeLevelData(int level, int exp, int targetEXP)
    {
        this.level = level;
        this.exp = exp;
        this.targetEXP = targetEXP;
    }
}

public class TrashData
{
    public int trashNum;
    public int trashPoint;
    public bool changed;

    public TrashData(int trashNum, int trashPoint)
    {
        this.trashNum = trashNum;
        this.trashPoint = trashPoint;
    }
}


public class VendingMachineData
{
    public string money;
    public long lastTime;
    public bool unlocked;
    BigInteger? m;
    public BigInteger Money { get { if (!m.HasValue) m = BigInteger.Parse(money); return m.Value; } set { m = value; changed = true; } }
    public bool changed;

    public VendingMachineData(string money,  bool unlocked, long lastTime)
    {
        this.money = money;
        this.unlocked = unlocked;
        this.lastTime = lastTime;
    }
}

public class MachineLevelData : ITableID<int>
{
    public int id;
    public int level;
    public string price;
    public string sale_proceed;
    public float cooking_time;
    public int max_height;
    public int type;

    public int fishes;
    public string calculatedPrice;
    public BigInteger calculatedSales;
    public float calculatedCookingTimer;
    public int calculatedHeight;

    public string Price { get { return price; } set { price = value; price_value = null; } }
    BigInteger? price_value;
    public BigInteger Price_Value { get { if (!price_value.HasValue) price_value = Utility.StringToBigInteger(price); return price_value.Value; } set { price_value = value; } }
    public int ID => type;

    public string Name => throw new System.NotImplementedException();

    public MachineLevelData(int id, int level, string price, string sale_proceed, float cooking_time, int max_height, int type, int fishes)
    {
        this.id = id;
        this.level = level;
        this.price = price;
        this.sale_proceed = sale_proceed;
        this.cooking_time = cooking_time;
        this.max_height = max_height;
        this.type = type;
        this.fishes = fishes;
    }
}

public class MiniGame
{
    public MiniGameType type;
    public bool activate;
    public bool changed;
    public Fishing fishing;
    public MiniGame(MiniGameType type, bool activate)
    {
        this.type = type;
        this.activate = activate;
    }
}

public class Fishing
{
    public int fishNum;
    public bool isDirty;
    public Fishing(int fishNum, bool isDirty)
    {
        this.fishNum = fishNum;
        this.isDirty = isDirty;
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

