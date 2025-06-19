using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingCost : MonoBehaviour
{
    [NonSerialized] public RectTransform rectTransform;
    [NonSerialized] public float height;
    [NonSerialized] public TMP_Text text;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        text = GetComponent<TMP_Text>();
        text.fontSize = 55;
    }
   
    public void Floating()
    {
       // StartCoroutine(FloatingHeight());
        StartCoroutine(FloatingRoutine());
    }

    IEnumerator FloatingHeight()
    {
        while (true)
        {
            rectTransform.position = rectTransform.position + Vector3.up * Time.unscaledDeltaTime;
            yield return null;
        }
    }
    IEnumerator FloatingRoutine()
    {
        float f = 0;
        float origin = rectTransform.position.y;
        float target = rectTransform.position.y + height;


        Color imageColor = text.color;
        imageColor.a = 1;
        text.color = imageColor;
        float startColor = 1;
        float endColor = 0;
        while (f <= 1f)
        {
            Vector3 pos = rectTransform.position;
            pos.y = Mathf.Lerp(origin, target, f);
            rectTransform.position = pos;

            if (f >= 0.5f)
            {
                imageColor.a = Mathf.Lerp(startColor, endColor, (f - 0.5f) * 2);
                text.color = imageColor;
            }
            f += Time.unscaledDeltaTime;
            yield return null;
        }

        GameInstance.GameIns.restaurantManager.ReturnFloatingCost(this);
    }
}
