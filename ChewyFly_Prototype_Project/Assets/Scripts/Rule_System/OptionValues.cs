using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionValues : MonoBehaviour
{
    // �ŏ��l�ƍő�l��ݒ�
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
            // �l�𐧌�
            bgmValue = Math.Max(soundMinValue, Math.Min(soundMaxValue, value));
        }
    }
    public int SEValue
    {
        get { return seValue; }
        set
        {
            seValue = Math.Max(soundMinValue, Math.Min(soundMaxValue, value));
        }
    }
    public int JumpSensibility
    {
        get { return jumpSensibility; }
        set
        {
            jumpSensibility = Math.Max(sensibilityMinValue, Math.Min(sensibilityMaxValue, value));
        }
    }
    public int CameraSensibility
    {
        get { return cameraSensibility; }
        set
        {
            cameraSensibility = Math.Max(sensibilityMinValue, Math.Min(sensibilityMaxValue, value));
        }
    }
    
    public void StaticValueCheck()
    {
        Debug.Log("bgm��" + BGMValue + "�Ase��" + SEValue + "�Ajump���x��" + JumpSensibility + "�Acamera���x��" + CameraSensibility);
    }
}
