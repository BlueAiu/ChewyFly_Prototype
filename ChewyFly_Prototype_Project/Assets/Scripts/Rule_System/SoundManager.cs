using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    AudioSource bgmAudioSource;
    [SerializeField]
    AudioSource seAudioSource;

    void Start()
    {
        OptionValues option = FindObjectOfType<OptionValues>();//âπó Çèâä˙âª
        SetBGMVolume(option);
        SetSEVolume(option);
    }

    public void PlayBGM(AudioClip clip, float skipTime = 0f)
    {
        bgmAudioSource.clip = clip;

        if (clip == null)
        {
            return;
        }

        bgmAudioSource.clip = clip;
        bgmAudioSource.time = skipTime;
        bgmAudioSource.Play();
    }

    public void PlaySE(AudioClip clip, float skipTime = 0f)
    {
        if (clip == null)
        {
            return;
        }

        seAudioSource.clip = clip;
        seAudioSource.time = skipTime;
        seAudioSource.Play();
    }

    public void SetBGMVolume(OptionValues optionValues)
    {
        if (optionValues == null) return;
        bgmAudioSource.volume = (float)optionValues.BGMValue / (OptionValues.soundMaxValue - OptionValues.soundMinValue);
    }

    public void SetSEVolume(OptionValues optionValues)
    {
        if (optionValues == null) return;
        seAudioSource.volume = (float)optionValues.SEValue / (OptionValues.soundMaxValue - OptionValues.soundMinValue);
    }
}
