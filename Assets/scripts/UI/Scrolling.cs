using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class Scrolling : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    Animator animator;
    Canvas canvas;
    Canvas GetCanvas { get { if(canvas == null) canvas = GetComponentInParent<Canvas>(); return canvas; } }

    public RectTransform parentRect;
    public Image border;
    Vector2 SetParentVector { set { parentRect.anchoredPosition = value; 
            Color color = border.color;
            color.a = (Mathf.Abs(-270f - value.y) * .3313f * 0.0045f) * 0.87f;
            border.color = color;
            if (border.color.a == 0) border.raycastTarget = false;
            else border.raycastTarget = true;
        } }
    Vector2 latestVector = Vector3.zero;
    bool isSpread = false;
    Coroutine scrollCoroutine;
    bool isDown = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }
    void Start()
    {
    }
    private void OnEnable()
    {
      //  animator.enabled = true;
    //    animator.SetInteger(AnimationKeys.scrolling, 0);
    }
    private void OnDisable()
    {
       // animator.SetInteger(AnimationKeys.scrolling, 0);
        animator.enabled = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        //Vector2 pos = Input.mousePosition;
        Vector2 pos;

#if UNITY_ANDROID || UNITY_IOS
                pos = Touchscreen.current.touches[0].position.ReadValue();
#else
        pos = Mouse.current.position.ReadValue();
#endif

        float diff = pos.y - latestVector.y;
        diff /= GetCanvas.scaleFactor;
        Vector2 curPos = parentRect.anchoredPosition;
        pos.x = parentRect.anchoredPosition.x;
        if (diff > 0)
        {
       
            isDown = false;
            curPos.y = curPos.y + diff;
            if (curPos.y > 400)
            {
                curPos.y = 400;
            }
            else
            {
               animator.enabled = true;
               animator.SetInteger(AnimationKeys.scrolling, 1);
            }
            SetParentVector = curPos; 
        }
        else if(diff < 0)
        {
            isDown = true;
            curPos.y = curPos.y + diff;
            if (curPos.y < -270)
            {
                curPos.y = -270;
            }
            else
            {
                animator.enabled = true;
                animator.SetInteger(AnimationKeys.scrolling, 2);
            }
            SetParentVector = curPos;
        }
        else
        {
            isDown = false;
        }
       
        latestVector = pos;

    }

    IEnumerator Down()
    {
        Vector2 curPos = parentRect.anchoredPosition;
        Vector2 targetPos = new Vector2(curPos.x, -270);
        float f = (Mathf.Abs(-270 - curPos.y) / 460) * 0.2f;
        float multiply = 1 / f;
        float timer = 0;
        while (timer * multiply < 1)
        {
            SetParentVector = Vector2.Lerp(curPos, targetPos, timer * multiply);
            timer += Time.deltaTime;
            yield return null;
        }
        SetParentVector = targetPos;
        isSpread = false;
   //     animator.SetTrigger("gathering");
    }

    IEnumerator Up()
    {
        Vector2 curPos = parentRect.anchoredPosition;
        Vector2 targetPos = new Vector2(curPos.x, 400);
        float f = (Mathf.Abs(400 - curPos.y) / 460) * 0.2f;
        float multiply = 1 / f;
        float timer = 0;
        while (timer * multiply < 1)
        {
            SetParentVector = Vector2.Lerp(curPos, targetPos, timer * multiply);
            timer += Time.deltaTime;
            yield return null;
        }
        SetParentVector = targetPos;
        isSpread = true;
      //  animator.SetTrigger("spread");
    }



    public void OnPointerDown(PointerEventData eventData)
    {

        Vector3 pos;// = Input.mousePosition;
#if UNITY_ANDROID || UNITY_IOS
                pos = Touchscreen.current.touches[0].position.ReadValue();
#else
        pos = Mouse.current.position.ReadValue();
#endif
        latestVector = pos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      
        if (parentRect.anchoredPosition.y == 400)
        {
          //  animator.SetTrigger("spread");
            isSpread = true;
            return;
        }
        if (parentRect.anchoredPosition.y == -270)
        {
       //     animator.SetTrigger("gathering");
            isSpread = false;
            return;
        }
        if (scrollCoroutine != null) StopCoroutine(scrollCoroutine);
        if (isDown)
        {
            scrollCoroutine = StartCoroutine(Down());
            return;
        }
        else
        {
            scrollCoroutine = StartCoroutine(Up());
            return;
        }
      
    }

    public void Shut()
    {
        if (!isSpread && isDown) return; 
        if (scrollCoroutine != null) StopCoroutine(scrollCoroutine);
      
        scrollCoroutine = StartCoroutine(Down());
        animator.enabled = true;
        animator.SetInteger(AnimationKeys.scrolling, 2);
        isSpread = false;
        isDown = true;
    }
}
