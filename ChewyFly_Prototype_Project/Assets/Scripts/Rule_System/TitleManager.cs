using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button creditButton;

    [Header("BGM")]
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private AudioClip BGM;

    // Start is called before the first frame update
    void Start()
    {
        startButton.Select();

        soundManager.PlayBGM(BGM);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
