using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.U2D;
using Unity.Collections.LowLevel.Unsafe;
using TMPro;
public class AssetLoader : MonoBehaviour
{
    [NonSerialized]
    public AssetBundle bundle;
    [NonSerialized]
    public AssetBundle bundle_scene;
    public static Dictionary<string, string> loadedMap = new Dictionary<string, string>();
    public static Dictionary<string, GameObject> loadedAssets = new Dictionary<string, GameObject>();
    public static Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();
    public static Dictionary<string, Sprite> atlasSprites = new Dictionary<string, Sprite>();
    public static Dictionary<string, SpriteAtlas> loadedAtlases = new Dictionary<string, SpriteAtlas>();
    public static Dictionary<int, ItemStruct> items = new Dictionary<int, ItemStruct>();
    public static Dictionary<int, ItemStruct> sprites = new Dictionary<int, ItemStruct>();
    public static Dictionary<int, ItemStruct> atlases = new Dictionary<int, ItemStruct>();
    public static Dictionary<int, MachineLevelStruct> machines_levels = new Dictionary<int, MachineLevelStruct>();
    public static Dictionary<int, EmployeeLevelStruct> employees_levels = new Dictionary<int, EmployeeLevelStruct>();
    public static Dictionary<int, AnimalStruct> animals = new Dictionary<int, AnimalStruct>();
    public static Dictionary<int, GoodsStruct> goods = new Dictionary<int, GoodsStruct>();
    public static Dictionary<int, StringStruct> itemAssetKeys = new Dictionary<int, StringStruct>();
    public static Dictionary<int, StringStruct> spriteAssetKeys = new Dictionary<int, StringStruct>();
    public static Dictionary<int, StringStruct> atlasesKeys = new Dictionary<int, StringStruct>();
    public static Dictionary<int, LevelData> levelData = new Dictionary<int, LevelData>();
    public static List<RestaurantParam> restaurantParams = new List<RestaurantParam>();

    public static TMP_FontAsset font;
    public static Material font_mat;
    int unloadNum;
    public bool assetLoadSuccessful;
    public bool sceneLoaded;
    string[] tables = new string[9]
    {
        "all", "employees", "machines", "animals", "level","furniture", "sprites", "atlases", "shop"
    };
    Dictionary<string, string> tableContents = new Dictionary<string, string>();


    private void Awake()
    {
        GameInstance.GameIns.assetLoader = this;
    }
    public async UniTask DownloadAsset_SceneBundle(CancellationToken cancellationToken = default)
    {
        try
        {
            string homeUrl = Path.Combine(SaveLoadSystem.LoadServerURL(), "restaurant_scene");
            Debug.Log(homeUrl);
            Hash128 bundleHash = SaveLoadSystem.ComputeHash128(System.Text.Encoding.UTF8.GetBytes(homeUrl));
            if (Caching.IsVersionCached(homeUrl, bundleHash))
            {
                Debug.Log("Asset Found");
            }
            else
            {
                Debug.Log("Asset Not Found");
            }
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(homeUrl, bundleHash, 0);
            await www.SendWebRequest();
            
            if (www.result == UnityWebRequest.Result.Success)
            {
                bundle_scene = DownloadHandlerAssetBundle.GetContent(www);
                if (Caching.IsVersionCached(homeUrl, bundleHash))
                {
                    Debug.Log("AssetBundle Cached Successfully");
                }

                string[] scenePaths = bundle_scene.GetAllScenePaths();

                foreach (string scenePath in scenePaths)
                {
                    string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                    if (!loadedMap.ContainsKey(sceneName))
                    {
                        loadedMap.Add(sceneName, scenePath);
                    }
                }

                if (!bundle.isStreamedSceneAssetBundle)
                {
                    sceneLoaded = true;
                }
            }
        }
        catch (OperationCanceledException)
        {

        }
    }

