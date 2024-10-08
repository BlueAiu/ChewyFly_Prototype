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

    // Start is called before the first frame update
    void Start()
    {
        restartButton.Select();//始まった時点でリスタートボタンを選択状態にしておきます

        resultText.text = "You made " +
            ObjectReferenceManeger.madeDonuts.ToString() + " donuts.";
        scoreText.text = "Score:  " +
            ObjectReferenceManeger.totalScore.ToString();

        foreach(var i in ObjectReferenceManeger.completeDonuts)
        {
            i.transform.position = Vector3.zero;
            i.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
