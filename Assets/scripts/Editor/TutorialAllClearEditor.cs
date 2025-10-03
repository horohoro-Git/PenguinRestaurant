using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class TutorialAllClearEditor : EditorWindow
{
    [MenuItem("Tools/TutorialAllClear %F1")]
    public static void TutorialClear()
    {
        Clear();
    }


    static void Clear()
    {
        Tutorials tutorial = GameInstance.GameIns.restaurantManager.tutorials;

        Dictionary<int, List<TutorialStruct>> tutorialStructs = GameInstance.GameIns.restaurantManager.tutorialStructs;

        List<TutorialStruct> lastTutorialStruct = new();
        foreach (var v in tutorialStructs) lastTutorialStruct = v.Value;

        if(tutorial.id <= lastTutorialStruct[0].id)
        {
            tutorial.id = lastTutorialStruct[0].id + 1;
            tutorial.worked = true;
            int checkTier = 0;
            foreach(var v in AnimalManager.gatchaTiers)
            {
                (int, List<int>, bool) f = v.Value;
                checkTier += f.Item1;
            }

            if(checkTier == 0)
            {
                //못 받은 고양이 손님 추가
                GameInstance.GameIns.gatcharManager.CheckSuccess(100, 0);
            }

            SaveLoadSystem.SaveTutorialData(tutorial);
            Tutorials.TutorialUpdate(tutorial.id);
            GameInstance.GameIns.uiManager.TutorialEnd(true);
        }
    }
}
