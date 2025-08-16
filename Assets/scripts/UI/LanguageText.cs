using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageText : MonoBehaviour
{
    public int id;
    public bool noUpdateWhenStarting;
    TMP_Text text;
    Language language;
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        App.languageTexts.Enqueue(this);
    }

    private void Start()
    {
        if(App.languages.ContainsKey(id) && !noUpdateWhenStarting) text.text = App.languages[id].text;

    }

    public void ChangeText()
    {
        if (App.languages.ContainsKey(id)) text.text = App.languages[id].text;
    }
}
