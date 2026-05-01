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

    private void AddItem(ItemData itemData)
    {
        //if(inventoryItems.Count >= _maxSlots)
        //{
        //    //TODO: Show message to player that inventory is full
        //    return;
        //}

        _inventoryUI.AddItem(itemData);
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
