using UnityEngine;

[RequireComponent(typeof(RunBehaviour))]
[RequireComponent(typeof(AirBehaviour))]
public class GlideBehaviour : PlayerBehaviour
{
    [Header("Planeur - Paramètres")]
    // [SerializeField] private float glideSpeed = 20f;
    // [SerializeField] private float glideDescentRate = -0.56f;
    // [SerializeField] private float glideUpForce = 1.8f;
    // [SerializeField] private float glideTurnMultiplier = 0.7f;
    [SerializeField] private float gravity = -4f;
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float turnSpeed = 200f;

    public override void Init()
    {
        actionManager.AddAction("Jump", context =>
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
        player.gravity = gravity;
        player.moveSpeed = moveSpeed;
        player.turnSpeed = turnSpeed;
        animator.SetBool("Glide", true);
    }

    public override void Exit()
    {
        animator.SetBool("Glide", false);
    }

    public override void Run()
    {
        if (player.isGrounded)
        {
            player.ChangeBehaviour<RunBehaviour>();
        }
    }
}