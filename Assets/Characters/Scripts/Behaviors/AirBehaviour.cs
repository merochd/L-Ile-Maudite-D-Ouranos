using UnityEngine;

[RequireComponent(typeof(RunBehaviour))]
public class AirBehaviour : PlayerBehaviour
{
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float turnSpeed = 50f;

    public override void Enter()
    {
        player.gravity = gravity;
        player.moveSpeed = moveSpeed;
        player.turnSpeed = turnSpeed;
    }

    public override void Exit()
    {
    }

    public override void Init()
    {
    }

    public override void Run()
    {
        if (player.isGrounded)
        {
            player.ChangeBehaviour<RunBehaviour>();
        }
    }
}