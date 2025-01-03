using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RayTransparency : MonoBehaviour
{
    [SerializeField] GameObject player;
    Vector3 targetPosition;
    float targetOffsetYFoot = 0.1f;
    float targetOffsetYHead = 1.6f;

    public GameObject[] prevRaycast;
    public List<GameObject> raycastHitsList_ = new();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        prevRaycast = raycastHitsList_.ToArray();
        raycastHitsList_.Clear();
        targetPosition = player.transform.position;
        targetPosition.y += targetOffsetYFoot;
        Vector3 _difference = (targetPosition - this.transform.position);
        RayCastHit(_difference);

        foreach(var i in prevRaycast.Except<GameObject>(raycastHitsList_))
        {
            var noSampleMaterial = i.GetComponent<RayhitedObject>();
            if (i != null)
            {
                noSampleMaterial.NotClearMaterialInvoke();
            }
        }
    }

    public void RayCastHit(Vector3 _difference)
    {
        var _direction = _difference.normalized;

        Ray _ray = new Ray(this.transform.position, _direction);
        RaycastHit[] raycastHits = Physics.RaycastAll(_ray);

        foreach(var i in raycastHits)
        {
            float distance = Vector3.Distance(i.point,transform.position);
            if(distance < _difference.magnitude)
            {
                var hited = i.collider.GetComponent<RayhitedObject>();
                if (i.collider.tag == "Fadeobj")
                {
                    hited.ClearMaterialInvoke();
                    raycastHitsList_.Add(i.collider.gameObject);
                }
            }
        }
    }
}
