using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TimerManager : MonoBehaviour //�Q�[���v���C�̃^�C�}�[���V�[���ڍs
{
    [Tooltip("�v���C���[")]
    [SerializeField] GameObject player;

    [Tooltip("�J�E���g�_�E���̃e�L�X�g")]
    [SerializeField] TextMeshProUGUI countdownText;

    [Tooltip("�������Ԃ̃e�L�X�g")]
    [SerializeField] Image timerCircleImage;
    //[Tooltip("�������Ԃ̃e�L�X�g")]
    //[SerializeField] TextMeshProUGUI timerText;
    [Tooltip("�^�C���A�b�v��UI")]
    [SerializeField] GameObject timeUpObj;

    [Header("�Q�[���̐�������")]
    [Tooltip("�Q�[���̐�������(�b)")]
    [SerializeField] float timeLimit = 180f;

    [Tooltip("�J�E���g�_�E����1���Ƃ̕b��")]//�Ⴆ��0.5�ɂ����1.5�b�Ŏn�܂�܂�
    [SerializeField] float timePerCountdown = 1f;

    [Tooltip("�Q�[���X�^�[�g!��������܂ł̎���")]
    [SerializeField] float countdownFadeTime = 1f;

    [Header("�I�����̃^�C�}�[���o�̕ϐ�")]
    [Tooltip("�����Ԃ̉�%����^�C�}�[�̐F��ς��邩(0�`1�̊�)")]
    [SerializeField] float timerColorChangeRatio = 0.25f;
    [Tooltip("�ω���̃^�C�}�[�̐F")]
    [SerializeField] Color endTimerColor;
    Color startTimerColor;

    [SerializeField] float endCountDownPerTime = 1f;
    [Tooltip("��������^�C�}�[")]
    [SerializeField] GameObject highLightTimerObject;
    [Tooltip("�^�C�}�[���g��k������Ƃ��̃^�C�}�[�̎���")]
    [SerializeField] float highLightTimeCycle = 0.5f;
    [Tooltip("�^�C�}�[����������Ƃ���Scale�̃J�[�u")]
    [SerializeField] AnimationCurve highLightScaleCurve;
    [Tooltip("�^�C�}�[����������Ƃ��̍ő��Scale")]
    [SerializeField] float highLightScale = 1.2f;
    int previousEndCountDownNum = endCountdownNum + 1;
    Vector3 defaultTimerScale;

    [Header("�I�����̃J�E���g�_�E���̉��o�̕ϐ�")]
    [Tooltip("�I�����̃J�E���g�_�E���̃e�L�X�g")]
    [SerializeField] TextMeshProUGUI endCountDownText;
    [SerializeField] float endCountDownMaxScale = 2f;
    Vector3 endCountDownDefaultScale;

    [Tooltip("�I������bgm�̉�����(1���f�t�H���g)")]
    [SerializeField] float finishBGMSpeedPitch = 1.1f;
    bool isSpeedUpBGM;//bgm������������(�I���ԋ߂�)

    float countdownTimer = 0f;
    float timer = 0f;
    float timeUpTimer = 0f;

    const int startCountdownNum = 3;
    const int endCountdownNum = 10;

    [Tooltip("�Q�[���I�����^�C���A�b�v�ƕ\�����鎞��")]
    [SerializeField] float timeUpTime = 3f;

    [Header("�Q�[���̉�")]
    [SerializeField] SoundManager soundManager;
    [SerializeField] AudioClip gameBGM;
    [SerializeField] AudioClip endCountDownSE;
    [SerializeField] AudioClip finishWhistleSE;//�I�����ɖ�z�C�b�X��

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
        EnablePlayerController(false);

        countdownText.enabled = true;
        countdownText.text = startCountdownNum.ToString();
        countdownTimer = timePerCountdown * startCountdownNum;
        timeUpObj.SetActive(false);

        timer = timeLimit;
        timerCircleImage.fillAmount = 1f;
        startTimerColor = timerCircleImage.color;
        defaultTimerScale = highLightTimerObject.transform.localScale;
        endCountDownText.enabled = false;
        endCountDownDefaultScale = endCountDownText.transform.localScale;
        isSpeedUpBGM = false;

        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundManager.PlayBGM(gameBGM);
        //timerText.enabled = true;
        //timerText.text = (Mathf.Floor(timer * 10) / 10).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (countdownTimer > 0)//�J�E���g�_�E�����
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0)
            {
                EnablePlayerController(true);
                countdownText.text = "�X�^�[�g�I";
            }
            else
            {
                countdownText.text = ((int)(countdownTimer / timePerCountdown) + 1).ToString();
            }
        }
        else if (timer > 0)
        {
            if (-countdownFadeTime < countdownTimer)//"�Q�[���X�^�[�g"��\������]�C
            {
                countdownTimer -= Time.deltaTime;
                if (countdownTimer <= -countdownFadeTime)
                {
                    countdownText.enabled = false;
                }
                else
                {
                    countdownText.alpha = 1 + countdownTimer / countdownFadeTime;//�������������ɂ��Ă����܂�
                }
            }
            EndCountDown();
            GamePlayTimer();
        }
        else
        {
            timeUpTimer += Time.deltaTime;
            if (timeUpTimer > timeUpTime)
            {
                SceneManager.LoadScene("ResultScene");
            }
        }
    }
    void EndCountDown()//�Q�[���I���ԋ߂̉��o
    {
        if (timer <= timeLimit * timerColorChangeRatio)//�^�C�}�[�̐F��ς���
        {
            float timerColorRatio = 1f - (timer / (timeLimit * timerColorChangeRatio));
            timerCircleImage.color = Color.Lerp(startTimerColor, endTimerColor, timerColorRatio);
        }
        if (timer <= endCountdownNum * endCountDownPerTime)//�I�����̃J�E���g�_�E��
        {

            highLightTimerObject.transform.localScale =
                Vector3.Lerp(defaultTimerScale, defaultTimerScale * highLightScale,
                highLightScaleCurve.Evaluate(((endCountdownNum * endCountDownPerTime - timer) % highLightTimeCycle) / highLightTimeCycle));

            int currentCountDownNum = (int)((timer - timer % endCountDownPerTime) / endCountDownPerTime) + 1;

            float currentCountDownRatio = (timer - (currentCountDownNum - 1) * endCountDownPerTime) / endCountDownPerTime;
            endCountDownText.alpha = currentCountDownRatio;
            endCountDownText.transform.localScale =
                Vector3.Lerp(endCountDownDefaultScale, endCountDownDefaultScale * endCountDownMaxScale, 1f - currentCountDownRatio);

            if (previousEndCountDownNum != currentCountDownNum)//�J�E���g�_�E���^�C�}�[���ω�������
            {
                endCountDownText.enabled = true;
                endCountDownText.text = currentCountDownNum.ToString();
                soundManager.PlaySE(endCountDownSE);
            }
            previousEndCountDownNum = currentCountDownNum;
        }
    }
    void GamePlayTimer()
    {
        timer -= Time.deltaTime;
        timerCircleImage.fillAmount = timer / timeLimit;

        if(!isSpeedUpBGM && timer <= timeLimit * timerColorChangeRatio)
        {
            isSpeedUpBGM = true;
            soundManager.SetBGMPitch(finishBGMSpeedPitch);
        }
        if (timer <= 0)//�I��
        {
            FinishGame();
        }
    }
    void FinishGame()
    {
        timeUpTimer = 0f;
        EnablePlayerController(false);
        timeUpObj.SetActive(true);
        endCountDownText.enabled = false;
        soundManager.PlaySE(finishWhistleSE);
    }
    void EnablePlayerController(bool isActive)
    {
        player.GetComponent<PlayerController>().enabled = isActive;
        player.GetComponent<FlicStrength>().enabled = isActive;
    }
}
