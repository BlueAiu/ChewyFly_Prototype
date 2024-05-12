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
    void OnEnable()//�I�u�W�F�N�g���L���ɂȂ�����f���P�[�g�Ŋ֐���ǉ�����
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
        Debug.Log("�W�����v�L�[�������ꂽ");//�����ɃW�����v�Ăяo��
    }
    private void OnMove(InputAction.CallbackContext obj)
    {
        var value = obj.ReadValue<Vector2>();
        var direction = new Vector3(value.x, 0, value.y);

        Debug.Log("�����L�[�̓��͂�" + direction);//�����ɍ��E�㉺�ړ��Ăяo��
        dir = direction;
    }
    private void OnMoveStop(InputAction.CallbackContext obj)
    {
        Debug.Log("�����L�[��0�ɂȂ����@�l��" + Vector3.zero);//�����Ńx�N�g����0�ɂ��Ď~�߂�
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


    //�ȉ�������ł�----------------------
    Vector3 dir = Vector3.zero;
    private float speed = 10;
    private void Update()
    {
        transform.Translate(dir * Time.deltaTime * speed);

        if (isLeftAngle && isRightAngle)
        {
            Debug.Log("�p�x���Z�b�g");
        }
        if (isRightAngle)
        {
            Debug.Log("�E�Ɏ��_�𓮂����Ă�");
        }
        if (isLeftAngle)
        {
            Debug.Log("���Ɏ��_�𓮂����Ă�");
        }
    }
}
