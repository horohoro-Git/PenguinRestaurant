using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuildEditor : EditorWindow
{
    [MenuItem("Tools/CreateAssetBudle")]
    public static void AssetBundle()
    {
        string directory = "./Bundle";

        string pcDir = Path.Combine(directory, "PC");
        if (!Directory.Exists(pcDir)) Directory.CreateDirectory(pcDir);
        BuildPipeline.BuildAssetBundles(pcDir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);


        string androidDir = Path.Combine(directory, "Android");
        if (!Directory.Exists(androidDir)) Directory.CreateDirectory(androidDir);
        BuildPipeline.BuildAssetBundles(androidDir, BuildAssetBundleOptions.None, BuildTarget.Android);

        EditorUtility.DisplayDialog("Asset Bundle Build", "Build Complete", "Succeeded");
    }
}
