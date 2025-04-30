using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpGradeExitBtn : MonoBehaviour
{
    public Button UpGradeMenuExitBtn;

    private void Start()
    {
        UpGradeMenuExitBtn.onClick.AddListener(() => Sceneoff());
    }
    void Sceneoff()
    {
  //      SceneManager.UnloadScene("UpGradeScense");

    }
}
