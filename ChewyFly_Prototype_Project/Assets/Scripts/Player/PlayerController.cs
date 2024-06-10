using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController character;
    InputScript input;

    [Tooltip("�v���C���[���f���J����")]
    [SerializeField] protected GameObject playerCamera;

    [Tooltip("����Ă�h�[�i�c")]
    [SerializeField] GameObject ridingDonut;

    [Header("�󒆂ɂ���Ƃ��̈ړ�")]
    [Tooltip("�ړ��̑���")]
    [SerializeField] float speed = 5f;

    float velocityY = 0f;

    [Tooltip("�d�͉����x")]
    [SerializeField] float gravity = 10f;
    
    [Tooltip("�W�����v��")]
    [SerializeField] float jumpPower = 3f;
    [Tooltip("�I�[���x (jumpPower�ȏ�ɂ��邱��)")]
    [SerializeField] float terminalVelocity = 3f;

    [Tooltip("�v���C���[���ړ������Ɍ�������")]
    [SerializeField] float playerRotateSpeed = 450f;

    Vector3 previousDirection = Vector3.zero;

    [Header("�h�[�i�c�̏�ɏ���Ă���ꍇ�̒e������")]
    [Tooltip("�������Ɣ��肷�鋫�E")]
    [SerializeField] float flicBorder = 0.1f;
    [Tooltip("�e�����Ɣ��肷�鑁��")]
    [SerializeField] float flicSpeed = 5f;
    [Tooltip("�e���͂̋���")]
    [SerializeField] float flicPower = 20f;

    [Tooltip("�h�[�i�c����]�����鑬��")]
    [SerializeField] float rotateSpeed = 5f;

    private void Awake()//Start��肳��ɑO�Ɋi�[���Ă���
    {
        character = GetComponent<CharacterController>();
        input = GetComponent<InputScript>();
        if (playerCamera == null)
            playerCamera = GameObject.Find("PlayerCameraParent");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var direction = input.isLeftStick();
        direction = playerCamera.transform.TransformDirection(direction);

        UpdateRotation(direction);

        if(ridingDonut == null)
        {
            //�ړ�
            velocityY -= gravity * Time.deltaTime;
            velocityY = Mathf.Max(velocityY, -terminalVelocity);

            Vector3 velocity = direction * speed + Vector3.up * velocityY;

            character.Move(velocity * Time.deltaTime);
        }
        else if(input.isAButton())  //����Ă�h�[�i�c��؂藣���ăW�����v
        {
            ridingDonut = null;
            transform.parent = null;
            velocityY = jumpPower;
        }
    }

    private void FixedUpdate()
    {
        if (ridingDonut != null)    //�h�[�i�c�ɏ���Ă���ꍇ
        {
            var direction = input.isLeftStick();
            direction = playerCamera.transform.TransformDirection(direction);

            var dounutRigid = ridingDonut.GetComponent<DonutRigidBody>();

            //�e������
            if (isFlic(direction))
            {
                dounutRigid.TakeImpulse(previousDirection.normalized * -flicPower);
            }

            //��]����
            dounutRigid.SetTorque(RotateInput() * rotateSpeed);

            previousDirection = direction;
        }
    }

    //���X�e�B�b�N�̕����Ƀv���C���[�̐��ʂ�������
    //FacingForward
    private void UpdateRotation(Vector3 dir)
    {
        if (dir.sqrMagnitude <= 0) return;  //���͂���Ă���Ȃ�

        Quaternion from = transform.rotation;
        Quaternion to = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(from, to, playerRotateSpeed * Time.deltaTime);
    }

    private bool isFlic(Vector3 dir)
    {
        if(dir.sqrMagnitude > flicBorder * flicBorder) return false;
        if(previousDirection.sqrMagnitude <= flicBorder * flicBorder) return false;

        float deltaMagnitude = previousDirection.magnitude - dir.magnitude;
        float rightInputSpeed = deltaMagnitude / Time.fixedDeltaTime;

        if(rightInputSpeed > flicSpeed) return true;
        else return false;
    }

    private float RotateInput()
    {
        float rotate = 0;
        if (input.isRightShoulder())
            rotate++;
        if (input.isLeftShoulder())
            rotate--;
        return rotate;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Donuts" && character.isGrounded) //�h�[�i�c�ɒ��n
        {
            ridingDonut = hit.transform.parent.gameObject;
            transform.parent = ridingDonut.transform;
            //hit.gameObject.GetComponent<DonutSphereReference>().OnPlayerEnter();
        }
    }
}
