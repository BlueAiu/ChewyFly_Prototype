using UnityEngine;

public class FlicModeUI : MonoBehaviour
{
    //TMP_Text TMPtext;
    UnityEngine.UI.Image image_;
    [SerializeField] FlicStrength player;

    [SerializeField] Sprite donutModeSprite;
    [SerializeField] Sprite jumpModeSprite;

    private void Awake()
    {
        //TMPtext = GetComponent<TMP_Text>();
        image_ = GetComponent<UnityEngine.UI.Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isJumpMode)
        {
            //TMPtext.text = "Jump";
            image_.sprite = jumpModeSprite;
        }
        else
        {
            //TMPtext.text = "Donut";
            image_.sprite= donutModeSprite;
        }
    }
}
