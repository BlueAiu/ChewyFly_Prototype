using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutRigidBody : MonoBehaviour
{
    Rigidbody rb;
    DonutsUnionScript union;

    [Header("ドーナツの浮力")]
    [Tooltip("浮力係数")]
    [SerializeField] float buoyancy = 20f;
    [Tooltip("油の表面のY座標")]
    [SerializeField] float surfaceY = 0f;

    [Header("油中の抵抗力")]
    [Tooltip("抵抗力係数")]
    [SerializeField] float resistance = 2f;

    Vector3 impulse = Vector3.zero;
    Vector3 bounce = Vector3.zero;
    [Tooltip("バウンドの強さ")]
    [SerializeField] float boundPower = 20f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        union = GetComponent<DonutsUnionScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if(impulse != Vector3.zero)
        {
            rb.AddForce(impulse, ForceMode.VelocityChange);
            impulse = Vector3.zero;
            union.IsSticky = true;
        }
        
        if (bounce != Vector3.zero)
        {
            rb.AddForce(bounce, ForceMode.VelocityChange);
            bounce = Vector3.zero;
        }
    }

    public void TakeImpulse(Vector3 _impulse)
    {
        impulse += _impulse;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.name == "Oil")
        {
            //沈んでいる体積を求める
            float depth = surfaceY - transform.position.y;
            float sinkVolume = Mathf.Min(depth + transform.localScale.y / 2, transform.localScale.y);

            //浮力
            rb.AddForce(Vector3.up * buoyancy * sinkVolume);

            //摩擦力
            rb.AddForce(-rb.velocity.normalized * (rb.velocity.sqrMagnitude * resistance * sinkVolume));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!union.IsSticky)
        {
            Vector3 boundDirection = transform.position - collision.transform.position;
            boundDirection = boundDirection.normalized;

            bounce += boundDirection * boundPower;
        }
    }
}
