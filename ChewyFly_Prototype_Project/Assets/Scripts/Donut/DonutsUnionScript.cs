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

        UpdateBounceAnim();//バウンドのアニメーションを更新
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
    private void OnTriggerStay(Collider other)
    {
        if(other.name == "Oil" && !IsComplete) //油に浸かっているかつまだ完成していない時
        {
            if (GetComponent<DonutRigidBody>().IsFreeze) return;    //固定されているなら焼き色は進めない

            int bakedValue = 0;
            foreach(var sphere in donutSpheres) //焼き色を変える
            {
                var sphereColor = sphere.GetComponent<DonutSphereColor>();
                sphereColor.BakeDonut(donutSpheres.Count);
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
    public Vector3 GetDonutsCenterPoint()//ドーナツの中心座標を返す
    {
        Vector3 center = Vector3.zero;
        foreach(var sphere in donutSpheres)
        {
            center += sphere.transform.position;
        }
        return center /= donutSpheres.Count;
    }
    public void StopAllBurntEffect()//すべてのBurntEffectを停止する
    {
        foreach (var sphere in donutSpheres)
        {
            DonutSphereColor donutColor = sphere.GetComponent<DonutSphereColor>();
            donutColor.StopBurntEffect();
        }
    }
    public int[] GetBurntDonutsNum()//それぞれの焦げ色のドーナツの数
    {
        int[] burntDonutsNum = new int[(int)DonutBakedState.Burnt + 1];
        foreach (var sphere in donutSpheres)
        {
            int index = sphere.GetComponent<DonutSphereColor>().BakedNum;
            burntDonutsNum[index]++;
        }
        return burntDonutsNum;
    }
}
