using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReferenceManeger : MonoBehaviour
{
    [Tooltip("�v���C���[")]
    [SerializeField] GameObject player;

    [Tooltip("�������ɐV�������̂ɕς�����")]
    public DonutsUnionScript currentDonutsUnion;

    [Tooltip("��������h�[�i�c�̂���")]
    [SerializeField] GameObject donutSphere;

    [Tooltip("��������h�[�i�c���̃I�u�W�F�N�g")]
    [SerializeField] GameObject donutUnion;

    [Tooltip("�h�[�i�c�ɕK�v�Ȃ��Ƃ̐�")]
    [SerializeField] int donutNeedSpheres = 6;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");

        //CreateDonutSphere_Test
        //Vector3 p = new Vector3(1, 2, 1);
        //CreateDonutSphere(p);

        CreateDonutUnion();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateDonutUnion()
    {
        GameObject newUnion = Instantiate(donutUnion) as GameObject;
        newUnion.GetComponent<DonutsUnionScript>().objManeger = this;
        currentDonutsUnion = newUnion.GetComponent<DonutsUnionScript>();
    }

    public void CreateDonutSphere(Vector3 position)
    {
        GameObject newSphere = Instantiate(donutSphere, position, Quaternion.identity) as GameObject;
        newSphere.GetComponent<DonutSphereReference>().objManeger = this;
    }

    public void EntryToUnion(GameObject obj)
    {
        int sphereCount = currentDonutsUnion.AddSphere(obj);
        if(sphereCount >= donutNeedSpheres)
        {
            currentDonutsUnion.CombineDonuts(player.transform.position);
        }
    }
}
