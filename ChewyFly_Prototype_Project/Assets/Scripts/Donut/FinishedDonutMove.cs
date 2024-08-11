using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DonutRigidBody : MonoBehaviour
{
    [Header("���������Ƃ��̓���")]
    [Tooltip("�h�[�i�c���J�����Ɍ����ė��Ă�܂ł̎���")]
    [SerializeField] float phese1Time = 1f;
    [Tooltip("�h�[�i�c�𗧂Ă�p�x")]
    [SerializeField] float standDonutAngle = 90;
    [Tooltip("��Ɉړ����鋗��")]
    [SerializeField] float phese1ShiftY = 2f;

    [Tooltip("���񂾂��ɉ������鎞��")]
    [SerializeField] float phese2Time = 1f;
    [Tooltip("��ɉ�����������x")]
    [SerializeField] float phese2Acceleration = 1f;

    [Tooltip("���������h�[�i�c��u���Ă����ꏊ")]
    [SerializeField] Vector3 storageArea;

    Transform cameraAxis;
    Collider[] colliders;
    bool isFinishMoving = false;

    int actionPhese = 0;
    float actionTimer = 0f;

    public void SetMoveMode()
    {
        rb.isKinematic = true;
        isFinishMoving = true;

        colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }

        transform.parent = cameraAxis;
    }

    void FinishedDonutMove()
    {
        if (!isFinishMoving) return;

        actionTimer += Time.deltaTime;

        switch (actionPhese)
        {
            case 0:
                var rotationDir = cameraAxis.TransformDirection(Vector3.left);
                transform.RotateAround(transform.position, rotationDir, standDonutAngle * Time.deltaTime / phese1Time);
                transform.Translate(Vector3.up * phese1ShiftY * Time.deltaTime / phese1Time);

                if(actionTimer > phese1Time)
                {
                    actionTimer = 0f; actionPhese++;
                }
                break;

            case 1:
                transform.position += Vector3.up * (actionTimer * phese2Acceleration);

                if(actionTimer > phese2Time)
                {
                    actionTimer = 0f; actionPhese++;
                }
                break;

            case 2:
                transform.parent = null;
                rb.isKinematic = false;
                isFinishMoving = false;

                foreach(var c in colliders)
                {
                    c.enabled = true;
                }

                transform.position = storageArea;

                break;
        }
    }
}
