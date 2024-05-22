using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputScript : MonoBehaviour
{//�S�̂�ʂ��ăQ�[���p�b�h���ڑ�����Ă����炻������D�悵�܂�
    public bool isJump()//�W�����v������
    {
        if (Gamepad.current == null)
            return Keyboard.current.spaceKey.wasPressedThisFrame;
        return Gamepad.current.aButton.wasPressedThisFrame;
    }
    public bool isRotateCameraRight()//�J�������E�ɓ������Ă��邩
    {
        if (Gamepad.current == null)
            return Keyboard.current.eKey.isPressed;
        return Gamepad.current.rightShoulder.isPressed;
    }
    public bool isRotateCameraLeft()//�J���������ɓ������Ă��邩
    {
        if (Gamepad.current == null)
            return Keyboard.current.qKey.isPressed;
        return Gamepad.current.leftShoulder.isPressed;
    }

    public Vector3 isMove()//x���Az���ɒl���������x�N�g��3��Ԃ��܂�
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)//�Q�[���p�b�h���Ȃ��Ȃ�pc�̑����D�悵�܂�
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
    public Vector3 isRightStick()//x���Az���ɒl���������x�N�g��3��Ԃ��܂�
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)//�Q�[���p�b�h���Ȃ��Ȃ�pc�̑����D�悵�܂�
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

    public Vector3 isLeftDpad()//x���Az���ɒl���������x�N�g��3��Ԃ��܂�
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            Vector3 keyDirection = Vector3.zero;//������������܂���wasd�ׂ̗̕��̃L�[�Ō��m���܂�
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
        return new Vector3(gamepad.dpad.ReadValue().x, 0, gamepad.dpad.ReadValue().y);//�\���L�[
    }
    /*public bool isOption()//�ꉞ�A�Q�[�����̈ꎞ��~�{�^�����ł������ׂ̈̊֐�
    {
        if (Gamepad.current == null)
            return Keyboard.current.escapeKey.wasPressedThisFrame;
        return Gamepad.current.startButton.wasPressedThisFrame;
    }*/
    public bool isBackButton()//xBox�R���g���[���[��BACK�{�^��
    {
        if (Gamepad.current == null)
            return Keyboard.current.escapeKey.wasPressedThisFrame;
        return Gamepad.current.selectButton.wasPressedThisFrame;
    }
    public bool isXButton()//�E�Ă̍��ɂ���{�^��
    {
        if (Gamepad.current == null)
            return Keyboard.current.xKey.wasPressedThisFrame;
        return Gamepad.current.xButton.wasPressedThisFrame;
    }
    public bool isYButton()//�E�Ă̏�ɂ���{�^��
    {
        if (Gamepad.current == null)
            return Keyboard.current.cKey.wasPressedThisFrame;
        return Gamepad.current.yButton.wasPressedThisFrame;
    }
    public bool isAButton()//�E�ẲE�ɂ���{�^��
    {
        if (Gamepad.current == null)
            return Keyboard.current.vKey.wasPressedThisFrame;
        return Gamepad.current.bButton.wasPressedThisFrame;
    }
    public bool isRightTrigger()//�E�̃g���K�[
    {
        if (Gamepad.current == null)
            return Keyboard.current.mKey.wasPressedThisFrame;
        return Gamepad.current.rightTrigger.isPressed;
    }
    public bool isLeftTrigger()//���̃g���K�[
    {
        if (Gamepad.current == null)
            return Keyboard.current.nKey.wasPressedThisFrame;
        return Gamepad.current.leftTrigger.isPressed;
    }



    //�ȉ�������(�m�F�p)�ł�----------------------
    private float speed = 10;
    private void Update()
    {
        transform.Translate(isMove() * Time.deltaTime * speed);
        if (isJump())
            Debug.Log("�W�����v����");
        if (isRotateCameraLeft())
            Debug.Log("�J���������ɓ������Ă��܂�");
        if (isRotateCameraRight())
            Debug.Log("�J�������E�ɓ������Ă��܂�");
        if (isRotateCameraLeft() && isRotateCameraRight())
            Debug.Log("�J�������������Z�b�g���܂���");
        if (isRightStick() != Vector3.zero)
            Debug.Log(isRightStick());
        if (isLeftDpad() != Vector3.zero)
            Debug.Log(isLeftDpad());
        if (isBackButton())
            Debug.Log("�o�b�N�{�^����������܂���");
        if (isXButton())
            Debug.Log("x�{�^����������܂���");
        if (isYButton())
            Debug.Log("y�{�^����������܂���");
        if (isAButton())
            Debug.Log("a�{�^����������܂���");
        if (isRightTrigger())
            Debug.Log("�E�g���K�[��������܂���");
        if (isLeftTrigger())
            Debug.Log("���g���K�[��������܂���");

    }
}
