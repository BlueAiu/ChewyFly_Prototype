using UnityEngine;

public class RayhitedObject : MonoBehaviour
{
    Color color = Color.white;

    MeshRenderer[] meshRenderers;
    MaterialPropertyBlock m_mpb;

    public MaterialPropertyBlock mpb
    {
        get { return m_mpb ?? (m_mpb = new MaterialPropertyBlock()); }
    }

    private void Awake()
    {
        meshRenderers = this.GetComponentsInChildren<MeshRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearMaterialInvoke()
    {
        color.a = 0.25f;

        mpb.SetColor(Shader.PropertyToID("_Color"), color);
        for(int i=0;i<meshRenderers.Length;i++)
        {
            meshRenderers[i].GetComponent<Renderer>().material.shader = Shader.Find("Transmitted");
            meshRenderers[i].SetPropertyBlock(mpb);
        }
    }

    public void NotClearMaterialInvoke()
    {
        color.a = 1f;
        mpb.SetColor(Shader.PropertyToID("_Color"), color);
        for(int i=0;i<meshRenderers.Length; i++)
        {
            meshRenderers[i].GetComponent<Renderer>().material.shader = Shader.Find("Standard");
            meshRenderers[i].SetPropertyBlock(mpb);
        }
    }
}
