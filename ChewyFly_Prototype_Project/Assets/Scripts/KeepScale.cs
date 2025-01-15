using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepScale : MonoBehaviour
{
    Vector3 initialScale;

    // Start is called before the first frame update
    void Start()
    {
        initialScale = transform.lossyScale;
    }

    // Update is called once per frame
    void Update()
    {
        var parentScale = transform.parent.lossyScale;
        transform.localScale = new Vector3(
            initialScale.x/parentScale.x,
            initialScale.y/parentScale.y,
            initialScale.z/parentScale.z);
    }
}
