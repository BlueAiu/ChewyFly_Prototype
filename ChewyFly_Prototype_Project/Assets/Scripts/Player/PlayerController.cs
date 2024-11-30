using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Cinemachine.CinemachinePathBase;

public class PlayerController : MonoBehaviour
{
    CharacterController character;
    Animator animator;
    InputScript input;

    bool isFreeze = false;

    [Tooltip("�v���C���[���f���J����")]
    [SerializeField] public GameObject playerCamera { get; private set; }

    [Tooltip("����Ă���h�[�i�c")]
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
            if(value != null)//�o�E���h�̍Đ��������ׂ�Ƃ�����GetComponent���Ȃ��悤�ɂ����œ���Ă���
                ridingDonutUnion = _ridingDonut.GetComponent<DonutsUnionScript>();
            else
                ridingDonutUnion = null;
        }
    }
    public DonutsUnionScript ridingDonutUnion { get; private set; } = null;

    [Tooltip("�Q�[�����[���I�u�W�F�N�g")]
    [SerializeField] ObjectReferenceManeger objManeger;

    //[Header("�󒆂ɂ���Ƃ��̈ړ�")]
    //[Tooltip("�ړ��̑���")]
    //[SerializeField] float speed = 5f;

    public Vector3 velocity { get; set; } = Vector3.zero;

    //float velocityY = 0f;

    [Tooltip("�d�͉����x")]
    [SerializeField] float gravity = 10f;

    //[Tooltip("�W�����v��")]
    //[SerializeField] float jumpPower = 5f;
    //[Tooltip("�I�[���x (jumpPower�ȏ�ɂ��邱��)")]
    //[SerializeField] float terminalVelocity = 3f;

    [Tooltip("�W�����v�̔��ˊp�x(0~90)")]
    [SerializeField] float jumpSlopeAngle = 45f;

    [Tooltip("�v���C���[���ړ������Ɍ�������")]
    [SerializeField] float playerRotateSpeed = 450f;

    [Tooltip("�h�[�i�c�ɏ�����Ƃ���y���W�̂���")]
    [SerializeField] float aboveDonut = 1f;

    Vector3 previousDirection = Vector3.zero;

    //[Header("�h�[�i�c�̏�ɏ���Ă���ꍇ�̒e������")]
    //[Tooltip("�������Ɣ��肷�鋫�E")]
    //[SerializeField] float flicBorder = 0.1f;
    //[Tooltip("�e�����Ɣ��肷�鑁��")]
    //[SerializeField] float flicSpeed = 5f;
    //[Tooltip("�e���͂̋���")]
    //[SerializeField] float flicPower = 20f;

    [Tooltip("�h�[�i�c����]�����鑬��")]
    [SerializeField] float rotateSpeed = 5f;

    Vector3 donutRidePos = Vector3.zero;
    [Tooltip("�h�[�i�c�ɏ��ʒu�����̐��x")]
    [SerializeField] float donutRideAccuracy = 0.01f;
    [Tooltip("�h�[�i�c�ɏ��ʒu�����̑���")]
    [SerializeField] float donutRideSpeed = 2f;

    [Header("���ɗ�������")]

    [Tooltip("���ɗ��������̃G�t�F�N�g")]
    [SerializeField] GameObject damageEffect;

    [Tooltip("���ɗ����������܂鎞�Ԃ̒���")]
    [SerializeField] float oilSinkTime = 1f;

    [Tooltip("���ɗ��������̃W�����v�̒���")]
    [SerializeField] float oilJumpTime = 3f;

    [Header("�h�[�i�c�����������Ƃ�")]
    [Tooltip("�h�[�i�c�����������Ƃ����܂鎞�Ԃ̒���")]
    [SerializeField] float completeReactionTime = 1f;

    [Tooltip("���̃h�[�i�c�ɃW�����v���钷��")]
    [SerializeField] float completeJumpTime = 1f;

    [Header("�h�[�i�c�������̉��o����")]
    [Tooltip("�������J�����̕��Ɍ�������")]
    [SerializeField] float completeTime_LookCameraRotate = 0.3f;
    [Tooltip("�|�[�Y�����Ă��鎞��")]
    [SerializeField] float completeTime_Pose;
    [Tooltip("�J���������̈ʒu�ɖ߂鎞��")]
    [SerializeField] float completeTime_Reset;
    float completeReactionTimer = 0f;//�Y�[�����p�̃^�C�}�[
    bool isCompleteDonutReaction = false;//�������̃��A�N�V���������H
    Quaternion completeReactionRotateFrom;//�v���C���[����������
    Quaternion completeReactionRotateTo;//�����܂ŉ�]
    [SerializeField] Camera mainCamera;
    PlayerCameraRotation playerCameraRotation;

    private void Awake()//Start��肳��ɑO�Ɋi�[���Ă���
    {
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        input = GetComponent<InputScript>();
        playerCameraRotation = GetComponent<PlayerCameraRotation>();
        if (playerCamera == null)
            playerCamera = GameObject.Find("CameraAxis");
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

        UpdateRotation(-direction); //���X�e�B�b�N�̔��Ε���������

        if(ridingDonut == null)
        {
            //�ړ�
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

        //else if(input.isAButton())  //����Ă�h�[�i�c��؂藣���ăW�����v
        //{
        //    DetachDonut();
        //    velocityY = jumpPower;
        //}

        animator.SetFloat("VerticalVelocity", velocity.y);
        animator.SetBool("IsRideDonut", ridingDonut != null);
    }

    private void FixedUpdate()
    {
        if (ridingDonut != null)    //�h�[�i�c�ɏ���Ă���ꍇ
        {
            
            //var direction = input.isLeftStick();
            //direction = playerCamera.transform.TransformDirection(direction);

            var dounutRigid = ridingDonut.GetComponent<DonutRigidBody>();

            //�e������
            //if (isFlic(direction))
            //{
            //    dounutRigid.TakeImpulse(previousDirection.normalized * -flicPower);
            //}
            
            //��]����
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

    //�n���ꂽ�����Ƀv���C���[�̐��ʂ�������
    //FacingForward
    private void UpdateRotation(Vector3 dir)
    {
        if (dir.sqrMagnitude <= 0) return;  //���͂���Ă���Ȃ�

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

    //�w�肵�����ˊp�x�ŃW�����v����
    //����ł��璅�n�܂ł̎��Ԃ͔򋗗��ɔ�Ⴕ�Ă���
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

    public void JumpTo(Vector3 target, float time)  //����ł��璅�n�܂ł̎��Ԃ��w�肷��
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
        if (hit.gameObject.tag == "Donuts" && character.isGrounded) //�h�[�i�c�ɒ��n
        {
            AttachDonut(hit.transform.parent.gameObject);
            //hit.gameObject.GetComponent<DonutSphereReference>().OnPlayerEnter();

            donutRidePos = hit.transform.localPosition + Vector3.up * aboveDonut;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Oil")   //���ɒ���
        {
            transform.position += Vector3.up * (ObjectReferenceManeger.oilSurfaceY - transform.position.y);
            isFreeze = true;
            Invoke(nameof(OilJump), oilSinkTime);
            animator.SetTrigger("JumpFailtuer");
        }
    }

    void OilJump()
    {
        isFreeze = false;

        var targetPos = objManeger.ClosestDonut(isFleeze: true).transform.position + new Vector3(0, aboveDonut, 0);
        JumpTo(targetPos, oilJumpTime);

        Instantiate(damageEffect, transform.position, Quaternion.identity);
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
        var targetPos = objManeger.ClosestDonut(isFleeze: true).transform.position + new Vector3(0, aboveDonut, 0);
        JumpTo(targetPos, completeJumpTime);

        animator.SetTrigger("JumpTrigger");
    }
    void StartCompleteReaction()//�h�[�i�c�������̃��A�N�V�����J�n
    {
        completeReactionTimer = 0f;
        isCompleteDonutReaction = true;
        completeReactionRotateFrom = transform.rotation;
        Vector3 direction = mainCamera.transform.position - transform.position;
        completeReactionRotateTo = Quaternion.LookRotation(direction);

        playerCameraRotation.StartZoom(transform, completeTime_LookCameraRotate);
    }
    void Update_CompleteDonutReaction()//Update�ŌĂ΂��h�[�i�c�������̃��A�N�V����
    {
        if (!isCompleteDonutReaction) return;

        if (completeReactionTimer < completeTime_LookCameraRotate)
        {
            //�v���C���[���J�����Ɍ�����
            transform.rotation = Quaternion.Lerp(completeReactionRotateFrom, completeReactionRotateTo,
                completeReactionTimer / completeTime_LookCameraRotate);

            if(completeReactionTimer + Time.deltaTime > completeTime_LookCameraRotate)//��]���I�������
            {
                transform.rotation = completeReactionRotateTo;
                animator.SetTrigger("CompletePose");
            }
        }
        else if (completeReactionTimer > completeTime_LookCameraRotate + completeTime_Pose)//��]�A�|�[�Y���ԏI��
        {
            playerCameraRotation.Zoom_Reset(completeTime_Reset);//�J���������̈ʒu�ɖ߂���
            CompleteJump();//�W�����v
            return;
        }
        completeReactionTimer += Time.deltaTime;
    }
}
