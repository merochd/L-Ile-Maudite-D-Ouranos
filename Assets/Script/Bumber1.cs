using UnityEngine;

public class Bumper : MonoBehaviour
{
    public float jumpForceAdded;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            JumpBehaviour scriptJumpPlayer = other.gameObject.GetComponent<JumpBehaviour>();
            scriptJumpPlayer.jumpPower += jumpForceAdded;
            Debug.Log("on a bumper");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            JumpBehaviour scriptJumpPlayer = other.gameObject.GetComponent<JumpBehaviour>();
            scriptJumpPlayer.jumpPower -= jumpForceAdded;
            Debug.Log("off the bumper");
        }
    }
}