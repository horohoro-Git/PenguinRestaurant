using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullFilledTrashcan : MonoBehaviour
{
    Color normal = new Color(1 / 255f * 150, 1 / 255f * 150, 1 / 255f * 150, 1);
    Color invisible = new Color(0, 0, 0, 0);
    Color visible = new Color(1, 1, 1, 1);
    Color highlight = new Color(1, 1, 0, 1);

    Coroutine highlightCoroutine;
    [NonSerialized] public Image uiImage;
    [NonSerialized] public Outline uiOutline;
    private void Awake()
    {
        uiImage = GetComponent<Image>();
        uiOutline = GetComponent<Outline>();
    }
    public void ChangeHighlight(bool on)
    {
        if(highlightCoroutine != null) StopCoroutine(highlightCoroutine);
        if (on)
        {
            uiImage.color = visible;
            StartCoroutine(ShowHighlight());
        }
        else
        {
            uiImage.color = normal;
            uiOutline.effectColor = invisible;
        }
    }

    IEnumerator ShowHighlight()
    {
        while(true)
        {

            float f = 0;
            Color start = uiOutline.effectColor;
            Color target = highlight;
            while (f <= 1)
            {
                uiOutline.effectColor = Color.Lerp(start, target, f);
                f += Time.unscaledDeltaTime / 0.5f;
                yield return null;
            }

            f = 0;
            start = uiOutline.effectColor;
            target = visible;
            while (f <= 1)
            {
                uiOutline.effectColor = Color.Lerp(start, target, f);
                f += Time.unscaledDeltaTime / 0.5f;
                yield return null;
            }

            yield return null;
        }
        
    }
}
