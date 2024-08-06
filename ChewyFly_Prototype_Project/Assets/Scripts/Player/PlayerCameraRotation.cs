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
    [SerializeField] float sensityvity = 180f;

    [Tooltip("視点リセット時のカメラの回転の速さ")]
    [SerializeField] float resetSensityvity = 800f;
    bool isResetCamera = false;

    private void Awake()//Startよりさらに前に格納しておく
    {
        input = GetComponent<InputScript>();
        if (playerCamera == null)
            playerCamera = GameObject.Find("PlayerCameraParent");
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

        playerCamera.transform.position = transform.position;//カメラをプレイヤーに移動させる
    }
}
