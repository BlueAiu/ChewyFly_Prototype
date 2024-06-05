using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button creditButton;
    // Start is called before the first frame update
    void Start()
    {
        startButton.Select();//始まった時点でスタートボタンを選択状態にしておきます
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
