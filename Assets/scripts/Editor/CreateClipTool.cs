using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//한글

public class CreateClipTool : EditorWindow
{
    AnimationClip clip1;
    AnimationClip clip2;
    string mergedClipName = "NewMergedClip";


    [MenuItem("Tools/CreateMergeClip")]
    public static void ShowWindow()
    {
        GetWindow<CreateClipTool>("CreateMergeClip");

    }
    private void OnGUI()
    {
        GUILayout.Label("Merge Two Clips", EditorStyles.boldLabel);

        clip1 = (AnimationClip)EditorGUILayout.ObjectField("Clip 1", clip1, typeof(AnimationClip), false); 
        clip2 = (AnimationClip)EditorGUILayout.ObjectField("Clip 2", clip2, typeof(AnimationClip), false);
        mergedClipName = EditorGUILayout.TextField("Merged Name", mergedClipName);


        if (GUILayout.Button("Merge"))
        {
            if (clip1 != null && clip2 != null)
            {
                Merge();
            }
            else
            {
                Debug.Log("You must select two animation clips.");
            }
        }
    }
    void Merge()
    {
        AnimationClip mergedClip = new AnimationClip();

        // 첫 번째 클립의 키프레임 추가
        foreach (var binding in AnimationUtility.GetCurveBindings(clip1))
        {
            var curve1 = AnimationUtility.GetEditorCurve(clip1, binding);
            if (curve1 != null)
            {
                var newCurve = new AnimationCurve(curve1.keys);
                mergedClip.SetCurve(binding.path, binding.type, binding.propertyName, newCurve);

                Debug.Log($"첫 번째 클립 추가 - Path: {binding.path}, Property: {binding.propertyName}, Keys: {newCurve.keys.Length}");
            }
        }

        // 두 번째 클립의 키프레임 추가 (시간 오프셋 적용 및 병합)
        float offsetTime = clip1.length;
        foreach (var binding in AnimationUtility.GetCurveBindings(clip2))
        {
            var curve2 = AnimationUtility.GetEditorCurve(clip2, binding);
            if (curve2 != null)
            {
                var newCurve = new AnimationCurve();
                foreach (var key in curve2.keys)
                {
                    float adjustedTime = key.time + offsetTime;
                    newCurve.AddKey(new Keyframe(adjustedTime, key.value, key.inTangent, key.outTangent));
                }

                // 기존 곡선 확인 및 병합 처리
                var existingCurve = AnimationUtility.GetEditorCurve(mergedClip, binding);
                if (existingCurve != null)
                {
                    foreach (var key in existingCurve.keys)
                    {
                        newCurve.AddKey(key);
                    }
                }

                mergedClip.SetCurve(binding.path, binding.type, binding.propertyName, newCurve);

                Debug.Log($"두 번째 클립 추가 - Path: {binding.path}, Property: {binding.propertyName}, Keys: {newCurve.keys.Length}");
            }
        }



        string savePath = $"Assets/Assets/MergedAnimations/{mergedClipName}.anim";

        AssetDatabase.CreateAsset(mergedClip, savePath);
        AssetDatabase.SaveAssets();

        Debug.Log("Saved");
    }
        
}
