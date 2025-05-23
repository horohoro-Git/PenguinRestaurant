using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthographicCamera : MonoBehaviour
{
    bool isChangingCameraZoom;

    public void ZoomOut()
    {
        if (!isChangingCameraZoom)
        {
            isChangingCameraZoom = true;
            StartCoroutine(CameraZoomOut());
        }
    }

    IEnumerator CameraZoomOut()
    {
        while (true)
        {
            if(InputManger.cachingCamera.orthographicSize >=30)
            {
                isChangingCameraZoom = false;
                yield break;
            }
            else
            {
                float target = 30;
                float current = InputManger.cachingCamera.orthographicSize;
                float f = 0;
                while (f < 0.5f)
                {
                    float t = Mathf.Lerp(current, target, f * 2);
                    InputManger.cachingCamera.orthographicSize = t;

                    f += Time.deltaTime;
                    yield return null;
                }
                InputManger.cachingCamera.orthographicSize = target;
            }
            yield return null;
        }
    }
}
