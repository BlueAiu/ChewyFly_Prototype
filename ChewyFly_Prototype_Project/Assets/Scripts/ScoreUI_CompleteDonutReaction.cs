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
    [Tooltip("�A���t�@��1(�\�����Ă���)�̎���")]
    [SerializeField] float alphaOne_Time = 1f;
    [Tooltip("�����鎞��")]
    [SerializeField] float disappearTime = 1f;//�����鎞��
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
                typeText = "�h�[�i�c���ł���";
                break;
            case ObjectReferenceManeger.DonutScoreType.BurntColor:
                typeText = "�h�[�i�c���g������";
                break;
            case ObjectReferenceManeger.DonutScoreType.Ideal:
                typeText = "���ꂢ�Ȃ�����";
                break;
            case ObjectReferenceManeger.DonutScoreType.OverNum:
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
        scoreText.text = typeText + " +" + _score.ToString();

        RectTransform uiElement = GetComponent<RectTransform>();
        // �A���J�[�𒆉��ɐݒ�
        uiElement.anchorMin = new Vector2(0.5f, 0.5f);
        uiElement.anchorMax = new Vector2(0.5f, 0.5f);

        // �s�{�b�g�𒆉��ɐݒ�
        uiElement.pivot = new Vector2(0.5f, 0.5f);

        // �X�N���[���̊����Ɋ�Â��Ĉʒu��ݒ�
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
