using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScalerController : MonoBehaviour
{
    private void Awake()
    {
        CanvasScaler scaler = GetComponent<CanvasScaler>();

        float ratio = (float)Screen.width / Screen.height;
        float targetRatio = 9f / 16f;



        scaler.matchWidthOrHeight = targetRatio < ratio ? 0 : 1;
    }
}
