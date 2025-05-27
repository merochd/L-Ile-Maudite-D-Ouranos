using UnityEngine;

[RequireComponent(typeof(AirBehaviour))]
public class JumpBehaviour : PlayerBehaviour
{
    [SerializeField] public  float jumpPower = 7f;

    public override void Init()
    {
        actionManager.AddAction("Jump", context =>
        {
            
            if (player.isGrounded && player.currentBehaviour != this)
            {
                player.ChangeBehaviour(this);
            }
        });
    }

    public override void Enter()
    {
        animator.SetTrigger("Jump");
        rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
    }

    public override void Exit()
    {

    }

    public override void Run()
    {
        if (!player.isGrounded)
        {
            player.ChangeBehaviour<AirBehaviour>();
        }
    }
}