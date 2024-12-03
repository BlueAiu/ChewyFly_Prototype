using System;
using UnityEngine;

public class OptionValues : MonoBehaviour
{
    // 最小値と最大値を設定
    public const int soundMinValue = 0;
    public const int soundMaxValue = 100;
    public const int sensitivityMinValue = -5;
    public const int sensitivityMaxValue = 5;

    private static int bgmValue = 50;
    private static int seValue = 50;
    private static int jumpSensitivity = 0;
    private static int cameraSensitivity = 0;


    [SerializeField] SoundManager soundManager;
    private void Start()
    {
        if (soundManager == null) soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
    public int BGMValue
    {
        get { return bgmValue; }
        set
        {
            // 値を制限
            bgmValue = Math.Clamp(value, soundMinValue, soundMaxValue);
            if (soundManager != null)
                soundManager.SetBGMVolume(this);
        }
    }
    public int SEValue
    {
        get { return seValue; }
        set
        {
            seValue = Math.Clamp(value, soundMinValue, soundMaxValue);
            if (soundManager != null)
                soundManager.SetSEVolume(this);
        }
    }
    public int JumpSensitivity
    {
        get { return jumpSensitivity; }
        set
        {
            jumpSensitivity = Mathf.Clamp(value, sensitivityMinValue, sensitivityMaxValue);
        }
    }
    public int CameraSensitivity
    {
        get { return cameraSensitivity; }
        set
        {
            cameraSensitivity = Math.Clamp(value, sensitivityMinValue, sensitivityMaxValue);
        }
    }
    
    public void StaticValueCheck()
    {
        Debug.Log("bgmは" + BGMValue + "、seは" + SEValue + "、jump感度は" + JumpSensitivity + "、camera感度は" + CameraSensitivity);
    }

    public float GetCameraSensitivityRatio()//最小値を0、最大値を1としたときの値の割合
    {
        return (float)(CameraSensitivity - sensitivityMinValue) / (sensitivityMaxValue - sensitivityMinValue);
    }
    public float GetJumpSensitivityRatio()//最小値を0、最大値を1としたときの値の割合
    {
        return (float)(JumpSensitivity - sensitivityMinValue) / (sensitivityMaxValue - sensitivityMinValue);
    }
}
