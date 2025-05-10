using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODController : MonoBehaviour
{
    [SerializeField]
    List<SkinnedMeshRenderer> renderers = new List<SkinnedMeshRenderer>();


    public Animator animator;
  //  public Animator DummyAnimator { get { if(animator == null) animator = GetComponent<Animator>(); return animator; } }
    private void Start()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].enabled = false;
        }
    }
    public SkinnedMeshRenderer GetRenderer(int i)
    {
        return renderers[i - 1];
    }

    public void RendererActivate(bool isActive)
    {
        renderers[0].gameObject.SetActive(isActive);
        renderers[1].gameObject.SetActive(isActive);
    }

}
