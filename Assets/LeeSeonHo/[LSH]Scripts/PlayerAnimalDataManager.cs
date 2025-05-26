using System.Collections.Generic;
using System.IO; // ���� �����
using UnityEngine;

[System.Serializable]
public class HaveAnimal
{
    public int id;         // ���� ID
    public string name;    // ���� �̸�
    public int tier;      // ���� ���
    public float speed;
    public float eatSpeed;
    public int minOrder;
    public int maxOrder;
    public int likeFood;
    public int hateFood;
    public int buff;
}

[System.Serializable]
public class PlayerAnimalData
{
    public List<HaveAnimal> animals = new List<HaveAnimal>(); // ���� ����Ʈ
}

public class PlayerAnimalDataManager : MonoBehaviour
{
    private string saveFilePath;
    public PlayerAnimalData playerAnimalData;
    public Dictionary<int, Animal> lockAnimals = new Dictionary<int, Animal>();

    public Dictionary<int, AnimalStruct> animals = new Dictionary<int, AnimalStruct>();

    private void Awake()
    {
        GameInstance.GameIns.playerAnimalDataManager = this;
      //  saveFilePath = Path.Combine(Application.persistentDataPath, "PlayerAnimalData.json");
        //LoadPlayerData();
     
        InitializeAnimalData(); // ���� �����͸� �ʱ�ȭ     
    }

    private void Start()
    {
        foreach (var animal in animals)
        {
            Debug.Log(animal.Value.name + "AAA");
            lockAnimals[animal.Key] = AssetLoader.loadedAssets[AssetLoader.itemAssetKeys[animal.Key].Name].GetComponent<Animal>();
        }
      //  PlayerAnimalUpdate(true,-1);
    }

    private void InitializeAnimalData()
    {
        // ���� �����Ͱ� ��� ���� ��� �ʱ�ȭ
        if (playerAnimalData.animals.Count == 0)
        {
            playerAnimalData.animals = new List<HaveAnimal>
            {
                new HaveAnimal { id = 0, name = "�����", tier = 1, speed = 5, eatSpeed = 5, minOrder = 2, maxOrder = 5, likeFood = 1, hateFood = 4, buff = 2},
                new HaveAnimal { id = 1, name = "���", tier = 0, speed = 5, eatSpeed = 3, minOrder = 1, maxOrder = 2, likeFood = 4, hateFood = 2, buff = 1},
                new HaveAnimal { id = 2, name = "������", tier = 0, speed = 6, eatSpeed = 6, minOrder = 4, maxOrder = 5, likeFood = 1, hateFood = 3, buff = 3},
                new HaveAnimal { id = 3, name = "�޹���", tier = 0, speed = 5, eatSpeed = 3, minOrder = 1, maxOrder = 2, likeFood = 3, hateFood = 1, buff = 1},
                new HaveAnimal { id = 4, name = "�䳢", tier = 0, speed = 10, eatSpeed = 3, minOrder = 1, maxOrder = 4, likeFood = 2, hateFood = 1, buff = 0},
                new HaveAnimal { id = 5, name = "�ź���", tier = 0, speed = 2, eatSpeed = 2, minOrder = 1, maxOrder = 4, likeFood = 2, hateFood = 3, buff = 0},
                new HaveAnimal { id = 6, name = "���Ƹ�", tier = 0, speed = 5, eatSpeed = 2, minOrder = 1, maxOrder = 2, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 7, name = "��", tier = 0, speed = 5, eatSpeed = 2, minOrder = 4, maxOrder = 6, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 8, name = "�糪��", tier = 0, speed = 5, eatSpeed = 2, minOrder = 2, maxOrder = 5, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 9, name = "����", tier = 0, speed = 5, eatSpeed = 2, minOrder = 1, maxOrder = 2, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 10, name = "�ڳ���", tier = 0, speed = 5, eatSpeed = 2, minOrder = 6, maxOrder = 8, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 11, name = "�ö�ְ�", tier = 0, speed = 5, eatSpeed = 2, minOrder = 1, maxOrder = 3, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 12, name = "����", tier = 0, speed = 5, eatSpeed = 2, minOrder = 2, maxOrder = 5, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 13, name = "����", tier = 0, speed = 5, eatSpeed = 2, minOrder = 2, maxOrder = 5, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 14, name = "��ż", tier = 0, speed = 5, eatSpeed = 2, minOrder = 1, maxOrder = 2, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 15, name = "�����", tier = 0, speed = 5, eatSpeed = 2, minOrder = 6, maxOrder = 8, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 16, name = "�ڻԻ�", tier = 0, speed = 5, eatSpeed = 2, minOrder = 1, maxOrder = 3, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 17, name = "���̿���", tier = 0, speed = 5, eatSpeed = 2, minOrder = 4, maxOrder = 6, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 18, name = "Ÿ��", tier = 0, speed = 5, eatSpeed = 2, minOrder = 2, maxOrder = 5, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 19, name = "��踻", tier = 0, speed = 5, eatSpeed = 2, minOrder = 2, maxOrder = 5, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 20, name = "�����", tier = 0, speed = 5, eatSpeed = 2, minOrder = 4, maxOrder = 6, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 21, name = "�û���", tier = 0, speed = 5, eatSpeed = 2, minOrder = 1, maxOrder = 2, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 22, name = "�ϱذ�", tier = 0, speed = 5, eatSpeed = 2, minOrder = 4, maxOrder = 6, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 23, name = "����", tier = 0, speed = 5, eatSpeed = 2, minOrder = 2, maxOrder = 5, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 24, name = "�ٴٻ���", tier = 0, speed = 5, eatSpeed = 2, minOrder = 2, maxOrder = 5, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 25, name = "��", tier = 0, speed = 5, eatSpeed = 2, minOrder = 1, maxOrder = 4, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 26, name = "�ٴ��ڳ���", tier = 0, speed = 5, eatSpeed = 2, minOrder = 1, maxOrder = 4, likeFood = 1, hateFood = 2, buff = 0},
                new HaveAnimal { id = 27, name = "����", tier = 0, speed = 5, eatSpeed = 2, minOrder = 4, maxOrder = 6, likeFood = 1, hateFood = 2, buff = 0}
            };

       //     SavePlayerData();
        }
    }

