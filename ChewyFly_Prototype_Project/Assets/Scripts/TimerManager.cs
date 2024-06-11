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

    [Header("�Q�[���̐�������")]
    [Tooltip("�Q�[���̐�������(�b)")]
    [SerializeField] float timeLimit = 180f;

    [Tooltip("�J�E���g�_�E����1���Ƃ̕b��")]//�Ⴆ��0.5�ɂ����1.5�b�Ŏn�܂�܂�
    [SerializeField] float timePerCountdown = 1f;

    [Tooltip("�Q�[���X�^�[�g!��������܂ł̎���")]
    [SerializeField] float countdownFadeTime = 1f;

    float countdownTimer;
    float timer;

    const int countdownNum = 3;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerController>().enabled = false;

        countdownText.enabled = true;
        countdownText.text = countdownNum.ToString();
        countdownTimer = timePerCountdown * countdownNum;

        timer = timeLimit;
        timerCircleImage.fillAmount = 1f;

        //timerText.enabled = true;
        //timerText.text = (Mathf.Floor(timer * 10) / 10).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (countdownTimer > 0)//�J�E���g�_�E�����
        {
            countdownTimer -= Time.deltaTime;
            countdownText.text = ((int)(countdownTimer / timePerCountdown) + 1).ToString();

            if (countdownTimer <= 0)
            {
                player.GetComponent<PlayerController>().enabled = true;
                countdownText.text = "GameStart!";
            }
        }
        else
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
    }
    void GamePlayTimer()
    {
        timer -= Time.deltaTime;
        timerCircleImage.fillAmount = timer / timeLimit;

        //timerText.text = (Mathf.Floor(timer * 10) / 10).ToString();
        if (timer <= 0)//�I��
        {
            SceneManager.LoadScene("ResultScene");
        }
    }
}
