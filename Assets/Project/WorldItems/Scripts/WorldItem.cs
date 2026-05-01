using UnityEngine;

public class WorldItem : MonoBehaviour, IInteractable
{
    //public ItemData data;

    public void Interact(Interactor interactor)
    {
        //var inventory = interactor.GetComponent<Inventory>();

        //if (inventory == null) return;

        //if (inventory.AddItem(data))
        //{
        //    Destroy(gameObject);
        //}

        Destroy(gameObject);
    }
}
