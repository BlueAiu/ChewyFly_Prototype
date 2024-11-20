using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultAddScoreText : MonoBehaviour//ResultSceneのドーナツ表示演出時に出てくる文字の制御をする
{
    [SerializeField] TMP_Text scoreText;
    Vector3 position_default;
    Vector3 position_moveFrom;
    [SerializeField] RectTransform nextTextPosition;
    [Tooltip("文字の色")]
    [SerializeField] Color color_default;
    [SerializeField] Color color_Orange;
    [SerializeField] Color color_Yellow;

    [SerializeField] float moveX;
    [SerializeField] float moveTime;
    float moveTimer;
    enum TextState {Move, Stop}
    TextState textState = TextState.Stop;
    CanvasGroup canvasGroup;
    private void Start()
    {
        if (canvasGroup == null)
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        switch (textState)
        {
            case TextState.Move:
                moveTimer += Time.deltaTime;
                if(moveTimer < moveTime)
                {
                    transform.position = Vector3.Lerp(position_moveFrom, position_default, moveTimer / moveTime);
                    canvasGroup.alpha = moveTimer / moveTime;
                }
                else
                {
                    canvasGroup.alpha = 1f;
                    textState = TextState.Stop;
                }
                break;
        }
    }
    public void SetDefaultPosition(Vector3 pos)//初期位置を設定
    {
        position_default = pos;
        position_moveFrom = new Vector3(pos.x + moveX, pos.y, pos.z);
        transform.position = position_default;
        gameObject.SetActive(false);
    }
    public Vector3 GetNextTextPoint()//次のtextの位置を返す
    {
        return nextTextPosition.transform.position;
    }
    public void SetScore(ObjectReferenceManeger.DonutScoreType type, int score)//何のスコアが何点かを受け取り表示
    {
        scoreText.color = color_default;
        string typeText = "";
        switch (type)
        {
            case ObjectReferenceManeger.DonutScoreType.Base:
                typeText = "ドーナツができた";
                break;
            case ObjectReferenceManeger.DonutScoreType.BurntColor:
                typeText = "ドーナツが揚がった";
                break;
            case ObjectReferenceManeger.DonutScoreType.Ideal:
                scoreText.color = color_Yellow;
                typeText = "きれいなかたち";
                break;
            case ObjectReferenceManeger.DonutScoreType.OverNum:
                scoreText.color = color_Orange;
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

        gameObject.SetActive(true);
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        textState = TextState.Move;
        moveTimer = 0f;
        scoreText.text = typeText + " +" + score.ToString();

    }
    public void CloseText()
    {
        gameObject.SetActive(false);
    }
}
