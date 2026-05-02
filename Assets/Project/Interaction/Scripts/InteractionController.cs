using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private Interactor _interactor;
    private PlayerInputHandler _input;

    private void Awake()
    {
        _input = GetComponent<PlayerInputHandler>();
        _input.OnInteract += HandleOnInteract;
    }

    private void HandleOnInteract()
    {
        _interactor.TryInteract();
    }
}
