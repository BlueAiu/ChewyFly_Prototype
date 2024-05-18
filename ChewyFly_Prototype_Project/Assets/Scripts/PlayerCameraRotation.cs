using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraRotation : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    InputScript input;

    [Tooltip("ÉJÉÅÉâÇÃâÒì]ÇÃë¨Ç≥")]
    [SerializeField] float sensityvity = 180f;

    // Start is called before the first frame update
    void Start()
    {
        if (cam == null)
            cam = GameObject.Find("Main Camera");
        input = GetComponent<InputScript>();
    }

    // Update is called once per frame
    void Update()
    {
        float rot = 0;
        if (input.isRotateCameraRight())
            rot++;
        if (input.isRotateCameraLeft())
            rot--;
        rot = rot * sensityvity * Time.deltaTime;
        transform.Rotate(Vector3.up, rot);
    }
}
