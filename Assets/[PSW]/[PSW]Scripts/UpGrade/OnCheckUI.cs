using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnCheckUI : MonoBehaviour
{
    public GameObject onUpGradeMenu;
    public GameObject offUpGradeMenu;



    public Button inUpGradeBtn;

    public void OnUpGradeMenu_Btn()
    {
        onUpGradeMenu.SetActive(true);
    }

    public void OnUpGradeMenu_back_Btn()
    {
        offUpGradeMenu.SetActive(false);
    }
}
