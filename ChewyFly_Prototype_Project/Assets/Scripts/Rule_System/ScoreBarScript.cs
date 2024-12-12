using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ScoreBarScript : MonoBehaviour
{
    [SerializeField] RectTransform whiteBar_;
    [SerializeField] RectTransform normalBar_;
    [SerializeField] RectTransform clearBar_;

    Bar whiteBar;
    Bar normalBar;
    Bar clearBar;


    const float normalWidth = 0.75f;
    const float clearWidth = 0.25f;

    float _score = 0;
    public float Score 
    {
        get
        {
            return _score;
        }
        set
        {
            _score = Mathf.Min(value, maxBarScore);
            whiteBar.Set(_score);
        } 
    }
    float shiftScore = 0;
    float maxBarScore;

    [Tooltip("バーの種類が変わるスコア値")]
    [SerializeField] float scoreQuota = 800;
    [Tooltip("バーが増加する速さ")]
    [SerializeField] float scoreShiftRate = 1f;

    // Start is called before the first frame update
    void Start()
    {
        maxBarScore = scoreQuota / normalWidth;

        whiteBar = new(maxBarScore, whiteBar_);
        normalBar = new(scoreQuota, normalBar_);
        clearBar = new(maxBarScore * clearWidth, clearBar_);

        whiteBar.Set(0);
        normalBar.Set(0);
        clearBar.Set(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(shiftScore < Score)
        {
            shiftScore += scoreShiftRate * Time.deltaTime;

            normalBar.Set(shiftScore);
            clearBar.Set(shiftScore - scoreQuota);
        }
    }
}
