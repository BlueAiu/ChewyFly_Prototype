using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class ObjectReferenceManeger : MonoBehaviour
{
    [Tooltip("�ŏ��̃h�[�i�c�𗎂Ƃ��ʒu")]
    [SerializeField] Transform dropPosition;

    [Header("�h�[�i�c���c�ɗ�������")]
    [SerializeField] float intervalVerticalDistance = 1f;
    [Header("�h�[�i�c�����ɗ�������")]
    [SerializeField] float intervalHorizontalDistance = 1f;

    [Header("�h�[�i�c���c�ɕ��ׂ鐔")]
    [SerializeField] int verticalSetUpNum = 5;
    [Header("�h�[�i�c�����ɕ��ׂ鐔")]
    [SerializeField] int horizontalSetUpNum = 5;

    int verticalNum = 0;
    int horizontalNum = 0;
    public Vector3 GetDonutDropPosition(Vector3 parentPos, Vector3 centerPos)//�V�����h�[�i�c�𗎂Ƃ��ʒu��Ԃ�
    {
        Vector3 dropPos = new Vector3(
            dropPosition.position.x + intervalHorizontalDistance * horizontalNum,
            dropPosition.position.y,
            dropPosition.position.z - (intervalVerticalDistance * verticalNum));

        horizontalNum++;
        if (horizontalNum >= horizontalSetUpNum)//�������Ȃ�V�������ʒu�ɐݒu����
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
