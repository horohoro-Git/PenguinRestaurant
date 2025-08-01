using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DownloadManager : MonoBehaviour
{
    public GameObject downloadBorder;
    public Button downloadBtn;

    public TMP_Text downloadAmount;

    bool downloading;
    long contentData;
    private void Awake()
    {
        downloadBtn.onClick.AddListener(() =>
        {
            AssetDownload(App.GlobalToken).Forget();
            downloadBorder.SetActive(false);
        });
    }
    private void Start()
    {
        GetDownloadSize(App.GlobalToken).Forget();
    }


    async UniTask GetDownloadSize(CancellationToken cancellationToken = default)
    {
        
        string url = Path.Combine(AssetLoader.serverUrl, "map");
        UnityWebRequest headRequest = UnityWebRequest.Head(url);
        await headRequest.SendWebRequest();

        await UniTask.NextFrame(cancellationToken: cancellationToken);

        long contentMap = 0;
        if (headRequest.result == UnityWebRequest.Result.Success)
        {
            string contentLengthStr = headRequest.GetResponseHeader("Content-Length");
            long.TryParse(contentLengthStr, out contentMap);
            
        }
        await AssetLoader.GetServerUrl("town_01");
        string url2 = Path.Combine(AssetLoader.serverUrl, "town_01");
        UnityWebRequest headRequest2 = UnityWebRequest.Head(url2);
        await headRequest2.SendWebRequest();

        await UniTask.NextFrame(cancellationToken: cancellationToken);
        long contentAsset = 0;
        if (headRequest2.result == UnityWebRequest.Result.Success)
        {
            string contentLengthStr = headRequest2.GetResponseHeader("Content-Length");
            long.TryParse(contentLengthStr, out contentAsset);
        }
        string url3 = Path.Combine(AssetLoader.serverUrl, "town_01_scene");
        UnityWebRequest headRequest3 = UnityWebRequest.Head(url3);
        await headRequest3.SendWebRequest();

        await UniTask.NextFrame(cancellationToken: cancellationToken);
        long contentScene = 0;
        if (headRequest3.result == UnityWebRequest.Result.Success)
        {
            string contentLengthStr = headRequest3.GetResponseHeader("Content-Length");
            long.TryParse(contentLengthStr, out contentScene);
        }
        contentData = contentMap + contentScene + contentAsset;
        downloadBorder.SetActive(true);
        if (contentData >= (1024f * 1024f * 1024f))
        {

            float sizeInGB = contentData / (1024f * 1024f * 1024f);
            string formatted = sizeInGB.ToString("0.##");
            downloadAmount.text = "다운로드 크기 : " + formatted + "GB";
        }
        if (contentData >= (1024f * 1024f))
        {

            float sizeInMB = contentData / (1024f * 1024f);
            string formatted = sizeInMB.ToString("0.##");
            downloadAmount.text = "다운로드 크기 : " + formatted + "MB";
        }
        else if (contentData >= 1024f)
        {
            float sizeInKB = contentData / 1024f;
            string formatted = sizeInKB.ToString("0.##");
            downloadAmount.text = "다운로드 크기 : " + formatted + "KB";

        }
        else
        {
            downloadAmount.text = "다운로드 크기 : " + contentData.ToString() + "B";

        }
    }

    async UniTask AssetDownload(CancellationToken cancellationToken)
    {


        //레벨 데이터
        await AssetLoader.GetServerUrl("");
        string target = Path.Combine(AssetLoader.serverUrl, "map");
        Hash128 bundleHash = SaveLoadSystem.ComputeHash128(System.Text.Encoding.UTF8.GetBytes(target));
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(target, bundleHash, 0);

        var operation = www.SendWebRequest();
        ulong levelDownloadedBytes = 0;
        while (!www.isDone)
        {
            float progress = www.downloadProgress;
            levelDownloadedBytes = www.downloadedBytes;

            string contentLengthStr = www.GetResponseHeader("Content-Length");
         
            string downloadedStr = FormatBytes((long)levelDownloadedBytes);
            string percentStr = contentData > 0 ? $"{(100f * levelDownloadedBytes / contentData):F1}%" : $"{(progress * 100f):F1}%";

            Debug.Log($"{downloadedStr} / {FormatBytes(contentData)} ({percentStr})");

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        /*if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"다운로드 실패: {www.error}");
            return;
        }
   */
        await AssetLoader.GetServerUrl("town_01");

        //씬 데이터
        string urlScene = Path.Combine(AssetLoader.serverUrl, "town_01_scene");
        Hash128 bundleHash2 = SaveLoadSystem.ComputeHash128(System.Text.Encoding.UTF8.GetBytes(urlScene));
        UnityWebRequest www2 = UnityWebRequestAssetBundle.GetAssetBundle(urlScene, bundleHash2, 0);
        var operation2 = www2.SendWebRequest();
        ulong sceneDownloadedBytes = 0;
        while (!www2.isDone)
        {
            float progress = www2.downloadProgress;
            sceneDownloadedBytes = www2.downloadedBytes;

            string contentLengthStr = www2.GetResponseHeader("Content-Length");
       

            string downloadedStr = FormatBytes((long)sceneDownloadedBytes + (long)levelDownloadedBytes);
            string percentStr = contentData > 0 ? $"{(100f * (levelDownloadedBytes + sceneDownloadedBytes) / contentData):F1}%" : $"{(progress * 100f):F1}%";

            Debug.Log($"{downloadedStr} / {FormatBytes(contentData)} ({percentStr})");

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
        //에셋 데이터
        string urlAsset = Path.Combine(AssetLoader.serverUrl, "town_01");
        Hash128 bundleHash3 = SaveLoadSystem.ComputeHash128(System.Text.Encoding.UTF8.GetBytes(urlAsset));
        UnityWebRequest www3 = UnityWebRequestAssetBundle.GetAssetBundle(urlAsset, bundleHash3, 0);
        var operation3 = www3.SendWebRequest();
        ulong assetDownloadedBytes = 0;
        while (!www3.isDone)
        {
            float progress = www3.downloadProgress;
            assetDownloadedBytes = www3.downloadedBytes;

            string contentLengthStr = www3.GetResponseHeader("Content-Length");
            
            string downloadedStr = FormatBytes((long)sceneDownloadedBytes + (long)levelDownloadedBytes + (long)assetDownloadedBytes);
            string percentStr = contentData > 0 ? $"{(100f * (levelDownloadedBytes + sceneDownloadedBytes + assetDownloadedBytes) / contentData):F1}%" : $"{(progress * 100f):F1}%";

            Debug.Log($"{downloadedStr} / {FormatBytes(contentData)} ({percentStr})");

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
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
}
