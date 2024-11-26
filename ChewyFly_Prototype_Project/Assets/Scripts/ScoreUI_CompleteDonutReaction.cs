using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ScoreUI_CompleteDonutReaction : MonoBehaviour//ドーナツ完成時のリアクション時出てくるスコアのUI
{
    [SerializeField] TMP_Text scoreText;
    [Tooltip("上昇値(一秒につき)")]
    [SerializeField] float upHeightPerSecond = 5f;
    [Tooltip("アルファが1(表示している)の時間")]
    [SerializeField] float alphaOne_Time = 1f;
    [Tooltip("消える時間")]
    [SerializeField] float disappearTime = 1f;//消える時間
    float timer = 0f;
    CanvasGroup canvasGroup;
    bool isShiftAppear;//演出上、出現時間をずらしているか？
    float shiftAppearTime;
    public void ScoreInitialized(ObjectReferenceManeger.DonutScoreType _type, int _score,Vector3 pos, float _shiftAppearTime = 0f)
    {
        timer = 0f;
        string typeText = "";
        switch (_type)
        {
            case ObjectReferenceManeger.DonutScoreType.Base:
                typeText = "ドーナツができた";
                break;
            case ObjectReferenceManeger.DonutScoreType.BurntColor:
                typeText = "ドーナツが揚がった";
                break;
            case ObjectReferenceManeger.DonutScoreType.Ideal:
                typeText = "きれいなかたち";
                break;
            case ObjectReferenceManeger.DonutScoreType.OverNum:
                typeText = "大きなドーナツ";
                break;
            case ObjectReferenceManeger.DonutScoreType.Pyramid:
                typeText = "ピラミッド";
                break;
            case ObjectReferenceManeger.DonutScoreType.Flower:
                typeText = "フラワー";
                break;
            case ObjectReferenceManeger.DonutScoreType.Straight:
                typeText = "まっすぐ";
                break;
            case ObjectReferenceManeger.DonutScoreType.Infinity:
                typeText = "インフィニティ";
                break;
        }
        scoreText.text = typeText + " +" + _score.ToString();

        transform.position = pos;

        if(_shiftAppearTime != 0)
        {
            isShiftAppear = true;
            shiftAppearTime = _shiftAppearTime;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup != null)
        {
            if(!isShiftAppear)
            canvasGroup.alpha = 1.0f;
            else 
                canvasGroup.alpha = 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isShiftAppear)//まだ出現させない
        {
            timer += Time.deltaTime;
            if(timer > shiftAppearTime)
            {
                isShiftAppear = false;
                timer = 0f;
                if (canvasGroup != null)
                    canvasGroup.alpha = 1.0f;
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer < disappearTime)
            {
                Vector3 pos = transform.position;
                pos.y += upHeightPerSecond * Time.deltaTime;
                transform.position = pos;
                if (canvasGroup != null && alphaOne_Time < timer)
                {
                    canvasGroup.alpha = (disappearTime - timer) / (disappearTime - alphaOne_Time);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
