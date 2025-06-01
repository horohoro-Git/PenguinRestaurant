using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SoundManager : MonoBehaviour
{
    protected static AudioSource audio;

    public static AudioSource AudioSource { get { if (audio == null) audio = FindObjectOfType<AudioSource>(); return audio; }}

    public static void ResetAudio()
    {
        audio = null;
    }
}
