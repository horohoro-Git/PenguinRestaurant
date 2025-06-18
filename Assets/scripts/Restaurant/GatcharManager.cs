using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System.Text;
using Unity.VisualScripting;
public class GatcharManager : MonoBehaviour
{

    public MapType mapType;
    public GameObject[] gameObjects;
    public GameObject popup;
    public GameObject popup_NewCustomer;
    public GameObject popup_TierUp;
    public Image backGlow;
    public Transform penguinPoint;
    public int price;
    public Sprite[] sprites;
    public Image NewAnimalImage;
    public Image TierUpAnimalImage;
    public bool isSpawning;
    public float spawnTerm = 0.2f;
    public float rollingSpeed = 10;
    public GameObject advertise;
    GameObject advertisementGO;

    BigInteger gatchaPrice;

    public List<Texture2D> adTextures = new List<Texture2D>();

    public CinemachineVirtualCamera virtualCamera1;
    public CinemachineVirtualCamera virtualCamera2;
    public CinemachineVirtualCamera virtualCamera3;
    public CinemachineVirtualCamera virtualCamera4;

    public List<Vector3> advertisePositions = new List<Vector3>();
    public List<Vector3> advertiseRotations = new List<Vector3>();

    private bool isFast;
    private int mapInt;
    private int x = 5;
    public List<Vector3> customerPositions = new List<Vector3>();
    public List<Vector3> customerRotations = new List<Vector3>();
    //5 : -96.5, -100, -1104, 20, -110, 0
    //0 : -99.5, -100, -1102, 20, -180, 0
    //-5: -102.5, -100, -1104, 20, -250, 0
    private List<GameObject> spawnedAnimals = new List<GameObject>();
    private List<int> spawnedAnimalTypes = new List<int>();

    Queue<Rolling> rollings = new Queue<Rolling>();

    private PlayerAnimalDataManager playerAnimalDataManager;

    public Shadow shadow;
    public RectTransform shadowUI;
    Queue<Shadow> deactivateShadows = new Queue<Shadow>();
    List<Shadow> activateShadows = new List<Shadow>();
    GameObject gatchaPenguin;
    Animator animator;
    Animator GetAnimator {  get { if (animator == null) animator = gatchaPenguin.GetComponent<Animator>(); return animator; } }
    Coroutine gatchaPenguinEmotion;

    Dictionary<int, int> randomAnimalKey = new Dictionary<int, int>();

    CancellationTokenSource gatchaToken = new CancellationTokenSource();
    CancellationTokenSource emotionToken = new CancellationTokenSource();
    StringBuilder sb = new StringBuilder();
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

