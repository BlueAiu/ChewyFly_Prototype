using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    static bool isTransitionToMainGame;//メインゲームに遷移中か
    public static void TransitionToMainGame()
    {
        isTransitionToMainGame = true;
    }

    [SerializeField] SoundManager soundManager;
    [SerializeField] AudioClip BGM;

    [System.Serializable]
    private class DescObject//Scene上にある説明文
    {
        public RectTransform rectTransform;
        public Image image;
        internal int index;//現在の、また動いた場合これから向かうべき位置のindex
    }
    [System.Serializable]
    class DescriptionData//説明の具体的な内容
    {
        public Sprite sprite;
        [TextArea] public string desc;
    }
    enum MoveState { Stop, Right, Left , Loop};//説明ウィンドウが動く状態

    [Header("Sceneのオブジェクト参照の項目")]
    [SerializeField] DescObject[] descObjects = new DescObject[sceneDescImageNum];
    [SerializeField] Transform[] defaultTransform = new Transform[sceneDescImageNum];//最初の位置(固定)
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] Image triangle_Left;
    [SerializeField] Image triangle_Right;

    [SerializeField] Button skipButton;//ゲームに移行する場合のスキップボタン

    [Header("右左の矢印の色")]
    [SerializeField] Color triangleColor_Right;
    [SerializeField] Color triangleColor_Dark;

    [Header("説明文とその画像")]
    [SerializeField] DescriptionData[] descData;
    const int sceneDescImageNum = 5;
    int descNum;//説明文の数

    [Tooltip("画像が動いて止まる時間")]
    [SerializeField] float moveTime = 1f;
    float moveTimer = 0f;
    bool isHalfLapse = false;//moveTimerが半分経過したか？
    MoveState moveState;
    [Tooltip("右端左端から反対方向に移動した後入力を受け付けない時間")]
    [SerializeField] float loopStopTime = 0.2f;
    int _currentIndex = 0;//現在表示中の画像の番号(中心)
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
    void InitializeImages()//初期化
    {
        for (int i = 0; i < sceneDescImageNum; i++)//初期位置を設定
        {
            defaultTransform[i].position = descObjects[i].rectTransform.position;
            defaultTransform[i].localScale = descObjects[i].rectTransform.localScale;
        }
        moveState = MoveState.Stop;
        SetAllSprites();
    }
    void SetAllSprites()//CurrentIndexに対応したスプライトを表示
    {
        SetDescText(CurrentIndex);
        SetTriangleColor();
        for (int i = 0; i < sceneDescImageNum; i++)
        {
            descObjects[i].rectTransform.position = defaultTransform[i].position;
            descObjects[i].rectTransform.localScale = defaultTransform[i].localScale;
            descObjects[i].index = CurrentIndex + i - (sceneDescImageNum / 2);
            SetSpriteImage(descObjects[i]);//最初の画像をセット
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
                || input.isRightStick().x > 0)//右を入力した
            {
                StartMove(true);
            }
            else if (input.isLeftShoulder() || input.isLeftTrigger()
                || input.isLeftDpad().x < 0 || input.isLeftStick().x < 0
                || input.isRightStick().x < 0)//左を入力した
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
        else if (moveState == MoveState.Right || moveState == MoveState.Left)//動いてる
        {
            UpdateImagesMove();
        }

        if (isTransitionToMainGame && input.isNorthButton())
        {
            StartMainGame();
        }
    }

    void SetSpriteImage(DescObject descObj)//index通りに画像を入れる
    {
        if (descObj.index < 0 || descNum <= descObj.index)//範囲外なら表示しない
        {
            descObj.rectTransform.gameObject.SetActive(false);
            return;
        }
        descObj.rectTransform.gameObject.SetActive(true);
        descObj.image.sprite = descData[descObj.index].sprite;
    }
    void SetDescText(int index)//説明文を入れる
    {
        if (index < 0 || index >= descNum)
        { Debug.Log("範囲外にアクセスしようとしました"); descText.text = ""; return; }

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

    public void StartMove(bool inputRight)//説明文を動かし始める
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
            else//左端なら右端に移す
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
                if (isTransitionToMainGame)//右端に行き着いている
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
                    || CurrentIndex + (sceneDescImageNum / 2) < objIndex) continue;//ずっと見えないものは動かさない

            int newIndex = obj.index - CurrentIndex + (sceneDescImageNum / 2);//これから向かうindex
            int previousIndex = moveState == MoveState.Right ? newIndex - 1 : newIndex + 1;//移動する前のindex

            obj.rectTransform.position = Vector3.Lerp(
                defaultTransform[previousIndex].position,
                defaultTransform[newIndex]     .position, moveTimer / moveTime);
            obj.rectTransform.localScale = Vector3.Lerp(
                defaultTransform[previousIndex].localScale,
                defaultTransform[newIndex]     .localScale, moveTimer / moveTime);
        }

        if (!isHalfLapse && moveTime >= moveTimer / 2f)//半分経過したら色、テキストを変化
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
            if (obj.index < CurrentIndex - (sceneDescImageNum / 2))//動かさなかったスプライトを更新
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
    public void CloseTutorial()  //画面を閉じる処理
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
