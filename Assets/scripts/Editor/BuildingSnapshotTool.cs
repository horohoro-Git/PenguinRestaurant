using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildingSnapshotTool : EditorWindow
{
    Camera captureCamera;
    RenderTexture renderTex;
    int resolution = 512;
    GameObject targetObject;

    [MenuItem("Tools/Building Snapshot Tool")]
    public static void ShowWindow()
    {
        GetWindow<BuildingSnapshotTool>("Building Snapshot Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Building Snapshot Capture", EditorStyles.boldLabel);
        targetObject = (GameObject)EditorGUILayout.ObjectField("BuildingObject", targetObject, typeof(GameObject), true);
        resolution = EditorGUILayout.IntField("Resolution", resolution);

        if (GUILayout.Button("Capture"))
        {
            if (targetObject == null)
            {
                Debug.LogWarning("Select the target object");
                return;
            }
            CaptureBuilding();
        }
    }

    void CaptureBuilding()
    {
        string folder = "Assets/Snapshots";
        Directory.CreateDirectory(folder);

        renderTex = new RenderTexture(resolution, resolution, 24);
        captureCamera = new GameObject("TempCaptureCamera").AddComponent<Camera>();
        captureCamera.backgroundColor = new Color(0, 0, 0, 0);
        captureCamera.clearFlags = CameraClearFlags.SolidColor;
        captureCamera.orthographic = true;
        captureCamera.orthographicSize = 50;
        captureCamera.targetTexture = renderTex;
        captureCamera.cullingMask = LayerMask.GetMask("BuildingCapture");

        Bounds bounds = CalculateBounds(targetObject);
        Vector3 center = bounds.center;
        float distance = 40f;

        // 카메라 회전
        Quaternion rotation = Quaternion.Euler(60f, 45f, 0f);
        Vector3 offset = rotation * (Vector3.back * distance);

        captureCamera.transform.position = center + offset;
        captureCamera.transform.rotation = rotation;

        captureCamera.Render();

        RenderTexture.active = renderTex;
        Texture2D tex = new Texture2D(renderTex.width, renderTex.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        tex.Apply();

        byte[] png = tex.EncodeToPNG();
        string path = Path.Combine(folder, targetObject.name + "_snapshot.png");
        File.WriteAllBytes(path, png);
        Debug.Log("Save: " + path);

        RenderTexture.active = null;
        DestroyImmediate(captureCamera.gameObject);
        renderTex.Release();
    }

    Bounds CalculateBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
            bounds.Encapsulate(rend.bounds);
        return bounds;
    }
}
