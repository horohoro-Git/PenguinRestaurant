using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterQuality : MonoBehaviour
{
    public Color cleanShallowColor;
    public Color cleanHorizonColor;
    public Color dirtyShallowColor;
    public Color dirtyHorizonColor;

    [NonSerialized] public Material material;

    [NonSerialized] public bool isDirty;
    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
      //  StartCoroutine(ChangeDirtyQuality());
    }

    public void ChangeDirty(bool animate = true)
    {
        if(animate) StartCoroutine(ChangeDirtyQuality());
        else
        {
       
            material.SetColor("_ShallowColor", dirtyShallowColor);
            material.SetColor("_HorizonColor", dirtyHorizonColor);
            isDirty = true;
        }
    }

    public void ChangeClean(bool animate = true)
    {
        if (animate) StartCoroutine (ChangeCleanQuality());  
        else
        {
            material.SetColor("_ShallowColor", cleanShallowColor);
            material.SetColor("_HorizonColor", cleanHorizonColor);
            isDirty = false;
        }
    }

    IEnumerator ChangeDirtyQuality()
    {
        float f = 0;
        Color startShallow = material.GetColor("_ShallowColor");
        Color targetShallow = dirtyShallowColor;
        Color startHorizon = material.GetColor("_HorizonColor");
        Color targetHorizon = dirtyHorizonColor;
        while (f <= 5f)
        {
            Color shallowColor = Color.Lerp(startShallow, targetShallow, f / 5);
            Color horizonColor = Color.Lerp(startHorizon, targetHorizon, f / 5);
            material.SetColor("_ShallowColor", shallowColor);
            material.SetColor("_HorizonColor", horizonColor);
            f += Time.deltaTime;    
            yield return null;
        }
        isDirty = true;
        GameInstance.GameIns.restaurantManager.miniGame.fishing.isDirty = true;
        GameInstance.GameIns.restaurantManager.miniGame.changed = true;
        GameInstance.GameIns.fishingManager.KillAllFishes();

       // yield return StartCoroutine(ChangeCleanQuality());
    }
    IEnumerator ChangeCleanQuality()
    {
        float f = 0;
        Color startShallow = material.GetColor("_ShallowColor");
        Color targetShallow = cleanShallowColor;
        Color startHorizon = material.GetColor("_HorizonColor");
        Color targetHorizon = cleanHorizonColor;
        while (f <= 5f)
        {
            Color shallowColor = Color.Lerp(startShallow, targetShallow, f / 5);
            Color horizonColor = Color.Lerp(startHorizon, targetHorizon, f / 5);
            material.SetColor("_ShallowColor", shallowColor);
            material.SetColor("_HorizonColor", horizonColor);
            f += Time.deltaTime;
            yield return null;
        }
        isDirty = false;
        GameInstance.GameIns.fishingManager.working = false;

        if(GameInstance.GameIns.app.currentScene == SceneState.Fishing) GameInstance.GameIns.uiManager.fishingStartButton.gameObject.SetActive(true);
    }
}
