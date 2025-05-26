using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject followTarget;
    public float maxDistance = 3f;
    public float nearDistance = 0.4f;

    

    void Start()
    {
        
    }


    void Update()
    {
        var followTransform = followTarget.transform;
        var followPoint = followTransform.position;
        var followDir = followTransform.forward * -maxDistance;
        var distance = maxDistance;

        Debug.DrawRay(followPoint, followDir, Color.red, 0.1f);

        if (Physics.Raycast(followPoint, followDir, out RaycastHit hit, maxDistance))
        {
            distance = hit.distance + nearDistance;
        }

        Debug.DrawRay(followPoint, followDir.normalized * distance, Color.green, 0.1f);

        transform.position = followPoint + followDir.normalized * distance;

        Vector3 lookDir = followPoint - transform.position;
        transform.rotation = Quaternion.LookRotation(lookDir, followTransform.up);
    }

    
}