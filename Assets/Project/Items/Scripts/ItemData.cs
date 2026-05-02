using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    // TODO: Add more properties to the item data, such as stats, effects, etc.
    // Obs: Add an ID field to handle the item in a more robust way, instead of relying on the name. This will help avoid issues with duplicate 
    // names and make it easier to reference items in code.
    [SerializeField] private string _itemName;
    [SerializeField] private string _tooltipDescription;
    [SerializeField] private ItemType _itemType;
    [SerializeField] private Sprite _icon;

    public string ItemName => _itemName;
    public string TooltipDescription => _tooltipDescription;
    public ItemType ItemType => _itemType;
    public Sprite Icon => _icon;
}

public enum ItemType
{
    Consumable,
    Weapon,
    Armor
}