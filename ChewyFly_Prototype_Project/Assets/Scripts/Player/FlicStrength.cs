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

    [Tooltip("��󂪍ő剽�{�܂ŐL�т邩(1�ȏ�)")]
    [SerializeField] float maxArrowLength;

    [SerializeField] float minFlicBorder;

    [Header("�X�e�B�b�N��|�������Ԃɂ�����������")]
    [Tooltip("�Ō�̃L�[�̉����̒l�ŗ͂��ő�܂ł��܂�܂ł̎��Ԃ����܂�܂�")]
    [SerializeField] AnimationCurve flicPowerCurve;
    float lastPowerCurveTime;

    [Tooltip("�ő�܂ł��߂����̉������")]
    [SerializeField] float maxFlicDonutPower;

    [Tooltip("�͂������Ƃ��̏�����ւ̗�")]
    [SerializeField] float flicUpPower;

    [Tooltip("�e�����͂����m����X�e�B�b�N�̑���")]
    [SerializeField] float stickSpeed = 40f;
    [SerializeField] float maxStickSpeed = 45f;
    [SerializeField] float minStickSpeed = 0f;

    [Tooltip("���ɒe�����͂��ł���܂ł̎���")]
    [SerializeField] float flicCoolTime = 0.5f;

    [Tooltip("�e���W�����v�̃W�����v��")]
    [SerializeField] float jumpPower = 5f;

    [Tooltip("�W�����v���n�n�_")]
    [SerializeField] Transform jumpPoint;

    [Tooltip("�W�����v�����̔��a")]
    [SerializeField] float jumpAdjustRadius = 0.7f;

    Vector3 arrowLocalScale;
    float flicTime;
    float lastFlicTime;
    float flicPower;
    [Tooltip("�����͂̕ω��̎���")]
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


    private void Awake()//Start��肳��ɑO�Ɋi�[���Ă���
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
            // �o�E���f�B���O�{�b�N�X�̃T�C�Y���擾
            Vector3 size =  arrowRenderer.bounds.size;
            arrowZSize = size.z;// z�������̑傫�����擾
        }
        arrowStretchOrigin = flicArrowSprite.transform.localPosition.z - arrowZSize / 2;
        Keyframe lastKey = flicPowerCurve.keys[flicPowerCurve.length - 1];
        lastPowerCurveTime = lastKey.time;
    }

    // Start is called before the first frame update
    void Start()
    {
        OptionValues option = FindObjectOfType<OptionValues>();//���x��������
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
        if (playerController.ridingDonut != null && !playerController.ridingDonutUnion.isBouncing)    //�h�[�i�c�ɏ���Ă��ăh�[�i�c���o�E���h���łȂ��ꍇ
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
        if (playerController.ridingDonutUnion != null && playerController.ridingDonutUnion.isBouncing) return;//�o�E���h���Ȃ�󂯕t���Ȃ�

        if (IsFlic(direction))  //�e����鏈��
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
        if (IsFlic(direction))  //�e����鏈��
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



    void StretchArrow(Vector3 arrowDir)//������]�A�������΂�
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
        // �X�P�[����ύX
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
