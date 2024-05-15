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
        return new Vector3(gamepad.leftStick.ReadValue().x,0, gamepad.leftStick.ReadValue().y);
    }

    /*public bool isOption()//�ꉞ�A�Q�[�����̈ꎞ��~�{�^�����ł������ׂ̈̊֐�
    {
        if (Gamepad.current == null)
            return Keyboard.current.escapeKey.wasPressedThisFrame;
        return Gamepad.current.startButton.wasPressedThisFrame;
    }*/



    //�ȉ�������(�m�F�p)�ł�----------------------
    private float speed = 10;
    private void Update()
    {
        transform.Translate(isMove() * Time.deltaTime * speed);
        if (isJump())
            Debug.Log("�W�����v����");
        if(isRotateCameraLeft())
            Debug.Log("�J���������ɓ������Ă��܂�");
        if (isRotateCameraRight())
            Debug.Log("�J�������E�ɓ������Ă��܂�");
        if(isRotateCameraLeft() && isRotateCameraRight())
            Debug.Log("�J�������������Z�b�g���܂���");

    }
}
