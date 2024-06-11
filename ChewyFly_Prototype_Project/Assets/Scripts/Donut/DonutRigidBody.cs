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
    [Tooltip("�ړ��̒�R�͌W��")]
    [SerializeField] float movementResistance = 2f;
    [Tooltip("��]�̒�R�͌W��")]
    [SerializeField] float rotationResistance = 2f;

    Vector3 impulse = Vector3.zero;
    Vector3 bounce = Vector3.zero;
    [Tooltip("�o�E���h�̋���")]
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

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        //�v���C���[����󂯎�����e������
        if(impulse != Vector3.zero)
        {
            rb.AddForce(impulse, ForceMode.VelocityChange);
            impulse = Vector3.zero;
            union.IsSticky = true;
        }
        
        //�h�[�i�c���m�̏Փˎ��Ɏ󂯎�����o�E���h
        if (bounce != Vector3.zero)
        {
            rb.AddForce(bounce, ForceMode.Impulse);
            bounce = Vector3.zero;
        }

        //�v���C���[����󂯎������]����
        rb.AddTorque(Vector3.up * torque, ForceMode.Acceleration);
        //����Ă�h�[�i�c���]�����Ȃ��悤�ɂ���
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    //�v���C���[����e�����͂��󂯎��
    public void TakeImpulse(Vector3 _impulse)
    {
        impulse += _impulse;
    }

    //�v���C���[�����]���͂��󂯎��
    public void SetTorque(float _torque)
    {
        torque = _torque;
    }

    private void OnTriggerStay(Collider other)
    {
        //���ɐZ�����Ă���Ƃ�
        if(other.name == "Oil")
        {
            //����ł���̐ς����߂�
            float depth = surfaceY - transform.position.y;
            float sinkVolume = Mathf.Min(depth + transform.localScale.y / 2, transform.localScale.y);

            //����
            rb.AddForce(Vector3.up * buoyancy * sinkVolume);

            //��R��
            rb.AddForce(-rb.velocity.normalized * (rb.velocity.sqrMagnitude * movementResistance * sinkVolume));

            rb.AddTorque(-rb.angularVelocity.normalized * (rb.angularVelocity.sqrMagnitude * rotationResistance * sinkVolume));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //�h�[�i�c���m�̃o�E���h
        if (!union.IsSticky)
        {
            //�o�E���h�̕������v�Z
            Vector3 boundDirection = transform.position - collision.transform.position;
            boundDirection = boundDirection.normalized;

            //�o�E���h�̗͗ʂ�ۑ�
            bounce += boundDirection * boundPower;
        }
    }
}