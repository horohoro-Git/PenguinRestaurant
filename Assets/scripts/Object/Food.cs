using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
public class Food : MonoBehaviour
{
    [NonReorderable]
    public Transform transforms;

    public MeshFilter meshFilter;
    public MachineType parentType;
    int index = 0;

    public Transform targetCharacter {  get; set; }

    [NonReorderable]
    public int foodIndex;
    [NonReorderable]
    public bool withFuel;
   
    [NonSerialized] public BigInteger foodPrice;
    void Awake()
    {
        transforms = transform;
        meshFilter = GetComponent<MeshFilter>();
    }
   
    public void Setup(Transform transform, int index)
    {
        targetCharacter = transform;
        transforms.SetParent(transform);
        this.index = index;
        transforms.position = transform.position + new Vector3(0, (index) * 0.7f, 0);
    }
    
    public void Release()
    {
        targetCharacter = null;
        transforms.SetParent(WorkSpaceManager.foodCollects.transform);
    }

    public void Resets()
    {
        meshFilter.mesh = null;
        parentType = MachineType.None;
        foodIndex = 0;
        foodPrice = 0;
    }
}
