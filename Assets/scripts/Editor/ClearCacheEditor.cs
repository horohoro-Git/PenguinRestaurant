using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClearCacheEditor : EditorWindow
{
    [MenuItem("Tools/ClearCache")]
    public static void ClearCache()
    {
        Caching.ClearCache();
        Debug.Log("Cache Clear");
    }
}
