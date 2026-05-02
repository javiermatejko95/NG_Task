using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Script imported from another project simplified but it can be expanded to the project's needs.
/// </summary>
public class Weapon : MonoBehaviour
{
    [Header("Visual & Sound Feedback")]
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private AudioClip[] _swingSounds;

    [Header("References")]
    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

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

    public void PlaySwingSound(int index)
    {
        if (_swingSounds != null && _swingSounds.Length > 0 && _audioSource != null)
        {
            _audioSource.PlayOneShot(_swingSounds[index]);
        }
    }
}
