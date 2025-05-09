using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODManager : MonoBehaviour
{
    Camera cachingCamera;
    public List<LODGroup> groups = new List<LODGroup>();
    public List<Animal> animals = new List<Animal>();

    public static LOD_Type lod_type = LOD_Type.LOD1;
    private void Start()
    {
        if (cachingCamera == null) cachingCamera = Camera.main;
        if(cachingCamera.orthographicSize <= 10)
        {
            lod_type = LOD_Type.LOD1;
        }
        else
        {
            lod_type = LOD_Type.LOD2;
        }
    }
    private void Update()
    {
        LevelOfDetail();
    }


    void LevelOfDetail()
    {
        if (cachingCamera == null) cachingCamera = Camera.main;

        if (cachingCamera.orthographicSize <= 10)
        {
            if (lod_type == LOD_Type.LOD1) return;

            lod_type = LOD_Type.LOD1;
        }
        else
        {
            if (lod_type == LOD_Type.LOD2) return;
            lod_type = LOD_Type.LOD2;
        }

        for (int i = 0; i < groups.Count; i++) groups[i].ForceLOD((int)lod_type);
      
        for(int i = 0; i<animals.Count; i++) animals[i].InstancingLOD((int)lod_type);
    }
}
