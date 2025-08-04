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
using Newtonsoft.Json;
public class AssetLoader : MonoBehaviour
{
    [NonSerialized]
    public AssetBundle bundle;
    [NonSerialized]
    public AssetBundle bundle_scene;
    [NonSerialized]
    public AssetBundle b_reg;
    public static Dictionary<string, AudioClip> loadedSounds = new Dictionary<string, AudioClip>();
    public static Dictionary<string, string> loadedMap = new Dictionary<string, string>();
    public static Dictionary<string, GameObject> loadedAssets = new Dictionary<string, GameObject>();
    public static Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();
    public static Dictionary<string, Sprite> atlasSprites = new Dictionary<string, Sprite>();
    public static Dictionary<string, SpriteAtlas> loadedAtlases = new Dictionary<string, SpriteAtlas>();
    public static Dictionary<int, ItemStruct> items = new Dictionary<int, ItemStruct>();
    public static Dictionary<int, ItemStruct> sprites = new Dictionary<int, ItemStruct>();
    public static Dictionary<int, ItemStruct> atlases = new Dictionary<int, ItemStruct>();
    public static Dictionary<int, MachineLevelData> machines_levels = new Dictionary<int, MachineLevelData>();

    public static Dictionary<int, EmployeeLevelStruct> employees_levels = new Dictionary<int, EmployeeLevelStruct>();
    public static Dictionary<int, MachineLevelOffset> machineLevelOffsets = new Dictionary<int, MachineLevelOffset>();
    public static Dictionary<int, AnimalStruct> animals = new Dictionary<int, AnimalStruct>();
    public static Dictionary<int, GoodsStruct> goods = new Dictionary<int, GoodsStruct>();
    public static Dictionary<int, StringStruct> itemAssetKeys = new Dictionary<int, StringStruct>();
    public static Dictionary<int, StringStruct> spriteAssetKeys = new Dictionary<int, StringStruct>();
    public static Dictionary<int, StringStruct> atlasesKeys = new Dictionary<int, StringStruct>();
    public static Dictionary<int, LevelData> levelData = new Dictionary<int, LevelData>();
    public static Dictionary<int, ItemStruct> sounds = new Dictionary<int, ItemStruct>();
    public static Dictionary<int, ItemStruct> fishingAnimals = new Dictionary<int, ItemStruct>();
    public static Dictionary<int, AnimalPersnality> animalPersonalities = new Dictionary<int, AnimalPersnality>();
    public static List<MapContent> maps = new List<MapContent>();
    public static List<RestaurantParam> restaurantParams = new List<RestaurantParam>();

    public static TMP_FontAsset font;
    public static Material font_mat;
    int unloadNum;
    public bool assetLoadSuccessful;
    public bool sceneLoaded;
    public string mapContents;
    public string soundContents;

    public GameRegulation gameRegulation;

    string[] tables = new string[12]
    {
       "all", "employees", "machines", "machine_level_offsets", "animals", "level","furniture", "sprites", "atlases", "shop", "fishing", "animal_personality"
    };
    Dictionary<string, string> tableContents = new Dictionary<string, string>();

    public static string serverUrl;
    private void Awake()
    {
        GameInstance.GameIns.assetLoader = this;
        SpriteAtlasManager.atlasRequested += (tag, callback) =>
        {
         //   Debug.Log($"[TAG]: {tag}");
            if(loadedAtlases.ContainsKey(tag)) callback(loadedAtlases[tag]);

        };
    }

    public static async UniTask GetServerUrl(string addUrl, CancellationToken cancellationToken = default)
    {
        serverUrl = await SaveLoadSystem.LoadServerURL(cancellationToken);
        if(addUrl != "") serverUrl = Path.Combine(serverUrl, addUrl);
#if UNITY_IOS || UNITY_ANDROID
        serverUrl = Path.Combine(serverUrl, "android");
#else
        serverUrl = Path.Combine(serverUrl, "pc");
#endif
    }

