using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public AnimalController animal { get; set; }
    public Transform transforms;

    private void Awake()
    {
        transforms = transform;
    }
}
