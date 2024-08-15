using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScript : MonoBehaviour
{
    [Tooltip("�����ȖA�G�t�F�N�g")]
    [SerializeField] GameObject smallBubble;
    [Tooltip("�؍݂��鎞��")]
    [SerializeField] float lifeTimeMin = 5f;
    [SerializeField] float lifeTimeMax = 10f;
    [Tooltip("�h�[�i�c��e����΂���")]
    [SerializeField] float donutBoundPower = 5f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(Delete), Random.Range(lifeTimeMin, lifeTimeMax));
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    void Delete()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Donuts")
        {
            //�o�E���h�̕������v�Z
            Vector3 boundDirection = other.transform.position - transform.position;
            boundDirection -= Vector3.up * boundDirection.y;
            boundDirection = boundDirection.normalized;

            //�o�E���h�̗͗ʂ�^����
            other.transform.parent.gameObject.GetComponent<DonutRigidBody>().bounce = boundDirection * donutBoundPower;
        }
    }
}
