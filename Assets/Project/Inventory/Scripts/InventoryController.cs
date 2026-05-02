using System;
using System.Linq;
using UnityEngine;

//Obs: inventorry controller should be the one controlling the whole management of the inventory.Right now it's delegating the responsibility to the UI,
//which is not ideal. 
//The UI should only be responsible for displaying the inventory, not managing it.
public class InventoryController : MonoBehaviour
{
    [SerializeField] private int _maxSlots = 18;
    [SerializeField] private InventoryUI _inventoryUI;

    private void Awake()
    {
        InventoryEvents.OnItemAdded += AddItem;
        InventoryEvents.OnItemRemoved += RemoveItem;
        InventoryEvents.OnItemUsed += UseItem;
        InventoryEvents.OnItemEquipped += EquipItem;
        InventoryEvents.OnItemUnequipped += UnequipItem;
        InventoryEvents.OnItemSwapped += SwapItems;

        _inventoryUI.Initialize(_maxSlots);
    }

    private void AddItem(ItemData itemData, Action onSuccess = null, Action onFailure = null)
    {
        bool isFull = !_inventoryUI.GetAllSlots().Any(slot => !slot.HasItem);

        if (isFull)
        {
            //TODO: Show message to player that inventory is full
            onFailure?.Invoke();
            return;
        }

        _inventoryUI.AddItem(itemData);
        onSuccess?.Invoke();
    }

    private void RemoveItem(InventoryItem item)
    {
        _inventoryUI.RemoveItem(item);
    }

    private void UseItem(InventoryItem item)
    {
        //TODO: Implement item usage logic here
        _inventoryUI.RemoveItem(item);
    }

    private void EquipItem(InventoryItem item)
    {

    }

    private void UnequipItem(InventoryItem item)
    {

    }

    private void SwapItems(InventoryItem item, InventoryItem item2)
    {
        ItemData temp = item.GetItem();

        item.SetItem(item2.GetItem());
        item2.SetItem(temp);
    }    
}
