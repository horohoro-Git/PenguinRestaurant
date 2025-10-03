using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HorizontalNestedScrollRect : ScrollRect
{
    private bool routeToParent = false;

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        base.OnInitializePotentialDrag(eventData);
        transform.parent.GetComponentInParent<ScrollRect>()?.OnInitializePotentialDrag(eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
        {
            routeToParent = false;
            base.OnBeginDrag(eventData);
        }
        else
        {
            routeToParent = true;
            transform.parent.GetComponentInParent<ScrollRect>()?.OnBeginDrag(eventData);
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (routeToParent)
            transform.parent.GetComponentInParent<ScrollRect>()?.OnDrag(eventData);
        else
            base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (routeToParent)
            transform.parent.GetComponentInParent<ScrollRect>()?.OnEndDrag(eventData);
        else
            base.OnEndDrag(eventData);

        routeToParent = false; 
    }
}
