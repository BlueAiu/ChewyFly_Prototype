using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutRigidBody : MonoBehaviour
{
    Rigidbody rb;

    [Header("ƒh[ƒiƒc‚Ì•‚—Í")]
    [Tooltip("•‚—ÍŒW”")]
    [SerializeField] float buoyancy = 20f;
    [Tooltip("–û‚Ì•\–Ê‚ÌYÀ•W")]
    [SerializeField] float surfaceY = 0f;

    [Header("–û’†‚Ì’ïR—Í")]
    [Tooltip("’ïR—ÍŒW”")]
    [SerializeField] float resistance = 2f;


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
            //•‚—Í
            float depth = surfaceY - transform.position.y;
            float sinkVolume = Mathf.Min(depth + transform.localScale.y / 2, transform.localScale.y);
            rb.AddForce(Vector3.up * buoyancy * sinkVolume);

            //–€C—Í
            rb.AddForce(-rb.velocity.normalized * rb.velocity.sqrMagnitude * resistance);
        }
    }
}
