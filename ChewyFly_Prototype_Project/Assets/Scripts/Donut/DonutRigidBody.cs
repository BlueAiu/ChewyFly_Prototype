using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DonutRigidBody : MonoBehaviour
{
    Rigidbody rb;
    DonutsUnionScript union;

    [Header("ドーナツの浮力")]
    [Tooltip("浮力係数")]
    [SerializeField] float buoyancy = 20f;
    [Tooltip("油の表面のY座標")]
    [SerializeField] float surfaceY = 0f;

    [Header("油中の抵抗力")]
    [Tooltip("移動の抵抗力係数")]
    [SerializeField] float movementResistance = 2f;
    [Tooltip("回転の抵抗力係数")]
    [SerializeField] float rotationResistance = 2f;

    Vector3 impulse = Vector3.zero;
    public Vector3 bounce { get; set; } = Vector3.zero;
    [Tooltip("バウンドの強さ")]
    [SerializeField] float boundPower = 20f;

    float torque = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        union = GetComponent<DonutsUnionScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        cameraAxis = GameObject.Find("CameraAxis").transform;
    }

    // Update is called once per frame
    void Update()
    {
        FinishedDonutMove();
    }

    private void FixedUpdate()
    {
        if (isFinishMoving) return;

        //プレイヤーから受け取った弾き入力
        if(impulse != Vector3.zero)
        {
            rb.AddForce(impulse, ForceMode.VelocityChange);
            impulse = Vector3.zero;
            union.IsSticky = true;
        }
        
        //ドーナツや壁、泡に当たった時のバウンド
        if (bounce != Vector3.zero)
        {
            rb.AddForce(bounce, ForceMode.Impulse);
            bounce = Vector3.zero;
        }

        //プレイヤーから受け取った回転入力
        rb.AddTorque(Vector3.up * torque, ForceMode.Acceleration);
        //乗ってるドーナツが転覆しないようにする
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    //プレイヤーから弾き入力を受け取る
    public void TakeImpulse(Vector3 _impulse)
    {
        impulse += _impulse;
    }

    //プレイヤーから回転入力を受け取る
    public void SetTorque(float _torque)
    {
        torque = _torque;
    }

    private void OnTriggerStay(Collider other)
    {
        //油に浸かっているとき
        if(other.name == "Oil")
        {
            //沈んでいる体積を求める
            float depth = surfaceY - transform.position.y;
            float sinkVolume = Mathf.Min(depth + transform.localScale.y / 2, transform.localScale.y);

            //浮力
            rb.AddForce(Vector3.up * buoyancy * sinkVolume);

            //抵抗力
            rb.AddForce(-rb.velocity.normalized * (rb.velocity.sqrMagnitude * movementResistance * sinkVolume));

            rb.AddTorque(-rb.angularVelocity.normalized * (rb.angularVelocity.sqrMagnitude * rotationResistance * sinkVolume));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //ドーナツ同士のバウンド
        if (!union.IsSticky && collision.gameObject.tag == "Donut")
        {
            //バウンドの方向を計算
            Vector3 boundDirection = transform.position - collision.transform.position;
            boundDirection = boundDirection.normalized;

            //バウンドの力量を保存
            bounce += boundDirection * boundPower;
        }
    }
}
