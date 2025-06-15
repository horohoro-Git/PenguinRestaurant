using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Emote : MonoBehaviour
{
    [NonSerialized] public Image image; 
    [NonSerialized] public RectTransform rectTransform; 
    [NonSerialized] public float height; 
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Update()
    {
        rectTransform.position = rectTransform.position + Vector3.up * Time.unscaledDeltaTime;
    }

    public void Emotion()
    {
        StartCoroutine(EmotionRoutine());
    }

    IEnumerator EmotionRoutine()
    {
        float f = 0;
        float origin = rectTransform.position.y;
        float target = rectTransform.position.y + height;

        Color imageColor = image.color;
        float startColor = 1;
        float endColor = 0;
        while (f <= 1f)
        {
            Vector3 pos = rectTransform.position;
            pos.y = Mathf.Lerp(origin, target, f);
            rectTransform.position = pos;

            imageColor.a = Mathf.Lerp(startColor, endColor, f);
            image.color = imageColor;

            f += Time.unscaledDeltaTime / 0.5f;
            yield return null;
        }

        GameInstance.GameIns.restaurantManager.ReturnEmote(this);
    }
}
