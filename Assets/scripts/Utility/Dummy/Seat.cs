using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public AnimalController customer;
    public Transform transforms;

    private void Awake()
    {
        transforms = transform;
    }
}
