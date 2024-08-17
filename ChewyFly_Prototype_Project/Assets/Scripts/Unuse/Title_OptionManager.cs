using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title_OptionManager : MonoBehaviour//optionを一つのシーンで管理する場合
{
    [SerializeField] private GameObject TitleUIParent;
    [SerializeField] private GameObject OptionUIParent;
    InputScript input;
    enum TITLESCENE {Title, Option, Credit }
    TITLESCENE titleScene = TITLESCENE.Title;
    OptionManager option;
    TitleManager titleManager;


    // Start is called before the first frame update
    void Start()
    {
        option = GetComponent<OptionManager>();
        titleManager = GetComponent<TitleManager>();
        input = GetComponent<InputScript>();
        OpenTitleUI();
    }
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
        //titleManager.startButton.Select();//これを有効にする場合はstartButtonをpublicにする
    }
    public void OpenOptionUI()
    {
        titleScene = TITLESCENE.Option;
        TitleUIParent.SetActive(false);
        OptionUIParent.SetActive(true);
        option.OpenOption();
    }
}
