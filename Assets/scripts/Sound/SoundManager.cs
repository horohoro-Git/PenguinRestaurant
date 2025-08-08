using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class SoundManager : MonoBehaviour 
{
    static SoundManager soundManager;
    public static SoundManager Instance { get; set; }
    
    public Queue<AudioSource> audioSources = new Queue<AudioSource>();
    public Dictionary<int, AudioSource> audioWithKey = new Dictionary<int, AudioSource>();
    List<AudioSource> restaurantAudios = new List<AudioSource>();

    public Dictionary<AudioClip, int> currentPlayingAudios = new Dictionary<AudioClip, int>();  
    public List<AudioSource> currentPlayingAudioSouces = new List<AudioSource>();  
    GameObject audios;

    [Range(5, 1000)]
    public float min;
    [Range(5, 1000)]
    public float max;
    private void Awake()
    {
        Instance = this;
        audios = new GameObject();
        audios.name = "Audios";

        for (int i = 0; i < 1000; i++)
        {
            GameObject g = new GameObject();
            AudioSource a = g.AddComponent<AudioSource>();
            a.playOnAwake = false;
            audioSources.Enqueue(a);
            g.transform.SetParent(audios.transform);
        }

    }
  

    public void PlayAudio(AudioClip clip, float volume)
    {
        PlayAudioAsync(clip, volume, App.GlobalToken).Forget();
    }
    public bool PlayAudioWithKey(AudioClip clip, float volume, int key)
    {
        bool exists = AudioStop(key);
        AudioSource audio;
        if (audioSources.Count > 0)
        {
            audio = audioSources.Dequeue();
            currentPlayingAudioSouces.Add(audio);
        }
        else
        {
            GameObject newAudio = new GameObject();
            audio = newAudio.AddComponent<AudioSource>();
            newAudio.transform.SetParent(audios.transform);
            currentPlayingAudioSouces.Add(audio);
        }

        audio.spatialBlend = 0;
        audio.clip = clip;
        audio.volume = App.gameSettings.soundEffects ? 1 : 0; //volume;
        audio.Play();
        audioWithKey[key] = audio;

        return exists;
    }
    public bool AudioStop(int key)
    {
        if(audioWithKey.ContainsKey(key))
        {
            AudioSource audio = audioWithKey[key];
            audioWithKey.Remove(key);  
            audio.Stop();
            if (audioSources.Count > 1000)
            {
                currentPlayingAudioSouces.Remove(audio);
                Destroy(audio);
            }
            else
            {
                currentPlayingAudioSouces.Remove(audio);
                audioSources.Enqueue(audio);
            }
            return true;
        }
        else
        {
            return false;
        }
    }



    async UniTask PlayAudioAsync(AudioClip clip, float volume, CancellationToken cancellationToken = default)
    {
        AudioSource audio;
        if (audioSources.Count > 0)
        {
            audio = audioSources.Dequeue();
            currentPlayingAudioSouces.Add(audio);
        }
        else
        {
            GameObject newAudio = new GameObject();
            audio = newAudio.AddComponent<AudioSource>();
            newAudio.transform.SetParent(audios.transform);
            currentPlayingAudioSouces.Add(audio);
        }

        audio.spatialBlend = 0;
        audio.clip = clip;
        if (currentPlayingAudios.ContainsKey(clip))
        {
            currentPlayingAudios[clip]++;
        }
        else
        {
            currentPlayingAudios[clip] = 1;
        }

        audio.volume = (App.gameSettings.soundEffects ? 1f : 0) / currentPlayingAudios[clip]; //volume / currentPlayingAudios[clip];
        audio.Play();
        float length = clip.length;

        await UniTask.Delay((int)(length * 1000), DelayType.UnscaledDeltaTime, cancellationToken: cancellationToken);
        audio.Stop();
        currentPlayingAudios[clip]--;
        if (audioSources.Count > 1000)
        {
            currentPlayingAudioSouces.Remove(audio);
            Destroy(audio);
        }
        else
        {
            currentPlayingAudioSouces.Remove(audio);
            audioSources.Enqueue(audio);
        }
    }
   
    public void PlayAudio3D(AudioClip clip, float volume, float maxDistance, float minDistance, Vector3 pos)
    {
        PlayAudio3DAsync(clip, volume, maxDistance, minDistance, pos, App.GlobalToken).Forget();
    }
    async UniTask PlayAudio3DAsync(AudioClip clip, float volume, float maxDistance, float minDistance, Vector3 pos, CancellationToken cancellationToken = default)
    {
        AudioSource audio;
        if (audioSources.Count > 0)
        {
            audio = audioSources.Dequeue();
            currentPlayingAudioSouces.Add(audio);
        }
        else
        {
            GameObject newAudio = new GameObject();
            audio = newAudio.AddComponent<AudioSource>();
            newAudio.transform.SetParent(audios.transform);
            currentPlayingAudioSouces.Add(audio);
        }

        audio.transform.position = pos;
        audio.spatialBlend = 1;
        audio.maxDistance = max; //maxDistance;
        audio.minDistance = min;//minDistance;
        audio.clip = clip;
        if (currentPlayingAudios.ContainsKey(clip))
        {
            currentPlayingAudios[clip]++;
        }
        else
        {
            currentPlayingAudios[clip] = 1;
        }
        audio.volume = (App.gameSettings.soundEffects ? 1f : 0) / currentPlayingAudios[clip]; //volume / currentPlayingAudios[clip];
        audio.Play();

        restaurantAudios.Add(audio);
        float length = clip.length;

        await UniTask.Delay((int)(length * 1000), DelayType.UnscaledDeltaTime, cancellationToken: cancellationToken);
        currentPlayingAudios[clip]--;
        audio.Stop();
        restaurantAudios.Remove(audio);
        if (audioSources.Count > 1000)
        {
            currentPlayingAudioSouces.Remove(audio);
            Destroy(audio);
        }
        else
        {
            currentPlayingAudioSouces.Remove(audio);
            audioSources.Enqueue(audio);

        }

    }

    public void RestaurantSoundsOnoff(bool on)
    {
        if(on)
        {
            for (int i = 0; i < restaurantAudios.Count; i++)
            {
                restaurantAudios[i].UnPause();
            }
        }
        else
        {
            for (int i = 0; i < restaurantAudios.Count; i++)
            {

                restaurantAudios[i].Pause();
            }

        }
        for (int i = 0; i < GameInstance.GameIns.workSpaceManager.foodMachines.Count; i++)
        {
            GameInstance.GameIns.workSpaceManager.foodMachines[i].MachineSound(!on);
        }
    }
}
