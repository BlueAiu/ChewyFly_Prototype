using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionManager : OptionValues
{
    [Header("ボタンUI")]
    [SerializeField] Button bgmButton;
    [SerializeField] Button seButton;
    [SerializeField] Button jumpSensibilityButton;
    [SerializeField] Button cameraSensibilityButton;

    [Header("スライダーUI")]
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider seSlider;
    [SerializeField] Slider jumpSensibilitySlider;
    [SerializeField] Slider cameraSensibilitySlider;

    [SerializeField] EventSystem eventSystem;//現在選択しているボタンの取得に必要
    private Button previousButton;
    private Slider nowSlider;
    private int previousValue;//設定を変更しない場合この値に戻す

    public bool OnUseSlider { get; private set; } = false;
    bool isSelectSlider;
    InputScript input;

    LoadSceneManager LoadSceneManager;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputScript>();

        //最大値、最小値に初期化
        bgmSlider.maxValue = soundMaxValue; bgmSlider.minValue = soundMinValue;
        seSlider.maxValue = soundMaxValue; seSlider.minValue = soundMinValue;
        jumpSensibilitySlider.maxValue = sensibilityMaxValue; jumpSensibilitySlider.minValue = sensibilityMinValue;
        cameraSensibilitySlider.maxValue = sensibilityMaxValue; cameraSensibilitySlider.minValue = sensibilityMinValue;

        OpenOption();

        LoadSceneManager = GetComponent<LoadSceneManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (OnUseSlider)
        {
            if (isSelectSlider)//ボタンを押すタイミングとUpdateのタイミングが合わないのでここでスライダーを有効化する
            {
                nowSlider.enabled = true;
                nowSlider.Select();
                isSelectSlider = false;
            }
            else
            {
                if (input.isAButton())
                {
                    ActiveOptionButtons(true);
                    nowSlider = null;
                }
                else if (input.isBButton())//Bボタンを押したなら前の値に戻す
                {
                    nowSlider.value = previousValue;
                    ActiveOptionButtons(true);
                    nowSlider = null;
                }
            }
        }
        else//ボタン選択状態
        {
            if (input.isBButton())
            {
                LoadSceneManager.LoadSceneName("TitleScene");
            }
        }
    }
    public void OpenOption()//最初にオプションを開く場合の処理
    {
        nowSlider = null;
        previousButton = null;
        ActiveOptionButtons(true);

        bgmSlider.SetValueWithoutNotify(BGMValue);//イベントを発生させずにスライダーの値を変更
        seSlider.SetValueWithoutNotify(SEValue);
        jumpSensibilitySlider.SetValueWithoutNotify(JumpSensibility);
        cameraSensibilitySlider.SetValueWithoutNotify(CameraSensibility);

        //ボタンだけを選択できる状態にするためスライダーを止める
        bgmSlider.enabled = false; seSlider.enabled = false;
        jumpSensibilitySlider.enabled = false; cameraSensibilitySlider.enabled = false;
    }
    void ActiveOptionButtons(bool active)//左にあるボタンたちを有効、非有効化する
    {
        bgmButton.enabled = active;
        seButton.enabled = active;
        jumpSensibilityButton.enabled = active;
        cameraSensibilityButton.enabled = active;
        if (nowSlider != null && active) nowSlider.enabled = false;
        OnUseSlider = !active;

        if (active)
        {
            if (previousButton == null)
                bgmButton.Select();
            else
                previousButton.Select();
        }
    }
    public void SelectSlider(Slider slider)//ボタンの左にあるスライダーを選択状態にする(Buttonが呼ぶ)
    {
        isSelectSlider = true;
        nowSlider = slider;
        previousButton = eventSystem.currentSelectedGameObject.gameObject.GetComponent<Button>();
        previousValue = (int)slider.value;

        ActiveOptionButtons(false);
    }

    //それぞれの値を変更する
    public void SetBGMValue(Slider slider)
    {
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
