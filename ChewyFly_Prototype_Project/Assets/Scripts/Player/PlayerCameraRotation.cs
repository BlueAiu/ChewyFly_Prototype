using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraRotation : MonoBehaviour
{
    protected InputScript input;

    [Tooltip("�v���C���[���f���J�����̎�")]
    [SerializeField] protected GameObject playerCameraAxis;

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

    [Tooltip("��̒��S�ʒu(�J�����̈ʒu�����߂邽��)")]
    [SerializeField] Transform potCenterPoint;
    [Tooltip("�J���������������̏ꏊ")]
    [SerializeField] Transform cameraLookPoint;
    [Tooltip("��̒��S�ƃv���C���[�̊Ԃ̂ǂ̂��炢�̔䗦�̈ʒu�ɃJ�����������邩")]
    [SerializeField] float cameraLookPointRatio = 0.5f;
    private void Awake()//Start��肳��ɑO�Ɋi�[���Ă���
    {
        input = GetComponent<InputScript>();
        if (playerCameraAxis == null)
            playerCameraAxis = GameObject.Find("CameraAxis");

        OptionValues option = FindObjectOfType<OptionValues>();//�J�����̉�]���x��������
        SetCameraSensityvity(option);
    }
    private void Update()
    {
        cameraLookPoint.position = potCenterPoint.position + (transform.position - potCenterPoint.position) * cameraLookPointRatio;
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
            playerCameraAxis.transform.Rotate(0, rot * sensitivity * Time.deltaTime, 0);
            isResetCamera = false;
        }
        else if(isResetCamera)
        {
            playerCameraAxis.transform.rotation = Quaternion.RotateTowards(playerCameraAxis.transform.rotation, transform.rotation, resetSensitivity * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, playerCameraAxis.transform.rotation) < 0.1f)
            {
                isResetCamera = false;
            }
        }

        AutoCameraRotation();

        playerCameraAxis.transform.position = transform.position;//�J�������v���C���[�Ɉړ�������
    }
    void AutoCameraRotation()
    {
        if(canRotatoAuto && 
            Vector3.SqrMagnitude(transform.position - Vector3.zero) > nonRotationalAreaRadius * nonRotationalAreaRadius)
        {
            Vector3 centerDir = (Vector3.zero + Vector3.up * transform.position.y) - transform.position;
            Quaternion lookCenter = Quaternion.LookRotation(centerDir, Vector3.up);

            playerCameraAxis.transform.rotation = Quaternion.RotateTowards(playerCameraAxis.transform.rotation, lookCenter, autoRotateSensitivity * Time.deltaTime);
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

        sensitivity = minSensitivity + (maxSensitivity - minSensitivity) * optionValues.GetCameraSensitivityRatio();
    }
}
