using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum DonutBakedState
{
    Unburnt,
    First,
    Second,
    Burnt
}

public class DonutSphereColor : MonoBehaviour
{
    [Tooltip("�h�[�i�c�̐F�}�e���A��")]
    [SerializeField] Material[] materials;

    float changeTimer = 0f;
    [Tooltip("�h�[�i�c�̕ϐF����")]
    [SerializeField] float[] changeTimes;

    [Tooltip("�ϐF���Ԃ������͈�")]
    [SerializeField] float changeTimeOffsetRange = 1f;

    public int BakedNum { get; private set; } = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material = materials[BakedNum];
        for(int i = 0; i < changeTimes.Length; i++)
        {
            float timeOffset = Random.Range(-changeTimeOffsetRange, changeTimeOffsetRange);
            changeTimes[i] += timeOffset;
            if (i < changeTimes.Length - 1)
            {
                changeTimes[i + 1] -= timeOffset;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BakeDonut()
    {
        if (BakedNum < changeTimes.Length)
        {
            changeTimer += Time.deltaTime;

            if (changeTimer > changeTimes[BakedNum])
            {
                BakedNum++;
                GetComponent<Renderer>().material = materials[BakedNum];
                changeTimer = 0f;
            }
        }
    }

    public void CopySphereColorValue(DonutSphereColor _sphereColor)//���̃R���|�[�l���g�ɏ����R�s�[����
    {
        changeTimer = _sphereColor.changeTimer;
        BakedNum = _sphereColor.BakedNum;
        GetComponent<Renderer>().material = materials[BakedNum];
    }
}
