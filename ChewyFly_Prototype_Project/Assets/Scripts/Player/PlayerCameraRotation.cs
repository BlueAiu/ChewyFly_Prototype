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
    [SerializeField] float sensitivity = 180f;
    [Tooltip("�J�����̍ő��]���x")]
    [SerializeField] float maxSensitivity = 330f;
    [Tooltip("�J�����̍ŏ���]���x")]
    [SerializeField] float minSensitivity = 30f;


    [Tooltip("���_���Z�b�g���̃J�����̉�]�̑���")]
    [SerializeField] float resetSensitivity = 800f;
    bool isResetCamera = false;


    [Header("�����J������]")]
    [Tooltip("���S���炱�̋������������Ǝ�����]���Ȃ�")]
    [SerializeField] float nonRotationalAreaRadius = 3f;
    [Tooltip("�J������]����������ケ�̋����𗣂��܂Ŏ�����]���Ȃ�")]
    [SerializeField] float reapplicationDistance = 1f;
    [Tooltip("������]�̉�]��")]
    [SerializeField] float autoRotateSensitivity = 120f;

    bool canRotatoAuto = false;
    Vector3 lastInputPosition = Vector3.zero;

    private void Awake()//Start��肳��ɑO�Ɋi�[���Ă���
    {
        input = GetComponent<InputScript>();
        if (playerCamera == null)
            playerCamera = GameObject.Find("PlayerCameraParent");

        OptionValues option = FindObjectOfType<OptionValues>();//���ʂ�������
        SetCameraSensityvity(option);
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
            playerCamera.transform.Rotate(0, rot * sensitivity * Time.deltaTime, 0);
            isResetCamera = false;
        }
        else if(isResetCamera)
        {
            playerCamera.transform.rotation = Quaternion.RotateTowards(playerCamera.transform.rotation, transform.rotation, resetSensitivity * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, playerCamera.transform.rotation) < 0.1f)
            {
                isResetCamera = false;
            }
        }

        AutoCameraRotation();

        playerCamera.transform.position = transform.position;//�J�������v���C���[�Ɉړ�������
    }

    void AutoCameraRotation()
    {
        if(canRotatoAuto && 
            Vector3.SqrMagnitude(transform.position - Vector3.zero) > nonRotationalAreaRadius * nonRotationalAreaRadius)
        {
            Vector3 centerDir = (Vector3.zero + Vector3.up * transform.position.y) - transform.position;
            Quaternion lookCenter = Quaternion.LookRotation(centerDir, Vector3.up);

            playerCamera.transform.rotation = Quaternion.RotateTowards(playerCamera.transform.rotation, lookCenter, autoRotateSensitivity * Time.deltaTime);
        }

        if(input.isRightStick() != Vector3.zero || input.isRightStickButton())
        {
            lastInputPosition = transform.position;
            canRotatoAuto = false;
        }
        else if(Vector3.SqrMagnitude(transform.position - lastInputPosition) > reapplicationDistance * reapplicationDistance)
        {
            canRotatoAuto= true;
        }
    }

    void SetCameraSensityvity(OptionValues optionValues)
    {
        if (optionValues == null) return;

        float ratio = (float)(optionValues.CameraSensitivity - OptionValues.sensitivityMinValue) / 
            (OptionValues.sensitivityMaxValue - OptionValues.sensitivityMinValue);
        sensitivity = minSensitivity + (maxSensitivity - minSensitivity) * ratio;
    }
}
