using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : OptionValues
{
    [Header("�{�^��Image")]
    [SerializeField] Image bgmButton;
    [SerializeField] Image seButton;
    [SerializeField] Image jumpSensitivityButton;
    [SerializeField] Image cameraSensitivityButton;
    [SerializeField] Sprite SelectSprite;
    [SerializeField] Sprite NoneSelectSprite;


    [Header("�X���C�_�[UI")]
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider seSlider;
    [SerializeField] Slider jumpSensitivitySlider;
    [SerializeField] Slider cameraSensitivitySlider;
    InputScript input;

    [Tooltip("�X���C�_�[�̒l�������ɕ�����H")]
    [SerializeField] int sliderValueDivideNum;

    LoadSceneManager LoadSceneManager;

    [SerializeField] TextMeshProUGUI resetCountText;
    [Tooltip("���̎��ԃ{�^������������S���l�����Z�b�g")]
    [SerializeField] float resetPushTime;
    bool isValueChanged;
    float _resetTimer = 0f;
    float ResetTimer
    {
        get
        {
            return _resetTimer;
        }
        set
        {
            if (value == 0f)
            {
                resetCountText.gameObject.SetActive(false);
            }
            else if (_resetTimer == 0f)//�ŏ��ɉ�����
            {
                resetCountText.gameObject.SetActive(true);
                resetCountText.text = resetCountNum.ToString();
            }

            _resetTimer = value;
            if (_resetTimer > 0f && _resetTimer < resetPushTime)
            {
                resetCountText.text = ((int)(((resetPushTime - _resetTimer) / resetPushTime) * resetCountNum) + 1).ToString();
            }
        }
    }
    const int resetCountNum = 3;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputScript>();

        //�ő�l�A�ŏ��l�ɏ�����
        bgmSlider.minValue = 0; bgmSlider.maxValue = sliderValueDivideNum;
        seSlider.minValue = 0; seSlider.maxValue = sliderValueDivideNum;
        jumpSensitivitySlider.minValue = 0; jumpSensitivitySlider.maxValue = sliderValueDivideNum;
        cameraSensitivitySlider.minValue = 0; cameraSensitivitySlider.maxValue = sliderValueDivideNum;

        OpenOption();
        isValueChanged = true;
        ResetTimer = 0f;

        LoadSceneManager = GetComponent<LoadSceneManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (input.isBButton())
        {
            LoadSceneManager.LoadSceneName("TitleScene");//�^�C�g���ɖ߂�
        }
        else
            ResetUpdate();
    }
    public void OpenOption()
    {
        SetSliderValue(bgmSlider, BGMValue);
        SetSliderValue(seSlider, SEValue);
        SetSliderValue(jumpSensitivitySlider, JumpSensitivity);
        SetSliderValue(cameraSensitivitySlider, CameraSensitivity);

        bgmSlider.Select();
        SliderSelect(bgmButton);
    }
    void SetSliderValue(Slider slider, int value)//�C�x���g�𔭐��������ɃX���C�_�[�̒l��ύX
    {
        if (slider == bgmSlider || slider == seSlider)
        {
            slider.SetValueWithoutNotify(sliderValueDivideNum * (float)((float)(value - soundMinValue) / (soundMaxValue - soundMinValue)));
        }
        else if (slider == jumpSensitivitySlider || slider == cameraSensitivitySlider)
        {
            slider.SetValueWithoutNotify(sliderValueDivideNum * (float)((float)(value - sensitivityMinValue) / (sensitivityMaxValue - sensitivityMinValue)));
        }
    }
    void ResetUpdate()//Update�ŌĂ΂��l���Z�b�g�̏���
    {
        if (isValueChanged &&
            (input.isLeftTrigger() || input.isLeftShoulder()) && (input.isRightTrigger() || input.isRightShoulder()))//�E���̃{�^���������������烊�Z�b�g
        {
            ResetTimer += Time.deltaTime;
            if (ResetTimer > resetPushTime)
            {
                ResetAllSliderValue();
            }
        }
        else if (ResetTimer != 0f)
        {
            ResetTimer = 0f;
        }
    }
    void ResetAllSliderValue()//slider�̒l�����ׂă��Z�b�g
    {
        bgmSlider.value = sliderValueDivideNum / 2f;//�X���C�_�[�𓮂����Βl�͏���ɓ���
        seSlider.value = sliderValueDivideNum / 2f;
        jumpSensitivitySlider.value = sliderValueDivideNum / 2f;
        cameraSensitivitySlider.value = sliderValueDivideNum / 2f;
        ResetTimer = 0f;
        isValueChanged = false;
    }

    //���ꂼ��̒l��ύX����
    public void SetBGMValue(Slider slider)
    {
        BGMValue = soundMinValue + (int)((soundMaxValue - soundMinValue) * (slider.value / sliderValueDivideNum));
        isValueChanged = true;
    }
    public void SetSEValue(Slider slider)
    {
        SEValue = soundMinValue + (int)((soundMaxValue - soundMinValue) * (slider.value / sliderValueDivideNum));
        isValueChanged = true;
    }
    public void SetJumpSensibility(Slider slider)
    {
        JumpSensitivity = sensitivityMinValue + (int)((sensitivityMaxValue - sensitivityMinValue) * (slider.value / sliderValueDivideNum));
        isValueChanged = true;
    }
    public void SetCameraSensibility(Slider slider)
    {
        CameraSensitivity = sensitivityMinValue + (int)((sensitivityMaxValue - sensitivityMinValue) * (slider.value / sliderValueDivideNum));
        isValueChanged = true;
    }
    public void SliderSelect(Image sliderButtonImage)//button��I����Ԃ�Sprite�ɕς���
    {
        bgmButton.sprite = NoneSelectSprite;
        seButton.sprite = NoneSelectSprite;
        jumpSensitivityButton.sprite = NoneSelectSprite;
        cameraSensitivityButton.sprite = NoneSelectSprite;

        sliderButtonImage.sprite = SelectSprite;
    }
}
