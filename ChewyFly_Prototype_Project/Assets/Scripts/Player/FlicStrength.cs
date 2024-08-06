using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class FlicStrength : MonoBehaviour
{
    PlayerController playerController;
    InputScript input;

    [SerializeField] GameObject arrowSprite;
    [SerializeField] GameObject jumpArrowSprite;
    [SerializeField] Transform arrowParent;
    [Header("矢印が最大何倍まで伸びるか(1以上)")]
    [SerializeField] float maxArrowLength;

    [SerializeField] float minFlicBorder;

    [Header("スティックを倒した時間における加える力")]
    [Tooltip("最後のキーの横軸の値で力が最大までたまるまでの時間が決まります")]
    [SerializeField] AnimationCurve flicPowerCurve;
    float lastPowerCurveTime;

    [Header("最大までためた時の加える力")]
    [SerializeField] float maxFlicPower;

    [Header("はじいたときの上方向への力")]
    [SerializeField] float flicUpPower;

    [Header("弾き入力を検知するスティックの速さ")]
    [SerializeField] float stickSpeed = 40f;

    [Header("次に弾き入力ができるまでの時間")]
    [SerializeField] float flicCoolTime = 0.5f;

    [Header("弾きジャンプのジャンプ力")]
    [SerializeField] float jumpPower = 5f;

    Vector3 arrowLocalScale;
    float flicTime;
    float lastFlicTime;
    Vector3 flicPreviousDirection;
    float arrowStretchOrigin;
    float arrowZSize;

    public bool isJumpMode { get; private set; } = false;

    private void Awake()//Startよりさらに前に格納しておく
    {
        flicTime = 0f;
        lastFlicTime = 0f;
        flicPreviousDirection = Vector3.zero;
        playerController = GetComponent<PlayerController>();
        input = GetComponent<InputScript>();
        arrowLocalScale = arrowSprite.transform.localScale;

        Renderer arrowRenderer = arrowSprite.GetComponent<Renderer>();
        if (arrowRenderer != null)
        {
            // バウンディングボックスのサイズを取得
            Vector3 size =  arrowRenderer.bounds.size;
            arrowZSize = size.z;// z軸方向の大きさを取得
        }
        arrowStretchOrigin = arrowSprite.transform.localPosition.z - arrowZSize / 2;
        Keyframe lastKey = flicPowerCurve.keys[flicPowerCurve.length - 1];
        lastPowerCurveTime = lastKey.time;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (playerController.ridingDonut != null)
        {
            if (input.isAButton())
            {
                isJumpMode = !isJumpMode;
                flicTime = 0;
                arrowSprite.SetActive(false);
                jumpArrowSprite.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerController.ridingDonut != null)    //ドーナツに乗っている場合
        {
            if (isJumpMode)
            {
                FlicJump();
            }
            else
            {
                FlicDonut();
            }
        }
        else
        {
            arrowSprite.SetActive(false);
            jumpArrowSprite.SetActive(false);
            flicTime = 0f;
        }
    }

    void FlicDonut()
    {
        var direction = input.isLeftStick();
        direction = playerController.playerCamera.transform.TransformDirection(direction);

        lastFlicTime += Time.fixedDeltaTime;

        if (IsFlic(direction))  //弾かれる処理
        {
            flicTime = Mathf.Clamp(flicTime, 0f, lastPowerCurveTime);
            float flicPower = flicPowerCurve.Evaluate(flicTime);

            var dounutRigid = playerController.ridingDonut.GetComponent<DonutRigidBody>();
            dounutRigid.TakeImpulse(flicPreviousDirection.normalized * -flicPower * maxFlicPower
                + Vector3.up * flicUpPower);
            flicTime = 0f;
            lastFlicTime = 0f;
        }

        if (direction.sqrMagnitude > minFlicBorder * minFlicBorder && lastFlicTime > flicCoolTime)
        {
            arrowSprite.SetActive(true);
            flicTime += Time.fixedDeltaTime;
            StretchArrow(direction);
        }
        else
        {
            arrowSprite.SetActive(false);
            flicTime = 0f;
        }

        flicPreviousDirection = direction;
    }

    void FlicJump()
    {
        var direction = input.isLeftStick();
        direction = playerController.playerCamera.transform.TransformDirection(direction);

        lastFlicTime += Time.fixedDeltaTime;

        if (IsFlic(direction))  //弾かれる処理
        {
            
            float flicPower = jumpArrowSprite.transform.localScale.z;
            var controller = GetComponent<PlayerController>();

            controller.JumpTo(transform.position + -flicPreviousDirection.normalized * (flicPower * jumpPower));

            flicTime = 0f;
            lastFlicTime = 0f;
        }

        if (direction.sqrMagnitude > minFlicBorder * minFlicBorder && lastFlicTime > flicCoolTime)
        {
            jumpArrowSprite.SetActive(true);
            flicTime += Time.fixedDeltaTime;
            StretchArrow(direction);
        }
        else
        {
            jumpArrowSprite.SetActive(false);
            flicTime = 0f;
        }

        flicPreviousDirection = direction;
    }

    void StretchArrow(Vector3 arrowDir)//矢印を回転、引き延ばす
    {
        Quaternion rotation = Quaternion.LookRotation(-arrowDir);
        arrowParent.rotation = rotation;
        if (flicTime < lastPowerCurveTime)
        {
            float scaleFactor = 1 + (flicTime / lastPowerCurveTime) * (maxArrowLength - 1);
            // スケールを変更
            Vector3 newScale = arrowLocalScale;
            newScale.z *= scaleFactor;
            arrowSprite.transform.localScale = newScale;
            jumpArrowSprite.transform.localScale = newScale;

            arrowSprite.transform.localPosition = new Vector3(0, 0, arrowStretchOrigin + arrowZSize / 2 * scaleFactor);
            jumpArrowSprite.transform.localPosition = new Vector3(0, 0, arrowStretchOrigin + arrowZSize / 2 * scaleFactor);
        }
    }

    private bool IsFlic(Vector3 dir)
    {
        if(flicTime <= 0f) return false;
        if (flicPreviousDirection.sqrMagnitude <= minFlicBorder * minFlicBorder) return false;

        float deltaMagnitude = (dir - flicPreviousDirection).magnitude;
        float rightInputSpeed = deltaMagnitude / Time.fixedDeltaTime;

        if (rightInputSpeed < stickSpeed) return false;
        else return true;
    }
}
