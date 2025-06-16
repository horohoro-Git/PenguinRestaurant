using UnityEngine;

public class Rolling : MonoBehaviour
{
    private Animator animator;
    private bool isMoving = false; // �̵� ���¸� ����
    private Vector3 targetPosition; // ��ǥ ��ġ
    Quaternion targetRotation;
    public float moveSpeed; // �̵� �ӵ�
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
        isMoving = true; // �̵� ����
       // animator.SetTrigger("Roll"); // Roll �ִϸ��̼� ����
        animator.SetInteger(AnimationKeys.state, 1);
        moveSpeed = GameInstance.GameIns.gatcharManager.rollingSpeed;
    }

    void Update()
    {
        if (isMoving)
        {
            // ���� ��ġ���� ��ǥ ��ġ�� õõ�� �̵�
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.unscaledDeltaTime); //deltaTime);

            // ��ǥ ��ġ�� �����ߴ��� Ȯ��
            if (Vector3.Distance(transform.position, targetPosition) < 1f)
            {
                transform.position = targetPosition;
                transform.rotation = targetRotation;
                isMoving = false; // �̵� ����
                                  // animator.SetTrigger("Idle A"); // Idle A �ִϸ��̼� ����
                animator.SetInteger(AnimationKeys.state, 0);
            }
        }
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
