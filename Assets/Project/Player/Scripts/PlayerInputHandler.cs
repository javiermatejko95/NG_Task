using UnityEngine;
using UnityEngine.InputSystem;

// Obs: script imported from other project

/// <summary>
/// Centralizes all player input reading using the new Input System.
/// Acts as an intermediary layer between Input Actions and other scripts.
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions _inputActions;
    private PlayerInput _playerInput;

    // ──────────────────────────────────────────────
    // INPUT VALUES
    // ──────────────────────────────────────────────
    public Vector2 MoveInput    { get; private set; }
    public Vector2 LookInput    { get; private set; }

    // Buttons with "pressed this frame" logic
    public bool JumpPressed     { get; private set; }
    public bool InteractPressed { get; private set; }

    // Held buttons
    public bool SprintHeld      { get; private set; }

    // Active control scheme information
    public bool IsGamepad       { get; private set; }
    public string CurrentControlScheme => _playerInput != null ? _playerInput.currentControlScheme : "Keyboard";

    // ──────────────────────────────────────────────
    // SETTINGS
    // ──────────────────────────────────────────────
    [Header("Mouse Settings")]
    [Tooltip("Sensibilidad del mouse")]
    public float mouseSensitivity = 1f;

    [Tooltip("Sensibilidad del stick del gamepad")]
    public float gamepadSensitivity = 2f;

    [Tooltip("Invertir eje Y del look")]
    public bool invertYAxis = false;

    // ══════════════════════════════════════════════
    // LIFECYCLE
    // ══════════════════════════════════════════════

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _playerInput  = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();

        // Subscribe to callbacks
        _inputActions.Player.Move.performed  += OnMove;
        _inputActions.Player.Move.canceled   += OnMove;

        _inputActions.Player.Look.performed  += OnLook;
        _inputActions.Player.Look.canceled   += OnLook;

        _inputActions.Player.Jump.performed  += OnJumpPerformed;

        _inputActions.Player.Sprint.performed += OnSprintPerformed;
        _inputActions.Player.Sprint.canceled  += OnSprintCanceled;

        _inputActions.Player.Interact.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed  -= OnMove;
        _inputActions.Player.Move.canceled   -= OnMove;

        _inputActions.Player.Look.performed  -= OnLook;
        _inputActions.Player.Look.canceled   -= OnLook;

        _inputActions.Player.Jump.performed  -= OnJumpPerformed;

        _inputActions.Player.Sprint.performed -= OnSprintPerformed;
        _inputActions.Player.Sprint.canceled  -= OnSprintCanceled;

        _inputActions.Player.Interact.performed -= OnInteractPerformed;

        _inputActions.Player.Disable();
    }

    private void Update()
    {
        // Reset “pressed this frame” values at the end of each frame
        JumpPressed = false;
        InteractPressed = false;

        // Detect if a gamepad is being used
        if (_playerInput != null)
            IsGamepad = _playerInput.currentControlScheme == "Gamepad";
    }

    // ══════════════════════════════════════════════
    // INPUT CALLBACKS
    // ══════════════════════════════════════════════

    private void OnMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        Vector2 raw = ctx.ReadValue<Vector2>();

        float sensitivity = IsGamepad ? gamepadSensitivity : mouseSensitivity;

        LookInput = new Vector2(
            raw.x * sensitivity,
            raw.y * sensitivity * (invertYAxis ? -1f : 1f)
        );
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        JumpPressed = true;
    }

    private void OnSprintPerformed(InputAction.CallbackContext ctx) => SprintHeld = true;
    private void OnSprintCanceled(InputAction.CallbackContext ctx)  => SprintHeld = false;

    private void OnInteractPerformed(InputAction.CallbackContext ctx) => InteractPressed = true;

    // ══════════════════════════════════════════════
    // PUBLIC UTILITIES
    // ══════════════════════════════════════════════

    /// <summary>Lock/unlock all player input</summary>
    public void SetInputEnabled(bool enabled)
    {
        if (enabled) _inputActions.Player.Enable();
        else         _inputActions.Player.Disable();
    }
}
