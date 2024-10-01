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
    [Tooltip("�^�C���A�b�v�̃e�L�X�g")]
    [SerializeField] TextMeshProUGUI timeUpText;

    [Header("�Q�[���̐�������")]
    [Tooltip("�Q�[���̐�������(�b)")]
    [SerializeField] float timeLimit = 180f;

    [Tooltip("�J�E���g�_�E����1���Ƃ̕b��")]//�Ⴆ��0.5�ɂ����1.5�b�Ŏn�܂�܂�
    [SerializeField] float timePerCountdown = 1f;

    [Tooltip("�Q�[���X�^�[�g!��������܂ł̎���")]
    [SerializeField] float countdownFadeTime = 1f;

    float countdownTimer = 0f;
    float timer = 0f;
    float timeUpTimer = 0f;

    const int countdownNum = 3;

    [Tooltip("�Q�[���I�����^�C���A�b�v�ƕ\�����鎞��")]
    [SerializeField] float timeUpTime = 3f;

    [Header("�Q�[���̉�")]
    [SerializeField] SoundManager soundManager;
    [SerializeField] AudioClip gameBGM;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
        player.GetComponent<InputScript>().isReceiveInput = false;

        countdownText.enabled = true;
        countdownText.text = countdownNum.ToString();
        countdownTimer = timePerCountdown * countdownNum;
        timeUpText.enabled = false;

        timer = timeLimit;
        timerCircleImage.fillAmount = 1f;

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
                player.GetComponent<InputScript>().isReceiveInput = true;
                countdownText.text = "GameStart!";
            }
            else
            {
                countdownText.text = ((int)(countdownTimer / timePerCountdown) + 1).ToString();
            }
        }
        else if(timer > 0)
        {
            if(-countdownFadeTime < countdownTimer)//"�Q�[���X�^�[�g"��\������]�C
            {
                countdownTimer -= Time.deltaTime;
                if(countdownTimer <= -countdownFadeTime)
                {
                    countdownText.enabled = false;
                }
                else
                {
                    countdownText.alpha = 1 +  countdownTimer / countdownFadeTime;//�������������ɂ��Ă����܂�
                }
            }

            GamePlayTimer();
        }
        else
        {
            timeUpTimer += Time.deltaTime;
            if(timeUpTimer > timeUpTime)
            {
                SceneManager.LoadScene("ResultScene");
            }
        }
    }
    void GamePlayTimer()
    {
        timer -= Time.deltaTime;
        timerCircleImage.fillAmount = timer / timeLimit;

        //timerText.text = (Mathf.Floor(timer * 10) / 10).ToString();
        if (timer <= 0)//�I��
        {
            FinishGame();
        }
    }
    void FinishGame()
    {
        timeUpTimer = 0f;
        player.GetComponent<InputScript>().isReceiveInput = false;
        timeUpText.enabled = true;
    }
}
