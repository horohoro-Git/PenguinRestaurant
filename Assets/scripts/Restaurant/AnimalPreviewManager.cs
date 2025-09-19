using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimalPreviewManager : MonoBehaviour
{
    public Dictionary<int, Queue<AnimalStagePreview>> previews = new();
    public GameObject previewAnimals;

    private void Awake()
    {
        GameInstance.GameIns.animalPreviewManager = this;
    }
    private void Start()
    {
        for (int i = 10100; i <= 10105; i++)
        {
            previews[i] = new Queue<AnimalStagePreview>();
           
            for(int j = 0; j < 6; j++)
            {
                AnimalStagePreview animalStagePreview = Instantiate(AssetLoader.loadedAssets[AssetLoader.itemAssetKeys[i].ID], previewAnimals.transform).GetComponent<AnimalStagePreview>();
                animalStagePreview.gameObject.SetActive(false);
                previews[i].Enqueue(animalStagePreview);
            }
        }
    }

    public AnimalStagePreview OutAnimal(int id)
    {
        AnimalStagePreview animalStagePreview = previews[id].Dequeue();
        animalStagePreview.gameObject.SetActive(true);

        return animalStagePreview;
    }

    public void InAnimal(int id, AnimalStagePreview animalStagePreview)
    {
        if (animalStagePreview.IsDestroyed()) return;
        animalStagePreview.gameObject.SetActive(false);
       // animalStagePreview.transform.SetParent(previewAnimals.transform);
        animalStagePreview.transform.localPosition = Vector3.zero;
        previews[id].Enqueue(animalStagePreview);
    }
}
    