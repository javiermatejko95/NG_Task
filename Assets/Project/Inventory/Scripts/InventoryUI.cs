using UnityEngine;

public class InventoryUI : UIPanel
{
    [SerializeField] private Transform _inventoryItemsParent;
    [SerializeField] private InventoryItem _inventoryItemPrefab;
    [SerializeField] private PlayerInputHandler _playerInputHandler;

    public void Initialize(int maxSlots)
    {
        _playerInputHandler.OnToggleInventory += HandleOnToggleInventory;

        for (int i = 0; i < maxSlots; i++)
        {
            Instantiate(_inventoryItemPrefab, _inventoryItemsParent);
        }
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
