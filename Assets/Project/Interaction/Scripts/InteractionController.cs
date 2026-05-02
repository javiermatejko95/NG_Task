using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private Interactor _interactor;
    private PlayerInputHandler _input;

    private void Awake()
    {
        _input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (_input.InteractPressed)
        {
            _interactor.TryInteract();
        }
    }
}
