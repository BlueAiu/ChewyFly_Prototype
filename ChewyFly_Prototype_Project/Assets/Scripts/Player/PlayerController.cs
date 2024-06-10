using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController character;
    InputScript input;

    [Tooltip("プレイヤーを映すカメラ")]
    [SerializeField] protected GameObject playerCamera;

    [Tooltip("乗ってるドーナツ")]
    [SerializeField] GameObject ridingDonut;

    [Header("空中にいるときの移動")]
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

    Vector3 previousDirection = Vector3.zero;

    [Header("ドーナツの上に乗っている場合の弾き操作")]
    [Tooltip("小さいと判定する境界")]
    [SerializeField] float flicBorder = 0.1f;
    [Tooltip("弾いたと判定する早さ")]
    [SerializeField] float flicSpeed = 5f;
    [Tooltip("弾く力の強さ")]
    [SerializeField] float flicPower = 20f;

    [Tooltip("ドーナツを回転させる速さ")]
    [SerializeField] float rotateSpeed = 5f;

    private void Awake()//Startよりさらに前に格納しておく
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
            //移動
            velocityY -= gravity * Time.deltaTime;
            velocityY = Mathf.Max(velocityY, -terminalVelocity);

            Vector3 velocity = direction * speed + Vector3.up * velocityY;

            character.Move(velocity * Time.deltaTime);
        }
        else if(input.isAButton())  //乗ってるドーナツを切り離してジャンプ
        {
            ridingDonut = null;
            transform.parent = null;
            velocityY = jumpPower;
        }
    }

    private void FixedUpdate()
    {
        if (ridingDonut != null)    //ドーナツに乗っている場合
        {
            var direction = input.isLeftStick();
            direction = playerCamera.transform.TransformDirection(direction);

            var dounutRigid = ridingDonut.GetComponent<DonutRigidBody>();

            //弾き入力
            if (isFlic(direction))
            {
                dounutRigid.TakeImpulse(previousDirection.normalized * -flicPower);
            }

            //回転入力
            dounutRigid.SetTorque(RotateInput() * rotateSpeed);

            previousDirection = direction;
        }
    }

    //左スティックの方向にプレイヤーの正面を向ける
    //FacingForward
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
        if (hit.gameObject.tag == "Donuts" && character.isGrounded) //ドーナツに着地
        {
            ridingDonut = hit.transform.parent.gameObject;
            transform.parent = ridingDonut.transform;
            //hit.gameObject.GetComponent<DonutSphereReference>().OnPlayerEnter();
        }
    }
}
