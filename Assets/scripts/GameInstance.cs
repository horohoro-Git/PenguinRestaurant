using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameInstance
{
    static GameInstance gameInstance = new GameInstance();

    public App app;
    public PlayerCamera playerCamera;
    public CalculatorScale calculatorScale;
    public AnimalManager animalManager;
    public WorkSpaceManager workSpaceManager;
    public InputManger inputManager;
    public UIManager uiManager;
    public MoneyManager moneyManager;
    public RestaurantManager restaurantManager;
    public ApplianceUIManager applianceUIManager;
    public GatcharManager gatcharManager;
    public PlayerAnimalDataManager playerAnimalDataManager;
   // public CoroutneManager coroutneManager;
    public AssetLoader assetLoader;
    public GridManager gridManager;
    public Store store;
    public LODManager lodManager;
    public UISoundManager uISoundManager;
    public GameSoundManager gameSoundManager;
    public BGMSoundManager bgMSoundManager;
    public GatchaSoundManager gatchaSoundManager;
    public FishingManager fishingManager;
    public FishingSoundManager fishingSoundManager;
    public static GameInstance GameIns { get { return gameInstance; }  }
    private static Queue<Vector3> pool = new Queue<Vector3>();
    public static List<GraphicRaycaster> graphicRaycasters = new List<GraphicRaycaster>();

    public static void Release(Vector3 v)
    {
        pool.Enqueue(v);  // 객체 풀에 반환
    }

    public void Clear()
    {
        if(assetLoader != null)
        {

        }
    }

    public static void AddGraphicCaster(GraphicRaycaster raycaster)
    {
        if(!graphicRaycasters.Contains(raycaster)) graphicRaycasters.Add(raycaster);
    }
    public static void RemoveGraphicCaster(GraphicRaycaster raycaster)
    {
        graphicRaycasters.Remove(raycaster);
    }
}
