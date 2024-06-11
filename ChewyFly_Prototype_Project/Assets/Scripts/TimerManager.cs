using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TimerManager : MonoBehaviour //ゲームプレイのタイマー兼シーン移行
{
    [Tooltip("プレイヤー")]
    [SerializeField] GameObject player;

    [Tooltip("カウントダウンのテキスト")]
    [SerializeField] TextMeshProUGUI countdownText;

    [Tooltip("制限時間のテキスト")]
    [SerializeField] Image timerCircleImage;
    //[Tooltip("制限時間のテキスト")]
    //[SerializeField] TextMeshProUGUI timerText;

    [Header("ゲームの制限時間")]
    [Tooltip("ゲームの制限時間(秒)")]
    [SerializeField] float timeLimit = 180f;

    [Tooltip("カウントダウンの1ごとの秒数")]//例えば0.5にすれば1.5秒で始まります
    [SerializeField] float timePerCountdown = 1f;

    [Tooltip("ゲームスタート!が消えるまでの時間")]
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
        if (countdownTimer > 0)//カウントダウン状態
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
            if(-countdownFadeTime < countdownTimer)//"ゲームスタート"を表示する余韻
            {
                countdownTimer -= Time.deltaTime;
                if(countdownTimer <= -countdownFadeTime)
                {
                    countdownText.enabled = false;
                }
                else
                {
                    countdownText.alpha = 1 +  countdownTimer / countdownFadeTime;//すこしずつ透明にしていきます
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
        if (timer <= 0)//終了
        {
            SceneManager.LoadScene("ResultScene");
        }
    }
}
