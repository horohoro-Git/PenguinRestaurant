using UnityEngine;

public class Rolling : MonoBehaviour
{
    private Animator animator;
    private bool isMoving = false; // 이동 상태를 추적
    private Vector3 targetPosition; // 목표 위치

    public float moveSpeed; // 이동 속도
    public int type;


    public void Roll()
    {
        targetPosition = Vector3.zero + new Vector3(this.transform.position.x, 0, -1000);
        animator = GetComponentInChildren<Animator>();
        isMoving = true; // 이동 시작
        animator.SetTrigger("Roll"); // Roll 애니메이션 시작
        moveSpeed = GameInstance.GameIns.gatcharManager.rollingSpeed;
    }

    void Update()
    {
        if (isMoving)
        {
            // 현재 위치에서 목표 위치로 천천히 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.unscaledDeltaTime); //deltaTime);

            // 목표 위치에 도달했는지 확인
            if (Vector3.Distance(transform.position, targetPosition) < 1f)
            {
                transform.position = targetPosition;
                isMoving = false; // 이동 중지
                animator.SetTrigger("Idle A"); // Idle A 애니메이션 실행
            }
        }
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
