using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public float money = 100000f;
    public TMP_Text text;

    void Start()
    {
        GameInstance.GameIns.moneyManager = this;
    }

  /*  void Update()
    {
        text.text = money.ToString();
    }
*/
    public void UserMoney(float foodPrice)
    {
        this.money += foodPrice;
        text.text = money.ToString();
    }
}
