using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public class AnimalStagePreview : MonoBehaviour
{
    Animator animator;
    Vector3 target;
    bool setup;
    NavMeshPath path;
    private void Awake()
    {
        animator = GetComponent<Animator>();
      
    }

    private void Start()
    {
      //  agent.enabled = true;
        
    }

    public void Setup()
    {
        StartCoroutine(DelayMove());
       // agent.enabled = true;
    }
    public void RandomMove()
    {
        target = transform.position + new Vector3(Random.Range(-6, 6), 0, Random.Range(-6, 6));

      //  agent.SetDestination(target);
    }
    IEnumerator DelayMove()
    {
        while(true)
        {
            target = transform.position + new Vector3(Random.Range(-6, 6), 0, Random.Range(-6, 6));
            path = new();
            bool exist = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            if(exist)
            {
                animator.SetInteger(AnimationKeys.state, 1);
                animator.speed = 0.8f;
                for (int i = 0; i < path.corners.Length; i++)
                {
                    Vector3 targetPos = path.corners[i];

                    while (Vector3.Distance(targetPos, transform.position) > 0.1f)
                    {
                       
                        Vector3 dir = (targetPos - transform.position).normalized;
                        transform.position += dir * 2.5f * Time.unscaledDeltaTime;
                        transform.rotation = Quaternion.LookRotation(dir);
                        yield return null;
                    }

                }
                animator.speed = 1;
                animator.SetInteger(AnimationKeys.state, 0);
            
                yield return null;
            }
            yield return new WaitForSecondsRealtime(Random.Range(0.3f, 1f));
        }
    }

}
