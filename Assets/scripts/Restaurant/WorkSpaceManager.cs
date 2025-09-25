using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static AssetLoader;
public class WorkSpaceManager : MonoBehaviour
{
   // 레스토랑 매니저에서 추가되는 확장가능 기구들
    public List<Counter> counters = new List<Counter>();
    public List<FoodMachine> foodMachines = new List<FoodMachine>();
    public List<Table> tables = new List<Table>();
    public List<TrashCan> trashCans = new List<TrashCan>();
    public List<FoodStack> foodStacks = new List<FoodStack>();
    public List<EndPoint> endPoints = new List<EndPoint>();
    public List<PackingTable> packingTables = new List<PackingTable>();
    public List<AnimalSpawner> spwaners = new List<AnimalSpawner>();
    public bool[] unlockFoods = new bool[4];
    public bool[] unlockCounter = new bool[2];
    public int[] unlocks = new int[4];
    public bool unlockTable;
    //직원에게 주는 보상 관리


    public Food food;
    public PackageFood box;
    public Garbage garbage;
    public GameObject[] particle;
    public RewardsBox rewardsBox;

    public static GameObject foodCollects;
    public static GameObject garbageCollects;
    public static GameObject particleCollects;
    public static GameObject machineFoodCollects;
    private void Awake()
    {
     
    }
    private void Start()
    {
        foodCollects = new();
        foodCollects.name = "foodCollects";
        garbageCollects = new GameObject();
        garbageCollects.name = "GarbageCollects";
        particleCollects = new GameObject();
        particleCollects.name = "particleCollects";
        machineFoodCollects = new GameObject();
        machineFoodCollects.name = "machineFoodCollects";
        Scene myScene = gameObject.scene;
        SceneManager.MoveGameObjectToScene(foodCollects, myScene);
        SceneManager.MoveGameObjectToScene(garbageCollects, myScene);
        SceneManager.MoveGameObjectToScene(particleCollects, myScene);
        SceneManager.MoveGameObjectToScene(machineFoodCollects, myScene);
        foodCollects.transform.position = Vector3.zero;
        garbageCollects.transform.position = Vector3.zero;
        particleCollects.transform.position = Vector3.zero;
        machineFoodCollects.transform.position = Vector3.zero;
        FoodManager.NewFood(food, 200, foodCollects);
        GarbageManager.NewGarbage(garbage, 50);
        for (int i = 0; i < particle.Length; i++)
        {
            ParticleManager.NewParticle(particle[i], 100, (ParticleType)i);
        }
    }

}
