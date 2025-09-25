using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
//using UnityEditor;
using UnityEngine.InputSystem.EnhancedTouch;


public enum SceneState
{
    Menu,
    Loading,
    Restaurant,
    Draw,
    Fishing
}

public enum Stage
{
    None = -1,
    Town_01,
    Forest_01
}

public class App : MonoBehaviour
{
    private static CancellationTokenSource globalCts = new();
    public static CancellationToken GlobalToken => globalCts.Token;

    public static SceneState currentScene;
    public static Stage currentStage;

    public static float restaurantTimeScale = 1f;
    //public static Language language;
    public static Queue<LanguageText> languageTexts = new();
    public static Dictionary<int, LanguageScript> languages = new();
    public static GameSettings gameSettings;
    Vector3 vector;
    public Vector3 pos { get { return vector; } set { vector = value; } }
    public static Dictionary<string, Scene> scenes = new();
    static Loading loading;
    public static bool loadedAllAssets;
    static List<GameObject> loadedScenesRootUI = new();
    static int currentLevel;
    public static int CurrentLevel { get { return currentLevel; } set { currentLevel = value; } }
    public static Dictionary<int, BundleCheck> bundleCheck = new();
    public static Dictionary<int, BundleCheck> currentHashes = new();
    public static Dictionary<int, List<BundleCheck>> cachedData = new();
    public static string previewStageName = "";
    [SerializeField]
    PlayerInput playerInput;
    int escapeTabCount = 0;
  
    private void Awake()
    {
     //   Screen.SetResolution(540, 960, false);
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        currentScene = SceneState.Restaurant;
       // currentStage = 
        GameInstance.GameIns.app = this;
        DontDestroyOnLoad(this);
       
        gameSettings = SaveLoadSystem.LoadGameSettings();
        bundleCheck = SaveLoadSystem.LoadDownloadedData();
        cachedData = SaveLoadSystem.LoadCachedDownloadedData();
        currentStage = (Stage)gameSettings.clearStage;
        TextAsset lang = null;
        if (gameSettings.language == Language.KOR) lang = Resources.Load<TextAsset>("language_kor");
        else lang = Resources.Load<TextAsset>("language_eng");
      
        List<LanguageScript> l = JsonConvert.DeserializeObject<List<LanguageScript>>(lang.text);
        languages.Clear();
        for (int i = 0; i < l.Count; i++) languages[l[i].id] = l[i];

        if (!scenes.ContainsKey("LoadingScene")) LoadLoadingScene(GlobalToken).Forget();
    }

    private void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("unblockBackKey");
        }
#endif
    }
    private void OnEnable()
    {
#if UNITY_ANDROID
        EnhancedTouchSupport.Enable();
        InputSystem.EnableDevice(Touchscreen.current);
#endif
        playerInput.actions["EscapeTab"].started += StartEscapeTab;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }
    private void OnDisable()
    {
        if(playerInput != null) playerInput.actions["EscapeTab"].started -= StartEscapeTab;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
    }
#if UNITY_EDITOR
    private void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
    {
        if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
        {
            GameExit().Forget();
        }
    }
