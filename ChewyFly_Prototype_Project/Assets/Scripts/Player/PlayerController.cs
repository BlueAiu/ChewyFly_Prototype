using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    CharacterController character;
    InputScript input;

    [Tooltip("�v���C���[���f���J����")]
    [SerializeField] public GameObject playerCamera { get; private set; }

    [Tooltip("����Ă���h�[�i�c")]
    [SerializeField] public GameObject ridingDonut { get; private set; }

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
    [Tooltip("�I�[���x (jumpPower�ȏ�ɂ��邱��)")]
    [SerializeField] float terminalVelocity = 3f;

    [Tooltip("�v���C���[���ړ������Ɍ�������")]
    [SerializeField] float playerRotateSpeed = 450f;

    [Tooltip("�h�[�i�c�̏�Ɉړ�������Ƃ���y���W�̂���")]
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

    private void Awake()//Start��肳��ɑO�Ɋi�[���Ă���
    {
        character = GetComponent<CharacterController>();
        input = GetComponent<InputScript>();
        if (playerCamera == null)
            playerCamera = GameObject.Find("CameraAxes");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var direction = input.isLeftStick();
        direction = playerCamera.transform.TransformDirection(direction);

        UpdateRotation(direction);

        if(ridingDonut == null)
        {
            //�ړ�
            //velocityY -= gravity * Time.deltaTime;
            //velocityY = Mathf.Max(velocityY, -terminalVelocity);

            //Vector3 velocity = direction * speed + Vector3.up * velocityY;

            if(velocity.y > -terminalVelocity)
            {
                velocity += Vector3.down * (gravity * Time.deltaTime);
            }

            character.Move(velocity * Time.deltaTime);
        }
        //else if(input.isAButton())  //����Ă�h�[�i�c��؂藣���ăW�����v
        //{
        //    DetachDonut();
        //    velocityY = jumpPower;
        //}
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
    }

    //���X�e�B�b�N�̕����Ƀv���C���[�̐��ʂ�������
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

    public void JumpTo(Vector3 target)  //����ł��璅�n�܂ł̎��ԂƔ򋗗�����Ⴕ�Ă���
    {
        DetachDonut();

        Vector3 direction = target - transform.position;
        Debug.Log(direction);
        
        velocity = (direction.normalized + Vector3.up) * Mathf.Sqrt(direction.magnitude * gravity / 2);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Donuts" && character.isGrounded) //�h�[�i�c�ɒ��n
        {
            AttachDonut(hit.transform.parent.gameObject);
            //hit.gameObject.GetComponent<DonutSphereReference>().OnPlayerEnter();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.name == "Oil")   //���ɒ���
        {
            //DetachDonut();
            var targetPos = objManeger.ClosestDonut().transform.position + new Vector3(0, aboveDonut, 0);
            //character.Move(targetPos - transform.position);
            //velocity = Vector3.zero;
            JumpTo(targetPos);
        }
    }
}
