using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Item Database",
    menuName = "Survival Game/Items/Item Database"
)]
public class ItemDatabase : ScriptableObject
{
    [SerializeField]
    private ItemData[] items;

    private Dictionary<string, ItemData> itemsById;

    public IReadOnlyList<ItemData> Items => items;

    private void OnEnable()
    {
        BuildLookup();
    }

    private void BuildLookup()
    {
        itemsById = new Dictionary<string, ItemData>();

        if (items == null)
        {
            return;
        }

        foreach (ItemData item in items)
        {
            if (item == null)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(item.itemId))
            {
                Debug.LogWarning(
                    $"Item '{item.name}' has an empty itemId."
                );

                continue;
            }

            if (itemsById.ContainsKey(item.itemId))
            {
                Debug.LogError(
                    $"Duplicate itemId found: {item.itemId}"
                );

                continue;
            }

            itemsById.Add(item.itemId, item);
        }
    }

    public ItemData GetItem(string itemId)
    {
        if (itemsById == null)
        {
            BuildLookup();
        }

        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        itemsById.TryGetValue(itemId, out ItemData item);

        return item;
    }
}