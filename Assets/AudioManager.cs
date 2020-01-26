using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioClip[] music;
    public AudioSource musicAudioSource;

    public void PlayTutorialMusic()
    {
        musicAudioSource.clip = music[0];
    }

    public void PlayGameMusic()
    {
        musicAudioSource.clip = music[1];
    }
}
