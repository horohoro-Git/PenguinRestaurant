using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimIns = AnimationInstancing.AnimationInstancing;

public class TestAnimationInstancingBlendShapes : MonoBehaviour
{
    public Renderer meshRenderer;
    public Animal a;
    AnimIns animIns;
    public Animator animator;
    public LODController controller;
    private void Start()
    {
        //Debug.Log(skinnedMeshRenderer.sharedMesh.GetInstanceID());


        Invoke("LateStart", 1);
    }
    public void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Alpha1))
        {
         
            // animIns = GetComponentInChildren<AnimIns>();
            a.PlayAnimation("Walk");
            // animIns.PlayAnim(0, "Walk");
            animator.SetInteger("state",1);
        }
     //   if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // animIns = GetComponentInChildren<AnimIns>();
            a.PlayAnimation("Idle_A");
            animator.SetInteger("state",0);

        }
      //  if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //  animIns = GetComponentInChildren<AnimIns>();
          //  a.InstancingVisible(false);
            a.CheckVisible(false, true);
            // meshRenderer.enabled = true;
            controller.GetRenderer(1).enabled = true;
            controller.GetRenderer(2).enabled = true;
            animator.SetInteger("emotion", 1);
        }
      //  if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //  animIns = GetComponentInChildren<AnimIns>();
            //animIns.gameObject.SetActive(true);
            a.CheckVisible(true, true);

            animator.SetInteger("emotion", 0);
            StartCoroutine(VisibleInstancing());
        }
    }

    IEnumerator VisibleInstancing()
    {
        yield return null;
        controller.GetRenderer(1).enabled = false;
        controller.GetRenderer(2).enabled = false;
        // animIns.gameObject.SetActive(true);
        // meshRenderer.enabled = false;
    }

    void LateStart()
    {
        a.gameObject.SetActive(true);
        a.PlayAnimation("Idle_A");
        a.InstancingLOD((int)LODManager.lod_type);
      //  AnimIns animIns = a.GetAnimIns(1);
      //  animIns.PlayAnim(a.animationDic["Idle_A"], "Idle_A");
     //   animIns = GetComponentInChildren<AnimIns>();
       // animIns.PlayAnim(1, "Idle_A");


    }
}
