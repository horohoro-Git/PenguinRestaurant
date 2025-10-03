using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTextFont : MonoBehaviour
{
    private void Awake()
    {
        if(GameInstance.GameIns.app) GameInstance.GameIns.app.GetSceneUI(gameObject);
    }

    private void OnEnable()
    {
        if (GameInstance.GameIns.app) GameInstance.GameIns.app.GetSceneUI(gameObject);
    }
}
