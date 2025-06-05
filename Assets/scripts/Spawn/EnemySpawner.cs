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
        try
        {
            while (true)
            {
                if(animalManager.customerControllers.Count > 0)
                {
                    if(currentBlackConsumer == null)
                    {
                        currentBlackConsumer = animalManager.NewBlackConsumer();
                        currentBlackConsumer.trans.position = transform.position;
                        currentBlackConsumer.animalStruct = AssetLoader.animals[201];
                        currentBlackConsumer.consumerCallback?.Invoke(currentBlackConsumer);
                    }
                }



                await UniTask.Delay(100, cancellationToken: cancellationToken);
            }
        }
        catch
        {

        }
    }
}
