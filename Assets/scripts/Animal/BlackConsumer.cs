using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using static GameInstance;
using static MoveCalculator;
using static UnityEngine.Rendering.DebugUI;
public class BlackConsumer : AnimalController
{
    public int ID { get { if (id == 0) id = GetInstanceID(); return id; } }
    bool bRunaway;
    public LODGroup lodGroup;

    public BlackConsumerState state;
    public Action<BlackConsumer> consumerCallback;
 //   Animator animator;

    Table targetTable;
    int seatIndex = 0;
    int preIndex = -1;
    [NonSerialized] public bool bDead = false;
    [NonSerialized] public Transform spawnerTrans;
    CancellationTokenSource cancellationTokenSource;
    public ParticleSystem particles;
    public AudioSource hitAudio;
    [NonSerialized] public EnemySpawner enemySpawner;
    public void CauseTrouble()
    {
        WorkSpaceManager workSpaceManager = GameIns.workSpaceManager;
        switch(state)
        {
            case BlackConsumerState.None:
                break;
            case BlackConsumerState.Spawn:
                Spawn();
                break;
            case BlackConsumerState.FindingTarget:
                List<Table> tables = workSpaceManager.tables;
               // tables = tables.OrderBy(t => t.transform.position - trans.position).ToList();
                tables = tables.OrderBy(t => Vector3.Distance(t.transform.position, trans.position)).ToList();
                for (int i = 0; i < tables.Count; i++)
                {
                    int seatFoodNum = 0;
                    for(int j=0; j < tables[i].seats.Length; j++)
                    {
                        if (tables[i].placedFoods[j] != null) seatFoodNum++;
                    }

                    if ((seatFoodNum > 0 || tables[i].foodStacks[0].foodStack.Count > 0) && tables[i].placed)
                    {
                        int targetIndex = 0;
                        int existCustomer = 0;
                        Table t = null;
                        for (int j = 0; j < tables[i].seats.Length; j++)
                        {
                           
                            if(tables[i].seats[j].animal != null && !tables[i].seats[j].isDisEnabled) existCustomer++;

                            if (t == null && tables[i].seats[j].animal == null && !tables[i].seats[j].isDisEnabled)
                            {
                                t = tables[i];
                                targetIndex = j;
                              
                            }
                        }
                        if (t != null && existCustomer == 1)
                        {
                            targetTable = t;
                            FoundTheTarget(targetIndex);
                            return;
                        }
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
         
            case BlackConsumerState.SubDue:
                RunAway();
                break;
        }
       
    }
   
    void Wait()
    {
        BlackConsumer_Wait(App.GlobalToken).Forget();
    }

    async UniTask BlackConsumer_Wait(CancellationToken cancellationToken)
    {
        await UniTask.Delay(500, cancellationToken: cancellationToken);
        consumerCallback?.Invoke(this);
    }
    void Patrol()
    {
        if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        Patrolling(cancellationTokenSource.Token).Forget();
    }

    void Spawn()
    {
        if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        Enter(cancellationTokenSource.Token).Forget();
    }

    async UniTask Enter(CancellationToken cancellationToken = default)
    {
        try
        {
            int x = 0;
            int y = 0;
            float size = GameIns.calculatorScale.distanceSize;
            float minX = GameIns.calculatorScale.minX;
            float minY = GameIns.calculatorScale.minY;
            //  MoveCalculator.GetBlocks[]
            int resX = 0;
            int resY = 0;
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    int xx = x + i;
                    int yy = y + j;
                    int calculatedX = Mathf.FloorToInt((xx - minX) / size);
                    int calculatedY = Mathf.FloorToInt((yy - minY) / size);

                    if (Utility.ValidCheck(calculatedY, calculatedX) && GetBlocks[GetIndex(calculatedX, calculatedY)]) continue;
                    resX = xx;
                    resY = yy;
                    goto Found;
                }
            }

        Found:
            Vector3 target = new Vector3(resX, 0, resY);

            while (true)
            {
                Stack<Vector3> moveTargets = await CalculateNodes_Async(target, false, cancellationToken);

                await BlackConsumer_Move(moveTargets, target, cancellationToken);

                if (reCalculate)
                {
                    while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                    reCalculate = false;
                    continue;
                }

                await UniTask.Delay(500, cancellationToken: cancellationToken);
                state = BlackConsumerState.FindingTarget;
                consumerCallback?.Invoke(this);


                return;
            }



        }
        catch
        {

        }
    }

