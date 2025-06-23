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
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rectTransform = GetComponent<RectTransform>();
        exclamation.sprite = AssetLoader.loadedAtlases["UI"].GetSprite("exclamation");
        
    }

    private void Update()
    {
        if (foodMachine != null)
        {
            Vector3 offset = new Vector3(-3.5f, 10f, -3.5f);
           
            rectTransform.position = foodMachine.transform.position + offset;

        }
    }


    public void UpdateGage(int energy, bool init)
    {
        if (!init)
        {
            WorkSpaceManager workSpaceManager = GameInstance.GameIns.workSpaceManager;
            for (int i = 0; i < workSpaceManager.foodMachines.Count; i++)
            {
                if (workSpaceManager.foodMachines[i].fuelGage != null && workSpaceManager.foodMachines[i].machineType == foodMachine.machineType && workSpaceManager.foodMachines[i] != foodMachine)
                {
                    workSpaceManager.foodMachines[i].fuelGage.energy += energy;
                    if (workSpaceManager.foodMachines[i].fuelGage.energy == 0)
                    {
                        workSpaceManager.foodMachines[i].fuelGage.exclamation.enabled = true;
                        workSpaceManager.foodMachines[i].fuelGage.animator.enabled = true;
                        workSpaceManager.foodMachines[i].fuelGage.animator.SetInteger(AnimationKeys.state, 1);
                    }
                    else
                    {
                        workSpaceManager.foodMachines[i].fuelGage.animator.SetInteger(AnimationKeys.state, 0);
                        workSpaceManager.foodMachines[i].fuelGage.animator.enabled = false;
                        workSpaceManager.foodMachines[i].fuelGage.exclamation.enabled = false;
                    }
                   // StartCoroutine(workSpaceManager.foodMachines[i].fuelGage.ChangingGage());
                }
            }
        }
        this.energy += energy;
        if(this.energy == 0)
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

        GameInstance.GameIns.restaurantManager.machineLevelDataChanged = true;
       // StartCoroutine(ChangingGage());
    }

  /*  public IEnumerator ChangingGage()
    {
        float cur = foreground.fillAmount;
        float target =  (float)energy / 100;
        float f = 0;
        while(f <= 0.2f)
        {
            float next = Mathf.Lerp(cur, target, f * 5);
            foreground.fillAmount = next;
            ChangeColor(next);
            f += Time.deltaTime;
            yield return null;
        }
        float amount = (float)energy / 100;
     
        foreground.fillAmount = amount;
        ChangeColor(amount);

    }*/

   
}
