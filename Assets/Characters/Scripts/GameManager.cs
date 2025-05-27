using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null) _instance = FindAnyObjectByType<GameManager>().GetComponent<GameManager>();
            return _instance;
        }
    }

    private static PlayerController _player;
    public static PlayerController player
    {
        get
        {
            if (_player == null) _player = FindAnyObjectByType<PlayerController>().GetComponent<PlayerController>();
            return _player;
        }
    }
}
