using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScript : MonoBehaviour
{
    [Tooltip("�����ȖA�G�t�F�N�g")]
    [SerializeField] GameObject smallBubble;
    [Tooltip("�e��������o���n�߂鎞��")]
    [SerializeField] float bubbleStartTime = 0.5f;
    [Tooltip("�e�����肪�؍݂��鎞��")]
    [SerializeField] float lifeTimeMin = 5f;
    [SerializeField] float lifeTimeMax = 10f;
    [Tooltip("�h�[�i�c��e����΂���")]
    [SerializeField] float donutBoundPower = 5f;

    new Collider collider;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;

        Invoke(nameof(BoundStart), bubbleStartTime);
        Invoke(nameof(Delete), bubbleStartTime + Random.Range(lifeTimeMin, lifeTimeMax));
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    void BoundStart()
    {
        collider.enabled = true;
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
