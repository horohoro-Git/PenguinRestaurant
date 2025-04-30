using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAspectRatio : MonoBehaviour
{
    private Camera _camera;
    private float _targetAspect = 9f / 16f; // ��ǥ ȭ�� ���� (9:16)
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
        // ���� ȭ�� ���� ���
        float currentAspect = (float)Screen.width / Screen.height;

        // Letterbox ���
        if (currentAspect > _targetAspect)
        {
            // ȭ���� �ʹ� ���� �� ���ο� ���� �¿쿡 �� �� �߰�
            float normalizedWidth = _targetAspect / currentAspect;
            float barThickness = (1f - normalizedWidth) / 2f;
            _camera.rect = new Rect(barThickness, 0, normalizedWidth, 1);
        }
        else
        {
            // ȭ���� �ʹ� ���� �� ���ο� ���� ���Ͽ� �� �� �߰�
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