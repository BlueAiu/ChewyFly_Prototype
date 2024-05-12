using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystemScript : MonoBehaviour
{
    PlayerInput _input;
    private bool isRightAngle;
    private bool isLeftAngle;
    void Awake()
    {
        TryGetComponent(out _input);
        isRightAngle = false; isLeftAngle = false;
    }
    void OnEnable()//オブジェクトが有効になったらデリケートで関数を追加する
    {
        _input.actions["Jump"].started   += OnJump;
        _input.actions["Move"].performed += OnMove;
        _input.actions["Move"].canceled  += OnMoveStop;
        _input.actions["RightAngle"].started   += OnRightAngle;
        _input.actions["RightAngle"].canceled  += OnRightAngle;
        _input.actions["LeftAngle"].started    += OnLeftAngle;
        _input.actions["LeftAngle"].canceled   += OnLeftAngle;
    }
    private void OnDisable()
    {
        _input.actions["Jump"].started   -= OnJump;
        _input.actions["Move"].performed -= OnMove;
        _input.actions["Move"].canceled  -= OnMoveStop;
        _input.actions["RightAngle"].started   -= OnRightAngle;
        _input.actions["RightAngle"].canceled  -= OnRightAngle;
        _input.actions["LeftAngle"].started    -= OnLeftAngle;
        _input.actions["LeftAngle"].canceled   -= OnLeftAngle;
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
        Debug.Log("ジャンプキーが押された");//ここにジャンプ呼び出し
    }
    private void OnMove(InputAction.CallbackContext obj)
    {
        var value = obj.ReadValue<Vector2>();
        var direction = new Vector3(value.x, 0, value.y);

        Debug.Log("方向キーの入力は" + direction);//ここに左右上下移動呼び出し
        dir = direction;
    }
    private void OnMoveStop(InputAction.CallbackContext obj)
    {
        Debug.Log("方向キーが0になった　値は" + Vector3.zero);//ここでベクトルを0にして止める
        dir = Vector3.zero;
    }
    private void OnRightAngle(InputAction.CallbackContext obj)
    {
        switch (obj.phase)
        {
            case InputActionPhase.Started:
                isRightAngle = true;
                break;
            case InputActionPhase.Canceled:
                isRightAngle = false;
                break;
        }
        SetAngle();
    }
    private void OnLeftAngle(InputAction.CallbackContext obj)
    {
        switch (obj.phase)
        {
            case InputActionPhase.Started:
                isLeftAngle = true;
                break;
            case InputActionPhase.Canceled:
                isLeftAngle = false;
                break;
        }
        SetAngle();
    }
    private void SetAngle()
    {
        if(isLeftAngle && isRightAngle)
        {
        }
        if(isRightAngle)
        {
        }
        if (isLeftAngle)
        {
        }
    }


    //以下実装例です----------------------
    Vector3 dir = Vector3.zero;
    private float speed = 10;
    private void Update()
    {
        transform.Translate(dir * Time.deltaTime * speed);

        if (isLeftAngle && isRightAngle)
        {
            Debug.Log("角度リセット");
        }
        if (isRightAngle)
        {
            Debug.Log("右に視点を動かしてる");
        }
        if (isLeftAngle)
        {
            Debug.Log("左に視点を動かしてる");
        }
    }
}
