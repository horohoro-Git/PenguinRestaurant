using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageText : MonoBehaviour
{
    public int id;
    TMP_Text text;
    Language language;
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        App.languageTexts.Enqueue(this);
    }

    private void Start()
    {
        text.text = App.languages[id].text;

    }

    public void ChangeText()
    {
        text.text = App.languages[id].text;
    }
}
