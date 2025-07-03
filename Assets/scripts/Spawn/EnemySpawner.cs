using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    float spawnTimer;
    BlackConsumer currentBlackConsumer;
    [NonSerialized] public bool bSpawned;
    private void Start()
    {
       
        spawnTimer = Time.time;
        EnemySpawn(App.GlobalToken).Forget();
    }

    async UniTask EnemySpawn(CancellationToken cancellationToken = default)
    {
        while (RestaurantManager.spawnTimer == 0)
        {
            await UniTask.NextFrame(cancellationToken: cancellationToken);
        }
        AnimalManager animalManager = GameInstance.GameIns.animalManager;
        WorkSpaceManager workSpace = GameInstance.GameIns.workSpaceManager;
        try
        {
            while (true)
            {
                List<Table> tables = workSpace.tables;
                bool canSpawn = false;
                for (int i = 0; i < tables.Count; i++)
                {
                    if (tables[i].foodStacks[0].foodStack.Count > 0)
                    {
                        canSpawn = true;
                        break;
                    }
                }
                canSpawn = true;
                if(animalManager.customerControllers.Count > 0 && canSpawn)
                {
                    if(!bSpawned)
                    {
                       // if (GameInstance.GameIns.restaurantManager.trashData.trashNum > 70)
                        {
                            bSpawned = true;
                            currentBlackConsumer = GameInstance.GameIns.animalManager.blackConsumer;
                            currentBlackConsumer.bDead = false;
                            currentBlackConsumer.enemySpawner = this;
                            currentBlackConsumer.spawnerTrans = transform;
                            currentBlackConsumer.trans.position = transform.position;
                            currentBlackConsumer.animalStruct = AssetLoader.animals[201];
                            currentBlackConsumer.gameObject.SetActive(true);
                            GameInstance.GameIns.lodManager.AddLODGroup(currentBlackConsumer.ID, currentBlackConsumer.lodGroup);
                            currentBlackConsumer.state = BlackConsumerState.FindingTarget;
                            currentBlackConsumer.consumerCallback?.Invoke(currentBlackConsumer);
                        }
                    }
               
                }



                await UniTask.Delay(500, cancellationToken: cancellationToken);
            }
        }
        catch
        {

        }
    }
}
