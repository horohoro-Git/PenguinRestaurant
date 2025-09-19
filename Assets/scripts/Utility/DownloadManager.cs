using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class DownloadManager : MonoBehaviour
{
    public GameObject downloadBorder;
    public Button downloadBtn;

    public TMP_Text downloadAmount;
    public Image downloadGage;
    public TMP_Text downloadText;
    public TMP_Text downloadPercentage;
    public TMP_Text downloadState;
    public GameObject downloadingStatus;
    public Button continueBtn;
    public TMP_Text updateText;
    public Image loadingImage;
    public GameObject serverFailed;
    public TMP_Text serverFailedText;
    public Button retry;
    public Button exit;
    public EventSystem eventSystem;
    public Sprite volume_on;
    public Sprite volume_off;
    public Image volume;
    public Button volumeControl;

    bool downloadMap;
    bool downloadScene;
    bool downloadAsset;
    long contentData;

    private void Awake()
    {
        downloadBtn.GetComponentInChildren<TMP_Text>().text = App.gameSettings.language == Language.KOR ? "다운로드" : "Download";
        downloadBtn.onClick.AddListener(() =>
        {
            AssetDownload(App.GlobalToken).Forget();
            downloadBorder.SetActive(false);
        });

        serverFailedText.text = App.gameSettings.language == Language.KOR ? "서버 연결에 실패했습니다." : "Failed to connect to the server.";
        retry.GetComponentInChildren<TMP_Text>().text = App.gameSettings.language == Language.KOR ? "재시도" : "Retry";
        exit.GetComponentInChildren<TMP_Text>().text = App.gameSettings.language == Language.KOR ? "종료" : "Exit";
        
        retry.onClick.AddListener(() =>
        {
            CheckDownloadGameResources(App.GlobalToken).Forget();

            serverFailed.SetActive(false);
        });

        exit.onClick.AddListener(() => 
        {
            App.GameExit().Forget();
        });


    }
    private void Start()
    {
        CheckDownloadGameResources(App.GlobalToken).Forget();

        if(App.gameSettings.soundBackgrounds)
        {
            volume.sprite = volume_on;
        }
        else
        {
            volume.sprite = volume_off;
        }
        volumeControl.onClick.AddListener(() =>
        {
            if (App.gameSettings.soundBackgrounds)
            {
                volume.sprite = volume_off;
                App.gameSettings.soundBackgrounds = false;
                GameInstance.GameIns.bgMSoundManager.Audios.volume = 0;
                SaveLoadSystem.SaveGameSettings(App.gameSettings);
            }
            else
            {
                App.gameSettings.soundBackgrounds = true;
                volume.sprite = volume_on;
                GameInstance.GameIns.bgMSoundManager.Audios.volume = 1;
                SaveLoadSystem.SaveGameSettings(App.gameSettings);
            }
        });
    }



    private void OnDestroy()
    {
        downloadBtn.onClick.RemoveAllListeners();
        retry.onClick.RemoveAllListeners();
        continueBtn.onClick.RemoveAllListeners();
        exit.onClick.RemoveAllListeners();
    }

    private async UniTask CheckDownloadGameResources(CancellationToken cancellationToken = default)
    {
        await UniTask.NextFrame(cancellationToken: cancellationToken);

        loadingImage.gameObject.SetActive(true);

        await AssetLoader.GetServerUrl("", cancellationToken);
        string testConnectionUrl = Path.Combine(AssetLoader.serverUrl, "testconnection.txt");
        UnityWebRequest www = UnityWebRequest.Get(testConnectionUrl);
        www.timeout = 5;
        var operation = www.SendWebRequest();
        while (!operation.isDone)
        {

            await UniTask.NextFrame(cancellationToken: cancellationToken);
        }
        loadingImage.gameObject.SetActive(false);

        if (www.result == UnityWebRequest.Result.Success)
        {
            bool a = await App.IsAlreadyCached("", "map", 1);
            bool b = await App.IsAlreadyCached("town_01", "town_01", 3);
            bool c = await App.IsAlreadyCached("town_01", "town_01_scene", 2);
            if (a && b && c)
            {
                continueBtn.gameObject.SetActive(true);
            }
            else
            {
                await GetDownloadSize(cancellationToken);
            }
        }
        else
        {
            serverFailed.SetActive(true);
        }    
    }

    async UniTask GetDownloadSize(CancellationToken cancellationToken = default)
    {
        await AssetLoader.GetServerUrl("", cancellationToken);
        string url = Path.Combine(AssetLoader.serverUrl, "map");
        UnityWebRequest headRequest = UnityWebRequest.Head(url);
        await headRequest.SendWebRequest();

        await UniTask.NextFrame(cancellationToken: cancellationToken);

        long contentMap = 0;
        if (!Caching.IsVersionCached(url, App.currentHashes[1].hash))
        {
            if (headRequest.result == UnityWebRequest.Result.Success)
            {
                string contentLengthStr = headRequest.GetResponseHeader("Content-Length");
                long.TryParse(contentLengthStr, out contentMap);

            }
            downloadMap = true;
        }
        await AssetLoader.GetServerUrl("town_01", cancellationToken);
        string url2 = Path.Combine(AssetLoader.serverUrl, "town_01");
        UnityWebRequest headRequest2 = UnityWebRequest.Head(url2);
        await headRequest2.SendWebRequest();

        await UniTask.NextFrame(cancellationToken: cancellationToken);
        long contentAsset = 0;
        if (!Caching.IsVersionCached(url2, App.currentHashes[3].hash))
        {
            if (headRequest2.result == UnityWebRequest.Result.Success)
            {
                string contentLengthStr = headRequest2.GetResponseHeader("Content-Length");
                long.TryParse(contentLengthStr, out contentAsset);
            }
            downloadAsset = true;
        }
        string url3 = Path.Combine(AssetLoader.serverUrl, "town_01_scene");
        UnityWebRequest headRequest3 = UnityWebRequest.Head(url3);
        await headRequest3.SendWebRequest();

        await UniTask.NextFrame(cancellationToken: cancellationToken);
        long contentScene = 0;
        if (!Caching.IsVersionCached(url3, App.currentHashes[2].hash))
        {
            if (headRequest3.result == UnityWebRequest.Result.Success)
            {
                string contentLengthStr = headRequest3.GetResponseHeader("Content-Length");
                long.TryParse(contentLengthStr, out contentScene);
            }
            downloadScene = true;
        }

        contentData = contentMap + contentScene + contentAsset;

        if (contentData == 0)   //이미 같은 파일을 같고 있음
        {
            await AssetLoader.GetServerUrl("", cancellationToken);
            string target = Path.Combine(AssetLoader.serverUrl, "map");
            if (!App.currentHashes.ContainsKey(1)) await DownloadHash(1, target, App.GlobalToken);
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(target, App.currentHashes[1].hash, 0);
            await www.SendWebRequest();
            App.bundleCheck[1] = new BundleCheck(1, (long)www.downloadedBytes, App.currentHashes[1].hash, App.currentHashes[1].timeStamp);
            SaveLoadSystem.SaveDownloadedData();

            await AssetLoader.GetServerUrl("town_01", cancellationToken);

            //씬 데이터
            string urlScene = Path.Combine(AssetLoader.serverUrl, "town_01_scene");
            if (!App.currentHashes.ContainsKey(2)) await DownloadHash(2, urlScene, App.GlobalToken);

            UnityWebRequest www2 = UnityWebRequestAssetBundle.GetAssetBundle(urlScene, App.currentHashes[2].hash, 0);
            await www2.SendWebRequest();
  
            App.bundleCheck[2] = new BundleCheck(2, (long)www2.downloadedBytes, App.currentHashes[2].hash, App.currentHashes[2].timeStamp);
            SaveLoadSystem.SaveDownloadedData();

            //에셋 데이터
            string urlAsset = Path.Combine(AssetLoader.serverUrl, "town_01");
            if (!App.currentHashes.ContainsKey(3)) await DownloadHash(3, urlAsset, App.GlobalToken);

            UnityWebRequest www3 = UnityWebRequestAssetBundle.GetAssetBundle(urlAsset, App.currentHashes[3].hash, 0);
            await www3.SendWebRequest();

            App.bundleCheck[3] = new BundleCheck(3, (long)www3.downloadedBytes, App.currentHashes[3].hash, App.currentHashes[3].timeStamp);
            SaveLoadSystem.SaveDownloadedData();
            continueBtn.gameObject.SetActive(true);
            return;
        }

        downloadBorder.SetActive(true);
        if (contentData >= (1024f * 1024f * 1024f))
        {

            float sizeInGB = contentData / (1024f * 1024f * 1024f);
            string formatted = sizeInGB.ToString("0.##");
            downloadAmount.text = (App.gameSettings.language == Language.KOR ? "다운로드 크기\n" : "Download size\n") + formatted + "GB";
        }
        if (contentData >= (1024f * 1024f))
        {

            float sizeInMB = contentData / (1024f * 1024f);
            string formatted = sizeInMB.ToString("0.##");
            downloadAmount.text = (App.gameSettings.language == Language.KOR ? "다운로드 크기\n" : "Download size\n") + formatted + "MB";
        }
        else if (contentData >= 1024f)
        {
            float sizeInKB = contentData / 1024f;
            string formatted = sizeInKB.ToString("0.##");
            downloadAmount.text = (App.gameSettings.language == Language.KOR ? "다운로드 크기\n" : "Download size\n") + formatted + "KB";

        }
        else
        {
            downloadAmount.text = (App.gameSettings.language == Language.KOR ? "다운로드 크기\n" : "Download size\n") + contentData.ToString() + "B";

        }

      

        if (App.bundleCheck.ContainsKey(1) && App.bundleCheck.ContainsKey(2) && App.bundleCheck.ContainsKey(3))
        {
            updateText.gameObject.SetActive(true);
            updateText.text = App.gameSettings.language == Language.KOR ? "새로운 업데이트 발견 !!!" : "NEW UPDATE DETECTED!!!";
        }
    }

    async UniTask AssetDownload(CancellationToken cancellationToken)
    {
        try
        {
            downloadingStatus.SetActive(true);
            downloadState.text = App.gameSettings.language == Language.KOR ? "다운로드 중" : "Downloading";

            downloadText.text = FormatBytes(0) + " / " + FormatBytes(contentData);
            downloadPercentage.text = "0%";
            downloadGage.fillAmount = 0;

            //레벨 데이터
            await AssetLoader.GetServerUrl("", cancellationToken);
            string target = Path.Combine(AssetLoader.serverUrl, "map");
            Hash128 bundleHash = new Hash128();
            if (!App.currentHashes.ContainsKey(1)) await DownloadHash(1, target, App.GlobalToken);

            bundleHash = App.currentHashes[1].hash;
            //     Hash128 bundleHash = SaveLoadSystem.ComputeHash128(System.Text.Encoding.UTF8.GetBytes(target));

            ulong levelDownloadedBytes = 0;

            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(target, bundleHash, 0);
            var operation = www.SendWebRequest();
            while (!www.isDone)
            {
                if (downloadMap)
                {
                    float progress = www.downloadProgress;
                    levelDownloadedBytes = www.downloadedBytes;

                    string contentLengthStr = www.GetResponseHeader("Content-Length");

                    string downloadedStr = FormatBytes((long)levelDownloadedBytes);

                    float percent = contentData > 0 ? (100f * levelDownloadedBytes / contentData) : (progress * 100f);
                    string percentStr = $"{percent:F1}%";
                    downloadText.text = downloadedStr + " / " + FormatBytes(contentData);
                    downloadPercentage.text = percentStr;
                    downloadGage.fillAmount = percent / 100f;
                }
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
            App.bundleCheck[1] = new BundleCheck(1, (long)www.downloadedBytes, bundleHash, App.currentHashes[1].timeStamp);
            BundleCheck newCachingBundle = new BundleCheck(bundleHash, App.currentHashes[1].timeStamp, "map");
            if (!App.cachedData.ContainsKey(1)) App.cachedData[1] = new List<BundleCheck>();
            else if (App.cachedData[1] == null) App.cachedData[1] = new List<BundleCheck>();

            for (int i = App.cachedData[1].Count - 1; i >= 0; i--) 
            {
                if (App.cachedData[1][i].hash != newCachingBundle.hash)
                {
                    Caching.ClearCachedVersion(App.cachedData[1][i].bundleName, App.cachedData[1][i].hash);
                }
                App.cachedData[1].RemoveAt(i);
                    
            }
       
            App.cachedData[1].Add(newCachingBundle);

            SaveLoadSystem.SaveDownloadedData();
            SaveLoadSystem.SaveCachedDownloadedData();
            await AssetLoader.GetServerUrl("town_01", cancellationToken);

            //씬 데이터
            string urlScene = Path.Combine(AssetLoader.serverUrl, "town_01_scene");
            Hash128 bundleHash2 = new Hash128();
            if (!App.currentHashes.ContainsKey(2)) await DownloadHash(2, urlScene, App.GlobalToken);

            bundleHash2 = App.currentHashes[2].hash;
            // Hash128 bundleHash2 = SaveLoadSystem.ComputeHash128(System.Text.Encoding.UTF8.GetBytes(urlScene));
            UnityWebRequest www2 = UnityWebRequestAssetBundle.GetAssetBundle(urlScene, bundleHash2, 0);
            var operation2 = www2.SendWebRequest();
            ulong sceneDownloadedBytes = 0;
            while (!www2.isDone)
            {
                if (downloadScene)
                {
                    float progress = www2.downloadProgress;
                    sceneDownloadedBytes = www2.downloadedBytes;

                    string contentLengthStr = www2.GetResponseHeader("Content-Length");


                    string downloadedStr = FormatBytes((long)sceneDownloadedBytes + (long)levelDownloadedBytes);
                    float percent = contentData > 0 ? (100f * (levelDownloadedBytes + sceneDownloadedBytes) / contentData) : (progress * 100f);
                    string percentStr = $"{percent:F1}%";
                    downloadText.text = downloadedStr + " / " + FormatBytes(contentData);
                    downloadPercentage.text = percentStr;
                    downloadGage.fillAmount = percent / 100f;
                }
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
            App.bundleCheck[2] = new BundleCheck(2, (long)www2.downloadedBytes, bundleHash2, App.currentHashes[2].timeStamp);
            BundleCheck newCachingBundle2 = new BundleCheck(bundleHash2, App.currentHashes[2].timeStamp, "town_01_scene");
            if (!App.cachedData.ContainsKey(2)) App.cachedData[2] = new List<BundleCheck>();
            else if(App.cachedData[2] == null) App.cachedData[2] = new List<BundleCheck>();

            for (int i = App.cachedData[2].Count - 1; i >= 0; i--)
            {
                if (App.cachedData[2][i].hash != newCachingBundle2.hash)
                {
                    Caching.ClearCachedVersion(App.cachedData[2][i].bundleName, App.cachedData[2][i].hash);
                }
                App.cachedData[2].RemoveAt(i);

            }
            App.cachedData[2].Add(newCachingBundle2);
            SaveLoadSystem.SaveDownloadedData();
            SaveLoadSystem.SaveCachedDownloadedData();


            //에셋 데이터
            string urlAsset = Path.Combine(AssetLoader.serverUrl, "town_01");
            Hash128 bundleHash3 = new Hash128();
            if (!App.currentHashes.ContainsKey(3)) await DownloadHash(3, urlAsset, App.GlobalToken);

            bundleHash3 = App.currentHashes[3].hash;
            // Hash128 bundleHash3 = SaveLoadSystem.ComputeHash128(System.Text.Encoding.UTF8.GetBytes(urlAsset));
            UnityWebRequest www3 = UnityWebRequestAssetBundle.GetAssetBundle(urlAsset, bundleHash3, 0);
            var operation3 = www3.SendWebRequest();
            ulong assetDownloadedBytes = 0;
            while (!www3.isDone)
            {
                if (downloadAsset)
                {
                    float progress = www3.downloadProgress;
                    assetDownloadedBytes = www3.downloadedBytes;

                    string contentLengthStr = www3.GetResponseHeader("Content-Length");

                    string downloadedStr = FormatBytes((long)sceneDownloadedBytes + (long)levelDownloadedBytes + (long)assetDownloadedBytes);
                    float percent = contentData > 0 ? (100f * (levelDownloadedBytes + sceneDownloadedBytes + assetDownloadedBytes) / contentData) : (progress * 100f);
                    string percentStr = $"{percent:F1}%";
                    downloadText.text = downloadedStr + " / " + FormatBytes(contentData);
                    downloadPercentage.text = percentStr;
                    downloadGage.fillAmount = percent / 100f;
                }
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
            App.bundleCheck[3] = new BundleCheck(3, (long)www3.downloadedBytes, bundleHash3, App.currentHashes[3].timeStamp);
            BundleCheck newCachingBundle3 = new BundleCheck(bundleHash3, App.currentHashes[3].timeStamp, "town_01");
            if (!App.cachedData.ContainsKey(3)) App.cachedData[3] = new List<BundleCheck>();
            else if (App.cachedData[3] == null) App.cachedData[3] = new List<BundleCheck>();

            for (int i = App.cachedData[3].Count - 1; i >= 0; i--)
            {
                if (App.cachedData[3][i].hash != newCachingBundle3.hash)
                {
                    Caching.ClearCachedVersion(App.cachedData[3][i].bundleName, App.cachedData[3][i].hash);
                }
                App.cachedData[3].RemoveAt(i);

            }
           
            App.cachedData[3].Add(newCachingBundle3);
            SaveLoadSystem.SaveDownloadedData();
            SaveLoadSystem.SaveCachedDownloadedData();

            if (www.result != UnityWebRequest.Result.Success || www2.result != UnityWebRequest.Result.Success || www3.result != UnityWebRequest.Result.Success)
            {
                // Debug.LogError($"다운로드 실패: {www.error}");
                downloadState.text = App.gameSettings.language == Language.KOR ? "다운로드 실패" : "Download failed";
                return;
            }
            downloadText.text = FormatBytes(contentData) + " / " + FormatBytes(contentData);
            downloadPercentage.text = "100%";
            downloadGage.fillAmount = 1;
            downloadState.text = App.gameSettings.language == Language.KOR ? "다운로드 완료" : "Download complete";

            continueBtn.gameObject.SetActive(true);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    string FormatBytes(long bytes)
    {
        if (bytes >= 1024 * 1024)
            return $"{(bytes / (1024f * 1024f)):0.##} MB";
        else if (bytes >= 1024)
            return $"{(bytes / 1024f):0.##} KB";
        else
            return $"{bytes} B";
    }

    public void Continue()
    {
        Load(App.GlobalToken).Forget();
        continueBtn.gameObject.SetActive(false);
       // StartCoroutine(UnloadNextFrame());
    }

    async UniTask Load(CancellationToken cancellationToken = default)
    {
        eventSystem.gameObject.SetActive(false);
        await App.GameLoad(cancellationToken);
        StartCoroutine(UnloadNextFrame());
    }

    IEnumerator UnloadNextFrame()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.UnloadSceneAsync("DownloadScene");
    }

    async UniTask DownloadHash(int id, string url, CancellationToken cancellationToken = default)
    {
        UnityWebRequest www = UnityWebRequest.Get(url + ".mani");
        await www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string manifestText = www.downloadHandler.text;
            Manifest manifest = JsonConvert.DeserializeObject<Manifest>(manifestText);
            App.currentHashes[id] = new BundleCheck(Hash128.Parse(manifest.hash), manifest.timeStamp);
        }
    }
}
