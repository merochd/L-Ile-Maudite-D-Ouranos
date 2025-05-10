using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Mouvement - Vitesse")]
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float turnSpeed = 270f;
    [SerializeField] private float jumpPower = 7.5f;
    [SerializeField] private float gravityStrength = -15f;

    [Header("Planeur - Paramètres")]
    [SerializeField] private float glideSpeed = 20f;
    [SerializeField] private float glideDescentRate = -0.56f;
    [SerializeField] private float glideUpForce = 1.8f;
    [SerializeField] private float glideTurnMultiplier = 0.7f;
    [SerializeField] private float glideGravityScale = 0.15f;

    [Header("Saut & Sol")]
    [SerializeField] private LayerMask groundLayer = 1;
    [SerializeField] private LayerMask walkableWallLayer;
  

    [Header("Références")]
    private Rigidbody rb;
    private Animator animator;
    private InputManager input;
    [SerializeField] private Transform cameraTransform;

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
    [SerializeField] private float fallSpeedThreshold = -12f;
    [SerializeField] private float fallHeightThreshold = 6f;
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
        if (gravityState)
            GravityPower();
    }

    private void CheckGround()
    {
        Vector3 origin = transform.position + transform.up * 0.3f;
        if (Physics.Raycast(origin, -transform.up, out RaycastHit hit, 0.6f, groundLayer))
        {
            isGrounded = true;
            gravityState = true;
            rb.useGravity = false ;
            transform.rotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;

            if (wasFalling && !isGliding)
            {
                animator.SetTrigger("FallImpact");
                StartCoroutine(TriggerGetUpAfterDelay(1f));
            }

            wasFalling = false;
            isGliding = false;
            animator.SetBool("Gliding", false);
            isJumping = false;
        }
        else
        {
            isGrounded = false;
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

        smoothedMoveX = Mathf.Lerp(smoothedMoveX, input.move.x, Time.deltaTime * 7f);
        smoothedMoveY = Mathf.Lerp(smoothedMoveY, input.move.y, Time.deltaTime * 7f);

        float moveSpeed = isSprinting ? sprintSpeed : isCrouching ? crouchSpeed : runSpeed;
        smoothedMoveSpeed = Mathf.Lerp(smoothedMoveSpeed, moveSpeed, Time.deltaTime * 2f);

        float turn = smoothedMoveX * turnSpeed * Time.deltaTime;
        transform.rotation *= Quaternion.AngleAxis(turn, Vector3.up);

        float moveForward = smoothedMoveY * smoothedMoveSpeed * Time.deltaTime *1.2f ;
        transform.position += transform.forward * moveForward;
    }


    private void ApplyGravity()
    {
        float gravityForce = isGliding
            ? gravityStrength * glideGravityScale
            : (isGrounded ? gravityStrength : gravityStrength * 2f);
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
        else if (!isGrounded && !isGliding && rb.linearVelocity.y <= 0f)
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
        if (isGrounded) return;
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
        if (!gravityState || isJumping)
            return;

        RaycastHit hit;
        Vector3 origin = transform.position + transform.up * 0.7f;
        Vector3 dir    = (transform.forward - transform.up).normalized;
        float maxDist  = 1.8f;
        
        


      
        if (Physics.Raycast(origin, dir, out hit, maxDist, walkableWallLayer))
        {
            Wall wall = hit.collider.GetComponent<Wall>();
            if (wall != null && wall.isWalkable== false) return;
            else
            {
                DoRotateToNormal(hit.normal);
                onWall = true;
                animator.SetBool("isWall", onWall =true);
                rb.useGravity = true ;
                StopGlide();
            }
        }
        else
        {
          DoRotateToNormal(Vector3.up);
          rb.useGravity = false ;
          StopGlide();
          
        }
        
        
    }

    private void DoRotateToNormal(Vector3 normal)
    {
          if (Vector3.Angle(transform.up, normal) < 1f)
            return;

        Quaternion endRotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
        transform.DORotateQuaternion(endRotation, 1f);

        
    }

    private IEnumerator TriggerGetUpAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetTrigger("GetUp");
    }

    private void StopGlide()
    {
        if (onWall== true){ 
        isGliding = false;
        animator.SetBool("Gliding", false);
        animator.SetBool("IsFalling", false );
        }
        
      }    
} 
