using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using static GameInstance;

public class BlackConsumer : AnimalController
{
    //AudioSource audioSource;

    bool bRunaway;
    // public AnimalStruct animalStruct { get; set; }

    public BlackConsumerState state;
    public Action<BlackConsumer> consumerCallback;
    Table targetTable;
    public void CauseTrouble()
    {
        WorkSpaceManager workSpaceManager = GameIns.workSpaceManager;
        switch(state)
        {
            case BlackConsumerState.None:
                break;
            case BlackConsumerState.FindingTarget:
                List<Table> tables = workSpaceManager.tables;
                tables = tables.OrderBy(t => t.transform.position - trans.position).ToList();
                for (int i = 0; i < tables.Count; i++)
                {
                    if (tables[i].foodStacks.Count > 0)
                    {
                        targetTable = tables[i];
                        FoundTheTarget();
                        return;
                    }
                }
                Patrol();
                break;
            case BlackConsumerState.FoundTarget:
                break;
            case BlackConsumerState.MoveToTarget:
                break;
            case BlackConsumerState.Steal:
                StealFood();
                break;
            case BlackConsumerState.Kidding:
                break;
            case BlackConsumerState.SubDue:
                break;
        }
      
    }
   
    void Patrol()
    {
        Patrolling(App.GlobalToken).Forget();
    }

    void FoundTheTarget()
    {
     //   state = BlackConsumerState.FoundTarget;
        targetTable.hasProblem = true;



    }


    void StealFood()
    {
        BlackConsumer_StealFood(App.GlobalToken).Forget();
    }

    void RunAway()
    {
        bRunaway = false;
    }


