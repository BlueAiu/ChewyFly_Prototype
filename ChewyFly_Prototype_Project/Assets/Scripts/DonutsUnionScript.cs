using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutsUnionScript : MonoBehaviour
{
    [Tooltip("生成時に生成主から参照を渡される")]
    public ObjectReferenceManeger objManeger;

    [Tooltip("印のついたもとを記録するリスト")]
    [SerializeField]
    List<GameObject> donutSpheres = new List<GameObject>();

    [Header("合体時")]
    [Tooltip("プレイヤーの頭上の高さ")]
    [SerializeField] float overHeadHeight = 2f;
    [Tooltip("ドーナツの半径")]
    [SerializeField] float donutRadius = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int AddSphere(GameObject obj)
    {
        donutSpheres.Add(obj);
        return donutSpheres.Count;
    }

    public void CombineDonuts(Vector3 position)
    {
        transform.position = position + Vector3.up * overHeadHeight;

        foreach(var i in donutSpheres)
        {
            i.GetComponent<Rigidbody>().isKinematic = true;
            i.transform.parent = this.transform;
            i.GetComponent<DonutSphereReference>().DisableMark();
        }

        //もとオブジェクトを移動させる
        for (int i = 0; i < donutSpheres.Count; i++)
        {
            Vector3 lineUpPos = Vector3.zero;
            lineUpPos.x = Mathf.Cos(i * Mathf.PI / (donutSpheres.Count / 2));
            lineUpPos.z = Mathf.Sin(i * Mathf.PI / (donutSpheres.Count / 2));
            lineUpPos *= donutRadius;

            donutSpheres[i].transform.position = transform.position +  lineUpPos;
        }
    }
}
