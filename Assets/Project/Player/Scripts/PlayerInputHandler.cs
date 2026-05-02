using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Centralizes all player input reading using the new Input System.
/// Acts as an intermediary layer between Input Actions and other scripts.
///
/// COMBAT INPUT CONFIGURATION in PlayerInputActions asset:
/// ─────────────────────────────────────────────────────────
/// Action Map: "Player"
///
/// Attack (Button) — Left Mouse Button / Gamepad West
///   Add TWO interactions on this action:
///   1. "Tap"  → duration 0.3s   (triggers OnAttackTap)
///   2. "Hold" → duration 0.5s   (triggers OnAttackHold)
///   NOTE: In "Interactions" dropdown, add both. Set "Trigger Behavior" = "Fire And Forget"
///
/// AltAttack (Button) — Right Mouse Button / Gamepad East
///   No special interaction needed (plain button press)
///   The CombatController detects simultaneous Attack+AltAttack for area attack.
/// ─────────────────────────────────────────────────────────
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions _inputActions;
    private PlayerInput _playerInput;

    // ──────────────────────────────────────────────
    // MOVEMENT INPUT
    // ──────────────────────────────────────────────
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    // ──────────────────────────────────────────────
    // STANDARD BUTTON INPUTS (pressed this frame)
    // ──────────────────────────────────────────────
    public bool JumpPressed { get; private set; }
    public bool InteractPressed { get; private set; }

    // ──────────────────────────────────────────────
    // HELD BUTTONS
    // ──────────────────────────────────────────────
    public bool SprintHeld { get; private set; }

    // ──────────────────────────────────────────────
    // COMBAT INPUTS
    // ──────────────────────────────────────────────

    /// <summary>
    /// Tap del ataque (click izquierdo suelto antes del Hold threshold).
    /// El CombatController lo consume para avanzar el combo.
    /// </summary>
    public bool AttackTapPressed { get; private set; }

    /// <summary>
    /// El botón de ataque está siendo mantenido activamente este frame.
    /// El CombatController lo usa para mostrar feedback de carga.
    /// </summary>
    public bool AttackHeld { get; private set; }

    /// <summary>
    /// El Hold se completó (ataque cargado listo).
    /// Se dispara una vez cuando se alcanza el Hold threshold.
    /// </summary>
    public bool AttackChargeReady { get; private set; }

    /// <summary>
    /// El botón de ataque fue soltado este frame (independientemente de si fue tap o hold).
    /// Útil para ejecutar el charged attack al soltar.
    /// </summary>
    public bool AttackReleased { get; private set; }

    /// <summary>
    /// Click derecho presionado este frame.
    /// </summary>
    public bool AltAttackPressed { get; private set; }

    /// <summary>
    /// Click derecho está siendo mantenido este frame.
    /// </summary>
    public bool AltAttackHeld { get; private set; }

    // ──────────────────────────────────────────────
    // INVENTORY INPUTS
    // ──────────────────────────────────────────────

    public bool ToggleInventoryPressed { get; private set; }

    // ──────────────────────────────────────────────
    // EVENTS
    // ──────────────────────────────────────────────

    public event Action<bool> OnToggleInventory;
    public event Action OnLightAttack;

    // ──────────────────────────────────────────────
    // SETTINGS
    // ──────────────────────────────────────────────
    [Header("Mouse Settings")]
    public float mouseSensitivity = 1f;
    public float gamepadSensitivity = 2f;
    public bool invertYAxis = false;

    // ──────────────────────────────────────────────
    // INTERNAL STATE
    // ──────────────────────────────────────────────
    // Rastreamos si el hold ya se completó para no re-disparar
    private bool _chargeAlreadyFired;

    // ══════════════════════════════════════════════
    // LIFECYCLE
    // ══════════════════════════════════════════════

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();

        // Movement
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;
        _inputActions.Player.Look.performed += OnLook;
        _inputActions.Player.Look.canceled += OnLook;

        // Standard
        _inputActions.Player.Jump.performed += OnJumpPerformed;
        _inputActions.Player.Sprint.performed += OnSprintPerformed;
        _inputActions.Player.Sprint.canceled += OnSprintCanceled;
        _inputActions.Player.Interact.performed += OnInteractPerformed;

        // Combat — Attack (Left Click)
        _inputActions.Player.Attack.started += OnAttackStarted;
        _inputActions.Player.Attack.performed += OnAttackPerformed;
        _inputActions.Player.Attack.canceled += OnAttackCanceled;

        _inputActions.Player.ToggleInventory.performed += OnToggleInventoryPerformed;

        _inputActions.Inventory.ToggleInventory.performed += OnToggleInventoryPerformed;
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMove;
        _inputActions.Player.Move.canceled -= OnMove;
        _inputActions.Player.Look.performed -= OnLook;
        _inputActions.Player.Look.canceled -= OnLook;

        _inputActions.Player.Jump.performed -= OnJumpPerformed;
        _inputActions.Player.Sprint.performed -= OnSprintPerformed;
        _inputActions.Player.Sprint.canceled -= OnSprintCanceled;
        _inputActions.Player.Interact.performed -= OnInteractPerformed;

        _inputActions.Player.Attack.started -= OnAttackStarted;
        _inputActions.Player.Attack.performed -= OnAttackPerformed;
        _inputActions.Player.Attack.canceled -= OnAttackCanceled;

        _inputActions.Player.Disable();
    }

    private void Update()
    {
        // Resetear valores "this frame" al final de cada frame
        JumpPressed = false;
        InteractPressed = false;
        AttackTapPressed = false;
        AttackChargeReady = false;
        AttackReleased = false;
        AltAttackPressed = false;

        // AttackHeld: verdadero mientras el botón esté presionado físicamente
        AttackHeld = _inputActions.Player.Attack.IsPressed();
    }

    // ══════════════════════════════════════════════
    // MOVEMENT CALLBACKS
    // ══════════════════════════════════════════════

    private void OnMove(InputAction.CallbackContext ctx)
        => MoveInput = ctx.ReadValue<Vector2>();

    private void OnLook(InputAction.CallbackContext ctx)
    {
        Vector2 raw = ctx.ReadValue<Vector2>();
        float sensitivity = mouseSensitivity;
        LookInput = new Vector2(
            raw.x * sensitivity,
            raw.y * sensitivity * (invertYAxis ? -1f : 1f)
        );
    }

    // ══════════════════════════════════════════════
    // STANDARD CALLBACKS
    // ══════════════════════════════════════════════

    private void OnJumpPerformed(InputAction.CallbackContext ctx) => JumpPressed = true;
    private void OnSprintPerformed(InputAction.CallbackContext ctx) => SprintHeld = true;
    private void OnSprintCanceled(InputAction.CallbackContext ctx) => SprintHeld = false;
    private void OnInteractPerformed(InputAction.CallbackContext ctx) => InteractPressed = true;

    // ══════════════════════════════════════════════
    // COMBAT CALLBACKS
    // ══════════════════════════════════════════════

    /// <summary>
    /// Started: el botón fue presionado (inicio del gesto, sin importar Tap o Hold).
    /// Reseteamos el flag de carga aquí.
    /// </summary>
    private void OnAttackStarted(InputAction.CallbackContext ctx)
    {
        _chargeAlreadyFired = false;
    }

    /// <summary>
    /// Performed: se dispara cuando una Interaction se completa.
    /// Con Tap  → rápido release antes del Hold threshold.
    /// Con Hold → se mantiene el tiempo configurado.
    /// Diferenciamos la Interaction activa por su tipo.
    /// </summary>
    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        // ctx.interaction nos dice qué Interaction disparó este callback
        if (ctx.interaction is UnityEngine.InputSystem.Interactions.TapInteraction)
        {
            AttackTapPressed = true;
            OnLightAttack?.Invoke();
        }
        else if (ctx.interaction is UnityEngine.InputSystem.Interactions.HoldInteraction)
        {
            if (!_chargeAlreadyFired)
            {
                AttackChargeReady = true;
                _chargeAlreadyFired = true;
            }
        }
    }

    /// <summary>
    /// Canceled: el botón fue soltado (en cualquier caso).
    /// Útil para ejecutar el ataque cargado en el momento del release.
    /// </summary>
    private void OnAttackCanceled(InputAction.CallbackContext ctx)
    {
        AttackReleased = true;
    }

    // ══════════════════════════════════════════════
    // INVENTORY CALLBACKS
    // ══════════════════════════════════════════════

    private void OnToggleInventoryPerformed(InputAction.CallbackContext ctx)
    {
        ToggleInventoryPressed = !ToggleInventoryPressed; // Toggle the state

        OnToggleInventory?.Invoke(ToggleInventoryPressed);

        if (ToggleInventoryPressed)
        {
            _inputActions.Player.Disable(); // Disable player controls when inventory is open
            _inputActions.Inventory.Enable(); // Enable inventory controls
            return;
        }

        CloseInventory();
    }

    // ══════════════════════════════════════════════
    // PUBLIC UTILITIES
    // ══════════════════════════════════════════════

    public void SetInputEnabled(bool enabled)
    {
        if (enabled) _inputActions.Player.Enable();
        else _inputActions.Player.Disable();
    }

    public void CloseInventory()
    {
        ToggleInventoryPressed = false;

        _inputActions.Player.Enable(); // Enable player controls
        _inputActions.Inventory.Disable(); // Disable inventory controls when inventory is closed
    }
}