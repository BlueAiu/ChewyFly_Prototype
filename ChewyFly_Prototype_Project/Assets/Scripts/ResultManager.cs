using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button titleButton;
    // Start is called before the first frame update
    void Start()
    {
        restartButton.Select();//始まった時点でリスタートボタンを選択状態にしておきます
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadSceneName(string sceneName)//渡されたシーン名のシーンを読み込みます
    {
        SceneManager.LoadScene(sceneName);
    }
}
