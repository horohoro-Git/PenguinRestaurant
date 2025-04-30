using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//한글

public class AnimatedAnimal : MonoBehaviour
{
    public Animator animator;
    public Transform transforms;
    private void Awake()
    {
        transforms = transform; 
    }
    private void Start()
    {
       // animator.SetInteger("state", 0);
        //animator.SetInteger("state", 2);
        GetComponentInChildren<Renderer>().enabled = false;
    }
}
