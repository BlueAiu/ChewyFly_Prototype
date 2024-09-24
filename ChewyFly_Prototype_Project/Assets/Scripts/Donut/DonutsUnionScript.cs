using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class DonutsUnionScript : MonoBehaviour
{
    Rigidbody rb;
    //AudioSource mergeSE;

    [Tooltip("�������ɐ����傩��Q�Ƃ�n�����")]
    public ObjectReferenceManeger objManeger;

    [Tooltip("�q�̃h�[�i�c���L�^���郊�X�g")]
    [SerializeField] public
    List<GameObject>donutSpheres = new List<GameObject>();

    //[Header("���̎�")]
    //[Tooltip("�v���C���[�̓���̍���")]
    //[SerializeField] float overHeadHeight = 2f;
    //[Tooltip("�h�[�i�c�̔��a")]
    //[SerializeField] float donutRadius = 1f;

    //�����������Ԃł��邩
    bool _isSticky = false;
    public bool IsSticky 
    {
        get {  return _isSticky; }
        set { _isSticky = value; SetStickyEffect(value); }
    }

    [Tooltip("�h�[�i�c������������")]
    [SerializeField] public float stickySpeed = 5f;

    //�������Ă���h�[�i�c�̐�
    public int unionCount { get; private set; } = 1;
    [Tooltip("���̂̍ő吔")]
    [SerializeField] int unionCountMax = 6;

    //�h�[�i�c���������Ă��邩
    public bool IsComplete { get; private set; } = false;

    [Tooltip("�h�[�i�c�̎��ʂ̑����{��")]
    [SerializeField] float donutMassRate = 1f;

    [SerializeField] SoundManager soundManager;
    [SerializeField] AudioClip mergeSE;

    [Tooltip("�h�[�i�c�̃o�E���h�̓����̕ω�(������0����1�̊Ԃ�)")]
    [SerializeField] AnimationCurve bounceScaleCurve;
    GameObject bounceDonutsParent = null;//�h�[�i�c���g��k������Ƃ��̑S�̂̐e(union�̃g�����X�t�H�[�����R�s�[��������)
    GameObject bounceDonutsAxis = null;//�h�[�i�c���g��k������Ƃ��̎�
    [SerializeField] float maxBounceTime = 0.5f;
    [SerializeField] float maxBounceScaleSize = 1f;
    float bounceTimer = 0f;
    public bool isBouncing { get; private set; } = false;//���݃o�E���h�̃A�j���[�V��������

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (donutSpheres.Count == 0)
        {
            donutSpheres.Add(transform.GetChild(0).gameObject);
        }
        if(hexaPositions.Count == 0)
        {
            hexaPositions.Add(Vector2.zero);
        }
        //mergeSE = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //���x���x���Ƃ������Ȃ�
        if (rb.velocity.sqrMagnitude < stickySpeed * stickySpeed)
        {
            IsSticky = false;
        }
        if (isBouncing)//�o�E���h�̃A�j���[�V������
        {
            CopyUnionPosAndScale();
            bounceTimer += Time.deltaTime;
            if (bounceTimer < maxBounceTime)
            {
                float bounceScaleValue = bounceScaleCurve.Evaluate(bounceTimer / maxBounceTime) * maxBounceScaleSize;
                bounceDonutsAxis.transform.localScale = new Vector3(
                    bounceScaleValue, bounceScaleValue, 1f / bounceScaleValue);
            }
            else//�o�E���h�̃A�j���[�V�����I���
            {
                StopDonutsBounce();
            }
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
        if (collision.gameObject.tag == "Donuts" && IsSticky && unionCount < unionCountMax && !isBouncing)
        {
            DonutsUnionScript otherDonutsUnion = collision.transform.GetComponent<DonutsUnionScript>();
            //����̃h�[�i�c�������o�E���h�Đ����Ȃ�~�߂�
            if (otherDonutsUnion.isBouncing) { otherDonutsUnion.StopDonutsBounce(); }

            MergeDonuts(collision);

            //mergeSE.Play();
            soundManager.PlaySE(mergeSE);

            if (unionCount >= unionCountMax) //�h�[�i�c����������
            {
                IsComplete = true;
                rb.velocity = Vector3.zero;
                objManeger.CompleteDonut(this.gameObject);
            }

            InstantiateBounceDonuts(collision.transform);
        }
    }
    private void InstantiateBounceDonuts(Transform lookAtTransform)//�o�E���h���邽�߂̌����ڂ����̃h�[�i�c��p�ӂ���
    {
        if (isBouncing) StopDonutsBounce();

        bounceDonutsParent = new GameObject("BounceDonutsParent");//�o�E���h����h�[�i�c�̑S�̂̐e��p��
        CopyUnionPosAndScale();

        bounceDonutsAxis = new GameObject("BounceDonutsAxis");//�o�E���h����h�[�i�c�̊g��k���̒��S��p��
        bounceDonutsAxis.transform.parent = bounceDonutsParent.transform;
        bounceDonutsAxis.transform.localPosition = Vector3.zero;
        bounceDonutsAxis.transform.LookAt(lookAtTransform);
        bounceTimer = 0f;
        isBouncing = true;

        foreach (Transform child in transform)//�R���C�_�[�Ȃǂ̒��g���Ȃ��Amesh�݂̂̃h�[�i�c��p��
        {
            if (child.tag == "Donuts")
            {
                GameObject donutMeshObj = Instantiate(child.gameObject);//�����̃h�[�i�c�𕡐�
                donutMeshObj.transform.position = child.position;

                DonutSphereColor donutColor = child.GetComponent<DonutSphereColor>();//DonutSphereColor�̒��g�𓯂��ɂ���
                DonutSphereColor bounceDonutColor = donutMeshObj.GetComponent<DonutSphereColor>();
                bounceDonutColor.CopySphereColorValue(donutColor);

                Component[] donutComponents = donutMeshObj.GetComponents<Component>();
                foreach (Component component in donutComponents)//�폜���Č����ڂ����ɂ���
                {
                    if (!(component is Transform) && !(component is MeshFilter) && !(component is MeshRenderer) && !(component is DonutSphereColor))
                    {
                        Destroy(component);
                    }
                }
                child.GetComponent<MeshRenderer>().enabled = false;//���Ƃ̃h�[�i�c�͂��������\��
                donutMeshObj.transform.parent = bounceDonutsAxis.transform;
            }
        }
    }
    private void CopyUnionPosAndScale()//�R�s�[�����o�E���h�p�̃h�[�i�c�̈ʒu�����X�̃h�[�i�c�̈ʒu�ɍX�V
    {
        bounceDonutsParent.transform.position = transform.position;
        bounceDonutsParent.transform.rotation = transform.rotation;
    }
    public void StopDonutsBounce()//�h�[�i�c�̃o�E���h�̃A�j���[�V�������~����
    {
        foreach (Transform child in transform)//�h�[�i�c�����ׂčĕ\��
        {
            if (child.tag == "Donuts")
            {
                child.GetComponent<MeshRenderer>().enabled = true;
            }
        }
        isBouncing = false;
        bounceTimer = 0f;
        Destroy(bounceDonutsParent);
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

    void SetStickyEffect(bool active)
    {
        foreach(var sphere in donutSpheres)
        {
            sphere.transform.GetChild(0).gameObject.SetActive(active);
        }
    }
}
