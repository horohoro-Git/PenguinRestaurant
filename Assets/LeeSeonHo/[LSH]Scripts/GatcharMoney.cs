using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GatcharMoney : MonoBehaviour
{
    public TextMeshProUGUI moneyText;    
    public float money;
    public float price;

    // Update is called once per frame
    void Update()
    {
        moneyText.text = money.ToString();
    }

    public void Purchase()
    {
        money -= price;
    }
}