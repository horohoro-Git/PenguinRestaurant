using Newtonsoft.Json;
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
            Debug.Log(item.ID);
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
            Debug.Log(item.ID);
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

                        animalsDic[id] = new AnimalStruct(id, name, asset_name, tier, speed, eat_speed, min_order, max_order, is_customer);
                    }
                }
            }
        }
        else
        {
            AnimalStruct animalStruct = AssetLoader.animals[100];
            animalsDic[100] = animalStruct;
            Debug.Log("No player data found. Creating new data." + animalStruct.asset_name);
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

                    writer.Write(id);
                    writer.Write(name);
                    writer.Write(asset_name);
                    writer.Write(tier);
                    writer.Write(speed);
                    writer.Write(eat_speed);
                    writer.Write(min_order);
                    writer.Write(max_order);
                    writer.Write(is_customer);
                }
            }
        }
    }


    //레스토랑 로드
    public static List<RestaurantParam> LoadRestaurantData()
    {
        string p = Path.Combine(path, "Save/Restaurant.dat");
        byte[] data;
        List<RestaurantParam> restaurantParams = new List<RestaurantParam>();
        restaurantParams.AddRange(AssetLoader.restaurantParams.Select(stat => new RestaurantParam(stat)));

        if (File.Exists(p))
        {
            using (FileStream fs = new FileStream(p, FileMode.Open))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);

                using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
                {
                    for (int i = 0; i < restaurantParams.Count; i++)
                    {
                        if (data.Length > 0)
                        {
                            bool unlock = reader.ReadBoolean();
                            if(unlock) restaurantParams[i].unlock = unlock;

                        }
                    }
                }
            }
        }
        else
        {
            
            Debug.Log("No player data found. Creating new data.");
        }

        return restaurantParams;
    }

    //레스토랑 저장
    public static void SaveRestaurantData(List<RestaurantParam> restaurantParams)
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
                for (int i = 0; i < restaurantParams.Count; i++)
                {
                //    int id = restaurantParams[i].id;
                 //   int type = restaurantParams[i].type;
                    bool unlock = restaurantParams[i].unlock;
                ////    writer.Write(id);
                 //   writer.Write(type);
                    writer.Write(unlock);
                //    Debug.Log(id + " " + type + " " + unlock);
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
         
