using UnityEngine;
using UnityEngine.InputSystem;

public class InputScript : MonoBehaviour
{//�S�̂�ʂ��ăQ�[���p�b�h�ƃL�[�{�[�h�����Ƃ����͂����m���܂�
    public bool isAButton()//�W�����v������
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.aButton.wasPressedThisFrame;
        return gamepadInput || Keyboard.current.spaceKey.wasPressedThisFrame;
    }
    public bool holdAButton()//A�{�^�����p�����ĉ����Ă���
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.aButton.isPressed;
        return gamepadInput || Keyboard.current.spaceKey.isPressed;
    }
    public bool holdBButton()//B�{�^�����p�����ĉ����Ă���
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.bButton.isPressed;
        return gamepadInput || Keyboard.current.zKey.isPressed;
    }
    public bool holdXButton()//X�{�^�����p�����ĉ����Ă���
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.xButton.isPressed;
        return gamepadInput || Keyboard.current.xKey.isPressed;
    }
    public bool holdYButton()//Y�{�^�����p�����ĉ����Ă���
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.yButton.isPressed;
        return gamepadInput || Keyboard.current.cKey.isPressed;
    }
    public bool isBButton()//�W�����v������
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.bButton.wasPressedThisFrame;
        return gamepadInput || Keyboard.current.zKey.wasPressedThisFrame;
    }
    public bool isRightShoulder()//R�{�^��
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.rightShoulder.isPressed;
        return gamepadInput || Keyboard.current.eKey.isPressed;
    }
    public bool isLeftShoulder()//L�{�^��
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.leftShoulder.isPressed;
        return gamepadInput || Keyboard.current.qKey.isPressed;
    }

    public Vector3 isLeftStick()//x���Az���ɒl���������x�N�g��3��Ԃ��܂�
    {
        //�Q�[���p�b�h�̓���
        var gamepad = Gamepad.current;
        Vector3 gamepadDirection = Vector3.zero;
        if (gamepad != null)
            gamepadDirection = new Vector3(gamepad.leftStick.ReadValue().x, 0, gamepad.leftStick.ReadValue().y);

        //�L�[�{�[�h�̓���
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

        //�X�e�B�b�N���|����Ă�����X�e�B�b�N��D�悷��
        if (gamepadDirection != Vector3.zero)
            return gamepadDirection;
        else
            return keyDirection;
    }
    public Vector3 isRightStick()//x���Az���ɒl���������x�N�g��3��Ԃ��܂�
    {
        //�Q�[���p�b�h�̓���
        var gamepad = Gamepad.current;
        Vector3 gamepadDirection = Vector3.zero;
        if (gamepad != null)
            gamepadDirection = new Vector3(gamepad.rightStick.ReadValue().x, 0, gamepad.rightStick.ReadValue().y);

        //�L�[�{�[�h�̓���
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

        //�X�e�B�b�N���|����Ă�����X�e�B�b�N��D�悷��
        if (gamepadDirection != Vector3.zero)
            return gamepadDirection;
        else
            return keyDirection;
    }

    public Vector3 isLeftDpad()//x���Az���ɒl���������x�N�g��3��Ԃ��܂�
    {
        //�Q�[���p�b�h�̓���
        var gamepad = Gamepad.current;
        Vector3 gamepadDirection = Vector3.zero;
        if (gamepad != null)
            gamepadDirection = new Vector3(gamepad.dpad.ReadValue().x, 0, gamepad.dpad.ReadValue().y);//�\���L�[

        //�L�[�{�[�h�̓���
        Vector3 keyDirection = Vector3.zero;//������������܂���wasd�ׂ̗̕��̃L�[�Ō��m���܂�
        if (Keyboard.current.fKey.isPressed)
            keyDirection.x--;
        if (Keyboard.current.hKey.isPressed)
            keyDirection.x++;
        if (Keyboard.current.tKey.isPressed)
            keyDirection.z++;
        if (Keyboard.current.gKey.isPressed)
            keyDirection.z--;
        keyDirection = keyDirection.normalized;

        //�\����������Ă�����\����D�悷��
        if (gamepadDirection != Vector3.zero)
            return gamepadDirection;
        else
            return keyDirection;
    }
    /*public bool isOption()//�ꉞ�A�Q�[�����̈ꎞ��~�{�^�����ł������ׂ̈̊֐�
    {
        if (Gamepad.current == null)
            return Keyboard.current.escapeKey.wasPressedThisFrame;
        return Gamepad.current.startButton.wasPressedThisFrame;
    }*/
    public bool isRightStickButton()//xBox�R���g���[���[�̉E�X�e�B�b�N��������
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.rightStickButton.IsPressed();
        return gamepadInput || Keyboard.current.pKey.wasPressedThisFrame;
    }
    public bool isLeftStickButton()//xBox�R���g���[���[�̍��X�e�B�b�N��������
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.leftStickButton.IsPressed();
        return gamepadInput || Keyboard.current.oKey.wasPressedThisFrame;
    }
    public bool isBackButton()//xBox�R���g���[���[��BACK�{�^��
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.selectButton.wasPressedThisFrame;
        return gamepadInput || Keyboard.current.escapeKey.wasPressedThisFrame;
    }
    public bool isWestButton()//�E�Ă̍��ɂ���{�^��
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.xButton.wasPressedThisFrame;
        return gamepadInput || Keyboard.current.xKey.wasPressedThisFrame;
    }
    public bool isNorthButton()//�E�Ă̏�ɂ���{�^��
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.yButton.wasPressedThisFrame;
        return gamepadInput|| Keyboard.current.cKey.wasPressedThisFrame;
    }
    public bool isEastButton()//�E�ẲE�ɂ���{�^��
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.bButton.wasPressedThisFrame;
        return gamepadInput || Keyboard.current.vKey.wasPressedThisFrame;
    }
    public bool isRightTrigger()//�E�̃g���K�[
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.rightTrigger.isPressed;
        return gamepadInput || Keyboard.current.mKey.wasPressedThisFrame;
    }
    public bool isLeftTrigger()//���̃g���K�[
    {
        bool gamepadInput = false;
        if (Gamepad.current != null)
            gamepadInput = Gamepad.current.leftTrigger.isPressed;
        return gamepadInput || Keyboard.current.nKey.wasPressedThisFrame;
    }
    public bool isResetHighScoreKey()//�n�C�X�R�A�̃��Z�b�g�{�^��
    {
        return Keyboard.current.deleteKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame;
    }



    //�ȉ�������(�m�F�p)�ł�----------------------
    //private float speed = 10;
    private void Update()
    {
        //transform.Translate(isMove() * Time.deltaTime * speed);
        //if (isJump())
        //    Debug.Log("�W�����v����");
        //if (isRotateCameraLeft())
        //    Debug.Log("�J���������ɓ������Ă��܂�");
        //if (isRotateCameraRight())
        //    Debug.Log("�J�������E�ɓ������Ă��܂�");
        //if (isRotateCameraLeft() && isRotateCameraRight())
        //    Debug.Log("�J�������������Z�b�g���܂���");
        //if (isRightStick() != Vector3.zero)
        //    Debug.Log(isRightStick());
        //if (isLeftDpad() != Vector3.zero)
        //    Debug.Log(isLeftDpad());
        //if (isBackButton())
        //    Debug.Log("�o�b�N�{�^����������܂���");
        //if (isXButton())
        //    Debug.Log("x�{�^����������܂���");
        //if (isYButton())
        //    Debug.Log("y�{�^����������܂���");
        //if (isAButton())
        //    Debug.Log("a�{�^����������܂���");
        //if (isRightTrigger())
        //    Debug.Log("�E�g���K�[��������܂���");
        //if (isLeftTrigger())
        //    Debug.Log("���g���K�[��������܂���");
        //Debug.Log(Gamepad.current.rightStickButton.isPressed);
    }
}
