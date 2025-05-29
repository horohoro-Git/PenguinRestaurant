using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            if (!Directory.Exists(pcDir)) Directory.CreateDirectory(pcDir);
            BuildPipeline.BuildAssetBundles(pcDir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

            


          /*  string pc = Path.Combine(sourceDir, "PC");
            string[] bundles = Directory.GetFiles(pc);
            Debug.Log(bundles.Length);
            foreach (var bundlePath in bundles)
            {
        
              
                string fileName = Path.GetFileName(bundlePath);
                if (fileName == "map")
                {
                    string destPath = Path.Combine(targetDir, "pc", fileName);
                    Debug.Log(destPath);
                    if (File.Exists(destPath))
                    {
                        File.Delete(destPath);  // 기존 파일 삭제 (덮어쓰기 대비)
                    }
                    File.Move(bundlePath, destPath);
                }
                else if (fileName == "town_01_scene" || fileName == "town_01")
                {

                    string destPath = Path.Combine(targetDir, "town_01", "pc", fileName);

                    if (File.Exists(destPath))
                    {
                        File.Delete(destPath);  // 기존 파일 삭제 (덮어쓰기 대비)
                    }
                    File.Move(bundlePath, destPath);
                }

            }*/

            string androidDir = Path.Combine(directory, "Android");
            if (!Directory.Exists(androidDir)) Directory.CreateDirectory(androidDir);
            BuildPipeline.BuildAssetBundles(androidDir, BuildAssetBundleOptions.None, BuildTarget.Android);


          /*  string android = Path.Combine(sourceDir, "Android");
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

*/
            EditorUtility.DisplayDialog("Asset Bundle Build", "Build Complete", "Succeeded");
        }
        catch
        {
            Debug.Log("Fail");
        }
    }


}
