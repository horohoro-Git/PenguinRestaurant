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
            string url = Path.Combine(Application.persistentDataPath, "screenshot.png"); // ��� ��ġ

            ScreenCapture.CaptureScreenshot(url); //������ ��� ���
            Debug.Log("Screenshot saved to: " + url);
        }
    }
}
