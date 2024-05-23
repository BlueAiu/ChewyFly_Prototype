using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerCameraRotation
{
    CharacterController character;
    //InputScript input;

    [Tooltip("����Ă�h�[�i�c")]
    [SerializeField] GameObject ridingDonut;

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

    [Header("�h�[�i�c��e��")]
    Vector3 previousDirection = Vector3.zero;

    [Tooltip("�������Ɣ��肷�鋫�E")]
    [SerializeField] float flicBorder = 0.1f;
    [Tooltip("�e�����Ɣ��肷�鑁��")]
    [SerializeField] float flicSpeed = 5f;
    [Tooltip("�e���͂̋���")]
    [SerializeField] float flicPower = 20f;
    
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        //input = GetComponent<InputScript>();
    }

    // Update is called once per frame
    void Update()
    {
        var direction = input.isMove();
        direction = playerCamera.transform.TransformDirection(direction);

        UpdateRotation(direction);

        if(ridingDonut == null)
        {
            //�ړ�
            velocityY -= gravity * Time.deltaTime;
            velocityY = Mathf.Max(velocityY, -terminalVelocity);

            Vector3 velocity = direction * speed + Vector3.up * velocityY;

            character.Move(velocity * Time.deltaTime);

            //�W�����v
            if (character.isGrounded && input.isJump())
            {
                velocityY = jumpPower;
            }
        }
    }

    private void FixedUpdate()
    {
        if (ridingDonut != null)    //�h�[�i�c�ɏ���Ă���ꍇ
        {
            var direction = input.isMove();

            if (isFlic(direction))
            {
                ridingDonut.GetComponent<DonutRigidBody>().
                    TakeImpulse(previousDirection.normalized * -flicPower);
            }

            previousDirection = direction;
        }
    }

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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Donuts")
        {
            //hit.gameObject.GetComponent<DonutSphereReference>().OnPlayerEnter();
        }
    }
}
