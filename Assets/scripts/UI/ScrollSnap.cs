using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollSnap : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler 
{
    public RectTransform selectTranform;
    public ScrollRect scrollRect;
    public RectTransform viewport;
    public RectTransform content;
    public ContentSizeFitter fitter;
    bool setup = false;
    public List<RectTransform> panelList = new();
    int panelIndex = 0;
    bool isSnapping;
    bool isDragging;
    int selectedPanel = 0;
    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }
    public void OnDrag(PointerEventData eventData)
    {

    }

    public void Setup(int index)
    {
        selectedPanel = index;

        content.anchoredPosition = new Vector2(-550 * (selectedPanel), content.anchoredPosition.y);
        selectTranform.gameObject.SetActive(true);

        GameInstance.GameIns.worldUI.SnapToTarget(selectTranform, selectedPanel);
        setup = true;
    }

    void Update()
    {
        if (setup && !isDragging && isSnapping) 
        {
            float[] positions = new float[content.childCount];

            RectTransform target = content.GetChild(selectedPanel) as RectTransform;

            Vector2 targetPos = target.anchoredPosition;

            float viewportHalfWidth = selectTranform.rect.width * 0.5f;
            float itemHalfWidth = target.rect.width * 0.5f;

            float newX = -(targetPos.x) - viewportHalfWidth + itemHalfWidth;
          
            Vector2 newPos = new Vector2(newX, content.anchoredPosition.y);
        
            // 목표 위치에 거의 도달하면 정지
            if (Vector2.Distance(content.anchoredPosition, new Vector2(-550 * (selectedPanel), content.anchoredPosition.y)) < 5f)
            {
                content.anchoredPosition = new Vector2(-550 * (selectedPanel), content.anchoredPosition.y); //newPos + new Vector2(540, 0f);

                selectTranform.gameObject.SetActive(true);
                 
                GameInstance.GameIns.worldUI.SnapToTarget(selectTranform, selectedPanel);
                isSnapping = false;
            }
            else
            {
                content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, new Vector2(-550 * (selectedPanel), content.anchoredPosition.y), Time.unscaledDeltaTime * 10); //newPos + new Vector2(540, 0f), Time.unscaledDeltaTime * 10);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        selectTranform.gameObject.SetActive(false);
       // GameInstance.GameIns.worldUI.BeginDrag();
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      //  GameInstance.GameIns.worldUI.EndDrag();
        float[] distances = new float[panelList.Count];
        for(int i = 0; i < distances.Length ;i++)
        {
            distances[i] = ((panelList[i].anchoredPosition) + content.anchoredPosition - new Vector2(viewport.rect.width * (0.5f - content.anchorMin.x), viewport.rect.height * (0.5f - content.anchorMin.y))).magnitude;
        }
        float min = Mathf.Min(distances);
        for (int i = 0; i < distances.Length; i++)
        {
            if (distances[i] == min)
            {
                selectedPanel = i;
                break;
            }
        }

        isDragging = false;
        isSnapping = true;
        
    }
}
