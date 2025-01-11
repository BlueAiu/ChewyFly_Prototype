using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    static bool isTransitionToMainGame;//���C���Q�[���ɑJ�ڒ���
    public static void TransitionToMainGame()
    {
        isTransitionToMainGame = true;
    }

    [SerializeField] SoundManager soundManager;
    [SerializeField] AudioClip BGM;

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
    enum MoveState { Stop, Right, Left , Loop};//�����E�B���h�E���������

    [Header("Scene�̃I�u�W�F�N�g�Q�Ƃ̍���")]
    [SerializeField] DescObject[] descObjects = new DescObject[sceneDescImageNum];
    [SerializeField] Transform[] defaultTransform = new Transform[sceneDescImageNum];//�ŏ��̈ʒu(�Œ�)
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] Image triangle_Left;
    [SerializeField] Image triangle_Right;

    [SerializeField] Button skipButton;//�Q�[���Ɉڍs����ꍇ�̃X�L�b�v�{�^��

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
    [Tooltip("�E�[���[���甽�Ε����Ɉړ���������͂��󂯕t���Ȃ�����")]
    [SerializeField] float loopStopTime = 0.2f;
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

        soundManager.PlayBGM(BGM);

        if (ObjectReferenceManeger.HighScore > 0)
            isTransitionToMainGame = false;

        skipButton.gameObject.SetActive(isTransitionToMainGame);
    }
    void InitializeImages()//������
    {
        for (int i = 0; i < sceneDescImageNum; i++)//�����ʒu��ݒ�
        {
            defaultTransform[i].position = descObjects[i].rectTransform.position;
            defaultTransform[i].localScale = descObjects[i].rectTransform.localScale;
        }
        moveState = MoveState.Stop;
        SetAllSprites();
    }
    void SetAllSprites()//CurrentIndex�ɑΉ������X�v���C�g��\��
    {
        SetDescText(CurrentIndex);
        SetTriangleColor();
        for (int i = 0; i < sceneDescImageNum; i++)
        {
            descObjects[i].rectTransform.position = defaultTransform[i].position;
            descObjects[i].rectTransform.localScale = defaultTransform[i].localScale;
            descObjects[i].index = CurrentIndex + i - (sceneDescImageNum / 2);
            SetSpriteImage(descObjects[i]);//�ŏ��̉摜���Z�b�g
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (input.isBButton())
        {
            CloseTutorial();
        }

        if (moveState == MoveState.Stop)
        {
            if (input.isRightShoulder() || input.isRightTrigger()
                || input.isLeftDpad().x > 0 || input.isLeftStick().x > 0
                || input.isRightStick().x > 0)//�E����͂���
            {
                StartMove(true);
            }
            else if (input.isLeftShoulder() || input.isLeftTrigger()
                || input.isLeftDpad().x < 0 || input.isLeftStick().x < 0
                || input.isRightStick().x < 0)//������͂���
            {
                StartMove(false);
            }
        }
        else if (moveState == MoveState.Loop)
        {
            moveTimer += Time.deltaTime;
            if (moveTimer > loopStopTime)
                moveState = MoveState.Stop;
        }
        else if (moveState == MoveState.Right || moveState == MoveState.Left)//�����Ă�
        {
            UpdateImagesMove();
        }

        if (isTransitionToMainGame && input.isNorthButton())
        {
            StartMainGame();
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
        if (!inputRight)
        {
            if (0 < CurrentIndex)
            {
                moveState = MoveState.Right;
                CurrentIndex--;
            }
            else//���[�Ȃ�E�[�Ɉڂ�
            {
                CurrentIndex = descNum - 1;
                moveState = MoveState.Loop;
                SetAllSprites();
            }
        }
        else if (inputRight)
        {
            if (CurrentIndex < descNum - 1)
            {
                moveState = MoveState.Left;
                CurrentIndex++;
            }
            else
            {
                if (isTransitionToMainGame)//�E�[�ɍs�������Ă���
                { StartMainGame(); return; }

                CurrentIndex = 0;
                moveState = MoveState.Loop;
                SetAllSprites();
            }
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
    public void StartMainGame()
    {
        sceneManager.LoadSceneName("MainScene");
    }
    public void OnDestroy()
    {
        isTransitionToMainGame = false;
    }
}
