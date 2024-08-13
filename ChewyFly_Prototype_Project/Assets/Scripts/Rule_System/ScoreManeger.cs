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
    void AddDonutScore(GameObject donut)//�h�[�i�c�̃h�[�i�c�̌`��]�����A���_
    {
        int donutScore = donutScore_base;//�܂���b�_

        if (IsIdealDonut(donut)) //���z�I�Ȍ`�Ȃ���Z
        {
            donutScore += donutScore_ideal;
        }

        int unionCount = donut.GetComponent<DonutsUnionScript>().unionCount;
        if(unionCount > idealDonutNum) //�h�[�i�c�̐����Z�𒴂����Ȃ���Z
        {
            donutScore += (unionCount - idealDonutNum) * donutScore_over;
        }

        totalScore += donutScore;
        SetDonutScoreText();
    }
    void SetDonutScoreText() //UI�Ɍ��݂̃X�R�A��\��
    {
        donutScoreText.text = "Score:" + totalScore.ToString();
    }
}
