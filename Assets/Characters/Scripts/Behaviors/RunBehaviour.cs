using UnityEngine;

public class RunBehaviour : PlayerBehaviour
{
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float turnSpeed = 270f;

    public bool isGrounded;

    public override void Enter()
    {
        base.Enter();

        if (!player.isGrounded)
        {
            player.ChangeBehaviour<AirBehaviour>();
            return;
        }

        player.gravity = gravity;
        player.moveSpeed = moveSpeed;
        player.turnSpeed = turnSpeed;
        player.animator.SetBool("Run", true);
        player.animator.SetBool("Air",false );
    
    }

    public override void Exit()
    {
        base.Exit();

        player.animator.SetBool("Run", false);
        
    }

    public override void Run()
    {
        if (!player.isGrounded)
        {
            player.ChangeBehaviour<AirBehaviour>().Run();
            return;
        }

        base.Run();
    }
}