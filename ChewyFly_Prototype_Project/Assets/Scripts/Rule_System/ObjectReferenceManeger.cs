using System.Collections.Generic;
using UnityEngine;

public partial class ObjectReferenceManeger : MonoBehaviour
{
    [Tooltip("プレイヤー")]
    [SerializeField] GameObject player;
    PlayerController playerController;

    [Tooltip("油")]
    [SerializeField] Transform oil;
    public static float oilSurfaceY;

    [Tooltip("ゲーム上のドーナツ達")]
    [SerializeField] List<GameObject> donutsList;

    [Tooltip("生成するドーナツオブジェクト")]
    [SerializeField] GameObject donutUnion;


    [Header("ドーナツを生成する")]
    //[SerializeField] Vector3 donutSpawnMin = Vector3.zero;
    //[SerializeField] Vector3 donutSpawnMax = Vector3.zero;
    [SerializeField] float donutSpawnRadius = 5f;
    [SerializeField] float donutSpawnYMin = 1f;
    [SerializeField] float donutSpawnYMax = 1.5f;

    [Tooltip("プレイヤーの周りにドーナツを生成しない距離")]
    [SerializeField] float notSpawnDistance = 1f;

    [Tooltip("ゲーム開始直後に生成する数")]
    [SerializeField] int startSpawnCount = 10;

    [Tooltip("時間ごとに生成する周期")]
    [SerializeField] float spawnTimePeriod = 5f;

    [Tooltip("最低限ゲーム上に存在するドーナツの数")]
    [SerializeField] int minimumDonutCount = 15;


    [Header("ドーナツを弾く泡生成")]
    [Tooltip("生成する泡オブジェクト")]
    [SerializeField] GameObject oilBubble;

    [Tooltip("泡を生成する周期")]
    [SerializeField] float bubbleSpawnPeriod = 3f;

    //[SerializeField] Vector3 bubbleSpawnMin = Vector3.zero;
    //[SerializeField] Vector3 bubbleSpawnMax = Vector3.zero;
    [SerializeField] float bubbleSpawnRadius = 5f;
    [SerializeField] float bubbleSpawnYMin = 0f;
    [SerializeField] float bubbleSpawnYMax = 0.5f;

    //完成したドーナツの数
    public static int madeDonuts { get; private set; }

    [Header("ドーナツがくっついた時")]
    [Tooltip("発生地との距離における衝撃の大きさ")]
    [SerializeField] AnimationCurve impactAttenuation;

    [Header("ドーナツが完成したとき")]
    //[Tooltip("ドーナツが完成したときのジャンプの長さ")]
    //[SerializeField] float completeJumpTime = 3f;
    [Tooltip("完成した時のエフェクト")]
    [SerializeField] GameObject[] completeDonutEffects;

    public static List<GameObject> completeDonuts = new();



    private void Awake()
    {
       ClearCompleteDonuts();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        if (canvas == null)
            canvas = GameObject.Find("Canvas");

        oilSurfaceY = oil.position.y + oil.localScale.y;

        madeDonuts = 0;
        totalScore = 0;

        for(int i = 0; i < startSpawnCount; i++)
        {
            CreateDonutUnion();
        }

        InvokeRepeating(nameof(CreateDonutUnion), spawnTimePeriod, spawnTimePeriod);
        SetDonutScoreText();

        InvokeRepeating(nameof(CreateBubble), bubbleSpawnPeriod, bubbleSpawnPeriod);
    }

    // Update is called once per frame
    void Update()
    {
        if(donutsList.Count < minimumDonutCount)
        {
            CreateDonutUnion();
        }
    }

    Vector3 RandomVector(Vector3 spawnMin, Vector3 spawnMax)
    {
        return new Vector3(Random.Range(spawnMin.x,spawnMax.x),
            Random.Range(spawnMin.y,spawnMax.y),
            Random.Range(spawnMin.z,spawnMax.z));
    }

    Vector3 RandomVector(float radius, float yMin, float yMax)
    {
        float r = Mathf.Sqrt(Random.value) * radius;
        float theta = Random.Range(0f, 2 * Mathf.PI);

        return new Vector3(r * Mathf.Cos(theta), Random.Range(yMin, yMax), r * Mathf.Sin(theta));
    }

    Vector3 MoveAwayDonut(Vector3 SpawnPos)
    {
        Vector3[] directions = new Vector3[4]
        {
            Vector3.forward,
            Vector3.right,
            Vector3.back,
            Vector3.left
        };

        float minDistance = float.MaxValue;
        Vector3 minDirection = Vector3.zero;

        foreach(var d in directions)
        {
            var dir = d * notSpawnDistance;

            if (Vector3.SqrMagnitude(SpawnPos + dir - player.transform.position) < notSpawnDistance * notSpawnDistance)
                continue;

            float distance = Vector3.SqrMagnitude(SpawnPos + dir);

            if(distance < minDistance)
            {
                minDistance = distance;
                minDirection = dir;
            }
        }

        return SpawnPos + minDirection;
    }

