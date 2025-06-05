using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public AudioListener audioListener;
    public Transform GetTransform;
    private Vector3 moveDir = Vector3.zero;
    public float bounceForce = 3f;

    public Camera cam;
    public float zoomSpeed = 0.1f;
    public float minZoom = 10f;
    public float maxZoom = 30f;

    private float previousDistance = 0f;

    private void Awake()
    {
        GameInstance.GameIns.playerCamera = this;
        audioListener = GetComponentInChildren<AudioListener>();    
    }
    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Touchscreen.current == null || Touchscreen.current.touches.Count < 2)
            return;

        var touch0 = Touchscreen.current.touches[0];
        var touch1 = Touchscreen.current.touches[1];

        if (!touch0.press.isPressed || !touch1.press.isPressed)
            return;

        Vector2 touch0Pos = touch0.position.ReadValue();
        Vector2 touch1Pos = touch1.position.ReadValue();

        float currentDistance = Vector2.Distance(touch0Pos, touch1Pos);

        if (previousDistance == 0f)
        {
            previousDistance = currentDistance;
            return;
        }

        float delta = currentDistance - previousDistance;

        if (cam.orthographic)
        {
            cam.orthographicSize -= delta * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
        else
        {
            cam.fieldOfView -= delta * zoomSpeed;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
        }

        previousDistance = currentDistance;
#endif
    }

    void LateUpdate()
    {
#if UNITY_ANDROID || UNITY_IOS
        // 손이 떨어졌을 때 초기화
        if (Touchscreen.current == null || Touchscreen.current.touches.Count < 2)
        {
            previousDistance = 0f;
        }
#endif
    }
}
