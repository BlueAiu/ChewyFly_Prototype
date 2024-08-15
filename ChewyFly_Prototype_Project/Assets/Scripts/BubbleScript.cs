using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScript : MonoBehaviour
{
    [Tooltip("小さな泡エフェクト")]
    [SerializeField] GameObject smallBubble;
    [Tooltip("滞在する時間")]
    [SerializeField] float lifeTimeMin = 5f;
    [SerializeField] float lifeTimeMax = 10f;
    [Tooltip("ドーナツを弾き飛ばす力")]
    [SerializeField] float donutBoundPower = 5f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(Delete), Random.Range(lifeTimeMin, lifeTimeMax));
    }

    // Update is called once per frame
    void Update()
    {
        ;
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
