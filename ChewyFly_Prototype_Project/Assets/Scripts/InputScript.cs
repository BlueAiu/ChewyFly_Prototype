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
        return new Vector3(gamepad.leftStick.ReadValue().x, 0, gamepad.leftStick.ReadValue().y);
    }
    public Vector3 isRightStick()//x軸、z軸に値が入ったベクトル3を返します
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)//ゲームパッドがないならpcの操作を優先します
        {
            Vector3 keyDirection = Vector3.zero;
            if (Keyboard.current.leftArrowKey.isPressed)
                keyDirection.x--;
            if (Keyboard.current.rightArrowKey.isPressed)
                keyDirection.x++;
            if (Keyboard.current.upArrowKey.isPressed)
                keyDirection.z++;
            if (Keyboard.current.downArrowKey.isPressed)
                keyDirection.z--;
            return keyDirection;
        }
        return new Vector3(gamepad.rightStick.ReadValue().x, 0, gamepad.rightStick.ReadValue().y);
    }

    public Vector3 isLeftDpad()//x軸、z軸に値が入ったベクトル3を返します
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            Vector3 keyDirection = Vector3.zero;//少し無理ありますがwasdの隣の方のキーで検知します
            if (Keyboard.current.fKey.isPressed)
                keyDirection.x--;
            if (Keyboard.current.hKey.isPressed)
                keyDirection.x++;
            if (Keyboard.current.tKey.isPressed)
                keyDirection.z++;
            if (Keyboard.current.gKey.isPressed)
                keyDirection.z--;
            return keyDirection;
        }
        return new Vector3(gamepad.dpad.ReadValue().x, 0, gamepad.dpad.ReadValue().y);//十字キー
    }
    /*public bool isOption()//一応、ゲーム中の一時停止ボタンができた時の為の関数
    {
        if (Gamepad.current == null)
            return Keyboard.current.escapeKey.wasPressedThisFrame;
        return Gamepad.current.startButton.wasPressedThisFrame;
    }*/
    public bool isBackButton()//xBoxコントローラーのBACKボタン
    {
        if (Gamepad.current == null)
            return Keyboard.current.escapeKey.wasPressedThisFrame;
        return Gamepad.current.selectButton.wasPressedThisFrame;
    }
    public bool isXButton()//右ての左にあるボタン
    {
        if (Gamepad.current == null)
            return Keyboard.current.xKey.wasPressedThisFrame;
        return Gamepad.current.xButton.wasPressedThisFrame;
    }
    public bool isYButton()//右ての上にあるボタン
    {
        if (Gamepad.current == null)
            return Keyboard.current.cKey.wasPressedThisFrame;
        return Gamepad.current.yButton.wasPressedThisFrame;
    }
    public bool isAButton()//右ての右にあるボタン
    {
        if (Gamepad.current == null)
            return Keyboard.current.vKey.wasPressedThisFrame;
        return Gamepad.current.bButton.wasPressedThisFrame;
    }
    public bool isRightTrigger()//右のトリガー
    {
        if (Gamepad.current == null)
            return Keyboard.current.mKey.wasPressedThisFrame;
        return Gamepad.current.rightTrigger.isPressed;
    }
    public bool isLeftTrigger()//左のトリガー
    {
        if (Gamepad.current == null)
            return Keyboard.current.nKey.wasPressedThisFrame;
        return Gamepad.current.leftTrigger.isPressed;
    }



    //以下実装例(確認用)です----------------------
    private float speed = 10;
    private void Update()
    {
        transform.Translate(isMove() * Time.deltaTime * speed);
        if (isJump())
            Debug.Log("ジャンプした");
        if (isRotateCameraLeft())
            Debug.Log("カメラを左に動かしています");
        if (isRotateCameraRight())
            Debug.Log("カメラを右に動かしています");
        if (isRotateCameraLeft() && isRotateCameraRight())
            Debug.Log("カメラ方向をリセットしました");
        if (isRightStick() != Vector3.zero)
            Debug.Log(isRightStick());
        if (isLeftDpad() != Vector3.zero)
            Debug.Log(isLeftDpad());
        if (isBackButton())
            Debug.Log("バックボタンが押されました");
        if (isXButton())
            Debug.Log("xボタンが押されました");
        if (isYButton())
            Debug.Log("yボタンが押されました");
        if (isAButton())
            Debug.Log("aボタンが押されました");
        if (isRightTrigger())
            Debug.Log("右トリガーが押されました");
        if (isLeftTrigger())
            Debug.Log("左トリガーが押されました");

    }
}
