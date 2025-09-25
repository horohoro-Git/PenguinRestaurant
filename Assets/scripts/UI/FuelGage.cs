using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelGage : MonoBehaviour
{
    [NonSerialized] public Animator animator;
    public FoodMachine foodMachine;
    public Image exclamation;
    RectTransform rectTransform;
    int energy = 0;
    Color green = Color.green;
    Color yellowGreen = new Color(50, 205, 50);
    Color yellow = Color.yellow;
    Color red = Color.red;
    bool show;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rectTransform = GetComponent<RectTransform>();
        exclamation.sprite = AssetLoader.loadedAtlases["UI"].GetSprite("exclamation");
        exclamation.enabled = false;
        animator.enabled = false;
    }

    private void Update()
    {
        if (foodMachine != null)
        {
            switch (foodMachine.rotateLevel)
            {
                case 0:
                    Vector3 offset0 = new Vector3(-4f, 10f, -6f);
                    rectTransform.position = foodMachine.transform.position + offset0;
                    break;
                case 1:
                    Vector3 offset1 = new Vector3(-4f, 10f, -6f);
                    rectTransform.position = foodMachine.transform.position + offset1;
                    break;
                case 2:
                    Vector3 offset2 = new Vector3(-6f, 10f, -4f);
                    rectTransform.position = foodMachine.transform.position + offset2;
                    break;
                case 3:
                    Vector3 offset3 = new Vector3(-6f, 10f, -4f);
                    rectTransform.position = foodMachine.transform.position + offset3;
                    break;
            
            }

        }
    }


    public void UpdateGage(FoodMachine machine, int energy, bool init)
    {
        if (!init)
        {
            WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;
      
            for (int i = 0; i < workSpaceManager.foodMachines.Count; i++)
            {
                if (workSpaceManager.foodMachines[i].fuelGage != null && workSpaceManager.foodMachines[i].machineType == foodMachine.machineType && workSpaceManager.foodMachines[i] != foodMachine)
                {
                    workSpaceManager.foodMachines[i].fuelGage.energy += energy;
                }
            }
        }
        this.energy += energy;
      

        GameInstance.GameIns.restaurantManager.machineLevelDataChanged = true;
       // StartCoroutine(ChangingGage());
    }
    public void ShowGage(bool hasFish)
    {
       // if (show == bShow) return;
        show = hasFish;
      

        if (!show)
        {
            exclamation.enabled = true;
            animator.enabled = true;
            animator.SetInteger(AnimationKeys.state, 1);
           
        }
        else
        {
            animator.SetInteger(AnimationKeys.state, 0);
            animator.enabled = false;
            exclamation.enabled = false;
        }
    }
}
