using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutSphereReference : MonoBehaviour
{
    [Tooltip("自分の子の印オブジェクト")]
    [SerializeField] GameObject Mark;

    // Start is called before the first frame update
    void Start()
    {
        if(Mark == null)
        {
            Mark = transform.GetChild(0).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayerEnter()
    {
        if (!Mark.activeSelf)
        {
            Mark.SetActive(true);
            Debug.Log("marked");
        }
    }
}
