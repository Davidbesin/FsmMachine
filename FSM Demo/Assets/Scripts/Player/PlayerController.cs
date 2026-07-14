using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float acceleration = 18f;
    [SerializeField] private float deceleration = 24f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -20f;

    [Header("Dash SlowDown")]
    [SerializeField] private float rampDuration = 1.5f;
    [SerializeField] private float minDashMultiplier = 0.3f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private InputSystem_Actions inputActions;

    // Input
    public Vector3 InputDir { get; private set; }
    private bool isSprinting;
    private bool jumpPressedThisFrame;
    public bool JumpPressedThisFrame => jumpPressedThisFrame;

    // SlowDown (Dash modifier)
    private bool isSlowDownHeld;
    private float slowDownStartTime;

    // Movement
    private Vector3 moveDirection;
    private float currentSpeed;
    public float CurrentSpeed => currentSpeed;
    private float targetSpeed;
    private float verticalVelocity;
    public float VerticalVelocity => verticalVelocity;

    // Public Info
    public bool IsGrounded => controller.isGrounded;
    public bool IsMoving => currentSpeed > 0.01f;
    public bool IsRunning => IsMoving && isSprinting;
    public bool IsWalking => IsMoving && !isSprinting;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        controller = GetComponent<CharacterController>();
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Jump.performed += OnJumpPerformed;
        inputActions.Player.Dash.performed += OnDashPerformed;
        inputActions.Player.Sprint.started += OnSprintStarted;
        inputActions.Player.Sprint.canceled += OnSprintCanceled;
        inputActions.Player.SlowDown.started += OnSlowDownStarted;
        inputActions.Player.SlowDown.canceled += OnSlowDownCanceled;
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Dash.performed -= OnDashPerformed;
        inputActions.Player.Sprint.started -= OnSprintStarted;
        inputActions.Player.Sprint.canceled -= OnSprintCanceled;
        inputActions.Player.SlowDown.started -= OnSlowDownStarted;
        inputActions.Player.SlowDown.canceled -= OnSlowDownCanceled;

        inputActions.Player.Disable();
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }

    private void Update()
    {
        ReadInput();
        MovementFSM.Instance.CurrentState?.Tick();

        jumpPressedThisFrame = false;
    }

    //========================================================
    // INPUT
    //========================================================

    private void ReadInput()
    {
        Vector2 move = inputActions.Player.Move.ReadValue<Vector2>();

        float forward = Mathf.Max(0f, move.y); // Only allow forward input

        InputDir = new Vector3(0f, 0f, forward);
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        jumpPressedThisFrame = true;
    }

    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        if (!IsGrounded)
            return;

        if( MovementFSM.Instance.CurrentStateType == MovementStateType.Dash) return;

        MovementFSM.Instance.ChangeState(MovementFSM.Instance.Dash);
    }

    private void OnSprintStarted(InputAction.CallbackContext ctx)
    {
        isSprinting = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        isSprinting = false;
    }

    private void OnSlowDownStarted(InputAction.CallbackContext ctx)
    {
        isSlowDownHeld = true;
        Debug.Log("SlowDown Started");
        slowDownStartTime = Time.time;
    }

    private void OnSlowDownCanceled(InputAction.CallbackContext ctx)
    {
        isSlowDownHeld = false;
    }

    private float GetSlowDownMultiplier()
    {
        if (!isSlowDownHeld) return 1f;

        float t = Mathf.Clamp01((Time.time - slowDownStartTime) / rampDuration);
        return Mathf.Lerp(1f, minDashMultiplier, t);
    }

    public void Dash(float dashSpeed)
    {
        float finalSpeed = dashSpeed * GetSlowDownMultiplier();

        Vector3 velocity =
            transform.forward * finalSpeed +
            Vector3.up * verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    public void HandleMovement()
    {
        bool hasInput = InputDir.sqrMagnitude > 0.01f;

        targetSpeed = hasInput
            ? (isSprinting ? runSpeed : walkSpeed)
            : 0f;

        float rate = targetSpeed > currentSpeed
            ? acceleration
            : deceleration;

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            rate * Time.deltaTime);

        if (hasInput)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            moveDirection =
                forward * InputDir.z +
                right * InputDir.x;

            moveDirection.Normalize();
        }
    }

    //Affect Speed from Anywhere
    public void MultiplySpeed(float multiplier)
    {
        currentSpeed *= multiplier;
    }

    //========================================================
    // JUMP / GRAVITY
    //========================================================
    public void HandleJumpAndGravity()
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (jumpPressedThisFrame)
            {
                verticalVelocity =
                    Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity =
            moveDirection * currentSpeed +
            Vector3.up * verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}