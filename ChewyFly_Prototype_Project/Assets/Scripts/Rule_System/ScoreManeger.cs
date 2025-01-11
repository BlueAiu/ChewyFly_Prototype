using System.Collections.Generic;
using TMPro;
using UnityEngine;

public partial class ObjectReferenceManeger : MonoBehaviour
{
    public static int HighScore 
    {
        get
        {
            if (PlayerPrefs.HasKey("HighScore"))
            {
                return PlayerPrefs.GetInt("HighScore");
            }
            else 
            { 
            return 0;
            }
        }
        private set
        {
            PlayerPrefs.SetInt("HighScore", value);
        }
    }

    [SerializeField] public static int totalScore { get; private set; } = 0;

    [Header("ドーナツのスコア計算")]
    [Tooltip("基礎点")]
    [SerializeField] int donutScore_base = 160;
    [Tooltip("形が完璧な場合の追加点")]
    [SerializeField] int donutScore_ideal = 400;
    [Tooltip("追加点:直線")]
    [SerializeField] int donutScore_straight = 200;
    [Tooltip("追加点:フラワー型")]
    [SerializeField] int donutScore_flower = 200;
    [Tooltip("追加点:ピラミッド型")]
    [SerializeField] int donutScore_pyramid = 200;
    [Tooltip("追加点:インフィニティ")]
    [SerializeField] int donutScore_infinity = 800;
    [Tooltip("6個を超えた場合の加算点")]
    [SerializeField] int donutScore_over = 30;

    [Tooltip("それぞれの焦げに対応する加算点(後になるほど焦げる)")]
    [SerializeField] int[] burntScores = new int[(int)DonutBakedState.Burnt + 1];

    [Header("スコア表示用のテキスト")]
    [SerializeField] TMP_Text donutScoreText;
    [SerializeField] TMP_Text donutNumText;
    [SerializeField] ScoreBarScript scoreBar;

    [SerializeField] GameObject canvas;
    [SerializeField] GameObject completeReactionScoreUIPrefab;
    [Tooltip("スコアのUIが出現する時のそれぞれずれの時間")]
    [SerializeField] float scoreUIAppearDiffTime = 0.2f;
    [SerializeField] Transform scorePos;
    [SerializeField] Transform scorePos_Next;//次のUIを置く高さ
    int scoreTypeNum = 0;
    [Tooltip("完成時のエフェクトの発生する時のカメラからの距離")]
    [SerializeField] float completeEffectCameraDistance = 5f;

    const int checkRange = 10;
    const int idealDonutNum = 6;
    const int pyramidDonutNum = 6;
    const int largePyramidDonutNum = 10;
    const int flowerDonutNum = 7;
    const int infinityDonutNum = 10;

    readonly Vector2[] circleShapePos = new Vector2[]
    {
        new Vector2(-1, 1),
        new Vector2(0, 1),
        new Vector2(-1, 0),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(1, -1),

        new Vector2(0,0)
    };

    readonly Vector2[] pyramidShapePos = new Vector2[]
   {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(0, 2),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(2, 0),

        new Vector2(0, 3),
        new Vector2(1, 2),
        new Vector2(2, 1),
        new Vector2(3, 0)
   };

    public enum DonutScoreType//スコアの種類(スコアの種類数を知るためにEndは最後に置く)
    {
        Base ,BurntColor, OverNum, Ideal, Pyramid, Flower, Straight, Infinity, End
    }

    bool IsIdealDonut(GameObject donut)
    {
        var donutShape = donut.GetComponent<DonutsUnionScript>().hexaPositions;

        return CheckDonutShape(donutShape, circleShapePos,idealDonutNum) > 0;
    }

    //ドーナツの特別な形状をenum:DonutScoreTypeで返す
    //特殊な形状出ない場合、Baseを返す
    DonutScoreType DonutShapeType(GameObject donut)
    {
        var donutsPos = donut.GetComponent<DonutsUnionScript>().hexaPositions;
        int donutCount = donutsPos.Count;

        if (donutCount == idealDonutNum && CheckDonutShape(donutsPos, circleShapePos, idealDonutNum) > 0)
            return DonutScoreType.Ideal;

        if (donutCount == pyramidDonutNum &&
            (CheckDonutShape(donutsPos, pyramidShapePos, pyramidDonutNum) > 0 || CheckDonutShape(donutsPos, pyramidShapePos, pyramidDonutNum, true) > 0)) 
            return DonutScoreType.Pyramid;

        if (donutCount == largePyramidDonutNum &&
            (CheckDonutShape(donutsPos, pyramidShapePos, largePyramidDonutNum) > 0 || CheckDonutShape(donutsPos, pyramidShapePos, largePyramidDonutNum, true) > 0))
            return DonutScoreType.Pyramid;

        if(donutCount == flowerDonutNum &&  CheckDonutShape(donutsPos, circleShapePos, flowerDonutNum) > 0)
            return DonutScoreType.Flower;

        if(donutCount == infinityDonutNum && CheckDonutShape(donutsPos, circleShapePos, idealDonutNum) == 2)
            return DonutScoreType.Infinity;

        if (CheckStaightShape(donutsPos))
            return DonutScoreType.Straight;

        return DonutScoreType.Base;
    }

