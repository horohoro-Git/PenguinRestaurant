using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static AssetLoader;
public class WorldMapStage : MonoBehaviour
{
    public Stage id;
    public Image image;
    public TMP_Text title;
    public TMP_Text stageLevel;
    Button btn;
    public bool locked;
    private void Start()
    {
        btn = GetComponent<Button>();

        btn.onClick.AddListener(() =>
        {
            if (id == Stage.None) return;

            if (App.currentStage == id)
            {
                //이미 로드된 스테이지
                Debug.Log("Already Loaded");
                GameInstance.GameIns.uiManager.ChangeRestaurant();
            }
            else
            {
                //새로운 스테이지 로드
                Debug.Log("New Stage");
            }
        });
    }

    IEnumerator ChangeWorldMap()
    {

      /*  Image[] images = GameInstance.GameIns.worldUI.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            image.raycastTarget = false;
        }*/
        Animator animator = GameInstance.GameIns.worldUI.GetComponent<Animator>();
        animator.SetInteger("state", 1);
        RestaurantManager.restaurantTimer = 0;
        Time.timeScale = 0;
        SoundManager.Instance.PlayAudio(loadedSounds["CloudClose"], 1);
        yield return new WaitForSecondsRealtime(1);
        //   worldMap.worldScene.gameObject.SetActive(true);
        WorldUI worldUI = GameInstance.GameIns.worldUI.GetComponent<WorldUI>();
        worldUI.worldScene.SetActive(false);
        worldUI.bg.SetActive(false);
        SoundManager.Instance.PlayAudio(loadedSounds["CloudOpen"], 1);
        animator.SetInteger("state", 2);
        yield return new WaitForSecondsRealtime(0.5f);
        GameInstance.GameIns.inputManager.InputDisAble = false;
        RestaurantManager.restaurantTimer = 1;
        Time.timeScale = 1;
        App.restaurantTimeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        Debug.Log("Start");
        GameInstance.GameIns.bgMSoundManager.BGMChange(901000, 1);
    }
}
