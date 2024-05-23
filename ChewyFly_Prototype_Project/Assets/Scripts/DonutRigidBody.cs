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

    Vector3 impulse = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(impulse != Vector3.zero)
        {
            rb.AddForce(impulse, ForceMode.Impulse);
            impulse = Vector3.zero;
        }
    }

    public void TakeImpulse(Vector3 _impulse)
    {
        impulse = _impulse;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.name == "Oil")
        {
            //’¾‚ñ‚Å‚¢‚é‘ÌÏ‚ğ‹‚ß‚é
            float depth = surfaceY - transform.position.y;
            float sinkVolume = Mathf.Min(depth + transform.localScale.y / 2, transform.localScale.y);

            //•‚—Í
            rb.AddForce(Vector3.up * buoyancy * sinkVolume);

            //–€C—Í
            rb.AddForce(-rb.velocity.normalized * (rb.velocity.sqrMagnitude * resistance * sinkVolume));
        }
    }
}
