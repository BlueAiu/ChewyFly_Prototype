using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    void Awake()
    {
        Time.timeScale = 1.0f;//TimeScaleは最初で1にしておく
    }

    public void LoadSceneName(string sceneName)//渡されたシーン名のシーンを読み込みます
    {
        SceneManager.LoadScene(sceneName);
    }
    public void LoadNowScene()//現在のシーンを再ロード
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()  //ゲームを終了する
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
