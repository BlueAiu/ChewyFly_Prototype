using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ObjectReferenceManeger : MonoBehaviour
{
    [Tooltip("�v���C���[")]
    [SerializeField] GameObject player;

    [Tooltip("�Q�[����̃h�[�i�c�B")]
    [SerializeField] List<GameObject> donutsList;

    [Tooltip("��������h�[�i�c�I�u�W�F�N�g")]
    [SerializeField] GameObject donutUnion;

    [Header("��������͈�")]
    [SerializeField] Vector3 spawnMin = Vector3.zero;
    [SerializeField] Vector3 spawnMax = Vector3.zero;

    [Tooltip("�Q�[���J�n����ɐ������鐔")]
    [SerializeField] int startSpawnCount = 10;

    [Tooltip("���Ԃ��Ƃɐ����������")]
    [SerializeField] float spawnTimePeriod = 5f;

    [Tooltip("�Œ���Q�[����ɑ��݂���h�[�i�c�̐�")]
    [SerializeField] int minimumDonutCount = 15;

    //���������h�[�i�c�̐�
    public static int madeDonuts { get; private set; }



    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");

        madeDonuts = 0;

        for(int i = 0; i < startSpawnCount; i++)
        {
            CreateDonutUnion();
        }

        InvokeRepeating(nameof(CreateDonutUnion), spawnTimePeriod, spawnTimePeriod);
    }

    // Update is called once per frame
    void Update()
    {
        if(donutsList.Count < minimumDonutCount)
        {
            CreateDonutUnion();
        }
    }

    Vector3 RandomVector()
    {
        return new Vector3(Random.Range(spawnMin.x,spawnMax.x),
            Random.Range(spawnMin.y,spawnMax.y),
            Random.Range(spawnMin.z,spawnMax.z));
    }

    //�V���ȃh�[�i�c���ɒǉ�����
    public void CreateDonutUnion()
    {
        var position = RandomVector();
        GameObject newUnion = Instantiate(donutUnion, position, Quaternion.identity) as GameObject;
        newUnion.GetComponent<DonutsUnionScript>().objManeger = this;
        donutsList.Add(newUnion);
    }

    //�h�[�i�c��炩�珜�O����
    public void RemoveDonut(GameObject donut)
    {
        if(player.transform.parent == donut.transform)
        {
            player.GetComponent<PlayerController>().DetachDonut();
        }
        donutsList.Remove(donut);
        Destroy(donut);
    }

    //�v���C���[�ƍł������̋߂��h�[�i�c��T��
    public GameObject ClosestDonut()
    {
        GameObject closestDonut = null;
        float closestSqrDistance = float.MaxValue;

        foreach(var donut in donutsList)
        {
            float sqrDistnce = (player.transform.position - donut.transform.position).sqrMagnitude;

            if(sqrDistnce < closestSqrDistance)
            {
                closestDonut = donut;
                closestSqrDistance = sqrDistnce;
            }
        }

        return closestDonut;
    }

    //�h�[�i�c�����������Ƃ��s����
    public void CompleteDonut(GameObject donut)
    {
        donutsList.Remove(donut);

        player.GetComponent<PlayerController>().
            JumpTo(ClosestDonut().transform.position);

        madeDonuts++;

        if (IsIdealDonut(donut))
            Debug.Log("It is ideal donut.");

        donut.GetComponent<DonutRigidBody>().SetMoveMode();
    }

    //�v
    //public void CreateDonutSphere(Vector3 position)
    //{
    //    GameObject newSphere = Instantiate(donutSphere, position, Quaternion.identity) as GameObject;
    //    newSphere.GetComponent<DonutSphereReference>().objManeger = this;
    //}

    //�v
    //public void EntryToUnion(GameObject obj)
    //{
    //    int sphereCount = currentDonutsUnion.AddSphere(obj);
    //    if(sphereCount >= donutNeedSpheres)
    //    {
    //        currentDonutsUnion.CombineDonuts(player.transform.position);
    //    }
    //}
}
