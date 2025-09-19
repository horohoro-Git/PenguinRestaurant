using SRF;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class StagePreviewController : MonoBehaviour
{
    [System.Serializable]
    public struct PreviewType
    {
        public List<GameObject> spawnPoints;
        public GameObject previewType;
    }
    public RenderTexture renderTexture;
    public Camera renderCamera;

    public List<PreviewType> previewTypes = new();
    public List<NavMeshSurface> navMeshSurfaces = new();
    public NavMeshDataInstance navMeshData; 
    [NonSerialized] public Dictionary<int, Queue<AnimalStagePreview>> animalPreviews = new();
    int type = -1;

    private void Awake()
    {
        GameInstance.GameIns.stagePreviewController = this;
     /*   for(int i = 0; i < planes.Count; i++)
        {
            planes[i].BuildNavMesh();
            planes[i].GetComponent<MeshCollider>().enabled = false;
            planes[i].GetComponent<MeshRenderer>().enabled = false;
        }*/
    }

 
    private void OnDestroy()
    {
        foreach(var v in animalPreviews)
        {
            while(v.Value.Count > 0)
            {
                GameInstance.GameIns.animalPreviewManager.InAnimal(v.Key, v.Value.Dequeue());
            }
        }
        NavMesh.RemoveNavMeshData(navMeshData);
    }



    public void SpawnCharacter(int[] ids)
    {
        int previewNum = Random.Range(0, previewTypes.Count);
        if (previewTypes.Count > 1)
        {
            while (type == previewNum)
            {
                previewNum = Random.Range(0, previewTypes.Count);
            }
        }
        type = previewNum;
        int spawnNum = previewTypes[previewNum].spawnPoints.Count;
        for (int i = 0; i < previewTypes.Count; i++)
        {
            if (i == previewNum)
            {
                previewTypes[i].previewType.SetActive(true);
            }
            else
            {
                previewTypes[i].previewType.SetActive(false);
            }
        }
        navMeshData = NavMesh.AddNavMeshData(navMeshSurfaces[type].navMeshData, navMeshSurfaces[type].transform.position, navMeshSurfaces[type].transform.rotation);
        for (int i = 0; i < spawnNum; i++)
        {
           // NavMeshHit hit;
           // bool found = NavMesh.SamplePosition(previewTypes[previewNum].spawnPoints[i].transform.position, out hit, 5f, NavMesh.AllAreas);
            int rand = ids.Random();
            AnimalStagePreview animalStagePreview = GameInstance.GameIns.animalPreviewManager.OutAnimal(rand);
            if (!animalPreviews.ContainsKey(rand)) animalPreviews[rand] = new();
            animalPreviews[rand].Enqueue(animalStagePreview);

            Vector3 localPos = previewTypes[previewNum].spawnPoints[i].transform.position - navMeshSurfaces[type].transform.position;
            localPos = Quaternion.Inverse(navMeshSurfaces[type].transform.rotation) * localPos;
            Vector3 warpedPos = navMeshSurfaces[type].transform.position + navMeshSurfaces[type].transform.rotation * localPos;
       //     animalStagePreview.agent.Warp(warpedPos);
           /* if (NavMesh.SamplePosition(previewTypes[previewNum].spawnPoints[i].transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                Debug.Log(animalStagePreview.agent.Warp(hit.position) + "Warp Working");
            }*/
          
           // animalStagePreview.agent.Add
           // animalStagePreview.transform.SetParent(previewTypes[previewNum].spawnPoints[i].transform, true);
            animalStagePreview.transform.position = previewTypes[previewNum].spawnPoints[i].transform.position;
            animalStagePreview.transform.rotation = Quaternion.Euler(Vector3.zero);
            animalStagePreview.Setup();
         //   animalStagePreview.RandomMove();
        }
        //   StartCoroutine(DelaySpawn(ids));
    }

    public void RemoveCharacter()
    {
        NavMesh.RemoveNavMeshData(navMeshData);

        foreach(var v in animalPreviews)
        {
            int count = v.Value.Count;
            for (int i = 0; i < count; i++)
            {
                GameInstance.GameIns.animalPreviewManager.InAnimal(v.Key, v.Value.Dequeue());
            }
        }

    }

    IEnumerator DelaySpawn(int[] ids)
    {
        yield return new WaitForSecondsRealtime(2);
      
    }
}
