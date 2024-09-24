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

    GameObject donutBounceScaleAxis = null;//�h�[�i�c���g��k������Ƃ��̐e
    bool isBouncing = false;//���݃o�E���h�̃A�j���[�V��������
    [SerializeField] float maxBounceTime = 0.5f;
    [SerializeField] float maxBounceScaleSize = 1f;
    float bounceTimer = 0f;
    [Tooltip("�h�[�i�c�̃o�E���h�̓����̐���")]
    [SerializeField] AnimationCurve bounceScaleCurve;

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
        if (isBouncing)
        {
            if(donutBounceScaleAxis != null)
            {
                bounceTimer += Time.deltaTime;
                if(bounceTimer < maxBounceTime)
                {
                    float bounceScaleValue = bounceScaleCurve.Evaluate(bounceTimer / maxBounceTime) * maxBounceScaleSize;
                    donutBounceScaleAxis.transform.localScale = new Vector3(
                        bounceScaleValue, bounceScaleValue, 1f / bounceScaleValue);
                }
                else//�o�E���h�̃A�j���[�V�����͏I�����
                {
                    foreach (Transform child in transform)
                    {
                        if (child.tag == "Donuts")
                        {
                            child.GetComponent<MeshRenderer>().enabled = true;
                        }
                    }
                    isBouncing = false;
                    Destroy(donutBounceScaleAxis);
                }
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
            if (collision.transform.GetComponent<DonutsUnionScript>().isBouncing) return;

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
        Vector3 playerStandDonutPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        foreach (Transform child in transform)//�v���C���[�̉��̃h�[�i�c�𒲂ׂ�
        {
            if (Vector3.Distance(transform.position, playerStandDonutPosition) >
                Vector3.Distance(transform.position, child.position))
            {
                playerStandDonutPosition = child.position;
            }
        }
        donutBounceScaleAxis = new GameObject("donutBounceScaleAxis");
        donutBounceScaleAxis.transform.position = playerStandDonutPosition;
        donutBounceScaleAxis.transform.LookAt(lookAtTransform);
        donutBounceScaleAxis.transform.parent = transform;
        bounceTimer = 0f;
        isBouncing = true;

        foreach (Transform child in transform)
        {
            if (child.tag == "Donuts")
            {
                GameObject donutMeshObj = Instantiate(child.gameObject);
                donutMeshObj.transform.position = child.position;
                Component[] donutComponents = donutMeshObj.GetComponents<Component>();
                foreach (Component component in donutComponents)
                {
                    if (!(component is Transform) && !(component is MeshFilter) && !(component is MeshRenderer))
                    {
                        Destroy(component);
                    }
                }
                child.GetComponent<MeshRenderer>().enabled = false;
                donutMeshObj.transform.parent = donutBounceScaleAxis.transform;
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

    void SetStickyEffect(bool active)
    {
        foreach(var sphere in donutSpheres)
        {
            sphere.transform.GetChild(0).gameObject.SetActive(active);
        }
    }
}
