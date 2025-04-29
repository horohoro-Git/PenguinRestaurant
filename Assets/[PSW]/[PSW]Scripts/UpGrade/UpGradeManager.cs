using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpGradeManager : MonoBehaviour
{
    public GameObject onUpGradeMenu;
    //[SerializeField] public Button firstButton;
    [SerializeField] public Button scendButton;

    
    FoodList.foodPriceType foodPriceType;
    private void Start()
    {
        
       
        //scendButton.onClick.AddListener(() => OnUpGradeMenu_Btn());
    }


    public void OnUpGradeMenu_Btn()
    {
        SceneManager.LoadScene("UpGradeScense", LoadSceneMode.Additive);
        //onUpGradeMenu.SetActive(true);
    }
}
