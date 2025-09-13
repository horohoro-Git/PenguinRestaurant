using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static AssetLoader;

public class WorldUI : MonoBehaviour
{
    public GameObject worldScene;
    public GameObject bg;
    public WorldMapStage worldMapStage;
    public RectTransform worldRect;
    public Queue<WorldMapStage> worldMapStages = new();
    List<WorldMapStage> stages = new();
    [NonSerialized] public WorldMapStage selectedStage;

    public ScrollSnap scrollSnap;
    int backgroundImageID;
    IEnumerator bgCoroutine;
    IEnumerator bgOriginCoroutine;
    IEnumerator borderCoroutine;
    int currentIndex = -1;
    private void Awake()
    {
        GameInstance.GameIns.worldUI = this;
        for (int i = 0; i < 100; i++)
        {
            WorldMapStage stage = Instantiate(worldMapStage, worldRect);
            stage.gameObject.SetActive(false);
            worldMapStages.Enqueue(stage);
        }
    }
    private void Start()
    {
        int clearStages = App.gameSettings.clearStage;
        for (int i = 0; i <= clearStages; i++)
        {
            WorldMapStage worldMapStage = worldMapStages.Dequeue();
            worldMapStage.gameObject.SetActive(true);
            worldMapStage.id = (Stage)i;

            scrollSnap.panelList.Add(worldMapStage.GetComponent<RectTransform>());
            if(i == maps.Count)
            {
                worldMapStage.title.text = "";
                worldMapStage.locked = true;
                stages.Add(worldMapStage);
                break;
            }
            else
            {
                MapContent mapContent = maps[i];
                Sprite sprite = loadedSprites[spriteAssetKeys[mapContent.stage_id].ID];
                worldMapStage.image.sprite = sprite;
                //이미지 크기 조정
                float width = sprite.bounds.size.x;
                float height = sprite.bounds.size.y;

                float aspectRatio = height / width;
                float recalculateHeight = 400 * aspectRatio > 580 ? 580 : 400 * aspectRatio;
                
                worldMapStage.image.rectTransform.sizeDelta = new Vector2(400, recalculateHeight);
                worldMapStage.stageLevel.text = $"Stage{i + 1}";
                worldMapStage.title.text = App.languages[mapContent.stage_id].text;
            }
            stages.Add(worldMapStage);
        }

        //test
        for (int i = 0; i <= 3; i++)
        {
            WorldMapStage worldMapStage = worldMapStages.Dequeue();
            worldMapStage.gameObject.SetActive(true);
            worldMapStage.locked = true;
            scrollSnap.panelList.Add(worldMapStage.GetComponent<RectTransform>());
            stages.Add(worldMapStage);
        }

      //  scrollSnap.Setup((int)App.currentStage);
    }

