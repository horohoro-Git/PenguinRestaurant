using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AndroidToast : MonoBehaviour
{
    private static AndroidJavaClass toastPlugin;
    public static void Show(string message, int duration = 0)  // duration: 0 = LENGTH_SHORT, 1 = LENGTH_LONG
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass pluginClass;
            pluginClass = new AndroidJavaClass("com.dreamone.toasthelper.ToastHelper");
            pluginClass.CallStatic("init", activity);
            pluginClass.CallStatic("showToast", message, duration);
        }
    }

    public static void Close()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass pluginClass;
            pluginClass = new AndroidJavaClass("com.dreamone.toasthelper.ToastHelper");
           
            pluginClass.CallStatic("cancelToast");
        }
    }
}

