using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerCameraRotation
{
    CharacterController character;
    //InputScript input;

    [Tooltip("移動の速さ")]
    [SerializeField] float speed = 5f;

    float velocityY = 0f;

    [Tooltip("重力加速度")]
    [SerializeField] float gravity = 10f;
    
    [Tooltip("ジャンプ力")]
    [SerializeField] float jumpPower = 3f;
    [Tooltip("終端速度 (jumpPower以上にすること)")]
    [SerializeField] float terminalVelocity = 3f;

    [Tooltip("プレイヤーが移動方向に向く速さ")]
    [SerializeField] float playerRotateSpeed = 450f;
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        //input = GetComponent<InputScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //移動
        var direction = input.isMove();
        direction = playerCamera.transform.TransformDirection(direction);

        if(direction.magnitude > 0)//入力されているなら
        {
            UpdateRotation(direction);
        }

        character.Move(direction * (speed *  Time.deltaTime));

        velocityY -= gravity * Time.deltaTime;
        velocityY = Mathf.Max(velocityY, -terminalVelocity);
        character.Move(Vector3.up * (velocityY * Time.deltaTime));

        //ジャンプ
        if (character.isGrounded)
        {
            if (input.isJump())
            {
                velocityY = jumpPower;
            }
        }
    }
    private void UpdateRotation(Vector3 dir)
    {
        Quaternion from = transform.rotation;
        Quaternion to = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(from, to, playerRotateSpeed * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Donuts")
        {
            //hit.gameObject.GetComponent<DonutSphereReference>().OnPlayerEnter();
        }
    }
}
