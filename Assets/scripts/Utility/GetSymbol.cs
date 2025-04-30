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
    const string SYMBOL2 = "HAS_DOTWEEN";
    static GetSymbol()
    {
        EditorApplication.delayCall += UpdateSymbol;
    }
    static void UpdateSymbol()
    {
        bool hasAsset = Type.GetType("AnimationInstancing.AnimationInstancing") != null;
        bool hasAsset2 = Type.GetType("AnimationInstancing.AnimationInstancing") != null;
        SetDefineSymbol(hasAsset, hasAsset2);
    }
    static void SetDefineSymbol(bool enable, bool enable2)
    {
        var buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget);
        var symbols = new HashSet<string>(defines.Split(';'));

        bool changed = false;
        if (enable)
        {
            symbols.Add(SYMBOL);
            changed = true;
        }
        else
        {
            symbols.Remove(SYMBOL);
            changed = true;
        }
        if (enable2)
        {
            symbols.Add(SYMBOL2);
            changed = true;
        }
        else
        {
            symbols.Remove(SYMBOL2);
            changed = true;
        }
        if (changed)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
            buildTarget,
            string.Join(";", symbols));


            Debug.Log($"[SYMBOL] {SYMBOL} = {enable}");
            Debug.Log($"[SYMBOL2] {SYMBOL2} = {enable2}");
            AssetDatabase.Refresh();
        }
    }
}
#endif