using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform GetTransform;
    private Vector3 moveDir = Vector3.zero;
    public float bounceForce = 3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            Debug.Log("AA");
            Vector3 normal = collision.contacts[0].normal;

            // �̵� ������ �ݻ� ���ͷ� ����
            Vector3 bounce = Vector3.Reflect(moveDir, normal) * bounceForce;

            // �θ�(CameraRoot) ��ġ�� ƨ�ܳ�
            Transform parent = transform.parent;
            if (parent != null)
            {
                parent.position += bounce;
            }

            // ������ ���� �ڸ��� �ǵ��� (�� �ϸ� ��� �浹)
            transform.localPosition = Vector3.zero;
        }
    }
}
