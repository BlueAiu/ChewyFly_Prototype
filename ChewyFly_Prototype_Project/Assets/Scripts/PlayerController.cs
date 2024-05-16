using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController character;
    InputScript input;

    [Tooltip("�ړ��̑���")]
    [SerializeField] float speed = 5f;

    float velocityY = 0f;

    [Tooltip("�d�͉����x")]
    [SerializeField] float gravity = 10f;

    [Tooltip("�W�����v��")]
    [SerializeField] float jumpPower = 3f;
    [Tooltip("�I�[���x (jumpPower�ȏ�ɂ��邱��)")]
    [SerializeField] float terminalVelocity = 3f;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        input = GetComponent<InputScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //�ړ�
        var direction = input.isMove();
        character.Move(direction * (speed *  Time.deltaTime));

        velocityY -= gravity * Time.deltaTime;
        velocityY = Mathf.Max(velocityY, -terminalVelocity);
        character.Move(Vector3.up * (velocityY * Time.deltaTime));

        //�W�����v
        if (character.isGrounded)
        {
            if (input.isJump())
            {
                velocityY = jumpPower;
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Donuts")
        {
            hit.gameObject.GetComponent<DonutSphereReference>().OnPlayerEnter();
        }
    }
}
