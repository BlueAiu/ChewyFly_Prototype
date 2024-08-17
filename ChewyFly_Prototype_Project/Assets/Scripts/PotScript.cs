using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotScript : MonoBehaviour
{
    [Tooltip("�h�[�i�c�𒆉��֒e����΂���")]
    [SerializeField] float donutBoundPower = 3f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Donuts")
        {
            //�o�E���h�̕������v�Z
            Vector3 boundDirection = Vector3.up * collision.transform.position.y - collision.transform.position;
            boundDirection = boundDirection.normalized;

            //�o�E���h�̗͗ʂ�^����
            collision.gameObject.GetComponent<DonutRigidBody>().bounce = boundDirection * donutBoundPower;
        }
    }
}
