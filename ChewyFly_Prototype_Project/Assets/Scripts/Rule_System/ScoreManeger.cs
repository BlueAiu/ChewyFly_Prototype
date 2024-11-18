using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public partial class ObjectReferenceManeger : MonoBehaviour
{
    [SerializeField] public static int totalScore { get; private set; } = 0;

    [Header("�h�[�i�c�̃X�R�A�v�Z")]
    [Tooltip("��b�_")]
    [SerializeField] int donutScore_base = 160;
    [Tooltip("�`�������ȏꍇ�̒ǉ��_")]
    [SerializeField] int donutScore_ideal = 200;
    [Tooltip("6�𒴂����ꍇ�̉��Z�_")]
    [SerializeField] int donutScore_over = 30;

    [Tooltip("���ꂼ��̏ł��ɑΉ�������Z�_(��ɂȂ�قǏł���)")]
    [SerializeField] int[] burntScores = new int[(int)DonutBakedState.Burnt + 1];

    [Header("�X�R�A�\���p�̃e�L�X�g")]
    [SerializeField] TMP_Text donutScoreText;

    const int checkRange = 10;
    const int idealDonutNum = 6;

    readonly Vector2[] idealShapePos = new Vector2[]
    {
        new Vector2(-1, 1),
        new Vector2(0, 1),
        new Vector2(-1, 0),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(1, -1)
    };

    public enum DonutScoreType//�X�R�A�̎��(�X�R�A�̎�ސ���m�邽�߂�End�͍Ō�ɒu��)
    {
        Base ,BurntColor, OverNum, Ideal, Pyramid, Flower, Straight, Infinity, End
    }

    bool IsIdealDonut(GameObject donut)
    {
        var donutShape = donut.GetComponent<DonutsUnionScript>().hexaPositions;
        
        return CheckDonutShape(donutShape) == idealDonutNum;
    }

    //���z�̌`�Ǝ��Ă��邩������
    int CheckDonutShape(List<Vector2> donutsPos)
    {
        int maxFitDonuts = 0;

        for(int i = -checkRange; i <= checkRange; i++)
        {
            for(int j = -checkRange; j <= checkRange; j++)
            {
                int fitDonuts = 0;
                Vector2 currentPos = new Vector2(i, j);

                foreach(var v in idealShapePos)
                {
                    if(donutsPos.Contains(currentPos + v))
                    {
                        fitDonuts++;
                    }
                }

                maxFitDonuts = Mathf.Max(maxFitDonuts, fitDonuts);
            }
        }

        return maxFitDonuts;
    }
    void AddDonutScore(GameObject donut)//�h�[�i�c�̃h�[�i�c�̌`��]�����A���_�B
    {
        donut.AddComponent<DonutScoreSaver>();//ScoreSaver�ɓ_�����Ă��獇�v�_�ɉ��_
        DonutScoreSaver scoreSaver = donut.GetComponent<DonutScoreSaver>();

        bool isIdeal = false;//�h�[�i�c�����z�̌`��
        scoreSaver.AddDonutScoreType(DonutScoreType.Base, donutScore_base);//�܂���b�_

        if (IsIdealDonut(donut)) //���z�I�Ȍ`�Ȃ���Z
        {
            scoreSaver.AddDonutScoreType(DonutScoreType.Ideal, donutScore_ideal);
            isIdeal = true;
        }

        DonutsUnionScript unionScript = donut.GetComponent<DonutsUnionScript>();
        int unionCount = unionScript.unionCount;
        if(unionCount > idealDonutNum) //�h�[�i�c�̐����Z�𒴂����Ȃ���Z
        {
            scoreSaver.AddDonutScoreType(DonutScoreType.OverNum, (unionCount - idealDonutNum) * donutScore_over);
        }

        int[] burntDonutsNum = unionScript.GetBurntDonutsNum();//�ł��̏�Ԃɉ����ĉ��_
        for(int i = 0; i < burntDonutsNum.Length; i++)
        {
            scoreSaver.AddDonutScoreType(DonutScoreType.BurntColor, burntDonutsNum[i] * burntScores[i]);
        }

        totalScore += scoreSaver.GetDonutTotalScore();//�Ōケ�̃h�[�i�c�ɂ����X�R�A�̕����_����
        SetDonutScoreText();

        CompleteDonutEffect(isIdeal, donut, unionScript.GetDonutsCenterPoint());//�������̃G�t�F�N�g
    }
    void SetDonutScoreText() //UI�Ɍ��݂̃X�R�A��\��
    {
        donutScoreText.text = "Score:" + totalScore.ToString();
    }
}
