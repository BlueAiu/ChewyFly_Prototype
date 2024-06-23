using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class DonutsUnionScript : MonoBehaviour
{
    const int triConnection = 6;
    const float triRadian = Mathf.PI / 3;
    const int triDegrees = 60;
    const float fullCircleRadian = 2 * Mathf.PI;
    const int fullCircleDegrees = 360;

    [Tooltip("DonutSphere���m�̊Ԃ̋���")]
    [SerializeField] float sphereDistance = 0.8f;

    //readonly Vector3 triangleAxisX = new Vector3 (1, 0, 0);
    //readonly Vector3 triangleAxisY = new Vector3 (0.5f, 0, Mathf.Sqrt(3) / 2);

    void MergeDonuts(Collision otherDonutcol)
    {
        var otherDonut = otherDonutcol.transform;
        var hitPoint = otherDonutcol.contacts[0].point;
        Vector3 connectDirection = Vector3.zero;

        var ourHitSphere = ContactSphere(transform, hitPoint);
        var otherHitSphere = ContactSphere(otherDonut, hitPoint);

        ConnectHitDonuts(ourHitSphere, otherHitSphere, hitPoint, out connectDirection);

        AdjustRotation(otherDonut, connectDirection);

        AdjustPosition(ourHitSphere, otherHitSphere, connectDirection);

        //����̎q��S�Ă������Ɉڂ�
        int childCount = otherDonut.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var child = otherDonut.GetChild(0);
            child.parent = transform;
            child.localPosition -= new Vector3(0, child.localPosition.y, 0);
            unionCount++;
        }
        foreach(var sphere in otherDonut.GetComponent<DonutsUnionScript>().donutSpheres)
        {
            donutSpheres.Add(sphere.Key, sphere.Value);
        }

        //���ʂ��v�Z
        rb.mass = 1 + (unionCount - 1) * donutMassRate;
        //�Փˑ��������
        objManeger.RemoveDonut(otherDonut.gameObject);
        //������������͂������Ȃ�
        IsSticky = false;
    }

    //�Փ˒n�_����ł��߂�Sphere��T��
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

    void ConnectHitDonuts(Transform ourDonut, Transform otherDonut, Vector3 hitPoint, out Vector3 connectDirection)
    {
        connectDirection = Vector3.zero;
        //�����̃h�[�i�c�̐ڑ��ɑ���̂�o�^
        int connectNum = 0;
        float minDistance = float.MaxValue;
        for (int i = 0; i < triConnection; i++) 
        {
            if (donutSpheres[ourDonut.gameObject][i] != null) continue;

            var connectPoint = ourDonut.localPosition + new Vector3(Mathf.Cos(i * triRadian), 0, Mathf.Sin(i * triRadian)) * sphereDistance;
            float distance = (transform.TransformPoint(connectPoint) - otherDonut.position).sqrMagnitude;

            if( distance < minDistance )
            {
                connectNum = i;
                minDistance = distance;

                connectDirection = new Vector3(Mathf.Cos(i * triRadian), 0, Mathf.Sin(i * triRadian)) * sphereDistance;
            }
        }
        donutSpheres[ourDonut.gameObject][connectNum] = otherDonut;

        //����̃h�[�i�c�̐ڑ��Ɏ����̂�o�^
        var otherDonutSpheres = otherDonut.parent.GetComponent<DonutsUnionScript>().donutSpheres;
        minDistance = float.MaxValue;
        for (int i = 0;i < triConnection; i++)
        {
            if (otherDonutSpheres[otherDonut.gameObject][i] != null) continue;

            var connectPoint = otherDonut.localPosition + new Vector3(Mathf.Cos(i * triRadian), 0, Mathf.Sin(i * triRadian)) * sphereDistance;
            float distance = (otherDonut.parent.TransformPoint(connectPoint) - ourDonut.position).sqrMagnitude;

            if(distance < minDistance)
            {
                connectNum = i;
                minDistance = distance;
            }
        }
        otherDonutSpheres[otherDonut.gameObject][connectNum] = ourDonut;
    }

    //��]�𒲐�����
    void AdjustRotation(Transform otherDonut, Vector3 connectDirection)
    {
        Quaternion closestRotation = Quaternion.identity;
        float minAngleDifferent = float.MaxValue;

        //�Z�p��̊p�x�̂����ł��߂��p�x��T��
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

    //�ʒu�𒲐�����
    void AdjustPosition(Transform ourDonut, Transform otherDonut, Vector3 connectDirection)
    {
        Vector3 changePosition = transform.TransformPoint(ourDonut.localPosition + connectDirection) - otherDonut.position;
        otherDonut.parent.position += changePosition;

        //otherDonut.parent.position = transform.TransformPoint(ourDonut.localPosition + connectDirection) - otherDonut.localPosition;
    }
}
