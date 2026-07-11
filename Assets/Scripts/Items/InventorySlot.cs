using System;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    [SerializeField]
    private ItemData item;

    [SerializeField]
    private int quantity;

    public ItemData Item => item;
    public int Quantity => quantity;
    public bool IsEmpty => item == null || quantity <= 0;

    public InventorySlot()
    {
        Clear();
    }

    public bool CanStack(ItemData otherItem)
    {
        return !IsEmpty &&
               item == otherItem &&
               quantity < item.maxStackSize;
    }

    public int Add(ItemData newItem, int amount)
    {
        if (newItem == null || amount <= 0)
        {
            return amount;
        }

        if (IsEmpty)
        {
            item = newItem;
        }
        else if (item != newItem)
        {
            return amount;
        }

        int availableSpace =
            item.maxStackSize - quantity;

        int amountToAdd =
            Mathf.Min(amount, availableSpace);

        quantity += amountToAdd;

        return amount - amountToAdd;
    }

    public bool Remove(int amount)
    {
        if (IsEmpty ||
            amount <= 0 ||
            quantity < amount)
        {
            return false;
        }

        quantity -= amount;

        if (quantity <= 0)
        {
            Clear();
        }

        return true;
    }

    public void Clear()
    {
        item = null;
        quantity = 0;
    }
}