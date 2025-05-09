using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODController : MonoBehaviour
{
    [SerializeField]
    List<SkinnedMeshRenderer> renderers = new List<SkinnedMeshRenderer>();

    public LODGroup lodGroup;
  //  LODGroup LOD { get { if (lodGroup == null) lodGroup = GetComponent<LODGroup>(); return lodGroup; } }
    public Animator animator;
    public Animal animal;
   // Animator AnimationController { get { if (animator == null) animator = GetComponent<Animator>(); return animator; } }
    public SkinnedMeshRenderer GetRenderer(int i)
    {
        return renderers[i - 1];
    }

    public void RendererActivate(bool isActive)
    {
        renderers[0].gameObject.SetActive(isActive);
        renderers[1].gameObject.SetActive(isActive);
    }

    /*public void ForceApplyLOD(int i)
    {
        bool visible = renderers[0].enabled;
      //  Debug.Log
        lodGroup.ForceLOD(i);
        Debug.Log(visible);
       // animal.GetAnimIns(1).visible = !visible;
      //  animal.GetAnimIns(2).visible = !visible;
        *//*if (renderers[0].enabled) renderers[0].enabled = true;
        if (renderers[1].enabled) renderers[1].enabled = true;
        int j = animator.GetInteger("emotion");
        animator.SetInteger("emotion", j);*//*
    }*/
}
