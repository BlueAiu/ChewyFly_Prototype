using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title_OptionManager : MonoBehaviour//option����̃V�[���ŊǗ�����ꍇ
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
        if (titleScene == TITLESCENE.Option && !option.OnUseSlider)//�X���C�_�[��I�����Ă��Ȃ�
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
        //titleManager.startButton.Select();//�����L���ɂ���ꍇ��startButton��public�ɂ���
    }
    public void OpenOptionUI()
    {
        titleScene = TITLESCENE.Option;
        TitleUIParent.SetActive(false);
        OptionUIParent.SetActive(true);
        option.OpenOption();
    }
}
