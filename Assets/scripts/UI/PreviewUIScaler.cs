using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���ұ׷��� ����� ���� UI�� ũ�⸦ �����ϴ� ���
public class PreviewUIScaler : MonoBehaviour
{
    RectTransform canvas;
    private void Awake()
    {
        canvas = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if(InputManger.cachingCamera != null)
        {
            if (InputManger.cachingCamera.orthographic)
            {
                float size = InputManger.cachingCamera.orthographicSize;
                float target = size / 15f;

                Vector3 scalerSize = new Vector3(target, target, target);
                canvas.localScale = scalerSize;
            }
        }
    }
}
