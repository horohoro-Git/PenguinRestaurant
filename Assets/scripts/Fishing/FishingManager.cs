using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class FishingManager : MonoBehaviour
{
    public GameObject fishGOs;
    public Transform penguinPoint;
    public WaterQuality water;
    public WaterSplash splash;
    Dictionary<int, ItemStruct> fishingAnimals = new Dictionary<int, ItemStruct>(); 

    Dictionary<int, Queue<Fish>> fishes = new Dictionary<int, Queue<Fish>>();

    Dictionary<int, List<Fish>> activatedFishes = new Dictionary<int, List<Fish>>();

    Queue<WaterSplash> waterSplashQueue = new Queue<WaterSplash>();


    public Barrel barrel;
    public Transform barrelPoint;
    public Transform throwPoint;
    public Transform splashes;
    Barrel barrelObject;
    public float maxPosX;
    public float maxPosY;
    public float minPosX;
    public float minPosY;

    public 

    float spawnTimer = 5;
    float timerGap = 5;
    int numberOfFishes = 0;
    public int FishCount { get { return numberOfFishes; } set { numberOfFishes = value; 
            
                                if(GameInstance.GameIns.uiManager)
                                {
                                    GameInstance.GameIns.uiManager.fishesNumText.text = "(" + numberOfFishes.ToString() + " / 50)";
                                }
                          } }

    GameObject penguin;
    Animator penguinAnimator;

    [NonSerialized] public bool working;
    private void Awake()
    {
        GameInstance.GameIns.fishingManager = this;
    }
    private void Start()
    {
        fishingAnimals = AssetLoader.fishingAnimals;

        barrelObject = Instantiate(barrel, barrelPoint);
        penguin = Instantiate(AssetLoader.loadedAssets[AssetLoader.itemAssetKeys[12].ID], penguinPoint);
        penguin.transform.position = penguinPoint.position;
        penguin.transform.rotation = Quaternion.Euler(0, 180, 0);
        NewFishes();

        for(int i=0; i<20; i++)
        {
            WaterSplash waterSplash = Instantiate(splash, splashes);
            waterSplash.gameObject.SetActive(false);
            waterSplashQueue.Enqueue(waterSplash);
        }
     //   SpawnFish();
    }
   

    private void Update()
    {
        if (numberOfFishes < 50 && !water.isDirty)
        {
            if (spawnTimer + timerGap <= Time.unscaledTime)
            {
                spawnTimer = Time.unscaledTime;

                SpawnFish();
            }
        }
    }


    public void StartFishing()
    {
        if (!working && !water.isDirty)
        {
            working = true;
            StartCoroutine(Fishing());
        }
       // water.ChangeDirty();
    }

    IEnumerator Fishing()
    {
        if(Utility.TryGetComponentInChildren<Head>(penguin, out Head head))
        {
            Vector3 pos = head.transform.position;
            barrelObject.transform.DOJump(pos, 2, 1, 0.2f);
            yield return CoroutneManager.waitForzerothree;
            barrelObject.target = head.transform;
        }

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }


        penguin.GetComponent<Animator>().SetInteger(AnimationKeys.state, 1);
        float f = 0;
        float current = 6;
        float target = 12;
        while (f <= 0.2f)
        {
            float next = Mathf.Lerp(current, target, f * 5);
            Vector3 vector3 = penguin.transform.position;
            vector3.y = next;
            penguin.transform.position = vector3;
            f += Time.unscaledDeltaTime;
            yield return null;
        }
        barrelObject.target = null;
        barrelObject.Throw(throwPoint.position);
        yield return null;
        f = 0;
        penguin.GetComponent<Animator>().SetInteger(AnimationKeys.state, 2);
        while (f <= 0.4f)
        {
            float next = Mathf.Lerp(target, current, f * 2.5f);
            Vector3 vector3 = penguin.transform.position;
            vector3.y = next;
            penguin.transform.position = vector3;
            f += Time.unscaledDeltaTime;
            yield return null;
        }
        penguin.transform.position = penguinPoint.position;
        penguin.GetComponent<Animator>().SetInteger(AnimationKeys.state, 0);

        timer = 0;
        while (timer < 0.5f)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        water.ChangeDirty();
    }

    public void SpawnFish()
    {
        int r = Random.Range(900, 903);
        Fish fish = GetFish(r);

        Vector3 pos = RandomLocation();

        fish.transform.position = pos;
        fish.Swim();
    }




    public void NewFishes()
    {
        foreach(var v in fishingAnimals)
        {
            fishes[v.Key] = new Queue<Fish>();
            activatedFishes[v.Key] = new List<Fish>();
            for (int i = 0; i < 50; i++)
            {
                Fish fish = Instantiate(AssetLoader.loadedAssets[v.Value.Name], fishGOs.transform).GetComponent<Fish>();
                fish.Setup(AssetLoader.animals[v.Key]);  
                fishes[v.Key].Enqueue(fish);
            }
        }
    }

    public Fish GetFish(int type)
    {
        Fish fish = fishes[type].Dequeue();
        fish.gameObject.SetActive(true);
        activatedFishes[type].Add(fish);
        FishCount++;
        return fish;
    }

    public void RemoveFish(Fish fish)
    {
        fish.gameObject.SetActive(false);
        activatedFishes[fish.animalStruct.id].Remove(fish);
        FishCount--;
        fishes[fish.animalStruct.id].Enqueue(fish);
    }
 

    public Vector3 RandomLocation()
    {
        float x = Random.Range(minPosX, maxPosX);
        float y = Random.Range(minPosY, maxPosY);
        Vector3 pos = new Vector3(x, -10, y);
        return pos;
    }

    public void KillAllFishes()
    {
        foreach (var fish in activatedFishes)
        {
            int c = fish.Value.Count;
            for(int i = 0; i< c; i++)
            {
                fish.Value[i].bDead = true;
            }
        }
    }

    public WaterSplash GetSplash()
    {
        WaterSplash waterSplash = waterSplashQueue.Dequeue();
        waterSplash.gameObject.SetActive(true);
        
        return waterSplash;
    }

    public void RemoveSplash(WaterSplash waterSplash)
    {
        waterSplash.gameObject.SetActive(false);    
        waterSplashQueue.Enqueue(waterSplash);
    }
}
