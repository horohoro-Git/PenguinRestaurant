using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GatcharManager : MonoBehaviour
{

    public MapType mapType;
    public GameObject[] gameObjects;
    public GameObject popup;
    public GameObject popup_NewCustomer;
    public GameObject popup_TierUp;
    public Image backGlow;

    public float price;
    public Sprite[] sprites;
    public Image NewAnimalImage;
    public Image TierUpAnimalImage;
    public bool isSpawning;
    public float spawnTerm = 0.2f;
    public float rollingSpeed = 10;

    private bool isFast;
    private int mapInt;
    private int x = 5;
    private List<GameObject> spawnedAnimals = new List<GameObject>();
    private List<int> spawnedAnimalTypes = new List<int>();

    Queue<Rolling> rollings = new Queue<Rolling>();

    private PlayerAnimalDataManager playerAnimalDataManager;
   
    private void Awake()
    {

        GameInstance.GameIns.gatcharManager = this;


        Debug.Log("GatchaScene Loaded");
    }
    private void OnEnable()
    {
        isSpawning = false;
        ClearAnimals();
    }
    private void Start()
    {
        GameInstance.GameIns.animalManager.NewGatchaAnimals();
        playerAnimalDataManager = GameInstance.GameIns.playerAnimalDataManager;

        if (mapType == MapType.town) mapInt = 0;
        else if (mapType == MapType.forest) mapInt = 6;
        else if (mapType == MapType.winter) mapInt = 12;
    }

    public void StartGatcha()
    {
        if (isSpawning) return;
        if (!Purchase()) return;
        popup_NewCustomer.SetActive(false);
        popup_TierUp.SetActive(false);
        popup.SetActive(false);
        isSpawning = true;
        ClearRollings();
        x = 5;
        StartCoroutine(SpawnRollingsWithDelay());
    }

    public bool Purchase()
    {
        if (GameInstance.GameIns.restaurantManager.playerData.money >= price)
        {
            GameInstance.GameIns.restaurantManager.playerData.money -= price;
            GameInstance.GameIns.uiManager.UpdateMoneyText(GameInstance.GameIns.restaurantManager.playerData.money);
           // SaveLoadManager.Save(SaveLoadManager.SaveState.ONLY_SAVE_PLAYERDATA);
            return true;
        }
        return false;
    }

    IEnumerator SpawnRollingsWithDelay()
    {
        for (int i = 0; i < 3; i++)
        {
            SpawnRolling();
            yield return new WaitForSecondsRealtime(spawnTerm);
        }

        yield return StartCoroutine(WaitRollingsReachTarget());
        CheckGatchaAnimals();
        isSpawning = false;
    }

    IEnumerator WaitRollingsReachTarget()
    {
        while (true)
        {
            bool allReached = true;

            foreach (Rolling animal in rollings)
            {
                if (animal != null && animal.IsMoving() == true)
                {
                    allReached = false;
                    break;
                }
            }

            if (allReached) break;

            yield return null;
        }
    }

    void SpawnRolling()
    {
        Rolling rolling = GameInstance.GameIns.animalManager.GetGatchaAnimal(MapType.town);
        rollings.Enqueue(rolling);
        rolling.transform.position = new Vector3(x, 0, -980);
        rolling.transform.rotation = Quaternion.Euler(0, 180, 0);
        x -= 5;
        rolling.Roll();
    }

    void CheckGatchaAnimals()
    {
        Dictionary<int, int> animalCounts = new Dictionary<int, int>();
        foreach (Rolling roll in rollings)
        {
            if(animalCounts.ContainsKey(roll.type))
            {
                animalCounts[roll.type]++;

            }
            else
            {
                animalCounts[roll.type] = 1;
            }
        }
        foreach (var pair in animalCounts)
        {
            if(pair.Value == 1)
            {
                popup.SetActive(true);
                if(AnimalManager.gatchaTiers.ContainsKey(pair.Key))
                {
                    if(AnimalManager.gatchaTiers[pair.Key] < 4) AnimalManager.gatchaTiers[pair.Key]++;
                    int tier = AnimalManager.gatchaTiers[pair.Key];
                    if (tier >= 2 && tier < 5)
                    {
                        popup_TierUp.SetActive(true);

                        AnimalStruct asset = AssetLoader.animals[pair.Key];
                        string n = asset.asset_name + "_Sprite";
                        TierUpAnimalImage.sprite = AssetLoader.loadedSprites[n];   //this.sprites[pair.Key];

                        if (tier == 2) backGlow.color = Color.blue;
                        else if (tier == 3) backGlow.color = new Color(0.5f, 0f, 0.5f);
                        else if (tier == 4) backGlow.color = Color.yellow;
                    }
                }
                else
                {
                    AnimalManager.gatchaTiers[pair.Key] = 1;
                    AnimalStruct asset = AssetLoader.animals[pair.Key];
                    string n = asset.asset_name + "_Sprite";
                    NewAnimalImage.sprite = AssetLoader.loadedSprites[n];// this.sprites[pair.Key];
                    popup_NewCustomer.SetActive(true);
                    AnimalManager.animalStructs[pair.Key] = asset;
                  //  GameInstance.GameIns.animalManager.AddNewAnimal(lockAnimals[keyValuePair.Key], keyValuePair.Key, animal);
                }
            
                break;
            }
        }
    }

    public void SpownAnimal()
    {
        int i = Random.Range(0, gameObjects.Length);
        //i = 5;
        GameObject animal = Instantiate(gameObjects[i], new Vector3(x, 0, -980), Quaternion.Euler(0, 180, 0));
        spawnedAnimals.Add(animal);
     //   animal[]
        spawnedAnimalTypes.Add(i);
        x -= 5;
    }

    public void SpownAnimal3()
    {
        if (isSpawning) return;

        if(!Purchase()) return;
        popup_NewCustomer.SetActive(false);
        popup_TierUp.SetActive(false);
        popup.SetActive(false);

        isSpawning = true;
        ClearAnimals();

        x = 5;
        StartCoroutine(SpownAnimalsWithDelay());
    }

    private IEnumerator SpownAnimalsWithDelay()
    {
        for (int i = 0; i < 3; i++)
        {
            SpownAnimal();
         //   yield return GetWaitTimer.WaitTimer.GetTimer(200); 
            yield return new WaitForSecondsRealtime(spawnTerm);
          
        }

        yield return StartCoroutine(WaitUntilAnimalsReachTarget());
        CheckForDuplicateAnimals();
        isSpawning = false;
    }




    private IEnumerator WaitUntilAnimalsReachTarget()
    {
        while (true)
        {
            bool allReached = true;

            foreach (GameObject animal in spawnedAnimals)
            {
                if (animal != null && animal.GetComponent<Rolling>()?.IsMoving() == true)
                {
                    allReached = false;
                    break;
                }
            }

            if (allReached) break;

            yield return null;
        }
    }

    private void CheckForDuplicateAnimals()
    {
        Dictionary<int, int> animalCounts = new Dictionary<int, int>();

        foreach (int type in spawnedAnimalTypes)
        {
            if (animalCounts.ContainsKey(type))
            {
                animalCounts[type]++;
            }
            else
            {
                animalCounts[type] = 1;
            }
        }

        foreach (var pair in animalCounts)
        {
            if (pair.Value > 2)
            {             
                AnimalStruct animal = playerAnimalDataManager.AddAnimal(pair.Key + mapInt + 100);

                if (animal.tier >= 5)
                {
                    return;
                }

                popup.SetActive(true);

                if (animal.tier == 1)
                {
                    popup_NewCustomer.SetActive(true);

                    NewAnimalImage.sprite = this.sprites[pair.Key];

                    GameInstance.GameIns.playerAnimalDataManager.PlayerAnimalUpdate(false, pair.Key + mapInt + 100);
                }
                else if (animal.tier >= 2 && animal.tier < 5)
                {
                    popup_TierUp.SetActive(true);

                    TierUpAnimalImage.sprite = this.sprites[pair.Key];
                                        
                    if (animal.tier == 2) backGlow.color = Color.blue;
                    else if (animal.tier == 3) backGlow.color = new Color(0.5f, 0f, 0.5f);
                    else if (animal.tier == 4) backGlow.color = Color.yellow;                    
                }
            }            
        }
    }
    void ClearRollings()
    {
        while(rollings.Count > 0)
        {
            GameInstance.GameIns.animalManager.RemoveGatchaAnimal(rollings.Dequeue());
        }
    }

    private void ClearAnimals()
    {
        foreach (GameObject animal in spawnedAnimals)
        {
            if (animal != null) Destroy(animal);
        }

        spawnedAnimals.Clear();
        spawnedAnimalTypes.Clear();
    }

    private void OnDestroy()
    {
        ClearAnimals();
    }

    public void GatcharSpeedUp()
    {
        if (!isFast)
        {
            spawnTerm = 0.04f;            
            rollingSpeed = 50;
            isFast = true;
            GameInstance.GameIns.uiManager.checkMark.SetActive(true);
        }
        else
        {
            spawnTerm = 0.2f;
            rollingSpeed = 10;
            isFast = false;
            GameInstance.GameIns.uiManager.checkMark.SetActive(false);
        }
        
    }
}
