using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using CryingSnow.FastFoodRush;



#if HAS_ANIMATION_INSTANCING
using AnimationInstancing;
using AnimIns = AnimationInstancing.AnimationInstancing;
#endif

public class Animal : MonoBehaviour
{
    int id = 0;
    public int ID { get { if (id == 0) id = GetInstanceID(); return id; } }

    Transform tran;
    public Transform trans { get { if (tran == null) tran = transform; return tran; } }
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

#if HAS_ANIMATION_INSTANCING
    AnimIns animIns;
    public AnimIns InstancingCharacter { get { if (animIns == null) animIns = GetComponentInChildren<AnimIns>(); return animIns;} }
    //Dictionary<int, AnimIns> instancingLODs = new Dictionary<int, AnimIns>();
#endif
    // public AnimationClip clip;
    public Dictionary<string, int> animationDic = new Dictionary<string, int>();
    private void Start()
    {
        GetAnimationInstancing();
    }

    public void GetAnimationInstancing()
    {
        Instancing().Forget();
    }

    async UniTask Instancing()
    {
        //yield return GetWaitTimer.WaitTimer.GetTimer(100);
        await UniTask.Delay(100);

#if HAS_ANIMATION_INSTANCING
        AnimIns animIns = GetComponentInChildren<AnimIns>();
        int c = animIns.GetAnimationCount();
        for (int i = 0; i < c; i++)
        {
            animationDic[animIns.aniInfo[i].animationName] = i;
        }
     //   PlayAnimation("Idle_A");
        gameObject.SetActive(false);

#endif
    }
    public void PlayTriggerAnimation(string str)
    {
#if HAS_ANIMATION_INSTANCING
        InstancingCharacter.PlayTriggerAnim(animationDic[str]);
        //  GetAnimIns(2).PlayAnim(animationDic[str], str);
#endif
    }
    public void PlayAnimation(string str)
    {
#if HAS_ANIMATION_INSTANCING
        InstancingCharacter.PlayAnim(animationDic[str], str);
      //  GetAnimIns(2).PlayAnim(animationDic[str], str);
#endif
    }

    public void InstancingLOD(int lod)
    {
        InstancingCharacter.lodLevel = (int)LODManager.lod_type;
    }


    public void CheckVisible(bool isVisible, bool force=false)
    {
        if(force)
        {

            InstancingCharacter.visible = isVisible;
            InstancingCharacter.lodLevel = (int)LODManager.lod_type;
            return;
        }
       
      /*  if (lODController != null)
        {
            bool enabled = lODController.GetRenderer(1).enabled;
            if (enabled)
            {
                InstancingCharacter.visibity = false;
              //  GetAnimIns(2).visibity = false;
            }
            else
            {
                //   GetAnimIns(1).visibity = true;
                //   GetAnimIns(2).visibity = true;
              //  InstancingCharacter.
               *//* foreach (var v in instancingLODs)
                {
                    if (v.Key == (int)LODManager.lod_type)
                    {
                        Debug.LogWarning("BB");
                        v.Value.visibity = true;
                    }
                    else
                    {
                        Debug.LogWarning("AA");
                        v.Value.visibity = false;
                    }
                }*//*
               InstancingCharacter.lodLevel = (int)LODManager.lod_type - 1;
            }*/
        
    }

    IEnumerator enable()
    {
#if HAS_ANIMATION_INSTANCING
        GetComponentInChildren<AnimIns>().PlayAnimation(animationDic["Idle_A"]);
        float timer = 0;
        while (true)
        {
      //      if(GetComponentInChildren<AnimationInstancing.AnimationInstancing>().GetCurrentAnimationInfo() != null)
            GetComponentInChildren<AnimIns>().PlayAnimation(animationDic["Idle_A"]);
            timer = GetComponentInChildren<AnimIns>().aniInfo[animationDic["Idle_A"]].totalFrame;
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
