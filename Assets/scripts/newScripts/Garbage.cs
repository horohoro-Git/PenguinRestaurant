using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글

public class Garbage : MonoBehaviour
{
    public Transform transforms;

    int index = 0;

    public Transform targetCharacter { get; set; }
    private void Awake()
    {
        transforms = transform;
    }
 
    public void Setup(Transform transform, int index)
    {
        targetCharacter = transform;
        this.index = index;

        transforms.SetParent(transform);
        transforms.position = transform.position + GameInstance.GetVector3(0, (index) * 0.5f, 0);
    }

    public void Release()
    {
        targetCharacter = null;
        transforms.SetParent(GarbageManager.garbageCollects.transform);
    }
}
