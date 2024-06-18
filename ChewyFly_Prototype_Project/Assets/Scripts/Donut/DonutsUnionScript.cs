using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutsUnionScript : MonoBehaviour
{
    [Tooltip("�������ɐ����傩��Q�Ƃ�n�����")]
    public ObjectReferenceManeger objManeger;

    [Tooltip("�q�̃h�[�i�c���L�^���郊�X�g")]
    [SerializeField]
    List<GameObject> donutSpheres = new List<GameObject>();

    //[Header("���̎�")]
    //[Tooltip("�v���C���[�̓���̍���")]
    //[SerializeField] float overHeadHeight = 2f;
    //[Tooltip("�h�[�i�c�̔��a")]
    //[SerializeField] float donutRadius = 1f;

    Rigidbody rb;
    //�����������Ԃł��邩
    public bool IsSticky { get; set; } = false;
    [Tooltip("�h�[�i�c������������")]
    [SerializeField] public float stickySpeed = 5f;

    //�������Ă���h�[�i�c�̐�
    int unionCount = 1;
    [Tooltip("���̂̍ő吔")]
    [SerializeField] int unionCountMax = 6;

    //�h�[�i�c���������Ă��邩
    public bool IsComplete { get; private set; } = false;

    //�h�[�i�c�̎��ʂ̑����{��
    [SerializeField] float donutMassRate = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (donutSpheres.Count == 0)
        {
            donutSpheres.Add(transform.GetChild(0).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //���x���x���Ƃ������Ȃ�
        if (rb.velocity.sqrMagnitude < stickySpeed * stickySpeed)
        {
            IsSticky = false;
        }
    }

    //�h�[�i�c�����X�g�ɒǉ�(�v)
    //public int AddSphere(GameObject obj)
    //{
    //    donutSpheres.Add(obj);
    //    return donutSpheres.Count;
    //}

    //��̂����h�[�i�c������(�v)
    //public void CombineDonuts(Vector3 position)
    //{
    //    transform.position = position + Vector3.up * overHeadHeight;

    //    //���ƃI�u�W�F�N�g�������̎q�ɂ���
    //    foreach(var i in donutSpheres)
    //    {
    //        i.GetComponent<Rigidbody>().isKinematic = true;
    //        i.transform.parent = this.transform;
    //        i.GetComponent<DonutSphereReference>().DisableMark();
    //    }

    //    //���ƃI�u�W�F�N�g���ړ�������
    //    for (int i = 0; i < donutSpheres.Count; i++)
    //    {
    //        Vector3 lineUpPos = Vector3.zero;
    //        lineUpPos.x = Mathf.Cos(i * Mathf.PI / (donutSpheres.Count / 2));
    //        lineUpPos.z = Mathf.Sin(i * Mathf.PI / (donutSpheres.Count / 2));
    //        lineUpPos *= donutRadius;

    //        donutSpheres[i].transform.position = transform.position +  lineUpPos;
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Donuts" && IsSticky && unionCount < unionCountMax) 
        {
            int childCount = collision.transform.childCount;

            //����̎q��S�Ă������Ɉڂ�
            for(int i=0;i < childCount;i++)
            {
                var child = collision.transform.GetChild(0);
                child.parent = transform;
                child.localPosition -= new Vector3(0, child.localPosition.y, 0);
                donutSpheres.Add(child.gameObject);
                unionCount++;
            }

            //���ʂ��v�Z
            rb.mass = 1 + (unionCount - 1) * donutMassRate;
            //�Փˑ��������
            objManeger.RemoveDonut(collision.gameObject);
            //������������͂������Ȃ�
            IsSticky = false;

            if(unionCount >= unionCountMax) //�h�[�i�c����������
            {
                IsComplete = true;
                rb.velocity = Vector3.zero;
                objManeger.CompleteDonut(this.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.name == "Oil") //���ɐZ�����Ă��鎞
        {
            int bakedValue = 0;
            foreach(var sphere in donutSpheres) //�Ă��F��ς���
            {
                var sphereColor = sphere.GetComponent<DonutSphereColor>();
                sphereColor.BakeDonut();
                bakedValue += sphereColor.BakedNum;
            }

            if(bakedValue == (int)DonutBakedState.Burnt * donutSpheres.Count)   //�S�ďł������A�h�[�i�c������
            {
                objManeger.RemoveDonut(gameObject);
            }
        }
    }
}
