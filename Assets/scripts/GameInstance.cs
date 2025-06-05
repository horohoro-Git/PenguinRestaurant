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
    public static GameInstance GameIns { get { return gameInstance; }  }
    private static Queue<Vector3> pool = new Queue<Vector3>();
    public static List<GraphicRaycaster> graphicRaycasters = new List<GraphicRaycaster>();
  /*  public static Stack<Node> GetNodes(ref Stack<Node> stackNodes, Node node)
    {
        Node nd = node;
        //   Stack<Coord> stack = new Stack<Coord>();
        stackNodes.Push(nd);
        while (!(nd.parentNode == null || nd.parentNode == Node.node))
        {
            Node n = nd;
            nd = nd.parentNode;
            n.parentNode = null;
            stackNodes.Push(nd);
        }
        return stackNodes;
    }*/
    static Vector3 newVector = new Vector3(0,0,0);
    public static Vector3 GetVector3(float x, float y, float z)
    {
       /* if(pool.Count > 0)
        {
            Vector3 returnVector = pool.Dequeue();
            returnVector.x = x;
            returnVector.y = y;
            returnVector.z = z;
            return returnVector;
        }
        else
        {
            return new Vector3(x,y,z);
        }*/
        return new Vector3(x,y,z);
    }
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
