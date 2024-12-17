using System.Collections.Generic;
using UnityEngine;

public partial class DonutsUnionScript : MonoBehaviour
{
    const int triConnection = 6;
    const float triRadian = Mathf.PI / 3;
    const int triDegrees = 60;
    const float fullCircleRadian = 2 * Mathf.PI;
    const int fullCircleDegrees = 360;

    [Tooltip("˜ZŠpÀ•Wã‚Å‚Ìƒh[ƒiƒc‚ÌˆÊ’u")]
    [SerializeField] public List<Vector2> hexaPositions = new List<Vector2>();

    [Tooltip("DonutSphere“¯m‚ÌŠÔ‚Ì‹——£")]
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

        //‘Šè‚Ìq‚ğ‘S‚Ä‚±‚Á‚¿‚ÉˆÚ‚·
        int childCount = otherDonut.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var child = otherDonut.GetChild(0);
            child.parent = transform;
            child.localPosition -= new Vector3(0, child.localPosition.y, 0);
            donutSpheres.Add(child.gameObject);
            hexaPositions.Add(CloseHexaPosition(child.localPosition));
            unionCount++;
        }

        //¿—Ê‚ğŒvZ
        rb.mass = 1 + (unionCount - 1) * donutMassRate;
        //Õ“Ë‘Šè‚ğÁ‹
        objManeger.RemoveDonut(otherDonut.gameObject);
        //‚­‚Á‚Â‚¯‚½’¼Œã‚Í‚­‚Á‚Â‚©‚È‚¢
        IsSticky = false;


        //‚­‚Á‚Â‚«ÕŒ‚‚ğ‹N‚±‚·
        objManeger.MergeImpact(gameObject);
    }

    //Õ“Ë’n“_‚©‚çÅ‚à‹ß‚¢Sphere‚ğ’T‚·
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

    //Ú‘±‚ÌŒü‚«‚ğ’²‚×‚é
    Vector3 ConnectDonutsDirection(Transform ourDonut, Transform otherDonut, Vector3 hitPoint)
    {
        var connectDirection = Vector3.zero;

        float minDistance = float.MaxValue;
        for (int i = 0; i < triConnection; i++) 
        {
            //Ú‘±æ‚ÌˆÊ’u‚©‚çÅ‚à‘Šè‚Ì‚à‚Ì‚Æ‹ß‚¢‚à‚Ì‚ğ’T‚·
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

    //‰ñ“]‚ğ’²®‚·‚é
    void AdjustRotation(Transform otherDonut, Vector3 connectDirection)
    {
        Quaternion closestRotation = Quaternion.identity;
        float minAngleDifferent = float.MaxValue;

        //˜ZŠpó‚ÌŠp“x‚Ì‚¤‚¿Å‚à‹ß‚¢Šp“x‚ğ’T‚·
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

    //ˆÊ’u‚ğ’²®‚·‚é
    void AdjustPosition(Transform ourDonut, Transform otherDonut, Vector3 connectDirection)
    {
        Vector3 changePosition = transform.TransformPoint(ourDonut.localPosition + connectDirection) - otherDonut.position;
        otherDonut.parent.position += changePosition;

        //otherDonut.parent.position = transform.TransformPoint(ourDonut.localPosition + connectDirection) - otherDonut.localPosition;
    }

    //˜ZŠpÀ•W‚ÅÅ‚à‹ß‚¢‚à‚Ì‚ğ‹‚ß‚é
    Vector2 CloseHexaPosition(Vector3 donutLocalPosition)
    {
        Vector2 hexaPosition = Vector2.zero;

        //ƒNƒ‰ƒƒ‹‚ÌŒö®‚ğ—p‚¢‚ÄOŠpÀ•W‚ğŠ„‚èo‚·
        // hexapos.x * triAxisX + hexapos.y * triAxisY = donutLocalPosition
        // hexapos.x * triAxisX.x + hexapos.y * triAxisY.x = donutLocalPosition.x
        // hexapos.x * triAxisX.z + hexapos.y * triAxisY.z = donutLocalPosition.z
        hexaPosition.x = (donutLocalPosition.x * triangleAxisY.z - donutLocalPosition.z * triangleAxisY.x)
            / (triangleAxisX.x * triangleAxisY.z - triangleAxisX.z * triangleAxisY.x);
        hexaPosition.y = (triangleAxisX.x * donutLocalPosition.z - triangleAxisX.z * donutLocalPosition.x)
            / (triangleAxisX.x * triangleAxisY.z - triangleAxisX.z * triangleAxisY.x);

        //sphereDistance‚Ì®””{‚É‚È‚é‚æ‚¤‚ÉlÌŒÜ“ü
        hexaPosition.x = Mathf.Round(hexaPosition.x);
        hexaPosition.y = Mathf.Round(hexaPosition.y);

        return hexaPosition;
    }
}
