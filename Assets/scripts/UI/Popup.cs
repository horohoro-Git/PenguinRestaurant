using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public RectTransform pRect;
   // public RectTransform parentRectTransform { get { if (pRect == null) pRect = GetComponentInParent<RectTransform>(); return pRect; } }
    Animator anim;
    public Animator animator { get { if(anim == null) anim = GetComponent<Animator>(); return anim; } }

    public void TriggerPopupClose()
    {
        GameInstance.GameIns.uiManager.audioSource.clip = GameInstance.GameIns.uISoundManager.UIClick();
        GameInstance.GameIns.uiManager.audioSource.Play();
        animator.SetTrigger(AnimationKeys.popup_close);
    }

    public void Close()
    {
        pRect.gameObject.SetActive(false);
    }
}
