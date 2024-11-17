using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ResultManager : MonoBehaviour//ドーナツのスコアを一つずつカウントしていく演出用のクラス
{
    [Header("演出上の項目")]
    [SerializeField] AnimationCurve cameraMoveCurve;
    [Tooltip("ドーナツを映すカメラの高さ")]
    [SerializeField] float donutCameraHeightY = 30f;
    [SerializeField] AnimationCurve madeDonutTextScaleCurve;
    [Tooltip("一つのドーナツを映す時間")]
    [SerializeField] float time_OneDonut = 0.5f;
    [Tooltip("次のドーナツに移動する時間")]
    [SerializeField] float time_cameraMove = 0.5f;

    ResultAddScoreText[] addScoreTexts;//resultの演出時の文字(事前にすべて用意し、見え隠れさせる)
    int donutIndex = 0;//どのindexのドーナツを見ているか

    Vector3 cameraPoint_from;//ここから
    Vector3 cameraPoint_to;//ここまでカメラが移動する
    float scoreCountTimer = 0f;//演出用のタイマー
    int directingScore = 0;//演出表示用のスコア
    int oneDonutScore = 0;//演出時のそのドーナツのスコア
    int directingMadeDonutNum = 0;//演出用の作成ドーナツ数
    Vector3 defaultMadeDonutTextLocalScale;

    void InitializeScoreCountClass()//演出を行う場合
    {
        addScoreTexts = new ResultAddScoreText[(int)ObjectReferenceManeger.DonutScoreType.End];
        Vector3 nextPoint = defaultAddScoreTextPos.position;
        for (int i = 0; i < (int)ObjectReferenceManeger.DonutScoreType.End; i++)//横から出てくる細かいスコアuiの初期化
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
    private void AddOneDonutScore()//ドーナツのスコアを加算する演出を始める
    {
        scoreCountState = ScoreCountState.ShowDonut;
        scoreCountTimer = 0f;
        donuts[donutIndex].SetActive(true);
        DonutsUnionScript union = donuts[donutIndex].GetComponent<DonutsUnionScript>();
        Instantiate(oneDonutEffect, union.GetDonutsCenterPoint(), Quaternion.identity);

        DonutScoreSaver saver = donuts[donutIndex].GetComponent<DonutScoreSaver>();
        int textIndex = 0;
        for (int i = 0; i < (int)ObjectReferenceManeger.DonutScoreType.End; i++)//そのドーナツのスコア状態を取得
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
    void CountScoreUpdate()//演出用のUpdate
    {
        scoreCountTimer += Time.deltaTime;
        switch (scoreCountState)
        {
            case ScoreCountState.ShowDonut://ドーナツを見せている
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
            case ScoreCountState.MoveToNextDonut://次のドーナツに移行中
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
    private void MoveToNextDonut()//次のドーナツに移動する準備をする
    {
        donutIndex++;
        if (donutIndex >= donuts.Count)//すべてのドーナツを見せ終わったら終了
        {
            CloseAllAddScoreTexts();
            DesplayResultUI();
            return;
        }
        scoreCountState = ScoreCountState.MoveToNextDonut;
        scoreCountTimer = 0f;

        cameraPoint_from = GetDonutCenterPoint(donuts[donutIndex - 1]);//カメラの位置を設定
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
    Vector3 GetDonutCenterPoint(GameObject donut)//unionを取り出して真ん中を返す
    {
        DonutsUnionScript union = donut.GetComponent<DonutsUnionScript>();
        if (union != null) return union.GetDonutsCenterPoint();
        return Vector3.zero;
    }
}
