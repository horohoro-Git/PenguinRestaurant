using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHighlightController : MonoBehaviour, IPointerDownHandler
{
    Animator animator;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }


    public void Highlight(int state)
    {
        animator.SetInteger(AnimationKeys.state, state);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Highlight(0);
    }
}
