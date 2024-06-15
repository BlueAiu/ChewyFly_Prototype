using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class PauseScreen : MonoBehaviour
{
    InputScript input;

    [Tooltip("�|�[�Y���j���[��UI�̐e")]
    [SerializeField] GameObject pauseMenuParent;

    [SerializeField] Button restartButton;
    [SerializeField] Button titleButton;

    bool isPause;
    // Start is called before the first frame update
    void Awake()
    {
        input = GetComponent<InputScript>();
        isPause = false;
        SetMenuActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (input.isBackButton())
        {
            if (isPause)
            {
                isPause = false;
                Time.timeScale = 1f;
                SetMenuActive(false);
            }
            else
            {
                isPause = true;
                Time.timeScale = 0;
                SetMenuActive(true);
            }
        }
    }
    void SetMenuActive(bool isActive)//���j���[�̕\��/��\��
    {
        pauseMenuParent.SetActive(isActive);
        if (isActive)
        {
            restartButton.Select();
        }
    }
}
