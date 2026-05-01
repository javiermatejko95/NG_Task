using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
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