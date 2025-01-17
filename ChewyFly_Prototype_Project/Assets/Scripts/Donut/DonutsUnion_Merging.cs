using System.Collections.Generic;
using UnityEngine;

public partial class DonutsUnionScript : MonoBehaviour
{
    const int triConnection = 6;
    const float triRadian = Mathf.PI / 3;
    const int triDegrees = 60;
    const float fullCircleRadian = 2 * Mathf.PI;
    const int fullCircleDegrees = 360;

    [Tooltip("六角座標上でのドーナツの位置")]
    [SerializeField] public List<Vector2> hexaPositions = new List<Vector2>();

    [Tooltip("DonutSphere同士の間の距離")]
    [SerializeField] float sphereDistance = 0.8f;


    readonly Vector3 triangleAxisX = new Vector3 (1, 0, 0);
    readonly Vector3 triangleAxisY = new Vector3 (0.5f, 0, Mathf.Sqrt(3) / 2);

    void MergeDonuts(Collision otherDonutcol)
    {
        var otherDonut = otherDonutcol.transform;
        var hitPoint = otherDonutcol.contacts[0].point;

        var ourHitSphere = ContactSphere(transform, hitPoint);
        var otherHitSphere = ContactSphere(otherDonut, hitPoint);

        var connectDirection = ConnectDonutsDirection(ourHitSphere, otherHitSphere, hitPoint);

        AdjustRotation(otherDonut, connectDirection);

        AdjustPosition(ourHitSphere, otherHitSphere, connectDirection);

        //相手の子を全てこっちに移す
        int childCount = otherDonut.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var child = otherDonut.GetChild(0);
            child.parent = transform;
            child.localPosition -= new Vector3(0, child.localPosition.y, 0);
            donutSpheres.Add(child.gameObject);
            hexaPositions.Add(CloseHexaPosition(child.localPosition));
        }
        //unionCount = donutSpheres.Count;

        //質量を計算
        rb.mass = 1 + (unionCount - 1) * donutMassRate;
        //衝突相手を消去
        objManeger.RemoveDonut(otherDonut.gameObject);
        //くっつけた直後はくっつかない
        IsSticky = false;


        //くっつき衝撃を起こす
        objManeger.MergeImpact(gameObject);
    }

    //衝突地点から最も近いSphereを探す
    Transform ContactSphere(Transform parents, Vector3 hitPoint)
    {
        Transform returnSphere = null;
        float minDistance = float.MaxValue;

        int childCount = parents.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var sphere = parents.GetChild(i);
            if (sphere.tag != "Donuts") continue;
            
            float Distance = (hitPoint - sphere.position).sqrMagnitude;

            if( Distance < minDistance )
            {
                returnSphere = sphere;
                minDistance = Distance;
            }
        }

        return returnSphere;
    }

    //接続の向きを調べる
    Vector3 ConnectDonutsDirection(Transform ourDonut, Transform otherDonut, Vector3 hitPoint)
    {
        var connectDirection = Vector3.zero;

        float minDistance = float.MaxValue;
        for (int i = 0; i < triConnection; i++) 
        {
            //接続先の位置から最も相手のものと近いものを探す
            var connectPoint = ourDonut.localPosition + new Vector3(Mathf.Cos(i * triRadian), 0, Mathf.Sin(i * triRadian)) * sphereDistance;
            if (hexaPositions.Contains(CloseHexaPosition(connectPoint))) continue;

            float distance = (transform.TransformPoint(connectPoint) - otherDonut.position).sqrMagnitude;

            if( distance < minDistance )
            {
                minDistance = distance;

                connectDirection = new Vector3(Mathf.Cos(i * triRadian), 0, Mathf.Sin(i * triRadian)) * sphereDistance;
            }
        }

        return connectDirection;
    }

    //回転を調整する
    void AdjustRotation(Transform otherDonut, Vector3 connectDirection)
    {
        Quaternion closestRotation = Quaternion.identity;
        float minAngleDifferent = float.MaxValue;

        //六角状の角度のうち最も近い角度を探す
        for (float i = 0; i < fullCircleDegrees; i += triDegrees)
        {
            Quaternion connectionRotation = Quaternion.Euler(0, transform.eulerAngles.y + i, 0);
            float angleDifferent = Quaternion.Angle(otherDonut.rotation, connectionRotation);

            if (angleDifferent < minAngleDifferent)
            {
                minAngleDifferent = angleDifferent;
                closestRotation = connectionRotation;
            }
        }

        otherDonut.rotation = closestRotation;
    }

    //位置を調整する
    void AdjustPosition(Transform ourDonut, Transform otherDonut, Vector3 connectDirection)
    {
        Vector3 changePosition = transform.TransformPoint(ourDonut.localPosition + connectDirection) - otherDonut.position;
        otherDonut.parent.position += changePosition;

        //otherDonut.parent.position = transform.TransformPoint(ourDonut.localPosition + connectDirection) - otherDonut.localPosition;
    }

    //六角座標で最も近いものを求める
    Vector2 CloseHexaPosition(Vector3 donutLocalPosition)
    {
        Vector2 hexaPosition = Vector2.zero;

        //クラメルの公式を用いて三角座標を割り出す
        // hexapos.x * triAxisX + hexapos.y * triAxisY = donutLocalPosition
        // hexapos.x * triAxisX.x + hexapos.y * triAxisY.x = donutLocalPosition.x
        // hexapos.x * triAxisX.z + hexapos.y * triAxisY.z = donutLocalPosition.z
        hexaPosition.x = (donutLocalPosition.x * triangleAxisY.z - donutLocalPosition.z * triangleAxisY.x)
            / (triangleAxisX.x * triangleAxisY.z - triangleAxisX.z * triangleAxisY.x);
        hexaPosition.y = (triangleAxisX.x * donutLocalPosition.z - triangleAxisX.z * donutLocalPosition.x)
            / (triangleAxisX.x * triangleAxisY.z - triangleAxisX.z * triangleAxisY.x);

        //sphereDistanceの整数倍になるように四捨五入
        hexaPosition.x = Mathf.Round(hexaPosition.x);
        hexaPosition.y = Mathf.Round(hexaPosition.y);

        return hexaPosition;
    }

    void AdjustDonutPosition()
    {
        for (int i = 0; i < donutSpheres.Count; i++) 
        {
            var donut = donutSpheres[i];
            var pos = hexaPositions[i];

            donut.transform.localPosition = (pos.x * triangleAxisX + pos.y * triangleAxisY) * sphereDistance;
        }

        rb.mass = 1 + (unionCount - 1) * donutMassRate;
    }
}
