using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraRotation : MonoBehaviour
{
    protected InputScript input;

    [Tooltip("プレイヤーを映すカメラ")]
    [SerializeField] protected GameObject playerCamera;

    [Tooltip("カメラの回転の速さ")]
    [SerializeField] float sensityvity = 180f;

    private void Awake()//Startよりさらに前に格納しておく
    {
        input = GetComponent<InputScript>();
        if (playerCamera == null)
            playerCamera = GameObject.Find("PlayerCameraParent");
    }

    void LateUpdate()//Updateの後にカメラの位置を制御する
    {
        float rot = 0;
        if(input.isRotateCameraRight() && input.isRotateCameraLeft())//同時押しで視点リセット
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
        playerCamera.transform.position = transform.position;//カメラをプレイヤーに移動させる
    }
}
