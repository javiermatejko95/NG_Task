using System.Collections;
using UnityEngine;

/// <summary>
/// Handles all player combat logic:
///   - Combo attack    : Left Click (tap, up to 3 hits)
///   - Charged attack  : Left Click hold (0.5s) → release
///   - Area attack     : Left Click + Right Click simultaneously
///
/// Attach to the same GameObject as PlayerController and PlayerInputHandler.
/// </summary>
public class CombatController : MonoBehaviour
{
    // ──────────────────────────────────────────────
    // COMBO SETTINGS
    // ──────────────────────────────────────────────
    [Header("Combo")]
    [Tooltip("Maximum hits in the combo chain")]
    public int maxComboSteps = 3;

    [Tooltip("Time window to input the next hit after a step completes")]
    public float comboWindowTime = 0.8f;

    [Tooltip("Duration of each combo hit animation (lock-out time)")]
    public float[] comboHitDuration = { 0.4f, 0.4f, 0.6f };

    [Tooltip("Damage per combo hit")]
    public float[] comboDamage = { 10f, 10f, 20f };

    [Tooltip("Layer mask for enemies")]
    public LayerMask enemyMask;

    // ──────────────────────────────────────────────
    // ATTACK HITBOX
    // ──────────────────────────────────────────────
    [Header("Hitbox")]
    [Tooltip("Origin of the combo/charged attack hitbox (usually a hand bone or weapon tip)")]
    public Transform attackOrigin;

    [Tooltip("Radius of the melee hitbox")]
    public float meleeRadius = 0.8f;

    [Tooltip("Forward reach of the melee hitbox")]
    public float meleeRange = 1.2f;

    // ──────────────────────────────────────────────
    // REFERENCES
    // ──────────────────────────────────────────────
    private PlayerInputHandler _input;

    // ──────────────────────────────────────────────
    // STATE MACHINE
    // ──────────────────────────────────────────────
    public enum CombatState
    {
        Idle,
        ComboAttacking
    }

    public CombatState State { get; private set; } = CombatState.Idle;

    // Combo state
    private int   _comboStep          = 0;   // 0-based index of the current hit
    private bool  _comboInputQueued   = false;
    private float _comboWindowCounter = 0f;
    private bool  _inHitLockout       = false;

    // ──────────────────────────────────────────────
    // PROPERTIES
    // ──────────────────────────────────────────────
    public bool IsAttacking => State != CombatState.Idle;
    public int  ComboStep   => _comboStep;

    // ──────────────────────────────────────────────
    // EVENTS
    // ──────────────────────────────────────────────
    public event System.Action<int>   OnComboHit;          // combo step (1-based)
    public event System.Action        OnComboFinished;
    public event System.Action        OnComboReset;

    // ══════════════════════════════════════════════
    // LIFECYCLE
    // ══════════════════════════════════════════════

    private void Awake()
    {
        _input = GetComponent<PlayerInputHandler>();

        if (attackOrigin == null)
            attackOrigin = transform;
    }

    private void Update()
    {
        HandleCombatInput();
        HandleComboWindow();
    }

    // ══════════════════════════════════════════════
    // MAIN INPUT DISPATCH
    // ══════════════════════════════════════════════

    private void HandleCombatInput()
    {
        switch (State)
        {
            case CombatState.Idle:
                HandleIdleInput();
                break;

            case CombatState.ComboAttacking:
                // Queue next hit during lockout window
                if (_input.AttackTapPressed)
                    _comboInputQueued = true;
                break;
        }
    }

    // ──────────────────────────────────────────────
    // IDLE → decide which attack starts
    // ──────────────────────────────────────────────

    private void HandleIdleInput()
    {

        // Start charging (visual feedback as soon as hold starts)
        // AttackHeld is true every frame the button is down.
        // We enter Charging state early to show the charge-up VFX;
        // the actual hit fires when the Hold interaction completes.
        if (_input.AttackHeld && State == CombatState.Idle && _comboStep == 0)
        {
            // Distinguish: is this the start of a hold or just a quick tap?
            // We only enter Charging if the button has been held for a couple frames.
            // The actual signal is AttackChargeReady (handled above).
            // Here we just trigger the OnChargeStarted visual feedback.
            // (This is only triggered once per hold thanks to the flag below)
        }

        // Combo: tap received
        if (_input.AttackTapPressed)
        {
            StartCoroutine(ExecuteComboStep());
        }
    }

    // ══════════════════════════════════════════════
    // COMBO WINDOW TIMER
    // Resets the combo if no input arrives in time between hits.
    // ══════════════════════════════════════════════

    private void HandleComboWindow()
    {
        if (State != CombatState.ComboAttacking) return;
        if (_inHitLockout) return; // don't count down during the hit animation

        _comboWindowCounter -= Time.deltaTime;

        if (_comboWindowCounter <= 0f)
        {
            ResetCombo();
        }
    }

    // ══════════════════════════════════════════════
    // COROUTINES — ATTACK EXECUTION
    // ══════════════════════════════════════════════

    /// <summary>Executes one step of the combo chain.</summary>
    private IEnumerator ExecuteComboStep()
    {
        if (_comboStep >= maxComboSteps)
        {
            ResetCombo();
            yield break;
        }

        State            = CombatState.ComboAttacking;
        _inHitLockout    = true;
        _comboInputQueued = false;

        int step = _comboStep; // capture for events/damage

        // Fire event & apply damage
        OnComboHit?.Invoke(step + 1);
        ApplyMeleeDamage(comboDamage[step]);

        // Wait for the hit animation to finish
        float duration = step < comboHitDuration.Length ? comboHitDuration[step] : 0.4f;
        yield return new WaitForSeconds(duration);

        _inHitLockout = false;
        _comboStep++;

        // Last step in chain → finish
        if (_comboStep >= maxComboSteps)
        {
            OnComboFinished?.Invoke();
            yield return new WaitForSeconds(0.1f); // tiny grace before full reset
            ResetCombo();
            yield break;
        }

        // Open the combo window for the next input
        _comboWindowCounter = comboWindowTime;

        // If input was already queued during lockout, chain immediately
        if (_comboInputQueued)
        {
            _comboInputQueued = false;
            StartCoroutine(ExecuteComboStep());
        }
    }

    // ══════════════════════════════════════════════
    // DAMAGE APPLICATION
    // ══════════════════════════════════════════════

    private void ApplyMeleeDamage(float damage)
    {
        if (attackOrigin == null) return;

        Vector3 hitCenter = attackOrigin.position + attackOrigin.forward * meleeRange;

        Collider[] hits = Physics.OverlapSphere(hitCenter, meleeRadius, enemyMask,
            QueryTriggerInteraction.Ignore);

        foreach (var col in hits)
        {
            // Replace IDamageable with your own damage interface
            if (col.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(damage);
        }
    }

    // ══════════════════════════════════════════════
    // STATE RESET
    // ══════════════════════════════════════════════

    private void ResetCombo()
    {
        _comboStep          = 0;
        _comboWindowCounter = 0f;
        _comboInputQueued   = false;
        _inHitLockout       = false;
        State               = CombatState.Idle;
        OnComboReset?.Invoke();
    }

    private void ResetCombatState()
    {
        State = CombatState.Idle;
    }

    // ══════════════════════════════════════════════
    // GIZMOS
    // ══════════════════════════════════════════════

    private void OnDrawGizmosSelected()
    {
        // Melee hitbox
        if (attackOrigin != null)
        {
            Gizmos.color = Color.red;
            Vector3 center = attackOrigin.position + attackOrigin.forward * meleeRange;
            Gizmos.DrawWireSphere(center, meleeRadius);
        }
    }
}