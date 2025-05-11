using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ColliderRemoverEditor : EditorWindow
{
    private GameObject rootObject;
    private bool isProcessing = false;

    [MenuItem("Tools/Collider Remover")]
    public static void ShowWindow()
    {
        GetWindow<ColliderRemoverEditor>("Collider Remover");
    }

    private void OnGUI()
    {
        GUILayout.Label("Remove All Colliders from Object and Children", EditorStyles.boldLabel);

        rootObject = (GameObject)EditorGUILayout.ObjectField("Target GameObject", rootObject, typeof(GameObject), true);

        GUI.enabled = rootObject != null && !isProcessing;

        if (GUILayout.Button("Start Collider Removal"))
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(RemoveCollidersCoroutine(rootObject));
        }

        GUI.enabled = true;
    }

    private IEnumerator RemoveCollidersCoroutine(GameObject target)
    {
        isProcessing = true;

        List<Transform> allChildren = new List<Transform>(target.GetComponentsInChildren<Transform>(true));
        int totalCount = allChildren.Count;
        int processed = 0;

        foreach (Transform child in allChildren)
        {
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {

                meshRenderer.receiveShadows = false;
                meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            }
            Collider[] colliders = child.GetComponents<Collider>();
            foreach (Collider col in colliders)
            {
                Undo.DestroyObjectImmediate(col); // for undo support
            }

            processed++;

            if (processed % 50 == 0) // yield every 50 objects to avoid freeze
            {
                Debug.Log($"Processed {processed}/{totalCount} objects...");
                yield return null;
            }
        }

        Debug.Log($"Collider removal complete. Processed {processed} objects.");
        isProcessing = false;
    }
}
