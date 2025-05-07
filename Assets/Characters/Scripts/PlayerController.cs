using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Mouvement - Vitesse")]
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float runSpeed = 4.5f;
    [SerializeField] private float sprintSpeed = 7.5f;
    [SerializeField] private float turnSpeed = 150f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float gravityStrength = -20f;

    [Header("Planeur - Paramètres")]
    [SerializeField] private float glideSpeed = 8f;
    [SerializeField] private float glideDescentRate = -1.5f;
    [SerializeField] private float glideUpForce = 4f;
    [SerializeField] private float glideTurnMultiplier = 1.2f;
    [SerializeField] private float glideGravityScale = 0.3f;


    [Header("Saut & Sol")]
    [SerializeField] private LayerMask groundLayer = 1;

    [SerializeField] private LayerMask walkableWallLayer;

    
    [SerializeField] private float doubleJumpThreshold = 0.25f;

    [Header("Références")]
    private Rigidbody rb;
    private Animator animator;
    private InputManager input;

    [Header("États du Joueur")]
    private bool isGrounded;
    private bool isJumpBeginning;
    private bool isJumping;
    private bool isGliding;
    private bool isCrouching;
    private bool gravityState;
    private bool wasFalling = false;

    private bool onWall;


    [Header("Interpolations Mouvement")]
    private float smoothedMoveX;
    private float smoothedMoveY;
    private float smoothedMoveSpeed;

    [Header("Timers Internes")]
    private float lastJumpPressTime = -1f;

[SerializeField] private float fallSpeedThreshold = -5f; 
[SerializeField] private float fallHeightThreshold = 3f; 
private float fallStartY;     

void Awake()
    {
        input = GameManager.input;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
    }

    void FixedUpdate()
    {
        ApplyGravity();
        MovementCharacter();
    }
    void Update()
    {
        CheckGround();
        AnimatorStates();
        HandleFallingState();

        if (Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.JoystickButton2))
        ToggleGravity();
        if (gravityState) GravityPower();
    }

    private void CheckGround()
{
    Vector3 origin = transform.position + transform.up * 0.3f;
    isGrounded = Physics.Raycast(origin, -transform.up, out RaycastHit hit, 0.6f, groundLayer | walkableWallLayer);

    if (isGrounded)
    {
        if (wasFalling && !isGliding)
        {
            animator.SetTrigger("FallImpact");
            StartCoroutine(TriggerGetUpAfterDelay(1f));
        }

        wasFalling = false;
        isGliding = false;
        animator.SetBool("Gliding", false);

        if (!isJumpBeginning) isJumping = false;
    }
}

    private void HandleFallingState()
{
    if (!isGrounded && !isJumping && !isGliding)
    {
        
        if (!wasFalling && rb.linearVelocity.y < fallSpeedThreshold)
        {
            fallStartY = transform.position.y;
            wasFalling = true;
        }

        animator.SetBool("IsFalling", wasFalling);
        
    }
    else
    {
        if (wasFalling)
        {
            float fallDistance = fallStartY - transform.position.y;
            if (fallDistance > fallHeightThreshold)
            {
                animator.SetTrigger("FallImpact");
                StartCoroutine(TriggerGetUpAfterDelay(1f));
            }
        }

        wasFalling = false;
        animator.SetBool("IsFalling", false);
    }
}

    private void MovementCharacter()
    {
        if (isGliding)
        {
            GlideUpdate();
            return;
        }

        isCrouching = input.isCrouching;
        bool isSprinting = input.isSprinting;

        smoothedMoveX = Mathf.Lerp(smoothedMoveX, input.move.x, Time.deltaTime * 5f);
        smoothedMoveY = Mathf.Lerp(smoothedMoveY, input.move.y, Time.deltaTime * 5f);

        float moveSpeed = isSprinting ? sprintSpeed : isCrouching ? crouchSpeed : runSpeed;
        smoothedMoveSpeed = Mathf.Lerp(smoothedMoveSpeed, moveSpeed, Time.deltaTime * 5f);

        float turn = smoothedMoveX * turnSpeed * Time.deltaTime;
        transform.rotation *= Quaternion.AngleAxis(turn, Vector3.up);

        float moveForward = smoothedMoveY * smoothedMoveSpeed * Time.deltaTime;
        transform.position += transform.forward * moveForward;
    }
     private void ApplyGravity()
    {
        float gravityForce = isGliding ? gravityStrength * glideGravityScale : (isGrounded ? gravityStrength : gravityStrength * 2f);
        rb.AddForce(transform.up * gravityForce, ForceMode.Acceleration);
    }

    private void AnimatorStates()
    {
        animator.SetFloat("Forward", smoothedMoveY * smoothedMoveSpeed);
        animator.SetFloat("Turn", smoothedMoveX);
        animator.SetBool("Crouch", isCrouching);
        animator.SetBool("OnGround", isGrounded);
        animator.SetBool("Gliding", isGliding);
    }

   public void Jump()
{
    if (isGrounded)
    {
        isJumpBeginning = true;
        isJumping = true;
        DOVirtual.DelayedCall(0.1f, () => isJumpBeginning = false);

        rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
        lastJumpPressTime = Time.time;
    }
    else if (!isGliding && !isJumpBeginning && rb.linearVelocity.y <= 0f && Time.time - lastJumpPressTime <= doubleJumpThreshold)
    {

        ToggleGlide();
        lastJumpPressTime = -1f;
    }
    else
    {
        lastJumpPressTime = Time.time;
    }
}


