using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class AssetBundleBuildEditor : EditorWindow
{
    [MenuItem("Tools/CreateAssetBudle")]
    public static void AssetBundle()
    {
        try
        {
            string sourceDir = @"E:/Git/PenguinRestaurant/develop/Bundle";
            string targetDir = @"E:/nodejs/projectz/web/uploads/restaurant";
            string directory = "./Bundle";

            string pcDir = Path.Combine(directory, "PC");
            string pcDir2 = Path.Combine(directory, "PC", "Manifest");
            if (!Directory.Exists(pcDir)) Directory.CreateDirectory(pcDir);
            // BuildPipeline.BuildAssetBundles(pcDir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);


            BuildPipeline.BuildAssetBundles(pcDir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

            string pc = Path.Combine(sourceDir, "PC");
            string[] bundles = Directory.GetFiles(pc);
            Debug.Log(bundles.Length);
            foreach (var bundlePath in bundles)
            {

                string destPath = "";
                string fileName = Path.GetFileName(bundlePath);
                Debug.Log(fileName);

                if (fileName == "map")
                {
                    Debug.Log(fileName);
                    destPath = Path.Combine(targetDir, "pc", fileName);
                    Debug.Log(destPath);
                    if (File.Exists(destPath))
                    {
                        File.Delete(destPath);  // 기존 파일 삭제 (덮어쓰기 대비)
                    }
                    File.Move(bundlePath, destPath);

                }
                else if ((fileName == "town_01_scene" && fileName != "town_01_scene.manifest") || (fileName == "town_01" && fileName != "town_01.manifest"))
                {
                    Debug.Log(fileName);
                    destPath = Path.Combine(targetDir, "town_01", "pc", fileName);

                    if (File.Exists(destPath))
                    {
                        File.Delete(destPath);  // 기존 파일 삭제 (덮어쓰기 대비)
                    }
                    File.Move(bundlePath, destPath);
                }
            }


            foreach (var bundlePath in bundles)
            {
                string destPath = "";
                string fileName = Path.GetFileName(bundlePath);

                if (fileName == "map.manifest" || fileName == "town_01_scene.manifest" || fileName == "town_01.manifest")
                {
                    Debug.Log(fileName + "N");
                    string manifestPath = Path.Combine(pc, fileName);
                    string manifestText = File.ReadAllText(manifestPath);
                    string[] lines = manifestText.Split('\n');
                    string hashStr = null;
                    bool inAssetFileHash = false;

                    foreach (string rawLine in lines)
                    {
                        string line = rawLine.Trim();

                        if (line.StartsWith("AssetFileHash:"))
                        {
                            inAssetFileHash = true;
                        }
                        else if (inAssetFileHash && line.StartsWith("Hash:"))
                        {
                            hashStr = line.Split(':')[1].Trim();
                            break; // 찾았으니 반복 종료
                        }
                        else if (line.StartsWith("TypeTreeHash:") || line.StartsWith("IncrementalBuildHash:"))
                        {
                            // 다른 블럭 진입 → AssetFileHash 블럭 종료
                            inAssetFileHash = false;
                        }
                    }

                    Hash128 h = Hash128.Parse(hashStr);

                    string maniPath = Path.Combine(pcDir2, fileName.Substring(0, fileName.Length - 4));

                    string dir = Path.GetDirectoryName(maniPath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    Manifest manifest = new Manifest(h.ToString(), DateTime.UtcNow.ToString("yyyyMMddHHmmss"));

                    string writeText= JsonConvert.SerializeObject(manifest);

                    File.WriteAllText(maniPath, writeText);

                    if (fileName == "map.manifest") destPath = Path.Combine(targetDir, "pc", fileName.Substring(0, fileName.Length - 4));
                    else destPath = Path.Combine(targetDir, "town_01", "pc", fileName.Substring(0, fileName.Length - 4));

                    if (File.Exists(destPath))
                    {
                        File.Delete(destPath);  // 기존 파일 삭제 (덮어쓰기 대비)
                        Debug.Log("Remove" + destPath);
                    }

                    File.Move(maniPath, destPath);
                    Debug.Log("ManiPath " + maniPath + " dest PAthj " + destPath);
                }
                else
                {
                    continue;
                }
            }

            string androidDir = Path.Combine(directory, "Android");
            if (!Directory.Exists(androidDir)) Directory.CreateDirectory(androidDir);
            BuildPipeline.BuildAssetBundles(androidDir, BuildAssetBundleOptions.None, BuildTarget.Android);


            string android = Path.Combine(sourceDir, "Android");
            string[] bundles2 = Directory.GetFiles(android);
            foreach (var bundlePath in bundles2)
            {
                string fileName = Path.GetFileName(bundlePath);
                if (fileName == "map")
                {
                    string destPath = Path.Combine(targetDir, "android", fileName);

                    if (File.Exists(destPath))
                    {
                        File.Delete(destPath);  // 기존 파일 삭제 (덮어쓰기 대비)
                    }
                    File.Move(bundlePath, destPath);
                }
                else if (fileName == "town_01_scene" || fileName == "town_01")
                {

                    string destPath = Path.Combine(targetDir, "town_01", "android", fileName);

                    if (File.Exists(destPath))
                    {
                        File.Delete(destPath);  // 기존 파일 삭제 (덮어쓰기 대비)
                    }
                    File.Move(bundlePath, destPath);
                }
            }


            EditorUtility.DisplayDialog("Asset Bundle Build", "Build Complete", "Succeeded");
        }
        catch (OperationCanceledException e)
        {
            Debug.Log(e);
        }
    }
}
