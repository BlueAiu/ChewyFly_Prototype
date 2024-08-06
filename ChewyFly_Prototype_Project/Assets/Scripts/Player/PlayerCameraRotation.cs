using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraRotation : MonoBehaviour
{
    protected InputScript input;

    [Tooltip("�v���C���[���f���J����")]
    [SerializeField] protected GameObject playerCamera;

    [Tooltip("�J�����̉�]�̑���")]
    [SerializeField] float sensityvity = 180f;

    [Tooltip("���_���Z�b�g���̃J�����̉�]�̑���")]
    [SerializeField] float resetSensityvity = 800f;
    bool isResetCamera = false;

    private void Awake()//Start��肳��ɑO�Ɋi�[���Ă���
    {
        input = GetComponent<InputScript>();
        if (playerCamera == null)
            playerCamera = GameObject.Find("PlayerCameraParent");
    }

    void LateUpdate()//Update�̌�ɃJ�����̈ʒu�𐧌䂷��
    {
        float rot = 0;
        //Debug.Log(Gamepad.current.rightStickButton.isPressed);
        //if(input.isRotateCameraRight() && input.isRotateCameraLeft())//���������Ŏ��_���Z�b�g
        //{
        //    playerCamera.transform.rotation = transform.rotation;
        //}
        //else
        //{
        //    if (input.isRotateCameraRight())
        //        rot++;
        //    if (input.isRotateCameraLeft())
        //        rot--;
        //    playerCamera.transform.Rotate(0, rot * sensityvity * Time.deltaTime, 0);
        //}

        rot = input.isRightStick().x;
        if (input.isRightStickButton())
        {
            isResetCamera = true;
        }

        if (rot != 0)
        {
            playerCamera.transform.Rotate(0, rot * sensityvity * Time.deltaTime, 0);
            isResetCamera = false;
        }
        else if(isResetCamera)
        {
            playerCamera.transform.rotation = Quaternion.RotateTowards(playerCamera.transform.rotation, transform.rotation, resetSensityvity * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, playerCamera.transform.rotation) < 0.1f)
            {
                isResetCamera = false;
            }
        }

        playerCamera.transform.position = transform.position;//�J�������v���C���[�Ɉړ�������
    }
}
