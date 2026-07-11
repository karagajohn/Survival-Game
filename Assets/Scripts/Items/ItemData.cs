using UnityEngine;

[CreateAssetMenu(
    fileName = "New Item",
    menuName = "Survival Game/Items/Item"
)]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemId;

    public string displayName;

    [TextArea(2, 5)]
    public string description;

    [Header("Classification")]
    public ItemType itemType = ItemType.Resource;

    [Header("Inventory")]
    [Min(1)]
    public int maxStackSize = 99;

    public Sprite icon;

    [Header("World")]
    public GameObject worldPrefab;

    [Header("Equipment")]
    public GameObject heldPrefab;

    [Header("Values")]
    [Min(0)]
    public int baseValue;

    [Min(0)]
    public int gatherDamage = 1;

    [Min(0)]
    public int attackDamage = 1;
}