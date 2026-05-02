using UnityEngine;

/// <summary>
/// Script imported from another project simplified but it can be expanded to the project's needs.
/// </summary>
public class WeaponHolder : MonoBehaviour
{
    [Header("Weapon Setup")]
    [SerializeField] private Weapon _currentWeapon;
    public void AE_EnableTrail()
    {
        _currentWeapon.EnableTrail();
    }

    public void AE_DisableTrail()
    {
        _currentWeapon.DisableTrail();
    }
}
