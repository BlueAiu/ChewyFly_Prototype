using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraRotation : MonoBehaviour
{
    protected InputScript input;

    [Tooltip("プレイヤーを映すカメラの軸")]
    [SerializeField] protected GameObject playerCameraAxis;

    [Tooltip("カメラの回転の速さ")]
    [SerializeField] float sensitivity = 180f;
    [Tooltip("カメラの最大回転速度")]
    [SerializeField] float maxSensitivity = 330f;
    [Tooltip("カメラの最小回転速度")]
    [SerializeField] float minSensitivity = 30f;


    [Tooltip("視点リセット時のカメラの回転の速さ")]
    [SerializeField] float resetSensitivity = 800f;
    bool isResetCamera = false;


    [Header("自動カメラ回転")]
    [Tooltip("中心からこの距離より内側だと自動回転しない")]
    [SerializeField] float nonRotationalAreaRadius = 3f;
    [Tooltip("カメラ回転操作をした後この距離を離れるまで自動回転しない")]
    [SerializeField] float reapplicationDistance = 1f;
    [Tooltip("自動回転の回転速")]
    [SerializeField] float autoRotateSensitivity = 120f;

    bool canRotatoAuto = false;
    Vector3 lastInputPosition = Vector3.zero;

    [Tooltip("鍋の中心位置(カメラの位置を決めるため)")]
    [SerializeField] Transform potCenterPoint;
    [Tooltip("カメラが向く方向の場所")]
    [SerializeField] Transform cameraLookPoint;
    [Tooltip("鍋の中心とプレイヤーの間のどのくらいの比率の位置にカメラを向けるか")]
    [SerializeField] float cameraLookPointRatio = 0.5f;

    [Header("ズーム時のカメラに関連する変数")]
    [SerializeField] GameObject virtualCamera;
    bool isZoom = false;//ズームしているか
    bool isResetZoom = false;//ズーム状態から元に戻っているか
    CinemachineVirtualCamera cinemachineVirtualCamera;
    float zoomTime;
    float zoomTimer;
    float verticalFOV_default;//最初のFOV
    Transform lookAtZoomPoint;//カメラが参照するポイント
    Transform lookAtZoomPointRef;//ズームした後のポイント
    Transform defaultLookAtTransform;
    [SerializeField] AnimationCurve zoomCameraMoveCurve;
    Vector3 zoomShift;
    [Tooltip("ズーム後のFOV")]
    [SerializeField] float zoomFOV = 10f;

    private void Awake()//Startよりさらに前に格納しておく
    {
        input = GetComponent<InputScript>();
        if (playerCameraAxis == null)
            playerCameraAxis = GameObject.Find("CameraAxis");
        if(virtualCamera == null)
            virtualCamera = GameObject.Find("VirtualCamera");
        cinemachineVirtualCamera = virtualCamera.GetComponent<CinemachineVirtualCamera>();
        verticalFOV_default = cinemachineVirtualCamera.m_Lens.FieldOfView;
        Transform trans = cinemachineVirtualCamera.m_LookAt.transform;
        defaultLookAtTransform = trans;
        isZoom = false;
        isResetZoom = false;

        GameObject newObject = new GameObject("LookAtZoomPoint");
        lookAtZoomPoint = newObject.transform;

        OptionValues option = FindObjectOfType<OptionValues>();//カメラの回転速度を初期化
        SetCameraSensityvity(option);
    }
    private void Update()
    {
        cameraLookPoint.position = potCenterPoint.position + (transform.position - potCenterPoint.position) * cameraLookPointRatio;
    }
    void LateUpdate()//Updateの後にカメラの位置を制御する
    {
        if (!isZoom)
        {
            PlayerCameraControlUpdate();
        }
        else
        {
            ZoomUpdate();
        }
        playerCameraAxis.transform.position = transform.position;//カメラをプレイヤーに移動させる
    }
    void PlayerCameraControlUpdate()
    {
        float rot = 0;
        //Debug.Log(Gamepad.current.rightStickButton.isPressed);
        //if(input.isRotateCameraRight() && input.isRotateCameraLeft())//同時押しで視点リセット
        //{
        //    playerCamera.transform.rotation = transform.rotation;
        //}
        //else
        //{
        //    if (input.isRotateCameraRight())
        //        rot++;
        //    if (input.isRotateCameraLeft())
        //        rot--;
        //    playerCamera.transform.Rotate(0, rot * sensityvity * Time.deltaTime, 0);
        //}

        rot = input.isRightStick().x;
        if (input.isRightStickButton())
        {
            isResetCamera = true;
        }

        if (rot != 0)
        {
            playerCameraAxis.transform.Rotate(0, rot * sensitivity * Time.deltaTime, 0);
            isResetCamera = false;
        }
        else if (isResetCamera)
        {
            playerCameraAxis.transform.rotation = Quaternion.RotateTowards(playerCameraAxis.transform.rotation, transform.rotation, resetSensitivity * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, playerCameraAxis.transform.rotation) < 0.1f)
            {
                isResetCamera = false;
            }
        }

        AutoCameraRotation();
    }
    void AutoCameraRotation()
    {
        if(canRotatoAuto && 
            Vector3.SqrMagnitude(transform.position - Vector3.zero) > nonRotationalAreaRadius * nonRotationalAreaRadius)
        {
            Vector3 centerDir = (Vector3.zero + Vector3.up * transform.position.y) - transform.position;
            Quaternion lookCenter = Quaternion.LookRotation(centerDir, Vector3.up);

            playerCameraAxis.transform.rotation = Quaternion.RotateTowards(playerCameraAxis.transform.rotation, lookCenter, autoRotateSensitivity * Time.deltaTime);
        }

        if(input.isRightStick() != Vector3.zero || input.isRightStickButton())
        {
            lastInputPosition = transform.position;
            canRotatoAuto = false;
        }
        else if(Vector3.SqrMagnitude(transform.position - lastInputPosition) > reapplicationDistance * reapplicationDistance)
        {
            canRotatoAuto= true;
        }
    }

    void SetCameraSensityvity(OptionValues optionValues)
    {
        if (optionValues == null) return;

        sensitivity = minSensitivity + (maxSensitivity - minSensitivity) * optionValues.GetCameraSensitivityRatio();
    }
    void ZoomUpdate()
    {
        if (!isResetZoom)//ズームする場所に移動中
        {
            if (zoomTimer < zoomTime)
            {
                MoveZoomCamera(verticalFOV_default, zoomFOV, 
                    defaultLookAtTransform.position, lookAtZoomPointRef.position + zoomShift,  zoomTimer / zoomTime);
            }
        }
        else//ズームから戻り中
        {
            if (zoomTimer < zoomTime)
            {
                MoveZoomCamera(zoomFOV, verticalFOV_default,
                    lookAtZoomPointRef.position + zoomShift, defaultLookAtTransform.position, zoomTimer / zoomTime);
            }
            else
            {
                //状態を戻す
                cinemachineVirtualCamera.m_Lens.FieldOfView = verticalFOV_default;
                cinemachineVirtualCamera.m_LookAt = defaultLookAtTransform;
                isZoom = false;
            }
        }
        zoomTimer += Time.deltaTime;

    }
    void MoveZoomCamera(float fovFrom, float fovTo, Vector3 lookAt_From, Vector3 lookAt_To, float timeRatio)
    {
        float ratio = zoomCameraMoveCurve.Evaluate(timeRatio);
        cinemachineVirtualCamera.m_Lens.FieldOfView = fovFrom + (fovTo - fovFrom) * ratio;//fovを少しずつ変える
        lookAtZoomPoint.transform.position = Vector3.Lerp(lookAt_From, lookAt_To, ratio);//対象の位置に少しずつ移動
    }
    public void StartZoom(Transform _to, float _zoomTime)//ズームを始める
    {
        isZoom = true;
        isResetZoom = false;
        zoomTime = _zoomTime;
        zoomTimer = 0f;

        lookAtZoomPointRef = _to;//ズーム後のLookAtを設定
        cinemachineVirtualCamera.m_LookAt = lookAtZoomPoint;//cinemachineのlookAtを設定
    }
    public void Zoom_Reset(float _zoomResetTime)//ズームをやめさせる
    {
        isResetZoom = true;
        zoomTime = _zoomResetTime;
        zoomTimer = 0f;
    }
}
