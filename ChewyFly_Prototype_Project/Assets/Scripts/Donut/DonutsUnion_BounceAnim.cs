using UnityEngine;

public partial class DonutsUnionScript : MonoBehaviour
{
    [Tooltip("�h�[�i�c�̃o�E���h�̓����̕ω�(������0����1�̊Ԃ�)")]
    [SerializeField] AnimationCurve bounceScaleCurve;
    GameObject bounceDonutsParent = null;//�h�[�i�c���g��k������Ƃ��̑S�̂̐e(union�̃g�����X�t�H�[�����R�s�[��������)
    GameObject bounceDonutsAxis = null;//�h�[�i�c���g��k������Ƃ��̎�
    [SerializeField] float maxBounceTime = 0.5f;
    [SerializeField] float maxBounceScaleSize = 1f;
    float bounceTimer = 0f;
    public bool isBouncing { get; private set; } = false;//���݃o�E���h�̃A�j���[�V��������

    private void UpdateBounceAnim()//Update�֐��ŌĂ΂�āA�o�E���h�̃A�j���[�V�������X�V����
    {
        if (isBouncing)//�o�E���h�̃A�j���[�V������
        {
            CopyUnionPosAndScale();
            bounceTimer += Time.deltaTime;
            if (bounceTimer < maxBounceTime)
            {
                float bounceScaleValue = bounceScaleCurve.Evaluate(bounceTimer / maxBounceTime) * maxBounceScaleSize;
                bounceDonutsAxis.transform.localScale = new Vector3(
                    bounceScaleValue, bounceScaleValue, 1f / bounceScaleValue);
            }
            else//�o�E���h�̃A�j���[�V�����I���
            {
                StopDonutsBounce();
            }
        }
    }
    private void InstantiateBounceDonuts(Transform lookAtTransform)//�o�E���h���邽�߂̌����ڂ����̃h�[�i�c��p�ӂ���
    {
        if (isBouncing) StopDonutsBounce();
        bounceTimer = 0f;
        isBouncing = true;

        bounceDonutsParent = new GameObject("BounceDonutsParent");//�o�E���h����h�[�i�c�̑S�̂̐e��p��
        CopyUnionPosAndScale();

        bounceDonutsAxis = new GameObject("BounceDonutsAxis");//�o�E���h����h�[�i�c�̊g��k���̒��S��p��
        bounceDonutsAxis.transform.parent = bounceDonutsParent.transform;
        bounceDonutsAxis.transform.localPosition = Vector3.zero;
        bounceDonutsAxis.transform.LookAt(lookAtTransform);

        foreach (var sphere in donutSpheres)
        {
            GameObject donutMeshObj = Instantiate(sphere);//�����̃h�[�i�c�𕡐�
            donutMeshObj.transform.position = sphere.transform.position;

            DonutSphereColor donutColor = sphere.GetComponent<DonutSphereColor>();//DonutSphereColor�̒��g�𓯂��ɂ���
            DonutSphereColor bounceDonutColor = donutMeshObj.GetComponent<DonutSphereColor>();
            bounceDonutColor.CopySphereColorValue(donutColor);

            Component[] donutComponents = donutMeshObj.GetComponents<Component>();
            foreach (Component component in donutComponents)//�폜���Č����ڂ����ɂ���
            {
                if (!(component is Transform) && !(component is MeshFilter) && !(component is MeshRenderer) && !(component is DonutSphereColor))
                {
                    Destroy(component);
                }
            }
            sphere.GetComponent<MeshRenderer>().enabled = false;//���Ƃ̃h�[�i�c�͂��������\��
            donutMeshObj.transform.parent = bounceDonutsAxis.transform;
        }
    }
    private void CopyUnionPosAndScale()//�R�s�[�����o�E���h�p�̃h�[�i�c�̈ʒu�����X�̃h�[�i�c�̈ʒu�ɍX�V
    {
        bounceDonutsParent.transform.position = transform.position;
        bounceDonutsParent.transform.rotation = transform.rotation;
    }
    public void StopDonutsBounce()//�h�[�i�c�̃o�E���h�̃A�j���[�V�������~����
    {
        foreach (var sphere in donutSpheres)
        {
            sphere.GetComponent<MeshRenderer>().enabled = true;
        }
        isBouncing = false;
        bounceTimer = 0f;
        Destroy(bounceDonutsParent);
    }
}
