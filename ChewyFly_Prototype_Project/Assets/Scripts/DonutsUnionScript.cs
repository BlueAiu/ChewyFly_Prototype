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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddSphere(GameObject obj)
    {
        donutSpheres.Add(obj);
    }
}
