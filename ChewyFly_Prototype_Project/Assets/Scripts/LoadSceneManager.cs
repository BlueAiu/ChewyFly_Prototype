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
    public void LoadSceneName(string sceneName)//�n���ꂽ�V�[�����̃V�[����ǂݍ��݂܂�
    {
        SceneManager.LoadScene(sceneName);
    }
    public void LoadNowScene()//���݂̃V�[�����ă��[�h
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
