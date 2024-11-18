using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public partial class ObjectReferenceManeger : MonoBehaviour
{
    [SerializeField] public static int totalScore { get; private set; } = 0;

    [Header("ドーナツのスコア計算")]
    [Tooltip("基礎点")]
    [SerializeField] int donutScore_base = 160;
    [Tooltip("形が完璧な場合の追加点")]
    [SerializeField] int donutScore_ideal = 200;
    [Tooltip("6個を超えた場合の加算点")]
    [SerializeField] int donutScore_over = 30;

    [Tooltip("それぞれの焦げに対応する加算点(後になるほど焦げる)")]
    [SerializeField] int[] burntScores = new int[(int)DonutBakedState.Burnt + 1];

    [Header("スコア表示用のテキスト")]
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

    public enum DonutScoreType//スコアの種類(スコアの種類数を知るためにEndは最後に置く)
    {
        Base ,BurntColor, OverNum, Ideal, Pyramid, Flower, Straight, Infinity, End
    }

    bool IsIdealDonut(GameObject donut)
    {
        var donutShape = donut.GetComponent<DonutsUnionScript>().hexaPositions;
        
        return CheckDonutShape(donutShape) == idealDonutNum;
    }

    //理想の形と似ているかを見る
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
    void AddDonutScore(GameObject donut)//ドーナツのドーナツの形を評価し、加点。
    {
        donut.AddComponent<DonutScoreSaver>();//ScoreSaverに点を入れてから合計点に加点
        DonutScoreSaver scoreSaver = donut.GetComponent<DonutScoreSaver>();

        bool isIdeal = false;//ドーナツが理想の形か
        scoreSaver.AddDonutScoreType(DonutScoreType.Base, donutScore_base);//まず基礎点

        if (IsIdealDonut(donut)) //理想的な形なら加算
        {
            scoreSaver.AddDonutScoreType(DonutScoreType.Ideal, donutScore_ideal);
            isIdeal = true;
        }

        DonutsUnionScript unionScript = donut.GetComponent<DonutsUnionScript>();
        int unionCount = unionScript.unionCount;
        if(unionCount > idealDonutNum) //ドーナツの数が六個を超えたなら加算
        {
            scoreSaver.AddDonutScoreType(DonutScoreType.OverNum, (unionCount - idealDonutNum) * donutScore_over);
        }

        int[] burntDonutsNum = unionScript.GetBurntDonutsNum();//焦げの状態に応じて加点
        for(int i = 0; i < burntDonutsNum.Length; i++)
        {
            scoreSaver.AddDonutScoreType(DonutScoreType.BurntColor, burntDonutsNum[i] * burntScores[i]);
        }

        totalScore += scoreSaver.GetDonutTotalScore();//最後このドーナツについたスコアの分加点する
        SetDonutScoreText();

        CompleteDonutEffect(isIdeal, donut, unionScript.GetDonutsCenterPoint());//完成時のエフェクト
    }
    void SetDonutScoreText() //UIに現在のスコアを表示
    {
        donutScoreText.text = "Score:" + totalScore.ToString();
    }
}
