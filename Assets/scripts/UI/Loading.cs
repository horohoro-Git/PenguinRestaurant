using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    AudioListener audioSouce;
    Camera cam;
    public Image image;
    public TMP_Text text;

    float latestTimer;
    bool done;

    Queue<string> loadingQ = new();
    string loading;
    private void Awake()
    {
        loadingQ.Enqueue(".");
        loadingQ.Enqueue("..");
        loadingQ.Enqueue("...");
    }

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
        audioSouce = cam.GetComponent<AudioListener>();
        SceneManager.MoveGameObjectToScene(cam.gameObject, App.scenes["LoadingScene"]);
    }
    private void Update()
    {
        if (!done)
        {
            if (latestTimer < Time.time)
            {
                latestTimer = Time.time + 0.2f;
                LoadingText();
            }
        }
    }

    void LoadingText()
    {
        string q = loadingQ.Dequeue();
        string l = loading + q;
        loadingQ.Enqueue(q);

        text.text = l;
    }
    public void ChangeText(string t)
    {
        loading = t;
    }
    public void LoadingComplete()
    {
        done = true;
        text.text = App.gameSettings.language == Language.KOR ? "로딩 완료!" : "Loading Complete!";

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSecondsRealtime(1f);
        cam.enabled = false;
        float t = 0;
        float a = 1;
        while (t < 2)
        {
            t += Time.unscaledDeltaTime;
            a = (1 * ((2 - t) / 2));
            image.color = new Color(1,1, 1, a);
            text.color = new Color(1,1, 1, a);  
            yield return null;
        }
        image.color = new Color(1, 1, 1,0);

        image.raycastTarget = false;

        audioSouce.enabled = false;
        GameInstance.GameIns.playerCamera.audioListener.enabled = true;

        App.GlobalToken.ThrowIfCancellationRequested();
        if (App.currentScene == SceneState.Restaurant) GameInstance.GameIns.bgMSoundManager.BGMChange(901000, 0.4f);

        GameInstance.GameIns.restaurantManager.Tutorial();
        App.UnloadAsync("LoadingScene");
    }
}
