using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputScript : MonoBehaviour
{//全体を通してゲームパッドが接続されていたらそっちを優先します
    public bool isJump()//ジャンプしたか
    {
        if (Gamepad.current == null) 
            return Keyboard.current.spaceKey.wasPressedThisFrame;
        return Gamepad.current.aButton.wasPressedThisFrame;
    }
    public bool isRotateCameraRight()//カメラを右に動かしているか
    {
        if (Gamepad.current == null)
            return Keyboard.current.eKey.isPressed;
        return Gamepad.current.rightShoulder.isPressed;
    }
    public bool isRotateCameraLeft()//カメラを左に動かしているか
    {
        if (Gamepad.current == null)
            return Keyboard.current.qKey.isPressed;
        return Gamepad.current.leftShoulder.isPressed;
    }

    public Vector3 isMove()//x軸、z軸に値が入ったベクトル3を返します
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)//ゲームパッドがないならpcの操作を優先します
        {
            Vector3 keyDirection = Vector3.zero;
            if (Keyboard.current.aKey.isPressed)
                keyDirection.x--;
            if (Keyboard.current.dKey.isPressed)
                keyDirection.x++;
            if (Keyboard.current.wKey.isPressed)
                keyDirection.z++;
            if (Keyboard.current.sKey.isPressed)
                keyDirection.z--;
            return keyDirection;
        }
        return new Vector3(gamepad.leftStick.ReadValue().x,0, gamepad.leftStick.ReadValue().y);
    }

    /*public bool isOption()//一応、ゲーム中の一時停止ボタンができた時の為の関数
    {
        if (Gamepad.current == null)
            return Keyboard.current.escapeKey.wasPressedThisFrame;
        return Gamepad.current.startButton.wasPressedThisFrame;
    }*/



    //以下実装例(確認用)です----------------------
    private float speed = 10;
    private void Update()
    {
        transform.Translate(isMove() * Time.deltaTime * speed);
        if (isJump())
            Debug.Log("ジャンプした");
        if(isRotateCameraLeft())
            Debug.Log("カメラを左に動かしています");
        if (isRotateCameraRight())
            Debug.Log("カメラを右に動かしています");
        if(isRotateCameraLeft() && isRotateCameraRight())
            Debug.Log("カメラ方向をリセットしました");

    }
}
