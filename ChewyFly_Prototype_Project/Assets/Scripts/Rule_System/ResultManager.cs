using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button titleButton;

    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text scoreText;

    [SerializeField] Transform donutSetCenterPosition;

    List<GameObject> donuts = ObjectReferenceManeger.completeDonuts;

    [Tooltip("鳴らすBGMを分けるノルマスコア")]
    [SerializeField] int scoreQuota = 800;
    [SerializeField] SoundManager soundManager;
    [SerializeField] AudioClip clearBGM;
    [SerializeField] AudioClip failureBGM;

    // Start is called before the first frame update
    void Start()
    {
        restartButton.Select();//始まった時点でリスタートボタンを選択状態にしておきます

        resultText.text = "You made " +
            ObjectReferenceManeger.madeDonuts.ToString() + " donuts.";
        scoreText.text = "Score:  " +
            ObjectReferenceManeger.totalScore.ToString();

        Vector3 donutUnionsCenterPos = Vector3.zero;
        foreach (var d in donuts)  { donutUnionsCenterPos += d.transform.position;   }
        donutUnionsCenterPos /= donuts.Count;

        foreach(var i in donuts)
        {
            i.transform.position = i.transform.position + (donutSetCenterPosition.transform.position - donutUnionsCenterPos);
            //i.GetComponent<Rigidbody>().isKinematic = true;
        }

        if(ObjectReferenceManeger.totalScore < scoreQuota)
        {
            soundManager.PlayBGM(failureBGM);
        }
        else
        {
            soundManager.PlayBGM(clearBGM);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        ObjectReferenceManeger.ClearCompleteDonuts();
    }
}
