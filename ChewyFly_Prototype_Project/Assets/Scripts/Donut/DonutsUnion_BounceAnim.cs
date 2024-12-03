using UnityEngine;

public partial class DonutsUnionScript : MonoBehaviour
{
    [Tooltip("ドーナツのバウンドの動きの変化(横軸は0から1の間で)")]
    [SerializeField] AnimationCurve bounceScaleCurve;
    GameObject bounceDonutsParent = null;//ドーナツを拡大縮小するときの全体の親(unionのトランスフォームをコピーし続ける)
    GameObject bounceDonutsAxis = null;//ドーナツを拡大縮小するときの軸
    [SerializeField] float maxBounceTime = 0.5f;
    [SerializeField] float maxBounceScaleSize = 1f;
    float bounceTimer = 0f;
    public bool isBouncing { get; private set; } = false;//現在バウンドのアニメーション中か

    private void UpdateBounceAnim()//Update関数で呼ばれて、バウンドのアニメーションを更新する
    {
        if (isBouncing)//バウンドのアニメーション中
        {
            CopyUnionPosAndScale();
            bounceTimer += Time.deltaTime;
            if (bounceTimer < maxBounceTime)
            {
                float bounceScaleValue = bounceScaleCurve.Evaluate(bounceTimer / maxBounceTime) * maxBounceScaleSize;
                bounceDonutsAxis.transform.localScale = new Vector3(
                    bounceScaleValue, bounceScaleValue, 1f / bounceScaleValue);
            }
            else//バウンドのアニメーション終わり
            {
                StopDonutsBounce();
            }
        }
    }
    private void InstantiateBounceDonuts(Transform lookAtTransform)//バウンドするための見た目だけのドーナツを用意する
    {
        if (isBouncing) StopDonutsBounce();
        bounceTimer = 0f;
        isBouncing = true;

        bounceDonutsParent = new GameObject("BounceDonutsParent");//バウンドするドーナツの全体の親を用意
        CopyUnionPosAndScale();

        bounceDonutsAxis = new GameObject("BounceDonutsAxis");//バウンドするドーナツの拡大縮小の中心を用意
        bounceDonutsAxis.transform.parent = bounceDonutsParent.transform;
        bounceDonutsAxis.transform.localPosition = Vector3.zero;
        bounceDonutsAxis.transform.LookAt(lookAtTransform);

        foreach (var sphere in donutSpheres)
        {
            GameObject donutMeshObj = Instantiate(sphere);//既存のドーナツを複製
            donutMeshObj.transform.position = sphere.transform.position;

            DonutSphereColor donutColor = sphere.GetComponent<DonutSphereColor>();//DonutSphereColorの中身を同じにする
            DonutSphereColor bounceDonutColor = donutMeshObj.GetComponent<DonutSphereColor>();
            bounceDonutColor.CopySphereColorValue(donutColor);

            Component[] donutComponents = donutMeshObj.GetComponents<Component>();
            foreach (Component component in donutComponents)//削除して見た目だけにする
            {
                if (!(component is Transform) && !(component is MeshFilter) && !(component is MeshRenderer) && !(component is DonutSphereColor))
                {
                    Destroy(component);
                }
            }
            sphere.GetComponent<MeshRenderer>().enabled = false;//もとのドーナツはいったん非表示
            donutMeshObj.transform.parent = bounceDonutsAxis.transform;
        }
    }
    private void CopyUnionPosAndScale()//コピーしたバウンド用のドーナツの位置を元々のドーナツの位置に更新
    {
        bounceDonutsParent.transform.position = transform.position;
        bounceDonutsParent.transform.rotation = transform.rotation;
    }
    public void StopDonutsBounce()//ドーナツのバウンドのアニメーションを停止する
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
