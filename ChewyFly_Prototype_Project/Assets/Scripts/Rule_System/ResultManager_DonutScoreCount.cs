using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ResultManager : MonoBehaviour//�h�[�i�c�̃X�R�A������J�E���g���Ă������o�p�̃N���X
{
    [Header("���o��̍���")]
    [SerializeField] AnimationCurve cameraMoveCurve;
    [Tooltip("�h�[�i�c���f���J�����̍���")]
    [SerializeField] float donutCameraHeightY = 30f;
    [SerializeField] AnimationCurve madeDonutTextScaleCurve;
    [Tooltip("��̃h�[�i�c���f������")]
    [SerializeField] float time_OneDonut = 0.5f;
    [Tooltip("���̃h�[�i�c�Ɉړ����鎞��")]
    [SerializeField] float time_cameraMove = 0.5f;

    ResultAddScoreText[] addScoreTexts;//result�̉��o���̕���(���O�ɂ��ׂėp�ӂ��A�����B�ꂳ����)
    int donutIndex = 0;//�ǂ�index�̃h�[�i�c�����Ă��邩

    Vector3 cameraPoint_from;//��������
    Vector3 cameraPoint_to;//�����܂ŃJ�������ړ�����
    float scoreCountTimer = 0f;//���o�p�̃^�C�}�[
    int directingScore = 0;//���o�\���p�̃X�R�A
    int oneDonutScore = 0;//���o���̂��̃h�[�i�c�̃X�R�A
    int directingMadeDonutNum = 0;//���o�p�̍쐬�h�[�i�c��
    Vector3 defaultMadeDonutTextLocalScale;

    void InitializeScoreCountClass()//���o���s���ꍇ
    {
        addScoreTexts = new ResultAddScoreText[(int)ObjectReferenceManeger.DonutScoreType.End];
        Vector3 nextPoint = defaultAddScoreTextPos.position;
        for (int i = 0; i < (int)ObjectReferenceManeger.DonutScoreType.End; i++)//������o�Ă���ׂ����X�R�Aui�̏�����
        {
            addScoreTexts[i] = Instantiate(addScoreTextPrefab, defaultAddScoreTextPos).GetComponent<ResultAddScoreText>();
            addScoreTexts[i].SetDefaultPosition(nextPoint);
            nextPoint =  addScoreTexts[i].GetNextTextPoint();
        }

        resultUIParent.SetActive(false);
        ChangeCamera(false);
        SetScoreText(0);
        SetMadeDonutText(0);
        skipText.SetActive(true);

        cameraPoint_to = GetDonutCenterPoint(donuts[donutIndex]);
        cameraPoint_to.y += donutCameraHeightY;
        donutsCamera.gameObject.transform.position = cameraPoint_to;

        AddOneDonutScore();
    }
    private void AddOneDonutScore()//�h�[�i�c�̃X�R�A�����Z���鉉�o���n�߂�
    {
        scoreCountState = ScoreCountState.ShowDonut;
        scoreCountTimer = 0f;
        donuts[donutIndex].SetActive(true);
        DonutsUnionScript union = donuts[donutIndex].GetComponent<DonutsUnionScript>();
        Instantiate(oneDonutEffect, union.GetDonutsCenterPoint(), Quaternion.identity);

        DonutScoreSaver saver = donuts[donutIndex].GetComponent<DonutScoreSaver>();
        int textIndex = 0;
        for (int i = 0; i < (int)ObjectReferenceManeger.DonutScoreType.End; i++)//���̃h�[�i�c�̃X�R�A��Ԃ��擾
        {
            int typeScore = saver.GetDonutTypeScore((ObjectReferenceManeger.DonutScoreType)i);
            if (typeScore > 0)
            {
                addScoreTexts[textIndex].SetScore((ObjectReferenceManeger.DonutScoreType)i, typeScore);
                textIndex++;
            }
        }
        oneDonutScore = saver.GetDonutTotalScore();
        directingMadeDonutNum++;
        SetMadeDonutText(directingMadeDonutNum);
    }
    void CountScoreUpdate()//���o�p��Update
    {
        scoreCountTimer += Time.deltaTime;
        switch (scoreCountState)
        {
            case ScoreCountState.ShowDonut://�h�[�i�c�������Ă���
                if (scoreCountTimer < time_OneDonut)
                {
                    SetScoreText(directingScore + (int)(oneDonutScore * scoreCountTimer / time_OneDonut));
                    resultText.transform.localScale = defaultMadeDonutTextLocalScale *
                        madeDonutTextScaleCurve.Evaluate(scoreCountTimer / time_OneDonut);
                }
                else
                {
                    MoveToNextDonut();
                }
                break;
            case ScoreCountState.MoveToNextDonut://���̃h�[�i�c�Ɉڍs��
                if (scoreCountTimer < time_cameraMove)
                {
                    donutsCamera.gameObject.transform.position =
                        Vector3.Lerp(cameraPoint_from, cameraPoint_to,
                        cameraMoveCurve.Evaluate(scoreCountTimer / time_cameraMove));
                }
                else
                {
                    AddOneDonutScore();
                }
                break;
        }
    }
    private void MoveToNextDonut()//���̃h�[�i�c�Ɉړ����鏀��������
    {
        donutIndex++;
        if (donutIndex >= donuts.Count)//���ׂẴh�[�i�c�������I�������I��
        {
            CloseAllAddScoreTexts();
            DesplayResultUI();
            return;
        }
        scoreCountState = ScoreCountState.MoveToNextDonut;
        scoreCountTimer = 0f;

        cameraPoint_from = GetDonutCenterPoint(donuts[donutIndex - 1]);//�J�����̈ʒu��ݒ�
        cameraPoint_from.y += donutCameraHeightY;
        cameraPoint_to = GetDonutCenterPoint(donuts[donutIndex]);
        cameraPoint_to.y += donutCameraHeightY;

        CloseAllAddScoreTexts();

        directingScore += oneDonutScore;
        SetScoreText(directingScore);
    }
    void CloseAllAddScoreTexts()
    {
        for (int i = 0; i < (int)ObjectReferenceManeger.DonutScoreType.End; i++)
        {
            addScoreTexts[i].CloseText();
        }
    }
    Vector3 GetDonutCenterPoint(GameObject donut)//union�����o���Đ^�񒆂�Ԃ�
    {
        DonutsUnionScript union = donut.GetComponent<DonutsUnionScript>();
        if (union != null) return union.GetDonutsCenterPoint();
        return Vector3.zero;
    }
}
