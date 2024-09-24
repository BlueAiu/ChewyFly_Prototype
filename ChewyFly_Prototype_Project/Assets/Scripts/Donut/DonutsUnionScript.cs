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

    [Tooltip("ドーナツのバウンドの動きの変化(横軸は0から1の間で)")]
    [SerializeField] AnimationCurve bounceScaleCurve;
    GameObject bounceDonutsParent = null;//ドーナツを拡大縮小するときの全体の親(unionのトランスフォームをコピーし続ける)
    GameObject bounceDonutsAxis = null;//ドーナツを拡大縮小するときの軸
    [SerializeField] float maxBounceTime = 0.5f;
    [SerializeField] float maxBounceScaleSize = 1f;
    float bounceTimer = 0f;
    public bool isBouncing { get; private set; } = false;//現在バウンドのアニメーション中か

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
        if (isBouncing)//バウンドのアニメーション中
        {
            CopyUnionPosAndScale();
            bounceTimer += Time.deltaTime;
            if (bounceTimer < maxBounceTime)
            {
                float bounceScaleValue = bounceScaleCurve.Evaluate(bounceTimer / maxBounceTime) * maxBounceScaleSize;
                bounceDonutsAxis.transform.localScale = new Vector3(
                    bounceScaleValue, bounceScaleValue, 1f / bounceScaleValue);
            }
            else//バウンドのアニメーション終わり
            {
                StopDonutsBounce();
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
            DonutsUnionScript otherDonutsUnion = collision.transform.GetComponent<DonutsUnionScript>();
            //相手のドーナツがもしバウンド再生中なら止める
            if (otherDonutsUnion.isBouncing) { otherDonutsUnion.StopDonutsBounce(); }

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
        if (isBouncing) StopDonutsBounce();

        bounceDonutsParent = new GameObject("BounceDonutsParent");//バウンドするドーナツの全体の親を用意
        CopyUnionPosAndScale();

        bounceDonutsAxis = new GameObject("BounceDonutsAxis");//バウンドするドーナツの拡大縮小の中心を用意
        bounceDonutsAxis.transform.parent = bounceDonutsParent.transform;
        bounceDonutsAxis.transform.localPosition = Vector3.zero;
        bounceDonutsAxis.transform.LookAt(lookAtTransform);
        bounceTimer = 0f;
        isBouncing = true;

        foreach (Transform child in transform)//コライダーなどの中身がない、meshのみのドーナツを用意
        {
            if (child.tag == "Donuts")
            {
                GameObject donutMeshObj = Instantiate(child.gameObject);//既存のドーナツを複製
                donutMeshObj.transform.position = child.position;

                DonutSphereColor donutColor = child.GetComponent<DonutSphereColor>();//DonutSphereColorの中身を同じにする
                DonutSphereColor bounceDonutColor = donutMeshObj.GetComponent<DonutSphereColor>();
                bounceDonutColor.CopySphereColorValue(donutColor);

                Component[] donutComponents = donutMeshObj.GetComponents<Component>();
                foreach (Component component in donutComponents)//削除して見た目だけにする
                {
                    if (!(component is Transform) && !(component is MeshFilter) && !(component is MeshRenderer) && !(component is DonutSphereColor))
                    {
                        Destroy(component);
                    }
                }
                child.GetComponent<MeshRenderer>().enabled = false;//もとのドーナツはいったん非表示
                donutMeshObj.transform.parent = bounceDonutsAxis.transform;
            }
        }
    }
    private void CopyUnionPosAndScale()//コピーしたバウンド用のドーナツの位置を元々のドーナツの位置に更新
    {
        bounceDonutsParent.transform.position = transform.position;
        bounceDonutsParent.transform.rotation = transform.rotation;
    }
    public void StopDonutsBounce()//ドーナツのバウンドのアニメーションを停止する
    {
        foreach (Transform child in transform)//ドーナツをすべて再表示
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
