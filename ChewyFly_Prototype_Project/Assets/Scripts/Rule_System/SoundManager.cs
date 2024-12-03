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

    public void PlayBGM(AudioClip clip)
    {
        bgmAudioSource.clip = clip;

        if (clip == null)
        {
            return;
        }

        bgmAudioSource.clip = clip;
        bgmAudioSource.pitch = 1.0f;
        bgmAudioSource.Play();
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        seAudioSource.PlayOneShot(clip);
    }

    public void SetBGMVolume(OptionValues optionValues)
    {
        if (optionValues == null) return;
        bgmAudioSource.volume = (float)(optionValues.BGMValue - OptionValues.soundMinValue) / (OptionValues.soundMaxValue - OptionValues.soundMinValue);
    }

    public void SetSEVolume(OptionValues optionValues)
    {
        if (optionValues == null) return;
        seAudioSource.volume = (float)(optionValues.SEValue - OptionValues.soundMinValue) / (OptionValues.soundMaxValue - OptionValues.soundMinValue);
    }
    public void SetBGMPitch(float pitch)
    {
        bgmAudioSource.pitch = pitch;
    }
}
