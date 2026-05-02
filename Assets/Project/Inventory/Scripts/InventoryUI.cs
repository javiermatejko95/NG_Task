using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : UIPanel
{
    [SerializeField] private Transform _inventoryItemsParent;
    [SerializeField] private InventoryItem _inventoryItemPrefab;
    [SerializeField] private PlayerInputHandler _playerInputHandler;

    private List<InventoryItem> _inventoryItems = new();

    public void Initialize(int maxSlots)
    {
        _playerInputHandler.OnToggleInventory += HandleOnToggleInventory;

        for (int i = 0; i < maxSlots; i++)
        {
            InventoryItem inventoryItem = Instantiate(_inventoryItemPrefab, _inventoryItemsParent);
            _inventoryItems.Add(inventoryItem);
        }
    }

    public void AddItem(ItemData itemData)
    {
        foreach (InventoryItem inventoryItem in _inventoryItems)
        {
            if (!inventoryItem.HasItem)
            {
                inventoryItem.SetItem(itemData);
                return;
            }
        }
    }

    public void RemoveItem(InventoryItem item)
    {
        if (_inventoryItems.Contains(item))
        {
            item.Clear();
        }
    }

    /// <summary>
    /// Returns the ordered list of all slots.
    /// Used by InventorySaveHandler to capture and restore state
    /// without coupling the save system to internal inventory logic.
    /// </summary>
    public List<InventoryItem> GetAllSlots()
    {
        return _inventoryItems;
    }

    protected override void OnCloseButtonClicked()
    {
        base.OnCloseButtonClicked();

        _playerInputHandler.CloseInventory();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void HandleOnToggleInventory(bool toggle)
    {
        if (toggle)
        {
            Show();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        Hide();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
