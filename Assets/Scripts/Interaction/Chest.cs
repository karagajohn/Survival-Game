using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Chest : MonoBehaviour, IInteractable
{
    [Header("Loot")]
    [SerializeField]
    private List<LootEntry> lootTable =
        new List<LootEntry>();

    [SerializeField]
    [Min(1)]
    private int minimumLootRolls = 2;

    [SerializeField]
    [Min(1)]
    private int maximumLootRolls = 4;

    [SerializeField]
    [Min(1)]
    private int minimumItemAmount = 1;

    [SerializeField]
    [Min(1)]
    private int maximumItemAmount = 3;

    private bool opened;

    public void Interact()
    {
        if (opened)
        {
            return;
        }

        if (PlayerInventory.Instance == null)
        {
            Debug.LogError(
                "Chest: PlayerInventory instance was not found."
            );

            return;
        }

        opened = true;

        Debug.Log("Chest opened!");

        GenerateLoot();
    }

    private void GenerateLoot()
    {
        if (lootTable == null ||
            lootTable.Count == 0)
        {
            Debug.LogWarning(
                "Chest: Loot Table is empty."
            );

            return;
        }

        int minimumRolls =
            Mathf.Max(1, minimumLootRolls);

        int maximumRolls =
            Mathf.Max(
                minimumRolls,
                maximumLootRolls
            );

        int rollCount =
            Random.Range(
                minimumRolls,
                maximumRolls + 1
            );

        for (int i = 0; i < rollCount; i++)
        {
            ItemData selectedItem =
                SelectRandomLoot();

            if (selectedItem == null)
            {
                continue;
            }

            int minimumAmount =
                Mathf.Max(
                    1,
                    minimumItemAmount
                );

            int maximumAmount =
                Mathf.Max(
                    minimumAmount,
                    maximumItemAmount
                );

            int amount =
                Random.Range(
                    minimumAmount,
                    maximumAmount + 1
                );

            bool added =
                PlayerInventory.Instance.Add(
                    selectedItem,
                    amount
                );

            if (added)
            {
                Debug.Log(
                    $"Chest gave {amount}x " +
                    $"{selectedItem.displayName}."
                );
            }
            else
            {
                Debug.LogWarning(
                    $"Could not add {amount}x " +
                    $"{selectedItem.displayName} " +
                    "because the inventory is full."
                );
            }
        }
    }

    private ItemData SelectRandomLoot()
    {
        float totalChance = 0f;

        foreach (LootEntry entry in lootTable)
        {
            if (entry == null ||
                entry.item == null ||
                entry.chance <= 0f)
            {
                continue;
            }

            totalChance += entry.chance;
        }

        if (totalChance <= 0f)
        {
            return null;
        }

        float randomValue =
            Random.Range(0f, totalChance);

        float currentChance = 0f;

        foreach (LootEntry entry in lootTable)
        {
            if (entry == null ||
                entry.item == null ||
                entry.chance <= 0f)
            {
                continue;
            }

            currentChance += entry.chance;

            if (randomValue <= currentChance)
            {
                return entry.item;
            }
        }

        return null;
    }
}