using System.Collections.Generic;
using Unity.VisualScripting;
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
