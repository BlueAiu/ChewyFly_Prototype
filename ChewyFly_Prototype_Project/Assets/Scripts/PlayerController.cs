using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController character;
    InputScript input;

    [Tooltip("移動の速さ")]
    [SerializeField] float speed = 5f;

    float velocityY = 0f;

    [Tooltip("重力加速度")]
    [SerializeField] float gravity = 10f;

    [Tooltip("ジャンプ力")]
    [SerializeField] float jumpPower = 3f;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        input = GetComponent<InputScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //移動
        var direction = input.isMove();
        character.Move(direction * (speed *  Time.deltaTime));

        velocityY -= gravity * Time.deltaTime;
        character.Move(Vector3.up * (velocityY * Time.deltaTime));

        //着地とジャンプ
        if (character.isGrounded)
        {
            velocityY = 0f;

            if (input.isJump())
            {
                velocityY = jumpPower;
            }
        }
    }

}
