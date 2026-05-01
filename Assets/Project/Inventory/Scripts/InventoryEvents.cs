using System;

public static class InventoryEvents
{
    public static event Action<ItemData> OnItemAdded;
    public static event Action<InventoryItem> OnItemRemoved;
    public static event Action<InventoryItem> OnItemUsed;
    public static event Action<InventoryItem> OnItemEquipped;
    public static event Action<InventoryItem> OnItemUnequipped;
    public static event Action<InventoryItem, InventoryItem> OnItemSwapped;

    public static void RaiseOnItemAdded(ItemData item)
    {
        OnItemAdded?.Invoke(item);
    }

    public static void RaiseOnItemRemoved(InventoryItem item)
    {
        OnItemRemoved?.Invoke(item);
    }

    public static void RaiseOnItemUsed(InventoryItem item)
    {
        OnItemUsed?.Invoke(item);
    }

    public static void RaiseOnItemEquipped(InventoryItem item)
    {
        OnItemEquipped?.Invoke(item);
    }

    public static void RaiseOnItemUnequipped(InventoryItem item)
    {
        OnItemUnequipped?.Invoke(item);
    }

    public static void RaiseOnItemSwapped(InventoryItem item1, InventoryItem item2)
    {
        OnItemSwapped?.Invoke(item1, item2);
    }
}
