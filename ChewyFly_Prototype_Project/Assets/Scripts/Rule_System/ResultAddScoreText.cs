using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultAddScoreText : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    Vector3 position_default;
    Vector3 position_moveFrom;

    [SerializeField] float moveX;
    [SerializeField] float moveTime;
    float moveTimer;
    enum TextState {Move, Stop, Close}
    TextState textState = TextState.Stop;
    CanvasGroup canvasGroup;
    private void Start()
    {
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
            case TextState.Close:
                
                break;
        }
    }
    public void SetDefaultPosition(Vector3 pos)
    {
        position_default = pos;
        position_moveFrom = new Vector3(pos.x + moveX, pos.y, pos.z);
        transform.position = position_moveFrom;
        gameObject.SetActive(false);
    }
    public void SetScore(ObjectReferenceManeger.DonutScoreType type, int score)
    {
        string typeText = "";
        switch (type)
        {
            case ObjectReferenceManeger.DonutScoreType.Base:
                typeText = "BaseScore";
                break;
            case ObjectReferenceManeger.DonutScoreType.BurntColor:
                typeText = "BurntColorBonus";
                break;
            case ObjectReferenceManeger.DonutScoreType.Ideal:
                typeText = "IdealShapeScore";
                break;
            case ObjectReferenceManeger.DonutScoreType.OverNum:
                typeText = "OverNumBonus";
                break;
        }

        gameObject.SetActive(true);
        textState = TextState.Move;
        moveTimer = 0f;
        scoreText.text = typeText + " +" + score.ToString();

    }
    public void CloseText()
    {
        gameObject.SetActive(false);
    }
}