    public async UniTask Download_Regulation(CancellationToken cancellationToken = default)
    {
        await GetServerUrl("", cancellationToken);
        string target = Path.Combine(serverUrl, "map");
        //  Hash128 bundleHash = SaveLoadSystem.ComputeHash128(System.Text.Encoding.UTF8.GetBytes(target));
        Hash128 bundleHash = App.bundleCheck[1].hash;
        if (Caching.IsVersionCached(target, bundleHash))
        {
            Debug.Log("Asset Found");
        }
        else
        {
            Debug.Log("Asset Not Found");
        }
        cancellationToken.ThrowIfCancellationRequested();
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(target, bundleHash, 0);
        await www.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);

        if (www.result == UnityWebRequest.Result.Success)
        {
            b_reg = DownloadHandlerAssetBundle.GetContent(www);
            if (Caching.IsVersionCached(target, bundleHash))
            {
                Debug.Log("AssetBundle Cached Successfully");
            }
            cancellationToken.ThrowIfCancellationRequested();
            AssetBundleRequest assetRequest = b_reg.LoadAssetAsync<TextAsset>("maps");
            await assetRequest.ToUniTask(cancellationToken: cancellationToken);
            if (assetRequest != null)
            {
                mapContents = assetRequest.asset.ToString();
                maps = SaveLoadSystem.GetListData<MapContent>(mapContents);

                gameRegulation = SaveLoadSystem.LoadGameRegulation();
            }
            cancellationToken.ThrowIfCancellationRequested();
            AssetBundleRequest soundsRequest = b_reg.LoadAssetAsync<TextAsset>("sounds");
            await soundsRequest.ToUniTask(cancellationToken: cancellationToken);
            if (soundsRequest != null)
            {
                soundContents = soundsRequest.asset.ToString();
                sounds = SaveLoadSystem.GetDictionaryData<int, ItemStruct>(soundContents);

                foreach (var sound in sounds) 
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    AssetBundleRequest soundAsset = b_reg.LoadAssetAsync<AudioClip>(sound.Value.asset_name);
                    await soundAsset.ToUniTask(cancellationToken: cancellationToken);
                    if (soundAsset != null)
                    {
                        loadedSounds[sound.Value.asset_name] = (AudioClip)soundAsset.asset;
                    }
                }
            }
        }
    }

    public async UniTask DownloadAsset_SceneBundle(CancellationToken cancellationToken = default)
    {
        try
        {
           // string serverUrl = await SaveLoadSystem.LoadServerURL();
            string homeUrl = Path.Combine(serverUrl, gameRegulation.map_name + "_scene");
            //Hash128 bundleHash = SaveLoadSystem.ComputeHash128(System.Text.Encoding.UTF8.GetBytes(homeUrl));
            Hash128 bundleHash = App.bundleCheck[2].hash;
            if (Caching.IsVersionCached(homeUrl, bundleHash))
            {
                Debug.Log("Asset Found");
            }
            else
            {
                Debug.Log("Asset Not Found");
            }
            cancellationToken.ThrowIfCancellationRequested();
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(homeUrl, bundleHash, 0);
            await www.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);
            
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
            await GetServerUrl(gameRegulation.map_name, cancellationToken);
            string homeUrl = Path.Combine(serverUrl, gameRegulation.map_name);
            // Hash128 bundleHash = SaveLoadSystem.ComputeHash128(System.Text.Encoding.UTF8.GetBytes(homeUrl));
            Hash128 bundleHash = App.bundleCheck[3].hash;
            if (Caching.IsVersionCached(homeUrl, bundleHash))
            {
                Debug.Log("Asset Found");
            }
            else
            {
                Debug.Log("Asset Not Found");
            }
            cancellationToken.ThrowIfCancellationRequested();
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(homeUrl, bundleHash, 0);
            await www.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);
            if (www.result == UnityWebRequest.Result.Success)
            {
                bundle = DownloadHandlerAssetBundle.GetContent(www);

                if (Caching.IsVersionCached(homeUrl, bundleHash))
                {
                    Debug.Log("AssetBundle Cached Successfully");
                }

                if (!bundle.isStreamedSceneAssetBundle)
                {
                    await DatatableLoad(cancellationToken);

                    await UniTask.RunOnThreadPool(() =>
                    {
                        
                        items = SaveLoadSystem.GetDictionaryData<int, ItemStruct>(tableContents["all"]);
                        sprites = SaveLoadSystem.GetDictionaryData<int, ItemStruct>(tableContents["sprites"]);
                        atlases = SaveLoadSystem.GetDictionaryData<int, ItemStruct>(tableContents["atlases"]);
                        goods = SaveLoadSystem.GetDictionaryData<int, GoodsStruct>(tableContents["shop"]);
                        animals = SaveLoadSystem.GetDictionaryData<int, AnimalStruct>(tableContents["animals"]);
                        fishingAnimals = SaveLoadSystem.GetDictionaryData<int, ItemStruct>(tableContents["fishing"]);
                        employees_levels = SaveLoadSystem.GetDictionaryData<int, EmployeeLevelStruct>(tableContents["employees"]);
                        machines_levels = SaveLoadSystem.GetDictionaryDataClass<int, MachineLevelData>(tableContents["machines"]);
                        levelData = SaveLoadSystem.GetDictionaryDataClass<int, LevelData>(tableContents["level"]);
                        restaurantParams = SaveLoadSystem.GetListData<RestaurantParam>(tableContents["furniture"]);
                        machineLevelOffsets = SaveLoadSystem.GetDictionaryData<int, MachineLevelOffset>(tableContents["machine_level_offsets"]);
                        animalPersonalities = SaveLoadSystem.GetDictionaryData<int, AnimalPersnality>(tableContents["animal_personality"]);
                    }, cancellationToken: cancellationToken);
                    foreach (KeyValuePair<int, ItemStruct> keyValuePair in items) itemAssetKeys[keyValuePair.Key] = new StringStruct(keyValuePair.Value.asset_name);
                    foreach (KeyValuePair<int, ItemStruct> keyValuePair in sprites) spriteAssetKeys[keyValuePair.Key] = new StringStruct(keyValuePair.Value.asset_name);
                    foreach (KeyValuePair<int, ItemStruct> keyValuePair in atlases) atlasesKeys[keyValuePair.Key] = new StringStruct(keyValuePair.Value.asset_name);

                    await LoadAsync<SpriteAtlas, StringStruct, string>(atlasesKeys, loadedAtlases, cacellationToken: cancellationToken);
                    await LoadAsync<GameObject, StringStruct, string>(itemAssetKeys, loadedAssets, cacellationToken: cancellationToken);
                    await LoadAsync<Sprite, StringStruct, string>(spriteAssetKeys, loadedSprites, cacellationToken: cancellationToken);


                    cancellationToken.ThrowIfCancellationRequested();
                    AssetBundleRequest assetRequest = bundle.LoadAssetAsync<TMP_FontAsset>("BMDOHYEON_ttf SDF");
                  
                    await assetRequest.ToUniTask(cancellationToken: cancellationToken);
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
            Debug.Log("AssetLoadCanceled");
        }
    }


    async UniTask DatatableLoad(CancellationToken cancellationToken = default)
    {
        for (int i = 0; i < tables.Length; i++)
        {
            AssetBundleRequest request = bundle.LoadAssetAsync<TextAsset>(tables[i]);
            await request.ToUniTask(cancellationToken: cancellationToken);

            if (request != null)
            {
                TextAsset itemTextAsset = (TextAsset)request.asset;
                tableContents.Add(tables[i], itemTextAsset.text);
            }
        }
    }
    async UniTask LoadAsync<T, K, V>(Dictionary<int, K> keyValues, Dictionary<string, T> outputs, CancellationToken cacellationToken = default) where K : struct, ITableID<V>
    {
        foreach (KeyValuePair<int, K> keyValue in keyValues)
        {
            if (bundle.Contains(keyValue.Value.Name))
            {
                AssetBundleRequest assetRequest = bundle.LoadAssetAsync<T>(keyValue.Value.Name);
                await assetRequest.ToUniTask(cancellationToken: cacellationToken);
                if (assetRequest != null)
                {
                    if (assetRequest.asset is T castedAsset)
                    {
                        outputs[keyValue.Value.Name] = castedAsset;
                   //     Debug.Log(keyValue + " loaded");
                    }
                }
                else
                {
                 //   Debug.Log(keyValue + " loaded fail");
                    unloadNum++;
                }
            }
        }
    }
}
