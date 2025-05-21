using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글

public static class EditedDebug
{
    [System.Diagnostics.Conditional("Log")]
    public static void Log(object message)
    {
        UnityEngine.Debug.Log(message);
    }
    [System.Diagnostics.Conditional("Log")]
    public static void LogError(object message)
    {
        UnityEngine.Debug.LogError(message);
    }
    [System.Diagnostics.Conditional("Log")]
    public static void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message);
    }
}
