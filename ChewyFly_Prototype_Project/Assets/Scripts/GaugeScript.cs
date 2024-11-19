using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeScript : MonoBehaviour
{
    public GameObject GaugeInsideUI;//ゲージ内部UIオブジェクト

    float GaugeMax = 1000.0f;//ゲージ最大値
    float GaugeRemain = 0.0f;//ゲージ残量

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;//FPSを60に固定
    }

    // Update is called once per frame
    void Update()
    {
        if (GaugeRemain <= GaugeMax)
        {
            GaugeRemain += 1.0f;//ゲージ残量を1フレームごとに1ずつ増やす
        }
        float remaining = GaugeRemain / GaugeMax;
        GaugeInsideUI.GetComponent<Image>().fillAmount = remaining;
    }
}
