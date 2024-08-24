using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class OptionManager : MonoBehaviour
{
    InputScript input;

    [SerializeField] private Button bgmButton;
    [SerializeField] private Button seButton;
    [SerializeField] private Button hajikiPowerButton;
    [SerializeField] private Button cameraButton;

    void Awake()
    {
        input = GetComponent<InputScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        bgmButton.Select();//始まった時点でスタートボタンを選択状態にしておきます
    }

    // Update is called once per frame
    void Update()
    {
        if (input.isEastButton())
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
}
