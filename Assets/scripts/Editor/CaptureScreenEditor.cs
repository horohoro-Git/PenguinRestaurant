using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CaptureScreenEditor : EditorWindow
{
    [MenuItem("Tools/CaptureScreen")]
    public static void ShowWindow()
    {
        GetWindow<CaptureScreenEditor>().Show();
    }

    private void OnGUI()
    {
        if(GUILayout.Button("ScreenShot"))
        {
            string url = Path.Combine(Application.persistentDataPath, "screenshot.png"); // 찍는 위치

            ScreenCapture.CaptureScreenshot(url); //사진을 찍는 기능
            Debug.Log("Screenshot saved to: " + url);
        }
    }
}
