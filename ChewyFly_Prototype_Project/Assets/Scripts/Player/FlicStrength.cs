using System;
using UnityEngine;

public class FlicStrength : MonoBehaviour
{
    PlayerController playerController;
    Animator animator;
    InputScript input;
    [SerializeField] ObjectReferenceManeger objManeger;

    [SerializeField] GameObject flicArrowSprite;
    [SerializeField] GameObject jumpArrowSprite;
    [SerializeField] Transform arrowParent;

    [Tooltip("矢印が最大何倍まで伸びるか(1以上)")]
    [SerializeField] float maxArrowLength;

    [SerializeField] float minFlicBorder;

    [Header("スティックを倒した時間における加える力")]
    [Tooltip("最後のキーの横軸の値で力が最大までたまるまでの時間が決まります")]
    [SerializeField] AnimationCurve flicPowerCurve;
    float lastPowerCurveTime;

    [Tooltip("最大までためた時の加える力")]
    [SerializeField] float maxFlicDonutPower;

    [Tooltip("はじいたときの上方向への力")]
    [SerializeField] float flicUpPower;

    [Tooltip("弾き入力を検知するスティックの速さ")]
    [SerializeField] float stickSpeed = 40f;
    [SerializeField] float maxStickSpeed = 45f;
    [SerializeField] float minStickSpeed = 0f;

    [Tooltip("次に弾き入力ができるまでの時間")]
    [SerializeField] float flicCoolTime = 0.5f;

    [Tooltip("弾きジャンプのジャンプ力")]
    [SerializeField] float jumpPower = 5f;

    [Tooltip("ジャンプ着地地点")]
    [SerializeField] Transform jumpPoint;

    [Tooltip("ジャンプ調整の半径")]
    [SerializeField] float jumpAdjustRadius = 0.7f;

    Vector3 arrowLocalScale;
    float flicTime;
    float lastFlicTime;
    float flicPower;
    [Tooltip("引く力の変化の周期")]
    [SerializeField] float flicPowerPeriod = 2f;

    float FlicTime
    {
        get
        {
            return flicTime; ;
        }
        set
        {
            flicTime = value;
            flicPower = (1 - (float)Math.Cos(flicTime * Mathf.PI / flicPowerPeriod)) * lastPowerCurveTime;
            ;
        }
    }


    Vector3 flicPreviousDirection;
    float arrowStretchOrigin;
    float arrowZSize;

    public bool isJumpMode { get; private set; } = false;


    private void Awake()//Startよりさらに前に格納しておく
    {
        FlicTime = 0f;
        lastFlicTime = 0f;
        flicPreviousDirection = Vector3.zero;
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        input = GetComponent<InputScript>();
        arrowLocalScale = flicArrowSprite.transform.localScale;

        Renderer arrowRenderer = flicArrowSprite.GetComponent<Renderer>();
        if (arrowRenderer != null)
        {
            // バウンディングボックスのサイズを取得
            Vector3 size =  arrowRenderer.bounds.size;
            arrowZSize = size.z;// z軸方向の大きさを取得
        }
        arrowStretchOrigin = flicArrowSprite.transform.localPosition.z - arrowZSize / 2;
        Keyframe lastKey = flicPowerCurve.keys[flicPowerCurve.length - 1];
        lastPowerCurveTime = lastKey.time;
    }

    // Start is called before the first frame update
    void Start()
    {
        OptionValues option = FindObjectOfType<OptionValues>();//感度を初期化
        SetJumpSensityvity(option);
        StopArrowSprites();
    }

    private void Update()
    {
        if (input.isAButton())
        {
            isJumpMode = !isJumpMode;
            FlicTime = 0;
            StopArrowSprites();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerController.ridingDonut != null && !playerController.ridingDonutUnion.isBouncing)    //ドーナツに乗っていてドーナツがバウンド中でない場合
        {
            var direction = input.isLeftStick();
            direction = playerController.playerCamera.transform.TransformDirection(direction);

            lastFlicTime += Time.fixedDeltaTime;

            if (isJumpMode)
            {
                FlicJump(direction);
            }
            else
            {
                FlicDonut(direction);
            }

            if (direction.sqrMagnitude > minFlicBorder * minFlicBorder && lastFlicTime > flicCoolTime)
            {
                FlicTime += Time.fixedDeltaTime;
                StretchArrow(direction);
            }
            else
            {
                StopArrowSprites();
                FlicTime = 0f;
            }

            flicPreviousDirection = direction;
        }
        else
        {
            StopArrowSprites();
            FlicTime = 0f;
        }
    }

