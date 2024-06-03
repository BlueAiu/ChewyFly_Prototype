using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkFixedPosition : MonoBehaviour
{
    Transform parentPos;
    Vector3 localPos;
    Quaternion rota;

    // Start is called before the first frame update
    void Start()
    {
        parentPos = transform.parent;
        localPos = transform.localPosition; 
        rota = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = parentPos.position + localPos;
        transform.position = pos;
        transform.rotation = rota;
    }
}
