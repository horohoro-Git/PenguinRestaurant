using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    public Transform transforms;

    private void Awake()
    {
        transforms = transform;
    }
}
