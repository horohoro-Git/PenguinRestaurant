using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System;
public class GatcharManager : MonoBehaviour
{

    public MapType mapType;
    public GameObject[] gameObjects;
    public GameObject popup;
    public GameObject popup_NewCustomer;
    public GameObject popup_TierUp;
    public Image backGlow;
    public Transform penguinPoint;

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

    public Shadow shadow;
    public RectTransform shadowUI;
    Queue<Shadow> deactivateShadows = new Queue<Shadow>();
    List<Shadow> activateShadows = new List<Shadow>();
    WaitForSecondsRealtime spawnTerms = new WaitForSecondsRealtime(0.4f);
    GameObject gatchaPenguin;
    Animator animator;
    Animator GetAnimator {  get { if (animator == null) animator = gatchaPenguin.GetComponent<Animator>(); return animator; } }
    Coroutine gatchaPenguinEmotion;
    private void Awake()
    {
  //      t = cts.Token;
        GameInstance.GameIns.gatcharManager = this;

        for (int i = 0; i < 10; i++)
        {
            Shadow instancedShadow = Instantiate(shadow, shadowUI.transform);
            instancedShadow.Setup();
            //    instancedShadow.Set
            instancedShadow.transform.position = new Vector3(100, 100, 100);
            instancedShadow.gameObject.SetActive(false);
            deactivateShadows.Enqueue(instancedShadow);
        }

        gatchaPenguin = Instantiate(AssetLoader.loadedAssets[AssetLoader.itemAssetKeys[11].ID], penguinPoint);
        gatchaPenguin.transform.position = penguinPoint.position;
        Shadow s = GetShadow();
        AnimalStruct animalStruct = AssetLoader.animals[10];
        Vector2 sizeVector = new Vector2(animalStruct.size_width * 1.25f, animalStruct.size_height * 1.25f);
        s.SetSize(sizeVector, animalStruct.offset_x, animalStruct.offset_z);
        s.model = gatchaPenguin.transform;
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

    Shadow GetShadow()
    {
        Shadow s = deactivateShadows.Dequeue();
        s.gameObject.SetActive(true);
        activateShadows.Add(s);
        return s;
    }

    void RemoveShadow(Shadow s)
    {
        s.model = null;
        s.transform.position = new Vector3(100, 100, 100);
        s.gameObject.SetActive(false);
        deactivateShadows.Enqueue(s);
        activateShadows.Remove(s);
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
            yield return spawnTerms;
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
        AnimalStruct animalStruct = AssetLoader.animals[rolling.type];
        Shadow s = GetShadow();
        s.model = rolling.transform;
        s.SetSize(new Vector2(animalStruct.size_width, animalStruct.size_height), animalStruct.offset_x, animalStruct.offset_z);
        rollings.Enqueue(rolling);
        rolling.shadow = s;
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
                GetAnimator.SetInteger(AnimationKeys.state, 1);
                popup.SetActive(true);
                if(AnimalManager.gatchaTiers.ContainsKey(pair.Key))
                {
                    GetAnimator.SetInteger(AnimationKeys.emotion, 1);
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
                if(gatchaPenguinEmotion != null) StopCoroutine(gatchaPenguinEmotion);
                gatchaPenguinEmotion = StartCoroutine(EmotionDelay());
              
                break;
            }
        }
    }

    IEnumerator EmotionDelay()
    {
        float f = 0;
        while (f < 5)
        {
            f += Time.unscaledDeltaTime;
            yield return null;
        }
        GetAnimator.SetInteger(AnimationKeys.state, 0);
        GetAnimator.SetInteger(AnimationKeys.emotion, 0);
    }

    public void SpownAnimal()
    {
        int i = UnityEngine.Random.Range(0, gameObjects.Length);
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
                gatchaPenguin.GetComponent<Animator>().SetInteger(AnimationKeys.state, 1);
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
    public void ClearRollings()
    {
        while(rollings.Count > 0)
        {
            Rolling r = rollings.Dequeue();
            RemoveShadow(r.shadow);
            GameInstance.GameIns.animalManager.RemoveGatchaAnimal(r);

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
