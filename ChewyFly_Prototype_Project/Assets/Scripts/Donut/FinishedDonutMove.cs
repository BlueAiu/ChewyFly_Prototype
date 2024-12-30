using UnityEngine;

public partial class DonutRigidBody : MonoBehaviour
{
    [Header("完成したときの動き")]
    [Tooltip("ドーナツをカメラに向けて立てるまでの時間")]
    [SerializeField] float phese1Time = 1f;
    [Tooltip("ドーナツを立てる角度")]
    [SerializeField] float standDonutAngle = 90;
    //[Tooltip("上に移動する距離")]
    //[SerializeField] float phese1ShiftY = 2f;
    [Tooltip("ドーナツ完成時ドーナツをどのくらい奥に置くか(-なら手前に)")]
    [SerializeField] float donutShiftToBackLength = 2f;
    [Tooltip("ドーナツ完成時ドーナツをどのくらい左に置くか")]
    [SerializeField] float donutShiftToLeftLength = 2f;

    [Tooltip("だんだん上に加速する時間")]
    [SerializeField] float phese2Time = 1f;
    [Tooltip("上に加速する加速度")]
    [SerializeField] float phese2Acceleration = 1f;

    [Tooltip("完成したドーナツを置いておく場所")]
    [SerializeField] Vector3 storageArea;

    [Header("焦げた時の動き")]
    [Tooltip("ドーナツが沈む質量の倍率")]
    [SerializeField] float sinkMassRate = 2f;
    [Tooltip("沈んで消えるまでの時間")]
    [SerializeField] float sinkTime = 1.5f;

    Transform cameraAxis;
    Collider[] colliders;
    bool isFinishMoving = false;

    int actionPhese = 0;
    float actionTimer = 0f;

    bool isBurnt = false;

    Vector3 previousDonutPos;
    Vector3 newDonutPos;//完成時、phese1でこの位置にドーナツが移動する
    public void SetMoveMode(Vector3 playerPos)
    {
        rb.isKinematic = true;
        isFinishMoving = true;

        colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }

        union.StopAllBurntEffect();
        transform.parent = cameraAxis;
        torque = 0;

        Vector3 playerDir = playerPos - Camera.main.transform.position;
        playerDir.Normalize(); playerDir *= donutShiftToBackLength;
        Vector3 donutPosDiff = transform.position - GetComponent<DonutsUnionScript>().GetDonutsCenterPoint();
        donutPosDiff += -Camera.main.transform.right * donutShiftToLeftLength;
        newDonutPos = playerPos + playerDir + donutPosDiff;
        previousDonutPos = transform.position;
    }

    void FinishedDonutMove()
    {
        if (!isFinishMoving) return;

        actionTimer += Time.deltaTime;
        switch (actionPhese)
        {
            case 0:
                var rotationDir = cameraAxis.TransformDirection(Vector3.left);
                transform.RotateAround(transform.position, rotationDir, standDonutAngle * Time.deltaTime / phese1Time);
                //transform.Translate(Vector3.up * phese1ShiftY * Time.deltaTime / phese1Time);
                transform.position = Vector3.Lerp(previousDonutPos, newDonutPos, actionTimer / phese1Time);

                if (actionTimer > phese1Time)
                {
                    actionTimer = 0f; actionPhese++;
                }
                break;

            case 1:
                transform.position += Vector3.up * (actionTimer * phese2Acceleration);

                if(actionTimer > phese2Time)
                {
                    actionTimer = 0f; actionPhese++;
                }
                break;

            case 2:
                transform.parent = null;
                rb.isKinematic = false;
                isFinishMoving = false;

                foreach(var c in colliders)
                {
                    c.enabled = true;
                    PhysicMaterial physicMaterial = c.material;//弾性を0にする
                    if(physicMaterial != null)
                        physicMaterial.bounciness = 0f;
                }

                DontDestroyOnLoad(gameObject);

                //transform.position = storageArea;
                transform.position = union.objManeger.GetDonutDropPosition(transform.position, union.GetDonutsCenterPoint());

                break;
        }
    }

    public void SetSinkMode()
    {
        if (isBurnt) return;
        isBurnt = true;

        rb.mass *= sinkMassRate;
        Destroy(gameObject, sinkTime);
    }
}
