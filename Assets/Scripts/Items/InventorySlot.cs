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

    public bool IsEmpty =>
        item == null || quantity <= 0;

    public InventorySlot()
    {
        Clear();
    }

    // =========================================================
    // STACK
    // =========================================================

    public bool CanStack(ItemData otherItem)
    {
        return !IsEmpty &&
               item == otherItem &&
               quantity < item.maxStackSize;
    }

    // =========================================================
    // ADD
    // =========================================================

    public int Add(
        ItemData newItem,
        int amount
    )
    {
        if (newItem == null ||
            amount <= 0)
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
            Mathf.Min(
                amount,
                availableSpace
            );

        quantity += amountToAdd;

        return amount - amountToAdd;
    }

    // =========================================================
    // REMOVE
    // =========================================================

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

    // =========================================================
    // SET
    // Used by Save System
    // =========================================================

    public void Set(
        ItemData newItem,
        int newQuantity
    )
    {
        if (newItem == null ||
            newQuantity <= 0)
        {
            Clear();
            return;
        }

        item = newItem;

        quantity =
            Mathf.Clamp(
                newQuantity,
                1,
                item.maxStackSize
            );
    }

    // =========================================================
    // CLEAR
    // =========================================================

    public void Clear()
    {
        item = null;
        quantity = 0;
    }
}