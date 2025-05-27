using UnityEngine;

public class RunBehaviour : PlayerBehaviour
{
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float turnSpeed = 270f;

    public override void Init()
    {
        animator = GetComponent<Animator>();
    }

    public override void Enter()
    {
        player.gravity = gravity;
        player.moveSpeed = moveSpeed;
        player.turnSpeed = turnSpeed;
        animator.SetBool("Run", true);
    }

    public override void Exit()
    {
        animator.SetBool("Run", false);
    }

    public override void Run()

    {
    
        if (!player.isGrounded)
        {
            player.ChangeBehaviour<AirBehaviour>();
        }
    }
}