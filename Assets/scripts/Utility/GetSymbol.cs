#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[InitializeOnLoad]
public static class GetSymbol
{
    const string SYMBOL = "HAS_ANIMATION_INSTANCING";
    static GetSymbol()
    {
        EditorApplication.delayCall += UpdateSymbol;
    }
    static void UpdateSymbol()
    {
        bool hasAsset = Type.GetType("AnimationInstancing.AnimationInstancing") != null;
        SetDefineSymbol(hasAsset);
    }
    static void SetDefineSymbol(bool enable)
    {
        var buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget);
        var symbols = new HashSet<string>(defines.Split(';'));

        if (enable) symbols.Add(SYMBOL);
        else symbols.Remove(SYMBOL);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            buildTarget,
            string.Join(";", symbols));

        Debug.Log($"[SYMBOL] {SYMBOL} = {enable}");
    }
}
#endif