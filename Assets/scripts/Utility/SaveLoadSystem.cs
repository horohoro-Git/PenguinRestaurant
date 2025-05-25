using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class SaveLoadSystem
{
    static string path = Application.persistentDataPath;

    public static Hash128 ComputeHash128(byte[] rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] hashBytes = sha256Hash.ComputeHash(rawData);

            // SHA256 해시값을 Hash128으로 변환 (32바이트 -> 16바이트로 잘라서 Hash128으로 만들기)
            byte[] hash128Bytes = new byte[16];
            System.Array.Copy(hashBytes, hash128Bytes, 16); // 앞 16바이트만 사용

            return new Hash128(hash128Bytes[0], hash128Bytes[1], hash128Bytes[2], hash128Bytes[3]);
        }
    }

    //서버 불러오기
    public static string LoadServerURL()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "servercommunication.bytes");

        string content = File.ReadAllText(path);

        string decryptedData = EncryptorDecryptor.Decyptor(content, "AAA");
        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(decryptedData);
        string returnURL = data["server"].ToString();

        return returnURL;

    }



    public static List<T> GetListData<T>(string content)
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        return JsonConvert.DeserializeObject<List<T>>(data);
    }

    public static Dictionary<K, V> GetDictionaryData<K, V>(string content) where V : struct, ITableID<K>
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        List<V> vList = JsonConvert.DeserializeObject<List<V>>(data);

        Dictionary<K, V> d = new Dictionary<K, V>();
        foreach (var item in vList)
        {
       //     Debug.Log(item.ID);
            d.Add(item.ID, item);
        }
        return d;
    }

    public static Dictionary<K, V> GetDictionaryDataClass<K, V>(string content) where V : class, ITableID<K>
    {
        string data = EncryptorDecryptor.Decyptor(content, "AAA");

        List<V> vList = JsonConvert.DeserializeObject<List<V>>(data);

        Dictionary<K, V> d = new Dictionary<K, V>();
        foreach (var item in vList)
        {
    //        Debug.Log(item.ID);
            d.Add(item.ID, item);
        }
        return d;
    }


    public static Dictionary<int, int> LoadTier()
    {
        string p = Path.Combine(path, "Save/Tiers.dat");
        byte[] data;
        Dictionary<int, int> tierDic = new Dictionary<int, int>();

        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                if (data.Length > 0)
                {
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                    {
                        int key = reader.ReadInt32();
                        int value = reader.ReadInt32();

                        tierDic[key] = value;
                    }
                }
            }
        }
        else
        {
            tierDic[100] = 1;
        }
        return tierDic; 
    }

    //동물 데이터 로드
    public static Dictionary<int, AnimalStruct> LoadAnimalsData()
    {
        string p = Path.Combine(path, "Save/Animals.dat");
        byte[] data;
        Dictionary<int, AnimalStruct> animalsDic = new Dictionary<int, AnimalStruct>();

        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                if (data.Length > 0)
                {
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                    {
                        int id = reader.ReadInt32();
                        string name = reader.ReadString();
                        string asset_name = reader.ReadString();
                        int tier = reader.ReadInt32();
                        float speed = reader.ReadSingle();
                        float eat_speed = reader.ReadSingle();
                        int min_order = reader.ReadInt32();
                        int max_order = reader.ReadInt32();
                        bool is_customer = reader.ReadBoolean();
                        float size_width = reader.ReadSingle();
                        float size_height = reader.ReadSingle();
                        float offset_x = reader.ReadSingle();
                        float offset_z = reader.ReadSingle();
                        animalsDic[id] = new AnimalStruct(id, name, asset_name, tier, speed, eat_speed, min_order, max_order, is_customer, size_width, size_height, offset_x, offset_z);
                    }
                }
            }
        }
        else
        {
            // AnimalStruct animalStruct = AssetLoader.animals[100];
            animalsDic[10] = AssetLoader.animals[10];
            animalsDic[100] = AssetLoader.animals[100];
          //  Debug.Log("No player data found. Creating new data." + animalStruct.asset_name);
        }

        return animalsDic;
    }

    //동물 데이터 저장
    public static void SaveAnimalsData(Dictionary<int, AnimalStruct> animals)
    {
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string p = Path.Combine(path, "Save/Animals.dat");
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                foreach (KeyValuePair<int, AnimalStruct> animal in animals)
                {
                    int id = animal.Value.id;
                    string name = animal.Value.name;
                    string asset_name = animal.Value.asset_name;
                    int tier = animal.Value.tier;
                    float speed = animal.Value.speed;
                    float eat_speed = animal.Value.eat_speed;
                    int min_order = animal.Value.min_order;
                    int max_order = animal.Value.max_order;
                    bool is_customer = animal.Value.is_customer;
                    float size_width = animal.Value.size_width;
                    float size_height = animal.Value.size_height;
                    float offset_x = animal.Value.offset_x;
                    float offset_z = animal.Value.offset_z;

                    writer.Write(id);
                    writer.Write(name);
                    writer.Write(asset_name);
                    writer.Write(tier);
                    writer.Write(speed);
                    writer.Write(eat_speed);
                    writer.Write(min_order);
                    writer.Write(max_order);
                    writer.Write(is_customer);
                    writer.Write(size_width);
                    writer.Write(size_height);
                    writer.Write(offset_x);
                    writer.Write(offset_z);
                }
            }
        }
    }


    //레스토랑 건물 로드
    public static List<RestaurantParam> LoadRestaurantData()
    {
        string p = Path.Combine(path, "Save/Restaurant.dat");
        byte[] data;
        List<RestaurantParam> restaurantParams = new List<RestaurantParam>();
       // restaurantParams.AddRange(AssetLoader.restaurantParams.Select(stat => new RestaurantParam(stat)));

        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);

                using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                {
                    int counterNum = reader.ReadInt32();
                    for(int i = 0; i < counterNum;i++)
                    {
                        int id = reader.ReadInt32();
                        WorkSpaceType type = (WorkSpaceType)reader.ReadInt32();
                        int level = reader.ReadInt32();
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        float xx = reader.ReadSingle();
                        float yy = reader.ReadSingle();
                        float zz = reader.ReadSingle();
                        float rx = reader.ReadSingle();
                        float ry = reader.ReadSingle();
                        float rz = reader.ReadSingle();
                        float rw = reader.ReadSingle();
                        restaurantParams.Add(new RestaurantParam(id, type, level, new Vector3(x, y, z), new Vector3(xx,yy,zz), new Quaternion(rx, ry, rz, rw)));
                    }
                    int foodMachineNum = reader.ReadInt32();
                    for (int i = 0; i < foodMachineNum; i++)
                    {
                        int id = reader.ReadInt32();
                        WorkSpaceType type = (WorkSpaceType)reader.ReadInt32();
                        int level = reader.ReadInt32();
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        float rx = reader.ReadSingle();
                        float ry = reader.ReadSingle();
                        float rz = reader.ReadSingle();
                        float rw = reader.ReadSingle();
                        restaurantParams.Add(new RestaurantParam(id, type, level, new Vector3(x, y, z), Vector3.zero, new Quaternion(rx, ry, rz, rw)));
                    }
                    int tableNum = reader.ReadInt32();
                    for (int i = 0; i < tableNum; i++)
                    {
                        int id = reader.ReadInt32();
                        WorkSpaceType type = (WorkSpaceType)reader.ReadInt32();
                        int level = reader.ReadInt32();
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        float rx = reader.ReadSingle();
                        float ry = reader.ReadSingle();
                        float rz = reader.ReadSingle();
                        float rw = reader.ReadSingle();
                        restaurantParams.Add(new RestaurantParam(id, type, level, new Vector3(x, y, z), Vector3.zero, new Quaternion(rx, ry, rz, rw)));
                    }
                    int trashcCanNum = reader.ReadInt32();
                    for (int i = 0; i < trashcCanNum; i++)
                    {
                        int id = reader.ReadInt32();
                        WorkSpaceType type = (WorkSpaceType)reader.ReadInt32();
                        int level = reader.ReadInt32();
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        float xx = reader.ReadSingle();
                        float yy = reader.ReadSingle();
                        float zz = reader.ReadSingle();
                        float rx = reader.ReadSingle();
                        float ry = reader.ReadSingle();
                        float rz = reader.ReadSingle();
                        float rw = reader.ReadSingle();
                        restaurantParams.Add(new RestaurantParam(id, type, level, new Vector3(x, y, z), new Vector3(xx,yy,zz), new Quaternion(rx, ry, rz, rw)));
                    }
                }
            }
        }
        else
        {
            SaveRestaurantData();
        }

        return restaurantParams;
    }

    //레스토랑 건물 저장
    public static void SaveRestaurantData()
    {
        WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string p = Path.Combine(path, "Save/Restaurant.dat");
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                int counterNum = workSpaceManager.counters.Count;
                writer.Write(counterNum);
                for (int i = 0; i < counterNum; i++)
                {
                    writer.Write(workSpaceManager.counters[i].id);
                    writer.Write((int)workSpaceManager.counters[i].spaceType);
                    writer.Write((int)workSpaceManager.counters[i].rotateLevel);
                    writer.Write(workSpaceManager.counters[i].transforms.position.x);
                    writer.Write(workSpaceManager.counters[i].transforms.position.y);
                    writer.Write(workSpaceManager.counters[i].transforms.position.z);
                    writer.Write(workSpaceManager.counters[i].offset.localPosition.x);
                    writer.Write(workSpaceManager.counters[i].offset.localPosition.y);
                    writer.Write(workSpaceManager.counters[i].offset.localPosition.z);
                    writer.Write(workSpaceManager.counters[i].offset.localRotation.x);
                    writer.Write(workSpaceManager.counters[i].offset.rotation.y);
                    writer.Write(workSpaceManager.counters[i].offset.rotation.z);
                    writer.Write(workSpaceManager.counters[i].offset.rotation.w);

                    
                }
                int foodMachineNum = workSpaceManager.foodMachines.Count;
                writer.Write(foodMachineNum);
                for (int i = 0; i < foodMachineNum; i++)
                {
                    writer.Write(workSpaceManager.foodMachines[i].id);
                    writer.Write((int)workSpaceManager.foodMachines[i].spaceType);
                    writer.Write((int)workSpaceManager.foodMachines[i].rotateLevel);
                    writer.Write(workSpaceManager.foodMachines[i].transforms.position.x);
                    writer.Write(workSpaceManager.foodMachines[i].transforms.position.y);
                    writer.Write(workSpaceManager.foodMachines[i].transforms.position.z);
                    writer.Write(workSpaceManager.foodMachines[i].transforms.rotation.x);
                    writer.Write(workSpaceManager.foodMachines[i].transforms.rotation.y);
                    writer.Write(workSpaceManager.foodMachines[i].transforms.rotation.z);
                    writer.Write(workSpaceManager.foodMachines[i].transforms.rotation.w);
                }
                int tableNum = workSpaceManager.tables.Count;
                writer.Write(tableNum);
                for (int i = 0; i < tableNum; i++)
                {
                    writer.Write(workSpaceManager.tables[i].id);
                    writer.Write((int)workSpaceManager.tables[i].spaceType);
                    writer.Write((int)workSpaceManager.tables[i].rotateLevel);
                    writer.Write(workSpaceManager.tables[i].transforms.position.x);
                    writer.Write(workSpaceManager.tables[i].transforms.position.y);
                    writer.Write(workSpaceManager.tables[i].transforms.position.z);
                    writer.Write(workSpaceManager.tables[i].transforms.rotation.x);
                    writer.Write(workSpaceManager.tables[i].transforms.rotation.y);
                    writer.Write(workSpaceManager.tables[i].transforms.rotation.z);
                    writer.Write(workSpaceManager.tables[i].transforms.rotation.w);
                }
                int trashcanNum = workSpaceManager.trashCans.Count;
                writer.Write(trashcanNum);
                for (int i = 0; i < trashcanNum; i++)
                {
                    writer.Write(workSpaceManager.trashCans[i].id);
                    writer.Write((int)workSpaceManager.trashCans[i].spaceType);
                    writer.Write((int)workSpaceManager.trashCans[i].rotateLevel);
                    writer.Write(workSpaceManager.trashCans[i].transforms.position.x);
                    writer.Write(workSpaceManager.trashCans[i].transforms.position.y);
                    writer.Write(workSpaceManager.trashCans[i].transforms.position.z);
                    writer.Write(workSpaceManager.trashCans[i].offset.localPosition.x);
                    writer.Write(workSpaceManager.trashCans[i].offset.localPosition.y);
                    writer.Write(workSpaceManager.trashCans[i].offset.localPosition.z);
                    writer.Write(workSpaceManager.trashCans[i].offset.localRotation.x);
                    writer.Write(workSpaceManager.trashCans[i].offset.localRotation.y);
                    writer.Write(workSpaceManager.trashCans[i].offset.localRotation.z);
                    writer.Write(workSpaceManager.trashCans[i].offset.localRotation.w);
                }
            }

            File.WriteAllBytes(p, ms.ToArray());
        }
    }

    public static void SaveRestaurantCurrency(RestaurantCurrency currency)
    {
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string p = Path.Combine(path, "Save/RestaurantCurrency.dat");
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write(currency.money);
                writer.Write(currency.fishes);
                writer.Write(currency.affinity);
                writer.Write(currency.extension_level);
            }

            File.WriteAllBytes(p, ms.ToArray());
        }
    }

    public static RestaurantCurrency LoadRestaurantCurrency()
    {
        RestaurantCurrency currency = null;
        string p = Path.Combine(path, "Save/RestaurantCurrency.dat");
        byte[] data;
        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);

                using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                {
                    int money = reader.ReadInt32();
                    int fishes = reader.ReadInt32();
                    int affinity = reader.ReadInt32();
                    int extension_level = reader.ReadInt32();   

                    currency = new RestaurantCurrency(money, fishes, affinity, extension_level);
                }
            }
        }
        else
        {
            currency = new RestaurantCurrency(500, 0, 0, 0);
            SaveRestaurantCurrency(currency);
        }
        return currency;
    }

    public static Employees LoadEmployees()
    {
        Employees employees = null;
        string p = Path.Combine(path, "Save/Employees.dat");
        byte[] data;
        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);

                using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                {
                    int num = reader.ReadInt32();

                    employees = new Employees(new List<EmployeeLevelData>());

                    //List<EmployeeLevelData> employeeLevelDatas = new List<EmployeeLevelData>();
                    for(int i=0; i<num; i++)
                    {
                        employees.employeeLevelDatas.Add(new EmployeeLevelData(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()));
                    }
                //    employees = new Employees(employeeLevelDatas);
                }
            }
        }
        else
        {
            employees = new Employees(new List<EmployeeLevelData>());
            SaveEmployees(employees);
        }
        return employees;
    }

    public static void SaveEmployees(Employees employees)
    {
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string p = Path.Combine(path, "Save/Employees.dat");
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                int num = employees.employeeLevelDatas.Count;
                writer.Write(num);

                for (int i = 0; i < num; i++)
                {
                    writer.Write(employees.employeeLevelDatas[i].level);
                    writer.Write(employees.employeeLevelDatas[i].exp);
                    writer.Write(employees.employeeLevelDatas[i].targetEXP);
                }
              
            }

            File.WriteAllBytes(p, ms.ToArray());
        }
    }


    public static Dictionary<int, LevelData> LoadLevelData()
    {
        string p = Path.Combine(path, "Save/Level.dat");
        byte[] data;
        Dictionary<int, LevelData> levelDatas = new Dictionary<int, LevelData>();

        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                if (data.Length > 0)
                {
                    using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                    {

                    }
                }
            }
        }
        else
        {
            levelDatas = AssetLoader.levelData;
        }

        return levelDatas;
    }
}
         