    //理想の形と同じかを見る
    int CheckDonutShape(List<Vector2> donutsPos, Vector2[] shapePos, int checkNum, bool isInvert = false)
    {
        int invert = isInvert ? -1 : 1;
        int fullFitDonuts = 0;

        for(int i = -checkRange; i <= checkRange; i++)
        {
            for(int j = -checkRange; j <= checkRange; j++)
            {
                int fitDonuts = 0;
                Vector2 currentPos = new Vector2(i, j);

                for (int k = 0; k < checkNum; k++) 
                {
                    var v = shapePos[k];
                    if(donutsPos.Contains(currentPos + v * invert))
                    {
                        fitDonuts++;
                    }
                }

                if(fitDonuts == checkNum) fullFitDonuts++;
            }
        }

        return fullFitDonuts;
    }

    //ドーナツが直列であるか見る
    bool CheckStaightShape(List<Vector2> donutsPos)
    {
        var straightDir = donutsPos[1];

        if(straightDir.x == 0)
        {
            foreach (var d in donutsPos)
            {
                if (d.x != 0) return false;
            }
        }
        else if(straightDir.y == 0)
        {
            foreach (var d in donutsPos)
            {
                if (d.y != 0) return false;
            }
        }
        else if(straightDir.x == -straightDir.y)
        {
            foreach (var d in donutsPos)
            {
                if(d.x != -d.y) return false;
            }
        }
        else return false;

        return true;
    }

    DonutScoreType AddDonutScore(GameObject donut)//ドーナツのドーナツの形を評価し、加点。
    {
        donut.AddComponent<DonutScoreSaver>();//ScoreSaverに点を入れてから合計点に加点
        DonutScoreSaver scoreSaver = donut.GetComponent<DonutScoreSaver>();

        scoreTypeNum = 0;
        //bool isIdeal = false;//ドーナツが理想の形か
        AddOneScore(scoreSaver, DonutScoreType.Base, donutScore_base);//まず基礎点

        DonutsUnionScript unionScript = donut.GetComponent<DonutsUnionScript>();
        int unionCount = unionScript.unionCount;
        if(unionCount > idealDonutNum) //ドーナツの数が六個を超えたなら加算
        {
            AddOneScore(scoreSaver, DonutScoreType.OverNum, (unionCount - idealDonutNum) * donutScore_over);
        }

        int[] burntDonutsNum = unionScript.GetBurntDonutsNum();//焦げの状態に応じて加点
        int totalBurntScore = 0;
        for(int i = 0; i < burntDonutsNum.Length; i++)
        {
            totalBurntScore += burntDonutsNum[i] * burntScores[i];
        }
        AddOneScore(scoreSaver, DonutScoreType.BurntColor, totalBurntScore);

        DonutScoreType scoreType = DonutShapeType(donut); int typeScore = 0;
        switch (scoreType)
        {
            case DonutScoreType.Flower:
                typeScore = donutScore_flower;
                break;
            case DonutScoreType.Pyramid:
                typeScore = donutScore_pyramid;
                break;
            case DonutScoreType.Straight:
                typeScore = donutScore_straight;
                break;
            case DonutScoreType.Ideal:
                typeScore = donutScore_ideal;
                break;
            case DonutScoreType.Infinity:
                typeScore = donutScore_infinity;
                break;
        }
        AddOneScore(scoreSaver, scoreType, typeScore);

        totalScore += scoreSaver.GetDonutTotalScore();//最後このドーナツについたスコアの分加点する
        SetDonutScoreText();

        CompleteDonutEffect(scoreType);//完成時のエフェクト

        GameObject scoreUi = Instantiate(completeReactionScoreUIPrefab, canvas.transform);//スコアを表示
        int _score = scoreSaver.GetDonutTotalScore();

        ScoreUI_CompleteDonutReaction component = scoreUi.GetComponent<ScoreUI_CompleteDonutReaction>();
        Vector3 _scorePos = scorePos.position;
        component.ScoreInitialized(0, _score, _scorePos, scoreUIAppearDiffTime * scoreTypeNum);

        return scoreType;
    }
    void SetDonutScoreText() //UIに現在のスコアを表示
    {
        donutScoreText.text = " : " + totalScore.ToString();
        donutNumText.text = completeDonuts.Count.ToString() + " コ";

        scoreBar.Score = totalScore;
    }

    void AddOneScore(DonutScoreSaver _saver , DonutScoreType _type, int _score)//スコアを加算して表示
    {
        if (_score <= 0) return;
        _saver.AddDonutScoreType(_type, _score);//スコア加算

        //GameObject scoreUi = Instantiate(completeReactionScoreUIPrefab, canvas.transform);//スコアを表示
        //ScoreUI_CompleteDonutReaction component = scoreUi.GetComponent<ScoreUI_CompleteDonutReaction>();
        //Vector3 _scorePos = scorePos.position;
        //float scoreDiffHeight = scorePos_Next.transform.position.y - scorePos.transform.position.y;
        //_scorePos.y += scoreTypeNum * scoreDiffHeight;
        //component.ScoreInitialized(_type, _score, _scorePos, scoreUIAppearDiffTime * scoreTypeNum);

        scoreTypeNum++;
    }

    public static bool SetHighScore(int currentScore)//ハイスコアが出たらtrueを返す
    {
        if (currentScore > HighScore)
        {
            HighScore = currentScore;
            return true;
        }
        return false;
    }
}
