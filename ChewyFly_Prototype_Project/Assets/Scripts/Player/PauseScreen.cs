using UnityEngine;
using UnityEngine.UI;

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
        SetMenuActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (input.isBackButton())
        {
            if (isPause)
            {
                Time.timeScale = 1f;
                SetMenuActive(false);
            }
            else
            {
                Time.timeScale = 0;
                SetMenuActive(true);
            }
        }
    }
    void SetMenuActive(bool isActive)//���j���[�̕\��/��\��
    {
        isPause = isActive;
        pauseMenuParent.SetActive(isActive);
        if (isActive)
        {
            restartButton.Select();
        }
    }
}
