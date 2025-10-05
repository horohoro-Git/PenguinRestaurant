using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OrthographicCamera : MonoBehaviour
{
    Camera cam;

    private void Awake()
    {
        cam = GetComponentInParent<Camera>();
    }
    bool isChangingCameraZoom;
    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera.CompareTag("MainCamera")) 
        {
            if ((camera.cullingMask & (1 << LayerMask.NameToLayer("NoCulling"))) != 0)
            {
                camera.cullingMatrix = Matrix4x4.Ortho(-100, 100, -100, 100, 0.001f, 1000) *
                           camera.worldToCameraMatrix;
            }
        }
     
    }
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
            if(InputManager.cachingCamera.orthographicSize >=30)
            {
                isChangingCameraZoom = false;
                yield break;
            }
            else
            {
                float target = 30;
                float current = InputManager.cachingCamera.orthographicSize;
                float f = 0;
                while (f < 0.5f)
                {
                    float t = Mathf.Lerp(current, target, f * 2);
                    InputManager.cachingCamera.orthographicSize = t;

                    f += Time.deltaTime;
                    yield return null;
                }
                InputManager.cachingCamera.orthographicSize = target;
            }
            yield return null;
        }
    }
}
