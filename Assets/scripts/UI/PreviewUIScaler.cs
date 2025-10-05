using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//오소그래픽 사이즈에 따라서 UI의 크기를 변경하는 기능
public class PreviewUIScaler : MonoBehaviour
{
    RectTransform canvas;
    private void Awake()
    {
        canvas = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if(InputManager.cachingCamera != null)
        {
            if (InputManager.cachingCamera.orthographic)
            {
                float size = InputManager.cachingCamera.orthographicSize;
                float target = size / 15f;

                Vector3 scalerSize = new Vector3(target, target, target);
                canvas.localScale = scalerSize;
            }
        }
    }
}
