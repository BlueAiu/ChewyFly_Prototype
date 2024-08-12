using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button creditButton;

    // Start is called before the first frame update
    void Start()
    {
        startButton.Select();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
