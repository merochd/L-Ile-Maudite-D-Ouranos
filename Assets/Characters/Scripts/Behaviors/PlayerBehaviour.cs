using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ActionManager))]
[RequireComponent(typeof(PlayerController))]
public abstract class PlayerBehaviour : MonoBehaviour
{
    protected PlayerController player;
    protected ActionManager actionManager;
    protected Rigidbody rb;
    protected Animator animator;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        actionManager = GetComponent<ActionManager>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    public abstract void Init();
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Run();
}