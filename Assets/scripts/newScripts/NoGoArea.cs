using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoGoArea : MonoBehaviour
{
    public Vector3 moveVector;
    private void OnTriggerEnter(Collider other)
    {
        Animal animal = other.gameObject.GetComponentInParent<Animal>();
        if (animal != null)
        {
            animal.trans.position += moveVector;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Animal animal = collision.gameObject.GetComponentInParent<Animal>();
        if (animal != null)
        {
            animal.trans.position += moveVector;
        }
    }
}
