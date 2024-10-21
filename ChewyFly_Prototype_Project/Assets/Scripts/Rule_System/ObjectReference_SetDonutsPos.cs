using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class ObjectReferenceManeger : MonoBehaviour
{
    [Header("�h�[�i�c������̕ϐ�")]
    [Tooltip("�g���C�̃I�u�W�F�N�g")]
    [SerializeField] Transform tray;

    [Tooltip("�h�[�i�c���c�ɗ�������")]
    [SerializeField] float intervalVerticalDistance = 1f;
    [Tooltip("�h�[�i�c�����ɗ�������")]
    [SerializeField] float intervalHorizontalDistance = 1f;

    [Tooltip("�h�[�i�c���c�ɕ��ׂ鐔")]
    [SerializeField] int verticalSetUpNum = 5;
    [Tooltip("�h�[�i�c�����ɕ��ׂ鐔")]
    [SerializeField] int horizontalSetUpNum = 5;

    int verticalNum = 0;
    int horizontalNum = 0;
    const float dropHeight = 3f;
    public Vector3 GetDonutDropPosition(Vector3 parentPos, Vector3 centerPos)//�V�����h�[�i�c�𗎂Ƃ��ʒu��Ԃ�
    {
        Vector3 dropPos = new Vector3(
            tray.position.x - (intervalHorizontalDistance * horizontalSetUpNum / 2f) 
            + intervalHorizontalDistance * horizontalNum,
            tray.position.y + dropHeight,
            tray.position.z + (intervalVerticalDistance   * verticalSetUpNum   / 2f)
            - (intervalVerticalDistance * verticalNum));

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