    void FlicDonut(Vector3 direction)
    {
        if (playerController.ridingDonutUnion != null && playerController.ridingDonutUnion.isBouncing) return;//バウンド中なら受け付けない

        if (IsFlic(direction))  //弾かれる処理
        {
            transform.rotation = Quaternion.LookRotation(-flicPreviousDirection);

            //flicTime = Mathf.Clamp(flicTime, 0f, lastPowerCurveTime);
            float flicDonutPower = flicPowerCurve.Evaluate(flicPower);
            var impulseValue = flicPreviousDirection.normalized * -flicDonutPower * maxFlicDonutPower
                + Vector3.up * flicUpPower;
            
            var dounutRigid = playerController.ridingDonut.GetComponent<DonutRigidBody>();
            dounutRigid.TakeImpulse(impulseValue);

            FlicTime = 0f;
            lastFlicTime = 0f;
        }
    }

    void FlicJump(Vector3 direction)
    {
        if (IsFlic(direction))  //弾かれる処理
        {
            float flicJumpPower = jumpArrowSprite.transform.localScale.z;

            var jumpDirection = -flicPreviousDirection.normalized;

            jumpPoint.position = transform.position + jumpDirection * (flicJumpPower * jumpPower);
            var closeDonut = objManeger.ClosestDonut(jumpPoint.position);
            var jumpPointSqrDistance = (jumpPoint.position - closeDonut).sqrMagnitude;
            if (jumpAdjustRadius * jumpAdjustRadius > jumpPointSqrDistance)
            {
                jumpPoint.position = closeDonut;
            }

            playerController.JumpTo(jumpPoint.position);
            jumpPoint.LookAt(jumpPoint.position + playerController.velocity - Vector3.up * playerController.velocity.y * 2);

            FlicTime = 0f;
            lastFlicTime = 0f;

            animator.SetTrigger("JumpTrigger");
        }
    }



    void StretchArrow(Vector3 arrowDir)//矢印を回転、引き延ばす
    {
        if (isJumpMode)
        {
            jumpArrowSprite.SetActive(true);
        }
        else
        {
            flicArrowSprite.SetActive(true);
        }


        Quaternion rotation = Quaternion.LookRotation(-arrowDir);
        arrowParent.rotation = rotation;

        float scaleFactor = 1 + (flicPower / lastPowerCurveTime) * (maxArrowLength - 1);
        // スケールを変更
        Vector3 newScale = arrowLocalScale;
        newScale.z *= scaleFactor;
        flicArrowSprite.transform.localScale = newScale;
        jumpArrowSprite.transform.localScale = newScale;

        flicArrowSprite.transform.localPosition = new Vector3(0, 0, arrowStretchOrigin + arrowZSize / 2 * scaleFactor);
        jumpArrowSprite.transform.localPosition = new Vector3(0, 0, arrowStretchOrigin + arrowZSize / 2 * scaleFactor);
    }

    private bool IsFlic(Vector3 dir)
    {
        if(FlicTime <= 0f) return false;
        if (flicPreviousDirection.sqrMagnitude <= minFlicBorder * minFlicBorder) return false;

        float deltaMagnitude = (dir - flicPreviousDirection).magnitude;
        float rightInputSpeed = deltaMagnitude / Time.fixedDeltaTime;

        if (rightInputSpeed < stickSpeed) return false;
        else return true;
    }
    void SetJumpSensityvity(OptionValues optionValues)
    {
        if (optionValues == null) return;

        stickSpeed = minStickSpeed + (maxStickSpeed - minStickSpeed) * optionValues.GetJumpSensitivityRatio();
    }
    void StopArrowSprites()
    {
        flicArrowSprite.SetActive(false);
        jumpArrowSprite.SetActive(false);
    }
}
