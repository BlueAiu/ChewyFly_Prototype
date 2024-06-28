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

    Vector3 arrowLocalScale;
    float flicTime;
    Vector3 flicPreviousDirection;
    float arrowStretchOrigin;
    float arrowZSize;

    private void Awake()//Startよりさらに前に格納しておく
    {
        flicTime = 0f;
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

    // Update is called once per frame
    void Update()
    {
        if (playerController.ridingDonut != null)    //ドーナツに乗っている場合
        {
            var direction = input.isLeftStick();
            direction = playerController.playerCamera.transform.TransformDirection(direction);

            if (direction.sqrMagnitude > minFlicBorder * minFlicBorder)
            {
                arrowSprite.SetActive(true);
                flicTime += Time.deltaTime;
                flicPreviousDirection = direction;
                StretchArrow(direction);
            }
            else
            {
                arrowSprite.SetActive(false);
                if (flicTime > 0)
                {
                    flicTime = Mathf.Clamp(flicTime, 0f, lastPowerCurveTime);
                    float flicPower = flicPowerCurve.Evaluate(flicTime);

                    var dounutRigid = playerController.ridingDonut.GetComponent<DonutRigidBody>();
                    dounutRigid.TakeImpulse(flicPreviousDirection.normalized * -flicPower * maxFlicPower
                        + Vector3.up * flicUpPower);
                    flicTime = 0f;
                }
            }
        }
        else
        {
            arrowSprite.SetActive(false);
            flicTime = 0f;
        }
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

            arrowSprite.transform.localPosition = new Vector3(0, 0, arrowStretchOrigin + arrowZSize / 2 * scaleFactor);
        }
    }
}
