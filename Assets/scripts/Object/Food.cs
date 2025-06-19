using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using UnityEngine;

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
   // [NonReorderable]
   // public float foodPrice = 0f;

    [NonSerialized] public BigInteger foodPrice;
    // Start is called before the first frame update
    void Awake()
    {
        transforms = transform;
        meshFilter = GetComponent<MeshFilter>();
    }
    /*public override int GetHashCode() => foodIndex.GetHashCode();

    public override bool Equals(object obj)
    {
        if (obj is not Food other) return false;
        return foodIndex == other.foodIndex;
    }*/

  /*  private void Update()
    {
        transform.position = targetCharacter.position + new Vector3(0, (index) * 0.7f, 0);
    *//*    for (int i = 0; i < foodStacks.Count; i++)
        {
            for (int j = 0; j < foodStacks[i].foodStack.Count; j++)
            {
                foodStacks[i].foodStack[j].transforms.position = headPoint.position + GameInstance.GetVector3(0, (j) * 0.7f, 0);
            }
        }
        for (int i = 0; i < garbageList.Count; i++)
        {
            garbageList[i].transforms.position = headPoint.position + GameInstance.GetVector3(0, 0.5f * i, 0);
        }*//*
        //   targetCharacter
    }*/
    
    public void Setup(Transform transform, int index)
    {
        targetCharacter = transform;
        transforms.SetParent(transform);
        this.index = index;
        transforms.position = transform.position + GameInstance.GetVector3(0, (index) * 0.7f, 0);
    }
    
    public void Release()
    {
        targetCharacter = null;
        transforms.SetParent(FoodManager.foodCollects.transform);
    }

    public void Resets()
    {
        meshFilter.mesh = null;
        parentType = MachineType.None;
        foodIndex = 0;
        foodPrice = 0;
    }
}
