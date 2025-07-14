using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerCamera : MonoBehaviour
{
    public AudioListener audioListener;
    public Transform GetTransform;
    private Vector3 moveDir = Vector3.zero;
    public CinemachineBrain brain;
    public CinemachineBlendListCamera listCamera;
    public float bounceForce = 3f;

    public Camera cam;
    float zoomSpeed = 0.01f;
    public float minZoom = 10f;
    public float maxZoom = 30f;
    private float targetZoom;
    private float previousDistance = 0f;
    public float smoothTime = 0.01f;

    float zoomVelocity;
    bool isZooming;
    float initialDistance;
    private void Awake()
    {
        targetZoom = 15;
        GameInstance.GameIns.playerCamera = this;
        audioListener = GetComponentInChildren<AudioListener>();    
    }
    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        if(RestaurantManager.restaurantTimer > 0)
        {
            if (Touchscreen.current != null && Touchscreen.current.touches.Count >= 2)
            {
                var touch0 = Touchscreen.current.touches[0];
                var touch1 = Touchscreen.current.touches[1];

                if (touch0.isInProgress && touch1.isInProgress)
                {
                    Vector2 touch0Pos = touch0.position.ReadValue();
                    Vector2 touch1Pos = touch1.position.ReadValue();

                    float currentDistance = Vector2.Distance(touch0Pos, touch1Pos);

                    if (!isZooming)
                    {
                        initialDistance = currentDistance;
                        isZooming = true;
                    }
                    float distanceDelta = currentDistance - initialDistance;
                    UpdateTargetZoom(distanceDelta);
                    initialDistance = currentDistance;
                }
                else
                {
                    isZooming = false;
                }
            }
            else
            {
                isZooming = false;
            }
            ApplyZoom();
        }
#endif
    }
    private void UpdateTargetZoom(float distanceDelta)
    {
        if (InputManger.cachingCamera.orthographic)
        {
            targetZoom = InputManger.cachingCamera.orthographicSize - (distanceDelta * zoomSpeed);
        }
      
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
      
    }

    private void ApplyZoom()
    {
        if (InputManger.cachingCamera.orthographic)
        {
            InputManger.cachingCamera.orthographicSize = Mathf.SmoothDamp(
                InputManger.cachingCamera.orthographicSize,
                targetZoom,
                ref zoomVelocity,
                smoothTime);
        }
    }
    public static void ApplySafeArea(RectTransform panel)
    {
        float topMargin = 60f / Screen.height;    // iOS 최대 노치 높이 반영
        float bottomMargin = 80f / Screen.height; // 홈 인디케이터 + 추가 여유

        panel.anchorMin = new Vector2(0, bottomMargin);
        panel.anchorMax = new Vector2(1, 1 - topMargin);
    }
}
