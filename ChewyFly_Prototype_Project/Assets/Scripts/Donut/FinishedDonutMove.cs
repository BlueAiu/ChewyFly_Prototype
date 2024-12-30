using UnityEngine;

public partial class DonutRigidBody : MonoBehaviour
{
    [Header("���������Ƃ��̓���")]
    [Tooltip("�h�[�i�c���J�����Ɍ����ė��Ă�܂ł̎���")]
    [SerializeField] float phese1Time = 1f;
    [Tooltip("�h�[�i�c�𗧂Ă�p�x")]
    [SerializeField] float standDonutAngle = 90;
    //[Tooltip("��Ɉړ����鋗��")]
    //[SerializeField] float phese1ShiftY = 2f;
    [Tooltip("�h�[�i�c�������h�[�i�c���ǂ̂��炢���ɒu����(-�Ȃ��O��)")]
    [SerializeField] float donutShiftToBackLength = 2f;
    [Tooltip("�h�[�i�c�������h�[�i�c���ǂ̂��炢���ɒu����")]
    [SerializeField] float donutShiftToLeftLength = 2f;

    [Tooltip("���񂾂��ɉ������鎞��")]
    [SerializeField] float phese2Time = 1f;
    [Tooltip("��ɉ�����������x")]
    [SerializeField] float phese2Acceleration = 1f;

    [Tooltip("���������h�[�i�c��u���Ă����ꏊ")]
    [SerializeField] Vector3 storageArea;

    [Header("�ł������̓���")]
    [Tooltip("�h�[�i�c�����ގ��ʂ̔{��")]
    [SerializeField] float sinkMassRate = 2f;
    [Tooltip("����ŏ�����܂ł̎���")]
    [SerializeField] float sinkTime = 1.5f;

    Transform cameraAxis;
    Collider[] colliders;
    bool isFinishMoving = false;

    int actionPhese = 0;
    float actionTimer = 0f;

    bool isBurnt = false;

    Vector3 previousDonutPos;
    Vector3 newDonutPos;//�������Aphese1�ł��̈ʒu�Ƀh�[�i�c���ړ�����
    public void SetMoveMode(Vector3 playerPos)
    {
        rb.isKinematic = true;
        isFinishMoving = true;

        colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }

        union.StopAllBurntEffect();
        transform.parent = cameraAxis;
        torque = 0;

        Vector3 playerDir = playerPos - Camera.main.transform.position;
        playerDir.Normalize(); playerDir *= donutShiftToBackLength;
        Vector3 donutPosDiff = transform.position - GetComponent<DonutsUnionScript>().GetDonutsCenterPoint();
        donutPosDiff += -Camera.main.transform.right * donutShiftToLeftLength;
        newDonutPos = playerPos + playerDir + donutPosDiff;
        previousDonutPos = transform.position;
    }

    void FinishedDonutMove()
    {
        if (!isFinishMoving) return;

        actionTimer += Time.deltaTime;
        switch (actionPhese)
        {
            case 0:
                var rotationDir = cameraAxis.TransformDirection(Vector3.left);
                transform.RotateAround(transform.position, rotationDir, standDonutAngle * Time.deltaTime / phese1Time);
                //transform.Translate(Vector3.up * phese1ShiftY * Time.deltaTime / phese1Time);
                transform.position = Vector3.Lerp(previousDonutPos, newDonutPos, actionTimer / phese1Time);

                if (actionTimer > phese1Time)
                {
                    actionTimer = 0f; actionPhese++;
                }
                break;

            case 1:
                transform.position += Vector3.up * (actionTimer * phese2Acceleration);

                if(actionTimer > phese2Time)
                {
                    actionTimer = 0f; actionPhese++;
                }
                break;

            case 2:
                transform.parent = null;
                rb.isKinematic = false;
                isFinishMoving = false;

                foreach(var c in colliders)
                {
                    c.enabled = true;
                    PhysicMaterial physicMaterial = c.material;//�e����0�ɂ���
                    if(physicMaterial != null)
                        physicMaterial.bounciness = 0f;
                }

                DontDestroyOnLoad(gameObject);

                //transform.position = storageArea;
                transform.position = union.objManeger.GetDonutDropPosition(transform.position, union.GetDonutsCenterPoint());

                break;
        }
    }

    public void SetSinkMode()
    {
        if (isBurnt) return;
        isBurnt = true;

        rb.mass *= sinkMassRate;
        Destroy(gameObject, sinkTime);
    }
}
