using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssignBonesEditor : EditorWindow
{

    bool reverse;
    List<Transform> t = new List<Transform>();
    SkinnedMeshRenderer skinnedMeshRenderer;

    [MenuItem("Tools/AssignBones")]
    public static void AssignBones()
    {
        GetWindow<AssignBonesEditor>();
    }

    private void OnGUI()
    {
        int removeIndex = -1;
        for (int i = 0; i < t.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            t[i] = (Transform)EditorGUILayout.ObjectField(t[i], typeof(Transform), true);
            if (GUILayout.Button("X", GUILayout.Width(20)))
                removeIndex = i;
            EditorGUILayout.EndHorizontal();
        }

        if (removeIndex >= 0)
            t.RemoveAt(removeIndex);
        if (GUILayout.Button("AddBone"))
        {
            t.Add(null);
        }

        skinnedMeshRenderer = (SkinnedMeshRenderer)EditorGUILayout.ObjectField("SkinnedMeshRenderer", skinnedMeshRenderer, typeof(SkinnedMeshRenderer), true);
        reverse = EditorGUILayout.Toggle("Reverse Bones", reverse);
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
           

            List<Transform> edits = new List<Transform>();

            foreach (Transform t in foundBones)
            {
                Debug.Log(t.name);
            }

            if (reverse)
            {
                edits.Add(foundBones[0]);
                edits.Add(foundBones[1]);
                foundBones.RemoveAt(1);
                foundBones.RemoveAt(0);

                for(int i = foundBones.Count - 1; i >= 0; i--)
                {
                    Debug.Log("A");
                    edits.Add(foundBones[i]);
                }
            }
            else
            {
                edits.Add(foundBones[0]);
                edits.Add(foundBones[1]);
                foundBones.RemoveAt(1);
                foundBones.RemoveAt(0);
             
                for (int i = 0; i < foundBones.Count; i++)
                {
                    edits.Add(foundBones[i]);
                }
            }

            if (edits.Count >= skinnedMeshRenderer.sharedMesh.bindposes.Length)
            {
                skinnedMeshRenderer.bones = t.Take(skinnedMeshRenderer.sharedMesh.bindposes.Length).ToArray();

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
