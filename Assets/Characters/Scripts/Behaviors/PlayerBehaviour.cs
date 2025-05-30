using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public abstract class PlayerBehaviour : MonoBehaviour
{
    protected PlayerController player;

    void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    public virtual void Init()
    {
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void Run()
    {
        player.CheckGround();
        player.Move();
    }
}