using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using static UnityEditor.Experimental.GraphView.GraphView;
using System;

public partial class ResultManager : MonoBehaviour
{
    [Header("�Q�Ɨp�̍���")]
    [SerializeField] GameObject addScoreTextPrefab;
    [Tooltip("���o���ɂ͉B��ui�̐e")]
    [SerializeField] GameObject resultUIParent;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button titleButton;
    [SerializeField] RectTransform defaultAddScoreTextPos;
    [SerializeField] Camera resultCamera;
    [SerializeField] Camera donutsCamera;
    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Transform donutSetCenterPosition;
    [SerializeField] GameObject skipText;
    [Tooltip("�h�[�i�c����f�������̃G�t�F�N�g")]
    [SerializeField] GameObject oneDonutEffect;

    enum ScoreCountState { ShowDonut, MoveToNextDonut, Finish }//�X�R�A�̉��Z���o�̏��
    ScoreCountState scoreCountState;
    
    List<GameObject> donuts = ObjectReferenceManeger.completeDonuts;

    [Tooltip("�炷BGM�𕪂���m���}�X�R�A")]
    [SerializeField] int scoreQuota = 800;
    [SerializeField] SoundManager soundManager;
    [SerializeField] AudioClip clearBGM;
    [SerializeField] AudioClip failureBGM;

    InputScript input;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputScript>();
        if (input == null) { gameObject.AddComponent<InputScript>(); input = GetComponent<InputScript>(); }

        Vector3 donutUnionsCenterPos = Vector3.zero;
        foreach (var d in donuts)  { donutUnionsCenterPos += d.transform.position;   }
        donutUnionsCenterPos /= donuts.Count;
        defaultMadeDonutTextLocalScale = resultText.transform.localScale;

        foreach (var i in donuts)
        {
            i.transform.position = i.transform.position + (donutSetCenterPosition.transform.position - donutUnionsCenterPos);
            i.SetActive(false);
            //i.GetComponent<Rigidbody>().isKinematic = true;
        }

        if (donuts.Count == 0)
        {
            DesplayResultUI(); 
        }
        else//�쐬�����h�[�i�c������Ȃ牉�o������
        {
            InitializeScoreCountClass();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (scoreCountState != ScoreCountState.Finish)
        {
            if (input.isBButton())//�X�L�b�v����
            {
                CloseAllAddScoreTexts();
                DesplayResultUI();
            }
            else
            {
                CountScoreUpdate();
            }
        }
    }

    void DesplayResultUI()//�{�^������\�����āA�h�[�i�c���J�E���g���鉉�o�͏I��
    {
        scoreCountState = ScoreCountState.Finish;
        resultUIParent.SetActive(true);
        skipText.SetActive(false);
        restartButton.Select();//�n�܂������_�Ń��X�^�[�g�{�^����I����Ԃɂ��Ă����܂�
        foreach (var i in donuts)
        {
            i.SetActive(true);
        }
        ChangeCamera(true);

        SetMadeDonutText(ObjectReferenceManeger.madeDonuts);
        SetScoreText(ObjectReferenceManeger.totalScore);

        if (ObjectReferenceManeger.totalScore < scoreQuota)
        {
            soundManager.PlayBGM(failureBGM);
        }
        else
        {
            soundManager.PlayBGM(clearBGM);
        }
    }
    void ChangeCamera(bool activeResultCamera)//�J�����̐؂�ւ�
    {
        resultCamera.gameObject.SetActive(activeResultCamera);
        donutsCamera.gameObject.SetActive(!activeResultCamera);
    }
    void SetMadeDonutText(int _num)
    {
        resultText.transform.localScale = defaultMadeDonutTextLocalScale;
        resultText.text = "�h�[�i�c�� " + _num.ToString() + " �R�����";
    }
    void SetScoreText(int _score)
    {
        scoreText.text = "�X�R�A :  " + _score.ToString();
    }
    private void OnDestroy()
    {
        ObjectReferenceManeger.ClearCompleteDonuts();
    }
}