    public async UniTask DownloadAssetBundle(CancellationToken cancellationToken = default)
    {
        try
        {
            string homeUrl = SaveLoadSystem.LoadServerURL() + "/restaurant";
            Hash128 bundleHash = SaveLoadSystem.ComputeHash128(System.Text.Encoding.UTF8.GetBytes(homeUrl));
            if (Caching.IsVersionCached(homeUrl, bundleHash))
            {
                Debug.Log("Asset Found");
            }
            else
            {
                Debug.Log("Asset Not Found");
            }
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(homeUrl, bundleHash, 0);
            await www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                bundle = DownloadHandlerAssetBundle.GetContent(www);

                if (Caching.IsVersionCached(homeUrl, bundleHash))
                {
                    Debug.Log("AssetBundle Cached Successfully");
                }

                if (!bundle.isStreamedSceneAssetBundle)
                {
                    await DatatableLoad();

                    await UniTask.RunOnThreadPool(() =>
                    {
                        items = SaveLoadSystem.GetDictionaryData<int, ItemStruct>(tableContents["all"]);
                        sprites = SaveLoadSystem.GetDictionaryData<int, ItemStruct>(tableContents["sprites"]);
                        atlases = SaveLoadSystem.GetDictionaryData<int, ItemStruct>(tableContents["atlases"]);
                        goods = SaveLoadSystem.GetDictionaryData<int, GoodsStruct>(tableContents["shop"]);
                        machines_levels = SaveLoadSystem.GetDictionaryData<int, MachineLevelStruct>(tableContents["machines"]);
                        animals = SaveLoadSystem.GetDictionaryData<int, AnimalStruct>(tableContents["animals"]);
                        employees_levels = SaveLoadSystem.GetDictionaryData<int, EmployeeLevelStruct>(tableContents["employees"]);
                        levelData = SaveLoadSystem.GetDictionaryDataClass<int, LevelData>(tableContents["level"]);
                        restaurantParams = SaveLoadSystem.GetListData<RestaurantParam>(tableContents["furniture"]);
                    });
                    foreach (KeyValuePair<int, ItemStruct> keyValuePair in items) itemAssetKeys[keyValuePair.Key] = new StringStruct(keyValuePair.Value.asset_name);
                    foreach (KeyValuePair<int, ItemStruct> keyValuePair in sprites) spriteAssetKeys[keyValuePair.Key] = new StringStruct(keyValuePair.Value.asset_name);
                    foreach (KeyValuePair<int, ItemStruct> keyValuePair in atlases) atlasesKeys[keyValuePair.Key] = new StringStruct(keyValuePair.Value.asset_name);

                    SpriteAtlasManager.atlasRequested += (tag, callback) =>
                    {
                        Debug.Log($"[TAG]: {tag}");
                        callback(loadedAtlases[tag]);

                    };

                    await LoadAsync<SpriteAtlas, StringStruct, string>(atlasesKeys, loadedAtlases);
                    await LoadAsync<GameObject, StringStruct, string>(itemAssetKeys, loadedAssets);
                    await LoadAsync<Sprite, StringStruct, string>(spriteAssetKeys, loadedSprites);

                    AssetBundleRequest assetRequest = bundle.LoadAssetAsync<TMP_FontAsset>("BMDOHYEON_ttf");
                    await assetRequest;
                    if(assetRequest != null)
                    {
                        if (assetRequest.asset is TMP_FontAsset castedAsset)
                        {
                            font = castedAsset;
                            font_mat = font.material;
                        }
                    }
                }
            }
            else
            {
                unloadNum++;
                Debug.Log("Error");
            }
            if (unloadNum == 0)
            {
                assetLoadSuccessful = true;


            }
        }
        catch (OperationCanceledException)
        {

        }
    }


    async UniTask DatatableLoad()
    {
        for (int i = 0; i < tables.Length; i++)
        {
            AssetBundleRequest request = bundle.LoadAssetAsync<TextAsset>(tables[i]);
            await request;

            if (request != null)
            {
                TextAsset itemTextAsset = (TextAsset)request.asset;
                tableContents.Add(tables[i], itemTextAsset.text);
            }
        }
    }
    async UniTask LoadAsync<T, K, V>(Dictionary<int, K> keyValues, Dictionary<string, T> outputs) where K : struct, ITableID<V>
    {
        foreach (KeyValuePair<int, K> keyValue in keyValues)
        {
            if (bundle.Contains(keyValue.Value.Name))
            {
                AssetBundleRequest assetRequest = bundle.LoadAssetAsync<T>(keyValue.Value.Name);
                await assetRequest;
                if (assetRequest != null)
                {
                    if (assetRequest.asset is T castedAsset)
                    {
                        outputs[keyValue.Value.Name] = castedAsset;
                        Debug.Log(keyValue + " loaded");
                    }
                }
                else
                {
                    Debug.Log(keyValue + " loaded fail");
                    unloadNum++;
                }
            }
        }
    }
}
