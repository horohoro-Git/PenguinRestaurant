using UnityEngine;

public class Money : MonoBehaviour
{
    public void OnMouseDown()
    {
        // 부모 오브젝트의 MoneyPile 스크립트를 가져옴
        MoneyPile moneyPile = GetComponentInParent<MoneyPile>();
        if (moneyPile != null)
        {
            moneyPile.RemoveAllChildren();
        }
    }
}