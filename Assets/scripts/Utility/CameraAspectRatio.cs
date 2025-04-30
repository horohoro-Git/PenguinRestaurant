using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAspectRatio : MonoBehaviour
{
    private Camera _camera;
    private float _targetAspect = 9f / 16f; // 목표 화면 비율 (9:16)
    int _lastWidth;
    int _lastHeight;
    void Start()
    {
        _lastWidth = Screen.width;
        _lastHeight = Screen.height;
        _camera = GetComponent<Camera>();
        UpdateAspectRatio();
    }

    void UpdateAspectRatio()
    {
        // 현재 화면 비율 계산
        float currentAspect = (float)Screen.width / Screen.height;

        // Letterbox 계산
        if (currentAspect > _targetAspect)
        {
            // 화면이 너무 넓음 → 세로에 맞춰 좌우에 블랙 바 추가
            float normalizedWidth = _targetAspect / currentAspect;
            float barThickness = (1f - normalizedWidth) / 2f;
            _camera.rect = new Rect(barThickness, 0, normalizedWidth, 1);
        }
        else
        {
            // 화면이 너무 높음 → 가로에 맞춰 상하에 블랙 바 추가
            float normalizedHeight = currentAspect / _targetAspect;
            float barThickness = (1f - normalizedHeight) / 2f;
            _camera.rect = new Rect(0, barThickness, 1, normalizedHeight);
        }
    }

    void Update()
    {
        if (Screen.width != _lastWidth || Screen.height != _lastHeight)
        {
            UpdateAspectRatio();
        }
    }
}