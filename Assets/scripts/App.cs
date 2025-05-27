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
//한글

public enum SceneState
{
    Menu,
    Loading,
    Restaurant,
    Draw
}

public class App : MonoBehaviour
{
    private static CancellationTokenSource globalCts = new CancellationTokenSource();
    public static CancellationToken GlobalToken => globalCts.Token;

    public SceneState currentScene;

    Vector3 vector;
    public Vector3 pos { get { return vector; } set { vector = value; Debug.Log(value); } }
    static Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
    Loading loading;
    public static bool loadedAllAssets;
    List<GameObject> loadedScenesRootUI = new List<GameObject>();
    private void Awake()
    {
        string test = "1";
        for (int i = 0; i < 300000; i++) test += "0";
        BigInteger bigInteger = BigInteger.Parse(test);
        Debug.Log(Utility.GetFormattedMoney(bigInteger));
        currentScene = SceneState.Restaurant;
        GameInstance.GameIns.app = this;
        DontDestroyOnLoad(this);
    
        //JustTest().Forget();
        if (!scenes.ContainsKey("LoadingScene")) LoadLoadingScene(GlobalToken).Forget();
    }

    async UniTask LoadLoadingScene(CancellationToken cancellationToken = default)
    {
        try
        {
            await AssetLoader.GetServerUrl();

            cancellationToken.ThrowIfCancellationRequested();
            await LoadScene("LoadingScene", cancellationToken);

            await LoadGameAsset(cancellationToken);
        }
        catch (OperationCanceledException)
        {

        }
    }

    async UniTask LoadGameAsset(CancellationToken cancellationToken = default)
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
                loading.ChangeText("에셋 로딩 중");
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

    async UniTask LoadSceneWithBundle(CancellationToken cancellationToken = default)
    {
        try
        {

            if (GameInstance.GameIns.assetLoader.sceneLoaded)
            {
                Debug.Log("Load");
                await GameLoaded(GlobalToken);
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
    async UniTask LoadAssetWithBundle(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (GameInstance.GameIns.assetLoader.assetLoadSuccessful)
            {
                loading.ChangeText("맵 로딩 중");
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

    async UniTask GameLoaded(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await LoadScene(AssetLoader.loadedMap["InteractionScene"], cancellationToken);
            await LoadScene(AssetLoader.loadedMap["RestaurantScene"], cancellationToken);
            await LoadScene(AssetLoader.loadedMap["GatCharScene_Town"], cancellationToken);

            currentScene = SceneState.Restaurant;
            loading.ChangeText("식당을 불러오는 중");
            await UniTask.WhenAll(LoadFont(cancellationToken), LoadRestaurant(cancellationToken));
       
            loadedAllAssets = true;
            loading.LoadingComplete();
        }
        catch (OperationCanceledException)
        {

        }
    }

    async UniTask LoadFont(CancellationToken cancellationToken = default)
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


    async UniTask LoadRestaurant(CancellationToken cancellationToken = default)
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
    {
        if(GameInstance.GameIns != null)
        {
            GameInstance.GameIns.Clear();
            globalCts.Cancel();
            globalCts.Dispose();
            globalCts = null;
        }
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
        currentScene = SceneState.Restaurant; 
        GameInstance.GameIns.inputManager.cameraTrans.position = pos;
      //  GameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
        GameInstance.GameIns.inputManager.inputDisAble = false;
        GameInstance.GameIns.applianceUIManager.UIClearAll(true);
        GameInstance.GameIns.gatcharManager.ClearRollings();

     //   Utility.CheckHirable(GameInstance.GameIns.inputManager.cameraRange.position, ref i, ref j);
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }

    public void ChangeScene_DrawScene()
    {
        if (currentScene == SceneState.Draw) return;
        currentScene = SceneState.Draw;
      //  GameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
        GameInstance.GameIns.inputManager.inputDisAble = true;
        pos = GameInstance.GameIns.inputManager.cameraTrans.position;
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0f;
        GameInstance.GameIns.inputManager.cameraTrans.position = GameInstance.GetVector3(-80.35f, 0, -1080.7f);
        GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.drawSpeedUpBtn.gameObject.SetActive(true);

    }

    public static void UnloadAsync(string name)
    {
        if(scenes.ContainsKey(name))
        {
            SceneManager.UnloadSceneAsync(scenes[name]);
            scenes.Remove(name);
        }
    }
}
