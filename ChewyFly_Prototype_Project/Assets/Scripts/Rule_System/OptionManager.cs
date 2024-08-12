using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour
{
    [SerializeField] private Button bgmButton;
    [SerializeField] private Button seButton;
    [SerializeField] private Button hajikiPowerButton;
    [SerializeField] private Button cameraButton;

    // Start is called before the first frame update
    void Start()
    {
        bgmButton.Select();//始まった時点でスタートボタンを選択状態にしておきます
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
