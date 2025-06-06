using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    float spawnTimer;
    BlackConsumer currentBlackConsumer;
    private void Start()
    {
        spawnTimer = Time.time;
        EnemySpawn(App.GlobalToken).Forget();
    }

    async UniTask EnemySpawn(CancellationToken cancellationToken = default)
    {
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
                if(animalManager.customerControllers.Count > 0 && canSpawn)
                {
                    if(currentBlackConsumer == null)
                    {
                        currentBlackConsumer = animalManager.NewBlackConsumer();
                        currentBlackConsumer.spawnerTrans = transform;
                        currentBlackConsumer.trans.position = transform.position;
                        currentBlackConsumer.animalStruct = AssetLoader.animals[201];
                        Debug.Log(currentBlackConsumer.animalStruct.eat_speed);
                        GameInstance.GameIns.lodManager.AddLODGroup(currentBlackConsumer.ID, currentBlackConsumer.lodGroup);
                        currentBlackConsumer.state = BlackConsumerState.FindingTarget;
                        currentBlackConsumer.consumerCallback?.Invoke(currentBlackConsumer);
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
