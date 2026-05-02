using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Main player controller for a 3D game.
/// Requires: CharacterController, PlayerInputHandler
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    // ──────────────────────────────────────────────
    // MOVEMENT SETTINGS
    // ──────────────────────────────────────────────
    [Header("Movement")]
    [Tooltip("Walking speed (m/s)")]
    public float walkSpeed = 4f;

    [Tooltip("Sprint speed (m/s)")]
    public float sprintSpeed = 8f;

    [Tooltip("Acceleration/deceleration smoothing")]
    [Range(0f, 1f)]
    public float movementSmoothing = 0.1f;

    [Tooltip("Animation speed blend (smoothing)")]
    public float AnimationSpeedBlend = 10.0f;

    [Tooltip("Character rotation speed toward the movement direction")]
    public float rotationSpeed = 10f;

    // ──────────────────────────────────────────────
    // JUMP & GRAVITY
    // ──────────────────────────────────────────────
    [Header("Jump & Gravity")]
    [Tooltip("Jump height in meters")]
    public float jumpHeight = 1.5f;

    [Tooltip("Gravity multiplier")]
    public float gravityMultiplier = 2.5f;

    [Tooltip("Jump buffer time (allows jumping just before touching the ground)")]
    public float jumpBufferTime = 0.15f;

    [Tooltip("Coyote time (grace period) for jumping after stepping off a ledge")]
    public float coyoteTime = 0.15f;

    // ──────────────────────────────────────────────
    // GROUND CHECK
    // ──────────────────────────────────────────────
    [Header("Ground Check")]
    public LayerMask groundMask = ~0;
    public float groundCheckDistance = 0.2f;
    public Transform groundCheckOrigin;

    // ──────────────────────────────────────────────
    // REFERENCES (private)
    // ──────────────────────────────────────────────
    private CharacterController _cc;
    private PlayerInputHandler _input;
    private Transform _cameraTransform;
    private Animator _anim;

    // ──────────────────────────────────────────────
    // STATE
    // ──────────────────────────────────────────────
    private Vector3 _velocity;           // Current velocity (includes gravity)
    private Vector3 _moveDirection;      // Smoothed movement direction
    private Vector3 _smoothMoveVelocity; // Reference for SmoothDamp

    private bool _isGrounded;
    private bool _wasGrounded;
    private bool _isSprinting;

    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;

    private float _currentSpeed;
    private float _animationBlend;

    private bool _canMove = true;

    // ──────────────────────────────────────────────
    // ANIMATION ID's
    // ──────────────────────────────────────────────

    private readonly int SpeedHash = Animator.StringToHash("Speed");
    private readonly int Grounded = Animator.StringToHash("IsGrounded");
    private readonly int Jump = Animator.StringToHash("IsJumping");
    private readonly int FreeFall = Animator.StringToHash("IsFreeFalling");

    // ──────────────────────────────────────────────
    // PROPERTIES
    // ──────────────────────────────────────────────
    public bool IsGrounded  => _isGrounded;
    public bool IsSprinting => _isSprinting;
    public bool IsMoving    => _moveDirection.magnitude > 0.1f;
    public float CurrentSpeed => _currentSpeed;
    public Vector3 Velocity  => _velocity;

    // ──────────────────────────────────────────────
    // EVENTS
    // ──────────────────────────────────────────────
    public event Action OnJump;
    public event Action OnLand;
    public event Action<bool> OnSprintChanged;    

    // ──────────────────────────────────────────────
    // CONSTANTS
    // ──────────────────────────────────────────────
    private const float GRAVITY = -9.81f;

    // ══════════════════════════════════════════════
    // LIFECYCLE
    // ══════════════════════════════════════════════

    private void Awake()
    {
        _cc    = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputHandler>();
        _anim = GetComponent<Animator>();

        PlayerEvents.OnToggleCanMove += HandleToggleCanMove;

        if (Camera.main != null)
            _cameraTransform = Camera.main.transform;

        // If no ground check origin is assigned, use the object's own transform
        if (groundCheckOrigin == null)
            groundCheckOrigin = transform;
    }

    private void Update()
    {
        CheckGround();
        HandleCoyoteTime();
        HandleJumpBuffer();
        HandleMovement();
        HandleJump();
        ApplyGravity();

        _cc.Move(_velocity * Time.deltaTime);
    }

    // ══════════════════════════════════════════════
    // GROUND CHECK
    // ══════════════════════════════════════════════

    private void CheckGround()
    {
        _wasGrounded = _isGrounded;

        Vector3 origin = groundCheckOrigin.position + Vector3.up * 0.1f;
        _isGrounded = Physics.CheckSphere(origin, groundCheckDistance, groundMask,
                QueryTriggerInteraction.Ignore);

        // Landing event
        if (!_wasGrounded && _isGrounded)
        {
            OnLand?.Invoke();
            _anim.SetBool(Jump, false);
            _anim.SetBool(FreeFall, false);
        }

        _anim.SetBool(Grounded, _isGrounded);
    }

    // ══════════════════════════════════════════════
    // COYOTE TIME & JUMP BUFFER
    // ══════════════════════════════════════════════

    private void HandleCoyoteTime()
    {
        if (_isGrounded)
            _coyoteTimeCounter = coyoteTime;
        else
            _coyoteTimeCounter -= Time.deltaTime;
    }

    private void HandleJumpBuffer()
    {
        if (_input.JumpPressed)
            _jumpBufferCounter = jumpBufferTime;
        else
            _jumpBufferCounter -= Time.deltaTime;
    }

    // ══════════════════════════════════════════════
    // MOVEMENT
    // ══════════════════════════════════════════════

    private void HandleMovement()
    {
        Vector2 inputVector = _canMove ? _input.MoveInput : Vector2.zero;

        // Determines if sprinting
        bool wantsSprint   = _input.SprintHeld;
        bool wasSprinting  = _isSprinting;
        _isSprinting = wantsSprint && inputVector.magnitude > 0.1f;

        if (_isSprinting != wasSprinting)
            OnSprintChanged?.Invoke(_isSprinting);

        // Target speed based on state
        float targetSpeed = inputVector == Vector2.zero ? 0f : 
                            _isSprinting ? sprintSpeed :
                            walkSpeed;

        _currentSpeed = targetSpeed;

        // Build direction relative to the camera
        Vector3 desiredMove = Vector3.zero;
        if (inputVector.magnitude > 0.01f && _cameraTransform != null)
        {
            Vector3 camForward = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized;
            Vector3 camRight   = Vector3.ProjectOnPlane(_cameraTransform.right,   Vector3.up).normalized;

            desiredMove = (camForward * inputVector.y + camRight * inputVector.x).normalized;
        }

        // Movement smoothing
        _moveDirection = Vector3.SmoothDamp(
            _moveDirection,
            desiredMove * targetSpeed,
            ref _smoothMoveVelocity,
            movementSmoothing
        );

        // Apply to the horizontal velocity vector
        _velocity.x = _moveDirection.x;
        _velocity.z = _moveDirection.z;

        // Character rotation toward the movement direction
        if (desiredMove.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredMove);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * AnimationSpeedBlend);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        _anim.SetFloat(SpeedHash, _animationBlend);
    }

    private void HandleToggleCanMove(bool canMove)
    {
        _canMove = canMove;
    }

    // ══════════════════════════════════════════════
    // JUMP
    // ══════════════════════════════════════════════

    private void HandleJump()
    {
        // Normal jump (with coyote time + jump buffer)
        bool canJump = _coyoteTimeCounter > 0f && _jumpBufferCounter > 0f;

        if (canJump)
        {
            PerformJump();
            _coyoteTimeCounter = 0f;
            _jumpBufferCounter = 0f;
            return;
        }
    }

    private void PerformJump()
    {
        // v = sqrt(2 * g * h)
        _velocity.y = Mathf.Sqrt(2f * Mathf.Abs(GRAVITY) * gravityMultiplier * jumpHeight);
        OnJump?.Invoke();

        _anim.SetBool(Jump, true);
    }

    // ══════════════════════════════════════════════
    // GRAVITY
    // ══════════════════════════════════════════════

    private void ApplyGravity()
    {
        if (_isGrounded && _velocity.y < 0f)
        {
            // Small downward force to keep grounded
            _velocity.y = -2f;
        }
        else
        {
            _velocity.y += GRAVITY * gravityMultiplier * Time.deltaTime;
        }

        // Fall speed clamp
        _velocity.y = Mathf.Max(_velocity.y, GRAVITY * gravityMultiplier);
    }

    // ══════════════════════════════════════════════
    // GIZMOS
    // ══════════════════════════════════════════════

    private void OnDrawGizmosSelected()
    {
        if (groundCheckOrigin == null) return;

        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Vector3 origin = groundCheckOrigin.position + Vector3.up * 0.1f;
        Gizmos.DrawWireSphere(origin + Vector3.down * groundCheckDistance, 0.15f);
    }
}
