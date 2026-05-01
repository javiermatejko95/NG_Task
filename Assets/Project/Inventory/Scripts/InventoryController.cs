using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private int _maxSlots = 18;
    [SerializeField] private InventoryUI _inventoryUI;    

    private List<InventoryItem> inventoryItems = new();

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

    private void AddItem(InventoryItem item)
    {

    }

    private void RemoveItem(InventoryItem item)
    {

    }

    private void UseItem(InventoryItem item)
    {

    }

    private void EquipItem(InventoryItem item)
    {

    }

    private void UnequipItem(InventoryItem item)
    {

    }

    private void SwapItems(InventoryItem item, InventoryItem item2)
    {

    }    
}
