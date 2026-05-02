using UnityEngine;

/// <summary>
/// Obs: imported a simpler and adapted combat controller script I had used in another project
/// </summary>
public class CombatController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private ComboStepSO _comboSteps;
    [SerializeField] private PlayerInputHandler _playerInput;

    private bool _canMove = true;

    private bool _isComboWindowOpen = false;
    private int _currentComboIndex = 0;

    // Light Attack Buffering
    private bool _lightPending;
    private float _lightPendingTime;
    private const float LIGHT_DELAY = 0.03f;
    private const float LIGHT_BUFFER_EXPIRY = 0.3f;

    private void Awake()
    {
        _playerInput.OnLightAttack += HandleLightAttackInput;
    }

    private void OnDestroy()
    {
        _playerInput.OnLightAttack -= HandleLightAttackInput;
    }

    private void Update()
    {
        ResolveLightAttackBuffer();
    }

    private void HandleLightAttackInput()
    {
        _lightPending = true;
        _lightPendingTime = Time.time;
    }

    private void ResolveLightAttackBuffer()
    {
        if (!_lightPending) return;

        if (Time.time - _lightPendingTime >= LIGHT_BUFFER_EXPIRY)
        {
            _lightPending = false;
            return;
        }

        if (Time.time - _lightPendingTime >= LIGHT_DELAY)
        {
            _lightPending = false;
            TryExecuteComboStep();
        }
    }

    private void TryExecuteComboStep()
    {
        if (_currentComboIndex == 0)
        {
            if (!_canMove) return;
            ExecuteComboStep();
            return;
        }

        if (_currentComboIndex < _comboSteps.TriggerName.Length && _isComboWindowOpen)
        {
            ExecuteComboStep();
        }
    }

    private void ExecuteComboStep()
    {
        _animator.applyRootMotion = true;
        _animator.SetTrigger(_comboSteps.TriggerName[_currentComboIndex]);

        _isComboWindowOpen = false;
        _canMove = false;
        PlayerEvents.RaiseToggleCanMove(false);

        _currentComboIndex++;
    }

    private void ResetCombo()
    {
        _currentComboIndex = 0;
        _isComboWindowOpen = false;
        _canMove = true;
        PlayerEvents.RaiseToggleCanMove(true);
        _animator.applyRootMotion = false;
    }

    /// -----------------------------
    /// ANIMATION EVENTS
    /// -----------------------------

    public void AE_OpenComboWindow()
    {
        _isComboWindowOpen = true;
    }

    public void AE_CloseComboWindow()
    {
        _isComboWindowOpen = false;
    }

    public void AE_EndAttack()
    {
        _canMove = true;
        PlayerEvents.RaiseToggleCanMove(true);
    }

    public void AE_EndCombo()
    {
        ResetCombo();
    }
}