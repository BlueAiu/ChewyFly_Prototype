using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraRotation : MonoBehaviour
{
    protected InputScript input;

    [Tooltip("�v���C���[���f���J����")]
    [SerializeField] protected GameObject playerCamera;

    [Tooltip("�J�����̉�]�̑���")]
    [SerializeField] float sensityvity = 180f;

    private void Awake()//Start��肳��ɑO�Ɋi�[���Ă���
    {
        input = GetComponent<InputScript>();
        if (playerCamera == null)
            playerCamera = GameObject.Find("PlayerCameraParent");
    }

    void LateUpdate()//Update�̌�ɃJ�����̈ʒu�𐧌䂷��
    {
        float rot = 0;
        if(input.isRotateCameraRight() && input.isRotateCameraLeft())//���������Ŏ��_���Z�b�g
        {
            playerCamera.transform.rotation = transform.rotation;
        }
        else
        {
            if (input.isRotateCameraRight())
                rot++;
            if (input.isRotateCameraLeft())
                rot--;
            playerCamera.transform.Rotate(0, rot * sensityvity * Time.deltaTime, 0);
        }
        playerCamera.transform.position = transform.position;//�J�������v���C���[�Ɉړ�������
    }
}
