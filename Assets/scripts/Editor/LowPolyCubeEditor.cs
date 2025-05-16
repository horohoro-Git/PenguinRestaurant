using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LowPolyCubeEditor : EditorWindow
{
    [MenuItem("Create/CreateLowpolyCube")]
    public static void LowPolyCube()
    {
        GetWindow<LowPolyCubeEditor>().Show();
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Create"))
        {
            CreateLowpolyCube();
        }
    }
    public void CreateLowpolyCube()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = {
            new Vector3(-0.5f, -0.5f, -0.5f), // 0
            new Vector3( 0.5f, -0.5f, -0.5f), // 1
            new Vector3( 0.5f,  0.5f, -0.5f), // 2
            new Vector3(-0.5f,  0.5f, -0.5f), // 3
            new Vector3(-0.5f, -0.5f,  0.5f), // 4
            new Vector3( 0.5f, -0.5f,  0.5f), // 5
            new Vector3( 0.5f,  0.5f,  0.5f), // 6
            new Vector3(-0.5f,  0.5f,  0.5f), // 7
        };

        int[] triangles = {
            // back
            0, 2, 1, 0, 3, 2,
            // front
            4, 5, 6, 4, 6, 7,
            // left
            0, 7, 3, 0, 4, 7,
            // right
            1, 2, 6, 1, 6, 5,
            // top
            3, 7, 6, 3, 6, 2,
            // bottom
            0, 1, 5, 0, 5, 4
        };

        Vector2[] uv = new Vector2[8]; // 간단한 타일링용
        for (int i = 0; i < 8; i++) uv[i] = new Vector2(vertices[i].x + 0.5f, vertices[i].z + 0.5f);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        string path = "Assets/lowpolymesh.asset";
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        // mesh.GetComponent<MeshFilter>().mesh = mesh;
    }
}
