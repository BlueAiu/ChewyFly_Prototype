using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StageSelectManager : MonoBehaviour
{
    InputScript input;

    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button creditButton;

    void Awake()
    {
        input = GetComponent<InputScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        startButton.Select();
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
