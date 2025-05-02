using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//한글

public class Panel : MonoBehaviour
{
    public RectTransform rectTransform;

    public Button thisButton;
    public float height;
    public float speed;
    public Image interactiveImage;

    bool spread = false;

    Coroutine currentCoroutine;

    public void Start()
    {
        thisButton.onClick.AddListener(() =>
        {
            Spread(spread);
        });
    }

    void Spread(bool bSpread)
    {
        if(bSpread)
        {
            spread = false;
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(GatheringRectTranform());
        }
        else
        {
            spread = true;
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(SpreadRectTranform());
        }
    }

    IEnumerator SpreadRectTranform()
    {
        RectTransform rectTransforms = rectTransform;
        Vector3 currentVec2 = rectTransform.localPosition;
        float elapsedTimer = 0;
        while (elapsedTimer / 0.3f <= 1)
        {
            elapsedTimer += Time.unscaledDeltaTime;
            float cal = Mathf.Lerp(currentVec2.y, 0, elapsedTimer / 0.3f);
           
            Vector3 v = new Vector3(currentVec2.x, cal, currentVec2.z);
            rectTransform.localPosition = v;

          
            yield return null;
        }
        interactiveImage.enabled = true;
    }

    IEnumerator GatheringRectTranform()
    {
        RectTransform rectTransforms = rectTransform;
        Vector2 currentVec = rectTransform.sizeDelta;
        Vector3 currentVec2 = rectTransform.localPosition;
        float elapsedTimer = 0;
        while (elapsedTimer / 0.3f <= 1)
        {
            elapsedTimer += Time.unscaledDeltaTime;
            float cal = Mathf.Lerp(currentVec2.y, 300, elapsedTimer / 0.3f);

            Vector3 v = new Vector3(currentVec2.x, cal, currentVec2.z);
            rectTransform.localPosition = v;


            yield return null;
        }
        interactiveImage.enabled = false;
    }
}
