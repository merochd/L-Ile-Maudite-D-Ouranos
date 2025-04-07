using UnityEngine;

public class WallFalling : MonoBehaviour
{

    // l'object qui a se code tombera dans le vide et se destroy quand il n'est plus visible à la caméra
    public float fallDistance = 5f; // Distance de chute
    public float fallSpeed = 2f; // Vitesse de la chute
    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition - new Vector3(0, fallDistance, 0);
    }


    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}