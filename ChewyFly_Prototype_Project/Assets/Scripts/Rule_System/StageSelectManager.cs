using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] private Button stage1Button;
    [SerializeField] private Button stage2Button;
    [SerializeField] private Button stage3Button;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button optionButton;

    // Start is called before the first frame update
    void Start()
    {
        stage1Button.Select();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
