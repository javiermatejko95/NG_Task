using System;
using System.Collections.Generic;

/// <summary>
/// Plain serializable snapshot of the inventory state.
/// Only contains data — no Unity objects, no references.
///
/// JsonUtility requires [Serializable] on the class and all nested types.
/// </summary>
[Serializable]
public class InventorySaveData
{
    /// <summary>Ordered list of slots. Empty slots are stored as entries with an empty itemName.</summary>
    public List<SlotSaveData> slots = new();
}

/// <summary>
/// Serializable snapshot of one inventory slot.
/// We save the item's name (which matches ItemData's asset name) so we can
/// look it up in the ItemDatabase at load time.
/// </summary>
[Serializable]
public class SlotSaveData
{
    /// <summary>
    /// The name of the ItemData ScriptableObject asset.
    /// Empty string means the slot is empty.
    /// </summary>
    public string itemName;

    public bool IsEmpty => string.IsNullOrEmpty(itemName);

    public SlotSaveData() { }

    public SlotSaveData(string itemName)
    {
        this.itemName = itemName;
    }
}
