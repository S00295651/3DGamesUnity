using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;

    public bool IsCrouching { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsSprinting { get; private set; }
    public Vector3 Velocity { get { return characterController.velocity; } }

    private Vector2 movementInput;
    private Vector3 movementVelocity;
    private Vector3 movement;
    private float currentSpeed;

    [Header("Settings")]
    [SerializeField] private float WalkSpeed;
    [SerializeField] private float SprintSpeed;
    [SerializeField] private float CrouchSpeed;
    [SerializeField] private float CrouchHeight;
    [SerializeField] private float JumpHeight;
    [SerializeField] private float Gravity = 9.81f;

    private float NormalHeight;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        NormalHeight = characterController.height;
        currentSpeed = WalkSpeed;
    }

    private void OnEnable()
    {
        InputManager.Actions.Game.Move.performed += OnMove;
        InputManager.Actions.Game.Move.canceled += OnMove;

        InputManager.Actions.Game.Jump.performed += OnJump;
        InputManager.Actions.Game.Crouch.performed += OnCrouch;
        InputManager.Actions.Game.Sprint.performed += OnSprint;
    }

    private void OnDisable()
    {
        InputManager.Actions.Game.Move.performed -= OnMove;
        InputManager.Actions.Game.Move.canceled -= OnMove;

        InputManager.Actions.Game.Jump.performed -= OnJump;
        InputManager.Actions.Game.Crouch.performed -= OnCrouch;
        InputManager.Actions.Game.Sprint.performed -= OnSprint;
    }

    public void OnMove(InputAction.CallbackContext obj)
    {
        movementInput = obj.ReadValue<Vector2>();
    }

    public bool CanJump()
    {
        return characterController.isGrounded;
    }

    public void OnJump(InputAction.CallbackContext obj)
    {
        if(CanJump())
        {
            movementVelocity.y = Mathf.Sqrt(JumpHeight * 2f * Gravity);
        }
    }

    public void OnCrouch(InputAction.CallbackContext obj)
    {
        IsCrouching = ! IsCrouching;
        characterController.height = IsCrouching ? CrouchHeight : NormalHeight;
        currentSpeed = IsCrouching ? CrouchSpeed : WalkSpeed;
    }

    public void OnSprint(InputAction.CallbackContext obj)
    {
        IsSprinting = ! IsSprinting;

        if (!IsCrouching)
            {
                currentSpeed = IsSprinting ? SprintSpeed : WalkSpeed;
        }
    }

    private void Update()
    {
        if(characterController.isGrounded && movementVelocity.y < 0)
        {
            movementVelocity.y = -2f;
        }

        movement = transform.right * movementInput.x + transform.forward * movementInput.y;
        movementVelocity.y -= Gravity * Time.deltaTime;

        characterController.Move(((movement * currentSpeed) + movementVelocity) * Time.deltaTime);
    }
}