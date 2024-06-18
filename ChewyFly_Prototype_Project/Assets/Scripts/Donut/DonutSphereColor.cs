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
    [Tooltip("ドーナツの色マテリアル")]
    [SerializeField] Material[] materials;

    float changeTimer = 0f;
    [Tooltip("ドーナツの変色時間")]
    [SerializeField] float[] changeTimes;

    int colorNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material = materials[colorNum];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BakeDonut()
    {
        if (colorNum < changeTimes.Length)
        {
            changeTimer += Time.deltaTime;

            if (changeTimer > changeTimes[colorNum])
            {
                colorNum++;
                GetComponent<Renderer>().material = materials[colorNum];
                changeTimer = 0f;
            }
        }
    }

    public int BakedValue()
    {
        return colorNum;
    }
}
