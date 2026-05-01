using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    private readonly List<IInteractable> _inRange = new();

    public void TryInteract()
    {
        var target = GetBestTarget();

        if (target != null)
        {
            target.Interact(this);
        }
    }

    public void RemoveFromInRangeList(IInteractable interactable)
    {
        if (_inRange.Contains(interactable))
        {
            _inRange.Remove(interactable);
        }
    }

    private IInteractable GetBestTarget()
    {
        _inRange.RemoveAll(i => i == null);

        float bestDistance = float.MaxValue;
        IInteractable best = null;

        foreach (var interactable in _inRange)
        {
            if (interactable == null) continue;

            MonoBehaviour mb = interactable as MonoBehaviour;
            float dist = Vector3.Distance(transform.position, mb.transform.position);

            if (dist < bestDistance)
            {
                bestDistance = dist;
                best = interactable;
            }
        }

        return best;
    }

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<IInteractable>();

        if (interactable != null && !_inRange.Contains(interactable))
        {
            _inRange.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var interactable = other.GetComponent<IInteractable>();

        if (interactable != null)
        {
            _inRange.Remove(interactable);
        }
    }
}
