using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScript : MonoBehaviour
{
    [Tooltip("小さな泡エフェクト")]
    [SerializeField] GameObject smallBubble;
    [Tooltip("浮かび上がるまでの時間")]
    [SerializeField] float floatUpTime = 2f;
    [Tooltip("油面のy座標")]
    [SerializeField] float surfaceY = 1f;
    [Tooltip("浮力(浮かび上がる速さ)")]
    [SerializeField] float buoyancy = 1f;
    [Tooltip("触れたドーナツを弾き飛ばす力")]
    [SerializeField] float donutBoundPower = 5f;

    new Renderer renderer;
    new Rigidbody rigidbody;
    float actionTimer = 0f;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        renderer.enabled = false;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
        actionTimer += Time.deltaTime;

        if(actionTimer > floatUpTime)
        {
            if (!renderer.enabled) renderer.enabled = true;
            if (rigidbody.isKinematic) rigidbody.isKinematic = false;

            //沈んでいる体積を求める
            float depth = surfaceY - transform.position.y;
            float sinkVolume = Mathf.Min(depth + transform.localScale.y / 2, transform.localScale.y);

            //浮力
            rigidbody.AddForce(Vector3.up * (buoyancy * sinkVolume));
        }

        if(transform.position.y > surfaceY)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Donuts")
        {
            //バウンドの方向を計算
            Vector3 boundDirection = collision.transform.position - transform.position;
            boundDirection -= Vector3.up * boundDirection.y;
            boundDirection = boundDirection.normalized;

            //バウンドの力量を与える
            collision.gameObject.GetComponent<DonutRigidBody>().bounce = boundDirection * donutBoundPower;
        }
    }
}
