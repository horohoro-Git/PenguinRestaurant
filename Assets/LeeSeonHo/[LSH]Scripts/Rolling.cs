using UnityEngine;

public class Rolling : MonoBehaviour
{
    private Animator animator;
    private bool isMoving = false; // 이동 상태를 추적
    private Vector3 targetPosition; // 목표 위치
    Quaternion targetRotation;
    public float moveSpeed; // 이동 속도
    public int type;

    public Shadow shadow;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void Roll(Vector3 pos, Vector3 rot)
    {
        targetPosition = Vector3.zero + pos;
        targetRotation = Quaternion.Euler(rot);
        isMoving = true; // 이동 시작
       // animator.SetTrigger("Roll"); // Roll 애니메이션 시작
        animator.SetInteger(AnimationKeys.state, 1);
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
                transform.rotation = targetRotation;
                isMoving = false; // 이동 중지
                                  // animator.SetTrigger("Idle A"); // Idle A 애니메이션 실행
                animator.SetInteger(AnimationKeys.state, 0);
            }
        }
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
