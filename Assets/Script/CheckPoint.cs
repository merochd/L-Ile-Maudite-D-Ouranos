using UnityEngine;

public class CheckPoint : MonoBehaviour
{

    // Ce script permet de faire spawner le joueur au début du jeu, et de respawn lorsqu'il meurt.
    // les check point ont besoin d'un collider que le joueur peut toucher, cela va faire changer de place le spawnPoint
    //Le spawn a besoin du tag "PlayerSpawn", avec ce script, le playerspawn changera en fonction de quel checkpoint on a atteint en dernier
    private Transform playerSpawn;

    private void Awake()
    {
        GameObject spawnObject = GameObject.FindWithTag("PlayerSpawn");
        if (spawnObject != null)
        {
            playerSpawn = spawnObject.transform;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerSpawn.position = transform.position;
            Destroy(gameObject);
        }
    }
}