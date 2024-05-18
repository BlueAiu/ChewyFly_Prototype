using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReferenceManeger : MonoBehaviour
{
    [Tooltip("�������ɐV�������̂ɕς�����")]
    public DonutsUnionScript currentDonutsUnion;

    [Tooltip("��������h�[�i�c�̂���")]
    [SerializeField] GameObject donutSphere;

    // Start is called before the first frame update
    void Start()
    {
        //CreateDonutSphere_Test
        //Vector3 p = new Vector3(1, 2, 1);
        //CreateDonutSphere(p);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateDonutSphere(Vector3 position)
    {
        GameObject newSphere = Instantiate(donutSphere, position, Quaternion.identity) as GameObject;
        newSphere.GetComponent<DonutSphereReference>().objManeger = this;
    }

    public void EntryToUnion(GameObject obj)
    {
        currentDonutsUnion.AddSphere(obj);
    }
}
