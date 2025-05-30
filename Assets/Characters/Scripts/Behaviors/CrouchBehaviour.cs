using UnityEngine;

[RequireComponent(typeof(RunBehaviour))]
[RequireComponent(typeof(AirBehaviour))]
public class CrouchBehaviour : PlayerBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float turnSpeed = 270f;

    public override void Init()
    {
        base.Init();

        player.actionManager.AddAction("Crouch", context =>
        {
            if (player.isGrounded)
            {
                if (player.currentBehaviour == this)
                {
                    player.ChangeBehaviour<RunBehaviour>();
                }
                else
                {
                    player.ChangeBehaviour(this);
                }
            }
        });
    }

    public override void Enter()
    {
        base.Enter();

        player.moveSpeed = moveSpeed;
        player.turnSpeed = turnSpeed;
        player.animator.SetBool("Crouch", true);
    }

    public override void Exit()
    {
        base.Exit();

        player.animator.SetBool("Crouch", false);
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