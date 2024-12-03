using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    void Awake()
    {
        Time.timeScale = 1.0f;//TimeScale�͍ŏ���1�ɂ��Ă���
    }

    public void LoadSceneName(string sceneName)//�n���ꂽ�V�[�����̃V�[����ǂݍ��݂܂�
    {
        SceneManager.LoadScene(sceneName);
    }
    public void LoadNowScene()//���݂̃V�[�����ă��[�h
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()  //�Q�[�����I������
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
