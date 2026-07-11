using UnityEngine;

[CreateAssetMenu(
    fileName = "Item Registry",
    menuName = "Survival Game/Items/Item Registry"
)]
public class ItemRegistry : ScriptableObject
{
    [Header("Resources")]
    public ItemData wood;
    public ItemData stone;
    public ItemData food;
    public ItemData ironOre;
    public ItemData goldOre;

    public ItemData GetItem(ResourceKind kind)
    {
        switch (kind)
        {
            case ResourceKind.Wood:
                return wood;

            case ResourceKind.Stone:
                return stone;

            case ResourceKind.Food:
                return food;

            case ResourceKind.IronOre:
                return ironOre;

            case ResourceKind.GoldOre:
                return goldOre;

            default:
                Debug.LogError(
                    $"ItemRegistry: No ItemData mapping for {kind}."
                );

                return null;
        }
    }
}