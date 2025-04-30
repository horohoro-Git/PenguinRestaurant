using UnityEngine;
using System.Collections;

public class Zoom : MonoBehaviour
{
    public Camera cameraToMove; // 이동할 카메라
    public Transform playerObject; // 거리 기준이 될 플레이어 오브젝트
    private Vector3 originalPosition; // 카메라의 원래 위치
    private Quaternion originalRotation; // 카메라의 원래 회전
    public static bool isMoving; // 카메라 이동 상태

    private Transform targetObject; // 카메라가 초점을 맞출 오브젝트
    private const float smoothSpeed = 0.1f; // 카메라 이동 속도 (고정값)

    public bool skip =false;

    private void OnEnable()
    {
        if (skip == false)
        {
            targetObject = transform; // 자신을 타겟 오브젝트로 설정
            isMoving = true; // 카메라 이동 시작
            StartCoroutine(PositionCameraWithDelay(1f)); // 1초 뒤에 카메라 위치 조정
        }
    }

    private IEnumerator PositionCameraWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 지연 시간 대기
        StartCoroutine(SmoothMoveCamera());
    }

    private IEnumerator SmoothMoveCamera()
    {
        if (cameraToMove == null || playerObject == null)
        {
            yield break; // 카메라나 플레이어가 없으면 종료
        }

        // 카메라의 원래 위치와 회전 저장
        originalPosition = cameraToMove.transform.position;
        originalRotation = cameraToMove.transform.rotation;

        // 타겟 오브젝트의 위치
        Vector3 targetPosition = targetObject.position;

        // 카메라와 플레이어 간의 거리 계산
        float distanceFromTarget = Vector3.Distance(cameraToMove.transform.position, playerObject.position);

        // 카메라의 목표 위치 계산
        Vector3 cameraOffset = new Vector3(0, 0, -distanceFromTarget); // Z축으로 이동
        Vector3 newCameraPosition = targetPosition + cameraToMove.transform.rotation * cameraOffset;

        // 카메라를 부드럽게 이동
        float elapsedTime = 0f;
        Vector3 startingPosition = cameraToMove.transform.position;

        while (elapsedTime < smoothSpeed)
        {
            cameraToMove.transform.position = Vector3.Lerp(startingPosition, newCameraPosition, elapsedTime / smoothSpeed);
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 마지막 위치를 정확히 설정
        cameraToMove.transform.position = newCameraPosition;

        // 오브젝트를 바라보도록 카메라 회전
        cameraToMove.transform.LookAt(targetPosition);

        // 1초 대기 후 원래 위치로 돌아가기 
        yield return new WaitForSeconds(1f);
        StartCoroutine(ReturnToOriginalPosition());
    }

    private IEnumerator ReturnToOriginalPosition()
    {
        float elapsedTime = 0f;

        while (elapsedTime < smoothSpeed)
        {
            cameraToMove.transform.position = Vector3.Lerp(cameraToMove.transform.position, originalPosition, elapsedTime / smoothSpeed);
            cameraToMove.transform.rotation = Quaternion.Lerp(cameraToMove.transform.rotation, originalRotation, elapsedTime / smoothSpeed);
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 원래 위치와 회전을 정확히 설정
        cameraToMove.transform.position = originalPosition;
        cameraToMove.transform.rotation = originalRotation;

        isMoving = false; // 카메라 이동 종료
    }
}
