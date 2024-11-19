using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DonutBakedState
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

    [Tooltip("�ł���O�̃G�t�F�N�g")]
    [SerializeField] GameObject burntEffect;

    [Tooltip("�ł���G�t�F�N�g���n�܂鎞��")]
    [SerializeField] float burntWarningTime = 3f;

    public int BakedNum { get; private set; } = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material = materials[BakedNum];
        for (int i = 0; i < changeTimes.Length; i++)
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
        if (BakedNum == (int)DonutBakedState.Second
            && changeTimer + burntWarningTime > changeTimes[(int)DonutBakedState.Second])
        {
            SetBurntEffect(true);
        }
    }

    public void BakeDonut(int donutNum)
    {
        if (BakedNum < changeTimes.Length)
        {
            changeTimer += Time.deltaTime / donutNum;

            if (changeTimer > changeTimes[BakedNum])
            {
                BakedNum++;
                GetComponent<Renderer>().material = materials[BakedNum];
                changeTimer = 0f;

                if (BakedNum == (int)DonutBakedState.Burnt)
                {
                    SetBurntEffect(false);
                }
            }
        }
    }

    public void CopySphereColorValue(DonutSphereColor _sphereColor)//���̃R���|�[�l���g�ɏ����R�s�[����
    {
        changeTimer = _sphereColor.changeTimer;
        BakedNum = _sphereColor.BakedNum;
        GetComponent<Renderer>().material = materials[BakedNum];
    }
    void SetBurntEffect(bool isActive)//burntEffect���I���I�t����
    {
        if (burntEffect.activeSelf == isActive) return;//���łɂ��̏�ԂȂ�Ԃ�
        burntEffect.SetActive(isActive);
    }
    public void StopBurntEffect()//�G�t�F�N�g���o�Ȃ��悤�^�C�}�[�����炵�ăG�t�F�N�g���~�߂�
    {
        const float reduceTime = 10f;
        if (BakedNum == (int)DonutBakedState.Second && changeTimer + burntWarningTime > changeTimes[(int)DonutBakedState.Second])
            changeTimer = changeTimes[(int)DonutBakedState.Second] - burntWarningTime - reduceTime;

        SetBurntEffect(false);
    }
}
