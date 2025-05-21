using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraChild : MonoBehaviour
{
    public LayerMask mapCollisionMask; // 맵 경계 레이어

    public Vector3 boxSize = new Vector3(1, 1, 1); // 충돌 감지 박스 크기
    public float bounceForce = 1f;

    void LateUpdate()
    {
      /*  Vector3 checkPosition = transform.position;

        // 주변 박스 내에 충돌 대상이 있는지 확인
        Collider[] hits = Physics.OverlapBox(checkPosition, boxSize * 1, Quaternion.identity, 0b10000000);

        if (hits.Length > 0)
        {
            // 바운스 방향 계산
            Vector3 bounceDir = (transform.parent.position - checkPosition).normalized;

            // 바운스 처리
            transform.parent.position += bounceDir * bounceForce;

            Debug.Log("바운스 발생");
        }*/
    }

}
