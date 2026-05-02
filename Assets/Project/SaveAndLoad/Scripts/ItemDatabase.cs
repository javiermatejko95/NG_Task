using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Registry of all ItemData assets in the project.
/// Required by the save system to reconstruct items from their saved names.
///
/// Setup:
///   1. Right Click in Project → Create → Inventory → Item Database
///   2. Drag all your ItemData assets into the Items list
///   3. Assign this asset to InventorySaveHandler in the Inspector
///
/// Extension: replace the List lookup with an Addressables load by key
/// for large games with many items.
/// </summary>
[CreateAssetMenu(menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemData> _items = new();

    private Dictionary<string, ItemData> _lookup;

    private void OnEnable()
    {
        BuildLookup();
    }

    public ItemData Get(string itemName)
    {
        if (_lookup == null) BuildLookup();

        _lookup.TryGetValue(itemName, out ItemData result);

        if (result == null)
            Debug.LogWarning($"[ItemDatabase] Item not found: '{itemName}'. Make sure it's added to the database.");

        return result;
    }

    public bool Contains(string itemName)
    {
        if (_lookup == null) BuildLookup();
        return _lookup.ContainsKey(itemName);
    }

    private void BuildLookup()
    {
        _lookup = new Dictionary<string, ItemData>(_items.Count);

        foreach (var item in _items)
        {
            if (item == null) continue;

            if (!_lookup.TryAdd(item.name, item))
                Debug.LogWarning($"[ItemDatabase] Duplicate item name detected: '{item.name}'. Only the first entry will be used.");
        }
    }
}
