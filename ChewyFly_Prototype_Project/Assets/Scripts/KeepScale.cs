using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepScale : MonoBehaviour
{
    Vector3 initialScale;
    [SerializeField] Transform parentTransform;
    Vector3 initalParentScale;

    // Start is called before the first frame update
    void Start()
    {
        initialScale = transform.lossyScale;
        //initialScale = Vector3.one;
        initalParentScale = parentTransform.lossyScale;
    }

    // Update is called once per frame
    void Update()
    {
        var parentScale = parentTransform.lossyScale;
        transform.localScale = new Vector3(
            1/initialScale.x / parentScale.x * initalParentScale.x,
            1/initialScale.y / parentScale.y * initalParentScale.y,
            1/initialScale.z / parentScale.z * initalParentScale.z);
        
    }
}
