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
        // ���� �� ��� ���� ����
        foreach (string file in Directory.GetFiles(folderPath))
        {
            File.Delete(file);
        }

        foreach (string subDirectory in Directory.GetDirectories(folderPath))
        {
            DeleteFolder(subDirectory);  // ��������� ���� ���θ� ����
        }

        Directory.Delete(folderPath, true); //���� ����

        UnityEngine.Debug.Log("Save data folder deleted.");
    }
}
