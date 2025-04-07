using UnityEngine;
using DG.Tweening;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer = 1;

    private float groundRayOriginHeight = 1.0f;
    private float groundRayLength = 1.10f;
    private float jumpPower = 20f;
    private float gravityStrength = -9.8f;
    private float crouchSpeed = 2f;
    private float runSpeed = 5f;
    private float sprintSpeed = 10f;
    private float glidingSpeed = 10f;
    private float turnSpeed = 90f;
    private float yawSpeed = 90f;
    private float pitchSpeed = 90f;
    private float preJumpTime = 0.1f;



    private Rigidbody rb;
    private Animator animator;
    private InputManager input;
    private bool isGrounded;
    private bool isAnimated = false;
    private float smoothedMoveX;
    private float smoothedMoveY;
    private float smoothedMoveSpeed;
    private bool isJumpBeginning;
    private bool isJumping;
    private bool isGliding;





    void Awake()
    {
        input = GameManager.input;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void CheckGround()
    {
        Vector3 origin = transform.position + transform.up * 0.3f;
        RaycastHit hit;

        isGrounded = Physics.Raycast(origin, -transform.up, out hit, 0.6f, groundLayer);

        if (isGrounded)
        {
            isGliding = false;
            if (!isJumpBeginning) isJumping = false;
            // Debug.DrawLine(origin, hit.point, Color.green, 1f);
        }
        // else
        // {
        //     Debug.DrawLine(origin, hit.point, Color.red, 1f);
        // }
    }

    void Update()
    {
        CheckGround();

        if (isGliding)
        {
            GlideUpdate();
            return;
        }

        bool isSprinting = input.isSprinting;
        bool isCrouching = input.isCrouching;

        smoothedMoveX = Mathf.Lerp(smoothedMoveX, input.move.x, Time.deltaTime * 5f);
        smoothedMoveY = Mathf.Lerp(smoothedMoveY, input.move.y, Time.deltaTime * 5f);

        float moveSpeed =
            isSprinting ? sprintSpeed :
            isCrouching ? crouchSpeed :
            runSpeed;

        float turn = smoothedMoveX * turnSpeed * Time.deltaTime;

        rb.AddForce(transform.up * gravityStrength, ForceMode.Acceleration);

        smoothedMoveSpeed = Mathf.Lerp(smoothedMoveSpeed, moveSpeed, Time.deltaTime * 5f);

        Quaternion yawRotation = Quaternion.AngleAxis(turn, Vector3.up);
        transform.rotation = transform.rotation * yawRotation;

        // float moveX = input.move.x * moveSpeed * Time.deltaTime;
        float moveForward = smoothedMoveY * smoothedMoveSpeed * Time.deltaTime;
        transform.position += transform.forward * moveForward;
        // transform.position += transform.right * moveX + transform.forward * moveY;

        // if (jumpValue > 0)
        // {
        //     jumpValue = Mathf.Max(0, jumpValue - Time.deltaTime);
        //     animator.SetFloat(ANIM_JUMP, jumpValue);
        // }

        animator.SetFloat("Forward", smoothedMoveY * smoothedMoveSpeed);
        animator.SetFloat("Turn", smoothedMoveX);
        animator.SetBool("Crouch", isCrouching);
        animator.SetBool("OnGround", isGrounded);
        animator.SetBool("Gliding", isGliding);

        GravityPower();
    }

    public void GravityPower()
    {
        if (isJumping) return;

        RaycastHit hit;
        Vector3 wallOrigin = transform.position + transform.up * 0.7f;
        Vector3 wallDirection = (transform.forward - transform.up).normalized;
        float wallDistanceMax = 0.8f;

        Debug.DrawRay(wallOrigin, wallDirection * wallDistanceMax, Color.black, 0.1f);
        if (Physics.Raycast(wallOrigin, wallDirection, out hit, wallDistanceMax, groundLayer))
        {
            DoRotateToNormal(hit.normal);
            return;
        }

        Vector3 groundOrigin = transform.position + transform.up * 0.5f;
        Vector3 groundDirection = -transform.up;
        float groundDistanceMax = 5f;

        Debug.DrawRay(groundOrigin, groundDirection * groundDistanceMax, Color.blue, 0.1f);
        if (Physics.Raycast(groundOrigin, groundDirection, out hit, groundDistanceMax, groundLayer))
        {
            DoRotateToNormal(hit.normal);
            return;
        }

        DoRotateToNormal(transform.forward);

        // float maxDistance = 1f;
        // Vector3 origin = transform.position + transform.up * 0.5f;
        // RaycastHit hit;

        // isGrounded = Physics.Raycast(origin, -transform.up, out hit, maxDistance, groundLayer);





        // Vector3 origin2 = transform.position + transform.up * 0.2f + transform.forward * 0.5f;
        // Vector3 direction = -(transform.forward + transform.up).normalized;
        // Debug.DrawRay(origin2, direction, Color.blue, 0.1f);
        // float maxDistance2 = 0.6f;

        // if (Physics.Raycast(origin2, direction, out hit, maxDistance2, groundLayer))
        // {
        //     Debug.DrawRay(hit.point, hit.normal, Color.yellow, 10f);
        //     // Vector3 endOfGroundPosition = transform.position + transform.forward * (testDistance - hit.distance);
        //     // Debug.DrawRay(endOfGroundPosition, transform.up, Color.yellow, 10f);
        //     // DOMove(endOfGroundPosition, 0.1f, () =>
        //     // {
        //     //     DoRotateToNormal(hit.normal, 0.3f, () =>
        //     //     {
        //     //         DOMove(hit.point, 0.1f);
        //     //     });
        //     // });
        //     return;
        // }
    }

    public void Attack()
    {
    }

    public void Interact()
    {
        // Debug.Log("Interact");
        // float maxDistance = 1f;
        // Vector3 origin = transform.position + transform.up * 0.4f;
        // RaycastHit hit;

        // if (Physics.Raycast(origin, transform.forward, out hit, maxDistance, groundLayer))
        // {
        //     await DoRotateToNormal(hit.normal, 0.3f);
        //     return;
        // }

        // float testDistance = 1f;
        // Vector3 origin2 = transform.position + transform.up * -0.5f + transform.forward * 1f;
        // Debug.DrawRay(origin2, -transform.forward, Color.blue, 10f);
        // if (Physics.Raycast(origin2, -transform.forward, out hit, maxDistance, groundLayer))
        // {
        //     Vector3 endOfGroundPosition = transform.position + transform.forward * (testDistance - hit.distance);
        //     Debug.DrawRay(endOfGroundPosition, transform.up, Color.yellow, 10f);
        //     await DOMove(endOfGroundPosition, 0.1f);
        //     await DoRotateToNormal(hit.normal, 0.3f);
        //     await DOMove(hit.point, 0.1f);
        //     return;
        // }
    }

    void DoRotateToNormal(Vector3 normal)
    {
        // Calculer la rotation cible
        Quaternion endRotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;

        // Limiter la vitesse de rotation à 360 degrés maximum par secondes
        float maxDegreesDelta = Time.deltaTime * 360.0f;
        Quaternion limitedRotation = Quaternion.RotateTowards(transform.rotation, endRotation, maxDegreesDelta);

        // Appliquer la rotation limitée
        transform.rotation = limitedRotation;
    }

    public void Jump()
    {
        if (isJumpBeginning) return;
        if (isJumping)
        {
            ToggleGlide();
            return;
        }
        if (!isGrounded) return;

        // Stop le Gravity Power
        isJumpBeginning = true;
        isJumping = true;

        DOVirtual.DelayedCall(0.1f, () => isJumpBeginning = false);

        // Appliquer la force physique pour le saut
        rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
    }




    public void ToggleGlide()
    {
        if (isGrounded) return;

        isGliding = !isGliding;
        animator.SetBool("Gliding", isGliding);


    }






    public void GlideUpdate()
    {
        if (isGliding == true)
        {

        }
    }
}

