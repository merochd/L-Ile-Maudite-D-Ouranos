using UnityEngine;

[RequireComponent(typeof(AirBehaviour))]
public class JumpBehaviour : PlayerBehaviour
{
    [SerializeField] public  float jumpPower = 7f;

    public override void Init()
    {
        base.Init();

        player.actionManager.AddAction("Jump", context =>
        {
            if (player.isGrounded)
            {
                player.ChangeBehaviour(this);
                return;
            }
        });
    }

    public override void Enter()
    {
        base.Enter();

        player.animator.SetTrigger("Jump");
        player.rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
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