using UnityEngine;

/// <summary>
/// Script imported from another project simplified but it can be expanded to the project's needs.
/// </summary>
public class Weapon : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private TrailRenderer _trailRenderer;

    public void EnableTrail()
    {
        if (_trailRenderer != null)
        {
            if (!_trailRenderer.enabled)
            {
                _trailRenderer.enabled = true;
                _trailRenderer.Clear();
            }
        }
    }

    public void DisableTrail()
    {
        if (_trailRenderer != null)
        {
            _trailRenderer.enabled = false;
        }
    }
}
