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
    [Tooltip("タイムアップのUI")]
    [SerializeField] GameObject timeUpObj;

    [Header("ゲームの制限時間")]
    [Tooltip("ゲームの制限時間(秒)")]
    [SerializeField] float timeLimit = 180f;

    [Tooltip("カウントダウンの1ごとの秒数")]//例えば0.5にすれば1.5秒で始まります
    [SerializeField] float timePerCountdown = 1f;

    [Tooltip("ゲームスタート!が消えるまでの時間")]
    [SerializeField] float countdownFadeTime = 1f;

    [Header("終了時のタイマー演出の変数")]
    [Tooltip("総時間の何%からタイマーの色を変えるか(0～1の間)")]
    [SerializeField] float timerColorChangeRatio = 0.25f;
    [Tooltip("変化後のタイマーの色")]
    [SerializeField] Color endTimerColor;
    Color startTimerColor;

    [SerializeField] float endCountDownPerTime = 1f;
    [Tooltip("強調するタイマー")]
    [SerializeField] GameObject highLightTimerObject;
    [Tooltip("タイマーを拡大縮小するときのタイマーの周期")]
    [SerializeField] float highLightTimeCycle = 0.5f;
    [Tooltip("タイマーを強調するときのScaleのカーブ")]
    [SerializeField] AnimationCurve highLightScaleCurve;
    [Tooltip("タイマーを強調するときの最大のScale")]
    [SerializeField] float highLightScale = 1.2f;
    int previousEndCountDownNum = endCountdownNum + 1;
    Vector3 defaultTimerScale;

    [Header("終了時のカウントダウンの演出の変数")]
    [Tooltip("終了時のカウントダウンのテキスト")]
    [SerializeField] TextMeshProUGUI endCountDownText;
    [SerializeField] float endCountDownMaxScale = 2f;
    Vector3 endCountDownDefaultScale;

    [Tooltip("終了時のbgmの加速率(1がデフォルト)")]
    [SerializeField] float finishBGMSpeedPitch = 1.1f;
    bool isSpeedUpBGM;//bgmが加速したか(終了間近か)

    float countdownTimer = 0f;
    float timer = 0f;
    float timeUpTimer = 0f;

    const int startCountdownNum = 3;
    const int endCountdownNum = 10;

    [Tooltip("ゲーム終了時タイムアップと表示する時間")]
    [SerializeField] float timeUpTime = 3f;

    [Header("ゲームの音")]
    [SerializeField] SoundManager soundManager;
    [SerializeField] AudioClip gameBGM;
    [SerializeField] AudioClip endCountDownSE;
    [SerializeField] AudioClip finishWhistleSE;//終了時に鳴るホイッスル

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
        if (countdownTimer > 0)//カウントダウン状態
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0)
            {
                EnablePlayerController(true);
                countdownText.text = "スタート！";
            }
            else
            {
                countdownText.text = ((int)(countdownTimer / timePerCountdown) + 1).ToString();
            }
        }
        else if (timer > 0)
        {
            if (-countdownFadeTime < countdownTimer)//"ゲームスタート"を表示する余韻
            {
                countdownTimer -= Time.deltaTime;
                if (countdownTimer <= -countdownFadeTime)
                {
                    countdownText.enabled = false;
                }
                else
                {
                    countdownText.alpha = 1 + countdownTimer / countdownFadeTime;//すこしずつ透明にしていきます
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
    void EndCountDown()//ゲーム終了間近の演出
    {
        if (timer <= timeLimit * timerColorChangeRatio)//タイマーの色を変える
        {
            float timerColorRatio = 1f - (timer / (timeLimit * timerColorChangeRatio));
            timerCircleImage.color = Color.Lerp(startTimerColor, endTimerColor, timerColorRatio);
        }
        if (timer <= endCountdownNum * endCountDownPerTime)//終了時のカウントダウン
        {

            highLightTimerObject.transform.localScale =
                Vector3.Lerp(defaultTimerScale, defaultTimerScale * highLightScale,
                highLightScaleCurve.Evaluate(((endCountdownNum * endCountDownPerTime - timer) % highLightTimeCycle) / highLightTimeCycle));

            int currentCountDownNum = (int)((timer - timer % endCountDownPerTime) / endCountDownPerTime) + 1;

            float currentCountDownRatio = (timer - (currentCountDownNum - 1) * endCountDownPerTime) / endCountDownPerTime;
            endCountDownText.alpha = currentCountDownRatio;
            endCountDownText.transform.localScale =
                Vector3.Lerp(endCountDownDefaultScale, endCountDownDefaultScale * endCountDownMaxScale, 1f - currentCountDownRatio);

            if (previousEndCountDownNum != currentCountDownNum)//カウントダウンタイマーが変化した時
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
        if (timer <= 0)//終了
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
