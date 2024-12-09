using TMPro;
using UnityEngine;

public class ScoreUI_CompleteDonutReaction : MonoBehaviour//�h�[�i�c�������̃��A�N�V�������o�Ă���X�R�A��UI
{
    [SerializeField] TMP_Text scoreTypeText;
    [SerializeField] TMP_Text scoreText;
    [Tooltip("�㏸�l(��b�ɂ�)")]
    [SerializeField] float upHeightPerSecond = 5f;
    [Tooltip("�A���t�@��1(�\�����Ă���)�̎���")]
    [SerializeField] float alphaOne_Time = 1f;
    [Tooltip("�����鎞��")]
    [SerializeField] float disappearTime = 1f;//�����鎞��

    float timer = 0f;
    CanvasGroup canvasGroup;
    bool isShiftAppear;//���o��A�o�����Ԃ����炵�Ă��邩�H
    float shiftAppearTime;
    public void ScoreInitialized(ObjectReferenceManeger.DonutScoreType _type, int _score,Vector3 pos, float _shiftAppearTime = 0f)
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
        scoreTypeText.text = typeText + ":";
        scoreTypeText.text = "";
        scoreText.text = " +" + _score.ToString();

        transform.position = pos;

        if(_shiftAppearTime != 0)//ui�̏o�������炷���Ԃ�0�łȂ��Ȃ�
        {
            isShiftAppear = true;
            shiftAppearTime = _shiftAppearTime;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null)
        {
            gameObject.AddComponent<CanvasGroup>();
            canvasGroup = GetComponent<CanvasGroup>();
        }
        if (!isShiftAppear)
            canvasGroup.alpha = 1.0f;
        else
            canvasGroup.alpha = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isShiftAppear)//�܂��o�������Ȃ�
        {
            timer += Time.deltaTime;
            if(timer > shiftAppearTime)
            {
                isShiftAppear = false;
                timer = 0f;
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
                if (alphaOne_Time < timer)
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
