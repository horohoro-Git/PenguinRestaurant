using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//한글

public class CoroutineTest : MonoBehaviour
{
    TMP_Text text;
    private void Awake()
    {
        text = GetComponent<TMP_Text>();

    }

    private void Update()
    {
        text.text = GameInstance.GameIns.coroutneManager.GetCoroutines().ToString();
    }
}
