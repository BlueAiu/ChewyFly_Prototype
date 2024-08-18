using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionValues : MonoBehaviour
{
    // 最小値と最大値を設定
    protected const int soundMinValue = 0;
    protected const int soundMaxValue = 100;
    protected const int sensibilityMinValue = -5;
    protected const int sensibilityMaxValue = 5;

    private static int bgmValue = 50;
    private static int seValue = 50;
    private static int jumpSensibility = 0;
    private static int cameraSensibility = 0;

    public int BGMValue
    {
        get { return bgmValue; }
        set
        {
            // 値を制限
            bgmValue = Math.Clamp(value, soundMinValue, soundMaxValue);
        }
    }
    public int SEValue
    {
        get { return seValue; }
        set
        {
            seValue = Math.Clamp(value, soundMinValue, soundMaxValue);
        }
    }
    public int JumpSensibility
    {
        get { return jumpSensibility; }
        set
        {
            jumpSensibility = Mathf.Clamp(value, sensibilityMinValue, sensibilityMaxValue);
        }
    }
    public int CameraSensibility
    {
        get { return cameraSensibility; }
        set
        {
            cameraSensibility = Math.Clamp(value, sensibilityMinValue, sensibilityMaxValue);
        }
    }
    
    public void StaticValueCheck()
    {
        Debug.Log("bgmは" + BGMValue + "、seは" + SEValue + "、jump感度は" + JumpSensibility + "、camera感度は" + CameraSensibility);
    }
}
