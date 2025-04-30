using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentMoney : MonoBehaviour
{
    public int currentMoney = 10000; // 초기 돈 설정
    public TextMeshProUGUI moneyText; // UI 텍스트를 연결할 변수

    public void Start()
    {
        UpdateMoneyText();
    }

    // 돈을 추가하는 메서드
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyText();
    }

    // 돈을 소모하는 메서드
    public void SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyText();
        }
        else
        {
            //Debug.Log("돈이 부족합니다!");
        }
    }

    // UI 텍스트 업데이트 메서드
    private void UpdateMoneyText()
    {        
        if (currentMoney >= 1_000_000_000)
        {
            // 1_000_000_000 이상일 경우 B 단위로 표시
            float valueInK = currentMoney / 1_000_000_000f;
            moneyText.text = valueInK.ToString("F2") + "B";
        }
        else if (currentMoney >= 1_000_000)
        {
            // 1000000 이상일 경우 M 단위로 표시
            float valueInK = currentMoney / 1_000_000f;
            moneyText.text = valueInK.ToString("F2") + "M";
        }
        else if (currentMoney >= 1_000)
        {
            // 1000 이상일 경우 K 단위로 표시
            float valueInK = currentMoney / 1_000f;
            moneyText.text = valueInK.ToString("F2") + "K";
        }
        else
        {
            // 1000 미만일 경우 일반 표시
            moneyText.text = currentMoney.ToString();
        }
    }
}
