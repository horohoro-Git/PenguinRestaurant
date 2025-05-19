using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTextFont : MonoBehaviour
{
    private void Awake()
    {
        GameInstance.GameIns.app.GetSceneUI(gameObject);
    }
}
