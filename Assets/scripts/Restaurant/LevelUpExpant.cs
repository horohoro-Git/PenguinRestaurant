using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글

public class LevelUpExpant : MonoBehaviour
{
    public GameObject[] hideOBject;
    public GameObject[] visibleObject;


    // Start is called before the first frame update

    void Start()
    {
        for (int i = 0; i < hideOBject.Length; i++)
        {
            hideOBject[i].SetActive(false);
        }
        for (int i = 0; i < visibleObject.Length; i++)
        {
            visibleObject[i].SetActive(true);
        }
        MoveCalculator.CheckArea(GameInstance.GameIns.calculatorScale, true);
    }
}
