using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip[] music;
    public AudioSource musicAudioSource;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayTutorialMusic()
    {
        musicAudioSource.clip = music[0];
        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }

    public void StopTutorialMusic()
    {
        musicAudioSource.loop = false;
    }

    public void PlayGameMusic()
    {
        musicAudioSource.clip = music[1];
        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }
}
