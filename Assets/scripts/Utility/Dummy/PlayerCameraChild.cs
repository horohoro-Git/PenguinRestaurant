using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraChild : MonoBehaviour
{
    public LayerMask mapCollisionMask; // �� ��� ���̾�

    public Vector3 boxSize = new Vector3(1, 1, 1); // �浹 ���� �ڽ� ũ��
    public float bounceForce = 1f;

    void LateUpdate()
    {
      /*  Vector3 checkPosition = transform.position;

        // �ֺ� �ڽ� ���� �浹 ����� �ִ��� Ȯ��
        Collider[] hits = Physics.OverlapBox(checkPosition, boxSize * 1, Quaternion.identity, 0b10000000);

        if (hits.Length > 0)
        {
            // �ٿ ���� ���
            Vector3 bounceDir = (transform.parent.position - checkPosition).normalized;

            // �ٿ ó��
            transform.parent.position += bounceDir * bounceForce;

            Debug.Log("�ٿ �߻�");
        }*/
    }

}