#endif

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha5)) GameExit().Forget();
    }

    public async UniTask LoadLoadingScene(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await LoadScene("DownloadScene", "DownloadScene", cancellationToken);
         
        }
        catch (OperationCanceledException)
        {

        }
    }

    public static async UniTask GameLoad(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await LoadScene("LoadingScene", "LoadingScene", cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        await LoadRegulation(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        GameInstance.GameIns.bgMSoundManager.Setup();

        cancellationToken.ThrowIfCancellationRequested();
        await LoadGameAsset(cancellationToken);
    }
    public static async UniTask<bool> IsAlreadyCached(string url, string targetUrl, int id)
    {
        await AssetLoader.GetServerUrl(url);

        string target = Path.Combine(AssetLoader.serverUrl, targetUrl);
        UnityWebRequest www = UnityWebRequest.Get(target + ".mani");
        await www.SendWebRequest();
        Hash128 hash = new Hash128();
        if(www.result == UnityWebRequest.Result.Success)
        {
            string manifestText = www.downloadHandler.text;

            Manifest manifest = JsonConvert.DeserializeObject<Manifest>(manifestText);

            hash = Hash128.Parse(manifest.hash);
            currentHashes[id] = new BundleCheck(hash, manifest.timeStamp);
        }
      //  Hash128 bundleHash = SaveLoadSystem.ComputeHash128(System.Text.Encoding.UTF8.GetBytes(target));

        long size = 0;
        using (UnityWebRequest request = UnityWebRequest.Head(target))
        {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                long.TryParse(request.GetResponseHeader("Content-Length"), out size);
            }
        }
      //  Debug.Log(hash + " " + bundleCheck[id].hash);
        if (!bundleCheck.ContainsKey(id)) return false;
        return Caching.IsVersionCached(target, hash) && hash == bundleCheck[id].hash && currentHashes[id].timeStamp == bundleCheck[id].timeStamp;
    }
    public async static UniTask LoadRegulation(CancellationToken cancellationToken = default)
    {
        try
        {
            await GameInstance.GameIns.assetLoader.Download_Regulation(cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
    }

    public static async UniTask LoadGameAsset(CancellationToken cancellationToken = default)
    {
        try
        {
            if (scenes["LoadingScene"].IsValid() && scenes["LoadingScene"].isLoaded)
            {
                GameObject[] rootObjects = scenes["LoadingScene"].GetRootGameObjects();

                foreach (GameObject go in rootObjects)
                {
                    if (go.name == "Loading")
                    {
                        loading = go.GetComponent<Loading>();
                        break;
                    }
                }
            }

            if (GameInstance.GameIns.assetLoader)
            {
                loading.ChangeText(gameSettings.language == Language.KOR ? "¿¡¼Â ·Îµù Áß" : "Loading assets");
                await GameInstance.GameIns.assetLoader.DownloadAssetBundle(cancellationToken);
                await UniTask.Delay(500);
                await LoadAssetWithBundle(cancellationToken);
                /*   UniTask.Void(async () =>
                   {
                       await GameInstance.GameIns.assetLoader.DownloadAssetBundle();
                   });
                   loading.ChangeText("?ì…‹ ë¡œë”© ì¤?);
                   Invoke("LoadAssetWithBundle", 0.5f);
       */
            }
        }
        catch (OperationCanceledException)
        {

        }
    }

    static async UniTask LoadSceneWithBundle(CancellationToken cancellationToken = default)
    {
        try
        {

            if (GameInstance.GameIns.assetLoader.sceneLoaded)
            {
                await GameLoaded(true, GlobalToken);
            }
            else
            {
                await UniTask.Delay(500, cancellationToken: cancellationToken);
                await LoadSceneWithBundle(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {

        }
    }
    static async UniTask LoadAssetWithBundle(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (GameInstance.GameIns.assetLoader.assetLoadSuccessful)
            {
                loading.ChangeText(gameSettings.language == Language.KOR ? "¸Ê ·Îµù Áß" : "Loading map");
                await GameInstance.GameIns.assetLoader.DownloadAsset_SceneBundle(cancellationToken);
                await UniTask.Delay(500, cancellationToken: cancellationToken);
                await LoadSceneWithBundle(cancellationToken);
            }
            else
            {
                await UniTask.Delay(500, cancellationToken: cancellationToken);
                await LoadAssetWithBundle(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {

        }
    }

    public static async UniTask GameLoaded(bool firstLoading, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if(!scenes.ContainsKey("InteractionScene")) await LoadScene("InteractionScene", AssetLoader.loadedMap["InteractionScene"], cancellationToken);
            await LoadScene(GameInstance.GameIns.assetLoader.gameRegulation.map_name, AssetLoader.loadedMap[GameInstance.GameIns.assetLoader.gameRegulation.map_name], cancellationToken);
            await LoadScene(GameInstance.GameIns.assetLoader.gameRegulation.map_name + "_gatcha", AssetLoader.loadedMap[GameInstance.GameIns.assetLoader.gameRegulation.map_name + "_gatcha"], cancellationToken);
            await LoadScene(GameInstance.GameIns.assetLoader.gameRegulation.map_name + "_fishing", AssetLoader.loadedMap[GameInstance.GameIns.assetLoader.gameRegulation.map_name + "_fishing"], cancellationToken);

            currentScene = SceneState.Restaurant;

            if(firstLoading)
            {
                loading.ChangeText(gameSettings.language == Language.KOR ? "½Ä´çÀ» ºÒ·¯¿À´Â Áß" : "Preparing the restaurant");
                await UniTask.WhenAll(LoadFont(cancellationToken), LoadRestaurant(cancellationToken));

                loadedAllAssets = true;
                loading.LoadingComplete();
            }
            else
            {
                await UniTask.WhenAll(LoadFont(cancellationToken), LoadRestaurant(cancellationToken));
            }
          
        }
        catch (OperationCanceledException)
        {

        }
    }

    static async UniTask LoadFont(CancellationToken cancellationToken = default)
    {
        try
        {
           // loading.ChangeText("ê¸€ê¼?ì¤€ë¹„ì¤‘");

            while (loadedScenesRootUI.Count > 0)
            {
                GameObject ui = loadedScenesRootUI[loadedScenesRootUI.Count - 1];
                loadedScenesRootUI.RemoveAt(loadedScenesRootUI.Count - 1);
                List<TMP_Text> texts = new List<TMP_Text>();
                ui.GetComponentsInChildren(true, texts);

                for (int i = texts.Count - 1; i >= 0; i--)
                {
                    GameObject g = texts[i].gameObject;

                    if (g.CompareTag("BMD"))
                    {
                        texts[i].font = AssetLoader.font;
                        texts[i].fontSharedMaterial = AssetLoader.font_mat;
                      //  texts[i].fontMaterial = AssetLoader.font_mat;
                      //  texts[i].material = AssetLoader.font_mat;
                    }
                    texts.RemoveAt(i);
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                }
            }
            /*    TMP_Text[] texts = FindObjectsOfType<TMP_Text>(true);

                foreach (TMP_Text text in texts)
                {
                    if (text.CompareTag("BMD"))
                    {
                        text.font = AssetLoader.font;
                        text.fontSharedMaterial = AssetLoader.font_mat;
                    }
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                }*/

        }
        catch (OperationCanceledException)
        {

        }
    }


    static async UniTask LoadRestaurant(CancellationToken cancellationToken = default)
    {
        try
        {
           // l/oading.ChangeText("ê²Œìž„ ?•ë³´ ë¡œë“œ ì¤?);
            await GameInstance.GameIns.restaurantManager.LoadRestaurant(cancellationToken);


        }
        catch (OperationCanceledException e)
        {
            Debug.Log(e);
        }
    }

    public void GetSceneUI(GameObject go)
    {
        loadedScenesRootUI.Add(go);
    }
    private void OnApplicationQuit()
    {/*
        if(GameInstance.GameIns != null)
        {
            GameInstance.GameIns.Clear();
            globalCts.Cancel();
            globalCts.Dispose();
            globalCts = null;

            Resources.UnloadUnusedAssets();
            if (GameInstance.GameIns.assetLoader != null)
            {
                if(GameInstance.GameIns.assetLoader.bundle != null)
                {
                    GameInstance.GameIns.assetLoader.bundle.Unload(true);
                    GameInstance.GameIns.assetLoader.bundle = null;
                }
                if (GameInstance.GameIns.assetLoader.bundle_scene != null)
                {
                    GameInstance.GameIns.assetLoader.bundle_scene.Unload(true);
                    GameInstance.GameIns.assetLoader.bundle_scene = null;
                }
                if (GameInstance.GameIns.assetLoader.b_reg != null)
                {
                    GameInstance.GameIns.assetLoader.b_reg.Unload(true);
                    GameInstance.GameIns.assetLoader.b_reg = null;
                }
            }
        }*/
    }

    public static async UniTask LoadScene(string name, string path, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

            await asyncLoad.ToUniTask(cancellationToken: cancellationToken);

            scenes[name] = SceneManager.GetSceneByName(name);
        }
        catch (OperationCanceledException)
        {

        }
    }
    public static void UnloadScene(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            UniTask.Void(async () =>
            {
                AsyncOperation asyncUnLoad = SceneManager.UnloadSceneAsync(name);

                await asyncUnLoad.ToUniTask(cancellationToken: cancellationToken);

                scenes.Remove(name);
            });
        }
        catch (OperationCanceledException)
        {

        }
    }
  
    public void ChangeScene_Restaurant()
    {
        if (currentScene == SceneState.Restaurant) return;
        GameInstance.GameIns.bgMSoundManager.BGMChange(901000, 0.4f);
        currentScene = SceneState.Restaurant;
  //      ((Action<TutorialEventKey>)EventManager.Publish(TutorialEventKey.EnterRestaurant))?.Invoke(TutorialEventKey.EnterRestaurant);

        if (RestaurantManager.tutorialKeys.Contains((int)TutorialEventKey.EnterRestaurant))
        {
            Tutorials tutorials = GameInstance.GameIns.restaurantManager.tutorials;
            TutorialStruct tutorialStruct = GameInstance.GameIns.restaurantManager.tutorialStructs[tutorials.id][tutorials.count - 1];
           // if (!tutorialStruct.event_start) Tutorials.TutorialUnlockLateTime(tutorialStruct);
            ((Action<TutorialEventKey>)EventManager.Publish(TutorialEventKey.EnterRestaurant))?.Invoke(TutorialEventKey.EnterRestaurant);
            GameInstance.GameIns.uiManager.TutorialStart(tutorials.id, tutorials.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tutorials.id].Count);
            RestaurantManager.tutorialKeys.Remove((int)TutorialEventKey.EnterRestaurant);
        }


        GameInstance.GameIns.playerCamera.brain.enabled = false;
        
        GameInstance.GameIns.inputManager.cameraTrans.position = pos;
        GameInstance.GameIns.inputManager.cameraTrans.rotation = Quaternion.Euler(0, 45, 0);
        InputManger.cachingCamera.transform.localPosition = new Vector3(0, 200, 0);
        InputManger.cachingCamera.transform.localRotation = Quaternion.Euler(60, 0, 0);
        //  GameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
        InputManger.cachingCamera.orthographic = true;
        InputManger.cachingCamera.orthographicSize = 15;
        GameInstance.GameIns.inputManager.InputDisAble = false;
        GameInstance.GameIns.uiManager.fishingStartButton.gameObject.SetActive(false);
        GameInstance.GameIns.applianceUIManager.UIClearAll(true);
        GameInstance.GameIns.gatcharManager.ClearRollings();
        SoundManager.Instance.RestaurantSoundsOnoff(true);
        GameInstance.GameIns.gatcharManager.ResetToken();
        GameInstance.GameIns.gatcharManager.virtualCamera1.Priority = 1;
        GameInstance.GameIns.gatcharManager.virtualCamera2.Priority = 0;
        GameInstance.GameIns.gatcharManager.virtualCamera3.Priority = 0;
        GameInstance.GameIns.gatcharManager.virtualCamera4.Priority = 0;
        GameInstance.GameIns.gatcharManager.autoPlaying = false;

        StartCoroutine(RestaurantSceneNextFrame());
        if(RestaurantManager.tutorialKeys.Contains(11000)) StartCoroutine(BlackConsumerTutorial());
     
        //   Utility.CheckHirable(GameInstance.GameIns.inputManager.cameraRange.position, ref i, ref j);
        //   StartCoroutine(OrthographicNextFrame());
    }
    
    IEnumerator RestaurantSceneNextFrame()
    {
        yield return null;
        restaurantTimeScale = 1;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }
    IEnumerator BlackConsumerTutorial()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        ((Action<int>)EventManager.Publish(-1, true)).Invoke(11000);
    }
   

    public void ChangeScene_DrawScene()
    {
        if (currentScene == SceneState.Draw) return;
        if(currentScene == SceneState.Restaurant) pos = GameInstance.GameIns.inputManager.cameraTrans.position;
        currentScene = SceneState.Draw;
        if(RestaurantManager.tutorialKeys.Contains((int)TutorialEventKey.EnterGatcha))
        {
            Tutorials tutorials = GameInstance.GameIns.restaurantManager.tutorials;
            ((Action<TutorialEventKey>)EventManager.Publish(TutorialEventKey.EnterGatcha))?.Invoke(TutorialEventKey.EnterGatcha);
            GameInstance.GameIns.uiManager.TutorialStart(tutorials.id, tutorials.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tutorials.id].Count);
            RestaurantManager.tutorialKeys.Remove((int)TutorialEventKey.EnterGatcha);
        }
        GameInstance.GameIns.bgMSoundManager.BGMChange(900100, 0.2f);
        GameInstance.GameIns.inputManager.InputDisAble = true;
       
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0f;
        restaurantTimeScale = 0f;
        InputManger.cachingCamera.orthographic = false;
        GameInstance.GameIns.playerCamera.brain.enabled = true;
        GameInstance.GameIns.gatcharManager.virtualCamera1.Priority = 1;
        GameInstance.GameIns.gatcharManager.virtualCamera2.Priority = 0;
        GameInstance.GameIns.gatcharManager.virtualCamera3.Priority = 0;
        GameInstance.GameIns.gatcharManager.virtualCamera4.Priority = 0;
        if (!GameInstance.GameIns.gatcharManager.CheckCompleteGatcha())
        {
            if (!GameInstance.GameIns.uiManager.drawBtn.gameObject.activeSelf) GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
            if (!GameInstance.GameIns.uiManager.drawSpeedUpBtn.gameObject.activeSelf) GameInstance.GameIns.uiManager.drawSpeedUpBtn.gameObject.SetActive(true);
        }
        else
        {
            if (GameInstance.GameIns.uiManager.drawBtn.gameObject.activeSelf) GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(false);
            if (GameInstance.GameIns.uiManager.drawSpeedUpBtn.gameObject.activeSelf) GameInstance.GameIns.uiManager.drawSpeedUpBtn.gameObject.SetActive(false);
        }
        GameInstance.GameIns.gatcharManager.CheckMoney();
        SoundManager.Instance.RestaurantSoundsOnoff(false);

        GameInstance.GameIns.uiManager.fishingBtn.gameObject.SetActive(false);  
    }
  

    public void ChangeScene_Fishing()
    {
        if(currentScene == SceneState.Fishing) return;
        if(currentScene == SceneState.Restaurant) pos = GameInstance.GameIns.inputManager.cameraTrans.position;
        currentScene = SceneState.Fishing;
        if(RestaurantManager.tutorialKeys.Contains((int)TutorialEventKey.EnterFishing))
        {
            RestaurantManager.tutorialKeys.Remove((int)TutorialEventKey.EnterFishing);
            ((Action<TutorialEventKey>)EventManager.Publish(TutorialEventKey.EnterFishing))?.Invoke(TutorialEventKey.EnterFishing);
            Tutorials tutorials = GameInstance.GameIns.restaurantManager.tutorials;
            GameInstance.GameIns.uiManager.TutorialStart(tutorials.id, tutorials.count, GameInstance.GameIns.restaurantManager.tutorialStructs[tutorials.id].Count);
        }

        GameInstance.GameIns.gatcharManager.autoPlaying = false;
        GameInstance.GameIns.bgMSoundManager.BGMChange(900010, 0.2f);
        GameInstance.GameIns.inputManager.InputDisAble = true;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
       
        restaurantTimeScale = 0f;
        InputManger.cachingCamera.fieldOfView = 60f;
        InputManger.cachingCamera.nearClipPlane = 0.1f;
        InputManger.cachingCamera.farClipPlane = 1000f;
        InputManger.cachingCamera.orthographic = false;
        GameInstance.GameIns.inputManager.cameraTrans.position = new Vector3(988, 30, 57);
        GameInstance.GameIns.inputManager.cameraTrans.rotation = Quaternion.Euler(0, 0, 0);
        InputManger.cachingCamera.transform.localPosition = new Vector3(0, 0, 0);
        InputManger.cachingCamera.transform.localRotation = Quaternion.Euler(60, 180, 0);

        if(!GameInstance.GameIns.fishingManager.working) GameInstance.GameIns.uiManager.fishingStartButton.gameObject.SetActive(true);
        SoundManager.Instance.RestaurantSoundsOnoff(false);
    }

    public static async UniTask UnloadAsync(string name)
    {
        if(scenes.ContainsKey(name))
        {
            scenes.TryGetValue(name, out Scene scene);
            await SceneManager.UnloadSceneAsync(scenes[name]);
            scenes.Remove(name);
        }
        else
        {
            foreach(var s in scenes)
            {
                Debug.Log(s.Key + " Exist");
            }
        }
    }


    public static void RefreshUI()
    {
        foreach (var v in languageTexts)
        {
            v.ChangeText();
        }
    }


    public static async UniTask GameExit(bool onlyRestaurant = false)
    {
        RestaurantManager restaurantManager = GameInstance.GameIns.restaurantManager;
        AnimalManager animalManager = GameInstance.GameIns.animalManager;
        AnimalPreviewManager previewManager = GameInstance.GameIns.animalPreviewManager;
        if (restaurantManager != null)
        {
            restaurantManager.GameSave();
        }
        if (animalManager != null)
        {
            for(int i = animalManager.employeeControllers.Count - 1; i >= 0; i--)
            {
                Employee employee = animalManager.employeeControllers[i];
                animalManager.DespawnEmployee(employee);
            }
            for(int i = animalManager.customerControllers.Count - 1; i >= 0; i--)
            {
                Customer customer = animalManager.customerControllers[i];
                animalManager.DespawnCustomer(customer);
            }
            if (animalManager.blackConsumer != null)
            {
                if (animalManager.blackConsumer.gameObject != null)
                {
                    BlackConsumer blackConsumer = animalManager.blackConsumer;
                    blackConsumer.consumerCallback -= animalManager.BlackConsumerAction;
                    blackConsumer.gameObject.SetActive(false);
                    blackConsumer = null;
                }
            }
        }

        if (onlyRestaurant)
        {
            globalCts.Cancel();
            List<FoodMachine> foodMachines = GameInstance.GameIns.workSpaceManager.foodMachines;
            for(int i = foodMachines.Count - 1;i >= 0;i--)
            {
               
                Destroy(foodMachines[i].gameObject);
            }
            List<Counter> counters = GameInstance.GameIns.workSpaceManager.counters;
            for(int i= counters.Count - 1;i >= 0;i--) { Destroy(counters[i].gameObject); }
            List<Table> tables = GameInstance.GameIns.workSpaceManager.tables;
            for(int i= tables.Count - 1; i>=0; i--) { Destroy(tables[i].gameObject); }
            await UnloadAsync(GameInstance.GameIns.assetLoader.gameRegulation.map_name);
            await UnloadAsync(GameInstance.GameIns.assetLoader.gameRegulation.map_name + "_gatcha");
            await UnloadAsync(GameInstance.GameIns.assetLoader.gameRegulation.map_name + "_fishing");
            globalCts = new();
            return;
        }
        else
        {
            if (previewManager != null)
            {
                foreach (var v in previewManager.previews)
                {
                    while (v.Value.Count > 0)
                    {
                        AnimalStagePreview preview = v.Value.Dequeue();
                        Destroy(preview.gameObject);
                    }
                }
            }
        }
        if (Camera.main != null)
        {
            Camera cam = Camera.main;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.cullingMask = 0;
        }
        UIManager uIManager = GameInstance.GameIns.uiManager;
        if (uIManager != null) uIManager.fadeImage.color = Color.black;
        if (GameInstance.GameIns != null)
        {
            GameInstance.GameIns.Clear();
            if(globalCts != null) globalCts.Cancel();
          

            await Resources.UnloadUnusedAssets();
            if (GameInstance.GameIns.assetLoader != null)
            {
                if (GameInstance.GameIns.assetLoader.bundle != null)
                {
                    GameInstance.GameIns.assetLoader.bundle.Unload(true);
                    GameInstance.GameIns.assetLoader.bundle = null;
                }
                if (GameInstance.GameIns.assetLoader.bundle_scene != null)
                {
                    GameInstance.GameIns.assetLoader.bundle_scene.Unload(true);
                    GameInstance.GameIns.assetLoader.bundle_scene = null;
                }
                if (GameInstance.GameIns.assetLoader.b_reg != null)
                {
                    GameInstance.GameIns.assetLoader.b_reg.Unload(true);
                    GameInstance.GameIns.assetLoader.b_reg = null;
                }
            }
        }

        await UniTask.Yield();
        if (globalCts != null) globalCts.Dispose();
        globalCts = null;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }


    public void StartEscapeTab(InputAction.CallbackContext callbackContext)
    {
        escapeTabCount++;
        if(escapeTabCount > 1)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidToast.Close();
#endif
            //ê²Œìž„ ì¢…ë£Œ
            GameExit().Forget();
        }
        else StartCoroutine(EscapeTab());
    }
    
    IEnumerator EscapeTab()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidToast.Show(gameSettings.language == Language.KOR ? "´Ù½Ã ÅÇÇÏ¿© ³ª°¡±â" : "Tap again to exit");
#endif
        yield return new WaitForSecondsRealtime(1f);
        escapeTabCount--;
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidToast.Close();
#endif
    }
}