    public AnimalStruct AddAnimal(int animalId)
    {
        AnimalStruct animal = new AnimalStruct();
        if (animals.ContainsKey(animalId))
        {
            animal = animals[animalId]; //playerAnimalData.animals.Find(a => a.id == animalId);
            if (animal.tier < 4)
            {
                animal.tier++;

              /*  //���� ������ ���� �߰� ����(1: �̵� �ӵ�/ 2: �Դ� �ӵ�/ 3: �ֹ���
                if (animal.buff == 1)
                {
                    animal.speed++;
                }
                else if (animal.buff == 2)
                {
                    animal.eatSpeed++;
                }
                else if (animal.buff == 3)
                {
                    animal.minOrder++;
                    animal.maxOrder++;
                }*/

                animal.speed++;
                animal.eat_speed++;
                animal.min_order++;
                animal.max_order++;
                animals[animalId] = animal;
           //     SaveLoadSystem.SaveAnimalsData(animals);
            }
       //     SavePlayerData();
        }
        return animal;
    }

    public HaveAnimal PlayerAnimal(int animalId)
    {
        HaveAnimal animal = playerAnimalData.animals.Find(a => a.id == animalId);
        return animal;
    }

    private void SavePlayerData()
    {
        string json = JsonUtility.ToJson(playerAnimalData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Player data saved to {saveFilePath}");
    }

    private void LoadPlayerData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            playerAnimalData = JsonUtility.FromJson<PlayerAnimalData>(json);
            // Debug.Log("Player data loaded.");
        }
        else
        {
            playerAnimalData = new PlayerAnimalData(); // ������ �ʱ�ȭ
            Debug.Log("No player data found. Creating new data.");
        }
    }

    public void PlayerAnimalUpdate(bool bFirst, int id)
    {
        if (bFirst)
        {
     
            foreach(KeyValuePair<int, Animal> keyValuePair in lockAnimals)
            {
                AnimalStruct animal = animals[keyValuePair.Key];
                if (animal.tier > 0)
                {
                    keyValuePair.Value.speed = animal.speed;
                    keyValuePair.Value.eatSpeed = animal.eat_speed;
                    keyValuePair.Value.minOrder = animal.min_order;
                    keyValuePair.Value.maxOrder = animal.max_order;

                    //   Debug.Log(lockAnimals[i].id);
                    GameInstance.GameIns.animalManager.AddNewAnimal(lockAnimals[keyValuePair.Key], keyValuePair.Key, animal);
                }
            }
        }
        else
        {
            Debug.Log("ID" + id);
            AnimalStruct animal = animals[id];
            if (animal.tier > 0)
            {
                lockAnimals[id].speed = animal.speed;
                lockAnimals[id].eatSpeed = animal.eat_speed;
                lockAnimals[id].minOrder = animal.min_order;
                lockAnimals[id].maxOrder = animal.max_order;

                //   Debug.Log(lockAnimals[i].id);
                GameInstance.GameIns.animalManager.AddNewAnimal(lockAnimals[id], id, animal);
            }
        }
        
    }
}