    //新たなドーナツを鍋に追加する
    public void CreateDonutUnion()
    {
        var position = RandomVector(donutSpawnRadius, donutSpawnYMin, donutSpawnYMax);
        position = MoveAwayDonut(position);
        GameObject newUnion = Instantiate(donutUnion, position, Quaternion.identity) as GameObject;
        newUnion.GetComponent<DonutsUnionScript>().objManeger = this;
        donutsList.Add(newUnion);
    }

    //ドーナツを鍋から除外する
    public void RemoveDonut(GameObject donut)
    {
        if (player.transform.parent == donut.transform)
        {
            playerController.DetachDonut();
        }
        donutsList.Remove(donut);
        donut.GetComponent<DonutRigidBody>().SetSinkMode();
    }

    //targetと最も距離の近いドーナツを探す, 
    public Vector3 ClosestDonut(Vector3 target, bool isFleeze = false)
    {
        GameObject closestDonut = null;
        Vector3 closestPosition = Vector3.zero;
        float closestSqrDistance = float.MaxValue;

        foreach(var donut in donutsList)
        {
            for (int i = 0; i < donut.transform.childCount; i++)
            {
                var donutSphere = donut.transform.GetChild(i);

                if (player.transform == donutSphere) continue;  //プレイヤーは除外する
                if (playerController.rideDonutSphere == donutSphere) continue;  //プレイヤーが乗ってるドーナツ球は除外する

                float sqrDistnce = (target - donutSphere.position).sqrMagnitude;

                if (sqrDistnce < closestSqrDistance)
                {
                    closestDonut = donut;
                    closestPosition = donutSphere.position;
                    closestSqrDistance = sqrDistnce;
                }
            }
        }

        if (isFleeze)
            closestDonut.GetComponent<DonutRigidBody>().IsFreeze = true;

        return closestPosition;
    }

    //ドーナツがくっついた時呼び出される
    public void MergeImpact(GameObject donut)
    {
        var originPoint = donut.transform.position;
        foreach(var d in donutsList)
        {
            if(d == donut) continue;

            var direction = d.transform.position - originPoint;
            var distance = direction.magnitude;
            direction.Normalize();
            var atteLength = impactAttenuation.length;
            var lastTime = impactAttenuation.keys[atteLength - 1].time;
            distance *= atteLength / lastTime;
            distance = Mathf.Clamp(distance, 0, atteLength - 1);
            d.GetComponent<DonutRigidBody>().TakeImpulse(direction * impactAttenuation.keys[(int)distance].value, false);
        }
    }

    //ドーナツが完成したとき行われる
    public void CompleteDonut(GameObject donut)
    {
        donutsList.Remove(donut);

        playerController.DetachDonut();
        playerController.CompleteDonutReaction();

        madeDonuts++;

        completeDonuts.Add(donut);

        AddDonutScore(donut);//現在のドーナツの形を評価して加算

        donut.GetComponent<DonutRigidBody>().SetMoveMode(player.transform.position);
    }

    //泡を生成する
    void CreateBubble()
    {
        var position = RandomVector(bubbleSpawnRadius, bubbleSpawnYMin, bubbleSpawnYMax);
        Instantiate(oilBubble, position, Quaternion.identity);
    }
    void CompleteDonutEffect(DonutScoreType type, GameObject _donutParent, Vector3 effectPos)//完成時のエフェクト生成
    {
        GameObject completeEffect;

        completeEffect = Instantiate(completeDonutEffects[(int)type], effectPos, Quaternion.identity);

        completeEffect.transform.parent = _donutParent.transform;//エフェクトがドーナツについていくようにする
        completeEffect.transform.Translate(0, 1, 0);
    }

    public static void ClearCompleteDonuts()
    {
        foreach (var i in completeDonuts)
        {
            Destroy(i);
        }
        completeDonuts.Clear();
    }

    //没
    //public void CreateDonutSphere(Vector3 position)
    //{
    //    GameObject newSphere = Instantiate(donutSphere, position, Quaternion.identity) as GameObject;
    //    newSphere.GetComponent<DonutSphereReference>().objManeger = this;
    //}

    //没
    //public void EntryToUnion(GameObject obj)
    //{
    //    int sphereCount = currentDonutsUnion.AddSphere(obj);
    //    if(sphereCount >= donutNeedSpheres)
    //    {
    //        currentDonutsUnion.CombineDonuts(player.transform.position);
    //    }
    //}
}
