using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollFullScreen : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public ScrollSnap snap;
    public ScrollRect scrollRect;

    
    void OnEnable()
    {
        StartCoroutine(ForceUpdate());
    }
    IEnumerator ForceUpdate()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        // scrollRect.horizontal = true;
        scrollRect.OnBeginDrag(eventData);
        snap.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
     
        scrollRect.OnDrag(eventData);
        snap.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);
        snap.OnEndDrag(eventData);
     //   scrollRect.horizontal = false;
    }
}
