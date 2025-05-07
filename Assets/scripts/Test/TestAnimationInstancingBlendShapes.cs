using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimIns = AnimationInstancing.AnimationInstancing;

public class TestAnimationInstancingBlendShapes : MonoBehaviour
{
    public SkinnedMeshRenderer meshRenderer;
    AnimIns animIns;
    public Animator animator;
    private void Start()
    {
        //Debug.Log(skinnedMeshRenderer.sharedMesh.GetInstanceID());


        Invoke("LateStart", 1);
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
           // animIns = GetComponentInChildren<AnimIns>();
            animIns.PlayAnim(0, "Walk");
            animator.SetInteger("state",1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
           // animIns = GetComponentInChildren<AnimIns>();
            animIns.PlayAnim(1, "Idle_A");
            animator.SetInteger("state",0);

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
          //  animIns = GetComponentInChildren<AnimIns>();
            animIns.gameObject.SetActive(false);
            meshRenderer.enabled = true;
            animator.Play("Eyes_Happy");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
          //  animIns = GetComponentInChildren<AnimIns>();
            animIns.gameObject.SetActive(true);
           // meshRenderer.enabled = false;
            animator.Play("Empty");
            StartCoroutine(VisibleInstancing());
        }
    }

    IEnumerator VisibleInstancing()
    {
        yield return null;
       // animIns.gameObject.SetActive(true);
        meshRenderer.enabled = false;
    }

    void LateStart()
    {
        animIns = GetComponentInChildren<AnimIns>();
        animIns.PlayAnim(1, "Idle_A");


    }
}
