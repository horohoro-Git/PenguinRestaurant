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
    public RectTransform canvas;
    public GameObject fishIcon;
    Dictionary<int, ItemStruct> fishingAnimals = new Dictionary<int, ItemStruct>(); 

    Dictionary<int, Queue<Fish>> fishes = new Dictionary<int, Queue<Fish>>();

    Dictionary<int, List<Fish>> activatedFishes = new Dictionary<int, List<Fish>>();

    Queue<WaterSplash> waterSplashQueue = new Queue<WaterSplash>();

    Queue<GameObject> fishRewardIconQueue = new Queue<GameObject>();

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
    [NonSerialized] public int numberOfFishes = 0;
    public int FishCount { get { return numberOfFishes; } set { numberOfFishes = value; 
            
                                if(GameInstance.GameIns.uiManager)
                                {
                                    GameInstance.GameIns.uiManager.fishesNumText.text = "(" + numberOfFishes.ToString() + " / 50)";
                                }
                          } }

    GameObject penguin;
    Animator penguinAnimator;

    [NonSerialized] public bool working;

    public bool setup;

    Coroutine incomeCoroutine;
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

        for(int i=0; i<1000; i++)
        {
            GameObject fishIconObject = Instantiate(fishIcon, canvas.transform);
            fishIconObject.SetActive(false);
            fishRewardIconQueue.Enqueue(fishIconObject);
        }
    }
   

    private void Update()
    {
        if (setup)
        {
            if (numberOfFishes < 50 && !water.isDirty && !working)
            {
                if (spawnTimer + timerGap <= Time.unscaledTime)
                {
                    spawnTimer = Time.unscaledTime;

                    SpawnFish();
                }
            }
        }
    }

    public void LoadStatus(Fishing fishing)
    {
        if(fishing.isDirty)
        {
            working = true;
            water.ChangeDirty(false);
        }
        else
        {
            working = false;
            water.ChangeClean(false);
        }


        for (int i = 0; i < fishing.fishNum; i++)
        {
            int r = Random.Range(900, 903);
            Fish f = GetFish(r);
            Vector3 pos = RandomLocation();
            float rotRand = Random.Range(0, 360);
            f.trans.position = pos;
            f.trans.rotation = Quaternion.Euler(0,rotRand, 0);
            if(fishing.isDirty)
            {
                f.Dead(true);
                f.bDead = true;
            }
            else
            {
                f.Swim();
            }
        }
        FishCount = fishing.fishNum;
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
            SoundManager.Instance.PlayAudio(GameInstance.GameIns.gameSoundManager.ThrowSound(), 0.2f);
            yield return CoroutneManager.waitForzerothree;
            barrelObject.target = head.transform;
        }

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        SoundManager.Instance.PlayAudio(GameInstance.GameIns.gameSoundManager.Quack(), 0.2f);
        
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
    //    numberOfFishes++;
        FishCount++;
        GameInstance.GameIns.restaurantManager.miniGame.fishing.fishNum++;
        GameInstance.GameIns.restaurantManager.miniGame.changed = true;
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
        return fish;
    }

    public void RemoveFish(Fish fish)
    {
        fish.gameObject.SetActive(false);
        activatedFishes[fish.animalStruct.id].Remove(fish);
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


    public GameObject GetFishIcon()
    {
        GameObject f;
        if (fishRewardIconQueue.Count > 0)
        {
            f = fishRewardIconQueue.Dequeue();
            f.SetActive(true);
        }
        else
        {

            f = Instantiate(fishIcon, canvas.transform);
        }
        return f;
    }

    public void RemoveFishIcon(GameObject icon)
    {
        icon.SetActive(false);
        fishRewardIconQueue.Enqueue(icon);
    }

    public void CaughtFish(Vector3 pos)
    {
       
       
        StartCoroutine(GetCaughtFish(pos));
    }

    IEnumerator GetCaughtFish(Vector3 pos)
    {
        Vector3 target = InputManger.cachingCamera.WorldToScreenPoint(pos);

        int r = Random.Range(5, 10);
        int baseNum = GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes;
        GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes += r;
        GameInstance.GameIns.restaurantManager.restaurantCurrency.changed = true;
        yield return new WaitForSecondsRealtime(0.2f);

        Stack<RectTransform> stack = new Stack<RectTransform>();
        for (int i = 0; i < r; i++)
        {
            RectTransform icon = GetFishIcon().GetComponent<RectTransform>();
            icon.position = target;
            stack.Push(icon);
        }

        yield return new WaitForSecondsRealtime(0.2f);

        foreach (var v in stack)
        {
            StartCoroutine(SpreadFishes(v, target));
        }

        yield return new WaitForSecondsRealtime(0.5f);

        while (stack.Count > 0)
        {
            RectTransform icon = stack.Pop();
            StartCoroutine(Rewarding(icon, pos));
            yield return new WaitForSecondsRealtime(0.01f);
        }

      //  GameInstance.GameIns.uiManager.fishText.text = GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes.ToString();
    }
    IEnumerator SpreadFishes(RectTransform rect, Vector3 pos)
    {
        float x = Random.Range(-100, 100);
        float y = Random.Range(-100, 100);
        Vector3 target = pos;
        target.x += x;
        target.y += y;
        float f = 0;
        while(f <= 1)
        {
            rect.position = Vector3.Lerp(pos, target, f);
            f += Time.unscaledDeltaTime / 0.2f;
            yield return null;
        }
    }
    IEnumerator Rewarding(RectTransform icon, Vector3 pos)
    {
        Vector3 cur = icon.position;
        Vector3 target = GameInstance.GameIns.uiManager.fishImage.position;
   
        float f = 0;
        while (f <= 1)
        {
            icon.position = Vector3.Lerp(cur, target, f);
            f += Time.unscaledDeltaTime / 0.2f;
            yield return null;
        }
        icon.position = target;

        SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.Fish(), 0.4f);

        int before = GameInstance.GameIns.restaurantManager.restaurantCurrency.fishes;

        int num = int.Parse(GameInstance.GameIns.uiManager.fishText.text);
        GameInstance.GameIns.uiManager.fishText.text = (num + 1).ToString();
        RemoveFishIcon(icon.gameObject);
       
    }


    /*IEnumerator FishIncome()
    {

    }*/
}
