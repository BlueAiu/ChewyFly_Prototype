using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraRotation : MonoBehaviour
{
    protected InputScript input;

    [Tooltip("プレイヤーを映すカメラ")]
    [SerializeField] protected GameObject playerCamera;

    [Tooltip("カメラの回転の速さ")]
    [SerializeField] float sensitivity = 180f;
    [Tooltip("カメラの最大回転速度")]
    [SerializeField] float maxSensitivity = 330f;
    [Tooltip("カメラの最小回転速度")]
    [SerializeField] float minSensitivity = 30f;


    [Tooltip("視点リセット時のカメラの回転の速さ")]
    [SerializeField] float resetSensitivity = 800f;
    bool isResetCamera = false;


    [Header("自動カメラ回転")]
    [Tooltip("中心からこの距離より内側だと自動回転しない")]
    [SerializeField] float nonRotationalAreaRadius = 3f;
    [Tooltip("カメラ回転操作をした後この距離を離れるまで自動回転しない")]
    [SerializeField] float reapplicationDistance = 1f;
    [Tooltip("自動回転の回転速")]
    [SerializeField] float autoRotateSensitivity = 120f;

    bool canRotatoAuto = false;
    Vector3 lastInputPosition = Vector3.zero;

    private void Awake()//Startよりさらに前に格納しておく
    {
        input = GetComponent<InputScript>();
        if (playerCamera == null)
            playerCamera = GameObject.Find("PlayerCameraParent");

        OptionValues option = FindObjectOfType<OptionValues>();//音量を初期化
        SetCameraSensityvity(option);
    }

    void LateUpdate()//Updateの後にカメラの位置を制御する
    {
        float rot = 0;
        //Debug.Log(Gamepad.current.rightStickButton.isPressed);
        //if(input.isRotateCameraRight() && input.isRotateCameraLeft())//同時押しで視点リセット
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

        playerCamera.transform.position = transform.position;//カメラをプレイヤーに移動させる
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
