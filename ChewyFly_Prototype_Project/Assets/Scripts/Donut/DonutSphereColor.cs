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
    [Tooltip("ドーナツの色マテリアル")]
    [SerializeField] Material[] materials;

    float changeTimer = 0f;
    [Tooltip("ドーナツの変色時間")]
    [SerializeField] float[] changeTimes;

    [Tooltip("変色時間がずれる範囲")]
    [SerializeField] float changeTimeOffsetRange = 1f;

    [Tooltip("焦げる前のエフェクト")]
    [SerializeField] GameObject burntEffect;

    [Tooltip("焦げるエフェクトが始まる時間")]
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

    public void CopySphereColorValue(DonutSphereColor _sphereColor)//このコンポーネントに情報をコピーする
    {
        changeTimer = _sphereColor.changeTimer;
        BakedNum = _sphereColor.BakedNum;
        GetComponent<Renderer>().material = materials[BakedNum];
    }
    void SetBurntEffect(bool isActive)//burntEffectをオンオフする
    {
        if (burntEffect.activeSelf == isActive) return;//すでにその状態なら返す
        burntEffect.SetActive(isActive);
    }
    public void StopBurntEffect()//エフェクトが出ないようタイマーを減らしてエフェクトを止める
    {
        const float reduceTime = 10f;
        if (BakedNum == (int)DonutBakedState.Second && changeTimer + burntWarningTime > changeTimes[(int)DonutBakedState.Second])
            changeTimer = changeTimes[(int)DonutBakedState.Second] - burntWarningTime - reduceTime;

        SetBurntEffect(false);
    }
}
