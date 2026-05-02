using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Connects the generic SaveSystem with the existing inventory system.
/// Implements ISaveable<InventorySaveData> to define how inventory state
/// is captured and restored.
///
/// Deliberately kept separate from InventoryController and InventoryUI so
/// that save logic never bleeds into UI or game logic.
///
/// </summary>
public class InventorySaveHandler : MonoBehaviour, ISaveable<InventorySaveData>
{
    // ──────────────────────────────────────────────
    // CONFIGURATION
    // ──────────────────────────────────────────────
    [Header("References")]
    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private ItemDatabase _itemDatabase;

    [Header("Settings")]
    [Tooltip("Key used as the save filename. Change only if you want a different file per save slot.")]
    [SerializeField] private string _saveKey = "inventory";

    [Tooltip("Automatically load on Start")]
    [SerializeField] private bool _autoLoadOnStart = true;

    [Tooltip("Automatically save when the application quits")]
    [SerializeField] private bool _autoSaveOnQuit = true;

    // ──────────────────────────────────────────────
    // ISaveable
    // ──────────────────────────────────────────────
    public string SaveKey => _saveKey;

    // ══════════════════════════════════════════════
    // LIFECYCLE
    // ══════════════════════════════════════════════

    private void Start()
    {
        if (_autoLoadOnStart)
            Load();
    }

    private void OnApplicationQuit()
    {
        if (_autoSaveOnQuit)
            Save();
    }

    // ══════════════════════════════════════════════
    // PUBLIC API
    // ══════════════════════════════════════════════

    /// <summary>Saves the current inventory state to disk.</summary>
    public void Save()
    {
        InventorySaveData data = Capture();
        SaveSystem.Save(_saveKey, data);
    }

    /// <summary>
    /// Loads inventory state from disk and applies it.
    /// Safe to call even if no save file exists — will silently skip.
    /// </summary>
    public void Load()
    {
        if (!SaveSystem.TryLoad<InventorySaveData>(_saveKey, out InventorySaveData data))
            return;

        Restore(data);
    }

    /// <summary>Deletes the inventory save file.</summary>
    public void DeleteSave()
    {
        SaveSystem.Delete(_saveKey);
        Debug.Log("[InventorySaveHandler] Save deleted.");
    }

    public bool HasSave() => SaveSystem.Exists(_saveKey);

    // ══════════════════════════════════════════════
    // ISaveable IMPLEMENTATION
    // ══════════════════════════════════════════════

    /// <summary>
    /// Walks all InventoryItem slots in the UI and serializes their current ItemData
    /// (or an empty entry for empty slots) into an InventorySaveData snapshot.
    /// </summary>
    public InventorySaveData Capture()
    {
        List<InventoryItem> slots = _inventoryUI.GetAllSlots();

        var data = new InventorySaveData();

        foreach (InventoryItem slot in slots)
        {
            string itemName = slot.HasItem ? slot.GetItem().name : string.Empty;
            data.slots.Add(new SlotSaveData(itemName));
        }

        return data;
    }

    /// <summary>
    /// Reads a saved snapshot and pushes the corresponding ItemData assets
    /// back into the UI slots in order.
    /// Slots with no matching save entry are cleared.
    /// </summary>
    public void Restore(InventorySaveData data)
    {
        if (data == null || data.slots == null)
        {
            Debug.LogWarning("[InventorySaveHandler] Restore called with null data.");
            return;
        }

        List<InventoryItem> slots = _inventoryUI.GetAllSlots();

        for (int i = 0; i < slots.Count; i++)
        {
            // No save data for this slot index → clear it
            if (i >= data.slots.Count || data.slots[i].IsEmpty)
            {
                slots[i].Clear();
                continue;
            }

            ItemData item = _itemDatabase.Get(data.slots[i].itemName);

            if (item != null)
                slots[i].SetItem(item);
            else
                slots[i].Clear(); // item was removed from the database — graceful fallback
        }

        Debug.Log($"[InventorySaveHandler] Restored {data.slots.Count} slots.");
    }
}
