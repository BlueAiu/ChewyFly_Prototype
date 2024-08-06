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
    [Header("��󂪍ő剽�{�܂ŐL�т邩(1�ȏ�)")]
    [SerializeField] float maxArrowLength;

    [SerializeField] float minFlicBorder;

    [Header("�X�e�B�b�N��|�������Ԃɂ�����������")]
    [Tooltip("�Ō�̃L�[�̉����̒l�ŗ͂��ő�܂ł��܂�܂ł̎��Ԃ����܂�܂�")]
    [SerializeField] AnimationCurve flicPowerCurve;
    float lastPowerCurveTime;

    [Header("�ő�܂ł��߂����̉������")]
    [SerializeField] float maxFlicPower;

    [Header("�͂������Ƃ��̏�����ւ̗�")]
    [SerializeField] float flicUpPower;

    [Header("�e�����͂����m����X�e�B�b�N�̑���")]
    [SerializeField] float stickSpeed = 40f;

    [Header("���ɒe�����͂��ł���܂ł̎���")]
    [SerializeField] float flicCoolTime = 0.5f;

    [Header("�e���W�����v�̃W�����v��")]
    [SerializeField] float jumpPower = 5f;

    Vector3 arrowLocalScale;
    float flicTime;
    float lastFlicTime;
    Vector3 flicPreviousDirection;
    float arrowStretchOrigin;
    float arrowZSize;

    public bool isJumpMode { get; private set; } = false;

    private void Awake()//Start��肳��ɑO�Ɋi�[���Ă���
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
            // �o�E���f�B���O�{�b�N�X�̃T�C�Y���擾
            Vector3 size =  arrowRenderer.bounds.size;
            arrowZSize = size.z;// z�������̑傫�����擾
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
        if (playerController.ridingDonut != null)    //�h�[�i�c�ɏ���Ă���ꍇ
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

        if (IsFlic(direction))  //�e����鏈��
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

        if (IsFlic(direction))  //�e����鏈��
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

    void StretchArrow(Vector3 arrowDir)//������]�A�������΂�
    {
        Quaternion rotation = Quaternion.LookRotation(-arrowDir);
        arrowParent.rotation = rotation;
        if (flicTime < lastPowerCurveTime)
        {
            float scaleFactor = 1 + (flicTime / lastPowerCurveTime) * (maxArrowLength - 1);
            // �X�P�[����ύX
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
