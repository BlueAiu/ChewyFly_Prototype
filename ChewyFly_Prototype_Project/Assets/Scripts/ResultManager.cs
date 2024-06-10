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
        restartButton.Select();//�n�܂������_�ŃX�^�[�g�{�^����I����Ԃɂ��Ă����܂�
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadSceneName(string sceneName)//�n���ꂽ�V�[�����̃V�[����ǂݍ��݂܂�
    {
        SceneManager.LoadScene(sceneName);
    }
}
