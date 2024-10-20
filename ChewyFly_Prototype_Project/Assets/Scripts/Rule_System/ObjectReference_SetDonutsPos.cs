using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class ObjectReferenceManeger : MonoBehaviour
{
    [Tooltip("最初のドーナツを落とす位置")]
    [SerializeField] Transform dropPosition;

    [Header("ドーナツを縦に離す距離")]
    [SerializeField] float intervalVerticalDistance = 1f;
    [Header("ドーナツを横に離す距離")]
    [SerializeField] float intervalHorizontalDistance = 1f;

    [Header("ドーナツを縦に並べる数")]
    [SerializeField] int verticalSetUpNum = 5;
    [Header("ドーナツを横に並べる数")]
    [SerializeField] int horizontalSetUpNum = 5;

    int verticalNum = 0;
    int horizontalNum = 0;
    public Vector3 GetDonutDropPosition(Vector3 parentPos, Vector3 centerPos)//新しくドーナツを落とす位置を返す
    {
        Vector3 dropPos = new Vector3(
            dropPosition.position.x + intervalHorizontalDistance * horizontalNum,
            dropPosition.position.y,
            dropPosition.position.z - (intervalVerticalDistance * verticalNum));

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
