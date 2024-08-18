using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionManager : OptionValues
{
    [Header("�{�^��UI")]
    [SerializeField] Button bgmButton;
    [SerializeField] Button seButton;
    [SerializeField] Button jumpSensibilityButton;
    [SerializeField] Button cameraSensibilityButton;

    [Header("�X���C�_�[UI")]
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider seSlider;
    [SerializeField] Slider jumpSensibilitySlider;
    [SerializeField] Slider cameraSensibilitySlider;

    [SerializeField] EventSystem eventSystem;//���ݑI�����Ă���{�^���̎擾�ɕK�v
    private Button previousButton;
    private Slider currentSlider;
    private int previousValue;//�ݒ��ύX���Ȃ��ꍇ���̒l�ɖ߂�

    public bool OnUseSlider { get; private set; } = false;
    bool isSelectSlider;
    InputScript input;

    LoadSceneManager LoadSceneManager;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputScript>();

        //�ő�l�A�ŏ��l�ɏ�����
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
            if (isSelectSlider)//�{�^���������^�C�~���O��Update�̃^�C�~���O������Ȃ��̂ł����ŃX���C�_�[��L��������
            {
                currentSlider.enabled = true;
                currentSlider.Select();
                isSelectSlider = false;
            }
            else
            {
                if (input.isAButton())
                {
                    ActiveOptionButtons(true);
                    currentSlider = null;
                }
                else if (input.isBButton())//B�{�^�����������Ȃ�O�̒l�ɖ߂�
                {
                    currentSlider.value = previousValue;
                    ActiveOptionButtons(true);
                    currentSlider = null;
                }
            }
        }
        else//�{�^���I�����
        {
            if (input.isBButton())
            {
                LoadSceneManager.LoadSceneName("TitleScene");//�^�C�g���ɖ߂�
            }
        }
    }
    public void OpenOption()
    {
        currentSlider = null;
        previousButton = null;
        ActiveOptionButtons(true);

        bgmSlider.SetValueWithoutNotify(BGMValue);//�C�x���g�𔭐��������ɃX���C�_�[�̒l��ύX
        seSlider.SetValueWithoutNotify(SEValue);
        jumpSensibilitySlider.SetValueWithoutNotify(JumpSensibility);
        cameraSensibilitySlider.SetValueWithoutNotify(CameraSensibility);

        //�{�^��������I���ł����Ԃɂ��邽�߃X���C�_�[���~�߂�
        bgmSlider.enabled = false; seSlider.enabled = false;
        jumpSensibilitySlider.enabled = false; cameraSensibilitySlider.enabled = false;
    }
    void ActiveOptionButtons(bool active)//���ɂ���{�^��������L���A��L��������
    {
        bgmButton.enabled = active;
        seButton.enabled = active;
        jumpSensibilityButton.enabled = active;
        cameraSensibilityButton.enabled = active;
        if (currentSlider != null && active) currentSlider.enabled = false;
        OnUseSlider = !active;

        if (active)
        {
            if (previousButton == null)
                bgmButton.Select();
            else
                previousButton.Select();
        }
    }
    public void SelectSlider(Slider slider)//�{�^���̉E�ɂ���X���C�_�[��I����Ԃɂ���(Button���Ă�)
    {
        isSelectSlider = true;
        currentSlider = slider;
        previousButton = eventSystem.currentSelectedGameObject.gameObject.GetComponent<Button>();
        previousValue = (int)slider.value;

        ActiveOptionButtons(false);
    }

    //���ꂼ��̒l��ύX����
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
