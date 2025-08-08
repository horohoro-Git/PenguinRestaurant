using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSetting : Settings
{
    public Animator engAnimator;
    public Animator korAnimator;
    public Button engBtn;
    public Button korBtn;
    public RectTransform languageBorder;
    public TMP_Text result;
    TMP_Text eng_Text;
    TMP_Text kor_Text;
    Coroutine coroutine;
    bool isOpen;

    private void Awake()
    {
        eng_Text = engBtn.GetComponentInChildren<TMP_Text>();
        kor_Text = korBtn.GetComponentInChildren<TMP_Text>();
        int ids = 10200 + (int)App.gameSettings.language;
        result.GetComponent<LanguageText>().id = ids;
        result.text = App.languages[ids].text;
    }
    public void Click()
    {
        if (!isOpen)
        {
            isOpen = true;
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(OpenAnimation());
            engBtn.onClick.AddListener(() =>
            {
                
                App.gameSettings.language = Language.ENG;
                App.languages.Clear();
                TextAsset lang = null;
                result.GetComponent<LanguageText>().id = 10200 + (int)App.gameSettings.language;
                if (App.gameSettings.language == Language.KOR) lang = Resources.Load<TextAsset>("language_kor");
                else lang = Resources.Load<TextAsset>("language_eng");

                List<LanguageScript> l = JsonConvert.DeserializeObject<List<LanguageScript>>(lang.text);
                App.languages.Clear();
                for (int i = 0; i < l.Count; i++) App.languages[l[i].id] = l[i];
                App.RefreshUI();
                SaveLoadSystem.SaveGameSettings(App.gameSettings);
            });
            korBtn.onClick.AddListener(() =>
            {
                App.gameSettings.language = Language.KOR;
                App.languages.Clear();
                TextAsset lang = null;
                result.GetComponent<LanguageText>().id = 10200 + (int)App.gameSettings.language;
                if (App.gameSettings.language == Language.KOR) lang = Resources.Load<TextAsset>("language_kor");
                else lang = Resources.Load<TextAsset>("language_eng");

                List<LanguageScript> l = JsonConvert.DeserializeObject<List<LanguageScript>>(lang.text);
                App.languages.Clear();
                for (int i = 0; i < l.Count; i++) App.languages[l[i].id] = l[i];
                App.RefreshUI();
                SaveLoadSystem.SaveGameSettings(App.gameSettings);
            });
        }
        else
        {
            isOpen = false;
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(CloseAnimation());
            engBtn.onClick.RemoveAllListeners();
            korBtn.onClick.RemoveAllListeners();
        }
    }
    IEnumerator OpenAnimation()
    {
        animator.SetInteger("state", 1);
        yield return new WaitForSecondsRealtime(0.2f);

        engAnimator.SetInteger("state", 1);
        yield return new WaitForSecondsRealtime(0.1f);
        korAnimator.SetInteger("state", 1);
    }

    IEnumerator CloseAnimation()
    {
        korAnimator.SetInteger("state", 2);
        yield return new WaitForSecondsRealtime(0.1f);
        engAnimator.SetInteger("state", 2);

        yield return new WaitForSecondsRealtime(0.2f);
        animator.SetInteger("state", 2);

    }
    private void OnDisable()
    {
        if (isOpen)
        {
            isOpen = false;
            eng_Text.fontSize = 0;
            kor_Text.fontSize = 0;
            engBtn.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            korBtn.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            languageBorder.sizeDelta = Vector2.zero;
        }
    }
}
