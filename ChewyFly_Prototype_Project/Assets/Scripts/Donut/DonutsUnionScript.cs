using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutsUnionScript : MonoBehaviour
{
    [Tooltip("生成時に生成主から参照を渡される")]
    public ObjectReferenceManeger objManeger;

    [Tooltip("子のドーナツを記録するリスト")]
    [SerializeField]
    List<GameObject> donutSpheres = new List<GameObject>();

    //[Header("合体時")]
    //[Tooltip("プレイヤーの頭上の高さ")]
    //[SerializeField] float overHeadHeight = 2f;
    //[Tooltip("ドーナツの半径")]
    //[SerializeField] float donutRadius = 1f;

    Rigidbody rb;
    //くっつけられる状態であるか
    public bool IsSticky { get; set; } = false;
    [Tooltip("ドーナツがくっつく速さ")]
    [SerializeField] public float stickySpeed = 5f;

    //くっついているドーナツの数
    int unionCount = 1;
    [Tooltip("合体の最大数")]
    [SerializeField] int unionCountMax = 6;

    //ドーナツが完成しているか
    public bool IsComplete { get; private set; } = false;

    //ドーナツの質量の増加倍率
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
        //速度が遅いとくっつかない
        if (rb.velocity.sqrMagnitude < stickySpeed * stickySpeed)
        {
            IsSticky = false;
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
        if(collision.gameObject.tag == "Donuts" && IsSticky && unionCount < unionCountMax) 
        {
            int childCount = collision.transform.childCount;

            //相手の子を全てこっちに移す
            for(int i=0;i < childCount;i++)
            {
                var child = collision.transform.GetChild(0);
                child.parent = transform;
                child.localPosition -= new Vector3(0, child.localPosition.y, 0);
                donutSpheres.Add(child.gameObject);
                unionCount++;
            }

            //質量を計算
            rb.mass = 1 + (unionCount - 1) * donutMassRate;
            //衝突相手を消去
            objManeger.RemoveDonut(collision.gameObject);
            //くっつけた直後はくっつかない
            IsSticky = false;

            if(unionCount >= unionCountMax) //ドーナツが完成する
            {
                IsComplete = true;
                rb.velocity = Vector3.zero;
                objManeger.CompleteDonut(this.gameObject);
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
}
