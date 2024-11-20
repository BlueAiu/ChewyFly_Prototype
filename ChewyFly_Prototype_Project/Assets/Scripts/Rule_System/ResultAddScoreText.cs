using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultAddScoreText : MonoBehaviour//ResultScene�̃h�[�i�c�\�����o���ɏo�Ă��镶���̐��������
{
    [SerializeField] TMP_Text scoreText;
    Vector3 position_default;
    Vector3 position_moveFrom;
    [SerializeField] RectTransform nextTextPosition;
    [Tooltip("�����̐F")]
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
    public void SetDefaultPosition(Vector3 pos)//�����ʒu��ݒ�
    {
        position_default = pos;
        position_moveFrom = new Vector3(pos.x + moveX, pos.y, pos.z);
        transform.position = position_default;
        gameObject.SetActive(false);
    }
    public Vector3 GetNextTextPoint()//����text�̈ʒu��Ԃ�
    {
        return nextTextPosition.transform.position;
    }
    public void SetScore(ObjectReferenceManeger.DonutScoreType type, int score)//���̃X�R�A�����_�����󂯎��\��
    {
        scoreText.color = color_default;
        string typeText = "";
        switch (type)
        {
            case ObjectReferenceManeger.DonutScoreType.Base:
                typeText = "�h�[�i�c���ł���";
                break;
            case ObjectReferenceManeger.DonutScoreType.BurntColor:
                typeText = "�h�[�i�c���g������";
                break;
            case ObjectReferenceManeger.DonutScoreType.Ideal:
                scoreText.color = color_Yellow;
                typeText = "���ꂢ�Ȃ�����";
                break;
            case ObjectReferenceManeger.DonutScoreType.OverNum:
                scoreText.color = color_Orange;
                typeText = "�傫�ȃh�[�i�c";
                break;
            case ObjectReferenceManeger.DonutScoreType.Pyramid:
                typeText = "�s���~�b�h";
                break;
            case ObjectReferenceManeger.DonutScoreType.Flower:
                typeText = "�t�����[";
                break;
            case ObjectReferenceManeger.DonutScoreType.Straight:
                typeText = "�܂�����";
                break;
            case ObjectReferenceManeger.DonutScoreType.Infinity:
                typeText = "�C���t�B�j�e�B";
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
