using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    void Awake()
    {
        Time.timeScale = 1.0f;
    }
    void Update()
    {
        Debug.Log(Time.timeScale);
    }
    public void LoadSceneName(string sceneName)//渡されたシーン名のシーンを読み込みます
    {
        SceneManager.LoadScene(sceneName);
    }
    public void LoadNowScene()//現在のシーンを再ロード
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
