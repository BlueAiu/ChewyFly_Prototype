using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraRotation : MonoBehaviour
{
    protected InputScript input;

    [Tooltip("プレイヤーを映すカメラの軸")]
    [SerializeField] protected GameObject playerCameraAxis;

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

    [Tooltip("鍋の中心位置(カメラの位置を決めるため)")]
    [SerializeField] Transform potCenterPoint;
    [Tooltip("カメラが向く方向の場所")]
    [SerializeField] Transform cameraLookPoint;
    [Tooltip("鍋の中心とプレイヤーの間のどのくらいの比率の位置にカメラを向けるか")]
    [SerializeField] float cameraLookPointRatio = 0.5f;
    private void Awake()//Startよりさらに前に格納しておく
    {
        input = GetComponent<InputScript>();
        if (playerCameraAxis == null)
            playerCameraAxis = GameObject.Find("CameraAxis");

        OptionValues option = FindObjectOfType<OptionValues>();//カメラの回転速度を初期化
        SetCameraSensityvity(option);
    }
    private void Update()
    {
        cameraLookPoint.position = potCenterPoint.position + (transform.position - potCenterPoint.position) * cameraLookPointRatio;
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

        playerCameraAxis.transform.position = transform.position;//カメラをプレイヤーに移動させる
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
