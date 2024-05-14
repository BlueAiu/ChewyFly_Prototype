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
        //水平方向の移動
        var direction = input.isMove();
        character.Move(direction * speed *  Time.deltaTime);

        //垂直方向の移動
        velocityY -= gravity * Time.deltaTime;

        character.Move(Vector3.up * velocityY * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        velocityY = 0f;

        if (input.isJump())
        {
            velocityY = jumpPower;
        }
    }
}
