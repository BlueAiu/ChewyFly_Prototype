using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutScoreSaver : MonoBehaviour//ドーナツ完成時、donutにこのスクリプトが付く
{
    [SerializeField] int[] typeScores = new int[(int)ObjectReferenceManeger.DonutScoreType.End];
    public void AddDonutScoreType(ObjectReferenceManeger.DonutScoreType scoreType, int _score)//スコアの種類に対応した加点
    {
        typeScores[(int)scoreType] += _score;
    }
    public int GetDonutTypeScore(ObjectReferenceManeger.DonutScoreType scoreType)
    {
        return typeScores[(int)scoreType];
    }

    public int GetDonutTotalScore()//このドーナツのスコアの合計を返す
    {
        int totalScore = 0;
        for(int i = 0; i < typeScores.Length; i++)
        {
            totalScore += typeScores[i];
        }
        return totalScore;
    }
}
