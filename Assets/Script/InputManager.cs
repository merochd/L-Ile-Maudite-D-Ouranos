using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputEventType
{
    Attack,
    Interact,
    Jump,
    Previous,
    Next,
    Crouch,
    Sprint,

    Gliding
}

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction attackAction;
    private InputAction interactAction;
    private InputAction jumpAction;
    private InputAction previousAction;
    private InputAction nextAction;
    private InputAction crouchAction;
    private InputAction sprintAction;

    private InputAction glideAction;

    public Vector2 move;
    public Vector2 look;
    public bool isSprinting;
    public bool isCrouching;

    public bool isGliding;

    private void Awake()
    {
        moveAction = inputActionAsset.FindAction("Move");
        lookAction = inputActionAsset.FindAction("Look");
        attackAction = inputActionAsset.FindAction("Attack");
        interactAction = inputActionAsset.FindAction("Interact");
        jumpAction = inputActionAsset.FindAction("Jump");
        previousAction = inputActionAsset.FindAction("Previous");
        nextAction = inputActionAsset.FindAction("Next");
        crouchAction = inputActionAsset.FindAction("Crouch");
        sprintAction = inputActionAsset.FindAction("Sprint");
        glideAction = inputActionAsset.FindAction("Glide");
    }

    void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        attackAction.Enable();
        interactAction.Enable();
        jumpAction.Enable();
        previousAction.Enable();
        nextAction.Enable();
        crouchAction.Enable();
        sprintAction.Enable();
        glideAction.Enable();

        moveAction.started += OnMove;
        lookAction.started += OnLook;

        jumpAction.started += OnJump;
        previousAction.started += OnPrevious;
        nextAction.started += OnNext;
        crouchAction.started += OnCrouch;
        sprintAction.started += OnSprint;
        glideAction.started += OnGlide;

        moveAction.performed += OnMove;
        lookAction.performed += OnLook;

        moveAction.canceled += OnMove;
        lookAction.canceled += OnLook;
        sprintAction.canceled += OnSprint;
        glideAction.canceled += OnGlide;
    }

    void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        attackAction.Disable();
        interactAction.Disable();
        jumpAction.Disable();
        previousAction.Disable();
        nextAction.Disable();
        crouchAction.Disable();
        sprintAction.Disable();
        glideAction.Disable();

        moveAction.started -= OnMove;
        lookAction.started -= OnLook;
        jumpAction.started -= OnJump;
        previousAction.started -= OnPrevious;
        nextAction.started -= OnNext;
        crouchAction.started -= OnCrouch;
        sprintAction.started -= OnSprint;
        glideAction.started -= OnGlide;

        moveAction.performed -= OnMove;
        lookAction.performed -= OnLook;

        moveAction.canceled -= OnMove;
        lookAction.canceled -= OnLook;
        sprintAction.canceled -= OnSprint;
        glideAction.canceled -= OnGlide;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }




    private void OnCrouch(InputAction.CallbackContext context)
    {
        isCrouching = !isCrouching;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        GameManager.player.Jump();
    }

    private void OnPrevious(InputAction.CallbackContext context)
    {
        Debug.Log($"OnPrevious");
    }

    private void OnNext(InputAction.CallbackContext context)
    {
        Debug.Log($"OnNext");
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.started;
    }
    private void OnGlide(InputAction.CallbackContext context)
    {
        isGliding = context.started;
    }
}
