using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotScript : MonoBehaviour
{
    [Tooltip("ドーナツを中央へ弾き飛ばす力")]
    [SerializeField] float donutBoundPower = 3f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Donuts")
        {
            //バウンドの方向を計算
            Vector3 boundDirection = Vector3.up * collision.transform.position.y - collision.transform.position;
            boundDirection = boundDirection.normalized;

            //バウンドの力量を与える
            collision.gameObject.GetComponent<DonutRigidBody>().bounce = boundDirection * donutBoundPower;
        }
    }
}
