using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Furniture : MonoBehaviour
{
    public int id { get; set; }

    [field: SerializeField]
    public WorkSpaceType SpaceType { get; set; }

    public GameObject model;
    public Transform transforms;
    public bool spawned;
    [NonSerialized] public Vector3 originPos;
    public Vector3 offsetPoint;

    public int rotateLevel;
    [NonSerialized] public bool canTouchable = true;
    [NonSerialized] public bool placed;

    bool isDragging;
    float dragTimer;
    Mouse currentMouse;
    public virtual void Start()
    {
        currentMouse = Mouse.current;
        if(!spawned)
        {
            spawned = true;
            if(SpaceType != WorkSpaceType.Door)  StartCoroutine(ScaleAnimation());
        }
    }

    public virtual void Update()
    {
        if (SpaceType == WorkSpaceType.Vending) return;
     
        if (isDragging)
        {
#if UNITY_ANDROID || UNITY_IOS
           
#else
            if (currentMouse.leftButton.wasReleasedThisFrame)
            {
                isDragging = false;
                return;
            }
#endif
            Vector3 screenPos;

#if UNITY_ANDROID || UNITY_IOS
            screenPos = Touchscreen.current.touches[0].position.ReadValue();
#else
            screenPos = Mouse.current.position.ReadValue();
            //screenPos = Mouse.current.position.ReadValue();
#endif
            Ray ray = InputManger.cachingCamera.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1 << 13))
            {
                dragTimer += Time.deltaTime;
                if(dragTimer > 0.3f)
                {
                    isDragging = false;
                    dragTimer = 0;
                    GameInstance.GameIns.applianceUIManager.Replace(this);

                }
            }
            else
            {
                isDragging = false;
            }
        }
        else
        {
            dragTimer = 0;
        }
    }

    public virtual void OnDisable()
    {
        isDragging = false;
        dragTimer = 0;
    }

    IEnumerator ScaleAnimation()
    {
        //yield return null;
        Vector3 origin = model.transform.localScale;
        float f = 0;
        Vector3 start = origin;
        Vector3 target1 = origin * 1.2f;
        Vector3 target2 = origin * 0.95f;
        while (f <= 0.1f)
        {
            Vector3 targetPos = Vector3.Lerp(start, target1, f / 0.1f);
            model.transform.localScale = targetPos;
            f += Time.unscaledDeltaTime;
            yield return null;
        }
        model.transform.localScale = target1;
        f = 0;
        while (f <= 0.15f)
        {
            Vector3 targetPos = Vector3.Lerp(target1, target2, f / 0.15f);
            model.transform.localScale = targetPos;
            f += Time.unscaledDeltaTime;
            yield return null;
        }
        model.transform.localScale = target2;

        f = 0;
        while (f <= 0.05f)
        {
            Vector3 targetPos = Vector3.Lerp(target2, start, f / 0.05f);
            model.transform.localScale = targetPos;
            f += Time.unscaledDeltaTime;
            yield return null;
        }
        model.transform.localScale = start;

    }
    public void DragStart()
    {
        isDragging = true;
    }
}