    async UniTask Patrolling(CancellationToken cancellationToken = default)
    {
        try
        {
            while (true)
            {
                Vector3 target;
                float x = UnityEngine.Random.Range(-1f, 1f);
                float y = UnityEngine.Random.Range(-1f, 1f);
                Vector3 v3 = GameInstance.GetVector3(x, 0, y);
                float magnitude = v3.magnitude;
                if (magnitude > 0f)
                {
                    v3 = v3 / magnitude;
                }
                float speed = UnityEngine.Random.Range(animalStruct.speed, animalStruct.speed * 2);
                if (trans == null || !trans)
                {
                    await UniTask.NextFrame();
                    return;
                }
                target = trans.position + v3 * speed;

                bool interruptCheck = Physics.CheckBox(target, GameInstance.GetVector3(0.6f, 0.6f, 0.6f), Quaternion.identity, 1 << 6 | 1 << 7);
                bool validCheck = Physics.CheckBox(target, GameInstance.GetVector3(0.6f, 0.6f, 0.6f), Quaternion.identity, 1);

                if (validCheck && !interruptCheck)
                {
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                    Stack<Vector3> moveTargets = await CalculateNodes_Async(target, true, cancellationToken);
                    await UniTask.SwitchToMainThread(cancellationToken: cancellationToken);
                    if (moveTargets != null && moveTargets.Count > 0)
                    {
                        Vector3 test = moveTargets.Peek();
                        if (test.x == 100 && test.z == 100)
                        {
                            animator.SetInteger("state", 2);
                         //   animal.PlayAnimation(AnimationKeys.Idle);
                        }
                        else
                        /* if (moveNode.c == 100 && moveNode.r == 100)
                         {
                             moveNode = null;
                             animator.SetInteger("state", 2);
                             animal.PlayAnimation(AnimationKeys.Idle);
                            // PlayAnim(animal.animationDic[animation_LookAround], animation_LookAround);
                         }
                         else*/
                        {
                            await BlackConsumer_Move(moveTargets, target, cancellationToken);
                            if (reCalculate)
                            {
                                while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                                reCalculate = false;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        animator.SetInteger("state", 2);
                      //  animal.PlayAnimation(AnimationKeys.Idle);
                        //PlayAnim(animal.animationDic[animation_LookAround], animation_LookAround);
                    }
                    await UniTask.Delay(500, cancellationToken: cancellationToken);
                    return;
                }
                //    return;
            }
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Employee_Patrol task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Employee_Patrol: {ex.Message}");
            throw;
        }
    }

    async UniTask BlackConsumer_Table(CancellationToken cancellationToken = default)
    {
        try
        {
            while (true)
            {
              //  while (pause) await UniTask.Delay(100, cancellationToken: cancellationToken);
                Stack<Vector3> moveTargets = await CalculateNodes_Async(targetTable.cleanSeat.position, true, cancellationToken);
                if (moveTargets != null && moveTargets.Count > 0)
                {
                    Vector3 test = moveTargets.Peek();
                    if (test.z == 100 && test.x == 100)
                    {
                        // moveNode = null;
                    }
                    else
                    {
                        await BlackConsumer_Move(moveTargets, targetTable.cleanSeat.position, cancellationToken);

                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }

                        modelTrans.LookAt(targetTable.transforms);

                    }

                    await UniTask.Delay(200, cancellationToken: cancellationToken);

                    state = BlackConsumerState.Steal;
                    consumerCallback?.Invoke(this);
                }
                else
                {
                    await UniTask.Delay(200, cancellationToken: cancellationToken);
                    consumerCallback?.Invoke(this);
                }
                return;
          //  Escape: continue;
            }
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Employee_Trashcan task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Employee_Trashcan: {ex.Message}");
            throw;
        }
    }


    async UniTask BlackConsumer_StealFood(CancellationToken cancellationToken = default)
    {
     //   targetTable
    }

    async UniTask BlackConsumer_Move(Stack<Vector3> n, Vector3 loc, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            /* Node a = n.parentNode == null ? n : n.parentNode;
             Stack<Node> stack = new Stack<Node>();
             while (a != null)
             {
                 stack.Push(a);
                 a = a.parentNode;
                 await UniTask.NextFrame();
             }*/
            n.Pop();
            await UniTask.NextFrame(cancellationToken: cancellationToken);
            while (n.Count > 0)
            {
                if (trans == null || !trans)
                {
                    await UniTask.NextFrame();
                    return;
                }
                Vector3 target = n.Pop();
                /*   float r = GameInstance.GameIns.calculatorScale.minY + node.r * GameInstance.GameIns.calculatorScale.distanceSize;
                   float c = GameInstance.GameIns.calculatorScale.minX + node.c * GameInstance.GameIns.calculatorScale.distanceSize;
                   Vector3 target = new Vector3(c, 0, r);*/
                float cur = (target - trans.position).magnitude;

                while (true)
                {
                   // while (pause) await UniTask.Delay(100, cancellationToken: cancellationToken);
                    if (reCalculate)
                    {
                        Debug.Log("Recalculate");
                        return;
                    }

                    if (trans == null || !trans)
                    {
                        await UniTask.NextFrame();
                        return;
                    }


                    if (Vector3.Distance(trans.position, target) <= 0.01f) break;
                    cancellationToken.ThrowIfCancellationRequested();
                    animator.SetInteger("state", 1);
                   // animal.PlayAnimation(AnimationKeys.Walk);
                    // PlayAnim(animal.animationDic[animation_Run], animation_Run);
                    cur = (target - trans.position).magnitude;
                    Vector3 dir = (target - trans.position).normalized;
                    trans.position = Vector3.MoveTowards(trans.position, target, animalStruct.speed * Time.deltaTime);
                    float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                    modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                    // animatedAnimal.transforms.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                    await UniTask.NextFrame();
                }
            }

            Vector3 newLoc = loc;
            newLoc.y = 0;
            while (true)
            {
             //   while (pause) await UniTask.Delay(100, cancellationToken: cancellationToken);
                if (reCalculate)
                {
                    Debug.Log("Recalculate");
                    return;
                }
                animator.SetInteger("state", 1);
               // animal.PlayAnimation(AnimationKeys.Walk);
                trans.position = Vector3.MoveTowards(trans.position, newLoc, animalStruct.speed * Time.deltaTime);
                if (Vector3.Distance(trans.position, newLoc) <= 0.01f) break;
                Vector3 dir = newLoc - trans.position;
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }


            animator.SetInteger("state", 0);
          //  animal.PlayAnimation(AnimationKeys.Idle);
            //  PlayAnim(animal.animationDic[animation_Idle_A], animation_Idle_A);
        }
        catch (OperationCanceledException)
        {
            // 작업이 취소되었을 때의 정리 코드
            Debug.Log("Employee_Move task was cancelled");
            throw; // 필요에 따라 rethrow 또는 생략
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in Employee_Move: {ex.Message}");
            throw;
        }
    }

    public void DoTroll()
    {
        WorkSpaceManager workSpace = GameIns.workSpaceManager;

        

    }
}
