using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutRigdBody : MonoBehaviour
{
    Rigidbody rb;

    [Header("�h�[�i�c�̕���")]
    [Tooltip("���͌W��")]
    [SerializeField] float buoyancy = 1000f;
    [Tooltip("���̕\�ʂ�Y���W")]
    [SerializeField] float surfaceY = 0f;

    [Header("�h�[�i�c�Ɩ��̖��C��")]
    [Tooltip("���C�͌W��")]
    [SerializeField] float friction = 100f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.name == "Oil")
        {
            //����
            float depth = surfaceY - transform.position.y;
            float sinkVolume = Mathf.Min(depth + transform.localScale.y / 2, transform.localScale.y);
            rb.AddForce(Vector3.up * buoyancy * sinkVolume * Time.deltaTime);

            //���C��
            rb.AddForce(-rb.velocity * friction * Time.deltaTime);
        }
    }
}
