using UnityEngine;

public class TableScaleUp : MonoBehaviour
{
    public float duration = 0.3f; // 애니메이션 지속 시간
    public float bounceIntensity = 0.3f; // 진동 강도
    private Vector3 initialScale; // 초기 스케일
    private float elapsedTime = 0f; // 경과 시간
    private int bounceCount = 2; // 반복 횟수
    private int currentBounce = 0; // 현재 반복 카운터
    bool load;
    private void OnEnable()
    {
        // 초기값 설정
       // initialScale = transform.localScale; // 현재 스케일을 초기 스케일로 설정
       // elapsedTime = 0f; // 경과 시간 초기화
      //  currentBounce = 0; // 현재 반복 카운터 초기화
    }

    public void ObjectEnabled(bool load)
    {
        this.load = load;
        initialScale = new Vector3(3, 1, 3);  //transform.localScale; // 현재 스케일을 초기 스케일로 설정
        if (!load)
        {

            elapsedTime = 0f; // 경과 시간 초기화
            currentBounce = 0; // 현재 반복 카운터 초기화                              // saveLoadManager = GameObject.Find("SaveLoadManager").GetComponent<SaveLoadManager>();
        }
        else
        {
            transform.localScale = initialScale;
        }
      //  if (!load) Bounce();
    }
    private void Update()
    {
        // 애니메이션 진행 중이면
        if (currentBounce < bounceCount && !load)
        {
            elapsedTime += Time.deltaTime; // 경과 시간 업데이트
            float t = elapsedTime / duration; // 비율 계산

            // 홀수번째: 가로로만 커짐, 짝수번째: 세로로만 커짐
            float scaleMultiplier = 1 + Mathf.Sin(t * Mathf.PI) * bounceIntensity; // 진동 효과

            // 애니메이션이 끝나면 진동 강도 감소
            if (t >= 1f)
            {
                bounceIntensity *= 0.5f; // 진동 강도에 0.5을 곱함
                elapsedTime = 0f; // 경과 시간 초기화
                currentBounce++; // 반복 카운터 증가
            }

            Vector3 scaleChange = currentBounce % 2 == 0
                ? new Vector3(scaleMultiplier, 1, 1) // 짝수: 가로로만
                : new Vector3(1, 1, scaleMultiplier); // 홀수: 세로로만

            // 각 축을 개별적으로 곱해줌
            transform.localScale = new Vector3(
                initialScale.x * scaleChange.x,
                initialScale.y * scaleChange.y,
                initialScale.z * scaleChange.z
            );
        }
        else
        {
            // 모든 반복이 끝나면 원래 크기로 복원
            transform.localScale = initialScale;
        }
    }
}
