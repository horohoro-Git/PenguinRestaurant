using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
#if HAS_ANIMATION_INSTANCING
using AnimationInstancing;
#endif

public class Animal : MonoBehaviour
{
  

    public Transform trans;
    public Transform modelTrans;

    [HideInInspector]
    public float speed;
    [HideInInspector]
    public float eatSpeed;
    [HideInInspector]
    public int minOrder;
    [HideInInspector]
    public int maxOrder;
    [HideInInspector]
    public int likeFood;
    [HideInInspector]
    public int hateFood;
    //  Animator animator;

   // public AnimationClip clip;
    public Dictionary<string, int> animationDic = new Dictionary<string, int>();
    private void Start()
    {
        //StartCoroutine(enable());
        // animator = GetComponent<Animator>();

        //  if(gameObject.activeSelf)Invoke("enables", 0.1f);
        // GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        GetAnimationInstancing();
    }

    public void GetAnimationInstancing()
    {
        StartCoroutine(Instancing());
       
    }

    IEnumerator Instancing()
    {
        yield return GetWaitTimer.WaitTimer.GetTimer(100); //new WaitForSeconds(0.1f);
                                                           //  GetComponentInChildren<AnimationInstancing.AnimationInstancing>().Mode = 
                                                           //   GetComponentInChildren<AnimationInstancing.AnimationInstancing>().;
#if HAS_ANIMATION_INSTANCING
        for (int i = 0; i < GetComponentInChildren<AnimationInstancing.AnimationInstancing>().GetAnimationCount(); i++)
        {
            animationDic[GetComponentInChildren<AnimationInstancing.AnimationInstancing>().aniInfo[i].animationName] = i;
        }
        gameObject.SetActive(false);
        //GetComponentInChildren<AnimationInstancing.AnimationInstancing>().activePlease = false;
        // GetComponentInChildren<AnimationInstancing.AnimationInstancing>().PlayAnimation(animationDic["Idle_A"]);
        // GetComponentInChildren<AnimationInstancing.AnimationInstancing>().GetPreAnimationInfo
#endif
    }
    IEnumerator enable()
    {
#if HAS_ANIMATION_INSTANCING
        GetComponentInChildren<AnimationInstancing.AnimationInstancing>().PlayAnimation(animationDic["Idle_A"]);
        float timer = 0;
        while (true)
        {
      //      if(GetComponentInChildren<AnimationInstancing.AnimationInstancing>().GetCurrentAnimationInfo() != null)
            GetComponentInChildren<AnimationInstancing.AnimationInstancing>().PlayAnimation(animationDic["Idle_A"]);
            timer = GetComponentInChildren<AnimationInstancing.AnimationInstancing>().aniInfo[animationDic["Idle_A"]].totalFrame;
            for (int i = 0; i < timer; i++)
            {
                yield return null;
            }

         
        }
#else
        yield return null;
#endif
    }
    private void enables()
    {
#if HAS_ANIMATION_INSTANCING
        for (int i = 0; i < GetComponentInChildren<AnimationInstancing.AnimationInstancing>().GetAnimationCount(); i++)
        {
            Debug.Log(GetComponentInChildren<AnimationInstancing.AnimationInstancing>().aniInfo[i].animationName);
            animationDic[GetComponentInChildren<AnimationInstancing.AnimationInstancing>().aniInfo[i].animationName] = i;
        }
        StartCoroutine(enable());
#endif
    }
    /*  public enum AnimalState
      {
          Standing,
          Counter,
          Taking,
          Walking

      }

      public Action<WorkSpace> routineAction;
      public GameObject[] target;
      public Animator animator;
      public Transform head;
      public AnimalState state = AnimalState.Standing;
      public List<Food> foodList = new List<Food>();

      void Update()
      {
          for (int i = 0; i < foodList.Count; i++)
          {
              foodList[i].transform.position = new Vector3(head.position.x, head.position.y + i * 0.7f, head.position.z);
          }
      }

      public IEnumerator Move(Coord target, float minY, float minX, float distanceScale, WorkSpace type)
      {
          animator.SetInteger("state", 1);

          Coord cor = target;
          Stack<Coord> stack = new Stack<Coord>();
          stack.Push(cor);
          while (cor.parent != null)
          {
              cor = cor.parent;
              stack.Push(cor);
          }
          stack.Pop();
          while (stack.Count > 0)
          {
              Coord node = stack.Pop();

              float r = minY + node.r * distanceScale;
              float c = minX + node.c * distanceScale;
              Vector3 targetNode = new Vector3(c, 0, r);
              Vector3 dir = targetNode - transform.position;

              while (true)
              {
                  float magnitude = (targetNode - transform.position).magnitude;
                  if (magnitude <= 0.1f)
                  {
                      transform.position = targetNode;
                      break;
                  }

                  float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                  transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                  transform.Translate(dir.normalized * Time.deltaTime * 3f, Space.World);


                  yield return null;
              }
          }
          if (state != AnimalState.Counter)
          {
              if (type.workType == WorkSpace.WorkType.Counter)
              {
                  type.employee = this;
                  this.state = AnimalState.Standing;
                  if (type.customer) state = AnimalState.Counter;
                  StopCoroutine(type.GiveFoods(this));
                  StartCoroutine(type.GiveFoods(this));
              }
              else
              {
                  this.state = AnimalState.Taking;
                  StopCoroutine(type.GetFoods(this));
                  StartCoroutine(type.GetFoods(this));
              }
          }

          animator.SetInteger("state", 0);
      }*/
}
