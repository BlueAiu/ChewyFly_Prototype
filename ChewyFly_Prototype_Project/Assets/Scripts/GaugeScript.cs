using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeScript : MonoBehaviour
{
    public GameObject GaugeInsideUI;//�Q�[�W����UI�I�u�W�F�N�g

    float GaugeMax = 1000.0f;//�Q�[�W�ő�l
    float GaugeRemain = 0.0f;//�Q�[�W�c��

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;//FPS��60�ɌŒ�
    }

    // Update is called once per frame
    void Update()
    {
        if (GaugeRemain <= GaugeMax)
        {
            GaugeRemain += 1.0f;//�Q�[�W�c�ʂ�1�t���[�����Ƃ�1�����₷
        }
        float remaining = GaugeRemain / GaugeMax;
        GaugeInsideUI.GetComponent<Image>().fillAmount = remaining;
    }
}
