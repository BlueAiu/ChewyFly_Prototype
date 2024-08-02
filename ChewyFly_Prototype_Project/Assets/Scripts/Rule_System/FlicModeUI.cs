using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlicModeUI : MonoBehaviour
{
    TMP_Text TMPtext;
    [SerializeField] FlicStrength player;

    private void Awake()
    {
        TMPtext = GetComponent<TMP_Text>();
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
            TMPtext.text = "Jump";
        }
        else
        {
            TMPtext.text = "Donut";
        }
    }
}
