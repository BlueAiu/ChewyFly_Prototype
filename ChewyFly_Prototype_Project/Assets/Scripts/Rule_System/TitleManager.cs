using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private TMP_Text highScoreText;

    [Header("BGM")]
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private AudioClip BGM;

    // Start is called before the first frame update
    void Start()
    {
        startButton.Select();

        soundManager.PlayBGM(BGM);

        highScoreText.text = "ハイスコア " + ObjectReferenceManeger.HighScore;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
