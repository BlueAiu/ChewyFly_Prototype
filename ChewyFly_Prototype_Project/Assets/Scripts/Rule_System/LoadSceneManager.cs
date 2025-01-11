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
    public void LoadMainSceneFromTitle()//�^�C�g�����烁�C���V�[���Ɉڍs����ꍇ
    {
        if (ObjectReferenceManeger.HighScore <= 0)//MainScene�Ɉڍs�����n�C�X�R�A���܂�0�ȉ�
        {
            TutorialManager.TransitionToMainGame();
            SceneManager.LoadScene("TutorialScene");
            return;
        }
        SceneManager.LoadScene("MainScene");
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
