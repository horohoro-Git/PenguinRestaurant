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
        SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.UIPop(), 0.4f);
        animator.SetTrigger(AnimationKeys.popup_close);
    }

    public void Close()
    {
        pRect.gameObject.SetActive(false);
    }
}
