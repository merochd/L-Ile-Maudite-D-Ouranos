using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] float maxDistance = 3f;
    [SerializeField] float nearDistance = 0.4f;
    [SerializeField] float walkShakeAmount = 1f;
    [SerializeField] float runShakeAmount = 2.5f;
    [SerializeField] private float pitchSensitivity = 2f;
    [SerializeField] private float pitchMin = -40f;
    [SerializeField] private float pitchMax = 40f;

    private float pitch = 0f;

    float PerlinNoise(float x, float y)
    {
        return (Mathf.PerlinNoise(x, y) - 0.5f) * 2f;
    }

    void Update()
    {
        // input.look.y
        var player = GameManager.player;
        var followTransform = player.followTarget.transform;

        pitch -= player.look.y * pitchSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax); // limiter l'angle vertical

        followTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        var followPoint = followTransform.position;
        var followDir = followTransform.forward * -maxDistance;
        var distance = maxDistance;

        // Debug.DrawRay(followPoint, followDir, Color.red, 0.1f);

        if (Physics.Raycast(followPoint, followDir, out RaycastHit hit, maxDistance))
        {
            distance = hit.distance + nearDistance;
        }

        // Debug.DrawRay(followPoint, followDir.normalized * distance, Color.green, 0.1f);

        Vector3 targetPosition = followPoint + followDir.normalized * distance;

        Vector3 pos = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 3f);
        // if((pos - followTransform.position).sqrMagnitude <= 5)

        transform.position = pos;

        Vector3 lookDir = followPoint - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(lookDir, player.transform.up);

        // Debug.DrawRay(followPoint, Vector3.up, Color.red, 0.1f);
        // Debug.DrawRay(followPoint, followTransform.up, Color.yellow, 0.1f);

        // Génère un petit shake naturel avec Perlin Noise
        float shakeAmount = walkShakeAmount;
        float shakeSpeed = shakeAmount * 2f;
        float t = Time.time * shakeSpeed;
        Quaternion shakeRotation = Quaternion.Euler(new Vector3(
            PerlinNoise(t, 0f) * shakeAmount * 0.8f,
            PerlinNoise(t, 1f) * shakeAmount * 1.2f,
            PerlinNoise(t, 2f) * shakeAmount * 0.5f
        ));

        transform.rotation = targetRotation * shakeRotation;
    }
}



// [SerializeField] float shakeAmount = 0.5f;
// [SerializeField] float shakeSpeed = 1.0f;
// private Vector3 shakeOffset;

// void Update()
// {
//     var followTransform = followTarget.transform;
//     var followPoint = followTransform.position;
//     var followDir = followTransform.forward * -maxDistance;
//     var distance = maxDistance;

//     Debug.DrawRay(followPoint, followDir, Color.red, 0.1f);

//     if (Physics.Raycast(followPoint, followDir, out RaycastHit hit, maxDistance))
//     {
//         distance = hit.distance + nearDistance;
//     }

//     Debug.DrawRay(followPoint, followDir.normalized * distance, Color.green, 0.1f);

//     transform.position = followPoint + followDir.normalized * distance;

//     Vector3 lookDir = followPoint - transform.position;
//     Quaternion targetRotation = Quaternion.LookRotation(lookDir, followTransform.up);

//     // Génère un petit shake naturel avec Perlin Noise
//     float t = Time.time * shakeSpeed;
//     shakeOffset.x = (Mathf.PerlinNoise(t, 0.0f) - 0.5f) * 2f * shakeAmount;
//     shakeOffset.y = (Mathf.PerlinNoise(0.0f, t) - 0.5f) * 2f * shakeAmount;
//     shakeOffset.z = (Mathf.PerlinNoise(t, t) - 0.5f) * 2f * shakeAmount * 0.5f; // moins sur Z

//     Quaternion shakeRotation = Quaternion.Euler(shakeOffset);
//     transform.rotation = targetRotation * shakeRotation;
// }