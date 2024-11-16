using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutScoreSaver : MonoBehaviour//�h�[�i�c�������Adonut�ɂ��̃X�N���v�g���t��
{
    [Tooltip("���ꂼ��̃X�R�A���i�[���Ă���")]
    [SerializeField] int[] typeScores = new int[(int)ObjectReferenceManeger.DonutScoreType.End];
    public void AddDonutScoreType(ObjectReferenceManeger.DonutScoreType scoreType, int _score)//�X�R�A�̎�ނɑΉ������_��ۑ�
    {
        typeScores[(int)scoreType] += _score;
    }
    public int GetDonutTypeScore(ObjectReferenceManeger.DonutScoreType scoreType)//�ŗL�̃X�R�A��Ԃ�
    {
        return typeScores[(int)scoreType];
    }

    public int GetDonutTotalScore()//���̃h�[�i�c�̃X�R�A�̍��v��Ԃ�
    {
        int totalScore = 0;
        for(int i = 0; i < typeScores.Length; i++)
        {
            totalScore += typeScores[i];
        }
        return totalScore;
    }
}
