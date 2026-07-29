using UnityEngine;

[CreateAssetMenu(
    fileName = "New Item",
    menuName = "Survival Game/Items/Item"
)]
public class ItemData : ScriptableObject
{
    [Header("Equipment")]
    public GameObject heldPrefab;
    public Vector3 heldPosition;
    public Vector3 heldRotation;
    public Vector3 heldScale = Vector3.one;

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

    [Header("Values")]
    [Min(0)]
    public int baseValue;
    

    [Min(0)]
    public int gatherDamage = 1;

    [Min(0)]
    public int attackDamage = 1;

    [Header("Tool")]
    public ToolType toolType = ToolType.None;
    [Min(0)]
    public int treeDamage = 1;
    [Min(0)]
    public int rockDamage = 1;
    [Min(0)]
    public int oreDamage = 1;

    [Header("Building Placement")]
    public bool isPlaceable;

    public GameObject placeablePrefab;
}