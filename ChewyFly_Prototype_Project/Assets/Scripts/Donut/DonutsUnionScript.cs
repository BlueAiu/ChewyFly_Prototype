using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class DonutsUnionScript : MonoBehaviour
{
    Rigidbody rb;
    //AudioSource mergeSE;

    [Tooltip("生成時に生成主から参照を渡される")]
    public ObjectReferenceManeger objManeger;

    [Tooltip("子のドーナツを記録するリスト")]
    [SerializeField] public
    List<GameObject>donutSpheres = new List<GameObject>();

    //[Header("合体時")]
    //[Tooltip("プレイヤーの頭上の高さ")]
    //[SerializeField] float overHeadHeight = 2f;
    //[Tooltip("ドーナツの半径")]
    //[SerializeField] float donutRadius = 1f;

    //くっつけられる状態であるか
    bool _isSticky = false;
    public bool IsSticky 
    {
        get {  return _isSticky; }
        set { _isSticky = value; SetStickyEffect(value); }
    }

    [Tooltip("ドーナツがくっつく速さ")]
    [SerializeField] public float stickySpeed = 5f;

    //くっついているドーナツの数
    public int unionCount { get; private set; } = 1;
    [Tooltip("合体の最大数")]
    [SerializeField] int unionCountMax = 6;

    //ドーナツが完成しているか
    public bool IsComplete { get; private set; } = false;

    [Tooltip("ドーナツの質量の増加倍率")]
    [SerializeField] float donutMassRate = 1f;

    [SerializeField] SoundManager soundManager;
    [SerializeField] AudioClip mergeSE;

    GameObject donutBounceScaleAxis = null;//ドーナツを拡大縮小するときの親
    bool isBouncing = false;//現在バウンドのアニメーション中か
    [SerializeField] float maxBounceTime = 0.5f;
    [SerializeField] float maxBounceScaleSize = 1f;
    float bounceTimer = 0f;
    [Tooltip("ドーナツのバウンドの動きの推移")]
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
        //速度が遅いとくっつかない
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
                else//バウンドのアニメーションは終わった
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

    //ドーナツをリストに追加(没)
    //public int AddSphere(GameObject obj)
    //{
    //    donutSpheres.Add(obj);
    //    return donutSpheres.Count;
    //}

    //印のついたドーナツを合体(没)
    //public void CombineDonuts(Vector3 position)
    //{
    //    transform.position = position + Vector3.up * overHeadHeight;

    //    //もとオブジェクトを自分の子にする
    //    foreach(var i in donutSpheres)
    //    {
    //        i.GetComponent<Rigidbody>().isKinematic = true;
    //        i.transform.parent = this.transform;
    //        i.GetComponent<DonutSphereReference>().DisableMark();
    //    }

    //    //もとオブジェクトを移動させる
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

            if (unionCount >= unionCountMax) //ドーナツが完成する
            {
                IsComplete = true;
                rb.velocity = Vector3.zero;
                objManeger.CompleteDonut(this.gameObject);
            }

            InstantiateBounceDonuts(collision.transform);
        }
    }
    private void InstantiateBounceDonuts(Transform lookAtTransform)//バウンドするための見た目だけのドーナツを用意する
    {
        Vector3 playerStandDonutPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        foreach (Transform child in transform)//プレイヤーの下のドーナツを調べる
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
        if(other.name == "Oil") //油に浸かっている時
        {
            int bakedValue = 0;
            foreach(var sphere in donutSpheres) //焼き色を変える
            {
                var sphereColor = sphere.GetComponent<DonutSphereColor>();
                sphereColor.BakeDonut();
                bakedValue += sphereColor.BakedNum;
            }

            if(bakedValue == (int)DonutBakedState.Burnt * donutSpheres.Count)   //全て焦げた時、ドーナツを消去
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