        AnimalManager.gatchaTiers = SaveLoadSystem.LoadGatchaAnimals();
        LoadAnimals();
    }

    public void CheckMoney()
    {
        if (GameInstance.GameIns.restaurantManager.restaurantCurrency.Money >= gatchaPrice)
        {
            GameInstance.GameIns.uiManager.drawPriceText.color = Color.yellow;

        }
        else
        {
            GameInstance.GameIns.uiManager.drawPriceText.color = Color.red;

        }
    }

    public bool CheckCompleteGatcha()
    {
        switch (mapType)
        {
            case MapType.town:
                {
                    for (int i = 100; i <= 106; i++)
                    {
                        if (AnimalManager.gatchaTiers.ContainsKey(i))
                        {
                            if (AnimalManager.gatchaTiers[i] < 4) return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                break;
        
        }

        return true;
    }

    public void ResetToken()
    {
        isSpawning = false;
        if (gatchaToken != null) gatchaToken.Cancel();
        if(emotionToken != null)  emotionToken.Cancel();
        if(gatchaPenguin != null) gatchaPenguin.transform.position = penguinPoint.position;
        ClearRollings();
        popup_NewCustomer.SetActive(false);
        popup_TierUp.SetActive(false);
        popup.SetActive(false);
        if (advertisementGO != null)
        {
            advertisementGO.GetComponent<MeshRenderer>().material.SetFloat("_TopBend", 0);
            advertisementGO.GetComponent<MeshRenderer>().material.SetFloat("_BottomBend", 0);
            advertisementGO.SetActive(false);
        }
        x = 5;

        GetAnimator.SetInteger(AnimationKeys.state, 0);
        GetAnimator.SetInteger(AnimationKeys.emotion, 0);

    }
    void LoadAnimals()
    {
        int num = 0;
        foreach (var v in AnimalManager.gatchaTiers)
        {
            if (v.Value > 0)
            {
                AnimalStruct asset = AssetLoader.animals[v.Key];
                AnimalManager.animalStructs[v.Key] = asset;
                num += v.Value;
            }
        }

        gatchaPrice = 100 + Mathf.FloorToInt(Mathf.Pow((num - 1), 1.6f)) * 15;
        sb = Utility.GetFormattedMoney(gatchaPrice, sb);
        GameInstance.GameIns.uiManager.drawPriceText.text = sb.ToString();
   
        gatchaPrice = Utility.StringToBigInteger(sb.ToString());
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

    private void OnApplicationQuit()
    {
        if(gatchaToken != null) gatchaToken.Cancel();
        if(emotionToken != null) emotionToken.Cancel();
    }

    public void StartGatcha()
    {
        if (isSpawning) return;
        if (!Purchase()) return;

        randomAnimalKey.Clear();
        switch(mapType)
        {
            case MapType.town:
                for (int i = 0; i < 3; i++)
                {
                    int r = UnityEngine.Random.Range(100, 106);
                    if (!randomAnimalKey.ContainsKey(r))
                    {
                        randomAnimalKey[r] = 1;
                    }
                    else
                    {
                        randomAnimalKey[r]++;
                    }
                }
                break;
        }

        bool success = false;
        foreach(var pair in randomAnimalKey)
        {
            if(pair.Value == 1)
            {
                if (AnimalManager.gatchaTiers.ContainsKey(pair.Key))
                {
                    if (AnimalManager.gatchaTiers[pair.Key] < 4)
                    {
                        AnimalManager.gatchaTiers[pair.Key]++;
                        SaveLoadSystem.SaveGatchaAnimalsData();
                        success = true;
                    }
                }
                else
                {
                    AnimalManager.gatchaTiers[pair.Key] = 1;
                    AnimalStruct asset = AssetLoader.animals[pair.Key];
                    AnimalManager.animalStructs[pair.Key] = asset;
                    SaveLoadSystem.SaveGatchaAnimalsData();
                    success = true;
                    //  GameInstance.GameIns.animalManager.AddNewAnimal(lockAnimals[keyValuePair.Key], keyValuePair.Key, animal);
                }
                break;
            }
        }

        GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(false);
        ClearRollings();
        popup_NewCustomer.SetActive(false);
        popup_TierUp.SetActive(false);
        popup.SetActive(false);
        isSpawning = true;
        x = 5;

        GetAnimator.SetInteger(AnimationKeys.state, 0);
        GetAnimator.SetInteger(AnimationKeys.emotion, 0);

        if (advertisementGO == null)
        {
            advertisementGO = Instantiate(advertise);
        }
        else advertisementGO.SetActive(true);

        advertisementGO.GetComponent<MeshRenderer>().material.SetFloat("_TopBend", 0);
        advertisementGO.GetComponent<MeshRenderer>().material.SetFloat("_BottomBend", 0);

        if (gatchaToken != null) gatchaToken.Cancel();
        gatchaToken = new CancellationTokenSource();
        AdvertisementAsync(success, gatchaToken.Token).Forget();
     
        if(success)
        {
            int num = 0;
            foreach (var v in AnimalManager.gatchaTiers)
            {
                if (v.Value > 0)
                {
                    num += v.Value;
                }
            }
            gatchaPrice = 100 + Mathf.FloorToInt(Mathf.Pow((num - 1), 1.6f)) * 15;
            sb = Utility.GetFormattedMoney(gatchaPrice, sb);
            GameInstance.GameIns.uiManager.drawPriceText.text = sb.ToString();
            gatchaPrice = Utility.StringToBigInteger(sb.ToString());
        }
    }

    public async UniTask AdvertisementAsync(bool success, CancellationToken cancellationToken = default)
    {
       

        int randomTex = Random.Range(0, adTextures.Count);
        advertisementGO.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", adTextures[randomTex]);
        if (!isFast)
        {
            advertisementGO.transform.position = gatchaPenguin.GetComponentInChildren<Head>().transform.position + Vector3.up * 0.4f;
            advertisementGO.transform.rotation = Quaternion.Euler(Vector3.zero);
            advertisementGO.transform.SetParent(gatchaPenguin.GetComponentInChildren<Head>().transform);

            virtualCamera1.Priority = 0;
            virtualCamera2.Priority = 1;
            virtualCamera3.Priority = 0;
            virtualCamera4.Priority = 0;

            await UniTask.Delay(400, DelayType.UnscaledDeltaTime, cancellationToken:  cancellationToken);   

            bool throwSound = true;
            if (success)
            {
                int r = UnityEngine.Random.Range(0, 100);
                if (r < 40)
                {
                    SoundManager.Instance.PlayAudio(GameInstance.GameIns.gatchaSoundManager.Wink(), 0.4f);
                    GetAnimator.SetInteger(AnimationKeys.emotion, 1);
                    throwSound = false;
                    await UniTask.Delay(500, DelayType.UnscaledDeltaTime, cancellationToken: cancellationToken);
                }
            }

            Vector3 target = new Vector3(-0.5f, 5f, -1005f);
           

            await UniTask.Delay(200, DelayType.UnscaledDeltaTime, cancellationToken: cancellationToken);

            GetAnimator.SetInteger(AnimationKeys.state, 1);
            float timer = 0;
            float cur = gatchaPenguin.transform.position.y;
            float h = 3;
            advertisementGO.transform.SetParent(null);
            advertisementGO.transform.DOJump(target, 3, 1, 1f).SetUpdate(true);

            if (throwSound) SoundManager.Instance.PlayAudio(GameInstance.GameIns.gameSoundManager.Quack(), 0.2f);
            SoundManager.Instance.PlayAudio(GameInstance.GameIns.gatchaSoundManager.Paper(), 0.4f);

            while (timer <= 1)
            {
                float height = Mathf.Lerp(cur, h, timer);
                Vector3 pos = gatchaPenguin.transform.position;
                pos.y = height;
                gatchaPenguin.transform.position = pos;
                timer += Time.unscaledDeltaTime / 0.1f;
                await UniTask.NextFrame(cancellationToken: cancellationToken);  
            }
            GetAnimator.SetInteger(AnimationKeys.state, 2);
            await UniTask.NextFrame(cancellationToken: cancellationToken);
            timer = 0;
            while (timer <= 1)
            {
                float height = Mathf.Lerp(h, cur, timer);
                Vector3 pos = gatchaPenguin.transform.position;
                pos.y = height;
                gatchaPenguin.transform.position = pos;
                timer += Time.unscaledDeltaTime / 0.2f;
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }
            advertisementGO.GetComponent<MeshRenderer>().material.SetFloat("_TopBend", 1);
            advertisementGO.GetComponent<MeshRenderer>().material.SetFloat("_BottomBend", 1);


            GetAnimator.SetInteger(AnimationKeys.state, 0);
            await UniTask.Delay(200, DelayType.UnscaledDeltaTime, cancellationToken: cancellationToken);

            virtualCamera1.Priority = 0;
            virtualCamera2.Priority = 0;
            virtualCamera3.Priority = 1;
            virtualCamera4.Priority = 0;

            await UniTask.Delay(500, DelayType.UnscaledDeltaTime, cancellationToken: cancellationToken);

            for (int i = 0; i < advertisePositions.Count; i++)
            {

                Vector3 origin = advertisementGO.transform.position;
                target = advertisePositions[i];
                Quaternion originRot = advertisementGO.transform.rotation;
                Quaternion targetRot = Quaternion.Euler(advertiseRotations[i]);
                float f = 0;
                float duration = 0.5f;
                float elapsed = 0f;
                while (f <= 1)
                {
                    float easedF = Mathf.SmoothStep(0f, 1f, f);
                    Vector3 next = Vector3.Lerp(origin, target, easedF);
                    advertisementGO.transform.position = next;
                    Quaternion nextRot = Quaternion.Slerp(originRot, targetRot, easedF);
                    advertisementGO.transform.rotation = nextRot;
                    elapsed += Time.unscaledDeltaTime;
                    f = elapsed / duration;
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                }
            }

            SoundManager.Instance.PlayAudio(GameInstance.GameIns.gatchaSoundManager.Paper(), 0.4f);

            virtualCamera1.Priority = 0;
            virtualCamera2.Priority = 0;
            virtualCamera3.Priority = 0;
            virtualCamera4.Priority = 1;
            advertisementGO.GetComponent<MeshRenderer>().material.SetFloat("_TopBend", 0);
            advertisementGO.GetComponent<MeshRenderer>().material.SetFloat("_BottomBend", 0);

        }
        else
        {
            advertisementGO.transform.position = advertisePositions[advertisePositions.Count - 1];
        }
        SpawnRollingsWithDelayAsync(success, cancellationToken).Forget();
    }

    async UniTask SpawnRollingsWithDelayAsync(bool success, CancellationToken cancellationToken = default)
    {
        foreach (var v in randomAnimalKey)
        {
            for (int i = 0; i < v.Value; i++)
            {
                SpawnRolling(v.Key);
                //  yield return new WaitForSecondsRealtime(spawnTerm);
                await UniTask.Delay((int)(spawnTerm * 1000), DelayType.UnscaledDeltaTime, cancellationToken: cancellationToken);
            }
        }

        await WaitRollingsReachTargetAsync(cancellationToken);

        // yield return StartCoroutine(WaitRollingsReachTarget());

        virtualCamera1.Priority = 1;
        virtualCamera2.Priority = 0;
        virtualCamera3.Priority = 0;
        virtualCamera4.Priority = 0;
        await UniTask.Delay(200, DelayType.UnscaledDeltaTime, cancellationToken: cancellationToken);
        CheckGatchaAnimals(success);
        isSpawning = false;
        if (GameInstance.GameIns.app.currentScene == SceneState.Draw && !CheckCompleteGatcha())
        {
            if(!GameInstance.GameIns.uiManager.drawBtn.gameObject.activeSelf) GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
            if(!GameInstance.GameIns.uiManager.drawSpeedUpBtn.gameObject.activeSelf) GameInstance.GameIns.uiManager.drawSpeedUpBtn.gameObject.SetActive(true);
        }
        else
        {
            if (GameInstance.GameIns.uiManager.drawBtn.gameObject.activeSelf) GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(false);
            if (GameInstance.GameIns.uiManager.drawSpeedUpBtn.gameObject.activeSelf) GameInstance.GameIns.uiManager.drawSpeedUpBtn.gameObject.SetActive(false);
        }
    }
    IEnumerator Advertisement(bool success)
    {
        if(advertisementGO == null) advertisementGO = Instantiate(advertise);
        else advertisementGO.SetActive(true);
        if (!isFast)
        {
            advertisementGO.transform.position = gatchaPenguin.GetComponentInChildren<Head>().transform.position + Vector3.up * 0.4f;
            advertisementGO.transform.SetParent(gatchaPenguin.GetComponentInChildren<Head>().transform);

            virtualCamera1.Priority = 0;
            virtualCamera2.Priority = 1;
            virtualCamera3.Priority = 0;
            virtualCamera4.Priority = 0;

            yield return new WaitForSecondsRealtime(0.4f);

            bool throwSound = true;
            if (success)
            {
                int r = UnityEngine.Random.Range(0, 100);
                if (r < 40)
                {
                    SoundManager.Instance.PlayAudio(GameInstance.GameIns.gatchaSoundManager.Wink(), 0.4f);
                    GetAnimator.SetInteger(AnimationKeys.emotion, 1);
                    throwSound = false;
                    yield return new WaitForSecondsRealtime(0.5f);
                }
            }
         
            Vector3 target = new Vector3(-0.5f, 5f, -1005f);
            advertisementGO.GetComponent<MeshRenderer>().material.SetFloat("_TopBend", 0);
            advertisementGO.GetComponent<MeshRenderer>().material.SetFloat("_BottomBend", 0);

            yield return new WaitForSecondsRealtime(0.2f);

            GetAnimator.SetInteger(AnimationKeys.state, 1);
            float timer = 0;
            float cur = gatchaPenguin.transform.position.y;
            float h = 3;
            advertisementGO.transform.SetParent(null);
            advertisementGO.transform.DOJump(target, 3, 1, 1f).SetUpdate(true);

            if(throwSound) SoundManager.Instance.PlayAudio(GameInstance.GameIns.gameSoundManager.Quack(), 0.2f);
            SoundManager.Instance.PlayAudio(GameInstance.GameIns.gatchaSoundManager.Paper(), 0.4f);

            while (timer <= 1)
            {
                float height = Mathf.Lerp(cur, h, timer);
                Vector3 pos = gatchaPenguin.transform.position;
                pos.y = height;
                gatchaPenguin.transform.position = pos;
                timer += Time.unscaledDeltaTime / 0.1f;
                yield return null;
            }
            GetAnimator.SetInteger(AnimationKeys.state, 2);
            yield return null;
            timer = 0;
            while (timer <= 1)
            {
                float height = Mathf.Lerp(h, cur, timer);
                Vector3 pos = gatchaPenguin.transform.position;
                pos.y = height;
                gatchaPenguin.transform.position = pos;
                timer += Time.unscaledDeltaTime / 0.2f;
                yield return null;
            }
            advertisementGO.GetComponent<MeshRenderer>().material.SetFloat("_TopBend", 1);
            advertisementGO.GetComponent<MeshRenderer>().material.SetFloat("_BottomBend", 1);


            GetAnimator.SetInteger(AnimationKeys.state, 0);
            yield return new WaitForSecondsRealtime(0.2f);

            virtualCamera1.Priority = 0;
            virtualCamera2.Priority = 0;
            virtualCamera3.Priority = 1;
            virtualCamera4.Priority = 0;

            yield return new WaitForSecondsRealtime(0.5f);

            for (int i = 0; i < advertisePositions.Count; i++)
            {
                
                Vector3 origin = advertisementGO.transform.position;
                target = advertisePositions[i];
                Quaternion originRot = advertisementGO.transform.rotation;
                Quaternion targetRot = Quaternion.Euler(advertiseRotations[i]);
                float f = 0;
                float duration = 0.5f;
                float elapsed = 0f;
                while (f <= 1)
                {
                    float easedF = Mathf.SmoothStep(0f, 1f, f);
                    Vector3 next = Vector3.Lerp(origin, target, easedF);
                    advertisementGO.transform.position = next;
                    Quaternion nextRot = Quaternion.Slerp(originRot, targetRot, easedF);
                    advertisementGO.transform.rotation = nextRot;
                    elapsed += Time.unscaledDeltaTime;
                    f = elapsed / duration;
                    yield return null;
                }
            }

            SoundManager.Instance.PlayAudio(GameInstance.GameIns.gatchaSoundManager.Paper(), 0.4f);

            virtualCamera1.Priority = 0;
            virtualCamera2.Priority = 0;
            virtualCamera3.Priority = 0;
            virtualCamera4.Priority = 1;
            advertisementGO.GetComponent<MeshRenderer>().material.SetFloat("_TopBend", 0);
            advertisementGO.GetComponent<MeshRenderer>().material.SetFloat("_BottomBend", 0);

        }
        else
        {
            advertisementGO.transform.position = advertisePositions[advertisePositions.Count - 1];
        }
        StartCoroutine(SpawnRollingsWithDelay());
        
    }

    public bool Purchase()
    {
        if (GameInstance.GameIns.restaurantManager.restaurantCurrency.Money >= gatchaPrice)
        {
            Debug.Log((-gatchaPrice).ToString());
            //  GameInstance.GameIns.restaurantManager.restaurantCurrency.Money -= (int)price;
            GameInstance.GameIns.restaurantManager.GetMoney((-gatchaPrice).ToSafeString());

            SoundManager.Instance.PlayAudio(GameInstance.GameIns.gatchaSoundManager.Purchase(), 0.2f);
          
            return true;
        }
        return false;
    }

    IEnumerator SpawnRollingsWithDelay()
    {
        foreach(var v in randomAnimalKey)
        {
            for(int i=0; i<v.Value; i++)
            {
                SpawnRolling(v.Key);
                yield return new WaitForSecondsRealtime(spawnTerm);
            }
        }
     
        yield return StartCoroutine(WaitRollingsReachTarget());
     
        virtualCamera1.Priority = 1;
        virtualCamera2.Priority = 0;
        virtualCamera3.Priority = 0;
        virtualCamera4.Priority = 0;
        yield return new WaitForSecondsRealtime(0.2f);
        CheckGatchaAnimals(true);
        isSpawning = false;
        if (GameInstance.GameIns.app.currentScene == SceneState.Draw) GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
    }

    async UniTask WaitRollingsReachTargetAsync(CancellationToken cancellationToken = default)
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

            await UniTask.NextFrame(cancellationToken: cancellationToken);
        }
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

    void SpawnRolling(int type)
    {
        Rolling rolling = GameInstance.GameIns.animalManager.GetGatchaAnimal(MapType.town, type);
        AnimalStruct animalStruct = AssetLoader.animals[rolling.type];
        Shadow s = GetShadow();
        s.model = rolling.transform;
        s.SetSize(new Vector2(animalStruct.size_width, animalStruct.size_height), animalStruct.offset_x, animalStruct.offset_z);
        rollings.Enqueue(rolling);
        rolling.shadow = s;
        rolling.transform.position = new Vector3(x, 0, -980);
        rolling.transform.rotation = Quaternion.Euler(0, 180, 0);
        int index = 1 + (x == 0 ? 1 : x) / 5;
        x -= 5;
        rolling.Roll(customerPositions[index], customerRotations[index]);
    }

    void CheckGatchaAnimals(bool success)
    {
        if (success)
        {
            foreach (var pair in randomAnimalKey)
            {
                if (pair.Value == 1)
                {
                    //  GetAnimator.SetInteger(AnimationKeys.state, 1);
                    //   popup.SetActive(true);
                    if (AnimalManager.gatchaTiers.ContainsKey(pair.Key))
                    {
                        if (AnimalManager.gatchaTiers[pair.Key] == 1)
                        {
                            popup.SetActive(true);
                            GetAnimator.SetInteger(AnimationKeys.state, 1);
                            SoundManager.Instance.PlayAudio(GameInstance.GameIns.gatchaSoundManager.Unlock(), 0.4f);
                            AnimalStruct asset = AssetLoader.animals[pair.Key];
                            string n = asset.asset_name + "_Sprite";
                            NewAnimalImage.sprite = AssetLoader.loadedSprites[n];
                            popup_NewCustomer.SetActive(true);
                        }
                        else if (AnimalManager.gatchaTiers[pair.Key] <= 4)
                        {
                            popup.SetActive(true);
                            GetAnimator.SetInteger(AnimationKeys.emotion, 1);
                            GetAnimator.SetInteger(AnimationKeys.state, 1);
                            int tier = AnimalManager.gatchaTiers[pair.Key];
                            SoundManager.Instance.PlayAudio(GameInstance.GameIns.gatchaSoundManager.GradeUp(), 0.4f);

                            popup_TierUp.SetActive(true);

                            AnimalStruct asset = AssetLoader.animals[pair.Key];
                            string n = asset.asset_name + "_Sprite";
                            TierUpAnimalImage.sprite = AssetLoader.loadedSprites[n];
                            if (tier == 2) backGlow.color = Color.blue;
                            else if (tier == 3) backGlow.color = new Color(0.5f, 0f, 0.5f);
                            else if (tier == 4) backGlow.color = Color.yellow;

                        }
                    }
                    else
                    {
                        popup.SetActive(true);
                        SoundManager.Instance.PlayAudio(GameInstance.GameIns.gatchaSoundManager.Unlock(), 0.4f);

                        AnimalManager.gatchaTiers[pair.Key] = 1;
                        AnimalStruct asset = AssetLoader.animals[pair.Key];
                        string n = asset.asset_name + "_Sprite";
                        NewAnimalImage.sprite = AssetLoader.loadedSprites[n];// this.sprites[pair.Key];
                        popup_NewCustomer.SetActive(true);
                        AnimalManager.animalStructs[pair.Key] = asset;
                        //  GameInstance.GameIns.animalManager.AddNewAnimal(lockAnimals[keyValuePair.Key], keyValuePair.Key, animal);
                    }

                    if (emotionToken != null) emotionToken.Cancel();
                    emotionToken = new CancellationTokenSource();
                    EmotionDelayAsync(emotionToken.Token).Forget();
                    break;
                }
            }
        }
    }

    async UniTask EmotionDelayAsync(CancellationToken cancellationToken = default)
    {
        float f = 0;
        while (f < 5)
        {
            f += Time.unscaledDeltaTime;
            await UniTask.NextFrame(cancellationToken: cancellationToken);
        }
        GetAnimator.SetInteger(AnimationKeys.state, 0);
        GetAnimator.SetInteger(AnimationKeys.emotion, 0);
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
        if(advertisementGO != null) advertisementGO.SetActive(false);
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
