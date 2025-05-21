using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueuePoint : MonoBehaviour
{
    public AnimalController controller;
    Transform Transforms;
    public Transform transforms { get { if (Transforms == null) Transforms = transform; return Transforms; } }
}