using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
//한글

public enum SceneState
{
    Menu,
    Loading,
    Restaurant,
    Draw
}

public class App : MonoBehaviour
{
    private static CancellationTokenSource globalCts = new CancellationTokenSource();
    public static CancellationToken GlobalToken => globalCts.Token;

    public SceneState currentScene;
    GetWaitTimer getWaitTimer = new GetWaitTimer();
    public Vector3 pos;
    private void Awake()
    {
        //Resources.UnloadUnusedAssets();
        //SRDebug.Instance.HideDebugPanel();
        currentScene = SceneState.Restaurant;
        GameInstance.GameIns.app = this;
        DontDestroyOnLoad(this);
        SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
        /*   SceneManager.LoadSceneAsync("InteractionScene", LoadSceneMode.Additive);
           SceneManager.LoadSceneAsync("RestaurantScene", LoadSceneMode.Additive);
          // SceneManager.LoadSceneAsync("RestaurantScene(BackUp)", LoadSceneMode.Additive);
           SceneManager.LoadSceneAsync("GatCharScene_Town", LoadSceneMode.Additive);*/

        //   SceneManager.sceneCount
    }
    private void Start()
    {
        if (GameInstance.GameIns.assetLoader)
        {
            UniTask.Void(async () =>
            {
               await GameInstance.GameIns.assetLoader.DownloadAssetBundle();
            });
            GameObject.Find("Loading").GetComponent<Loading>().ChangeText("에셋 로딩 중");
            Invoke("LoadAsset", 0.5f);
        }
     /*   for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            // 현재 씬의 모든 루트 오브젝트 비활성화
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                bool visible = !(scene.name == "GatCharScene_Town");// || scene.name == "RestaurantScene");
              //  rootObject.SetActive(visible); // 특정 씬만 활성화
            }
        }*/
     
    }

    void LoadScene()
    {
        if (GameInstance.GameIns.assetLoader.sceneLoaded)
        {
            Debug.Log("Load");
            GameLoaded();
        }
        else
        {
            Invoke("LoadScene", 0.5f);
        }
    }
    void LoadAsset()
    {
        if (GameInstance.GameIns.assetLoader.assetLoadSuccessful)
        {
            UniTask.Void(async () =>
            {
                await GameInstance.GameIns.assetLoader.DownloadAsset_SceneBundle();
            });
            GameObject.Find("Loading").GetComponent<Loading>().ChangeText("맵 로딩 중");
            Invoke("LoadScene", 0.5f);
        }
        else
        {
            Invoke("LoadAsset", 0.5f);
        }
    }

    void GameLoaded()
    {
        SceneManager.LoadSceneAsync(AssetLoader.loadedMap["InteractionScene"], LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(AssetLoader.loadedMap["RestaurantScene"], LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(AssetLoader.loadedMap["GatCharScene_Town"], LoadSceneMode.Additive);
        currentScene = SceneState.Restaurant;

        GameObject.Find("Loading").GetComponent<Loading>().Set();
    }

    private void OnApplicationQuit()
    {
        if(GameInstance.GameIns != null)
        {
            GameInstance.GameIns.Clear();
            globalCts.Cancel();
            globalCts.Dispose();
            globalCts = null;
        }
    }
    //bool down=false;
    /*  public void Update()
      {
          if (Input.GetKeyDown(KeyCode.Alpha1) && !down )
          {

              if (GameInstance.GameIns.animalManager)
              {
                //  if (GameInstance.GameIns.animalManager.animalActionCoroutine != null) { StopCoroutine(GameInstance.GameIns.animalManager.animalActionCoroutine); }

                  for (int i = 0; i < SceneManager.sceneCount; i++)
                  {
                      Scene scene = SceneManager.GetSceneAt(i);

                      // 현재 씬의 모든 루트 오브젝트 비활성화
                      foreach (GameObject rootObject in scene.GetRootGameObjects())
                      {
                  //        rootObject.SetActive(scene.name != "RestaurantScene"); // 특정 씬만 활성화

                      }

                  }

              }
              down = true;
          }
          if (Input.GetKeyDown(KeyCode.Alpha2) && down)
          {
              down = false;

              if (GameInstance.GameIns.animalManager)
              {

                  for (int i = 0; i < SceneManager.sceneCount; i++)
                  {
                      Scene scene = SceneManager.GetSceneAt(i);

                      // 현재 씬의 모든 루트 오브젝트 비활성화
                      foreach (GameObject rootObject in scene.GetRootGameObjects())
                      {
                          bool visible = (scene.name == "InteractionScene" || scene.name == "RestaurantScene");
               //           rootObject.SetActive(visible); // 특정 씬만 활성화
                      }
                  }
              }
               GameInstance.GameIns.inputManager.inputDisAble = false;
              GameInstance.GameIns.inputManager.cameraTrans.position = pos;
              Time.timeScale = 1;
              //

              //GameInstance.GameIns.animalManager.StartRoutine();

              ////
              //for (int i = 0; i < GameInstance.GameIns.animalManager.employeeControllers.Count; i++)
              //{
              //    if (GameInstance.GameIns.animalManager.employeeControllers[i].spawning == false)
              //    {
              //        if (GameInstance.GameIns.animalManager.employeeControllers[i].restartCoroutine != null)
              //        {
              //            GameInstance.GameIns.animalManager.employeeControllers[i].restartCoroutine.Invoke();

              //        }

              //    }
              //    else
              //    {
              //        GameInstance.GameIns.animalManager.employeeControllers[i].StartFalling(false);
              //    }
              //}

              //for (int i = 0; i < GameInstance.GameIns.animalManager.customerControllers.Count; i++)
              //{
              //    if (GameInstance.GameIns.animalManager.customerControllers[i].restartCoroutine != null)
              //    {
              //        GameInstance.GameIns.animalManager.customerControllers[i].restartCoroutine.Invoke();
              //    }
              //}

              //for (int i = 0; i < GameInstance.GameIns.workSpaceManager.foodMachines.Count; i++)
              //{

              //    GameInstance.GameIns.workSpaceManager.foodMachines[i].Cooking();
              //}

              //for (int i = 0; i < GameInstance.GameIns.workSpaceManager.spwaners.Count; i++)
              //{
              //    if (GameInstance.GameIns.workSpaceManager.spwaners[i])
              //    {
              //        GameInstance.GameIns.workSpaceManager.spwaners[i].RestartSpawner();
              //    }
              //}
          }
      }
  */
    public void ChangeScene_Restaurant()
    {
        if (currentScene == SceneState.Restaurant) return;
        currentScene = SceneState.Restaurant; 
        GameInstance.GameIns.inputManager.cameraTrans.position = pos;
      //  GameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
        GameInstance.GameIns.inputManager.inputDisAble = false;
    //    GameInstance.GameIns.applianceUIManager.UIClearAll(true);
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }

    public void ChangeScene_DrawScene()
    {
        if (currentScene == SceneState.Draw) return;
        currentScene = SceneState.Draw;
        GameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
    //    GameInstance.GameIns.inputManager.inputDisAble = true;
        pos = GameInstance.GameIns.inputManager.cameraTrans.position;
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0f;
        GameInstance.GameIns.inputManager.cameraTrans.position = GameInstance.GetVector3(-80.35f, 0, -1080.7f);
        GameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        GameInstance.GameIns.uiManager.drawSpeedUpBtn.gameObject.SetActive(true);

    }
}