    void FoundTheTarget(int index)
    {
     //   state = BlackConsumerState.FoundTarget;
       
        if (index >= 0)
        {
            // targetTable.hasProblem = true;
            targetTable.tableState = TableState.HasTrouble;
            seatIndex = index;
            targetTable.seats[index].animal = this;
            Vector3 target = targetTable.seats[index].transform.position;
            if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            BlackConsumer_Table(cancellationTokenSource.Token).Forget();
        }
    }


    void StealFood()
    {
        BlackConsumer_StealFood(cancellationTokenSource.Token).Forget();
    }

    void RunAway()
    {
        if (cancellationTokenSource != null) cancellationTokenSource.Cancel();
       
        cancellationTokenSource = new CancellationTokenSource();
        BlackConsumer_GoHome(cancellationTokenSource.Token).Forget();
    }


    async UniTask Patrolling(CancellationToken cancellationToken = default)
    {
        try
        {
            Vector3 target = Vector3.zero;
            float minX = GameIns.calculatorScale.minX;
            float minY = GameIns.calculatorScale.minY;
            float size = GameIns.calculatorScale.distanceSize;
            while (true)
            {
                float x = UnityEngine.Random.Range(-1f, 1f);
                float y = UnityEngine.Random.Range(-1f, 1f);
                Vector3 v3 = new Vector3(x, 0, y);

                float magnitude = v3.magnitude;
                if (magnitude > 0f)
                {
                    v3 = v3 / magnitude;
                }
                float speed = UnityEngine.Random.Range(animalStruct.speed, animalStruct.speed * 2);

                target = trans.position + v3 * speed;

                int xx = Mathf.FloorToInt((target.x - minX) / size);
                int yy = Mathf.FloorToInt((target.z - minY) / size);

                if(!GetBlockEmployee[GetIndex(xx,yy)])
                {
                    goto MoveToTarget;
                }
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }
            MoveToTarget:
            while (true)
            {
                if (trans == null || !trans)
                {
                    await UniTask.NextFrame();
                    return;
                }
               

     //           bool interruptCheck = Physics.CheckBox(target, GameInstance.GetVector3(0.6f, 0.6f, 0.6f), Quaternion.identity, 1 << 6 | 1 << 7);
      //          bool validCheck = Physics.CheckBox(target, GameInstance.GetVector3(0.6f, 0.6f, 0.6f), Quaternion.identity, 1);

            //    if (validCheck && !interruptCheck)
                {
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                    Stack<Vector3> moveTargets = await CalculateNodes_Async(target, true, cancellationToken);
                    await UniTask.SwitchToMainThread(cancellationToken: cancellationToken);
                    if (moveTargets != null && moveTargets.Count > 0)
                    {
                        Vector3 test = moveTargets.Peek();
                        if (test.x == 100 && test.z == 100)
                        {
                            animator.SetInteger("state", 0);
                            Wait();
                            return;
                         //   animal.PlayAnimation(AnimationKeys.Idle);
                        }
                        else
                        {
                            await BlackConsumer_Move(moveTargets, target, cancellationToken);
                            if (reCalculate)
                            {
                                while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                                reCalculate = false;
                                continue;
                            }
                            animator.SetInteger("state", 0);
                            await UniTask.Delay(500, cancellationToken: cancellationToken);
                            consumerCallback?.Invoke(this);
                            return;
                        }
                    }
                    else
                    {
                        animator.SetInteger("state", 0);
                        Wait();
                        return;
                        //  animal.PlayAnimation(AnimationKeys.Idle);
                        //PlayAnim(animal.animationDic[animation_LookAround], animation_LookAround);
                    }
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
            Debug.Log($"Error in Employee_Patrol: {ex.Message}");
            throw;
        }
    }

    async UniTask BlackConsumer_Table(CancellationToken cancellationToken = default)
    {
        try
        {
            targetTable.animals.Add(this);
            while (true)
            {
                reCalculate = false;
                if(!targetTable.placed || targetTable.seats[seatIndex].isDisEnabled)
                {
                    await UniTask.Delay(200, cancellationToken: cancellationToken);
                    if (targetTable != null)
                    {
                        targetTable.stealing = false;
                        targetTable.hasProblem = false;
                        targetTable.stolen = false;
                        targetTable.seats[seatIndex].animal = null;
                        targetTable.animals.Remove(this);
                        targetTable = null;
                    }
                
                    state = BlackConsumerState.FindingTarget;
                    consumerCallback?.Invoke(this);

                    return;
                }
                Vector3 target = targetTable.seats[seatIndex].transform.position;
                Stack<Vector3> moveTargets = await CalculateNodes_Async(target, false, cancellationToken);
                if (moveTargets != null && moveTargets.Count > 0)
                {
                    await BlackConsumer_Move(moveTargets, target, cancellationToken);

                    if (reCalculate)
                    {
                        while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                        reCalculate = false;
                        continue;
                    }

                    modelTrans.rotation = targetTable.seats[seatIndex].transforms.rotation;

                    await UniTask.Delay(200, cancellationToken: cancellationToken);

                    state = BlackConsumerState.Steal;
                    consumerCallback?.Invoke(this);

                    return;
                }
                else
                {
                    await BlackConsumer_Move(moveTargets, target, cancellationToken);
                    continue;
                }
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
        try
        {

        StartStealFood:
           
            if(!targetTable.placed || targetTable.seats[seatIndex].isDisEnabled || targetTable.disableNum == 3) 
            {
               /* if (preIndex != -1 && targetTable.placedFoods[seatIndex] != null)
                {
                    Vector3 t = targetTable.transforms.position;

                    Vector3 forward = -targetTable.seats[preIndex].transform.forward;
                    t += forward;
                    t.y = 0.5f;

                    targetTable.placedFoods[seatIndex].transform.DOJump(t, 1, 1, 0.2f);
                    targetTable.placedFoods[preIndex] = targetTable.placedFoods[seatIndex];
                    targetTable.placedFoods[seatIndex] = null;
                }*/
                targetTable.stealing = false;
                targetTable.hasProblem = false;
                targetTable.stolen = false;

                targetTable.tableState = TableState.None;
                   
                
                targetTable.seats[seatIndex].animal = null;
                animator.SetTrigger(AnimationKeys.Normal);
                await UniTask.Delay(200, cancellationToken: cancellationToken);
                targetTable.animals.Remove(this);
                targetTable = null;
                state = BlackConsumerState.FindingTarget;
                consumerCallback?.Invoke(this);
                return;
            }

            //  targetTable.stealing = true;
            targetTable.tableState = TableState.Stealing;

            animator.SetTrigger(AnimationKeys.Happy);
            int num = 0;
            for (int i = 0; i < targetTable.seats.Length; i++)
            {
                if (targetTable.placedFoods[i] != null) num++;
            }
          
            await UniTask.Delay(200, cancellationToken: cancellationToken);

            while (true)
            {

                if (!targetTable.placed || targetTable.seats[seatIndex].isDisEnabled || targetTable.disableNum == 3)
                {
                    goto StartStealFood;
                }
                for (int i = 0; i < targetTable.seats.Length; i++)
                {
                    if (seatIndex != i)
                    {
                        if (targetTable.placedFoods[i] != null)
                        {
                            targetTable.placedFoods[seatIndex] = targetTable.placedFoods[i];
                            targetTable.placedFoods[i] = null;
                            preIndex = i;

                            Vector3 t = targetTable.transforms.position;
                            Vector3 forward = -targetTable.seats[seatIndex].transform.forward;
                            t += forward;
                            t.y = 0.5f;
                            targetTable.placedFoods[seatIndex].transform.DOKill();
                            targetTable.placedFoods[seatIndex].transform.DOJump(t, 1, 1, 0.2f);
                            await UniTask.Delay(300, cancellationToken: cancellationToken);
                            break;
                        }
                    }
                }

                //내려놓기
                if (targetTable.placedFoods[seatIndex] == null && targetTable.foodStacks[0].foodStack.Count > 0)
                {
                    Food f = targetTable.foodStacks[0].foodStack.Pop();
                    targetTable.placedFoods[seatIndex] = f.gameObject;
                   // preIndex = i;

                    Vector3 t = targetTable.transforms.position;
                    Vector3 forward = -targetTable.seats[seatIndex].transform.forward;
                    t += forward;
                    t.y = 0.5f;
                    targetTable.placedFoods[seatIndex].transform.DOKill();
                    targetTable.placedFoods[seatIndex].transform.DOJump(t, 1, 1, 0.2f);
                    await UniTask.Delay(300, cancellationToken: cancellationToken);
                }
                else if (targetTable.placedFoods[seatIndex] == null && targetTable.foodStacks[0].foodStack.Count == 0)
                {
                    //종료
                    targetTable.tableState = TableState.Stolen;
                    targetTable.stolen = true;
                    SoundManager.Instance.PlayAudio3D(GameIns.gameSoundManager.LaughAt(), 0.1f, 100, 5, trans.position);
                    Emote emote = GameIns.restaurantManager.GetEmote();
                    emote.image.sprite = GameInstance.GameIns.restaurantManager.emoteSprites[4001];
                    emote.rectTransform.position = trans.position + Vector3.up * 4; //InputManger.cachingCamera.WorldToScreenPoint(trans.position) + Vector3.up * 50;
                    emote.height = 3;
                    emote.Emotion(1.5f);
                    animator.SetBool("bounceTrigger", true);
                    animator.SetTrigger("bounce");
                    //audioSource.clip = 
                    await UniTask.Delay(5000, cancellationToken: cancellationToken);

                    targetTable.stealing = false;
                    targetTable.hasProblem = false;
                    targetTable.stolen = false;
                    animator.SetBool("bounceTrigger", false);
                    animator.SetTrigger(AnimationKeys.Normal);
                    targetTable.seats[seatIndex].animal = null;
                    targetTable = null;

                    state = BlackConsumerState.FindingTarget;
                    consumerCallback?.Invoke(this);
                    return;
                }

                //먹기
                if (targetTable.placedFoods[seatIndex] != null)
                {
                    await Eating(targetTable.placedFoods[seatIndex].transform, cancellationToken: cancellationToken);
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                    if (!targetTable.placed || targetTable.seats[seatIndex].isDisEnabled || targetTable.disableNum == 3) goto StartStealFood;
                    Food f = targetTable.placedFoods[seatIndex].GetComponent<Food>();
                    FoodManager.EatFood(f);
                    targetTable.numberOfFoods--;
                    targetTable.numberOfFoods = targetTable.numberOfFoods < 0 ? 0 : targetTable.numberOfFoods;
                    targetTable.numberOfGarbage++;
                    targetTable.placedFoods[seatIndex] = null;

                    //쓰레기 생성
                    if (targetTable.foodStacks[0].foodStack.Count == 0 && targetTable.numberOfFoods == 0)
                    {
                        targetTable.numberOfGarbage = targetTable.numberOfGarbage > 10 ? 10 : targetTable.numberOfGarbage;
                        targetTable.isDirty = true;

                        for (int i = 0; i < targetTable.numberOfGarbage; i++)
                        {
                            Garbage go = GarbageManager.CreateGarbage();
                            go.transforms.SetParent(targetTable.trashPlate.transforms);
                            targetTable.garbageList.Add(go);

                            float x = UnityEngine.Random.Range(-1f, 1f);
                            float z = UnityEngine.Random.Range(-1f, 1f);
                            go.transforms.position = targetTable.up.position + new Vector3(x, 0, z);
                        }
                    }

                    await UniTask.Delay(200, cancellationToken: cancellationToken);
                }
            }

        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
       
    }
    async UniTask Eating(Transform transforms, CancellationToken cancellationToken = default)
    {
        try
        {
            StartEating:
            if (!targetTable.placed || targetTable.seats[seatIndex].isDisEnabled || targetTable.disableNum == 3)
            {
                await UniTask.NextFrame(cancellationToken: cancellationToken);  
             
                return;
            }

            float tm = RestaurantManager.restaurantTimer;
            float timer = animalStruct.eat_speed;
            for (int i = 1; i <= 100; i++)
            {
                if(!targetTable.placed || targetTable.seats[seatIndex].isDisEnabled || targetTable.disableNum == 3)
                {
                    goto StartEating;
                }
                //if (tm + timer / 10 <= RestaurantManager.restaurantTimer)
                if(i != 0 && i % 10 == 0)
                {
                    tm = RestaurantManager.restaurantTimer;
                    SoundManager.Instance.PlayAudio3D(GameIns.gameSoundManager.Eat(), 0.1f, 100, 5, trans.position);
                    
                    animator.SetTrigger(AnimationKeys.eat);
                    Eat eat = ParticleManager.CreateParticle(ParticleType.Eating).GetComponent<Eat>();
                    eat.transform.position = transforms.position;
                    eat.PlayEating();
                }
                await Utility.CustomUniTaskDelay(timer / 100, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                //await UniTask.Delay((int)(timer * 10), cancellationToken: cancellationToken);
            }
        }
        catch (Exception ex) 
        {
            Debug.Log(ex);
        }
    }

    async UniTask BlackConsumer_GoHome(CancellationToken cancellationToken = default)
    {
        try
        {
            animator.SetTrigger(AnimationKeys.hit);
            await UniTask.Delay(1500, cancellationToken: cancellationToken);
            while (true)
            {
                
                Stack<Vector3> moveTargets = await CalculateNodes_Async(spawnerTrans.position, false, cancellationToken);
                if (moveTargets != null && moveTargets.Count > 0)
                {
                    Vector3 test = moveTargets.Peek();
                    if (test.z == 100 && test.x == 100)
                    {
                        // moveNode = null;
                        Debug.Log("None1");
                        return;
                    }
                    else
                    {
                        await BlackConsumer_Move(moveTargets, spawnerTrans.position, cancellationToken);

                        if (reCalculate)
                        {
                            while (bWait) await UniTask.NextFrame(cancellationToken: cancellationToken);
                            reCalculate = false;
                            continue;
                        }

                      //  modelTrans.rotation = targetTable.seats[seatIndex].transforms.rotation;
                    }

                    GameIns.restaurantManager.trashData.trashNum -= 70;
                    if (GameIns.restaurantManager.trashData.trashNum < 0) GameIns.restaurantManager.trashData.trashNum = 0;
                    GameIns.restaurantManager.trashData.changed = true;
                    await UniTask.Delay(200, cancellationToken: cancellationToken);
                    animator.SetTrigger(AnimationKeys.Normal);
                    animator.SetInteger(AnimationKeys.state, 0);

                    await UniTask.Delay(100, cancellationToken: cancellationToken);
                    enemySpawner.bSpawned = false;
                    GameIns.lodManager.RemoveLODGroup(ID);
                    gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log("None");
                    // await UniTask.Delay(200, cancellationToken: cancellationToken);
                    // consumerCallback?.Invoke(this);
                }
                return;
                //  Escape: continue;
            }

        }
        catch (Exception ex) 
        {
            Debug.Log(ex);
        }
    }

    async UniTask BlackConsumer_Move(Stack<Vector3> n, Vector3 loc, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
          
            if (n != null && n.Count > 0)
            {
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
                    float cur = (target - trans.position).magnitude;

                    while (true)
                    {
                        if (reCalculate)
                        {
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
                        await UniTask.NextFrame(cancellationToken: cancellationToken);
                    }
                }
            }
            Vector3 newLoc = loc;
            newLoc.y = 0;
            while (true)
            {
                if (reCalculate)
                {
                    return;
                }
                animator.SetInteger("state", 1);
                trans.position = Vector3.MoveTowards(trans.position, newLoc, animalStruct.speed * Time.deltaTime);
                if (Vector3.Distance(trans.position, newLoc) <= 0.01f) break;
                Vector3 dir = newLoc - trans.position;
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                modelTrans.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }
            animator.SetInteger("state", 0);

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


    public void Caught()
    {
        if (!bDead)
        {
            particles.Play();
            hitAudio.clip = GameIns.gameSoundManager.Hit();
            hitAudio.volume = 0.2f;
            hitAudio.Play();
            SoundManager.Instance.PlayAudio3D(GameIns.gameSoundManager.Pain(), 0.1f, 100, 5, trans.position);

            bDead = true;

            bool isStolen = false;
            if (targetTable != null)
            {
                if (targetTable.tableState == TableState.Stealing && targetTable.foodStacks[0].foodStack.Count == 0)
                {
                    for (int i = 0; i < targetTable.placedFoods.Length; i++)
                    {
                        if (targetTable.placedFoods[i] != null) break;
                        
                        if(i == targetTable.placedFoods.Length - 1) isStolen = true;
                    }
                }
                if(isStolen)
                {
                    targetTable.tableState = TableState.Stolen;
               
                }
                else
                {
                    targetTable.tableState = TableState.None;
              
                }
               
                
                targetTable.seats[seatIndex].animal = null;
                targetTable.animals.Remove(this);
                targetTable = null;
            }
            animator.SetTrigger(AnimationKeys.Dead);
            animator.SetInteger(AnimationKeys.state, 0);
            state = BlackConsumerState.SubDue;


            consumerCallback?.Invoke(this);
        }
    }


    private void OnApplicationQuit()
    {
        if(cancellationTokenSource != null) cancellationTokenSource.Cancel();
    }

}
