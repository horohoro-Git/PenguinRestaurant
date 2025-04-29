using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using UnityEditor.Animations; // ***** TextMeshPro�� ����ϱ� ���� �߰� *****

public class SliderController : MonoBehaviour
{
    public Slider slider; // ***** Slider UI ���� ���� *****
    public float incrementAmount = 10f; // ***** ������ *****
    public float duration = 0.3f; // ***** �ε巴�� �����ϴ� �ð� (��) *****
    public TextMeshProUGUI levelText; // ***** TextMeshPro ���� ���� ***** 

    private float targetValue; // ***** ���� ��ǥ�� *****
    private Coroutine currentCoroutine; // ***** ���� ���� ���� Coroutine *****
    private int level = 1; // ***** ���� ���� *****

    public Transform model;
    public RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        // targetValue = slider.value;
        //  level = 1; // ***** �ʱ� ���� ���� *****
        UpdateLevelText(); // ***** �ؽ�Ʈ �ʱ�ȭ *****
        currentCoroutine = StartCoroutine(SmoothIncrease());

        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (model != null)
        {
            Vector3 rectPosition = GameInstance.GetVector3(model.position.x - 50f, model.position.y + 120, model.position.z - 50f);
            rectTransform.position = rectPosition; // new Vector3(model.position.x - 50f, model.position.y + 120, model.position.z - 50f);
        }

    }

    

    public void GainExperience()
    {
        // �̹� ���� ���� Coroutine�� �ִٸ� ����
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // ���ο� Coroutine ����
        currentCoroutine = StartCoroutine(SmoothIncrease());
    }

   
    private IEnumerator SmoothIncrease()
    {
        float startValue = slider.value;
        float elapsedTime = 0f;

        // ***** �ִϸ��̼� ���� *****
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, Mathf.Min(targetValue, slider.maxValue), elapsedTime / duration);
            yield return null;
        }

        // ***** ������ ���� *****
        slider.value = Mathf.Min(targetValue, slider.maxValue);
        // Coroutine�� ������ null�� ����
        currentCoroutine = null;

        // ***** �����̴��� �� á���� Ȯ�� *****
        if (slider.value >= slider.maxValue)
        {
            float overflowValue = targetValue - slider.maxValue; // ***** targetValue���� �ʰ� �� ��� *****
          //  LevelUp(overflowValue);
        }
    }

    private void LevelUp(float overflowValue)
    {
        level++; // ***** ���� ���� *****
        slider.value = 0; // ***** �����̴� �ʱ�ȭ *****
        targetValue = overflowValue; // ***** �ʰ��� ���� ���ο� ��ǥ������ ���� *****
        UpdateLevelText(); // ***** �ؽ�Ʈ ������Ʈ *****

        // ***** �ʰ� ���� ���� ��� �ִϸ��̼��� �̾ ���� *****
        if (overflowValue > 0)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(SmoothIncrease());
        }
    }


    public void ClearEXP()
    {
        StopAllCoroutines();
        StartCoroutine(Clear());
    }

    IEnumerator Clear()
    {
        targetValue = 0;
      //  yield return new WaitForSeconds(0.f);
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Lerp(Mathf.Min(targetValue, slider.maxValue), 0, elapsedTime / duration);
            yield return null;
        }
        slider.value = 0f;
    }

    public void UpdateEXP(int value)
    {
        targetValue += (float)value;
    }

    public void UpdateLevel(int level)
    {
        this.level = level;
        if(levelText != null) levelText.text = $"Level {level}";
    
    }

    void UpdateLevelText()
    {
        levelText.text = $"Level {level}"; // ***** �ؽ�Ʈ�� "Level X"�� ���� *****
    }
}
