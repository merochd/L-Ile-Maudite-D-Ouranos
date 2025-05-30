using UnityEngine;

[RequireComponent(typeof(RunBehaviour))]
[RequireComponent(typeof(AirBehaviour))]
public class GlideBehaviour : PlayerBehaviour
{
    [Header("Planeur - Paramètres")]
    [SerializeField] private float gravity = 1f;
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float turnSpeed = 200f;

    public override void Init()
    {
        base.Init();

        player.actionManager.AddAction("Jump", context =>
        {
            if (!player.isGrounded)
            {
                if (player.currentBehaviour == this)
                {
                    player.ChangeBehaviour<AirBehaviour>();
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

        player.gravity = gravity;
        player.moveSpeed = moveSpeed;
        player.turnSpeed = turnSpeed;
        player.animator.SetBool("Glide", true);
    }

    public override void Exit()
    {
        base.Exit();

        player.animator.SetBool("Glide", false);
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