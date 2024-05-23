using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerCameraRotation
{
    CharacterController character;
    //InputScript input;

    [Tooltip("乗ってるドーナツ")]
    [SerializeField] GameObject ridingDonut;

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

    [Header("ドーナツを弾く")]
    Vector3 previousDirection = Vector3.zero;

    [Tooltip("小さいと判定する境界")]
    [SerializeField] float flicBorder = 0.1f;
    [Tooltip("弾いたと判定する早さ")]
    [SerializeField] float flicSpeed = 5f;
    [Tooltip("弾く力の強さ")]
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
            //移動
            velocityY -= gravity * Time.deltaTime;
            velocityY = Mathf.Max(velocityY, -terminalVelocity);

            Vector3 velocity = direction * speed + Vector3.up * velocityY;

            character.Move(velocity * Time.deltaTime);

            //ジャンプ
            if (character.isGrounded && input.isJump())
            {
                velocityY = jumpPower;
            }
        }
    }

    private void FixedUpdate()
    {
        if (ridingDonut != null)    //ドーナツに乗っている場合
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
        if (dir.sqrMagnitude <= 0) return;  //入力されているなら

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
