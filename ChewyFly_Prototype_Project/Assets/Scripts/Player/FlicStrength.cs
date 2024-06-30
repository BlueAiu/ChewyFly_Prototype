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

    Vector3 arrowLocalScale;
    float flicTime;
    float lastFlicTime;
    Vector3 flicPreviousDirection;
    float arrowStretchOrigin;
    float arrowZSize;

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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerController.ridingDonut != null)    //�h�[�i�c�ɏ���Ă���ꍇ
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
        else
        {
            arrowSprite.SetActive(false);
            flicTime = 0f;
        }
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

            arrowSprite.transform.localPosition = new Vector3(0, 0, arrowStretchOrigin + arrowZSize / 2 * scaleFactor);
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
