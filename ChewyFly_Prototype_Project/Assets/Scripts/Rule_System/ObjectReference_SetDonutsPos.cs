using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class ObjectReferenceManeger : MonoBehaviour
{
    [Header("ドーナツ完成後の変数")]
    [Tooltip("トレイのオブジェクト")]
    [SerializeField] Transform tray;

    [Tooltip("ドーナツを縦に離す距離")]
    [SerializeField] float intervalVerticalDistance = 1f;
    [Tooltip("ドーナツを横に離す距離")]
    [SerializeField] float intervalHorizontalDistance = 1f;

    [Tooltip("ドーナツを縦に並べる数")]
    [SerializeField] int verticalSetUpNum = 5;
    [Tooltip("ドーナツを横に並べる数")]
    [SerializeField] int horizontalSetUpNum = 5;

    int verticalNum = 0;
    int horizontalNum = 0;
    const float dropHeight = 3f;
    public Vector3 GetDonutDropPosition(Vector3 parentPos, Vector3 centerPos)//新しくドーナツを落とす位置を返す
    {
        Vector3 dropPos = new Vector3(
            tray.position.x - (intervalHorizontalDistance * horizontalSetUpNum / 2f) 
            + intervalHorizontalDistance * horizontalNum,
            tray.position.y + dropHeight,
            tray.position.z + (intervalVerticalDistance   * verticalSetUpNum   / 2f)
            - (intervalVerticalDistance * verticalNum));

        horizontalNum++;
        if (horizontalNum >= horizontalSetUpNum)//超えたなら新しい横位置に設置する
        {
            horizontalNum = 0;
            verticalNum++;
            if(verticalNum >= verticalSetUpNum)
            {
                verticalNum = 0;
            }
        }
        return dropPos + (parentPos - centerPos);
    }
}
