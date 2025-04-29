using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

[InitializeOnLoad]
public class OpenGameDataDir
{
    [MenuItem("Tools/OpenDataDir %`")]
    public static void OpenDataDir()
    {
        Open();
    }


    [MenuItem("Tools/OpenDataDir %`", validate = true)]
    static bool CheckPlaying()
    {
      
        return !EditorApplication.isPlaying && Directory.Exists(Application.persistentDataPath);

    }
    [MenuItem("Tools/RemoveDataDir &`")]
    public static void RemoveSaveDataDir()
    {
        string path = Path.Combine(Application.persistentDataPath, "Save");
        DeleteFolder(path);
    }
    [MenuItem("Tools/RemoveDataDir &`", validate = true)]
    static bool CheckPlaying2()
    {

        return !EditorApplication.isPlaying && Directory.Exists(Application.persistentDataPath + "/Save");

    }

    static void Open()
    {
        string path = Application.persistentDataPath;
           Application.OpenURL(path);

    }
    static void DeleteFolder(string folderPath)
    {
        // 폴더 내 모든 파일 삭제
        foreach (string file in Directory.GetFiles(folderPath))
        {
            File.Delete(file);
        }

        foreach (string subDirectory in Directory.GetDirectories(folderPath))
        {
            DeleteFolder(subDirectory);  // 재귀적으로 폴더 내부를 삭제
        }

        Directory.Delete(folderPath, true); //파일 삭제

        UnityEngine.Debug.Log("Save data folder deleted.");
    }
}
