using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    private class DescObject//Scene状にある説明文
    {
        public RectTransform rectTransform;
        public Image image;
        internal int index;//現在の、また動いた場合これから向かうべき位置のindex
    }
    [System.Serializable]
    class DescriptionData//説明の具体的な内容
    {
        public Sprite sprite;
        [TextArea]
        public string desc;
    }

    [Header("Sceneのオブジェクト参照の項目")]
    [SerializeField] DescObject[] descObjects = new DescObject[sceneDescImageNum];
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] Image triangle_Left;
    [SerializeField] Image triangle_Right;

    [Header("右左の矢印の色")]
    [SerializeField] Color triangleColor_Right;
    [SerializeField] Color triangleColor_Dark;

    [Header("説明文とその画像")]
    [SerializeField] DescriptionData[] descData;

    readonly Vector3[] defaultPosition = new Vector3[sceneDescImageNum];//最初の位置(固定)
    readonly Vector3[] defaultLocalScale = new Vector3[sceneDescImageNum];
    const int sceneDescImageNum = 5;
    int descNum = 0;//説明文の数

    [Tooltip("画像が動いて止まる時間")]
    [SerializeField] float moveTime = 1f;
    float moveTimer = 0f;
    bool isHalfLapse = false;//moveTimerが半分経過したか？
    enum MoveState { Stop, Right, Left };//説明ウィンドウが動く状態
    MoveState moveState;
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
    // Start is called before the first frame update
    void Start()
    {
        descNum = descData.Length;

        input = GetComponent<InputScript>();

        InitializeImages();
    }
    void InitializeImages()//初期化
    {
        moveState = MoveState.Stop;
        SetDescText(CurrentIndex);
        SetTriangleColor();
        for (int i = 0; i < sceneDescImageNum; i++)
        {
            defaultPosition[i] = descObjects[i].rectTransform.position;
            defaultLocalScale[i] = descObjects[i].rectTransform.localScale;

            descObjects[i].index = i - (sceneDescImageNum / 2);
            SetSpriteImage(descObjects[i]);//最初の画像をセット
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (moveState == MoveState.Stop)
        {
            if (input.isRightShoulder())//右に動いた
            {
                StartMove(true);
            }
            else if (input.isLeftShoulder())//右に動いた
            {
                StartMove(false);
            }
        }
        else//動いてる
        {
            UpdateImagesMove();
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
                    || CurrentIndex + (sceneDescImageNum / 2) < objIndex) continue;//ずっと見えないものは動かさない

            int newIndex = obj.index - CurrentIndex + (sceneDescImageNum / 2);//これから向かうindex
            int previousIndex;//移動する前のindex
            if (moveState == MoveState.Right)
                previousIndex = newIndex - 1;
            else
                previousIndex = newIndex + 1;

            obj.rectTransform.position = Vector3.Lerp(
                defaultPosition[previousIndex],
                defaultPosition[newIndex], moveTimer / moveTime);
            obj.rectTransform.localScale = Vector3.Lerp(
                defaultLocalScale[previousIndex],
                defaultLocalScale[newIndex], moveTimer / moveTime);
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
            obj.rectTransform.position = defaultPosition[index];
            obj.rectTransform.localScale = defaultLocalScale[index];
        }
        moveState = MoveState.Stop;
    }
}
