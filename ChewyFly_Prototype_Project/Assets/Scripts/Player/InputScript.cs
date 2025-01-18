using UnityEngine;
using UnityEngine.InputSystem;

public class InputScript : MonoBehaviour
{//全体を通してゲームパッドとキーボード両方とも入力を検知します
    public bool isAButton()//ジャンプしたか
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.aButton.wasPressedThisFrame;
        return gamepadInput || Keyboard.current.spaceKey.wasPressedThisFrame;
    }
    public bool holdAButton()//Aボタンを継続して押している
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.aButton.isPressed;
        return gamepadInput || Keyboard.current.spaceKey.isPressed;
    }
    public bool holdBButton()//Bボタンを継続して押している
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.bButton.isPressed;
        return gamepadInput || Keyboard.current.zKey.isPressed;
    }
    public bool holdXButton()//Xボタンを継続して押している
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.xButton.isPressed;
        return gamepadInput || Keyboard.current.xKey.isPressed;
    }
    public bool holdYButton()//Yボタンを継続して押している
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.yButton.isPressed;
        return gamepadInput || Keyboard.current.cKey.isPressed;
    }
    public bool isBButton()//ジャンプしたか
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.bButton.wasPressedThisFrame;
        return gamepadInput || Keyboard.current.zKey.wasPressedThisFrame;
    }
    public bool isRightShoulder()//Rボタン
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.rightShoulder.isPressed;
        return gamepadInput || Keyboard.current.eKey.isPressed;
    }
    public bool isLeftShoulder()//Lボタン
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.leftShoulder.isPressed;
        return gamepadInput || Keyboard.current.qKey.isPressed;
    }

    public Vector3 isLeftStick()//x軸、z軸に値が入ったベクトル3を返します
    {
        //ゲームパッドの入力
        var gamepad = Gamepad.current;
        Vector3 gamepadDirection = Vector3.zero;
        if (gamepad != null)
            gamepadDirection = new Vector3(gamepad.leftStick.ReadValue().x, 0, gamepad.leftStick.ReadValue().y);

        //キーボードの入力
        Vector3 keyDirection = Vector3.zero;
        if (Keyboard.current.aKey.isPressed)
            keyDirection.x--;
        if (Keyboard.current.dKey.isPressed)
            keyDirection.x++;
        if (Keyboard.current.wKey.isPressed)
            keyDirection.z++;
        if (Keyboard.current.sKey.isPressed)
            keyDirection.z--;
        keyDirection = keyDirection.normalized;

        //スティックが倒されていたらスティックを優先する
        if (gamepadDirection != Vector3.zero)
            return gamepadDirection;
        else
            return keyDirection;
    }
    public Vector3 isRightStick()//x軸、z軸に値が入ったベクトル3を返します
    {
        //ゲームパッドの入力
        var gamepad = Gamepad.current;
        Vector3 gamepadDirection = Vector3.zero;
        if (gamepad != null)
            gamepadDirection = new Vector3(gamepad.rightStick.ReadValue().x, 0, gamepad.rightStick.ReadValue().y);

        //キーボードの入力
        Vector3 keyDirection = Vector3.zero;
        if (Keyboard.current.leftArrowKey.isPressed)
            keyDirection.x--;
        if (Keyboard.current.rightArrowKey.isPressed)
            keyDirection.x++;
        if (Keyboard.current.upArrowKey.isPressed)
            keyDirection.z++;
        if (Keyboard.current.downArrowKey.isPressed)
            keyDirection.z--;
        keyDirection = keyDirection.normalized;

        //スティックが倒されていたらスティックを優先する
        if (gamepadDirection != Vector3.zero)
            return gamepadDirection;
        else
            return keyDirection;
    }

    public Vector3 isLeftDpad()//x軸、z軸に値が入ったベクトル3を返します
    {
        //ゲームパッドの入力
        var gamepad = Gamepad.current;
        Vector3 gamepadDirection = Vector3.zero;
        if (gamepad != null)
            gamepadDirection = new Vector3(gamepad.dpad.ReadValue().x, 0, gamepad.dpad.ReadValue().y);//十字キー

        //キーボードの入力
        Vector3 keyDirection = Vector3.zero;//少し無理ありますがwasdの隣の方のキーで検知します
        if (Keyboard.current.fKey.isPressed)
            keyDirection.x--;
        if (Keyboard.current.hKey.isPressed)
            keyDirection.x++;
        if (Keyboard.current.tKey.isPressed)
            keyDirection.z++;
        if (Keyboard.current.gKey.isPressed)
            keyDirection.z--;
        keyDirection = keyDirection.normalized;

        //十字が押されていたら十字を優先する
        if (gamepadDirection != Vector3.zero)
            return gamepadDirection;
        else
            return keyDirection;
    }
    /*public bool isOption()//一応、ゲーム中の一時停止ボタンができた時の為の関数
    {
        if (Gamepad.current == null)
            return Keyboard.current.escapeKey.wasPressedThisFrame;
        return Gamepad.current.startButton.wasPressedThisFrame;
    }*/
    public bool isRightStickButton()//xBoxコントローラーの右スティック押し込み
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.rightStickButton.IsPressed();
        return gamepadInput || Keyboard.current.pKey.wasPressedThisFrame;
    }
    public bool isLeftStickButton()//xBoxコントローラーの左スティック押し込み
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.leftStickButton.IsPressed();
        return gamepadInput || Keyboard.current.oKey.wasPressedThisFrame;
    }
    public bool isBackButton()//xBoxコントローラーのBACKボタン
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.selectButton.wasPressedThisFrame;
        return gamepadInput || Keyboard.current.escapeKey.wasPressedThisFrame;
    }
    public bool isWestButton()//右ての左にあるボタン
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.xButton.wasPressedThisFrame;
        return gamepadInput || Keyboard.current.xKey.wasPressedThisFrame;
    }
    public bool isNorthButton()//右ての上にあるボタン
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.yButton.wasPressedThisFrame;
        return gamepadInput|| Keyboard.current.cKey.wasPressedThisFrame;
    }
    public bool isEastButton()//右ての右にあるボタン
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.bButton.wasPressedThisFrame;
        return gamepadInput || Keyboard.current.vKey.wasPressedThisFrame;
    }
    public bool isRightTrigger()//右のトリガー
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.rightTrigger.isPressed;
        return gamepadInput || Keyboard.current.mKey.wasPressedThisFrame;
    }
    public bool isLeftTrigger()//左のトリガー
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.leftTrigger.isPressed;
        return gamepadInput || Keyboard.current.nKey.wasPressedThisFrame;
    }
    public bool isResetHighScoreKey()//ハイスコアのリセットボタン
    {
        return Keyboard.current.deleteKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame;
    }



    //以下実装例(確認用)です----------------------
    //private float speed = 10;
    private void Update()
    {
        //transform.Translate(isMove() * Time.deltaTime * speed);
        //if (isJump())
        //    Debug.Log("ジャンプした");
        //if (isRotateCameraLeft())
        //    Debug.Log("カメラを左に動かしています");
        //if (isRotateCameraRight())
        //    Debug.Log("カメラを右に動かしています");
        //if (isRotateCameraLeft() && isRotateCameraRight())
        //    Debug.Log("カメラ方向をリセットしました");
        //if (isRightStick() != Vector3.zero)
        //    Debug.Log(isRightStick());
        //if (isLeftDpad() != Vector3.zero)
        //    Debug.Log(isLeftDpad());
        //if (isBackButton())
        //    Debug.Log("バックボタンが押されました");
        //if (isXButton())
        //    Debug.Log("xボタンが押されました");
        //if (isYButton())
        //    Debug.Log("yボタンが押されました");
        //if (isAButton())
        //    Debug.Log("aボタンが押されました");
        //if (isRightTrigger())
        //    Debug.Log("右トリガーが押されました");
        //if (isLeftTrigger())
        //    Debug.Log("左トリガーが押されました");
        //Debug.Log(Gamepad.current.rightStickButton.isPressed);
    }
}