public void ToggleGlide()
{
    if (isGrounded)
        return;

    isGliding = !isGliding;
    animator.SetBool("Gliding", isGliding);
    animator.SetBool("IsFalling", !isGliding);
}



    public void GlideUpdate()
    {
        if (!isGliding) return;

        smoothedMoveX = Mathf.Lerp(smoothedMoveX, input.move.x, Time.deltaTime * 10f);
        smoothedMoveY = Mathf.Lerp(smoothedMoveY, input.move.y, Time.deltaTime * 10f);

        Vector3 moveDirection = (transform.forward * smoothedMoveY + transform.right * smoothedMoveX).normalized;
        Vector2 horVel = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);
        Vector2 desiredMove = new Vector2(moveDirection.x, moveDirection.z) * glideSpeed;
        Vector2 offsetVel = desiredMove - horVel;
      
        rb.AddForce(new Vector3(offsetVel.x, 0, offsetVel.y), ForceMode.Acceleration);
        if (rb.linearVelocity.y < glideDescentRate)
            rb.AddForce(Vector3.up * glideUpForce);

        float turn = smoothedMoveX * turnSpeed * Time.deltaTime * glideTurnMultiplier;
        transform.rotation *= Quaternion.AngleAxis(turn, Vector3.up);      
        
    }

    public void ToggleGravity()
    {
        gravityState = !gravityState;
        Debug.Log("Gravité " + (gravityState ? "activée" : "désactivée"));
    }

 public void GravityPower()
{
    if (!gravityState || isJumping) return;

    RaycastHit hit;
    Vector3 origin = transform.position + transform.up * 0.7f;
    Vector3 dir = (transform.forward - transform.up).normalized;

    // 1) On lance d’abord le raycast
    if (Physics.Raycast(origin, dir, out hit, 0.8f, walkableWallLayer))
    {
        // 2) On récupère le script Wall sur le collider touché
        Wall wall = hit.collider.GetComponent<Wall>();
        if (wall == null || wall.isWalkable == false)
            return; // pas de mur « walkable », on sort

        // 3) On applique la rotation vers la normale
        DoRotateToNormal(hit.normal);
    }
}


private void DoRotateToNormal(Vector3 normal)
{
    if (Vector3.Angle(transform.up, normal) < 1f) return;

    Quaternion endRotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
    transform.DORotateQuaternion(endRotation, 0.25f);
}

    private IEnumerator TriggerGetUpAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetTrigger("GetUp");
    }

}

