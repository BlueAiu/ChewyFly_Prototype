using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    private class DescObject//Scene��ɂ��������
    {
        public RectTransform rectTransform;
        public Image image;
        internal int index;//���݂́A�܂��������ꍇ���ꂩ��������ׂ��ʒu��index
    }
    [System.Serializable]
    class DescriptionData//�����̋�̓I�ȓ��e
    {
        public Sprite sprite;
        [TextArea] public string desc;
    }
    enum MoveState { Stop, Right, Left };//�����E�B���h�E���������

    [Header("Scene�̃I�u�W�F�N�g�Q�Ƃ̍���")]
    [SerializeField] DescObject[] descObjects = new DescObject[sceneDescImageNum];
    [SerializeField] Transform[] defaultTransform = new Transform[sceneDescImageNum];//�ŏ��̈ʒu(�Œ�)
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] Image triangle_Left;
    [SerializeField] Image triangle_Right;

    [Header("�E���̖��̐F")]
    [SerializeField] Color triangleColor_Right;
    [SerializeField] Color triangleColor_Dark;

    [Header("�������Ƃ��̉摜")]
    [SerializeField] DescriptionData[] descData;
    const int sceneDescImageNum = 5;
    int descNum;//�������̐�

    [Tooltip("�摜�������Ď~�܂鎞��")]
    [SerializeField] float moveTime = 1f;
    float moveTimer = 0f;
    bool isHalfLapse = false;//moveTimer�������o�߂������H
    MoveState moveState;
    int _currentIndex = 0;//���ݕ\�����̉摜�̔ԍ�(���S)
    int CurrentIndex
    {
        get     {  return _currentIndex;   }
        set
        {
            _currentIndex = value;
            _currentIndex = Mathf.Clamp(_currentIndex, 0, descNum - 1);
        }
    }

    InputScript input;
    LoadSceneManager sceneManager;
    // Start is called before the first frame update
    void Start()
    {
        descNum = descData.Length;

        input = GetComponent<InputScript>();
        sceneManager = GetComponent<LoadSceneManager>();

        InitializeImages();
    }
    void InitializeImages()//������
    {
        moveState = MoveState.Stop;
        SetDescText(CurrentIndex);
        SetTriangleColor();
        for (int i = 0; i < sceneDescImageNum; i++)
        {
            defaultTransform[i].position = descObjects[i].rectTransform.position;
            defaultTransform[i].localScale = descObjects[i].rectTransform.localScale;
            descObjects[i].index = i - (sceneDescImageNum / 2);
            SetSpriteImage(descObjects[i]);//�ŏ��̉摜���Z�b�g
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (input.isAButton())
        {
            CloseTutorial();
        }

        if (moveState == MoveState.Stop)
        {
            if (input.isRightShoulder() || input.isRightTrigger()
                || input.isLeftDpad().x > 0)//�E����͂���
            {
                StartMove(true);
            }
            else if (input.isLeftShoulder() || input.isLeftTrigger()
                || input.isLeftDpad().x < 0)//������͂���
            {
                StartMove(false);
            }
        }
        else//�����Ă�
        {
            UpdateImagesMove();
        }
    }

    void SetSpriteImage(DescObject descObj)//index�ʂ�ɉ摜������
    {
        if (descObj.index < 0 || descNum <= descObj.index)//�͈͊O�Ȃ�\�����Ȃ�
        {
            descObj.rectTransform.gameObject.SetActive(false);
            return;
        }
        descObj.rectTransform.gameObject.SetActive(true);
        descObj.image.sprite = descData[descObj.index].sprite;
    }
    void SetDescText(int index)//������������
    {
        if (index < 0 || index >= descNum)
        { Debug.Log("�͈͊O�ɃA�N�Z�X���悤�Ƃ��܂���"); descText.text = ""; return; }

        descText.text = descData[index].desc;
    }
    void SetTriangleColor()
    {
        if (CurrentIndex > 0)
            triangle_Left.color = triangleColor_Right;
        else
            triangle_Left.color = triangleColor_Dark;

        if (CurrentIndex < descNum - 1)
            triangle_Right.color = triangleColor_Right;
        else
            triangle_Right.color = triangleColor_Dark;
    }

    public void StartMove(bool inputRight)//�������𓮂����n�߂�
    {
        if (moveState != MoveState.Stop) return;

        moveTimer = 0f; isHalfLapse = false;
        if (!inputRight && 0 < CurrentIndex)
        {
            moveState = MoveState.Right;
            CurrentIndex--;
        }
        else if (inputRight && CurrentIndex < descNum - 1)
        {
            moveState = MoveState.Left;
            CurrentIndex++;
        }
    }

    void UpdateImagesMove()
    {
        moveTimer += Time.deltaTime;
        if (moveTimer > moveTime)  {  FinishMoving(); return; }

        foreach (var obj in descObjects)
        {
            int objIndex = obj.index;
            if (objIndex < CurrentIndex - (sceneDescImageNum / 2) 
                    || CurrentIndex + (sceneDescImageNum / 2) < objIndex) continue;//�����ƌ����Ȃ����͓̂������Ȃ�

            int newIndex = obj.index - CurrentIndex + (sceneDescImageNum / 2);//���ꂩ�������index
            int previousIndex = moveState == MoveState.Right ? newIndex - 1 : newIndex + 1;//�ړ�����O��index

            obj.rectTransform.position = Vector3.Lerp(
                defaultTransform[previousIndex].position,
                defaultTransform[newIndex]     .position, moveTimer / moveTime);
            obj.rectTransform.localScale = Vector3.Lerp(
                defaultTransform[previousIndex].localScale,
                defaultTransform[newIndex]     .localScale, moveTimer / moveTime);
        }

        if (!isHalfLapse && moveTime >= moveTimer / 2f)//�����o�߂�����F�A�e�L�X�g��ω�
        {
            isHalfLapse = true;
            SetDescText(CurrentIndex);
            SetTriangleColor();
        }
    }
    void FinishMoving()
    {
        foreach (var obj in descObjects)
        {
            if (obj.index < CurrentIndex - (sceneDescImageNum / 2))//�������Ȃ������X�v���C�g���X�V
            {
                obj.index = CurrentIndex + (sceneDescImageNum / 2);
                SetSpriteImage(obj);
            }
            else if (CurrentIndex + (sceneDescImageNum / 2) < obj.index)
            {
                obj.index = CurrentIndex - (sceneDescImageNum / 2);
                SetSpriteImage(obj);
            }

            int index = obj.index - CurrentIndex + (sceneDescImageNum / 2);
            obj.rectTransform.position = defaultTransform[index].position;
            obj.rectTransform.localScale = defaultTransform[index].localScale;
        }
        moveState = MoveState.Stop;
    }
    public void CloseTutorial()  //��ʂ���鏈��
    {
        sceneManager.LoadSceneName("TitleScene");
    }
}
