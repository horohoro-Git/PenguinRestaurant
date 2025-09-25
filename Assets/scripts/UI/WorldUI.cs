using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    IEnumerator bgCoroutine;
    IEnumerator bgOriginCoroutine;
    IEnumerator borderCoroutine;
    [NonSerialized] public int currentIndex = -1;
    

    CancellationTokenSource tokenBG;
    CancellationTokenSource tokenBGOrigin;
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
        {
            WorldMapStage worldMapStage = worldMapStages.Dequeue();
            worldMapStage.gameObject.SetActive(true);
            worldMapStage.id = Stage.Forest_01;
            scrollSnap.panelList.Add(worldMapStage.GetComponent<RectTransform>());
            stages.Add(worldMapStage);
        }
 /*       //test
        for (int i = 0; i <= 3; i++)
        {
            WorldMapStage worldMapStage = worldMapStages.Dequeue();
            worldMapStage.gameObject.SetActive(true);
            worldMapStage.locked = true;
            scrollSnap.panelList.Add(worldMapStage.GetComponent<RectTransform>());
            stages.Add(worldMapStage);
        }*/

      //  scrollSnap.Setup((int)App.currentStage);
    }

    private void OnDestroy()
    {
        if(tokenBG != null) tokenBG.Cancel();
        if(tokenBGOrigin != null) tokenBGOrigin.Cancel();
    }
    
    public void EndToken()
    {
        if (tokenBG != null) tokenBG.Cancel();
        if (tokenBGOrigin != null) tokenBGOrigin.Cancel();
    }
    public void SnapToTarget(RectTransform selectTranform, int index)
    {
    
        if(stages[index].locked)
        {
            int preIndex = currentIndex;
            currentIndex = index;
            selectTranform.anchoredPosition = new Vector2(0, 50);
            selectTranform.sizeDelta = new Vector2(500, 600);
     //       if (bgCoroutine != null) StopCoroutine(bgCoroutine);
        //    if (bgOriginCoroutine != null) StopCoroutine(bgOriginCoroutine);
            //bgOriginCoroutine = ChangingNormal(bg.GetComponent<RawImage>(), false);
      //      StartCoroutine(bgOriginCoroutine);
       //     if (borderCoroutine != null) StopCoroutine(borderCoroutine);
      // //     borderCoroutine = ChangingNormal(worldScene.GetComponent<Image>(), true);
                //StartCoroutine(borderCoroutine);

            if (tokenBG != null) tokenBG.Cancel();
            if (tokenBGOrigin != null) tokenBGOrigin.Cancel();
            tokenBGOrigin = new();
            ChangingNormal(bg.GetComponent<RawImage>(), false, true, tokenBGOrigin.Token).Forget();
            ChangingNormal(worldScene.GetComponent<Image>(), true, false, tokenBGOrigin.Token).Forget();
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
                if (key != null && key.Length > 0)
                {
                    ints = key.Split(',').Select(int.Parse).ToArray();
                    if (ints.Length > 0)
                    {

                        //maps[index].map_name
                        if (tokenBG != null) tokenBG.Cancel();
                        tokenBG = new();
                        ChangingBG(ints, tokenBG.Token).Forget();
                      /*  if (bgCoroutine != null)
                        {
                            StopCoroutine(bgCoroutine);

                        }*/
                        //     bgCoroutine = ChangingBG(ints);
                        //   StartCoroutine(bgCoroutine);
                    }
                }
                else
                {
                    if (tokenBG != null) tokenBG.Cancel();
                    if (tokenBGOrigin != null) tokenBGOrigin.Cancel();
                    tokenBGOrigin = new();
                    ChangingNormal(bg.GetComponent<RawImage>(), false, true, tokenBGOrigin.Token).Forget();
                    ChangingNormal(worldScene.GetComponent<Image>(), true, false, tokenBGOrigin.Token).Forget();
            
                }
            }
        }
    }

    async UniTask ChangingBG(int[] keys, CancellationToken cancellationToken = default)
    {
        try
        {
            int index = currentIndex;

            Image borderImage = worldScene.GetComponent<Image>();
            RawImage image = bg.GetComponent<RawImage>();

            if (tokenBGOrigin != null) tokenBGOrigin.Cancel();
            tokenBGOrigin = new();
            await UniTask.WhenAll(ChangingNormal(bg.GetComponent<RawImage>(), false, false, tokenBGOrigin.Token), ChangingNormal(borderImage.GetComponent<Image>(), true, false, tokenBGOrigin.Token));
            await App.UnloadAsync(App.previewStageName);
            string targetName = maps[index].map_name + "_stage";
            await App.LoadScene(targetName, "", App.GlobalToken);
            App.previewStageName = targetName;
            int num = keys.Length;
            
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                GameInstance.GameIns.stagePreviewController.SpawnCharacter(keys);
                float f = 0;
                Color color = new Color(1, 1, 1, 0);
                while (f < 1)
                {
                    color.a = Mathf.Lerp(0, 1, f);
                    image.color = color;
                    f += Time.unscaledDeltaTime / 2f;
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                }
                color = Color.white;
                image.color = Color.white;
                borderImage.color = Color.black;
                await UniTask.Delay(5000, true, cancellationToken: cancellationToken);
                //   yield return new WaitForSecondsRealtime(5f);
                f = 0;
                while (f < 1)
                {
                    color.a = Mathf.Lerp(1, 0, f);
                    image.color = color;
                    f += Time.unscaledDeltaTime / 2f;
                    await UniTask.NextFrame(cancellationToken: cancellationToken);
                }
                cancellationToken.ThrowIfCancellationRequested();
                color.a = 0;
                image.color = color;
                GameInstance.GameIns.stagePreviewController.RemoveCharacter();
            }
        }
        catch (Exception ex)
        {
      //      Debug.LogException(ex);
        }
    }

    async UniTask ChangingNormal<T>(T image, bool isWhite, bool unload, CancellationToken cancellationToken = default)
    {
        float f = 0;
       
        if(image is Image uiImage)
        {
            Color color = new Color(1, 1, 1, uiImage.color.a);
            float offset = isWhite ? -color.a : 1 - color.a;

            while (f < (isWhite ? (1 + offset) : (1 - offset)))
            {
                color.a = isWhite ? Mathf.Lerp(0, 1, f - offset) : Mathf.Lerp(1, 0, f + offset);
                uiImage.color = color;
                f += Time.unscaledDeltaTime / 2;
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }
            color.a = isWhite ? 1 : 0;
            uiImage.color = color;
        }
        else if(image is RawImage rawImage)
        {
            Color color = new Color(1, 1, 1, rawImage.color.a);
            float offset = isWhite ? -color.a : 1 - color.a;

            while (f < (isWhite ? (1 + offset) : (1 - offset)))
            {
                color.a = isWhite ? Mathf.Lerp(0, 1, f - offset) : Mathf.Lerp(1, 0, f + offset);
                rawImage.color = color;
                f += Time.unscaledDeltaTime / 2;
                await UniTask.NextFrame(cancellationToken: cancellationToken);
            }
            color.a = isWhite ? 1 : 0;
            rawImage.color = color;
        }
        int index = currentIndex;

        if(unload)
        {
            await App.UnloadAsync(App.previewStageName);
        }
    }
 
}
