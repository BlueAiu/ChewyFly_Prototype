using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutRigdBody : MonoBehaviour
{
    Rigidbody rb;

    [Header("ドーナツの浮力")]
    [Tooltip("浮力係数")]
    [SerializeField] float buoyancy = 1000f;
    [Tooltip("油の表面のY座標")]
    [SerializeField] float surfaceY = 0f;

    [Header("ドーナツと油の摩擦力")]
    [Tooltip("摩擦力係数")]
    [SerializeField] float friction = 100f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.name == "Oil")
        {
            //浮力
            float depth = surfaceY - transform.position.y;
            float sinkVolume = Mathf.Min(depth + transform.localScale.y / 2, transform.localScale.y);
            rb.AddForce(Vector3.up * buoyancy * sinkVolume * Time.deltaTime);

            //摩擦力
            rb.AddForce(-rb.velocity * friction * Time.deltaTime);
        }
    }
}
