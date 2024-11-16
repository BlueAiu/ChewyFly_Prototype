using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ResultManager : MonoBehaviour//ドーナツのスコアを一つずつカウントしていくクラス
{
    ResultAddScoreText[] addScoreTexts;
    int donutIndex = 0;
    Vector3 cameraPoint_from;
    Vector3 cameraPoint_to;
    float scoreCountTimer = 0f;
    int directingScore = 0;
    int oneDonutScore = 0;
    int directingMadeDonutNum = 0;
    Vector3 defaultMadeDonutTextLocalScale;
    void InitializeScoreCountClass()
    {
        addScoreTexts = new ResultAddScoreText[(int)ObjectReferenceManeger.DonutScoreType.End];
        for (int i = 0; i < (int)ObjectReferenceManeger.DonutScoreType.End; i++)
        {
            addScoreTexts[i] = Instantiate(addScoreTextPrefab, defaultAddScoreTextPos).GetComponent<ResultAddScoreText>();
            Vector3 pos = defaultAddScoreTextPos.position;
            pos.y -= downAddScoreTextY * i;
            addScoreTexts[i].SetDefaultPosition(pos);
        }
        resultUIParent.SetActive(false);
        ChangeCamera(false);
        SetScoreText(0);
        SetMadeDonutText(0);
        AddOneDonutScore();

        cameraPoint_to = donuts[donutIndex].transform.position;
        cameraPoint_to.y += donutCameraHeightY;
        donutsCamera.gameObject.transform.position = cameraPoint_to;
    }
    void CountScoreUpdate()
    {
        scoreCountTimer += Time.deltaTime;
        switch (scoreCountState)
        {
            case ScoreCountState.ShowDonut:
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
            case ScoreCountState.MoveToNextDonut:
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
    private void AddOneDonutScore()
    {
        scoreCountState = ScoreCountState.ShowDonut;
        scoreCountTimer = 0f;

        donuts[donutIndex].SetActive(true);
        DonutScoreSaver saver = donuts[donutIndex].GetComponent<DonutScoreSaver>();
        int textIndex = 0;
        for (int i = 0; i < (int)ObjectReferenceManeger.DonutScoreType.End; i++)
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
    private void MoveToNextDonut()
    {
        donutIndex++;
        if (donutIndex >= donuts.Count)
        {
            for (int i = 0; i < (int)ObjectReferenceManeger.DonutScoreType.End; i++)
            {
                addScoreTexts[i].CloseText();
            }
            DesplayResultUI();
            return;
        }
        scoreCountState = ScoreCountState.MoveToNextDonut;
        scoreCountTimer = 0f;

        cameraPoint_from = donuts[donutIndex - 1].transform.position;
        cameraPoint_from.y += donutCameraHeightY;
        cameraPoint_to = donuts[donutIndex].transform.position;
        cameraPoint_to.y += donutCameraHeightY;

        for (int i = 0; i < (int)ObjectReferenceManeger.DonutScoreType.End; i++)
        {
            addScoreTexts[i].CloseText();
        }

        directingScore += oneDonutScore;
        SetScoreText(directingScore);
    }
}
