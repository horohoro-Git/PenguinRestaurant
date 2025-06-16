using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

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
    public static async Task<string> LoadServerURL()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "servercommunication.bytes");

        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();  // 비동기 대기

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load server URL: {www.error}");
                return null;
            }

            string content = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            string decryptedData = EncryptorDecryptor.Decyptor(content, "AAA");
            Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(decryptedData);
            string returnURL = data["server"].ToString();

            return returnURL;
        }
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


    public static void SaveGameRegulation(GameRegulation gameRegulation)
    {
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        string p = Path.Combine(path, "Save/Regulation.dat");
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write(gameRegulation.id);
                writer.Write(gameRegulation.map_name);
            }
        }
    }

    public static GameRegulation LoadGameRegulation()
    {
        GameRegulation regulation = null;
        string p = Path.Combine(path, "Save/Regulation.dat");
        byte[] data;
        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);

                using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                {
                    int id = reader.ReadInt32();
                    string map_name = reader.ReadString();

                    regulation = new GameRegulation(id, map_name);

                }
            }
        }
        else
        {
            regulation = new GameRegulation(0, "town_01");
            SaveGameRegulation(regulation);
        }
        return regulation;
    }

    //레스토랑 저장
    public static void SaveRestaurantData(RestaurantData restaurantData)
    {
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
                writer.Write(restaurantData.id);
                writer.Write(restaurantData.level_name);
                writer.Write(restaurantData.extension_level);
                writer.Write(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
             
                File.WriteAllBytes(p, ms.ToArray());
                
            }
        }
    }

    //레스토랑 로드
    public static RestaurantData LoadRestaurantData()
    {
        RestaurantData restaurant = null;
        string p = Path.Combine(path, "Save/Restaurant.dat");
        byte[] data;
        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);

                using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                {
                    //BigInteger money = BigInteger.Parse(reader.ReadString());
                    int id = reader.ReadInt32();
                    string level_name = reader.ReadString();
                    int extension_level = reader.ReadInt32();
              
                    restaurant = new RestaurantData(id, level_name, extension_level, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                }
            }
        }
        else
        {
            restaurant = new RestaurantData(0, "", 0, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            SaveRestaurantData(restaurant);
        }
        return restaurant;
    }

    //뽑기 동물 데이터 저장
    public static void SaveGatchaAnimalsData()
    {
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string p = Path.Combine(path, "Save/GatchaAnimals.dat");
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                Dictionary<int, int> animals = AnimalManager.gatchaTiers;
                foreach (var animal in animals)
                {
                    writer.Write(animal.Key);   //동물 타입
                    writer.Write(animal.Value); //동물 등급
                }
            }
            File.WriteAllBytes(p, ms.ToArray());
        }
    }

    //뽑기 동물 데이터 로드
    public static Dictionary<int, int> LoadGatchaAnimals()
    {
        Dictionary<int, int> animals = new Dictionary<int, int>();
        string p = Path.Combine(path, "Save/GatchaAnimals.dat");
        byte[] data;
        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);

                using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        int type = reader.ReadInt32();
                        int grade = reader.ReadInt32();
                        animals[type] = grade;
                    }
                }
            }
        }
        else
        {
            animals[100] = 1;
        }
        return animals;
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
    public static List<RestaurantParam> LoadRestaurantBuildingData()
    {
        string p = Path.Combine(path, "Save/RestaurantBuilding.dat");
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
                        restaurantParams.Add(new RestaurantParam(id, type, level, new UnityEngine.Vector3(x, y, z), new Vector3(xx,yy,zz), new Quaternion(rx, ry, rz, rw)));
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

                    bool hasDoor = reader.ReadBoolean();
                    if(hasDoor)
                    {
                        Door door = GameInstance.GameIns.restaurantManager.door;
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        float rx = reader.ReadSingle();
                        float ry = reader.ReadSingle();
                        float rz = reader.ReadSingle();
                        float rw = reader.ReadSingle();
                        Vector3 pos = new Vector3(x, y, z);
                        Quaternion rot = new Quaternion(rx, ry, rz, rw);
                        door.transform.position = pos;
                        door.transform.rotation = rot;
                        door.setup = true;

                    }
                }
            }
        }
        else
        {
            SaveRestaurantBuildingData();
        }

        return restaurantParams;
    }

    //레스토랑 건물 저장
    public static void SaveRestaurantBuildingData()
    {
        WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string p = Path.Combine(path, "Save/RestaurantBuilding.dat");
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

                if(GameInstance.GameIns.restaurantManager.door != null)
                {
                  
                    Door door = GameInstance.GameIns.restaurantManager.door;
                    if(door.setup)
                    {
                        writer.Write(true);
                        Vector3 pos = door.transform.position;
                        Quaternion rot = door.transform.rotation;
                        writer.Write(pos.x);
                        writer.Write(pos.y);
                        writer.Write(pos.z);
                        writer.Write(rot.x);
                        writer.Write(rot.y);
                        writer.Write(rot.z);
                        writer.Write(rot.w);
                    }
                    else
                    {
                        writer.Write(false);
                    }
                }
                else
                {
                    writer.Write(false);
                }
            }

            File.WriteAllBytes(p, ms.ToArray());
        }
    }
    public static Dictionary<MachineType, MachineLevelData> LoadFoodMachineStats()
    {
        string p = Path.Combine(path, "Save/RestaurantMachines.dat");
        byte[] data;
        Dictionary<MachineType, MachineLevelData> machineStats = new Dictionary<MachineType, MachineLevelData>();
        // restaurantParams.AddRange(AssetLoader.restaurantParams.Select(stat => new RestaurantParam(stat)));

        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);

                using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        int id = reader.ReadInt32();
                        int level = reader.ReadInt32();
                        string price = reader.ReadString();
                        int sale_proceed = reader.ReadInt32();
                        float cooking_time = reader.ReadSingle();
                        int max_height = reader.ReadInt32();
                        int type = reader.ReadInt32();
                        int fishes = reader.ReadInt32();    
                        if(!machineStats.ContainsKey((MachineType)type)) machineStats[(MachineType)type] = new MachineLevelData(id, level, price, sale_proceed, cooking_time, max_height, type, fishes);
                    }
                }
            }
        }
      
        return machineStats;
    }
    public static void SaveFoodMachineStats(Dictionary<MachineType, MachineLevelData> machines)
    {
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string p = Path.Combine(path, "Save/RestaurantMachines.dat");
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;
                foreach (var machine in machines)
                {
                    int id = machine.Value.id;
                    int level = machine.Value.level;
                    string price = machine.Value.price;
                    int sale_proceed = machine.Value.sale_proceed;
                    float cooking_time = machine.Value.cooking_time;
                    int max_height = machine.Value.max_height;
                    int type = (int)machine.Key;
                    int fishes = machine.Value.fishes;
                    for(int i = 0; i < workSpaceManager.counters.Count; i++)
                    {
                        List<FoodStack> foodStacks = workSpaceManager.counters[i].foodStacks;
                        for(int j = 0; j < foodStacks.Count; j++)
                        {
                            if(foodStacks[j].type == machine.Key)
                            {
                                fishes += foodStacks[j].foodStack.Count;
                            }
                        }
                    }

                    for(int i = 0; i < workSpaceManager.foodMachines.Count; i++)
                    {
                        FoodMachine fm = workSpaceManager.foodMachines[i];
                        if(fm.machineType == machine.Key)
                        {
                            fishes += fm.foodStack.foodStack.Count;
                        }
                    }

                    AnimalManager am = GameInstance.GameIns.animalManager;
                    for (int i = 0; i < am.employeeControllers.Count; i++)
                    {
                        Employee employee = am.employeeControllers[i];
                        fishes += employee.foodStacks[machine.Key].foodStack.Count;
                    }


                    writer.Write(id);
                    writer.Write(level);
                    writer.Write(price);
                    writer.Write(sale_proceed);
                    writer.Write(cooking_time);
                    writer.Write(max_height);
                    writer.Write(type);
                    writer.Write(fishes);
                  
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
                Debug.Log(currency.Money.ToString());
                writer.Write(currency.Money.ToString());
                writer.Write(currency.fishes);
                writer.Write(currency.affinity);
                writer.Write(currency.sale_num);
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
                    //BigInteger money = BigInteger.Parse(reader.ReadString());
                    string money = reader.ReadString();
                    int fishes = reader.ReadInt32();
                    int affinity = reader.ReadInt32();
                    int sale_num = reader.ReadInt32();
                    currency = new RestaurantCurrency(money, fishes, affinity, sale_num);
                }
            }
        }
        else
        {
            currency = new RestaurantCurrency("500", 0, 0, 0);
            SaveRestaurantCurrency(currency);
        }
        return currency;
    }
    public static void SaveVendingMachineData(VendingMachineData vendingData)
    {
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string p = Path.Combine(path, "Save/VendingData.dat");
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write(vendingData.Money.ToString());
                writer.Write(vendingData.unlocked);
            }

            File.WriteAllBytes(p, ms.ToArray());
        }
    }



    public static VendingMachineData LoadVendingMachineData()
    {
        VendingMachineData vending = null;
        string p = Path.Combine(path, "Save/VendingData.dat");
        byte[] data;
        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);

                using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                {
                    string money = reader.ReadString();
                    vending = new VendingMachineData(money, reader.ReadBoolean());
                }
            }
        }
        else
        {
            vending = new VendingMachineData("0", false);
            SaveVendingMachineData(vending);
        }
        return vending;
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


    public static void SaveMiniGameStatus(MiniGame miniGame)
    {
        string dir = Path.Combine(path, "Save");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string p = Path.Combine(path, "Save/m_Games.dat");
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                int type = (int)miniGame.type;
                bool activate = miniGame.activate;
                writer.Write(type);
                writer.Write(activate);


                switch (miniGame.type)
                {
                    case MiniGameType.None:
                        break;
                    case MiniGameType.Fishing:
                        Fishing fishing = miniGame.fishing;
                        writer.Write(fishing.fishNum);
                        writer.Write(fishing.isDirty);
                        break;
                }



            }

            File.WriteAllBytes(p, ms.ToArray());
        }
    }

    public static MiniGame LoadMiniGameStatus()
    {
        string p = Path.Combine(path, "Save/m_Games.dat");
        byte[] data;
       
        MiniGame miniGame = null;
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

                        MiniGameType type = (MiniGameType)reader.ReadInt32();
                        bool activate = reader.ReadBoolean();
                        miniGame = new MiniGame(type, activate);

                        switch (type)
                        {
                            case MiniGameType.None:
                                break;
                            case MiniGameType.Fishing:
                                int fishNum = reader.ReadInt32();
                                bool isDirty = reader.ReadBoolean();
                                Fishing fishing = new Fishing(fishNum, isDirty);
                                miniGame.fishing = fishing;
                                break;
                        }
                    }
                }
            }
        }
        else
        {
             miniGame = new MiniGame(MiniGameType.None, false);
        }


        return miniGame;
    }
}
         
