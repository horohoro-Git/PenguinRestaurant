using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글

public class WorkingSpot : MonoBehaviour
{
    public MachineType type;
    public Transform transforms;

    private void Awake()
    {
        transforms = transform;
    }
}
