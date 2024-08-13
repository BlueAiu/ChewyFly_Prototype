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
    void AddDonutScore(GameObject donut)//ドーナツのドーナツの形を評価し、加点
    {
        int donutScore = donutScore_base;//まず基礎点

        if (IsIdealDonut(donut)) //理想的な形なら加算
        {
            donutScore += donutScore_ideal;
        }

        int unionCount = donut.GetComponent<DonutsUnionScript>().unionCount;
        if(unionCount > idealDonutNum) //ドーナツの数が六個を超えたなら加算
        {
            donutScore += (unionCount - idealDonutNum) * donutScore_over;
        }

        totalScore += donutScore;
        SetDonutScoreText();
    }
    void SetDonutScoreText() //UIに現在のスコアを表示
    {
        donutScoreText.text = "Score:" + totalScore.ToString();
    }
}
