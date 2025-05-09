using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssignBonesEditor : EditorWindow
{
    SkinnedMeshRenderer skinnedMeshRenderer;

    [MenuItem("Tools/AssignBones")]
    public static void AssignBones()
    {
        GetWindow<AssignBonesEditor>();
    }

    private void OnGUI()
    {
        skinnedMeshRenderer = (SkinnedMeshRenderer)EditorGUILayout.ObjectField("SkinnedMeshRenderer", skinnedMeshRenderer, typeof(SkinnedMeshRenderer), true);
        if(GUILayout.Button("Start") && skinnedMeshRenderer != null)
        {
            Transform root = skinnedMeshRenderer.rootBone;

            if (root == null)
            {
                EditorUtility.DisplayDialog("Error", "No Root Bone", "OK");
                return;
            }
            List<Transform> foundBones = new List<Transform>();
            CollectBonesRecursive(root, foundBones);
            for (int i = 0; i < foundBones.Count; i++)
            {
                Debug.Log(foundBones[i].name);
            }

            List<Transform> edits = new List<Transform>();
            edits.Add(foundBones[0]);
            edits.Add(foundBones[1]);
            foundBones.RemoveAt(1);
            foundBones.RemoveAt(0);

            for(int i = foundBones.Count - 1;i >= 0;i--)
            {
                edits.Add(foundBones[i]);
            }

            if (edits.Count >= skinnedMeshRenderer.sharedMesh.bindposes.Length)
            {
                skinnedMeshRenderer.bones = edits.Take(skinnedMeshRenderer.sharedMesh.bindposes.Length).ToArray();

            }
            else
            {
                Debug.LogWarning("failed");
            }
        }
    }
    void CollectBonesRecursive(Transform current, List<Transform> result)
    {
        result.Add(current);
        foreach (Transform child in current)
        {
            CollectBonesRecursive(child, result);
        }
    }
    Transform FindChildByNameRecursive(Transform parent, string name)
    {
        if (parent.name == name)
            return parent;

        foreach (Transform child in parent)
        {
            Transform result = FindChildByNameRecursive(child, name);
            if (result != null)
                return result;
        }

        return null;
    }
}
