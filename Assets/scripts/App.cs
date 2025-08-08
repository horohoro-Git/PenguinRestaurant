using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Newtonsoft.Json;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEngine.InputSystem.EnhancedTouch;
using AnimationInstancing;

public enum SceneState
{
    Menu,
    Loading,
    Restaurant,
    Draw,
    Fishing
}

public class App : MonoBehaviour
{
    private static CancellationTokenSource globalCts = new();
    public static CancellationToken GlobalToken => globalCts.Token;

    public static SceneState currentScene;
    public static float restaurantTimeScale = 1f;
    //public static Language language;
    public static Queue<LanguageText> languageTexts = new();
    public static Dictionary<int, LanguageScript> languages = new();
    public static GameSettings gameSettings;
    Vector3 vector;
    public Vector3 pos { get { return vector; } set { vector = value; Debug.Log(value); } }
    public static Dictionary<string, Scene> scenes = new();
    static Loading loading;
    public static bool loadedAllAssets;
    static List<GameObject> loadedScenesRootUI = new();
    static int currentLevel;
    public static int CurrentLevel { get { return currentLevel; } set { currentLevel = value; } }
    public static Dictionary<int, BundleCheck> bundleCheck = new();
    public static Dictionary<int, BundleCheck> currentHashes = new();
    public static Dictionary<int, List<BundleCheck>> cachedData = new();

    [SerializeField]
    PlayerInput playerInput;
    int escapeTabCount = 0;
  
    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        currentScene = SceneState.Restaurant;
        GameInstance.GameIns.app = this;
        DontDestroyOnLoad(this);
       
        //설정 정보 가져오기
        gameSettings = SaveLoadSystem.LoadGameSettings();
        bundleCheck = SaveLoadSystem.LoadDownloadedData();
        cachedData = SaveLoadSystem.LoadCachedDownloadedData();
        

        //언어 설정
        TextAsset lang = null;
        if (gameSettings.language == Language.KOR) lang = Resources.Load<TextAsset>("language_kor");
        else lang = Resources.Load<TextAsset>("language_eng");
      
        List<LanguageScript> l = JsonConvert.DeserializeObject<List<LanguageScript>>(lang.text);
        languages.Clear();
        for (int i = 0; i < l.Count; i++) languages[l[i].id] = l[i];

        //로딩
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
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }
    private void OnDisable()
    {
        if(playerInput != null) playerInput.actions["EscapeTab"].started -= StartEscapeTab;
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
    }
