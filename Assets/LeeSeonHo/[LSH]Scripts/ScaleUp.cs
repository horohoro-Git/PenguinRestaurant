using UnityEngine;
using System.Collections;

public class ScaleUp : MonoBehaviour
{
    public float duration = 0.3f; // 애니메이션 지속 시간
    public float bounceIntensity = 0.3f; // 진동 강도

    public bool isTable;

    private Vector3 initialScale; // 초기 스케일
    private int bounceCount = 2; // 반복 횟수

    private SaveLoadManager saveLoadManager;

    /*private void OnEnable()
    {
        if (GetComponent<Table>() != null)
        {
            isTable = true;
        }
        // 초기값 설정
        initialScale = transform.localScale; // 현재 스케일을 초기 스케일로 설정
       // saveLoadManager = GameObject.Find("SaveLoadManager").GetComponent<SaveLoadManager>();

        Bounce();
    }*/
    public void ObjectEnabled(bool load)
    {
        if (GetComponent<Table>() != null)
        {
            isTable = true;
        }
        // 초기값 설정
        initialScale = transform.localScale; // 현재 스케일을 초기 스케일로 설정
                                             // saveLoadManager = GameObject.Find("SaveLoadManager").GetComponent<SaveLoadManager>();

       if(!load) Bounce();
    }

    public void Bounce()
    {
     //   if(saveLoadManager.isLoading == true)
        {
    //        return;
        }
      //  else
        {
            if (isTable)
            {
                StartCoroutine(TableBounceCoroutine());
            }
            else
            {
                StartCoroutine(BounceCoroutine());
            }
        }        
    }


    private IEnumerator BounceCoroutine()
    {
        int currentBounce = 0;
        float currentBounceIntensity = bounceIntensity;

        while (currentBounce < bounceCount)
        {
            float elapsedTime = 0f;

            // 한 번의 Bounce가 진행될 동안의 루프
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                // 홀수번째: 가로로만 커짐, 짝수번째: 세로로만 커짐
                float scaleMultiplier = 1 + Mathf.Sin(t * Mathf.PI) * currentBounceIntensity;

                Vector3 scaleChange = currentBounce % 2 == 0
                    ? new Vector3(scaleMultiplier, 1, 1) // 짝수: 가로로만
                    : new Vector3(1, scaleMultiplier, 1); // 홀수: 세로로만

                transform.localScale = new Vector3(
                    initialScale.x * scaleChange.x,
                    initialScale.y * scaleChange.y,
                    initialScale.z * scaleChange.z
                );

                yield return null; // 한 프레임 대기
            }

            // Bounce가 끝날 때 진동 강도 감소
            currentBounceIntensity *= 0.5f;
            currentBounce++;
        }

        // 모든 반복이 끝나면 원래 크기로 복원
        transform.localScale = initialScale;
    }

    private IEnumerator TableBounceCoroutine()
    {
        int currentBounce = 0;
        float currentBounceIntensity = bounceIntensity;

        while (currentBounce < bounceCount)
        {
            float elapsedTime = 0f;

            // 한 번의 Bounce가 진행될 동안의 루프
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                // 홀수번째: 가로로만 커짐, 짝수번째: 세로로만 커짐
                float scaleMultiplier = 1 + Mathf.Sin(t * Mathf.PI) * currentBounceIntensity;

                Vector3 scaleChange = currentBounce % 2 == 0
                    ? new Vector3(scaleMultiplier, 1, 1) // 짝수: 가로로만
                    : new Vector3(1, 1, scaleMultiplier); // 홀수: 세로로만

                transform.localScale = new Vector3(
                    initialScale.x * scaleChange.x,
                    initialScale.y * scaleChange.y,
                    initialScale.z * scaleChange.z
                );

                yield return null; // 한 프레임 대기
            }

            // Bounce가 끝날 때 진동 강도 감소
            currentBounceIntensity *= 0.5f;
            currentBounce++;
        }

        // 모든 반복이 끝나면 원래 크기로 복원
        transform.localScale = initialScale;
    }
}
