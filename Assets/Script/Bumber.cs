using UnityEngine;

public class Bumper : MonoBehaviour
{
    public float bounceForce = 50f;

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null)
        {
            rb.AddForce(Vector3.up * bounceForce, ForceMode.VelocityChange);
        }
    }
}