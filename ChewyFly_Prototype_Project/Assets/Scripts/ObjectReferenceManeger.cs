using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReferenceManeger : MonoBehaviour
{
    [Tooltip("プレイヤー")]
    [SerializeField] GameObject player;

    [Tooltip("完成時に新しいものに変えられる")]
    public DonutsUnionScript currentDonutsUnion;

    [Tooltip("生成するドーナツのもと")]
    [SerializeField] GameObject donutSphere;

    [Tooltip("生成するドーナツ合体オブジェクト")]
    [SerializeField] GameObject donutUnion;

    //[Tooltip("ドーナツに必要なもとの数")]
    //[SerializeField] int donutNeedSpheres = 6;

    [Header("生成する範囲")]
    [SerializeField] Vector3 spawnMin = Vector3.zero;
    [SerializeField] Vector3 spawnMax = Vector3.zero;

    [Tooltip("生成する数")]
    [SerializeField] int spawnCount = 10;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");

        //CreateDonutSphere_Test
        //Vector3 p = new Vector3(1, 2, 1);
        //CreateDonutSphere(p);

        for(int i = 0; i < spawnCount; i++)
        {
            CreateDonutUnion(RandomVector());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 RandomVector()
    {
        return new Vector3(Random.Range(spawnMin.x,spawnMax.x),
            Random.Range(spawnMin.y,spawnMax.y),
            Random.Range(spawnMin.z,spawnMax.z));
    }

    public void CreateDonutUnion(Vector3 position)
    {
        GameObject newUnion = Instantiate(donutUnion, position, Quaternion.identity) as GameObject;
        newUnion.GetComponent<DonutsUnionScript>().objManeger = this;
        //currentDonutsUnion = newUnion.GetComponent<DonutsUnionScript>();
    }

    //没
    //public void CreateDonutSphere(Vector3 position)
    //{
    //    GameObject newSphere = Instantiate(donutSphere, position, Quaternion.identity) as GameObject;
    //    newSphere.GetComponent<DonutSphereReference>().objManeger = this;
    //}

    //没
    //public void EntryToUnion(GameObject obj)
    //{
    //    int sphereCount = currentDonutsUnion.AddSphere(obj);
    //    if(sphereCount >= donutNeedSpheres)
    //    {
    //        currentDonutsUnion.CombineDonuts(player.transform.position);
    //    }
    //}
}
