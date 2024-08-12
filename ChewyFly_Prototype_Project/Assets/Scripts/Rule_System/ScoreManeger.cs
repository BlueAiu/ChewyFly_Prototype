using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ObjectReferenceManeger : MonoBehaviour
{
    [Header("ドーナツのスコア計算")]
    //[SerializeField] int baseDonutScore = 160;

    const int checkRange = 10;
    const int idealDonutNum = 6;

    readonly Vector2[] idealShapePos = new Vector2[]
    {
        new Vector2(-1, 1),
        new Vector2(0, 1),
        new Vector2(-1, 0),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(1, -1)
    };



    bool IsIdealDonut(GameObject donut)
    {
        var donutShape = donut.GetComponent<DonutsUnionScript>().hexaPositions;
        
        return CheckDonutShape(donutShape) == idealDonutNum;
    }

    //理想の形と似ているかを見る
    int CheckDonutShape(List<Vector2> donutsPos)
    {
        int maxFitDonuts = 0;

        for(int i = -checkRange; i <= checkRange; i++)
        {
            for(int j = -checkRange; j <= checkRange; j++)
            {
                int fitDonuts = 0;
                Vector2 currentPos = new Vector2(i, j);

                foreach(var v in idealShapePos)
                {
                    if(donutsPos.Contains(currentPos + v))
                    {
                        fitDonuts++;
                    }
                }

                maxFitDonuts = Mathf.Max(maxFitDonuts, fitDonuts);
            }
        }

        return maxFitDonuts;
    }
}
