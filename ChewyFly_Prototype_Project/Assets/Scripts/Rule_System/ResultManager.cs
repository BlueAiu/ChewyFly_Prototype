using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using static UnityEditor.Experimental.GraphView.GraphView;
using System;

public partial class ResultManager : MonoBehaviour
{
    [Header("参照用の項目")]
    [SerializeField] GameObject addScoreTextPrefab;
    [Tooltip("演出時には隠すuiの親")]
    [SerializeField] GameObject resultUIParent;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button titleButton;
    [SerializeField] RectTransform defaultAddScoreTextPos;
    [SerializeField] Camera resultCamera;
    [SerializeField] Camera donutsCamera;
    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Transform donutSetCenterPosition;
    [SerializeField] GameObject skipText;
    [Tooltip("ドーナツを一つ映した時のエフェクト")]
    [SerializeField] GameObject oneDonutEffect;

    enum ScoreCountState { ShowDonut, MoveToNextDonut, Finish }//スコアの加算演出の状態
    ScoreCountState scoreCountState;
    
    List<GameObject> donuts = ObjectReferenceManeger.completeDonuts;

    [Tooltip("鳴らすBGMを分けるノルマスコア")]
    [SerializeField] int scoreQuota = 800;
    [SerializeField] SoundManager soundManager;
    [SerializeField] AudioClip clearBGM;
    [SerializeField] AudioClip failureBGM;

    InputScript input;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputScript>();
        if (input == null) { gameObject.AddComponent<InputScript>(); input = GetComponent<InputScript>(); }

        Vector3 donutUnionsCenterPos = Vector3.zero;
        foreach (var d in donuts)  { donutUnionsCenterPos += d.transform.position;   }
        donutUnionsCenterPos /= donuts.Count;
        defaultMadeDonutTextLocalScale = resultText.transform.localScale;

        foreach (var i in donuts)
        {
            i.transform.position = i.transform.position + (donutSetCenterPosition.transform.position - donutUnionsCenterPos);
            i.SetActive(false);
            //i.GetComponent<Rigidbody>().isKinematic = true;
        }

        if (donuts.Count == 0)
        {
            DesplayResultUI(); 
        }
        else//作成したドーナツがあるなら演出を入れる
        {
            InitializeScoreCountClass();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (scoreCountState != ScoreCountState.Finish)
        {
            if (input.isBButton())//スキップした
            {
                CloseAllAddScoreTexts();
                DesplayResultUI();
            }
            else
            {
                CountScoreUpdate();
            }
        }
    }

    void DesplayResultUI()//ボタン等を表示して、ドーナツをカウントする演出は終了
    {
        scoreCountState = ScoreCountState.Finish;
        resultUIParent.SetActive(true);
        skipText.SetActive(false);
        restartButton.Select();//始まった時点でリスタートボタンを選択状態にしておきます
        foreach (var i in donuts)
        {
            i.SetActive(true);
        }
        ChangeCamera(true);

        SetMadeDonutText(ObjectReferenceManeger.madeDonuts);
        SetScoreText(ObjectReferenceManeger.totalScore);

        if (ObjectReferenceManeger.totalScore < scoreQuota)
        {
            soundManager.PlayBGM(failureBGM);
        }
        else
        {
            soundManager.PlayBGM(clearBGM);
        }
    }
    void ChangeCamera(bool activeResultCamera)//カメラの切り替え
    {
        resultCamera.gameObject.SetActive(activeResultCamera);
        donutsCamera.gameObject.SetActive(!activeResultCamera);
    }
    void SetMadeDonutText(int _num)
    {
        resultText.transform.localScale = defaultMadeDonutTextLocalScale;
        resultText.text = "ドーナツを " + _num.ToString() + " コ作った";
    }
    void SetScoreText(int _score)
    {
        scoreText.text = "スコア :  " + _score.ToString();
    }
    private void OnDestroy()
    {
        ObjectReferenceManeger.ClearCompleteDonuts();
    }
}
