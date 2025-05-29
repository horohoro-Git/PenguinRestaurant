using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelGage : MonoBehaviour
{
    public FoodMachine foodMachine;
    public Image background;
    public Image foreground;
    RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        background.sprite = AssetLoader.loadedAtlases["Town"].GetSprite("common_white_box");
        foreground.sprite = AssetLoader.loadedAtlases["Town"].GetSprite("common_white_box");
        foreground.type = Image.Type.Filled;
        foreground.fillMethod = Image.FillMethod.Horizontal;
        foreground.fillOrigin = 0;
    }

    private void Update()
    {
        if (foodMachine != null)
        {

            Vector3 offset = new Vector3(-5.5f, 10f, -5.5f);
            
            rectTransform.position = foodMachine.transform.position + offset;

            foreground.fillAmount -= 0.01f * Time.deltaTime;
        }
    }

}
