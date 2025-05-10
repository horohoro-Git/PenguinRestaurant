using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODManager : MonoBehaviour
{
    Camera cachingCamera;
  //  public List<LODGroup> groups = new List<LODGroup>();
 //   public List<Animal> animals = new List<Animal>();

    public Dictionary<int, LODGroup> groups = new Dictionary<int, LODGroup>();
    public Dictionary<int, Animal> animals = new Dictionary<int, Animal>();

    public static LOD_Type lod_type = LOD_Type.LOD0;
    private void Start()
    {
        if (cachingCamera == null) cachingCamera = Camera.main;
        if(cachingCamera.orthographicSize <= 10)
        {
            lod_type = LOD_Type.LOD0;
        }
        else
        {
            lod_type = LOD_Type.LOD1;
        }
    }
    private void Update()
    {
        LevelOfDetail();
    }


    public void AddLODGroup(int id,  LODGroup group)
    {
        if(!groups.ContainsKey(id)) groups[id] = group;
    }
    public void AddInstancedAnimal(int id, Animal animal)
    {
        if(!animals.ContainsKey(id)) animals[id] = animal;
    }

    public void RemoveLODGroup(int id)
    {
        if (groups.ContainsKey(id)) groups.Remove(id);
    }

    public void RemoveInstancedAnimal(int id)
    {
        if (animals.ContainsKey(id)) animals.Remove(id);
    }

    void LevelOfDetail()
    {
        if (cachingCamera == null) cachingCamera = Camera.main;

        if (cachingCamera.orthographicSize <= 10)
        {
            if (lod_type == LOD_Type.LOD0) return;

            lod_type = LOD_Type.LOD0;
        }
        else
        {
            if (lod_type == LOD_Type.LOD1) return;
            lod_type = LOD_Type.LOD1;
        }

        foreach (var group in groups) group.Value.ForceLOD((int)lod_type);
        foreach (var animal in animals) animal.Value.InstancingLOD((int)lod_type);

    }
}
