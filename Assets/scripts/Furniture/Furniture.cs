using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    public int id { get; set; }

    [field: SerializeField]
    public WorkSpaceType spaceType { get; set; }

    public GameObject model;
    public bool spawned;
    [NonSerialized] public Vector3 originPos;
    public Vector3 offsetPoint;

    public int rotateLevel;
    [NonSerialized] public bool canTouchable = true;
    [NonSerialized] public bool placed;
    public virtual void Start()
    {
        if(!spawned)
        {
            SaveLoadSystem.SaveRestaurantBuildingData();
            spawned = true;
            StartCoroutine(ScaleAnimation());
        }
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
}
