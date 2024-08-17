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

    /*[SerializeField] private GameObject TitleUIParent;
    [SerializeField] private GameObject OptionUIParent;
    InputScript input;
    enum TITLESCENE {Title, Option, Credit }
    TITLESCENE titleScene = TITLESCENE.Title;
    OptionManager option;*/


    // Start is called before the first frame update
    void Start()
    {
        startButton.Select();
        /*option = GetComponent<OptionManager>();
        input = GetComponent<InputScript>();
        OpenTitleUI();*/
    }
    /*
    // Update is called once per frame
    void Update()
    {
        if (titleScene == TITLESCENE.Option && !option.OnUseSlider)//スライダーを選択していない
        {
            if (input.isBButton())
            {
                OpenTitleUI();
            }
        }
    }
    public void OpenTitleUI()
    {
        titleScene = TITLESCENE.Title;
        TitleUIParent.SetActive(true);
        OptionUIParent.SetActive(false);
        startButton.Select();
    }
    public void OpenOptionUI()
    {
        titleScene = TITLESCENE.Option;
        TitleUIParent.SetActive(false);
        OptionUIParent.SetActive(true);
        option.OpenOption();
    }*/
}
