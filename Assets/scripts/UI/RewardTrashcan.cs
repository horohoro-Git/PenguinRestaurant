using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AssetLoader;

public class RewardTrashcan : MonoBehaviour
{
    public Button rewardBtn;
    public Animator animator;
    public Image trashcanImage;
    public Image resultImage;
    public TMP_Text clickerText;
    RectTransform rect;
    Vector3 origin;
    Vector3 velocity;
    bool clickable;
    int clickNum;
    private void Awake()
    {
        rect = trashcanImage.GetComponent<RectTransform>();
        origin = rect.position;
        rewardBtn.onClick.AddListener(() =>
        {
            if (clickable)
            {
                clickNum++;
                clickerText.text = clickNum.ToString();
                Vector3 pos = rect.position;
                float r1 = Random.Range(-30, 30);
                float r2 = Random.Range(10, 30);
                pos.x += r1;
                pos.y += r2;
                rect.position = pos;
                SoundManager.Instance.PlayAudio(GameInstance.GameIns.gameSoundManager.TrashcanHit(), 0.2f);
            }
        });
    }


    public void StartClicker()
    {
        trashcanImage.gameObject.SetActive(true);
        trashcanImage.sprite = AssetLoader.loadedSprites[AssetLoader.spriteAssetKeys[5001].ID];
        clickerText.gameObject.SetActive(true);
        clickerText.text = "0";
        rect.position = origin;
        StartCoroutine(Clicker());
    }

    IEnumerator Clicker()
    {
        SoundManager.Instance.PlayAudio(GameInstance.GameIns.uISoundManager.UIPop(), 0.4f);
        yield return CoroutneManager.waitForzerotwo;

        clickable = true;

        float f = 0;
        while(f <= 1)
        {
            Vector3 res = Vector3.SmoothDamp(rect.position, origin, ref velocity, 0.2f);
            rect.position = res;
            f += Time.deltaTime / 3;
            yield return null;
        }

        clickerText.gameObject.SetActive(false);
        clickable = false;
        rect.position = origin;

        trashcanImage.sprite = AssetLoader.loadedSprites[AssetLoader.spriteAssetKeys[5005].ID];
        SoundManager.Instance.PlayAudio(GameInstance.GameIns.gameSoundManager.Chest(0), 0.2f);
        yield return null;

        Vector2 v = Vector2.zero;
        f = 0;
        Vector2 scale = rect.sizeDelta;
        Vector2 target = new Vector2(500, 500);
        while (f <= 1)
        {
            Vector2 next = Vector2.SmoothDamp(scale, target, ref v, 0.02f);
            rect.sizeDelta = next;
            f += Time.deltaTime / 0.2f;
            yield return null;
        }
        yield return CoroutneManager.waitForzeroone;
        SoundManager.Instance.PlayAudio(GameInstance.GameIns.gameSoundManager.Chest(1), 0.2f);
        f = 0;
        Vector2 scale2 = rect.sizeDelta;
        Vector2 target2 = new Vector2(250, 250);
        while (f <= 1)
        {
            Vector2 next = Vector2.SmoothDamp(scale2, target2, ref v, 0.02f);
            rect.sizeDelta = next;
            f += Time.deltaTime / 0.2f;
            yield return null;
        }
      //  yield return null;
          yield return CoroutneManager.waitForzerotwo;
        SoundManager.Instance.PlayAudio(GameInstance.GameIns.gameSoundManager.Chest(2), 0.2f);
        f = 0;
        Vector2 scale3 = rect.sizeDelta;
        Vector2 target3 = new Vector2(4000,4000);
        while (f <= 1)
        {
            Vector2 next = Vector2.Lerp(scale3, target3, f);
            rect.sizeDelta = next;
            f += Time.deltaTime / 0.1f;
            yield return null;
        }
     //   yield return CoroutneManager.waitForzerothree;
        rect.sizeDelta = new Vector2(1000,1000);

        SoundManager.Instance.PlayAudio(GameInstance.GameIns.gameSoundManager.Chest(3 ), 0.2f);
        trashcanImage.gameObject.SetActive(false);
        ShowResult();

        yield return CoroutneManager.waitForone;

        animator.SetTrigger(AnimationKeys.popup_close);


        yield return CoroutneManager.waitForzerotwo;
        EndClicker();
    }

    void ShowResult()
    {
        resultImage.gameObject.SetActive(true);
        if (clickNum > 30)
        {
            resultImage.sprite = loadedSprites[spriteAssetKeys[5004].ID];
        }
        else if (clickNum > 15)
        {
            resultImage.sprite = loadedSprites[spriteAssetKeys[5003].ID];

        }
        else
        {
            resultImage.sprite = loadedSprites[spriteAssetKeys[5002].ID];

        }
    }

    void EndClicker()
    {
       
        GameInstance.GameIns.applianceUIManager.Reward(clickNum);
        clickNum = 0;
        resultImage.gameObject.SetActive(false);
        trashcanImage.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

}
