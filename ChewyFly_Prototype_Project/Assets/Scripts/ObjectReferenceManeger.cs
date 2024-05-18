using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReferenceManeger : MonoBehaviour
{
    public DonutsUnionScript currentDonutsUnion;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EntryToUnion(GameObject obj)
    {
        currentDonutsUnion.AddSphere(obj);
    }
}
