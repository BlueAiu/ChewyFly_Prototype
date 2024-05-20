using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutsUnionScript : MonoBehaviour
{
    [Tooltip("�������ɐ����傩��Q�Ƃ�n�����")]
    public ObjectReferenceManeger objManeger;

    [Tooltip("��̂������Ƃ��L�^���郊�X�g")]
    [SerializeField]
    List<GameObject> donutSpheres = new List<GameObject>();

    [Header("���̎�")]
    [Tooltip("�v���C���[�̓���̍���")]
    [SerializeField] float overHeadHeight = 2f;
    [Tooltip("�h�[�i�c�̔��a")]
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

        //���ƃI�u�W�F�N�g���ړ�������
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
