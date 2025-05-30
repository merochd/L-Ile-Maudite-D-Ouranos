using UnityEngine;
using UnityEngine.InputSystem;
using System;

public enum ActionState
{
    Started,
    Canceled,
    Performed,
}

[RequireComponent(typeof(RunBehaviour))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] public GameObject followTarget;

    [Header("Mouvement - Vitesse")]
    public float gravity;
    public float moveSpeed;
    public float turnSpeed;
    public float acceleration = 20f;
    public float smoothedTurn;

    // private float smoothedMoveX;
    // private float smoothedMoveY;
    // private float smoothedMoveSpeed;
    // private float smoothedTurnSpeed;

    [Header("États du Joueur")]
    public PlayerBehaviour currentBehaviour { get; private set; }
    public bool isGrounded { get; private set; }
    public bool isAnimated = false;

    // [Header("Mouvement - Vitesse")]
    // [SerializeField] private float crouchSpeed = 2.5f;
    // [SerializeField] private float runSpeed = 6f;
    // [SerializeField] private float sprintSpeed = 9f;
    // [SerializeField] private float turnSpeed = 270f;

    // [SerializeField] public bool isGrounded = true;
    // [SerializeField] private bool isJumpStarting = false;
    // [SerializeField] private bool isJumping = false;
    // [SerializeField] private bool isCrouching = false;
    // [SerializeField] private bool isGliding = false;
    // [SerializeField] private bool isSprinting = false;

    // private bool jumpPressed = false;

    // private bool onWall;

    // [Header("Interpolations Mouvement")]
    // private float smoothedMoveSpeed;

    // Références
    public ActionManager actionManager;
    public Rigidbody rb;
    public Animator animator;
    public Vector2 move = Vector2.zero;
    public Vector2 look = Vector2.zero;

    private PlayerBehaviour[] attachedBehaviors;
    public float velocityToAnim = 0.5f;

    void Awake()
    {
        actionManager = GetComponent<ActionManager>();

        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        actionManager.AddAction("Move", ActionState.Started, OnMove);
        actionManager.AddAction("Look", ActionState.Started, OnLook);

        actionManager.AddAction("Move", ActionState.Performed, OnMove);
        actionManager.AddAction("Look", ActionState.Performed, OnLook);

        actionManager.AddAction("Move", ActionState.Canceled, OnMove);
        actionManager.AddAction("Look", ActionState.Canceled, OnLook);

        attachedBehaviors = GetComponents<PlayerBehaviour>();
        foreach (var behaviour in attachedBehaviors)
            behaviour.Init();

        ChangeBehaviour<RunBehaviour>();
    }

    void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    void OnLook(InputAction.CallbackContext context)
    {
        look = Mouse.current.delta.ReadValue();
    }

    public T ChangeBehaviour<T>() where T : PlayerBehaviour
    {
        var behaviour = GetComponent<T>();

        ChangeBehaviour(behaviour);

        return behaviour;
    }

    public PlayerBehaviour ChangeBehaviour(PlayerBehaviour behaviour)
    {
        if (currentBehaviour == behaviour)
            return behaviour;

        if (!behaviour)
            {
                Debug.LogError("ChangeBehaviour", behaviour);
                throw new Exception($"ChangeBehaviour error {behaviour}");
            }

        Debug.Log($"ChangeBehaviour {currentBehaviour?.GetType().Name} -> {behaviour.GetType().Name}");

        currentBehaviour?.Exit();
        currentBehaviour = behaviour;
        behaviour.Enter();

        return behaviour;
    }

    public bool CheckGround()
    {
        Vector3 origin = transform.position + transform.up * 0.3f;
        if (Physics.Raycast(origin, -transform.up, out RaycastHit hit, 0.6f))
        {
            isGrounded = true;
            return true;
            
        }
        else
        {
            isGrounded = false;
            return false;
        }
    }

    void FixedUpdate()
    {
        currentBehaviour.Run();
    }

    public void Move()
    {
        // ApplyGravity
        // rb.AddForce(transform.up * gravity, ForceMode.Acceleration);

        Vector3 inputDirection = move.x * transform.right + move.y * transform.forward;

        // Direction désirée
        Vector3 desiredVelocity = inputDirection.normalized * moveSpeed;

        // Vitesse actuelle projetée sur le plan horizontal right et forward
        Vector3 flatVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, transform.up);

        // Calcul de la différence de vitesse
        Vector3 velocityDelta = desiredVelocity - flatVelocity;

        // Limite l'accélération maximale
        Vector3 force = Vector3.ClampMagnitude(
            velocityDelta * rb.mass / Time.fixedDeltaTime,
            acceleration * rb.mass
        );

        Vector3 gravityForce = transform.up * gravity;

        rb.AddForce(force + gravityForce, ForceMode.Acceleration);

        // smoothedMoveX = Mathf.Lerp(smoothedMoveX, move.x, Time.deltaTime * 10f);
        // smoothedMoveY = Mathf.Lerp(smoothedMoveY, move.y, Time.deltaTime * 10f);
        // 

        // float moveRight = smoothedMoveX * moveSpeed * Time.deltaTime;
        // float moveForward = smoothedMoveY * moveSpeed * Time.deltaTime;

        // transform.position += transform.forward * moveForward + transform.right * moveRight;

        smoothedTurn = Mathf.Lerp(smoothedTurn, look.x, Time.deltaTime * 10f);
        float turn = smoothedTurn * turnSpeed * Time.deltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.AngleAxis(turn, Vector3.up));
        // transform.rotation *= Quaternion.AngleAxis(turn, Vector3.up);

        var velocity = transform.InverseTransformDirection(rb.linearVelocity) * velocityToAnim;

        var forward = Mathf.Round(velocity.z);
        var right = Mathf.Round(velocity.x);
        var up = velocity.y;
        
        Debug.DrawRay(transform.position, forward * transform.forward, Color.blue, 0.1f);
        Debug.DrawRay(transform.position, right * transform.right, Color.red, 0.1f);
        Debug.DrawRay(transform.position, up * transform.up, Color.cyan, 0.1f);

        // Debug.Log($"Forward = {forward} ; Right = {right}; Up = {up}; Turn={turn}");

        animator.SetFloat("Forward", forward);
        animator.SetFloat("Right", right);
        animator.SetFloat("Up", up);
        animator.SetFloat("Turn", turn);
    }

    // private void Move()
    // {
    //     // ApplyGravity
    //     rb.AddForce(transform.up * gravity, ForceMode.Acceleration);

    //     smoothedMoveX = Mathf.Lerp(smoothedMoveX, move.x, Time.deltaTime * 10f);
    //     smoothedMoveY = Mathf.Lerp(smoothedMoveY, move.y, Time.deltaTime * 10f);
    //     smoothedTurn = Mathf.Lerp(smoothedTurn, look.x, Time.deltaTime * 10f);

    //     float moveRight = smoothedMoveX * moveSpeed * Time.deltaTime;
    //     float moveForward = smoothedMoveY * moveSpeed * Time.deltaTime;
    //     float turn = smoothedTurn * turnSpeed * Time.deltaTime;

    //     transform.rotation *= Quaternion.AngleAxis(turn, Vector3.up);
    //     transform.position += transform.forward * moveForward + transform.right * moveRight;

    //     animator.SetFloat("Forward", move.y);
    //     animator.SetFloat("Right", move.x);
    //     animator.SetFloat("Turn", turn);
    // }
    
    // private void SyncAnimatorStates()
    // {
    //     animator.SetFloat("Forward", smoothedMoveY);
    //     animator.SetFloat("Turn", smoothedMoveX);
    //     animator.SetBool("OnGround", isGrounded);
    // }

    // public void Jump()
    // {
    //     Debug.Log($"Jump isGrounded={isGrounded}");

    //     if (isGrounded)
    //     {
    //         isJumping = true;
    //         rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
    //     }
    //     else
    //     {
    //         isGliding = !isGliding;
    //     }
    // }


    // public void GlideUpdate()
    // {
    //     Debug.Log($"GlideUpdate isGliding={isGliding} isGrounded={isGrounded}");

    //     if (isGliding == false)
    //     {
    //         return;
    //     }

    //     if (isGrounded)
    //     {
    //         isGliding = false;
    //         return;
    //     }

    //     smoothedMoveX = Mathf.Lerp(smoothedMoveX, move.x, Time.deltaTime * 25f);
    //     smoothedMoveY = Mathf.Lerp(smoothedMoveY, move.y, Time.deltaTime * 25f);

    //     Vector3 moveDirection = (transform.forward * smoothedMoveY + transform.right * smoothedMoveX).normalized;
    //     Vector2 currentHorVel = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);
    //     Vector2 desiredHorVel = new Vector2(moveDirection.x, moveDirection.z) * glideSpeed;
    //     Vector2 diff = desiredHorVel - currentHorVel;

    //     rb.AddForce(new Vector3(diff.x, 0, diff.y), ForceMode.Acceleration);

    //     if (rb.linearVelocity.y < glideDescentRate)
    //         rb.AddForce(Vector3.up * glideUpForce, ForceMode.Acceleration);

    //     float turnAmount = smoothedMoveX * turnSpeed * Time.deltaTime * glideTurnMultiplier;
    //     transform.rotation *= Quaternion.AngleAxis(turnAmount, Vector3.up);
    // }



    // public void ToggleCrouch()
    // {
    //     isCrouching = !isCrouching;
    // }

    // public void SetSprinting(bool value)
    // {
    //     isSprinting = value;
    // }

    // public void SetMove(Vector2 value)
    // {
    //     move = value;
    // }

    // public void SetLook(Vector2 value)
    // {
    //     look = value;
    // }

    // public bool IsJumpPressed()
    // {
    //     return jumpPressed;
    // }

    // public void PressJump()
    // {
    //     jumpPressed = true;
    // }

    // public bool IsGrounded()
    // {
    //     return isGrounded; // Met à jour avec un Raycast comme tu le fais déjà
    // }

}
