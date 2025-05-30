using UnityEngine;

[RequireComponent(typeof(RunBehaviour))]
public class AirBehaviour : PlayerBehaviour
{
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float turnSpeed = 50f;
    
    public override void Enter()
    {
        base.Enter();

        player.gravity = gravity;
        player.moveSpeed = moveSpeed;
        player.turnSpeed = turnSpeed;
        player.animator.SetBool("Air", true);


    }

    public override void Exit()
    {
        base.Exit();

        player.animator.SetBool("Air", false);
    }

    public override void Run()
    {
        if (player.isGrounded)
        {
            player.ChangeBehaviour<RunBehaviour>().Run();
            return;
        }

        base.Run();
    }
}