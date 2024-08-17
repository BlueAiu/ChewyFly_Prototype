using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [SerializeField] Button bgmButton;
    [SerializeField] Button seButton;
    [SerializeField] Button jumpSensibilityButton;
    [SerializeField] Button cameraSensibilityButton;

    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider seSlider;
    [SerializeField] Slider jumpSensibilitySlider;
    [SerializeField] Slider cameraSensibilitySlider;

    [SerializeField] EventSystem eventSystem;
    private Button previousButton;
    private Slider nowSlider;
    private int previousValue;

    // 最小値と最大値を設定
    const int soundMinValue = 0;
    const int soundMaxValue = 100;
    const int sensibilityMinValue = -5;
    const int sensibilityMaxValue = 5;

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

    public bool OnUseSlider { get; private set; } = false;
    bool isSelectSlider;
    InputScript input;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputScript>();

        //最大値、最小値に初期化
        bgmSlider.maxValue = soundMaxValue; bgmSlider.minValue = soundMinValue;
        seSlider.maxValue = soundMaxValue; seSlider.minValue = soundMinValue;
        jumpSensibilitySlider.maxValue = sensibilityMaxValue; jumpSensibilitySlider.minValue = sensibilityMinValue;
        cameraSensibilitySlider.maxValue = sensibilityMaxValue; cameraSensibilitySlider.minValue = sensibilityMinValue;
        //Sliderの値を設定
        /*
        bgmSlider.value = BGMValue;
        seSlider.value = SEValue;
        jumpSensibilitySlider.value = JumpSensibility;
        cameraSensibilitySlider.value = CameraSensibility;*/
    }
    // Update is called once per frame
    void Update()
    {
        if (isSelectSlider)
        {
            nowSlider.enabled = true;
            nowSlider.Select();
            isSelectSlider = false;
        }
        else if (OnUseSlider)//スライダー選択状態
        {
            if (input.isAButton())
            {
                ActiveOptionButtons(true);
                nowSlider = null;
            }
            else if (input.isBButton())
            {
                nowSlider.value = previousValue;
                ActiveOptionButtons(true);
                nowSlider = null;
            }
        }
    }
    public void OpenOption()
    {
        nowSlider = null;
        previousButton = null;
        ActiveOptionButtons(true);

        bgmSlider.SetValueWithoutNotify(BGMValue);//イベントを発生させずにスライダーの値を変更
        seSlider.SetValueWithoutNotify(SEValue);
        jumpSensibilitySlider.SetValueWithoutNotify(JumpSensibility);
        cameraSensibilitySlider.SetValueWithoutNotify(CameraSensibility);

        bgmSlider.Rebuild(CanvasUpdate.Layout); // グラフィックを値の部分にいくように再描画
        seSlider.Rebuild(CanvasUpdate.Layout);
        jumpSensibilitySlider.Rebuild(CanvasUpdate.Layout);
        cameraSensibilitySlider.Rebuild(CanvasUpdate.Layout);
    }
    void ActiveOptionButtons(bool active)
    {
        bgmButton.enabled = active;
        seButton.enabled = active;
        jumpSensibilityButton.enabled = active;
        cameraSensibilityButton.enabled = active;
        if (nowSlider != null) nowSlider.enabled = !active;
        OnUseSlider = !active;

        if (active)
        {
            if (previousButton == null)
                bgmButton.Select();
            else
                previousButton.Select();
        }
    }
    public void SelectSlider(Slider slider)
    {
        isSelectSlider = true;
        nowSlider = slider;
        previousButton = eventSystem.currentSelectedGameObject.gameObject.GetComponent<Button>();
        previousValue = (int)slider.value;

        ActiveOptionButtons(false);
    }
    public void SetValueV()
    {
        bgmSlider.value = 5;
    }
    public void SetBGMValue(Slider slider)
    {
        Debug.Log("callde");
        BGMValue = (int)slider.value;
    }
    public void SetSEValue(Slider slider)
    {
        SEValue = (int)slider.value;
    }
    public void SetJumpSensibility(Slider slider)
    {
        JumpSensibility = (int)slider.value;
    }
    public void SetCameraSensibility(Slider slider)
    {
        CameraSensibility = (int)slider.value;
    }
}
