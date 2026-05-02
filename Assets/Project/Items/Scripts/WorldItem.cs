using UnityEngine;

public class WorldItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData _data;

    public void Interact(Interactor interactor)
    {
        InventoryEvents.RaiseOnItemAdded(_data, () => HandleOnSuccess(interactor));
    }

    private void HandleOnSuccess(Interactor interactor)
    {
        interactor.RemoveFromInRangeList(this);

        Destroy(gameObject);
    }
}
