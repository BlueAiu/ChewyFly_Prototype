using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpLandingPoint : MonoBehaviour
{
    public bool IsOnDonut { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(IsOnDonut);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Donuts"))
        {
            IsOnDonut = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Donuts"))
        {
            IsOnDonut = false;
        }
    }
}
