using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScript : MonoBehaviour
{
    [Tooltip("�����ȖA�G�t�F�N�g")]
    [SerializeField] GameObject smallBubble;
    [Tooltip("�����яオ��܂ł̎���")]
    [SerializeField] float floatUpTime = 2f;
    [Tooltip("���ʂ�y���W")]
    [SerializeField] float surfaceY = 1f;
    [Tooltip("����(�����яオ�鑬��)")]
    [SerializeField] float buoyancy = 1f;
    [Tooltip("�G�ꂽ�h�[�i�c��e����΂���")]
    [SerializeField] float donutBoundPower = 5f;

    new Renderer renderer;
    new Rigidbody rigidbody;
    float actionTimer = 0f;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        renderer.enabled = false;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
        actionTimer += Time.deltaTime;

        if(actionTimer > floatUpTime)
        {
            if (!renderer.enabled) renderer.enabled = true;
            if (rigidbody.isKinematic) rigidbody.isKinematic = false;

            //����ł���̐ς����߂�
            float depth = surfaceY - transform.position.y;
            float sinkVolume = Mathf.Min(depth + transform.localScale.y / 2, transform.localScale.y);

            //����
            rigidbody.AddForce(Vector3.up * (buoyancy * sinkVolume));
        }

        if(transform.position.y > surfaceY)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Donuts")
        {
            //�o�E���h�̕������v�Z
            Vector3 boundDirection = collision.transform.position - transform.position;
            boundDirection -= Vector3.up * boundDirection.y;
            boundDirection = boundDirection.normalized;

            //�o�E���h�̗͗ʂ�^����
            collision.gameObject.GetComponent<DonutRigidBody>().bounce = boundDirection * donutBoundPower;
        }
    }
}