#if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
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
            await LoadScene("DownloadScene", cancellationToken);
         
        }
        catch (OperationCanceledException)
        {

        }
    }

    public static async UniTask GameLoad(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await LoadScene("LoadingScene", cancellationToken);

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
                loading.ChangeText(gameSettings.language == Language.KOR ? "에셋 로딩 중" : "Loading assets");
                await GameInstance.GameIns.assetLoader.DownloadAssetBundle(cancellationToken);
                await UniTask.Delay(500);
                await LoadAssetWithBundle(cancellationToken);
                /*   UniTask.Void(async () =>
                   {
                       await GameInstance.GameIns.assetLoader.DownloadAssetBundle();
                   });
                   loading.ChangeText("에셋 로딩 중");
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
                await GameLoaded(GlobalToken);
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
                loading.ChangeText(gameSettings.language == Language.KOR ? "맵 로딩 중" : "Loading map");
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

    static async UniTask GameLoaded(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await LoadScene(AssetLoader.loadedMap["InteractionScene"], cancellationToken);
            await LoadScene(AssetLoader.loadedMap[GameInstance.GameIns.assetLoader.gameRegulation.map_name], cancellationToken);
            await LoadScene(AssetLoader.loadedMap[GameInstance.GameIns.assetLoader.gameRegulation.map_name + "_gatcha"], cancellationToken);
            await LoadScene(AssetLoader.loadedMap[GameInstance.GameIns.assetLoader.gameRegulation.map_name + "_fishing"], cancellationToken);

            currentScene = SceneState.Restaurant;
            loading.ChangeText(gameSettings.language == Language.KOR ? "식당을 불러오는 중" : "Preparing the restaurant");
            await UniTask.WhenAll(LoadFont(cancellationToken), LoadRestaurant(cancellationToken));
       
            loadedAllAssets = true;
            loading.LoadingComplete();
        }
        catch (OperationCanceledException)
        {

        }
    }

    static async UniTask LoadFont(CancellationToken cancellationToken = default)
    {
        try
        {
           // loading.ChangeText("글꼴 준비중");

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
           // l/oading.ChangeText("게임 정보 로드 중");
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

    public static async UniTask LoadScene(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            AsyncOperation asyncUnLoad = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

            await asyncUnLoad.ToUniTask(cancellationToken: cancellationToken);

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
        restaurantTimeScale = 1;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
//        InputManger.cachingCamera.enabled = false;
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
        //   Utility.CheckHirable(GameInstance.GameIns.inputManager.cameraRange.position, ref i, ref j);
        //   StartCoroutine(OrthographicNextFrame());
    }
   

    public void ChangeScene_DrawScene()
    {
        if (currentScene == SceneState.Draw) return;
        if(currentScene == SceneState.Restaurant) pos = GameInstance.GameIns.inputManager.cameraTrans.position;
        currentScene = SceneState.Draw;
        GameInstance.GameIns.bgMSoundManager.BGMChange(900100, 0.2f);
      //  GameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
        GameInstance.GameIns.inputManager.InputDisAble = true;
       
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0f;
        restaurantTimeScale = 0f;
     //   InputManger.cachingCamera.orthographicSize = 15;
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
        GameInstance.GameIns.bgMSoundManager.BGMChange(900010, 0.2f);
        GameInstance.GameIns.inputManager.InputDisAble = true;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
       
        restaurantTimeScale = 0f;
       // InputManger.cachingCamera.enabled = false;
        InputManger.cachingCamera.fieldOfView = 60f;
        InputManger.cachingCamera.nearClipPlane = 0.1f;
        InputManger.cachingCamera.farClipPlane = 1000f;
        InputManger.cachingCamera.orthographic = false;
      //  InputManger.cachingCamera.enabled = true;
        GameInstance.GameIns.inputManager.cameraTrans.position = new Vector3(988, 30, 57);
        GameInstance.GameIns.inputManager.cameraTrans.rotation = Quaternion.Euler(0, 0, 0);
        InputManger.cachingCamera.transform.localPosition = new Vector3(0, 0, 0);
        InputManger.cachingCamera.transform.localRotation = Quaternion.Euler(60, 180, 0);

        if(!GameInstance.GameIns.fishingManager.working) GameInstance.GameIns.uiManager.fishingStartButton.gameObject.SetActive(true);
        SoundManager.Instance.RestaurantSoundsOnoff(false);
        //     GameInstance.GameIns.fishingManager.StartFishing();
    }

    public static void UnloadAsync(string name)
    {
        if(scenes.ContainsKey(name))
        {
            SceneManager.UnloadSceneAsync(scenes[name]);
            scenes.Remove(name);
        }
    }


    public static void RefreshUI()
    {
        foreach (var v in languageTexts)
        {
            v.ChangeText();
        }
    }


    public static async UniTask GameExit()
    {
        if (Camera.main != null)
        {
            Camera cam = Camera.main;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.cullingMask = 0;
        }
        UIManager uIManager = GameInstance.GameIns.uiManager;
        if (uIManager != null) uIManager.fadeImage.color = Color.black;
        RestaurantManager restaurantManager = GameInstance.GameIns.restaurantManager;
        AnimalManager animalManager = GameInstance.GameIns.animalManager;
      //  AnimationInstancingMgr animationInstancingMgr = GameInstance.GameIns.animationInstancingManager;
     //   if(animationInstancingMgr != null) animationInstancingMgr.gameObject.SetActive(false);
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
        Debug.LogWarning("Good");
        escapeTabCount++;
        if(escapeTabCount > 1)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidToast.Close();
#endif
            //게임 종료
            GameExit().Forget();
        }
        else StartCoroutine(EscapeTab());
    }
    
    IEnumerator EscapeTab()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidToast.Show(gameSettings.language == Language.KOR ? "다시 탭하여 나가기" : "Tap again to exit");
#endif
        yield return new WaitForSecondsRealtime(1f);
        escapeTabCount--;
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidToast.Close();
#endif
    }
}
