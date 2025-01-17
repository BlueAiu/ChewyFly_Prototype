using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController character;
    Animator animator_Player;
    Animator animator_Stick;
    InputScript input;

    bool isFreeze = false;

    [Tooltip("プレイヤーを映すカメラ")]
    [SerializeField] public GameObject playerCamera { get; private set; }

    [Tooltip("乗っているドーナツ")]
    GameObject _ridingDonut = null;
    [SerializeField] public GameObject ridingDonut
    {
        get
        {
            return _ridingDonut;
        }
        private set
        {
            _ridingDonut = value;
            if (value != null)//バウンドの再生中か調べるとき毎回GetComponentしないようにここで入れておく
                ridingDonutUnion = _ridingDonut.GetComponent<DonutsUnionScript>();
            else
            {
                ridingDonutUnion = null;
                rideDonutSphere = null;
            }
        }
    }
    public DonutsUnionScript ridingDonutUnion { get; private set; } = null;

    public Transform rideDonutSphere { get; private set; }

    [Tooltip("ゲームルールオブジェクト")]
    [SerializeField] ObjectReferenceManeger objManeger;

    //[Header("空中にいるときの移動")]
    //[Tooltip("移動の速さ")]
    //[SerializeField] float speed = 5f;

    public Vector3 velocity { get; set; } = Vector3.zero;

    //float velocityY = 0f;

    [Tooltip("重力加速度")]
    [SerializeField] float gravity = 10f;

    //[Tooltip("ジャンプ力")]
    //[SerializeField] float jumpPower = 5f;
    //[Tooltip("終端速度 (jumpPower以上にすること)")]
    //[SerializeField] float terminalVelocity = 3f;

    [Tooltip("ジャンプの発射角度(0~90)")]
    [SerializeField] float jumpSlopeAngle = 45f;

    [Tooltip("プレイヤーが移動方向に向く速さ")]
    [SerializeField] float playerRotateSpeed = 450f;

    [Tooltip("ドーナツに乗ったときのy座標のずれ")]
    [SerializeField] float aboveDonut = 1f;

    Vector3 previousDirection = Vector3.zero;

    //[Header("ドーナツの上に乗っている場合の弾き操作")]
    //[Tooltip("小さいと判定する境界")]
    //[SerializeField] float flicBorder = 0.1f;
    //[Tooltip("弾いたと判定する早さ")]
    //[SerializeField] float flicSpeed = 5f;
    //[Tooltip("弾く力の強さ")]
    //[SerializeField] float flicPower = 20f;

    [Tooltip("ドーナツを回転させる速さ")]
    [SerializeField] float rotateSpeed = 5f;

    Vector3 donutRidePos = Vector3.zero;
    [Tooltip("ドーナツに乗る位置調整の精度")]
    [SerializeField] float donutRideAccuracy = 0.01f;
    [Tooltip("ドーナツに乗る位置調整の速さ")]
    [SerializeField] float donutRideSpeed = 2f;

    [Header("油に落ちた時")]

    [Tooltip("油に落ちた時のエフェクト")]
    [SerializeField] GameObject damageEffect;

    [Tooltip("油に落ちた時留まる時間の長さ")]
    [SerializeField] float oilSinkTime = 1f;

    [Tooltip("油に落ちた時のジャンプの長さ")]
    [SerializeField] float oilJumpTime = 3f;

    [Header("ドーナツが完成したとき")]
    //[Tooltip("ドーナツが完成したとき留まる時間の長さ")]
    //[SerializeField] float completeReactionTime = 1f;

    [Tooltip("次のドーナツにジャンプする長さ")]
    [SerializeField] float completeJumpTime = 1f;
    [Tooltip("ドーナツ完成時元の位置からどのくらいカメラに近づけるか")]
    [SerializeField] float completePlayerToCameraDistance = 1f;

    [Header("ドーナツ完成時の演出時間")]
    [Tooltip("完成時カメラの方に向く時間")]
    [SerializeField] float completeTime_LookCameraRotate = 0.3f;
    [Tooltip("ポーズをしている時間")]
    [SerializeField] float completeTime_Pose;
    [Tooltip("カメラが元の位置に戻る時間")]
    [SerializeField] float completeTime_Reset;
    float completeReactionTimer = 0f;//ズーム時用のタイマー
    enum CompleteDonutReactionState { None, LookCamera, Pose }
    CompleteDonutReactionState completeReactionState = CompleteDonutReactionState.None;
    Quaternion completeReactionRotateFrom;//プレイヤーをここから
    Quaternion completeReactionRotateTo;//ここまで回転
    Vector3 completeReactionPositionFrom;//プレイヤーをここから
    Vector3 completeReactionPositionTo;//ここまで回転
    [SerializeField] Camera mainCamera;
    PlayerCameraRotation playerCameraRotation;

    const float PlayerToHitPoint_DistanceRatio = 0.5f;
    //ドーナツ完成時、プレイヤーからカメラにレイを飛ばして別のオブジェクトに当たった場合
    //この値の比率の位置にプレイヤーを移動させる
    const string WallLayerName = "Wall";

    Vector3 initialPosition;
    float resetPosHeight;//この高さに落ちたら位置をリセット
    private void Awake()//Startよりさらに前に格納しておく
    {
        character = GetComponent<CharacterController>();
        animator_Player = GetComponent<Animator>();
        animator_Stick = transform.GetChild(1).GetComponent<Animator>();
        input = GetComponent<InputScript>();
        playerCameraRotation = GetComponent<PlayerCameraRotation>();
        if (playerCamera == null)
            playerCamera = GameObject.Find("CameraAxis");
        completeReactionState = CompleteDonutReactionState.None;

        initialPosition = transform.position;
        GameObject resetPosHeightObj = GameObject.Find("ResetHeightPoint");
        if (resetPosHeightObj != null)
        {
            resetPosHeight = resetPosHeightObj.transform.position.y;
            Destroy(resetPosHeightObj);
        }
        else Debug.Log("ResetHeightPointを用意してください");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isFreeze)
        {
            Update_CompleteDonutReaction();
            return;
        }

        var direction = input.isLeftStick();
        direction = playerCamera.transform.TransformDirection(direction);

        UpdateRotation(-direction); //左スティックの反対方向を向く

        if(ridingDonut == null)
        {
            //移動
            //velocityY -= gravity * Time.deltaTime;
            //velocityY = Mathf.Max(velocityY, -terminalVelocity);

            //Vector3 velocity = direction * speed + Vector3.up * velocityY;

            velocity += Vector3.down * (gravity * Time.deltaTime);

            character.Move(velocity * Time.deltaTime);
        }
        else
        {
            if ((transform.localPosition - donutRidePos).sqrMagnitude > donutRideAccuracy)
            {
                var tuningDirection = (ridingDonut.transform.TransformPoint(donutRidePos) - transform.position).normalized;
                character.Move(tuningDirection * (donutRideSpeed * Time.deltaTime));
            }
        }

        //else if(input.isAButton())  //乗ってるドーナツを切り離してジャンプ
        //{
        //    DetachDonut();
        //    velocityY = jumpPower;
        //}
        ResetPosition_Fall();

        animator_Player.SetFloat("VerticalVelocity", velocity.y);
        animator_Stick.SetFloat("VerticalVelocity", velocity.y);
        animator_Player.SetBool("IsRideDonut", ridingDonut != null);
        animator_Stick.SetBool("IsRideDonut",ridingDonut != null);
    }

    private void FixedUpdate()
    {
        if (ridingDonut != null)    //ドーナツに乗っている場合
        {
            
            //var direction = input.isLeftStick();
            //direction = playerCamera.transform.TransformDirection(direction);

            var dounutRigid = ridingDonut.GetComponent<DonutRigidBody>();

            //弾き入力
            //if (isFlic(direction))
            //{
            //    dounutRigid.TakeImpulse(previousDirection.normalized * -flicPower);
            //}
            
            //回転入力
            dounutRigid.SetTorque(RotateInput() * rotateSpeed);

            //previousDirection = direction;
        }
    }

    public void DetachDonut()
    {
        ridingDonut = null;
        transform.parent = null;
    }

    public void AttachDonut(GameObject donut)
    {
        if (donut.GetComponent<DonutsUnionScript>().IsComplete) return;
        ridingDonut = donut;
        transform.parent = donut.transform;
        donut.GetComponent<DonutRigidBody>().IsFreeze = false;
    }

    //渡された方向にプレイヤーの正面を向ける
    //FacingForward
    private void UpdateRotation(Vector3 dir)
    {
        if (dir.sqrMagnitude <= 0) return;  //入力されているなら

        Quaternion from = transform.rotation;
        Quaternion to = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(from, to, playerRotateSpeed * Time.deltaTime);
    }

    //private bool isFlic(Vector3 dir)
    //{
    //    if(dir.sqrMagnitude > flicBorder * flicBorder) return false;
    //    if(previousDirection.sqrMagnitude <= flicBorder * flicBorder) return false;

    //    float deltaMagnitude = previousDirection.magnitude - dir.magnitude;
    //    float rightInputSpeed = deltaMagnitude / Time.fixedDeltaTime;

    //    if(rightInputSpeed > flicSpeed) return true;
    //    else return false;
    //}

    private float RotateInput()
    {
        float rotate = 0;
        if (input.isRightShoulder())
            rotate++;
        if (input.isLeftShoulder())
            rotate--;
        return rotate;
    }

    //指定した発射角度でジャンプする
    //跳んでから着地までの時間は飛距離に比例している
    public void JumpTo(Vector3 target)  
    {
        jumpSlopeAngle = Mathf.Clamp(jumpSlopeAngle, 0, Mathf.PI / 2 * Mathf.Rad2Deg);
        DetachDonut();

        Vector3 direction = target - transform.position;
        var verticalDir = direction.y * Vector3.up;
        var horizontalDir = direction - verticalDir;
        var horizontalDistance = horizontalDir.magnitude;
        var jumpTime = Mathf.Sqrt(horizontalDistance * 2 / gravity * Mathf.Tan(jumpSlopeAngle * Mathf.Deg2Rad));
        var jumpVolume = horizontalDistance / (jumpTime * Mathf.Cos(jumpSlopeAngle * Mathf.Deg2Rad));
        
        velocity = horizontalDir / jumpTime     // = horizontalDir.normalized * jampVolume * Mathf.Cos(jumpSlopeAngle* Mathf.Deg2Rad);
            + Vector3.up * jumpVolume * Mathf.Sin(jumpSlopeAngle * Mathf.Deg2Rad)
            + verticalDir / jumpTime;

        transform.rotation = Quaternion.LookRotation(horizontalDir);
    }

    public void JumpTo(Vector3 target, float time)  //跳んでから着地までの時間を指定する
    {
        DetachDonut();

        var direction = target - transform.position;
        var horizontalVelocity = (direction - Vector3.up * direction.y) / time;
        var verticalVelocity = direction.y / time + gravity * time / 2;

        velocity = horizontalVelocity + verticalVelocity * Vector3.up;

        transform.rotation = Quaternion.LookRotation(horizontalVelocity);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Donuts" && character.isGrounded) //ドーナツに着地
        {
            AttachDonut(hit.transform.parent.gameObject);
            //hit.gameObject.GetComponent<DonutSphereReference>().OnPlayerEnter();

            donutRidePos = hit.transform.localPosition + Vector3.up * aboveDonut;
            rideDonutSphere = hit.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Oil")   //油に着水
        {
            transform.position += Vector3.up * (ObjectReferenceManeger.oilSurfaceY - transform.position.y);
            isFreeze = true;
            Invoke(nameof(OilJump), oilSinkTime);
            animator_Player.SetTrigger("JumpFailtuer");

            Instantiate(damageEffect, transform.position, Quaternion.identity);
            //油に触れた段階でエフェクトが発生するよう変更
            //animator_Stick.SetTrigger("JumpFailtuer");
        }
    }

    void OilJump()
    {
        isFreeze = false;

        var targetPos = objManeger.ClosestDonut(transform.position, isFleeze: true) + new Vector3(0, aboveDonut, 0);
        JumpTo(targetPos, oilJumpTime);

        //Instantiate(damageEffect, transform.position, Quaternion.identity);
    }

    public void CompleteDonutReaction()
    {
        isFreeze = true;
        StartCompleteReaction();
        //Invoke(nameof(CompleteJump), completeReactionTime);
    }

    void CompleteJump()
    {
        isFreeze = false;
        var targetPos = objManeger.ClosestDonut(transform.position, isFleeze: true) + new Vector3(0, aboveDonut, 0);
        JumpTo(targetPos, completeJumpTime);

        animator_Player.SetTrigger("JumpTrigger");
        animator_Stick.SetTrigger("JumpTrigger");
    }
    void StartCompleteReaction()//ドーナツ完成時のリアクション開始
    {
        completeReactionTimer = 0f;
        completeReactionState = CompleteDonutReactionState.LookCamera;
        completeReactionRotateFrom = transform.rotation;
        Vector3 direction = mainCamera.transform.position - transform.position;
        completeReactionRotateTo = Quaternion.LookRotation(direction);

        completeReactionPositionFrom = transform.position;
        direction = direction.normalized;

        Ray ray = new Ray(transform.position, direction);//プレイヤーからカメラまでのレイを準備
        RaycastHit hit;
        int layer = LayerMask.NameToLayer(WallLayerName);
        LayerMask layerMask = 1 << layer;
        if (Physics.Raycast(ray, out hit, completePlayerToCameraDistance, layerMask))//レイが当たったら移動は抑える
        {
            completeReactionPositionTo = transform.position + (hit.point - transform.position) * PlayerToHitPoint_DistanceRatio;
        }
        else
        {
            completeReactionPositionTo = transform.position + direction * completePlayerToCameraDistance;
        }

        playerCameraRotation.StartZoom(transform, completeTime_LookCameraRotate);
    }
    void Update_CompleteDonutReaction()//Updateで呼ばれるドーナツ完成時のリアクション
    {
        if (completeReactionState == CompleteDonutReactionState.None) return;

        switch (completeReactionState)
        {
            case CompleteDonutReactionState.LookCamera:

                //プレイヤーをカメラに向ける
                transform.rotation = Quaternion.Lerp(completeReactionRotateFrom, completeReactionRotateTo,
                    completeReactionTimer / completeTime_LookCameraRotate);
                transform.position = Vector3.Lerp(completeReactionPositionFrom, completeReactionPositionTo,
                    completeReactionTimer / completeTime_LookCameraRotate);

                if (completeReactionTimer + Time.deltaTime > completeTime_LookCameraRotate)//回転し終わったら
                {
                    transform.rotation = completeReactionRotateTo;
                    transform.position = completeReactionPositionTo;
                    //animator_Player.SetTrigger("CompletePose");
                    //animator_Stick.SetTrigger("CompletePose");
                    completeReactionState++;
                    completeReactionTimer = 0f;
                }
                break;
            case CompleteDonutReactionState.Pose:

                if (completeReactionTimer + Time.deltaTime > completeTime_Pose)
                {
                    playerCameraRotation.Zoom_Reset(completeTime_Reset);//カメラを元の位置に戻す

                    CompleteJump();//ジャンプ
                    completeReactionState = CompleteDonutReactionState.None;
                }
                break;
        }
        completeReactionTimer += Time.deltaTime;
    }
    void ResetPosition_Fall()//落ちた時初期位置に戻した後ジャンプする
    {
        if (transform.position.y < resetPosHeight)
        {
            if (ridingDonut == null)
            {
                transform.position = initialPosition;
                CompleteJump();
            }
            else
                ridingDonut.transform.position = initialPosition;
        }
    }
}
