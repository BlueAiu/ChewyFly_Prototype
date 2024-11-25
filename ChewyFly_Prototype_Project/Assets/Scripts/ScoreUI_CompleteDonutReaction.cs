using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ScoreUI_CompleteDonutReaction : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] float upHeightPerSecond = 5f;
    [Tooltip("アルファが1(表示している)の時間")]
    [SerializeField] float alphaOne_Time = 1f;
    [Tooltip("消える時間")]
    [SerializeField] float disappearTime = 1f;//消える時間
    float timer = 0f;
    CanvasGroup canvasGroup;
    public void ScoreInitialized(ObjectReferenceManeger.DonutScoreType _type, int _score,
        float horizontalPercent, float verticalPercent)
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

        RectTransform uiElement = GetComponent<RectTransform>();
        // アンカーを中央に設定
        uiElement.anchorMin = new Vector2(0.5f, 0.5f);
        uiElement.anchorMax = new Vector2(0.5f, 0.5f);

        // ピボットを中央に設定
        uiElement.pivot = new Vector2(0.5f, 0.5f);

        // スクリーンの割合に基づいて位置を設定
        float xPos = (horizontalPercent - 0.5f) * Screen.width;
        float yPos = (verticalPercent - 0.5f) * Screen.height;

        uiElement.anchoredPosition = new Vector2(xPos, yPos);
    }
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup != null)
        canvasGroup.alpha = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer < disappearTime)
        {
            Vector3 pos = transform.position;
            pos.y += upHeightPerSecond * Time.deltaTime;
            transform.position = pos;
            if(canvasGroup != null && alphaOne_Time < timer)
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
