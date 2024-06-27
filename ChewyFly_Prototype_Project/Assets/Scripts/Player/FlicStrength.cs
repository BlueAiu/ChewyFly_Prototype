using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class FlicStrength : MonoBehaviour
{
    PlayerController playerController;
    InputScript input;

    [SerializeField] GameObject arrow;
    [Header("矢印が最大何倍まで伸びるか(1以上)")]
    [SerializeField] float maxArrowLength;

    [SerializeField] float minFlicBorder;

    [SerializeField] float maxScaleFactor;
    Vector3 arrowLocalScale;

    [SerializeField] AnimationCurve flicPowerCurve;
    float lastPowerCurveTime;

    //[SerializeField] float maxFlicTime;
    [SerializeField] float maxFlicPower;

    [SerializeField] float flicUpPower;

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
        arrowLocalScale = arrow.transform.localScale;
        arrowStretchOrigin = arrow.transform.localPosition.z - arrowLocalScale.z / 2;

        Renderer arrowRenderer = arrow.GetComponent<Renderer>();
        if (arrowRenderer != null)
        {
            // バウンディングボックスのサイズを取得
            Vector3 size =  arrowRenderer.bounds.size;
            arrowZSize = size.z;// z軸方向の大きさを取得
        }
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
    }
    
    private void FixedUpdate()
    {
        if (playerController.ridingDonut != null)    //ドーナツに乗っている場合
        {
            var direction = input.isLeftStick();
            direction = playerController.playerCamera.transform.TransformDirection(direction);

            if (direction.sqrMagnitude > minFlicBorder * minFlicBorder)
            {
                arrow.SetActive(true);
                flicTime += Time.deltaTime;
                flicPreviousDirection = direction;
                StretchArrow();
            }
            else
            {
                arrow.SetActive(false);
                if (flicTime > 0)
                {
                    flicTime = Mathf.Clamp(flicTime, 0f, lastPowerCurveTime);
                    float flicPower = flicPowerCurve.Evaluate(flicTime);

                    var dounutRigid = playerController.ridingDonut.GetComponent<DonutRigidBody>();
                    Debug.Log(flicPower * maxFlicPower + "で押された");
                    dounutRigid.TakeImpulse(flicPreviousDirection.normalized * -flicPower * maxFlicPower
                        + Vector3.up * flicUpPower);
                    flicTime = 0f;
                }
            }
        }
    }
    void StretchArrow()
    {
        if (flicTime < lastPowerCurveTime)
        {
            float scaleFactor = 1 + (flicTime / lastPowerCurveTime) * (maxArrowLength - 1);
            // スケールを変更
            Vector3 newScale = arrowLocalScale;
            newScale.z *= scaleFactor;
            arrow.transform.localScale = newScale;

            arrow.transform.localPosition = new Vector3(0, 0, arrowStretchOrigin + arrowZSize / 2 * scaleFactor);
        }
    }
    /*private bool isFlic(Vector3 dir)
    {fixedPoint + arrow.transform.right * arrow.transform.localScale.x / 2
        if (dir.sqrMagnitude > flicBorder * flicBorder) return false;
        if (previousDirection.sqrMagnitude <= flicBorder * flicBorder) return false;

        float deltaMagnitude = previousDirection.magnitude - dir.magnitude;
        float rightInputSpeed = deltaMagnitude / Time.fixedDeltaTime;

        if (rightInputSpeed > flicSpeed) return true;
        else return false;
    }*/
}
