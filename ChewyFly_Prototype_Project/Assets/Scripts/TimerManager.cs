using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [Tooltip("�v���C���[")]
    [SerializeField] GameObject player;

    [Tooltip("�Q�[���̐�������(�b)")]
    [SerializeField] float timeLimit = 180f;

    [Tooltip("�J�E���g�_�E����1���Ƃ̕b��")]
    [SerializeField] float timePerCountdown = 1f;

    [Tooltip("�J�E���g�_�E���̃e�L�X�g")]
    [SerializeField] TextMeshProUGUI countdownText;

    [Tooltip("�������Ԃ̃e�L�X�g")]
    [SerializeField] TextMeshProUGUI timerText;

    float countdownTimer;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerController>().enabled = false;


        countdownText.enabled = true;
        countdownText.text = "3";

        countdownTimer = timePerCountdown * 3f;
        timer = timeLimit;
        timerText.enabled = true;
        timerText.text = (Mathf.Floor(timer * 10) / 10).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (countdownTimer > 0)
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
            if(-1 < countdownTimer)//�Q�[���X�^�[�g��\������]�C
            {
                countdownTimer -= Time.deltaTime;
                if(countdownTimer < -1)
                {
                    countdownText.enabled = false;
                }
                else
                {
                    countdownText.alpha = 1 + countdownTimer;
                }
            }

            timer -= Time.deltaTime; 
            timerText.text = (Mathf.Floor(timer * 10) / 10).ToString();

            if (timer <= 0)//�I��
            {
                SceneManager.LoadScene("ResultScene");
            }
        }
    }
}
