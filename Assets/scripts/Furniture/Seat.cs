using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public AnimalController animal { get; set; }
    public Transform transforms;

    [NonSerialized] public bool isDisEnabled;
    private void Awake()
    {
        transforms = transform;
    }
}
