using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class BlockRaycast : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, ICanvasRaycastFilter
{
    public ScrollRect rect;
    TMP_Text text;
    void Awake()
    {
        text = GetComponent<TMP_Text>();
      //  text.raycastTarget = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, eventData.pressEventCamera);
        if (linkIndex == 0)
        {
            rect.enabled = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rect.enabled = true;
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, sp, eventCamera);
        // ��ũ�� �ִ� ��ġ�� true �� Ŭ���� ����
        // ������ ������ false �� ScrollRect�� �̺�Ʈ ó��
        return linkIndex != -1;
    }
}
