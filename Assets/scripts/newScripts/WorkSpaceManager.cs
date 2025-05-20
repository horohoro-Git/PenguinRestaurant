using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AssetLoader;
public class WorkSpaceManager : MonoBehaviour
{
   // ������� �Ŵ������� �߰��Ǵ� Ȯ�尡�� �ⱸ��
    public List<Counter> counters = new List<Counter>();
    public List<FoodMachine> foodMachines = new List<FoodMachine>();
    public List<Table> tables = new List<Table>();
    public List<TrashCan> trashCans = new List<TrashCan>();
    public List<FoodStack> foodStacks = new List<FoodStack>();
    public List<EndPoint> endPoints = new List<EndPoint>();
    public List<PackingTable> packingTables = new List<PackingTable>();
    public List<AnimalSpawner> spwaners = new List<AnimalSpawner>();

    public int[] unlocks = new int[4];

    //�������� �ִ� ���� ����


    public Food food;
    public PackageFood box;
    public Garbage garbage;
    public GameObject[] particle;
    public RewardsBox rewardsBox;

    private void Awake()
    {
       // food = loadedAssets[itemAssetKeys[1000].Name].GetComponent<Food>();
        FoodManager.NewFood(food, 200);
        FoodManager.NewFood(food, 100, true);
        FoodManager.NewPackageBox(box, 30);
        GarbageManager.NewGarbage(garbage, 50);
        for (int i = 0; i < particle.Length; i++)
        {
            ParticleManager.NewParticle(particle[i], 50);
        }

        FoodManager.NewRewardsBox(rewardsBox, 5);


    }


    public void AddWorkSpace(Furniture newWork)
    {
        newWork.spawned = true;
        switch(newWork.spaceType)
        {
            case WorkSpaceType.None:
                break;
            case WorkSpaceType.Counter:
                break;
            case WorkSpaceType.Table:
                break;
            case WorkSpaceType.FoodMachine:
           //     newWork.GetComponent<FoodMachine>().PlaceTray();
                break;
        }
       // if(newWork.TryGetComponent<>)
    }
}
