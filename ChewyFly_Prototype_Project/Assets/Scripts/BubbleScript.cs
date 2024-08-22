using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScript : MonoBehaviour
{
    [Tooltip("小さな泡エフェクト")]
    [SerializeField] GameObject smallBubble;
    [Tooltip("弾き判定を出し始める時間")]
    [SerializeField] float bubbleStartTime = 0.5f;
    [Tooltip("弾き判定が滞在する時間")]
    [SerializeField] float lifeTimeMin = 5f;
    [SerializeField] float lifeTimeMax = 10f;
    [Tooltip("ドーナツを弾き飛ばす力")]
    [SerializeField] float donutBoundPower = 5f;

    new Collider collider;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;

        Invoke(nameof(BoundStart), bubbleStartTime);
        Invoke(nameof(Delete), bubbleStartTime + Random.Range(lifeTimeMin, lifeTimeMax));
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    void BoundStart()
    {
        collider.enabled = true;
    }

    void Delete()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Donuts")
        {
            //バウンドの方向を計算
            Vector3 boundDirection = other.transform.position - transform.position;
            boundDirection -= Vector3.up * boundDirection.y;
            boundDirection = boundDirection.normalized;

            //バウンドの力量を与える
            other.transform.parent.gameObject.GetComponent<DonutRigidBody>().bounce = boundDirection * donutBoundPower;
        }
    }
}
