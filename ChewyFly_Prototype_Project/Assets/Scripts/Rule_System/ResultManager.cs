using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class ResultManager : MonoBehaviour
{
    [Header("参照用の項目")]
    [SerializeField] GameObject addScoreTextPrefab;
    [SerializeField] Camera resultCamera;
    [SerializeField] Camera donutsCamera;
    [SerializeField] Transform donutSetCenterPosition;
    [Tooltip("ドーナツを一つ映した時のエフェクト")]
    [SerializeField] GameObject oneDonutEffect;
    [SerializeField] Animator PlayerAnimation;
    [SerializeField] Animator StickAnimation;

    [Header("UI")]

    [Tooltip("演出時には隠すuiの親")]
    [SerializeField] GameObject resultUIParent;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button titleButton;
    [SerializeField] RectTransform defaultAddScoreTextPos;
    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject skipText;
    [SerializeField] TMP_Text highScoreText;

    enum ScoreCountState { ShowDonut, MoveToNextDonut, Finish }//スコアの加算演出の状態
    ScoreCountState scoreCountState;
    
    List<GameObject> donuts = ObjectReferenceManeger.completeDonuts;

    [Header("Sound")]

    [Tooltip("鳴らすBGMを分けるノルマスコア")]
    [SerializeField] int scoreQuota = 800;
    [SerializeField] SoundManager soundManager;
    [SerializeField] AudioClip clearBGM;
    [SerializeField] AudioClip failureBGM;
    [SerializeField] AudioClip normalSE;
    [SerializeField] AudioClip shapeSE;

    InputScript input;

    const float shiftYPeriod = 1f;

    bool isHighScore = false;//ハイスコアが出たか?
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputScript>();
        if (input == null) { gameObject.AddComponent<InputScript>(); input = GetComponent<InputScript>(); }

        //Vector3 donutUnionsCenterPos = Vector3.zero;
        //foreach (var d in donuts)  { donutUnionsCenterPos += d.transform.position;   }
        //donutUnionsCenterPos /= donuts.Count;
        defaultMadeDonutTextLocalScale = resultText.transform.localScale;

        foreach (var i in donuts)
        {
            //i.transform.position = i.transform.position + (donutSetCenterPosition.transform.position - donutUnionsCenterPos);
            //i.SetActive(false);
            i.GetComponent<DonutRigidBody>().isConstraints = false;
            i.transform.position = donutSetCenterPosition.position;
            i.transform.rotation = Quaternion.identity;
            i.transform.Rotate(-60, 0, 0);
            i.GetComponent<Rigidbody>().isKinematic = true;
            i.SetActive(false);
        }

        if (donuts.Count == 0)
        {
            DesplayResultUI(); 
        }
        else//作成したドーナツがあるなら演出を入れる
        {
            InitializeScoreCountClass();
        }
        isHighScore = ObjectReferenceManeger.SetHighScore(ObjectReferenceManeger.totalScore);
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
            i.GetComponent<Rigidbody>().isKinematic = false;
        }
        float shiftY = 0;
        for(int i = donutIndex; i < donuts.Count; i++)
        {
            donuts[i].transform.position += Vector3.up * shiftY;
            shiftY += shiftYPeriod;
        }
        ChangeCamera(true);

        SetMadeDonutText(ObjectReferenceManeger.madeDonuts);
        SetScoreText(ObjectReferenceManeger.totalScore);

        if (ObjectReferenceManeger.totalScore < scoreQuota)
        {
            soundManager.PlayBGM(failureBGM);
            PlayerAnimation.SetTrigger("Failture");
            StickAnimation.SetTrigger("Failture");
        }
        else
        {
            soundManager.PlayBGM(clearBGM);
            PlayerAnimation.SetTrigger("Clear");
            StickAnimation.SetTrigger("Clear");
        }

        if (isHighScore)
        {
            highScoreText.gameObject.SetActive(true);
            highScoreText.text = "ハイスコア!";
        }
        else
        {
            highScoreText.gameObject.SetActive(false);
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
