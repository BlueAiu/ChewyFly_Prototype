using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [Tooltip("プレイヤー")]
    [SerializeField] GameObject player;

    [Tooltip("ゲームの制限時間(秒)")]
    [SerializeField] float timeLimit = 180f;

    [Tooltip("カウントダウンの1ごとの秒数")]
    [SerializeField] float timePerCountdown = 1f;

    [Tooltip("カウントダウンのテキスト")]
    [SerializeField] TextMeshProUGUI countdownText;

    [Tooltip("制限時間のテキスト")]
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
            if(-1 < countdownTimer)//ゲームスタートを表示する余韻
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

            if (timer <= 0)//終了
            {
                SceneManager.LoadScene("ResultScene");
            }
        }
    }
}
