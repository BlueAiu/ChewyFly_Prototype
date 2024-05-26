using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutRigidBody : MonoBehaviour
{
    Rigidbody rb;
    DonutsUnionScript union;

    [Header("�h�[�i�c�̕���")]
    [Tooltip("���͌W��")]
    [SerializeField] float buoyancy = 20f;
    [Tooltip("���̕\�ʂ�Y���W")]
    [SerializeField] float surfaceY = 0f;

    [Header("�����̒�R��")]
    [Tooltip("��R�͌W��")]
    [SerializeField] float resistance = 2f;

    Vector3 impulse = Vector3.zero;
    Vector3 bounce = Vector3.zero;
    [Tooltip("�o�E���h�̋���")]
    [SerializeField] float boundPower = 20f;

    float torque = 0;


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

        rb.AddTorque(Vector3.up * torque, ForceMode.Acceleration);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    public void TakeImpulse(Vector3 _impulse)
    {
        impulse += _impulse;
    }

    public void SetTorque(float _torque)
    {
        torque = _torque;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.name == "Oil")
        {
            //����ł���̐ς����߂�
            float depth = surfaceY - transform.position.y;
            float sinkVolume = Mathf.Min(depth + transform.localScale.y / 2, transform.localScale.y);

            //����
            rb.AddForce(Vector3.up * buoyancy * sinkVolume);

            //��R��
            rb.AddForce(-rb.velocity.normalized * (rb.velocity.sqrMagnitude * resistance * sinkVolume));

            rb.AddTorque(-rb.angularVelocity.normalized * (rb.angularVelocity.sqrMagnitude * resistance * sinkVolume));
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
