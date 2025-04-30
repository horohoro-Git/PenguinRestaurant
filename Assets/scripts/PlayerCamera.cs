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

            // 이동 방향을 반사 벡터로 조정
            Vector3 bounce = Vector3.Reflect(moveDir, normal) * bounceForce;

            // 부모(CameraRoot) 위치를 튕겨냄
            Transform parent = transform.parent;
            if (parent != null)
            {
                parent.position += bounce;
            }

            // 본인은 원래 자리로 되돌림 (안 하면 계속 충돌)
            transform.localPosition = Vector3.zero;
        }
    }
}