    public void BeginDrag()
    {
        if (bgCoroutine != null)
        {
            StopCoroutine(bgCoroutine);
          
        }
        if(bgOriginCoroutine != null) StopCoroutine(bgOriginCoroutine);
        bgOriginCoroutine = ChangingNormal(bg.GetComponent<Image>(), false);
        StartCoroutine(bgOriginCoroutine);

        if (borderCoroutine != null) StopCoroutine(borderCoroutine);
        borderCoroutine = ChangingNormal(worldScene.GetComponent<Image>(), true);
        StartCoroutine(borderCoroutine);
    }
    public void EndDrag()
    {
        if (bgOriginCoroutine != null) StopCoroutine(bgOriginCoroutine);
        if (borderCoroutine != null) StopCoroutine(borderCoroutine);
    }
    public void SnapToTarget(RectTransform selectTranform, int index)
    {
    
        if(stages[index].locked)
        {
            currentIndex = index;
            selectTranform.anchoredPosition = new Vector2(0, 50);
            selectTranform.sizeDelta = new Vector2(500, 600);
            if (bgCoroutine != null) StopCoroutine(bgCoroutine);
            if (bgOriginCoroutine != null) StopCoroutine(bgOriginCoroutine);
            bgOriginCoroutine = ChangingNormal(bg.GetComponent<Image>(), false);
            StartCoroutine(bgOriginCoroutine);
            if (borderCoroutine != null) StopCoroutine(borderCoroutine);
            borderCoroutine = ChangingNormal(worldScene.GetComponent<Image>(), true);
            StartCoroutine(borderCoroutine);
        }
        else
        {
            RectTransform rectTransform = stages[index].image.GetComponent<RectTransform>();
            selectTranform.anchoredPosition = new Vector2(0, -25f);
            selectTranform.sizeDelta = new Vector2(500, rectTransform.sizeDelta.y + 220f + 100f);

            float posY = Mathf.Lerp(-100, -25, 900f / (rectTransform.sizeDelta.y + 220f + 100f));

            if (currentIndex != index)
            {
                currentIndex = index;
                int[] ints;
                string key = maps[index].sprite_key;
                ints = key.Split(',').Select(int.Parse).ToArray();
                backgroundImageID = 0;
                if (ints.Length > 0)
                {
                    if (bgCoroutine != null)
                    {
                        StopCoroutine(bgCoroutine);

                    }
                    bgCoroutine = ChangingBG(ints);
                    StartCoroutine(bgCoroutine);
                }
                else
                {
                    if (bgCoroutine != null) StopCoroutine(bgCoroutine);
                    if (bgOriginCoroutine != null) StopCoroutine(bgOriginCoroutine);
                    bgOriginCoroutine = ChangingNormal(bg.GetComponent<Image>(), false);
                    StartCoroutine(bgOriginCoroutine);
                    if (borderCoroutine != null) StopCoroutine(borderCoroutine);
                    borderCoroutine = ChangingNormal(worldScene.GetComponent<Image>(), true);
                    StartCoroutine(borderCoroutine);
                }
            }
        }
    }

    IEnumerator ChangingBG(int[] keys)
    {


        Image borderImage = worldScene.GetComponent<Image>();
        Image image = bg.GetComponent<Image>();

        if (bgOriginCoroutine != null) StopCoroutine(bgOriginCoroutine);
        bgOriginCoroutine = ChangingNormal(bg.GetComponent<Image>(), false);
        StartCoroutine(bgOriginCoroutine);
        if (borderCoroutine != null) StopCoroutine(borderCoroutine);
        borderCoroutine = ChangingNormal(worldScene.GetComponent<Image>(), true);
        StartCoroutine(borderCoroutine);

        yield return bgOriginCoroutine;
        yield return borderCoroutine;
        int num = keys.Length;
        while(true)
        {
            int rand = UnityEngine.Random.Range(0, num);
            if(backgroundImageID == rand)
            {
                yield return null;
                continue;
            }
            backgroundImageID = rand;
            image.sprite = loadedSprites[spriteAssetKeys[keys[rand]].ID];
            float f = 0;
            Color color = new Color(1, 1, 1, 0);
            while (f < 1)
            {
                color.a = Mathf.Lerp(0, 1, f);
                image.color = color;
                f += Time.unscaledDeltaTime / 2f;
                yield return null;
            }
            color = Color.white;
            image.color = Color.white;
            borderImage.color = Color.black;
            yield return new WaitForSecondsRealtime(5f);
            f = 0;
            while (f < 1)
            {
                color.a = Mathf.Lerp(1, 0, f);
                image.color = color;
                f += Time.unscaledDeltaTime / 2f;
                yield return null;
            }
            color.a = 0;
            image.color = color;
        }

    }

    IEnumerator ChangingNormal(Image image, bool isWhite)
    {
     //   Image image = bg.GetComponent<Image>();

        float f = 0;
        Color color = new Color(1, 1, 1, image.color.a);
        float offset = isWhite ? -color.a : 1 - color.a;
      
        while (f < (isWhite ? (1 + offset) : (1 - offset)))
        {
            color.a = isWhite ? Mathf.Lerp(0, 1, f - offset) : Mathf.Lerp(1, 0, f + offset);
            image.color = color;
            f += Time.unscaledDeltaTime / 2;
            yield return null;
        }
        color.a = isWhite ? 1 : 0;
        image.color = color;
    }
 
}
